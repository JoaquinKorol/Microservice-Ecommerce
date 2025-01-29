using FluentValidation;
using Order.DTOs;

namespace Order.Validators
{
    public class UpdateOrderDTOValidator : AbstractValidator<UpdateOrderDTO>
    {
        public UpdateOrderDTOValidator()
        {
            RuleFor(x => x.ShippingAddress).NotEmpty().WithMessage("Shipping address is required.");
            RuleFor(x => x.Status).GreaterThan(0).WithMessage("Status must be a valid value.");
        }
    }
}
