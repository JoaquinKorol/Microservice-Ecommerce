using Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.DTOs;
using Payment.Models;
using Payment.Services;

namespace Payment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestDTO request)
        {
            var paymentUrl = await _paymentService.CreatePaymentAsync(request.Amount, request.Description, request.OrderId);
            return Ok(new { Url = paymentUrl }); 
        }

        [HttpGet]
        public async Task<IEnumerable<Payments>> GetPayments()
        {
            return await _paymentService.GetPaymentsAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Payments>> GetPaymentById(int id)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByIdAsync(id);
                return Ok(payment);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
