using FluentValidation;
using MediatR;
using ThreatFusion.Identity.Application.Abstractions;
using ThreatFusion.Identity.Application.Common.Models;

namespace ThreatFusion.Identity.Application
    .Features.Authentication.Register;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IIdentityService _identityService;
    private readonly IValidator<RegisterUserCommand> _validator;

    public RegisterUserCommandHandler(IIdentityService identityService, IValidator<RegisterUserCommand> validator)
    {
        _identityService = identityService;
        _validator = validator;
    }

    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return RegisterUserResult.Failure(validationResult.Errors.Select(error => error.ErrorMessage));
        }

        return await _identityService.RegisterAsync(request.FirstName, request.LastName, request.Email, request.Password, cancellationToken);
    }
}