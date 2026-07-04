using Application.Common.DTOs;
using Application.Common.DTOs.OrderDTOs;
using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Features.Orders.Queries.GetOrdersByCustomerId
{
    public sealed record GetOrderQuery(
        Guid customerId,
        int? page,
        int? pageSize,
        OrderStatus? Status,
        DateTime? From,
        DateTime? To
        ) : IRequest<Result<PaginatedResult<OrderSummaryDto>>>
    {
    }
}
