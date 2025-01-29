using Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Order.DTOs;
using Order.Models;
using Order.Services;
using System.ComponentModel.DataAnnotations;

namespace Order.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService; 
        }

        [HttpGet]
        public async Task<IEnumerable<OrderDTO>> GetOrders()
        {
            return await _orderService.GetOrdersAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Orders>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                return Ok(order);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Orders>>> GetOrdersByUserId(int userId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByUserIdAsync(userId);
                return Ok(orders);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<Orders>> CreateOrder([FromBody] CreateOrderDTO createOrderDTO)
        {
            try
            {
                var createdOrder = await _orderService.CreateOrderAsync(createOrderDTO);
                return CreatedAtAction(nameof(CreateOrder), new { id = createdOrder.OrderId }, createdOrder);
            }
            catch(ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch(InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }

        }
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<Orders>> DeleteOrder(int id)
        {
            try
            {
                await _orderService.DeleteOrderAsync(id);
                return Ok("Order deleted successfully.");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPut("update/{id}")]
        public async Task<ActionResult<Orders>> UpdateOrder(int id, [FromBody] UpdateOrderDTO order)
        {
            try
            {
                var updatedOrder = await _orderService.UpdateOrderAsync(id, order);
                return Ok(updatedOrder);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
    }
}
