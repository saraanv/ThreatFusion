using MediatR;

namespace ThreatFusion.Threat.Application.Features.ThreatRelations.GetGraph;

public sealed record GetThreatGraphQuery(
    long IndicatorId)
    : IRequest<ThreatGraphDto>;