using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Features.Shipments.Commands.UpdateShipment
{
    public sealed record UpdateShipmentCommand(int ShipmentId ,
        string? ShipmentMethod ,
        ShippingStatus? ShippingStatus ,
        DateTime? EstimatedDeliveryDate ,
        DateTime? ActualDeliveryDate) : IRequest<Result>
    {
    }
}
