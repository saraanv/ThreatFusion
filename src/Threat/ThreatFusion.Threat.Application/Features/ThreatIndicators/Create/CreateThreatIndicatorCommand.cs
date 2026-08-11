using MediatR;
using ThreatFusion.Threat.Domain.Enums;

namespace ThreatFusion.Threat.Application.Features.ThreatIndicators.Create;

public sealed record CreateThreatIndicatorCommand(
    IndicatorType Type,
    string Value,
    ThreatSeverity Severity,
    int Confidence,
    string SourceName,
    string? Description,
    DateTime? FirstSeenUtc,
    DateTime? LastSeenUtc,
    double? CvssScore,
    string? CvssVersion,
    string? CvssVector,
    string? CweId,
    string? ReferenceUrl)
    : IRequest<CreateThreatIndicatorResult>;