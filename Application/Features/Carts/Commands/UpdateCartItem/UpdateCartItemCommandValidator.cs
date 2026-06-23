
using FluentValidation;

namespace Application.Features.Carts.Commands.UpdateCartItem
{
    public class UpdateCartItemCommandValidator : AbstractValidator<UpdateCartItemCommand>
    {
        public UpdateCartItemCommandValidator()
        {
            RuleFor(x => x.quantity)
                .GreaterThanOrEqualTo(1).WithMessage("Quantity cannot be negative or zero");
        }
    }
}
