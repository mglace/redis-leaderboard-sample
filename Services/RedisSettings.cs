namespace RedisLeaderboardSample 
{
    public static class RedisSettings
    {
        public const string Server = "[SERVER]";

        public const int Port = 0;

        public const string Password = "[PASSWORD]";

        public static readonly string ConnectionString = $"{Server}:{Port},password={Password}";
    }
}