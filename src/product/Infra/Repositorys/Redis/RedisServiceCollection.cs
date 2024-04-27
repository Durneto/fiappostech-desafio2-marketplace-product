using ProductApi.Infra.Repositorys.Base;
using StackExchange.Redis;
using System.Data;
using System.Data.SqlClient;

namespace ProductApi.Infra.Repositorys.Redis
{
    public static class RedisServiceCollection
    {
        public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var stringConexao = configuration.GetValue<string>("ConnectionStringSQL");
            services.AddScoped<IDbConnection>((conexao) => new SqlConnection(stringConexao));
            services.AddScoped<IUow, Uow>();

            string redisHost = "localhost";
            string redisPort = "6379";
            string connectionString = $"{redisHost}:{redisPort}";
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectionString));

            services.AddSingleton(typeof(IRedisCache<>), typeof(RedisCache<>));
        }
    }
}
