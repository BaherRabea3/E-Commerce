
using Domain.Common;
using Domain.Enums;
using System.Data;

namespace Domain.Entities.Payments
{
    public static class PaymentErrors
    {
        public static Error NotFound
           => Error.NotFound("Payment.NotFound", $"Payment not found");
        public static Error IntentCreationError(string errorMessage)
            => Error.BadGateway("Payment.BadGateway", errorMessage);
        public static Error RefundFailed(string errorMessage)
            => Error.BadGateway("Payment.RefundFailed", errorMessage);
        public static Error NotRefundable(PaymentStatus currentStatus)
            => Error.Conflict(
        "Payment.NotRefundable",
        $"Payment cannot be refunded in its current state: '{currentStatus}'. Only Completed payments are refundable.");

        public static Error InvalidStatusTransition(PaymentStatus current, PaymentStatus attempted)
            => Error.Conflict(
        "Payment.InvalidStatusTransition",
        $"Cannot transition payment from '{current}' to '{attempted}'.");
    }
}
