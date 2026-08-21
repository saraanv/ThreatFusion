using MediatR;

namespace ThreatFusion.Threat.Application.Features.ThreatRelations.GetGraph;

public sealed record GetThreatGraphQuery(
    long IndicatorId,
    int Depth = 1)
    : IRequest<ThreatGraphDto>;