using MediatR;
using ThreatFusion.Threat.Application.Common.Models;
using ThreatFusion.Threat.Domain.Enums;

namespace ThreatFusion.Threat.Application.Features.ThreatIndicators.GetList;

public sealed record GetThreatIndicatorsQuery(
    IndicatorType? Type,
    ThreatSeverity? Severity,
    string? Source,
    int? MinConfidence,
    DateTime? FromDateUtc,
    DateTime? ToDateUtc,
    int PageNumber = 1,
    int PageSize = 20)
    : IRequest<PagedResult<ThreatIndicatorListItemDto>>;