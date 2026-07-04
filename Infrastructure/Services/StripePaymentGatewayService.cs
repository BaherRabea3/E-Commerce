
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Stripe;
using static Application.Common.Interfaces.IPaymentGatewayService;

namespace Infrastructure.Services
{
    public sealed class StripePaymentGatewayService : IPaymentGatewayService
    {
        private readonly PaymentIntentService _paymentIntentService;
        private readonly RefundService _refundService;
        public StripePaymentGatewayService(PaymentIntentService paymentIntentService, RefundService refundService)
        {
            _paymentIntentService = paymentIntentService;
            _refundService = refundService;
        }

        public async Task<IPaymentGatewayService.CreatePaymentIntentResult> CreatePaymentIntentAsync(decimal amount, string currency, int orderId, CancellationToken cancellationToken)
        {
            try
            {
                var paymentOptions = new PaymentIntentCreateOptions()
                {
                    Amount = Convert.ToInt64(Math.Round(amount * 100m)),
                    Currency = currency.ToLower(),
                    Metadata = new Dictionary<string, string> { ["order_id"] = orderId.ToString() }
                };
                var paymentIntent = await _paymentIntentService.CreateAsync(paymentOptions, null, cancellationToken);

                return new CreatePaymentIntentResult(paymentIntent.ClientSecret, paymentIntent.Id);
            }
            catch (StripeException ex)
            {
                throw new PaymentGatewayException(ex.Message);
            }
        }

        public async Task<GetClientSecretResult> GetClientSecretAsync(string gatewayTransactionId, CancellationToken cancellationToken)
        {
            try
            {
                var intent = await _paymentIntentService.GetAsync(gatewayTransactionId, cancellationToken: cancellationToken);

                return new GetClientSecretResult(intent.ClientSecret);
            }
            catch (StripeException ex)
            {
                throw new PaymentGatewayException(ex.Message);
            }
        }

        public async Task RefundPaymentAsync(string gatewayTransactionId, decimal amount, CancellationToken cancellationToken)
        {
            try
            {
                var refundOptions = new RefundCreateOptions()
                {
                    PaymentIntent = gatewayTransactionId,
                    Amount = Convert.ToInt64(Math.Round(amount * 100m))
                };

                await _refundService.CreateAsync(refundOptions, null, cancellationToken);

            }
            catch (StripeException ex)
            {
                throw new PaymentGatewayException(ex.Message);
            }
        }
    }
}
