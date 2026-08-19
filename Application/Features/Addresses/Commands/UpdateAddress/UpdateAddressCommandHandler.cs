
using Application.Common.Interfaces;
using Azure.Identity;
using Domain.Common;
using Domain.Entities.Addresses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Addresses.Commands.UpdateAddress
{
    public sealed class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, Result>
    {
        private readonly IAppDbContext _context;

        public UpdateAddressCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
        {
            var customer = await _context.Customers.FirstAsync(x => x.UserId == request.CustomerId, cancellationToken);

            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.CustomerId == customer.Id
                                        && a.Id == request.addressId);

            if(address is null)
                return Result.Failure(AddressErrors.NotFound);

            if (!string.IsNullOrWhiteSpace(request.City))
                address.city = request.City;

            if (!string.IsNullOrWhiteSpace(request.Country))
                address.country = request.Country;

            if (!string.IsNullOrWhiteSpace(request.Area))
                address.Area = request.Area;

            if (!string.IsNullOrWhiteSpace(request.State))
                address.state = request.State;

            if (!string.IsNullOrWhiteSpace(request.PostalCode))
                address.PostalCode = request.PostalCode;

            if (!string.IsNullOrWhiteSpace(request.Province))
                address.Province = request.Province;

            if (!string.IsNullOrWhiteSpace(request.Street))
                address.StreetBlock = request.Street;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
