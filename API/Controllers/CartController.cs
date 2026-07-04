using Application.Features.Carts.Commands.AddToCart;
using Application.Features.Carts.Commands.ClearCart;
using Application.Features.Carts.Commands.DeleteCartItem;
using Application.Features.Carts.Commands.UpdateCartItem;
using Application.Features.Carts.Queries.GetCart;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CartController : APIController
    {
        public CartController(IMediator mediator) : base(mediator)
        {
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {

            var response = await _mediator.Send(new GetCartQuery(CustomerId));

            return response.IsSuccess ? Ok(response) : HandleFailure(response);
        }

        [HttpPost("items")]
        public async Task<IActionResult> Create(int productId , int quantity)
        {

            var response = await _mediator
                .Send(new AddToCartCommand(productId, quantity, CustomerId));

            return response.IsSuccess ? Ok(response) : HandleFailure(response);
        }

        [HttpPost("items/{productId}")]
        public async Task<IActionResult> Update(int productId, int quantity)
        {

            var response = await _mediator
                .Send(new UpdateCartItemCommand(productId, quantity, CustomerId));

            return response.IsSuccess ? Ok(response) : HandleFailure(response);
        }

        [HttpDelete("items/{id}")]
        public async Task<IActionResult> Delete(int id, int quantity)
        {

            var response = await _mediator
                .Send(new DeleteCartItemCommand(id,  CustomerId));

            return response.IsSuccess ? Ok(response) : HandleFailure(response);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {

            var response = await _mediator
                .Send(new ClearCartCommand(CustomerId));

            return response.IsSuccess ? Ok(response) : HandleFailure(response);
        }

    }
}
