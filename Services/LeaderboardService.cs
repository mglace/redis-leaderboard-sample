using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisLeaderboardSample
{
    public class LeaderboardService
    {
        private static string Key = "leaderboard";

        private readonly ConnectionMultiplexer _connection;

        private StackExchange.Redis.IDatabase Database => _connection.GetDatabase();

        public LeaderboardService(ConnectionMultiplexer connection)
        {
            _connection = connection;
        }

        public async Task<TopicListItem> GetTopic(int topicId)
        {
            var entries = await Database.HashGetAllAsync($"topic:{topicId}");

            return TopicListItem.FromHash(entries);
        }

        public async Task<IEnumerable<TopicListItem>> GetTopics(int page, int pageSize)
        {
            var ids = await Database.SortedSetRangeByRankAsync(Key, 0, 30, Order.Descending);

            var topics = new ConcurrentBag<TopicListItem>();

            var tasks = ids
                .Select(async id => {

                    var topicId = int.Parse(id);

                    var topic = await GetTopic(int.Parse(id));

                    topics.Add(topic);

                });

            await Task.WhenAll(tasks);

            return topics.OrderByDescending(t => t.LastReply).ToArray();
        }

        public Task CreateTopic(int id, string subject, DateTime postTime)
        {
            var timestamp = postTime.Ticks;

            var memberKey = $"topic:{id}";

            var topic = new TopicListItem
            {
                Id = id,
                Subject = subject,
                LastReply = postTime,
                ReplyCount = 1
            };

            var tran = Database.CreateTransaction();

            tran.SortedSetAddAsync(Key, id, timestamp);
            tran.HashSetAsync(memberKey, topic.ToHash());

            return tran.ExecuteAsync();
        }

        public Task CreateReply(int topicId)
        {
            var now = DateTime.UtcNow;
            var timestamp = now.Ticks;

            var memberKey = $"topic:{topicId}";

            var tran = Database.CreateTransaction();

            tran.SortedSetAddAsync(Key, topicId, timestamp);
            tran.HashSetAsync(memberKey, "lastReply", now.ToString("o"));
            tran.HashIncrementAsync(memberKey, "replyCount");

            return tran.ExecuteAsync();
        }
    }
}