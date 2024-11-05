using Microsoft.AspNetCore.Mvc;

namespace ProductCatalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(new[] { "Product 1", "Product 2", "Product 3" });
        }
    }
}