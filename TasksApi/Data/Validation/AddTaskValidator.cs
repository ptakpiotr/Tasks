using FluentValidation;
using TasksApi.Models.DTO;

namespace TasksApi.Data.Validation
{
    public class AddTaskValidator : AbstractValidator<AddTaskDTO>
    {
        public AddTaskValidator()
        {
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.Content).NotEmpty().MaximumLength(250);
        }
    }
}
