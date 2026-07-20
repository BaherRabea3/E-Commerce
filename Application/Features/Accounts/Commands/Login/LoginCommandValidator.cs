
using FluentValidation;

namespace Application.Features.Accounts.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email cannot be empty")
                .EmailAddress().WithMessage("Email must be a proper email address");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password cannot be empty");

        }
    }
}
