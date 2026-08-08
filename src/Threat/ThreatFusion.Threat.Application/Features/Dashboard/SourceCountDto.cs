namespace ThreatFusion.Threat.Application.Features.Dashboard;

public sealed record SourceCountDto(
    string Source,
    int Count);