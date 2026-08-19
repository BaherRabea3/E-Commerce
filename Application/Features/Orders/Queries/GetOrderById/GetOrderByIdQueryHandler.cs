
using Application.Common.DTOs.AddressDTOs;
using Application.Common.DTOs.OrderDTOs;
using Application.Common.DTOs.PaymentDTOs;
using Application.Common.DTOs.ShipmentDTOs;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Orders;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Queries.GetOrderById
{
    public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDetailResponseDto>>
    {
        private readonly IAppDbContext _context;

        public GetOrderByIdQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<OrderDetailResponseDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {

            var customer = await _context.Customers.FirstAsync(x => x.UserId == request.CustomerId, cancellationToken);

            var order = await _context.Orders
                                .AsNoTracking()
                                .Include(o => o.Address)
                                .Include(o => o.Payment)
                                .Include(o => o.Shipment)
                                .FirstOrDefaultAsync(o => o.Id == request.Id
                                                         && o.CustomerId == customer.Id,
                                                         cancellationToken);

            if(order is null)
                return Result.Failure<OrderDetailResponseDto>(OrderErrors.NotFound);

            var items = await (from p in _context.Products
                        join oi in _context.OrderItems
                        on p.Id equals oi.ProductId 
                        where oi.OrderId == order.Id
                        select new OrderItemDto()
                        {
                            ProductId = oi.ProductId,
                            ProductImageUrl = p.Image,
                            ProductName = p.Name,
                            Quantity = oi.Quantity,
                            UnitPrice = oi.Price
                        }
                        ).ToListAsync(cancellationToken);

            return Result.Success(MapToResponse(order, items));
        }
        private OrderDetailResponseDto MapToResponse(Order order, List<OrderItemDto> items)
            => new OrderDetailResponseDto()
            {
                OrderId = order.Id,
                OrderDate = order.Date,
                Status = order.Status.ToString(),
                Subtotal = order.Subtotal,
                ShippingCost = order.ShippingCost,
                Total = order.Total,
                Items = items,
                Address = new ShippingAddressDto()
                {
                    Country = order.Address.country,
                    City = order.Address.city,
                    Area = order.Address.Area,
                    StreetBlock = order.Address.StreetBlock,
                    PostalCode = order.Address.PostalCode,
                    State = order.Address.state,
                    HouseNo = order.Address.HouseNo
                },
                Payment = new PaymentSummartDto()
                {
                    Method = order.Payment!.Method,
                    Status = order.Payment.Status.ToString(),
                    Amount = order.Payment.Amount,
                    PaidAt = order.Payment.Status == PaymentStatus.Completed
                        ? order.Payment.PaidAt
                        : null
                },
                Shipment = order.Shipment is null
                    ? null
                    : new ShipmentSummaryDto
                    {
                        TrackingNumber = order.Shipment.TrackingNumber,
                        Method = order.Shipment.Method,
                        Status = order.Shipment.Status.ToString(),
                        EstimatedDeliveryDate = order.Shipment.EstimatedDeliveryDate,
                        ActualDeliveryDate = order.Shipment.ActualDeliveryDate
                    }
            };
    }
}
