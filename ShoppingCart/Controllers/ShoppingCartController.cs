using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingCart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Este es el endpoint del carrito de compras.";
        }
    }
}
