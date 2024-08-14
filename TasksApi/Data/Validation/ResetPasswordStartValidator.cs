using FluentValidation;
using TasksApi.Models.DTO;

namespace TasksApi.Data.Validation
{
    public class ResetPasswordStartValidator : AbstractValidator<ResetPasswordStartDTO>
    {
        public ResetPasswordStartValidator()
        {
            RuleFor(p => p.Token).NotEmpty();
        }
    }
}
