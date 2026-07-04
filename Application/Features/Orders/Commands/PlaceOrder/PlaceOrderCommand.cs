using Application.Common.DTOs.OrderDTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Orders.Commands.PlaceOrder
{
    public sealed record PlaceOrderCommand(Guid customerId , int addressId, Guid IdempotencyKey, string paymentMethod = "Card") : IRequest<Result<PlaceOrderResponseDto>>
    {
    }
}
