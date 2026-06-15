
using FluentValidation;

namespace Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(c => c.name)
               .NotEmpty().WithMessage("Category name is required");

            RuleFor(c => c.description)
                .NotEmpty().WithMessage("Category description is required");

        }
    }
}
