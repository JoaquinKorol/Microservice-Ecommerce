using FluentValidation;
using ProductCatalog.Models;

namespace ProductCatalog.Validators
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("The name is required.")
                .MinimumLength(3).WithMessage("The name must be at least 3 characters long.")
                .MaximumLength(100).WithMessage("The name cannot exceed 100 characters.")
                .Must(name => name.Trim().Length > 0).WithMessage("The name cannot contain only whitespace.");

            RuleFor(p => p.Description)
                .MaximumLength(500).WithMessage("The description cannot exceed 500 characters.");

            RuleFor(p => p.Price)
                .GreaterThan(0).WithMessage("The price must be greater than 0.")
                .Must(price => price.ToString("F2") == price.ToString()).WithMessage("The price cannot have more than 2 decimal places.");


            RuleFor(p => p.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.")
                .LessThanOrEqualTo(10000).WithMessage("Stock cannot exceed 10,000 units.");
        }
    }
}
