using MediatR;

namespace ThreatFusion.Threat.Application.Features.Watchlists.GetMine;

public sealed record GetMyWatchlistQuery(
    long UserId)
    : IRequest<IReadOnlyCollection<WatchlistItemDto>>;