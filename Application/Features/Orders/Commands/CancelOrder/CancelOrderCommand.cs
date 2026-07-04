using Application.Common.DTOs.OrderDTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Orders.Commands.CancelOrder
{
    public sealed record CancelOrderCommand(int OrderId, Guid CustomerId) : IRequest<Result<CancelOrderResponseDto>>
    {
    }
}
