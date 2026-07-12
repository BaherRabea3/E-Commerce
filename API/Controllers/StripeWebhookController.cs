using Application.Features.Payments.Commands.ProcessStripeWebhook;
using Infrastructure.Services.PaymentServices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace API.Controllers
{
    [Route("api/webhooks/stripe")]
    [AllowAnonymous]
    public class StripeWebhookController : APIController
    {
        private readonly IOptions<StripeSettings> _options;
        public StripeWebhookController(IMediator mediator, IOptions<StripeSettings> options) : base(mediator)
        {
            _options = options;
        }

        [HttpPost]
        public async Task<IActionResult> Handle()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(json,
                                                         Request.Headers["Stripe-Signature"],
                                                         _options.Value.WebhookSecret);

            await _mediator
                .Send(new ProcessStripeWebhookCommand(stripeEvent.Id, stripeEvent.Type, json));

            return Ok();
        }
    }
}
