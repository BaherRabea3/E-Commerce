using Application.Common.DTOs;
using Application.Common.DTOs.OrderDTOs;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Orders;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Queries.GetOrdersByCustomerId
{
    public sealed class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, Result<PaginatedResult<OrderSummaryDto>>>
    {
        private readonly IAppDbContext _context;

        public GetOrderQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedResult<OrderSummaryDto>>> Handle(GetOrderQuery request,CancellationToken cancellationToken)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Include(o => o.Payment)
                .Include(o => o.Shipment)
                .Where(o => o.CustomerId == request.customerId)
                .OrderByDescending(o => o.Date)
                .AsQueryable();


            if (request.Status.HasValue)
                query = query.Where(o => o.Status == request.Status.Value);

            if (request.From.HasValue)
                query = query.Where(o => o.Date >= request.From.Value);

            if (request.To.HasValue)
                query = query.Where(o => o.Date <= request.To.Value);


            var totalCount = await query.CountAsync(cancellationToken);

            var orderList = await query
                .ApplyPagination(request.page, request.pageSize)
                .ToListAsync(cancellationToken);

            if (!orderList.Any())
                return Result.Failure<PaginatedResult<OrderSummaryDto>>(OrderErrors.NotFound);

            var response = new PaginatedResult<OrderSummaryDto>()
            {
                page = request.page ?? 1,
                pageSize = request.pageSize ?? 8,
                totalCount = totalCount,
                items = orderList.Select(o => new OrderSummaryDto()
                {
                    OrderId = o.Id,
                    OrderDate = o.Date,
                    Status = o.Status,
                    Total = o.Total,
                    PaymentStatus = o.Payment!.Status,
                    ShipmentStatus = o.Shipment != null ? o.Shipment.Status : null,
                    TrackingNumber = o.Shipment != null ? o.Shipment.TrackingNumber : null,
                    ItemCount = o.OrderItems.Count
                })
            };

            return Result.Success(response);
        }
    }
}
