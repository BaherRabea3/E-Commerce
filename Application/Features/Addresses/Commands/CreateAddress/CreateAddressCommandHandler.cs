
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Addresses;
using MediatR;

namespace Application.Features.Addresses.Commands.CreateAddress
{
    public sealed class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, Result>
    {
        private readonly IAppDbContext _context;

        public CreateAddressCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
        {
            var address = new Address()
            {
                 Area = request.Area,
                 city = request.City,
                 state = request.State,
                 country = request.Country,
                 CustomerId = request.CustomerId,
                 HouseNo = request.HouseNo,
                 PostalCode = request.PostalCode,
                 StreetBlock =  request.Street,
                 Province = request.Province,
            };

            _context.Addresses.Add(address);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
