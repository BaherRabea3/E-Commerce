
using Application.Common.DTOs.PaymentDTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Payments.Commands.RefundPayment
{
    public sealed record RefundPaymentCommand(int PaymentId) : IRequest<Result<RefundPaymentResponseDto>>
    {
    }
}
