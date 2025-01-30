using FluentValidation;
using Payment.DTOs;

namespace Payment.Validators
{
    public class PaymentRequestDTOValidator : AbstractValidator<PaymentRequestDTO>
    {
        public PaymentRequestDTOValidator()
        {
            RuleFor(x => x.Amount)
           .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(255).WithMessage("Description cannot be longer than 255 characters.");

            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("OrderId must be greater than zero.");
        }
    }
}
