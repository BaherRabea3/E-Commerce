using Application.Common.DTOs.ShipmentDTOs;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Orders;
using Domain.Entities.Shipments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Shipments.Queries.GetShipmentDetails
{
    public sealed class GetShipmentDetailsQueryHandler : IRequestHandler<GetShipmentDetailsQuery, Result<ShipmentDetailsDto>>
    {
        private readonly IAppDbContext _context;

        public GetShipmentDetailsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ShipmentDetailsDto>> Handle(GetShipmentDetailsQuery request, CancellationToken cancellationToken)
        {

            bool orderExisted = await _context.Orders.AnyAsync(o => o.Id == request.OrderId);

            if(!orderExisted)
               return Result.Failure<ShipmentDetailsDto>(OrderErrors.NotFound);

            var shipment = await _context.Shipments
                                         .AsNoTracking()
                                         .Where(s => s.OrderId == request.OrderId)
                                         .Select(s => new ShipmentDetailsDto
                                         {
                                             ShipmentId = s.Id,
                                             Method = s.Method,
                                             Status = s.Status.ToString(),
                                             TrackingNumber = s.TrackingNumber,
                                             ActualDeliveryDate = s.ActualDeliveryDate,
                                             EstimatedDeliveryDate = s.EstimatedDeliveryDate
                                         })
                                         .FirstOrDefaultAsync(cancellationToken);

            if (shipment is null)
                return Result.Failure<ShipmentDetailsDto>(ShipmentErrors.NotFound);

            return Result.Success(shipment);

        }
    }
}
