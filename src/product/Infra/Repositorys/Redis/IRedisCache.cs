namespace ProductApi.Infra.Repositorys.Redis
{
    public interface IRedisCache<T>
    {
        Task<List<T>> GetCache(string key);
        Task SaveCache(string key, List<T> list);
        Task ClearCache(string key);
    }
}
