using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServices.DTOs;
using UserServices.Validators;

namespace UserServices.Test
{
    public class UpdateDTOValidatorTests
    {
        private readonly UpdateDTOValidator _validator;

        public UpdateDTOValidatorTests()
        {
            _validator = new UpdateDTOValidator();
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrowException_WhenNotValidEmail()
        {
            var newUserDto = new UpdateDTO
            {
                Name = "Jane Doe",
                Email = "jane.doe.com"
            };

            ValidationResult result = await _validator.ValidateAsync(newUserDto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "The email address is not valid.");
        }
    }
}
