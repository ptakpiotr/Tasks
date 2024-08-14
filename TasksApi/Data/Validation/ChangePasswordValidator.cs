using FluentValidation;
using TasksApi.Helpers;
using TasksApi.Models.DTO;

namespace TasksApi.Data.Validation
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordDTO>
    {
        public ChangePasswordValidator()
        {
            RuleFor(p => p.OldPassword).NotEmpty();
            RuleFor(p => p.NewPassword).NotEqual(p => p.OldPassword).Must(PasswordHelpers.ValidatePassword);
        }
    }
}
