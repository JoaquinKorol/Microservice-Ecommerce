using Core.Exceptions;
using Core.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Order.DTOs;
using Order.Models;
using Order.Validators;

namespace Order.Services
{
    public class OrderService
    {
        private readonly IRepository<Orders> _respoitory;
        private readonly IRepository<OrderItem> _orderItemRepository;
        public OrderService(IRepository<Orders> repository, IRepository<OrderItem> orderItemRepository)
        {
            _respoitory = repository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersAsync()
        {
            var orders = await _respoitory.GetAllAsync();
            return orders.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                ShippingAddress = o.ShippingAddress,
                TotalAmount = o.TotalAmount,
                StatusId = o.StatusId,
                OrderItems = o.OrderItems.Select(i => new OrderItemDTO
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            });
        }

        public async Task<Orders> GetOrderByIdAsync(int id)
        {
            var order = await _respoitory.GetByIdAsync(id);
            if (order == null)
            {
                throw new NotFoundException($"Order with ID {id} not found.");
            }
            return order;
        }

        public async Task<IEnumerable<Orders>> GetOrdersByUserIdAsync(int userId)
        {
            var allOrders = await _respoitory.GetAllAsync();  
            var userOrders = allOrders.Where(order => order.UserId == userId);  

            if (!userOrders.Any())  
            {
                throw new NotFoundException($"No orders found for UserID {userId}.");
            }

            return userOrders;
        }

        public async Task<Orders> CreateOrderAsync(CreateOrderDTO createOrderDTO)
        {
            var validator = new CreateOrderDTOValidator();
            var validationResult = await validator.ValidateAsync(createOrderDTO);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.FirstOrDefault()?.ErrorMessage;
                throw new ValidationException(errors);
            }

            decimal totalAmount = createOrderDTO.OrderItems.Sum(item => item.Quantity * item.Price);

            var newOrder = new Orders
            {
                UserId = createOrderDTO.UserId,
                OrderDate = DateTime.UtcNow,
                ShippingAddress = createOrderDTO.ShippingAddress,
                TotalAmount = totalAmount,
                StatusId = 1,
            };

            await _respoitory.AddAsync(newOrder);

            foreach (var item in createOrderDTO.OrderItems)
            {

                var orderItem = new OrderItem
                { 
                    OrderId = newOrder.OrderId,  
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };

                
                await _orderItemRepository.AddAsync(orderItem);
            }
            return newOrder;
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _respoitory.GetByIdAsync(id);
            if (order == null)
            {
                throw new NotFoundException($"Order with ID {id} not found.");
            }
            await _respoitory.DeleteAsync(id);
        }

        public async Task<Orders> UpdateOrderAsync(int id, UpdateOrderDTO updateOrderDTO)
        {
            var existingOrder = await _respoitory.GetByIdAsync(id);
            if (existingOrder == null)
            {
                throw new NotFoundException($"Order with ID {id} not found.");
            }

            existingOrder.ShippingAddress = updateOrderDTO.ShippingAddress ?? existingOrder.ShippingAddress;
            existingOrder.TotalAmount = updateOrderDTO.OrderItems.Sum(item => item.Quantity * item.Price);
            existingOrder.StatusId = updateOrderDTO.Status;

            foreach (var item in updateOrderDTO.OrderItems)
            {
                var existingItem = existingOrder.OrderItems.FirstOrDefault(x => x.ProductId == item.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity = item.Quantity;
                    existingItem.Price = item.Price;
                }
                else
                {
                    var newItem = new OrderItem
                    {
                        OrderId = existingOrder.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price
                    };
                    await _orderItemRepository.AddAsync(newItem);
                }
            }

            await _respoitory.UpdateAsync(existingOrder);
            return existingOrder;
        }


    }
}
