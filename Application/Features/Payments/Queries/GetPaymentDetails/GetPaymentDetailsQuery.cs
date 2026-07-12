
using Application.Common.DTOs.PaymentDTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Payments.Queries.GetPaymentDetails
{
   public sealed record GetPaymentDetailsQuery(int OrderId , Guid CustomerId) : IRequest<Result<PaymentDetailsDto>>
    {
    }
}
