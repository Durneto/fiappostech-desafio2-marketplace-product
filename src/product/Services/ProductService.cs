using ProductApi.Domains.Base;
using ProductApi.Domains.Dtos.Product;
using ProductApi.Domains.Interfaces;
using ProductApi.Domains.Models;
using ProductApi.Infra.Repositorys.Base;
using ProductApi.Infra.Repositorys.Redis;
using System.Data;

namespace ProductApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IUow _uow;
        private readonly IRedisCache<Product> _redisCache;
        private readonly IProductRepository _repository;
        private readonly string _REDIS_CACHE_KEY = "PRODUCT_";

        public ProductService(IProductRepository repository, IUow uow, IRedisCache<Product> redisCache)
        {
            _uow = uow;
            _repository = repository;
            _redisCache = redisCache;
        }

        public async Task<ApiResult<ProductDto>> CreateAsync(ProductCreateDto dto)
        {
            var result = new ApiResult<ProductDto>();
            try
            {
                _uow.Open();
                _uow.BeginTransaction();

                var model = new Product(dto);

                if (model == null)
                    result.Erros.Add("O produto deve ser informado");

                if (model?.Name == string.Empty)
                    result.Erros.Add("O nome do produto deve ser informado");

                if (!result.HasError)
                {
                    model.Id = Guid.NewGuid().ToString();
                    model.CreateDate = DateTime.Now;

                    await _repository.CreateAsync(model);
                    _uow.Commit();

                    await _redisCache.SaveCache($"{_REDIS_CACHE_KEY}{model.Id}", [model]);

                    result.Data = model.ToDto();
                }
            }
            catch (Exception ex)
            {
                result.Erros.Add(ex.ToString());
                _uow.Rollback();
            }
            finally
            {
                _uow.Dispose();
            }

            return result;
        }

        public async Task<ApiResult> DeleteAsync(string id)
        {
            var result = new ApiResult();
            try
            {
                _uow.Open();
                _uow.BeginTransaction();

                var model = await _repository.GetAsync(id);

                if (model == null)
                    result.Erros.Add("O produto não foi localizado");

                if (!result.HasError)
                {
                    await _repository.DeleteAsync(id);
                    _uow.Commit();

                    await _redisCache.ClearCache($"{_REDIS_CACHE_KEY}{id}");
                }
            }
            catch (Exception ex)
            {
                result.Erros.Add(ex.ToString());
                _uow.Rollback();
            }
            finally
            {
                _uow.Dispose();
            }

            return result;
        }

        public async Task<ApiResult<List<ProductDto>>> GetAllAsync()
        {
            var result = new ApiResult<List<ProductDto>>();
            try
            {
                _uow.Open();

                var list = await _repository.GetAllAsync();
                var dtoList = list.Select(p => p.ToDto()).ToList();
                result.Data = dtoList;

                list.ToList().ForEach(p => _redisCache.SaveCache($"{_REDIS_CACHE_KEY}{p.Id}", [p]));
            }
            catch (Exception ex)
            {
                result.Erros.Add(ex.ToString());
            }
            finally
            {
                _uow.Dispose();
            }

            return result;
        }

        public async Task<ApiResult<ProductDto>> GetProductCacheAsync(string id)
        {
            var result = new ApiResult<ProductDto>();
            try
            {
                var list = await _redisCache.GetCache($"{_REDIS_CACHE_KEY}{id}");

                if (list.Any())
                    result.Data = list.First().ToDto();
            }
            catch (Exception ex)
            {
                result.Erros.Add(ex.ToString());
            }
            finally
            {
                _uow.Dispose();
            }

            return result;
        }
    }
}
