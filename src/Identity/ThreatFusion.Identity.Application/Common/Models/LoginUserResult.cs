namespace ThreatFusion.Identity.Application.Common.Models;

public sealed record LoginUserResult(
    bool IsSuccess,
    string? AccessToken,
    DateTime? ExpiresAtUtc,
    long? UserId,
    string? FirstName,
    string? LastName,
    string? Email,
    IReadOnlyCollection<string> Errors)
{
    public static LoginUserResult Success(
        string accessToken,
        DateTime expiresAtUtc,
        long userId,
        string firstName,
        string lastName,
        string email)
    {
        return new LoginUserResult(
            true,
            accessToken,
            expiresAtUtc,
            userId,
            firstName,
            lastName,
            email,
            Array.Empty<string>());
    }

    public static LoginUserResult Failure(
        params string[] errors)
    {
        return new LoginUserResult(
            false,
            null,
            null,
            null,
            null,
            null,
            null,
            errors);
    }
}