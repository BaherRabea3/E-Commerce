using API.Requests.Categories;
using Application.Features.Categories.Commands.CreateCategory;
using Application.Features.Categories.Commands.DeleteCategory;
using Application.Features.Categories.Commands.UpdateCategory;
using Application.Features.Categories.Queries.GetCategories;
using Application.Features.Categories.Queries.GetCategoryByid;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CategoryController : APIController
    {
        public CategoryController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _mediator.Send(new GetCategoriesQuery());

            return result.IsSuccess ? Ok(result) : HandleFailure(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetCategoryByIdQuery(id));

            return result.IsSuccess ? Ok(result) : HandleFailure(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryRequest request)
        {
            var result = await _mediator
                                .Send(new CreateCategoryCommand(request.name, request.description));

            return result.IsSuccess ? CreatedAtAction(nameof(GetById) , new {id = result.Value},null) : HandleFailure(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id , UpdateCategoryRequest request)
        {
            var result = await _mediator
                                .Send(new UpdateCategoryCommand(id, request.name, request.description));

            return result.IsSuccess ? NoContent() : HandleFailure(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator
                                .Send(new DeleteCategoryCommand(id));

            return result.IsSuccess ? NoContent() : HandleFailure(result);
        }
    }
}
