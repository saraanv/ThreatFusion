using ThreatFusion.Identity.Application.Common.Models;

namespace ThreatFusion.Identity.Application.Abstractions;

public interface IJwtTokenGenerator
{
    Task<TokenResult> GenerateAsync(
        long userId,
        string email,
        string firstName,
        string lastName,
        IEnumerable<string> roles);
}