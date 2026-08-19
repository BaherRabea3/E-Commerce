using Application.Common.DTOs;
using Application.Common.DTOs.OrderDTOs;
using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Features.Orders.Queries.GetAllOrders
{
    public sealed record GetAllOrdersQuery( 

    int? Page,
    int? PageSize,

    OrderStatus? Status,
    PaymentStatus? PaymentStatus,

    DateTime? From,
    DateTime? To,

    string? CustomerEmail,

    decimal? MinTotal,
    decimal? MaxTotal) : IRequest<Result<PaginatedResult<OrderSummaryDto>>>
    { }
}
