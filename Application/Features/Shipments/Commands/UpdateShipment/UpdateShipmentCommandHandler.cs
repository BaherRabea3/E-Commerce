
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Shipments;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Shipments.Commands.UpdateShipment
{
    public sealed class UpdateShipmentCommandHandler : IRequestHandler<UpdateShipmentCommand, Result>
    {
        private readonly IAppDbContext _context;

        public UpdateShipmentCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateShipmentCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _context.Shipments
                .Include(s => s.Order)
                .FirstOrDefaultAsync(s => s.Id == request.ShipmentId, cancellationToken);

            if (shipment is null)
                return Result.Failure(ShipmentErrors.NotFound);

            
            if (request.ShippingStatus.HasValue)
            {
                if(shipment.CanTransitionTo(request.ShippingStatus.Value))
                    return Result.Failure(ShipmentErrors.InvalidStatusTransition(shipment.Status, request.ShippingStatus.Value));

                shipment.Status = request.ShippingStatus.Value;

                if (request.ShippingStatus.Value == ShippingStatus.Delivered)
                {
                    shipment.Order!.Status = OrderStatus.Delivered;
                    shipment.ActualDeliveryDate = DateTime.UtcNow;
                }
            }


            if (!string.IsNullOrWhiteSpace(request.ShipmentMethod))
                shipment.Method = request.ShipmentMethod;

            if (request.EstimatedDeliveryDate.HasValue)
                shipment.EstimatedDeliveryDate = request.EstimatedDeliveryDate.Value;

            if (request.ActualDeliveryDate.HasValue)
                shipment.ActualDeliveryDate = request.ActualDeliveryDate.Value;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }
    }
}
