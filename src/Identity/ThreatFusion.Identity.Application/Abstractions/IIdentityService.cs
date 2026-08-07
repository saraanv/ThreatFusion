using ThreatFusion.Identity.Application.Common.Models;
using ThreatFusion.Identity.Application.Features.Users.AssignRole;

namespace ThreatFusion.Identity.Application.Abstractions;

public interface IIdentityService
{
    Task<RegisterUserResult> RegisterAsync(string firstName, string lastName, string email, string password, CancellationToken cancellationToken);

    Task<LoginUserResult> LoginAsync(string email, string password, CancellationToken cancellationToken);
    Task<AssignRoleResult> AssignRoleAsync(
        long userId,
        string role,
        CancellationToken cancellationToken);
}