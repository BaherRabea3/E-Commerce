using Application.Features.Carts.Commands.AddToCart;
using Application.Features.Carts.Commands.ClearCart;
using Application.Features.Carts.Commands.DeleteCartItem;
using Application.Features.Carts.Commands.UpdateCartItem;
using Application.Features.Carts.Queries.GetCart;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    public class CartController : APIController
    {
        public CartController(IMediator mediator) : base(mediator)
        {
        }

        public Guid GetCustomerId
            => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> Get()
        {

            var response = await _mediator.Send(new GetCartQuery(GetCustomerId));

            return response.IsSuccess ? Ok(response) : HandleFailure(response);
        }

        [HttpPost("items")]
        public async Task<IActionResult> Create(int productId , int quantity)
        {

            var response = await _mediator
                .Send(new AddToCartCommand(productId, quantity, GetCustomerId));

            return response.IsSuccess ? Ok(response) : HandleFailure(response);
        }

        [HttpPost("items/{productId}")]
        public async Task<IActionResult> Update(int productId, int quantity)
        {

            var response = await _mediator
                .Send(new UpdateCartItemCommand(productId, quantity, GetCustomerId));

            return response.IsSuccess ? Ok(response) : HandleFailure(response);
        }

        [HttpDelete("items/{id}")]
        public async Task<IActionResult> Delete(int id, int quantity)
        {

            var response = await _mediator
                .Send(new DeleteCartItemCommand(id,  GetCustomerId));

            return response.IsSuccess ? Ok(response) : HandleFailure(response);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {

            var response = await _mediator
                .Send(new ClearCartCommand(GetCustomerId));

            return response.IsSuccess ? Ok(response) : HandleFailure(response);
        }

    }
}
