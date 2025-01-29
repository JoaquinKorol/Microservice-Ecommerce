using Xunit;
using FluentValidation;
using ProductCatalog.Models;
using FluentValidation.Results;
using ProductCatalog.Validators;

public class ProductValidatorTests
{
    private readonly ProductValidator _validator;

    public ProductValidatorTests()
    {
        _validator = new ProductValidator();
    }

    [Fact]
    public void Validate_ValidProduct_ReturnsValidResult()
    {
        // Arrange
        var product = new Product
        {
            Name = "ValidProduct",
            Price = 100.00M,
            Stock = 10,
            Description = "Valid Description"
        };

        // Act
        var result = _validator.Validate(product);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_InvalidProduct_ReturnsInvalidResult()
    {
        // Arrange
        var product = new Product
        {
            Name = "",
            Price = -1,
            Stock = -10,
            Description = "Invalid Description"
        };

        // Act
        var result = _validator.Validate(product);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name" && e.ErrorMessage == "The name is required.");
        Assert.Contains(result.Errors, e => e.PropertyName == "Price" && e.ErrorMessage == "The price must be greater than 0.");
        Assert.Contains(result.Errors, e => e.PropertyName == "Stock" && e.ErrorMessage == "Stock cannot be negative.");
    }
}