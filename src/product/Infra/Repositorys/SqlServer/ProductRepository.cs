using Dapper;
using ProductApi.Domains.Interfaces;
using ProductApi.Domains.Models;
using ProductApi.Infra.Repositorys.Base;

namespace ProductApi.Infra.Repositorys.SqlServer
{
    public class ProductRepository : IProductRepository
    {
        private readonly IUow _uow;

        public ProductRepository(IUow uow)
        {
            _uow = uow;
        }

        public async Task CreateAsync(Product product)
        {
            await _uow.Connection.ExecuteAsync("INSERT INTO TB_PRODUTO (Id, Nome, Preco, DataCriacao) VALUES (@Id, @Name, @Value, @CreateDate)", product, _uow.Transaction);
        }

        public async Task DeleteAsync(string id)
        {
            await _uow.Connection.ExecuteAsync("DELETE FROM TB_PRODUTO WHERE Id = @Id", new { Id = id }, _uow.Transaction);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return (await _uow.Connection.QueryAsync<Product>("SELECT Id, Nome AS Name, Preco AS Value, DataCriacao AS CreateDate FROM TB_PRODUTO", null, _uow.Transaction)).ToList();
        }

        public async Task<Product> GetAsync(string id)
        {
            return await _uow.Connection.QueryFirstOrDefaultAsync<Product>("SELECT * FROM TB_PRODUTO WHERE Id = @Id", new { Id = id }, _uow.Transaction);
        }
    }
}
