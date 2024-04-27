using StackExchange.Redis;

namespace ProductApi.Infra.Repositorys.Redis
{
    public static class RedisServiceCollection
    {
        public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            string redisHost = configuration["RedisHost"];
            string redisPort = configuration["RedisPort"];
            string connectionString = $"{redisHost}:{redisPort}";
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectionString));

            services.AddSingleton(typeof(IRedisCache<>), typeof(RedisCache<>));
        }
    }
}
