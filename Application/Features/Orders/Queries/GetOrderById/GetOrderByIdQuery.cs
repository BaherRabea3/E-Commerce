
using Application.Common.DTOs.OrderDTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Orders.Queries.GetOrderById
{
    public sealed record GetOrderByIdQuery(int Id, int CustomerId) : IRequest<Result<OrderDetailResponseDto>>
    {
    }
}
