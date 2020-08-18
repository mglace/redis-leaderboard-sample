using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace RedisLeaderboardSample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IWebHost host = CreateHostBuilder(args).Build();

            await SeedData();

            await host.RunAsync();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();

        public static async Task SeedData()
        {
            var options = ConfigurationOptions.Parse(RedisSettings.ConnectionString);
            
            options.AllowAdmin = true;
            
            var connection = ConnectionMultiplexer.Connect(options);

            var leaderboardService = new LeaderboardService(connection);

            await connection.GetServer(RedisSettings.Server, RedisSettings.Port).FlushDatabaseAsync();

            await Task.WhenAll(
                leaderboardService.CreateTopic(1, "Topic #1", DateTime.UtcNow.AddHours(-24)),
                leaderboardService.CreateTopic(2, "Topic #2", DateTime.UtcNow.AddHours(-1)),
                leaderboardService.CreateTopic(3, "Topic #3", DateTime.UtcNow.AddHours(-12))
            );
        }
    }
}
