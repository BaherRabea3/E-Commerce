using Domain.Enums;
using FluentValidation;

namespace Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
    {
        public UpdateOrderStatusCommandValidator()
        {
            RuleFor(x => x.NewStatus)
                .NotEmpty().WithMessage("Order Status can't be empty")
                .IsInEnum().WithMessage("Invalid order status")
                .NotEqual(x => OrderStatus.AwaitingPayment)
                .NotEqual(x => OrderStatus.Confirmed)
                .WithMessage("This status is managed by the system and cannot be set manually.");
        }
    }
}
