using Core.Exceptions;
using Core.Interfaces;
using Moq;
using Order.DTOs;
using Order.Models;
using Order.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Test
{
    public class OrderServiceTests
    {
        private readonly Mock<IRepository<Orders>> _orderRepositoryMock;
        private readonly Mock<IRepository<OrderItem>> _orderItemRepositoryMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderRepositoryMock = new Mock<IRepository<Orders>>();
            _orderItemRepositoryMock = new Mock<IRepository<OrderItem>>();
            _orderService = new OrderService(_orderRepositoryMock.Object, _orderItemRepositoryMock.Object);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldThrowNotFoundException_WhenOrderDoesNotExist()
        {
            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Orders)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _orderService.GetOrderByIdAsync(1));
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldThrowValidationException_WhenInvalidDTO()
        {
            var invalidDto = new CreateOrderDTO { UserId = 0, ShippingAddress = "", OrderItems = new List<OrderItemDTO>() };
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _orderService.CreateOrderAsync(invalidDto));
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldThrowNotFoundException_WhenOrderDoesNotExist()
        {
            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Orders)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _orderService.DeleteOrderAsync(1));
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldThrowNotFoundException_WhenOrderDoesNotExist()
        {
            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Orders)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _orderService.UpdateOrderAsync(1, new UpdateOrderDTO()));
        }
    }
}
