using ProductApi.Domains.Base;
using ProductApi.Domains.Dtos.Product;

namespace ProductApi.Domains.Interfaces
{
    public interface IProductService
    {
        Task<ApiResult<ProductDto>> CreateAsync(ProductCreateDto dto);
        Task<ApiResult> DeleteAsync(string id);
        Task<ApiResult<List<ProductDto>>> GetAllAsync();
        Task<ApiResult<ProductDto>> GetProductCacheAsync(string id);
    }
}
