using MediatR;
using ThreatFusion.Threat.Domain.Enums;

namespace ThreatFusion.Threat.Application.Features.ThreatRelations.GetGraph;

public sealed record GetThreatGraphQuery(
    long IndicatorId,
    int Depth = 1,
    ThreatRelationType? RelationType = null,
    bool? IsAutomatic = null,
    double? MinRiskScore = null)
    : IRequest<ThreatGraphDto>;