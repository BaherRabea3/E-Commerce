
using Application.Common.DTOs.PaymentDTOs;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Payments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payments.Queries.GetPaymentDetails
{
    public sealed class GetPaymentDetailsQueryHandler : IRequestHandler<GetPaymentDetailsQuery, Result<PaymentDetailsDto>>
    {
        private readonly IAppDbContext _context;

        public GetPaymentDetailsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaymentDetailsDto>> Handle(GetPaymentDetailsQuery request, CancellationToken cancellationToken)
        {
            var payment = await _context.Payments
                                    .AsNoTracking()
                                    .Where(p => p.OrderId == request.OrderId
                                             && p.CustomerId == request.CustomerId)
                                    .Select(p => new PaymentDetailsDto
                                    {
                                        PaymentId = p.Id,
                                        Method = p.Method,
                                        Amount = p.Amount,
                                        Status = p.Status.ToString(),
                                        CreatedAt = p.CreatedAt,
                                        CompletedAt = p.PaidAt
                                    })
                                    .FirstOrDefaultAsync(cancellationToken);

            if (payment is null)
                return Result.Failure<PaymentDetailsDto>(PaymentErrors.NotFound);

            return Result.Success(payment);
            
        }
    }
}
