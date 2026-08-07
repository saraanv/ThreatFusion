using FluentValidation;
using MediatR;
using ThreatFusion.Identity.Application.Abstractions;
using ThreatFusion.Identity.Application.Common.Models;

namespace ThreatFusion.Identity.Application.Features.Authentication.Login;

public sealed class LoginUserCommandHandler
    : IRequestHandler<LoginUserCommand, LoginUserResult>
{
    private readonly IIdentityService _identityService;
    private readonly IValidator<LoginUserCommand> _validator;

    public LoginUserCommandHandler(
        IIdentityService identityService,
        IValidator<LoginUserCommand> validator)
    {
        _identityService = identityService;
        _validator = validator;
    }

    public async Task<LoginUserResult> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult =
            await _validator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            return LoginUserResult.Failure(
                validationResult.Errors
                    .Select(error => error.ErrorMessage)
                    .ToArray());
        }

        return await _identityService.LoginAsync(
            request.Email,
            request.Password,
            cancellationToken);
    }
}