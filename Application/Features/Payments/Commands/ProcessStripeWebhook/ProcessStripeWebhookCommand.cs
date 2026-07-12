
using Domain.Common;
using MediatR;

namespace Application.Features.Payments.Commands.ProcessStripeWebhook
{
    public sealed record ProcessStripeWebhookCommand(string EventId , string EventType , string RawJson) : IRequest<Result>
    {
    }
}
