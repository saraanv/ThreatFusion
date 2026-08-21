using MediatR;

namespace ThreatFusion.Threat.Application.Features.Watchlists.Remove;

public sealed record RemoveFromWatchlistCommand(
    long UserId,
    long ThreatIndicatorId)
    : IRequest<bool>;