
using Application.Common.DTOs.PaymentDTOs;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Payments;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Payments.Commands.UpdatePaymentStatus
{
    public sealed class UpdatePaymentStatusCommandHandler : IRequestHandler<UpdatePaymentStatusCommand, Result<UpdatePaymentStatusResponseDto>>
    {
        private readonly IAppDbContext _context;
        private readonly ILogger<UpdatePaymentStatusCommandHandler> _logger;
        public UpdatePaymentStatusCommandHandler(IAppDbContext context, ILogger<UpdatePaymentStatusCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<UpdatePaymentStatusResponseDto>> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(x => x.Id == request.PaymentId);

            if (payment is null)
                return Result.Failure<UpdatePaymentStatusResponseDto>(PaymentErrors.NotFound);

            if(!payment.CanTransitionTo(request.NewStatus))
                return Result
                    .Failure<UpdatePaymentStatusResponseDto>(PaymentErrors.InvalidStatusTransition(payment.Status, request.NewStatus));

            var previousStatus = payment.Status;
            payment.Status = request.NewStatus;

            if (request.NewStatus == PaymentStatus.Completed)
                payment.PaidAt = DateTime.UtcNow;

            _logger.LogWarning(
               "Admin manually overrode Payment {PaymentId} status from {From} to {To}",
               payment.Id, previousStatus, request.NewStatus);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(new UpdatePaymentStatusResponseDto
            {
                PaymentId = payment.Id,
                OrderId = payment.OrderId,
                PreviousStatus = previousStatus.ToString(),
                NewStatus = payment.Status.ToString(),
                UpdatedAt = DateTime.UtcNow
            });
        }
    }
}
