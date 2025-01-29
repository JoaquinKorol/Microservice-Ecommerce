using FluentValidation.TestHelper;
using Order.DTOs;
using Order.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Order.Test
{
    public class CreateOrderDTOValidatorTests
    {
        private readonly CreateOrderDTOValidator _validator;

        public CreateOrderDTOValidatorTests()
        {
            _validator = new CreateOrderDTOValidator();
        }

        [Fact]
        public void Should_Have_Error_When_UserId_Is_Less_Than_Or_Equal_To_Zero()
        {
            var model = new CreateOrderDTO { UserId = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.UserId);
        }

        [Fact]
        public void Should_Have_Error_When_ShippingAddress_Is_Empty()
        {
            var model = new CreateOrderDTO { ShippingAddress = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ShippingAddress);
        }

        [Fact]
        public void Should_Have_Error_When_OrderItems_Is_Empty()
        {
            var model = new CreateOrderDTO { OrderItems = new List<OrderItemDTO>() };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.OrderItems);
        }

        [Fact]
        public void Should_Have_Error_When_OrderItem_Has_Invalid_Quantity_Or_Price()
        {
            var model = new CreateOrderDTO
            {
                OrderItems = new List<OrderItemDTO>
            {
                new OrderItemDTO { Quantity = 0, Price = 10 },
                new OrderItemDTO { Quantity = 1, Price = -5 }
            }
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.OrderItems);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Model_Is_Valid()
        {
            var model = new CreateOrderDTO
            {
                UserId = 1,
                ShippingAddress = "123 Street, City",
                OrderItems = new List<OrderItemDTO>
            {
                new OrderItemDTO { Quantity = 2, Price = 20 }
            }
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
