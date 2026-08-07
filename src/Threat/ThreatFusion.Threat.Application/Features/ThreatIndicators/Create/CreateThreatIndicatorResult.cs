namespace ThreatFusion.Threat.Application.Features.ThreatIndicators.Create;

public sealed record CreateThreatIndicatorResult(
    bool IsSuccess,
    long? IndicatorId,
    IReadOnlyCollection<string> Errors)
{
    public static CreateThreatIndicatorResult Success(long id) =>
        new(
            true,
            id,
            Array.Empty<string>());

    public static CreateThreatIndicatorResult Failure(
        params string[] errors) =>
        new(
            false,
            null,
            errors);
}