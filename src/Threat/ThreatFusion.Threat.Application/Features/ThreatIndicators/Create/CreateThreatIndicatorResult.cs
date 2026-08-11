using ThreatFusion.Threat.Domain.Enums;

namespace ThreatFusion.Threat.Application.Features.ThreatIndicators.Create;

public sealed record CreateThreatIndicatorResult(
    bool IsSuccess,
    long? IndicatorId,
    ThreatIndicatorWriteStatus? Status,
    IReadOnlyCollection<string> Errors)
{
    public static CreateThreatIndicatorResult Created(long id) =>
        new(
            true,
            id,
            ThreatIndicatorWriteStatus.Created,
            Array.Empty<string>());

    public static CreateThreatIndicatorResult Updated(long id) =>
        new(
            true,
            id,
            ThreatIndicatorWriteStatus.Updated,
            Array.Empty<string>());

    public static CreateThreatIndicatorResult Unchanged(long id) =>
        new(
            true,
            id,
            ThreatIndicatorWriteStatus.Unchanged,
            Array.Empty<string>());

    public static CreateThreatIndicatorResult Failure(
        params string[] errors) =>
        new(
            false,
            null,
            null,
            errors);
}