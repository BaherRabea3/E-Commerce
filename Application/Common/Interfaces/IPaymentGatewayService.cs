
namespace Application.Common.Interfaces
{
    public interface IPaymentGatewayService
    {
        Task<CreatePaymentIntentResult> CreatePaymentIntentAsync(
            decimal amount, string currency, int orderId, CancellationToken cancellationToken);

        Task RefundPaymentAsync(
            string gatewayTransactionId, decimal amount, CancellationToken cancellationToken);

        Task<GetClientSecretResult> GetClientSecretAsync(string gatewayTransactionId, CancellationToken cancellationToken);

        string GetGatewayTransactionId(string Rawjson);

        sealed record CreatePaymentIntentResult(string? ClientSecret, string? GatewayTransactionId);
        sealed record GetClientSecretResult(string? ClientSecret);

    }
}
