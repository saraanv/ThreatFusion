using MediatR;

namespace ThreatFusion.Threat.Application
    .Features.ThreatRelations.AutoCorrelate;

public sealed record AutoCorrelateThreatIndicatorCommand(
    long IndicatorId)
    : IRequest<int>;