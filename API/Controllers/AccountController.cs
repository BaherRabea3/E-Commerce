using API.Requests.Account;
using Application.Features.Accounts.Commands.GenerateJwtToken;
using Application.Features.Accounts.Commands.Login;
using Application.Features.Accounts.Commands.Logout;
using Application.Features.Accounts.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    public class AccountController : APIController
    {
        public AccountController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var response = await _mediator.Send(new RegisterCommand(request.FirstName,
                                                              request.LastName,
                                                              request.Email,
                                                              request.DateOfBirth,
                                                              request.Password,
                                                              request.ConfirmationPassword));

            return response.IsSuccess ? Ok(response.Value) : HandleFailure(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginrRequest request)
        {
            var response = await _mediator.Send(new LoginCommand(request.Email, request.Password));

            return response.IsSuccess ? Ok(response.Value) : HandleFailure(response);
        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            string? Email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(Email))
                return Unauthorized();

            var response = await _mediator.Send(new LogoutCommand(Email));

            return response.IsSuccess ? NoContent() : HandleFailure(response);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> GenerateJwtToken(GenerateJwtTokenRequest request)
        {
            var response = await _mediator.Send(new GenerateJwtTokenCommand(request.Token, request.RefreshToken));

            return response.IsSuccess ? Ok(response.Value) : HandleFailure(response);
        }
    }
}
