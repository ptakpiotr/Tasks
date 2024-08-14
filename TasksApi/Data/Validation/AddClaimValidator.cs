using FluentValidation;
using TasksApi.Models.DTO;

namespace TasksApi.Data.Validation
{
    public class AddClaimValidator : AbstractValidator<AddClaimDTO>
    {
        public AddClaimValidator()
        {
            RuleFor(p => p.Type).NotEmpty();
            RuleFor(p => p.Value).NotEmpty();
        }
    }
}
