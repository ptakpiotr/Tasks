using FluentValidation;
using TasksApi.Helpers;
using TasksApi.Models.DTO;

namespace TasksApi.Data.Validation
{
    public class ResetPasswordEndValidator : AbstractValidator<ResetPasswordEndDTO>
    {
        public ResetPasswordEndValidator()
        {
            RuleFor(p => p.Token).NotEmpty();
            RuleFor(p => p.NewPassword).Must(PasswordHelpers.ValidatePassword);
        }
    }
}
