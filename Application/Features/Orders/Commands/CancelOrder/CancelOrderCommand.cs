using Application.Common.DTOs.OrderDTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Orders.Commands.CancelOrder
{
    public sealed record CancelOrderCommand(int OrderId, int CustomerId) : IRequest<Result<CancelOrderResponseDto>>
    {
    }
}
