using System.Security.Claims;

namespace ThreatFusion.Threat.API.Services;

public sealed class CurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long GetUserId()
    {
        var user =
            _httpContextAccessor
                .HttpContext?
                .User;

        var userIdClaim =
            user?.FindFirst("sub")
                ?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim) ||
            !long.TryParse(
                userIdClaim,
                out var userId))
        {
            throw new UnauthorizedAccessException(
                "User id was not found in access token.");
        }

        return userId;
    }
}