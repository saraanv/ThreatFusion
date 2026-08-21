using MediatR;

namespace ThreatFusion.Threat.Application.Features.ThreatRelations.GetByIndicator;

public sealed record GetThreatRelationsByIndicatorQuery(
    long IndicatorId)
    : IRequest<IReadOnlyCollection<ThreatRelationDto>>;