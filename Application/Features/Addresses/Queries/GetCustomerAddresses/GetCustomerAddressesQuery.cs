
using Application.Common.DTOs.AddressDTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Addresses.Queries.GetCustomerAddresses
{
    public sealed record GetCustomerAddressesQuery(Guid CustomerId) : IRequest<Result<List<AddressDto>>>
    {
    }
}
