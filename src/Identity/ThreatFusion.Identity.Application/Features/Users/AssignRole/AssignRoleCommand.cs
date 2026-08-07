using MediatR;

namespace ThreatFusion.Identity.Application
    .Features.Users.AssignRole;

public sealed record AssignRoleCommand(
    long UserId,
    string Role)
    : IRequest<AssignRoleResult>;