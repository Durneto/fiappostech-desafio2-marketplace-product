using ProductApi.Domains.Models;

namespace ProductApi.Domains.Interfaces
{
    public interface IProductRepository
    {
        Task CreateAsync(Product product);
        Task DeleteAsync(string id);
        Task<List<Product>> GetAllAsync();
        Task<Product> GetAsync(string id);
    }
}
