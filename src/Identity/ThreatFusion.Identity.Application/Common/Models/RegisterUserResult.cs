namespace ThreatFusion.Identity.Application.Common.Models;

public sealed record RegisterUserResult(
    bool IsSuccess,
    long? UserId,
    IReadOnlyCollection<string> Errors)
{
    public static RegisterUserResult Success(long userId)
    {
        return new RegisterUserResult(
            true,
            userId,
            Array.Empty<string>());
    }

    public static RegisterUserResult Failure(
        IEnumerable<string> errors)
    {
        return new RegisterUserResult(
            false,
            null,
            errors.ToArray());
    }
}