using Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProductCatalog.DTOs;
using ProductCatalog.Models;
using ProductCatalog.Services;

namespace ProductCatalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _productService.GetProductsAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost("create")]
        public async Task<Product> CreateProduct([FromBody] Product product)
        {
            await _productService.CreateProductAsync(product);
            return product;
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return Ok("Product deleted successfully.");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult<Product>> Update(int id, [FromBody] UpdateProductDTO updateDTO)
        {
            try
            {
                var updatedProduct = await _productService.UpdateProductAsync(id, updateDTO);
                return Ok(updatedProduct);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });

            }
        }
    }
}