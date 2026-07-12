
using Application.Common.DTOs.PaymentDTOs;
using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Features.Payments.Commands.UpdatePaymentStatus
{
    public sealed record UpdatePaymentStatusCommand(int PaymentId , PaymentStatus NewStatus) : IRequest<Result<UpdatePaymentStatusResponseDto>>
    {
    }
}
