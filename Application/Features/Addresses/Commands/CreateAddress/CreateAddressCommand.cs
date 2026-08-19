
using Domain.Common;
using MediatR;

namespace Application.Features.Addresses.Commands.CreateAddress
{
    public sealed record CreateAddressCommand(int CustomerId, string State,
      string PostalCode,
      string HouseNo,
      string Street,
      string Area,
      string Province,
      string City,
      string Country): IRequest<Result>
    {

    }
}