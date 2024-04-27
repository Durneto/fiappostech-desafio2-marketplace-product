using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Domains.Dtos.Product;
using ProductApi.Domains.Interfaces;
using ProductApi.Domains.Models;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService productService)
        {
            _service = productService;
        }

        /// <summary>
        /// Recurso responsável por obter todos os produtos
        /// </summary>
        /// <returns>Lista de produtos encontrados</returns>
        [HttpGet]
        [Authorize(Roles = $"{nameof(TypeAuthorization.Admin)},{nameof(TypeAuthorization.Operacao)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        /// <summary>
        /// Recurso responsável por obter um produtos do cache
        /// </summary>
        /// <returns>Produtos encontrado</returns>
        [HttpGet("GetCache")]
        [Authorize(Roles = $"{nameof(TypeAuthorization.Admin)},{nameof(TypeAuthorization.Operacao)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCache(string id)
        {
            var dto = await _service.GetProductCacheAsync(id);
            return Ok(dto);
        }

        /// <summary>
        /// Recurso responsável por criar um produto
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        /// {
        ///     "Name": "Produto 1",
        ///     "Value": 999999.0
        /// }
        /// 
        /// </remarks>
        /// <param name="dto">Json com os dados do produto</param>
        /// <returns>Código do status da execução</returns>
        [HttpPost]
        [Authorize(Roles = nameof(TypeAuthorization.Admin))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(ProductCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                var result = await _service.CreateAsync(dto);

                if(result.HasError)
                    return BadRequest(result);

                return CreatedAtAction(nameof(GetAll), new { id = result.Data.Id }, result.Data);
            }

            return BadRequest();
        }

        /// <summary>
        /// Recurso responsável por excluir um produto
        /// </summary>
        /// <param name="id">Código identificador do produto</param>
        /// <returns>Código do status da execução</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(TypeAuthorization.Admin))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _service.DeleteAsync(id);

            if (result.HasError)
                return BadRequest(result);

            return NoContent();
        }
    }
}
