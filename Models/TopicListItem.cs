using System;
using System.Linq;
using StackExchange.Redis;

namespace RedisLeaderboardSample
{
    public class TopicListItem
    {
        public int Id { get; set; }

        public string Subject { get; set; }

        public int ReplyCount { get; set; }

        public DateTime LastReply { get; set; }

        public HashEntry[] ToHash()
        {
            return new HashEntry[] 
            {
                new HashEntry("id", Id),
                new HashEntry("subject", Subject),
                new HashEntry("lastReply", LastReply.ToString("o")),
                new HashEntry("replyCount", ReplyCount)
            };
        }

        public static TopicListItem FromHash(HashEntry[] entries)
        {
            var dict = entries.ToDictionary(x => x.Name, x => x.Value);

            return new TopicListItem 
            {
                Id = (int)dict["id"],
                Subject = (string)dict["subject"],
                LastReply = DateTime.Parse((string)dict["lastReply"]),
                ReplyCount = (int)dict["replyCount"]
            };
        }
    }
}