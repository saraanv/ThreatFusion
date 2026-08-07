namespace ThreatFusion.Identity.Application
    .Features.Users.AssignRole;

public sealed record AssignRoleResult(
    bool IsSuccess,
    IReadOnlyCollection<string> Errors)
{
    public static AssignRoleResult Success() =>
        new(
            true,
            Array.Empty<string>());

    public static AssignRoleResult Failure(
        params string[] errors) =>
        new(
            false,
            errors);
}