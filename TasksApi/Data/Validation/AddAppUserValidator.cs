using FluentValidation;
using TasksApi.Helpers;
using TasksApi.Models.DTO;

namespace TasksApi.Data.Validation
{
    public class AddAppUserValidator : AbstractValidator<AddAppUserDTO>
    {
        public AddAppUserValidator()
        {
            RuleFor(p => p.Email).NotEmpty().EmailAddress();
            RuleFor(p => p.UserName).Length(3, 50);
            RuleFor(p => p.Password).Must(PasswordHelpers.ValidatePassword);
        }
    }
}
