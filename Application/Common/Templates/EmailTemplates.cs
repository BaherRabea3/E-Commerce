
namespace Application.Common.Templates
{
    public static class EmailTemplates
    {
        public static string OrderConfirmed(string customerName, int orderId, decimal total, DateTime estimatedDelivery) => $"""
            <!DOCTYPE html>
            <html>
            <body style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;">
                <div style="background-color: #4CAF50; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;">
                    <h1 style="color: white; margin: 0;">Order Confirmed ✓</h1>
                </div>
                <div style="background-color: #f9f9f9; padding: 30px; border-radius: 0 0 8px 8px;">
                    <p>Hi <strong>{customerName}</strong>,</p>
                    <p>Your order has been confirmed and we're preparing it for shipment.</p>
                    <div style="background: white; border: 1px solid #ddd; border-radius: 8px; padding: 20px; margin: 20px 0;">
                        <h3 style="margin-top: 0;">Order Details</h3>
                        <p><strong>Order ID:</strong> #{orderId}</p>
                        <p><strong>Total Paid:</strong> ${total:F2}</p>
                        <p><strong>Estimated Delivery:</strong> {estimatedDelivery:MMMM dd, yyyy}</p>
                    </div>
                    <p>We'll notify you once your order has been shipped.</p>
                    <p style="color: #888; font-size: 12px;">
                        If you have any questions, please contact our support team.
                    </p>
                </div>
            </body>
            </html>
            """;

        public static string OrderCancelled(string customerName, int orderId, bool refundIssued, decimal? refundAmount) => $"""
            <!DOCTYPE html>
            <html>
            <body style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;">
                <div style="background-color: #f44336; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;">
                    <h1 style="color: white; margin: 0;">Order Cancelled</h1>
                </div>
                <div style="background-color: #f9f9f9; padding: 30px; border-radius: 0 0 8px 8px;">
                    <p>Hi <strong>{customerName}</strong>,</p>
                    <p>Your order <strong>#{orderId}</strong> has been cancelled.</p>
                    {(refundIssued && refundAmount.HasValue
                        ? $"""
                           <div style="background: #fff3cd; border: 1px solid #ffc107; border-radius: 8px; padding: 20px; margin: 20px 0;">
                               <h3 style="margin-top: 0; color: #856404;">Refund Issued</h3>
                               <p>A refund of <strong>${refundAmount.Value:F2}</strong> has been issued to your original payment method.</p>
                               <p>Please allow 5-10 business days for the refund to appear.</p>
                           </div>
                           """
                        : "<p>No payment was collected for this order.</p>")}
                    <p style="color: #888; font-size: 12px;">
                        If you did not request this cancellation, please contact support immediately.
                    </p>
                </div>
            </body>
            </html>
            """;

        public static string OrderShipped(string customerName, int orderId, string? trackingNumber, string? carrier, DateTime estimatedDelivery) => $"""
            <!DOCTYPE html>
            <html>
            <body style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;">
                <div style="background-color: #2196F3; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;">
                    <h1 style="color: white; margin: 0;">Your Order is On Its Way! 🚚</h1>
                </div>
                <div style="background-color: #f9f9f9; padding: 30px; border-radius: 0 0 8px 8px;">
                    <p>Hi <strong>{customerName}</strong>,</p>
                    <p>Great news! Your order <strong>#{orderId}</strong> has been shipped.</p>
                    <div style="background: white; border: 1px solid #ddd; border-radius: 8px; padding: 20px; margin: 20px 0;">
                        <h3 style="margin-top: 0;">Shipping Details</h3>
                        {(trackingNumber is not null
                            ? $"<p><strong>Tracking Number:</strong> {trackingNumber}</p>"
                            : "")}
                        {(carrier is not null
                            ? $"<p><strong>Carrier:</strong> {carrier}</p>"
                            : "")}
                        <p><strong>Estimated Delivery:</strong> {estimatedDelivery:MMMM dd, yyyy}</p>
                    </div>
                    <p style="color: #888; font-size: 12px;">
                        Please allow 24 hours for tracking information to update.
                    </p>
                </div>
            </body>
            </html>
            """;

        public static string OrderDelivered(string customerName, int orderId) => $"""
            <!DOCTYPE html>
            <html>
            <body style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;">
                <div style="background-color: #4CAF50; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;">
                    <h1 style="color: white; margin: 0;">Order Delivered! 🎉</h1>
                </div>
                <div style="background-color: #f9f9f9; padding: 30px; border-radius: 0 0 8px 8px;">
                    <p>Hi <strong>{customerName}</strong>,</p>
                    <p>Your order <strong>#{orderId}</strong> has been delivered successfully.</p>
                    <p>We hope you enjoy your purchase! If you have any issues with your order, please contact our support team within 30 days.</p>
                    <p style="color: #888; font-size: 12px;">
                        Thank you for shopping with us!
                    </p>
                </div>
            </body>
            </html>
            """;

        public static string PaymentFailed(string customerName, int orderId, decimal amount) => $"""
            <!DOCTYPE html>
            <html>
            <body style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;">
                <div style="background-color: #FF9800; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;">
                    <h1 style="color: white; margin: 0;">Payment Failed</h1>
                </div>
                <div style="background-color: #f9f9f9; padding: 30px; border-radius: 0 0 8px 8px;">
                    <p>Hi <strong>{customerName}</strong>,</p>
                    <p>We were unable to process your payment of <strong>${amount:F2}</strong> for order <strong>#{orderId}</strong>.</p>
                    <div style="background: #fff3cd; border: 1px solid #ffc107; border-radius: 8px; padding: 20px; margin: 20px 0;">
                        <h3 style="margin-top: 0; color: #856404;">What to do next</h3>
                        <ul>
                            <li>Check that your card details are correct</li>
                            <li>Ensure you have sufficient funds</li>
                            <li>Try a different payment method</li>
                            <li>Contact your bank if the issue persists</li>
                        </ul>
                    </div>
                    <p>Your order has been held for 30 minutes while you retry payment. After that it will be automatically cancelled.</p>
                    <p style="color: #888; font-size: 12px;">
                        If you need assistance, please contact our support team.
                    </p>
                </div>
            </body>
            </html>
            """;

        public static string RefundIssued(string customerName, int orderId, decimal refundAmount) => $"""
            <!DOCTYPE html>
            <html>
            <body style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;">
                <div style="background-color: #9C27B0; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;">
                    <h1 style="color: white; margin: 0;">Refund Processed</h1>
                </div>
                <div style="background-color: #f9f9f9; padding: 30px; border-radius: 0 0 8px 8px;">
                    <p>Hi <strong>{customerName}</strong>,</p>
                    <p>Your refund for order <strong>#{orderId}</strong> has been successfully processed.</p>
                    <div style="background: white; border: 1px solid #ddd; border-radius: 8px; padding: 20px; margin: 20px 0;">
                        <h3 style="margin-top: 0;">Refund Details</h3>
                        <p><strong>Order ID:</strong> #{orderId}</p>
                        <p><strong>Refund Amount:</strong> ${refundAmount:F2}</p>
                        <p><strong>Processing Time:</strong> 5-10 business days</p>
                    </div>
                    <p style="color: #888; font-size: 12px;">
                        The refund will be returned to your original payment method.
                    </p>
                </div>
            </body>
            </html>
            """;
    }
}
