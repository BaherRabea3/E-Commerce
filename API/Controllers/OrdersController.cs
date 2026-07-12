using API.Requests.Orders;
using Application.Features.Orders.Commands.CancelOrder;
using Application.Features.Orders.Commands.PlaceOrder;
using Application.Features.Orders.Queries.GetOrderById;
using Application.Features.Orders.Queries.GetOrdersByCustomerId;
using Application.Features.Payments.Queries.GetPaymentDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class OrdersController : APIController
    {
        public OrdersController(IMediator mediator) : base(mediator) {  }



        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]GetCustomerOrdersRequest request)
        {
            var response = await _mediator
                .Send(new GetOrderQuery(CustomerId,
                                        request.page,
                                        request.pageSize,
                                        request.Status,
                                        request.From,
                                        request.To));

            return response.IsSuccess ? Ok(response) : HandleFailure(response);
        }

        [HttpGet("{id:int}", Name = "GetOrderById")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _mediator
                .Send(new GetOrderByIdQuery(id, CustomerId));

            return response.IsSuccess ? Ok(response) : HandleFailure(response);
        }

        [HttpGet("{id:int}/payment")]
        public async Task<IActionResult> GetPayment(int OrderId)
        {
            var response = await _mediator.Send(new GetPaymentDetailsQuery(OrderId, CustomerId));

            return response.IsSuccess ? Ok(response) : HandleFailure(response);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(PlaceOrderRequest request)
        {
            var response = await _mediator
                .Send(new PlaceOrderCommand(CustomerId, request.AddressId, request.IdempotencyKey, request.PaymentMethod));

            return response.IsSuccess 
                ? CreatedAtRoute("GetOrderById", new {response.Value.OrderId}, response) 
                : HandleFailure(response);
        }

        [HttpPatch("{id:int}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var response = await _mediator
                .Send(new CancelOrderCommand(id, CustomerId));

            return response.IsSuccess ? Ok(response) : HandleFailure(response);
        }
    }
}
