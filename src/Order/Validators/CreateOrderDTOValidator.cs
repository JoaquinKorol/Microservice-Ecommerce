using FluentValidation;
using Order.DTOs;

namespace Order.Validators
{
    public class CreateOrderDTOValidator : AbstractValidator<CreateOrderDTO>
    {
        public CreateOrderDTOValidator()
        {
            
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("UserId must be greater than 0.");

            
            RuleFor(x => x.ShippingAddress)
                .NotEmpty()
                .WithMessage("Shipping address cannot be empty.");


            RuleFor(x => x.OrderItems)
                .NotEmpty()
                .WithMessage("OrderItems cannot be empty.")
                .Must(items => items.All(item => item.Quantity > 0 && item.Price > 0))
                .WithMessage("Each item must have a valid quantity and price greater than 0.");
        }

    }
}
