using FluentValidation.TestHelper;
using Order.DTOs;
using Order.Validators;

namespace Order.Test
{
    public class UpdateOrderDTOValidatorTests
    {
        private readonly UpdateOrderDTOValidator _validator;

        public UpdateOrderDTOValidatorTests()
        {
            _validator = new UpdateOrderDTOValidator();
        }

        [Fact]
        public void Should_Have_Error_When_ShippingAddress_Is_Empty()
        {
            var model = new UpdateOrderDTO { ShippingAddress = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ShippingAddress);
        }

        [Fact]
        public void Should_Have_Error_When_Status_Is_Less_Than_Or_Equal_To_Zero()
        {
            var model = new UpdateOrderDTO { Status = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Status);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Model_Is_Valid()
        {
            var model = new UpdateOrderDTO
            {
                ShippingAddress = "456 Main Street, City",
                Status = 1
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();

        }
    }
}