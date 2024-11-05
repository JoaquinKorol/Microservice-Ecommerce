using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServices.DTOs;
using UserServices.Services;
using UserServices.Validators;
using FluentValidation.Results;


namespace UserServices.Test
{

   public class RegisterDTOValidatorTests
   {
       private readonly RegisterDTOValidator _validator;
       public RegisterDTOValidatorTests()
       {
           _validator = new RegisterDTOValidator();
       }

       [Fact]
       public async Task CreateUserAsync_ShouldThrowException_WhenNameIsEmpty()
       {
           var newUserDto = new RegisterDTO
           {
               Name = "",
               Email = "jane.doe@example.com",
               Password = "securePassword123"
           };

           ValidationResult result = await _validator.ValidateAsync(newUserDto); 

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "Name cannot be empty.");

       }

       [Fact]
       public async Task CreateUserAsync_ShouldThrowException_WhenEmailIsEmpty()
       {
           var newUserDto = new RegisterDTO
           {
               Name = "Jane Doe",
               Email = "",
               Password = "securePassword123"
           };

            ValidationResult result = await _validator.ValidateAsync(newUserDto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "Email cannot be empty.");

        }
       [Fact]
       public async Task CreateUserAsync_ShouldThrowException_WhenPasswordIsEmpty()
       {
           var newUserDto = new RegisterDTO
           {
               Name = "Jane Doe",
               Email = "jane.doe@example.com",
               Password = ""
           };

            ValidationResult result = await _validator.ValidateAsync(newUserDto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "Password cannot be empty.");

        }

        [Fact]
        public async Task CreateUserAsync_ShouldThrowException_WhenNotValidName()
        {
            var newUserDto = new RegisterDTO
            {
                Name = "Jane Doe123",
                Email = "jane@doe.com",
                Password = "securePassword123"
            };

            ValidationResult result = await _validator.ValidateAsync(newUserDto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "Invalid name. Only letters, spaces, hyphens, and apostrophes are allowed.");
        }

        [Fact]
        public async Task CreateUserAsync_ShouldThrowException_WhenNotValidEmail()  
        {
            var newUserDto = new RegisterDTO
            {
                Name = "Jane Doe",
                Email = "jane.doe.com",
                Password = "securePassword123"
            };

            ValidationResult result = await _validator.ValidateAsync(newUserDto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "The email address is not valid.");
        }
       

        [Fact]
        public async Task CreateUserAsync_ShouldThrowException_WhenNotValidPassword()
        {
            var newUserDto = new RegisterDTO
            {
                Name = "Jane Doe",
                Email = "jane@doe.com",
                Password = "123"
            };

            ValidationResult result = await _validator.ValidateAsync(newUserDto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "The password must be at least 8 characters long and include uppercase letters, lowercase letters, and numbers.");
        }
    }
}