using MediatR;

namespace ThreatFusion.Threat.Application.Features.Watchlists.Add;

public sealed record AddToWatchlistCommand(
    long UserId,
    long ThreatIndicatorId,
    string? Note)
    : IRequest<AddToWatchlistResult>;