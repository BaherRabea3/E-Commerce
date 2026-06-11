using API.Requests.Products;
using Application.Features.Products.Commands.CreateProduct;
using Application.Features.Products.Commands.DeleteProduct;
using Application.Features.Products.Commands.UpdateProduct;
using Application.Features.Products.Commands.UpdateProductStock;
using Application.Features.Products.Queries.GetProductById;
using Application.Features.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProductsController : APIController
    {
        public ProductsController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get(GetProductsRequest request)
        {
            var result = await _mediator
                .Send(new GetProductsQuery(request.categoryId,
                                           request.price,
                                           request.search,
                                           request.page,
                                           request.pageSize));

            return result.IsSuccess ? Ok(result) : HandleFailure(result);
        }

        [HttpGet("{id:int}",Name = "GetProductById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(id));

            return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductRequest request)
        {
            var result = await _mediator
                .Send(new CreateProductCommand(request.name,
                                               request.description,
                                               request.unitPrice,
                                               request.quantity,
                                               request.Image,
                                               request.categoryId));

            return result.IsSuccess ? CreatedAtRoute("GetProductById", new { id = result.Value }, null)
                : HandleFailure(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id , UpdateProductRequest request)
        {
             var result = await _mediator.Send(new UpdateProductCommand(id,
                                               request.name,
                                               request.description,
                                               request.unitPrice,
                                               request.quantity,
                                               request.Image,
                                               request.categoryId));

            return result.IsSuccess ? NoContent() : HandleFailure(result);
        }

        [HttpPatch("{id:int}/{stock:int}")]
        public async Task<IActionResult> Update(int id, int stock)
        {
            var result = await _mediator.Send(new UpdateProductStockCommand(id,stock));

            return result.IsSuccess ? NoContent() : HandleFailure(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteProductCommand(id));

            return result.IsSuccess ? NoContent() : HandleFailure(result);
        }
    }
}
