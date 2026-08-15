using MediatR;

namespace ThreatFusion.Threat.Application.Features.ThreatIndicators.GetById;

public sealed record GetThreatIndicatorByIdQuery(
    long Id)
    : IRequest<ThreatIndicatorDetailsDto?>;