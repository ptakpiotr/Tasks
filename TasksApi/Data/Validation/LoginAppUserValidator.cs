using FluentValidation;
using TasksApi.Models.DTO;

namespace TasksApi.Data.Validation
{
    public class LoginAppUserValidator : AbstractValidator<LoginAppUserDTO>
    {
        public LoginAppUserValidator()
        {
            RuleFor(p => p.Email).NotEmpty().EmailAddress();
            RuleFor(p => p.Password).NotEmpty();
        }
    }
}
