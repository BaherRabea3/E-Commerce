using FluentValidation;

namespace Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(c => c.name)
                .NotEmpty().WithMessage("Category name is required");

            RuleFor(c => c.description)
                .NotEmpty().WithMessage("Category description is required");
        }
    }
}
