using API.Requests.Orders;
using Application.Features.Orders.Commands.UpdateOrderStatus;
using Application.Features.Orders.Queries.GetAllOrders;
using Application.Features.Payments.Commands.RefundPayment;
using Application.Features.Payments.Commands.UpdatePaymentStatus;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : APIController
    {
        public AdminController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders([FromQuery] GetAllOrderRequest request)
        {
            var response = await _mediator.Send(new GetAllOrdersQuery(request.Page, request.PageSize
                                                                , request.Status, request.PaymentStatus
                                                                , request.From, request.To
                                                                , request.CustomerEmail
                                                                , request.MinTotal, request.MaxTotal));

            return response.IsSuccess ? Ok(response) : HandleFailure(response);
        }

        [HttpPatch("orders/{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderStatus orderStatus)
        {
            var response = await _mediator.Send(new UpdateOrderStatusCommand(id, orderStatus));

            return response.IsSuccess ? Ok(response) : HandleFailure(response);
        }

        [HttpPatch("{paymentId}/refund")]
        public async Task<IActionResult> RefundPayment(
         int paymentId)
        {
            var response = await _mediator.Send(new RefundPaymentCommand(paymentId));

            return response.IsSuccess ? Ok(response) : HandleFailure(response);
        }

        [HttpPatch("{paymentId}/status")]
        public async Task<IActionResult> UpdatePaymentStatus(
        int paymentId,
        [FromBody] PaymentStatus newStatus,
        CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(
                new UpdatePaymentStatusCommand(paymentId, newStatus),
                cancellationToken);

            return response.IsSuccess ? Ok(response) : HandleFailure(response);
        }
    }
}
