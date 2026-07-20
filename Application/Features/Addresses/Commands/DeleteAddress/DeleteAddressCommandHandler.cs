
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Addresses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Addresses.Commands.DeleteAddress
{
    public sealed class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, Result>
    {
        private readonly IAppDbContext _context;

        public DeleteAddressCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.CustomerId == request.CustomerId
                                        && a.Id == request.AddressId);

            if (address is null)
                return Result.Failure(AddressErrors.NotFound);

            _context.Addresses.Remove(address);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
