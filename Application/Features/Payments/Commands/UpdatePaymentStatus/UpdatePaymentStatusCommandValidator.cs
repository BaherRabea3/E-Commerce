
using Domain.Enums;
using FluentValidation;

namespace Application.Features.Payments.Commands.UpdatePaymentStatus
{
    public class UpdatePaymentStatusCommandValidator : AbstractValidator<UpdatePaymentStatusCommand>
    {
        public UpdatePaymentStatusCommandValidator()
        {
            RuleFor(x => x.PaymentId)
                .GreaterThan(0);

            RuleFor(x => x.NewStatus)
                .IsInEnum().WithMessage("Invalid payment status.")
                .NotEqual(PaymentStatus.Pending) .WithMessage("Cannot manually set status to Pending — this is a system-managed state.");
        }
    }
}
