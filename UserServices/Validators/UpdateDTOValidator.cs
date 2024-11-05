using FluentValidation;
using UserServices.DTOs;

namespace UserServices.Validators
{
    public class UpdateDTOValidator : AbstractValidator<UpdateDTO>
    {
        public UpdateDTOValidator() 
        { 
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("The email address is not valid.");

        }
    }
}
