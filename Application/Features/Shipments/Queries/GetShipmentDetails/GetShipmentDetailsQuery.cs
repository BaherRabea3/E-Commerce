
using Application.Common.DTOs.ShipmentDTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Shipments.Queries.GetShipmentDetails
{
    public sealed record GetShipmentDetailsQuery(int OrderId) : IRequest<Result<ShipmentDetailsDto>>
    {
    }
}
