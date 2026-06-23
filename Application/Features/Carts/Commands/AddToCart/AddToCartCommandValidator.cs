using FluentValidation;

namespace Application.Features.Carts.Commands.AddToCart
{
    public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
    {
        public AddToCartCommandValidator()
        {
            RuleFor(x => x.quantity)
                .GreaterThanOrEqualTo(1).WithMessage("Quantity cannot be negative or zero");
        }
    }
}
