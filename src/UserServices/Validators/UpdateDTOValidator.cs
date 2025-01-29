using FluentValidation;
using UserServices.DTOs;

namespace UserServices.Validators
{
    public class UpdateDTOValidator : AbstractValidator<UpdateUserDTO>
    {
        public UpdateDTOValidator() 
        { 
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("The email address is not valid.");

        }
    }
}
