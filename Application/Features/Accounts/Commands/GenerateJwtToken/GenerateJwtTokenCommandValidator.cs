
using FluentValidation;

namespace Application.Features.Accounts.Commands.GenerateJwtToken
{
    public class GenerateJwtTokenCommandValidator : AbstractValidator<GenerateJwtTokenCommand>
    {
        public GenerateJwtTokenCommandValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty();

            RuleFor(x => x.RefreshToken)
                .NotEmpty();
        }
    }
}
