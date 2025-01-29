using Core.Interfaces;
using Core.Exceptions;
using Microsoft.AspNetCore.Identity;
using ProductCatalog.DTOs;
using ProductCatalog.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using ProductCatalog.Validators;
using FluentValidation;

namespace ProductCatalog.Services
{
    public class ProductService
    {
        private readonly IRepository<Product> _repository;

        public ProductService(IRepository<Product> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {id} not found.");
            }
            return product;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            
            var validator = new ProductValidator();
            var validationResult = await validator.ValidateAsync(product);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.FirstOrDefault()?.ErrorMessage;
                throw new ValidationException(errors);
            }

            var newProduct = new Product
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
            };

            await _repository.AddAsync(newProduct);

            return newProduct;
        }


        public async Task DeleteProductAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {id} not found.");
            }

            await _repository.DeleteAsync(id);
        }

        public async Task<Product> UpdateProductAsync(int id, UpdateProductDTO updateProductDTO)
        {
            var existingProduct = await _repository.GetByIdAsync(id);
            if(existingProduct == null)
            {
                throw new NotFoundException($"Product with ID {id} not found");
            }

            existingProduct.Name = updateProductDTO.Name ?? existingProduct.Name;
            existingProduct.Description = updateProductDTO.Description ?? existingProduct.Description;
            if (updateProductDTO.Price.HasValue)
            {
                existingProduct.Price = updateProductDTO.Price.Value;
            }

            if (updateProductDTO.Stock.HasValue)
            {
                existingProduct.Stock = updateProductDTO.Stock.Value;
            }

            await _repository.UpdateAsync(existingProduct);

            return existingProduct;
            
        }

    }

}

