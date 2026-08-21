using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Settings;
using Application.Common.Templates;
using Domain.Enums;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;


namespace Infrastructure.Services.EmailServices
{
    public class EmailService : IEmailService
    {
        private IAppDbContext _context;
        private ILogger<EmailService> _logger;
        private EmailSettings _settings;
        public EmailService(IAppDbContext context, ILogger<EmailService> logger, IOptions<EmailSettings> options)
        {
            _context = context;
            _logger = logger;
            _settings = options.Value;
        }

        public async Task SendOrderConfirmedAsync(int orderId, CancellationToken cancellationToken)
        {
            var data = await GetOrderEmailData(orderId, cancellationToken);

            if(data is null)
                return;

            var estimatedDelivery = data.EstimatedDeliveryDate
                ?? DateTime.UtcNow.AddDays(5);

            var emailMessage = new EmailMessage()
            {
                To = data.CustomerEmail,
                Subject = $"Order #{orderId} Confirmed — We're preparing your order",
                Body = EmailTemplates.OrderConfirmed(data.CustomerName, orderId, data.Total, estimatedDelivery)
            };

            await SendAsync(emailMessage, cancellationToken);
        }

        public async Task SendOrderCancelledAsync(int orderId, CancellationToken cancellationToken)
        {
            var data = await GetOrderEmailData(orderId, cancellationToken);

            if (data is null) return;

            var refundIssued = data.PaymentStatus == PaymentStatus.Refunded;
            var refundAmount = refundIssued ? data.Total : (decimal?)null;

            var emailMessage = new EmailMessage()
            {
                To = data.CustomerEmail,
                Subject = $"Order #{orderId} Cancelled",
                Body = EmailTemplates.OrderCancelled(data.CustomerName, orderId, refundIssued,refundAmount)
            };
           
            await SendAsync(emailMessage, cancellationToken);
        }

        public async Task SendOrderShippedAsync(int orderId, CancellationToken cancellationToken)
        {
            var data = await GetOrderEmailData(orderId, cancellationToken);

            if (data is null) return;

            var emailMessage = new EmailMessage
            {
                To = data.CustomerEmail,
                Subject = $"Order #{orderId} Shipped — Your order is on its way!",
                Body = EmailTemplates.OrderShipped(
                    data.CustomerName,
                    orderId,
                    data.TrackingNumber,
                    data.ShipmentMethod,
                    data.EstimatedDeliveryDate ?? DateTime.UtcNow.AddDays(5))
            };

            await SendAsync(emailMessage, cancellationToken);
        }

        public async Task SendOrderDeliveredAsync(int orderId, CancellationToken cancellationToken)
        {
            var data = await GetOrderEmailData(orderId, cancellationToken);
            if (data is null) return;

            var emailMessage = new EmailMessage
            {
                To = data.CustomerEmail,
                Subject = $"Order #{orderId} Delivered — Enjoy your purchase!",
                Body = EmailTemplates.OrderDelivered(data.CustomerName, orderId)
            };

            await SendAsync(emailMessage, cancellationToken);
        }

        public async Task SendPaymentFailedAsync(int orderId, CancellationToken cancellationToken)
        {
            var data = await GetOrderEmailData(orderId, cancellationToken);
            if (data is null) return;

            var emailMessage = new EmailMessage
            {
                To = data.CustomerEmail,
                Subject = $"Payment Failed for Order #{orderId} — Action Required",
                Body = EmailTemplates.PaymentFailed(
                    data.CustomerName, orderId, data.Total)
            };

            await SendAsync(emailMessage, cancellationToken);
        }

        public async Task SendRefundIssuedAsync(int orderId, CancellationToken cancellationToken)
        {
            var data = await GetOrderEmailData(orderId, cancellationToken);
            if (data is null) return;

            var emailMessage = new EmailMessage
            {
                To = data.CustomerEmail,
                Subject = $"Refund Processed for Order #{orderId}",
                Body = EmailTemplates.RefundIssued(
                    data.CustomerName, orderId, data.Total)
            };

            await SendAsync(emailMessage, cancellationToken);
        }

        private async Task SendAsync(
             EmailMessage message, CancellationToken cancellationToken)
        {
            try
            {
                var email = new MimeMessage();

                email.From.Add(new MailboxAddress(
                    _settings.FromName, _settings.FromEmail));

                email.To.Add(MailboxAddress.Parse(message.To));
                email.Subject = message.Subject;

                var builder = new BodyBuilder();
                if (message.IsHtml)
                    builder.HtmlBody = message.Body;
                else
                    builder.TextBody = message.Body;

                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                
                await smtp.ConnectAsync(
                    _settings.Host,
                    _settings.Port,
                    _settings.EnableSsl
                        ? SecureSocketOptions.StartTls
                        : SecureSocketOptions.None,
                    cancellationToken);

                await smtp.AuthenticateAsync(
                    _settings.Username,
                    _settings.Password,
                    cancellationToken);

                await smtp.SendAsync(email, cancellationToken);
                await smtp.DisconnectAsync(true, cancellationToken);

                _logger.LogInformation(
                    "Email sent to {Email} — Subject: {Subject}",
                    message.To, message.Subject);
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex,
                    "Failed to send email to {Email} — Subject: {Subject}",
                    message.To, message.Subject);

                throw; // re-throw so Hangfire knows to retry
            }
        }
        private async Task<OrderEmailData?> GetOrderEmailData(
            int orderId, CancellationToken cancellationToken)
        {
            var data = await _context.Orders
                .AsNoTracking()
                .Where(o => o.Id == orderId)
                .Select(o => new OrderEmailData
                {
                    CustomerName = o.Customer!.Name,
                    CustomerEmail = o.Customer.Email,
                    Total = o.Total,
                    PaymentStatus = o.Payment!.Status,
                    TrackingNumber = o.Shipment != null
                        ? o.Shipment.TrackingNumber
                        : null,
                    ShipmentMethod = o.Shipment != null
                        ? o.Shipment.Method
                        : null,
                    EstimatedDeliveryDate = o.Shipment != null
                        ? o.Shipment.EstimatedDeliveryDate
                        : null
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (data is null)
            {
                _logger.LogWarning(
                    "Cannot send email — Order {OrderId} not found.", orderId);
                return null;
            }

            return data;
        }

        private sealed class OrderEmailData
        {
            public string CustomerName { get; set; } = default!;
            public string CustomerEmail { get; set; } = default!;
            public decimal Total { get; set; }
            public PaymentStatus PaymentStatus { get; set; }
            public string? TrackingNumber { get; set; }
            public string? ShipmentMethod { get; set; }
            public DateTime? EstimatedDeliveryDate { get; set; }
        }
    }
}
