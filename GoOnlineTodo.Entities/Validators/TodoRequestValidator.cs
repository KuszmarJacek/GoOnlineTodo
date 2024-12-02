using FluentValidation;
using GoOnlineTodo.Entities.DTOs;

namespace GoOnlineTodo.Entities.Validators
{
    public class TodoRequestValidator : AbstractValidator<TodoRequestDto>
    {
        public TodoRequestValidator() 
        {
            RuleFor(todo => todo.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(256).WithMessage("Title can't exceed 256 characters");

            RuleFor(todo => todo.Description)
                .MaximumLength(1024).WithMessage("Title can't exceed 1024 characters")
                // description is nullable, therefore validate only if it is provided
                .When(todo => !string.IsNullOrEmpty(todo.Description));

            RuleFor(todo => todo.ExpiryDateTime)
                .NotEmpty().WithMessage("Every todo needs to have an expiry date");

            RuleFor(todo => todo.PercentComplete)
                .InclusiveBetween(0, 100).WithMessage("Percentage of completion must be between 0 and 100.");
        }
    }
}
