
using FluentValidation;

namespace Application.Features.Payments.Commands.RefundPayment
{
    public class RefundPaymentCommandValidator : AbstractValidator<RefundPaymentCommand>
    {
        public RefundPaymentCommandValidator()
        {
            RuleFor(x => x.PaymentId)
                .GreaterThan(0);
        }
    }
}
