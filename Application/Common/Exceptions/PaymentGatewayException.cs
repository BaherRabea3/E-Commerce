
namespace Application.Common.Exceptions
{
    public class PaymentGatewayException : Exception
    {
        public PaymentGatewayException(string? message) : base(message)
        {
        }
    }
}
