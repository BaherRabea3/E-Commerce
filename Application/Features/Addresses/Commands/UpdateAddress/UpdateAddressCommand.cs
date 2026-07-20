
using Domain.Common;
using MediatR;

namespace Application.Features.Addresses.Commands.UpdateAddress
{
    public sealed record UpdateAddressCommand(Guid CustomerId, int addressId ,
        string? State,
        string? PostalCode,
        string? HouseNo,
        string? Street,
        string? Area,
        string?  Province,
        string? City,
        string? Country) : IRequest<Result>
    {
    }
}
