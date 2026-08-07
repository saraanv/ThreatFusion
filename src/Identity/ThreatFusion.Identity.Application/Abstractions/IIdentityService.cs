using ThreatFusion.Identity.Application.Common.Models;

namespace ThreatFusion.Identity.Application.Abstractions;

public interface IIdentityService
{
    Task<RegisterUserResult> RegisterAsync(string firstName, string lastName, string email, string password, CancellationToken cancellationToken);

    Task<LoginUserResult> LoginAsync(string email, string password, CancellationToken cancellationToken);
}