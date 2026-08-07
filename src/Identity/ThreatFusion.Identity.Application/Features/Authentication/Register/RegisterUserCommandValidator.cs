using FluentValidation;

namespace ThreatFusion.Identity.Application
    .Features.Authentication.Register;

public sealed class RegisterUserCommandValidator
    : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(command => command.FirstName)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100);

        RuleFor(command => command.LastName)
            .NotEmpty()
            .WithMessage("LastName is required.")
            .MaximumLength(100);

        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8);
    }
}