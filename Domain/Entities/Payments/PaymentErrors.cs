
using Domain.Common;

namespace Domain.Entities.Payments
{
    public static class PaymentErrors
    {
        public static Error IntentCreationError(string errorMessage)
            => Error.BadGateway("Payment.BadGateway", errorMessage);
        public static Error RefundFailed(string errorMessage)
            => Error.BadGateway("Payment.RefundFailed", errorMessage);
    }
}
