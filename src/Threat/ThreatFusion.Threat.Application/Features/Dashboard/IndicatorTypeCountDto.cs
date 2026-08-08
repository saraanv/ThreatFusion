namespace ThreatFusion.Threat.Application.Features.Dashboard;

public sealed record IndicatorTypeCountDto(
    string Type,
    int Count);