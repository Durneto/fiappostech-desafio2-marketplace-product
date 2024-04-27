using StackExchange.Redis;
using System.Text.Json;

namespace ProductApi.Infra.Repositorys.Redis
{
    public class RedisCache<T> : IRedisCache<T> where T : class
    {
        private readonly IDatabase _cache;

        public RedisCache(IConnectionMultiplexer redis)
        {
            _cache = redis.GetDatabase();
        }

        public async Task<List<T>> GetCache(string key)
        {
            var produtosCache = await _cache.StringGetAsync(key);

            if (produtosCache.IsNullOrEmpty)
            {
                return [];
            }
            else
            {
                return Deserialize<List<T>>(produtosCache);
            }
        }

        public async Task SaveCache(string key, List<T> list)
        {
            await _cache.StringSetAsync(key, Serialize(list), TimeSpan.FromMinutes(1));
        }

        public async Task ClearCache(string key)
        {
            await _cache.KeyDeleteAsync(key);
        }

        private byte[] Serialize<T>(T obj)
        {
            var json = JsonSerializer.Serialize(obj);
            return System.Text.Encoding.UTF8.GetBytes(json);
        }

        private T Deserialize<T>(byte[] bytes)
        {
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
