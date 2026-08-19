using Application.Common.DTOs;
using Application.Common.DTOs.OrderDTOs;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Queries.GetAllOrders
{
    public sealed class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<PaginatedResult<OrderSummaryDto>>>
    {
        private readonly IAppDbContext _context;

        public GetAllOrdersQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedResult<OrderSummaryDto>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Orders
                                .AsNoTracking()
                                .Include(o => o.Customer)
                                .Include(o => o.Payment)
                                .Include(o => o.Shipment)
                                .Include(o => o.OrderItems)
                                .AsQueryable();

            
            if (request.Status.HasValue)
            {
                query = query.Where(x => x.Status == request.Status);
            }
            if (request.PaymentStatus.HasValue)
            {
                query = query.Where(x => x.Payment!.Status == request.PaymentStatus);
            }
            if (request.From.HasValue)
            {
                query = query.Where(x => x.Date >= request.From);
            }
            if (request.To.HasValue)
            {
                query = query.Where(x => x.Date <= request.To);
            }
            if (!string.IsNullOrWhiteSpace(request.CustomerEmail))
            {
                query = query.Where(x => x.Customer!.Email == request.CustomerEmail);
            }
            if (request.MinTotal.HasValue)
            {
                query = query.Where(x => x.Total >= request.MinTotal);
            }
            if (request.MaxTotal.HasValue)
            {
                query = query.Where(x => x.Total <= request.MaxTotal);
            }

            int TotalCount = await query.CountAsync(cancellationToken);

           var response = await query
                .ApplyPagination(request.Page, request.PageSize)
                .ToListAsync(cancellationToken);

            var items = response.Select(o => new OrderSummaryDto()
            {
                OrderId = o.Id,
                Status = o.Status,
                OrderDate = o.Date,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer!.Name,
                CustomerEmail = o.Customer.Email,
                PaymentStatus = o.Payment!.Status,
                Total = o.Total,
                ShipmentStatus = o.Shipment != null ? o.Shipment.Status : null,
                TrackingNumber = o.Shipment != null ? o.Shipment.TrackingNumber : null,
                ItemCount = o.OrderItems.Count(),
            });

            return Result.Success(new PaginatedResult<OrderSummaryDto>()
            {
                page = request.Page ?? 1,
                pageSize = request.PageSize ?? 8,
                totalCount = TotalCount,
                items = items
            });
        }
    }
}
