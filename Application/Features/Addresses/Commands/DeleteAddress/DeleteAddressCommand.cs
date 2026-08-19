using Domain.Common;
using MediatR;

namespace Application.Features.Addresses.Commands.DeleteAddress
{
    public sealed record DeleteAddressCommand(int CustomerId ,int AddressId) : IRequest<Result>
    {
    }
}
