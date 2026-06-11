using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class APIController : ControllerBase
    {
        protected readonly IMediator _mediator;

        protected APIController(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected IActionResult HandleFailure(Result result) =>
            result switch
            {
                { IsSuccess: true } => throw new InvalidOperationException(),
                IValidationResult validationResult => BadRequest(
                    CreateProblemDetails(
                        title: "Validation Error",
                        status: StatusCodes.Status400BadRequest,
                        error: result.Error,
                        errors: validationResult.Errors)),

                { Error : { Type: var type } } when type == ErrorType.NotFound => NotFound(
                    CreateProblemDetails(
                        title: "Not Found Error",
                        status: StatusCodes.Status404NotFound,
                        error: result.Error)),

                _ => BadRequest(
                    CreateProblemDetails(
                        title: "Validation Error",
                        status: StatusCodes.Status400BadRequest,
                        error: result.Error))
            };

        
        private static ProblemDetails CreateProblemDetails(
            string title,
            int status,
            Error error,
            Error[]? errors = null) =>
            new()
            {
                Title = title,
                Status = status,
                Detail = error.Description,
                Type = error.Code,
                Extensions = { { nameof(errors), errors } }
            };
    }
}
