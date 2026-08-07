using MediatR;

namespace ThreatFusion.Threat.Application.Features.ThreatIndicators.Search;

public sealed record SearchThreatIndicatorQuery(
    string Value)
    : IRequest<IReadOnlyCollection<ThreatIndicatorDto>>;