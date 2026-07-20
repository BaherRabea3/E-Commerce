using Domain.Common;
using MediatR;

namespace Application.Features.Addresses.Commands.DeleteAddress
{
    public sealed record DeleteAddressCommand(Guid CustomerId ,int AddressId) : IRequest<Result>
    {
    }
}
