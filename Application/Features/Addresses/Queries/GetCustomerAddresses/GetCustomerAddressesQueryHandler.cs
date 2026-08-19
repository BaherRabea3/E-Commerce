
using Application.Common.DTOs.AddressDTOs;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Addresses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Addresses.Queries.GetCustomerAddresses
{
    public sealed class GetCustomerAddressesQueryHandler : IRequestHandler<GetCustomerAddressesQuery, Result<List<AddressDto>>>
    {
        private readonly IAppDbContext _context;

        public GetCustomerAddressesQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<AddressDto>>> Handle(GetCustomerAddressesQuery request, CancellationToken cancellationToken)
        {
            var customer = await _context.Customers.FirstAsync(x => x.UserId == request.CustomerId, cancellationToken);


            bool existed = await _context.Addresses
                .AnyAsync(a => a.CustomerId == customer.Id, cancellationToken);

            if(!existed)
                return Result.Failure<List<AddressDto>>(AddressErrors.NotFound);

            var AddressList = await _context.Addresses
                                      .AsNoTracking()
                                      .Where(a => a.CustomerId == customer.Id)
                                      .Select(a => new AddressDto
                                      {
                                          AddressId = a.Id,
                                          Area = a.Area,
                                          City = a.city,
                                          Country = a.country,
                                          PostalCode = a.PostalCode,
                                          State = a.state,
                                          StreetBlock = a.StreetBlock,
                                          HouseNo = a.HouseNo
                                      })
                                      .ToListAsync(cancellationToken);

            return Result.Success(AddressList);


        }
    }
}
