using FluentValidation;

namespace Application.Features.Products.Commands.UpdateProductStock
{
    public class UpdateProductStockCommandValidator : AbstractValidator<UpdateProductStockCommand>
    {
        public UpdateProductStockCommandValidator()
        {
            RuleFor(x => x.quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity can not be negative");
        }
    }
}
