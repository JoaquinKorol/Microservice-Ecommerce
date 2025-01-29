using Xunit;
using Moq;
using ProductCatalog.Services;
using ProductCatalog.Repositories;
using ProductCatalog.Models;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ValidationException = FluentValidation.ValidationException;
using ValidationResult = FluentValidation.Results.ValidationResult;
using Core.Interfaces;

public class ProductCatalogServiceTests
{
    private readonly Mock<IRepository<Product>> _mockRepo;
    private readonly ProductService _service;
    private readonly Mock<IValidator<Product>> _mockValidator;

    public ProductCatalogServiceTests()
    {
        _mockRepo = new Mock<IRepository<Product>>();
        _mockValidator = new Mock<IValidator<Product>>();
        _service = new ProductService(_mockRepo.Object);
    }

    [Fact]
    public async Task CreateProductAsync_ValidProduct_ReturnsProduct()
    {
        // Arrange
        var product = new Product { Name = "TestProduct", Price = 100.00M, Stock = 50, Description = "Test Description" };
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateProductAsync(product);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Name, result.Name);
        Assert.Equal(product.Price, result.Price);
        Assert.Equal(product.Stock, result.Stock);
    }

    [Fact]
    public async Task CreateProductAsync_InvalidProduct_ThrowsValidationException()
    {
        // Arrange
        var product = new Product { Name = "", Price = -100, Stock = -50, Description = "Invalid Product" };
        var validationFailure = new ValidationFailure("Name", "Name cannot be empty.");
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(async () => await _service.CreateProductAsync(product));
    }

    [Fact]
    public async Task GetAllProductsAsync_ReturnsListOfProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Name = "Product 1", Price = 100, Stock = 10, Description = "Description 1" },
            new Product { Name = "Product 2", Price = 200, Stock = 20, Description = "Description 2" }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        // Act
        var result = await _service.GetProductsAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count());
    }
}