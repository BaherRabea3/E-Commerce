using FluentValidation;

namespace Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        private readonly string[] _allowedExtensions = [".jpg", ".png"];
        private const int MaxFileSizeBytes = 2 * 1024 * 1024;
        public CreateProductCommandValidator()
        {

            RuleFor(x => x.name).NotEmpty().WithMessage("Product name is required")
                .MaximumLength(200).MinimumLength(2);

            RuleFor(x => x.description).NotEmpty().WithMessage("Product description is required")
                .MaximumLength(1000);

            RuleFor(x => x.unitPrice)
                .GreaterThan(0).WithMessage("Price must be greater than zero")
                .LessThan(100000).WithMessage("Price is too large");

            RuleFor(x => x.quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative");

            RuleFor(x => x.categoryId)
               .GreaterThan(0).WithMessage("Category is required");

            RuleFor(x => x.Image)
               .NotEmpty().WithMessage("Product Image is required")
               .Must(f => f.Length > 0).WithMessage("Product Image cannot be empty")
               .Must(f => _allowedExtensions.Contains(Path.GetExtension(f.FileName).ToLowerInvariant()))
                    .WithMessage($"Only {string.Join(", ", _allowedExtensions)} are allowed.")
               .Must(f => f.Length <= MaxFileSizeBytes).WithMessage("Image must not exceed 2MB.");

        }
    }
}
