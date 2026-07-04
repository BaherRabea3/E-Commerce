using Application.Common.DTOs.OrderDTOs;
using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Features.Orders.Commands.UpdateOrderStatus
{
    public sealed record UpdateOrderStatusCommand(int OrderId , OrderStatus NewStatus) : IRequest<Result<UpdateOrderStatusResponseDto>>
    {
    }
}
