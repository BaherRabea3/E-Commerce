
using FluentValidation;

namespace Application.Features.Accounts.Commands.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name can't be null");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name can't be null");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email can't be null")
                .EmailAddress().WithMessage("Invalid Email Address");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password can't be null");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match");
        }
    }
}
