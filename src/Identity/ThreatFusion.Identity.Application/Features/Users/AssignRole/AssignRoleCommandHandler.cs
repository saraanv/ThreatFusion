using MediatR;
using ThreatFusion.Identity.Application.Abstractions;

namespace ThreatFusion.Identity.Application
    .Features.Users.AssignRole;

public sealed class AssignRoleCommandHandler
    : IRequestHandler<
        AssignRoleCommand,
        AssignRoleResult>
{
    private readonly IIdentityService _identityService;

    public AssignRoleCommandHandler(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AssignRoleResult> Handle(
        AssignRoleCommand request,
        CancellationToken cancellationToken)
    {
        return await _identityService.AssignRoleAsync(
            request.UserId,
            request.Role,
            cancellationToken);
    }
}