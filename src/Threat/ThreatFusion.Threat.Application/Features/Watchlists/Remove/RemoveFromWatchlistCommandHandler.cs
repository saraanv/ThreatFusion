using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;

namespace ThreatFusion.Threat.Application.Features.Watchlists.Remove;

public sealed class RemoveFromWatchlistCommandHandler
    : IRequestHandler<
        RemoveFromWatchlistCommand,
        bool>
{
    private readonly IThreatDbContext _dbContext;

    public RemoveFromWatchlistCommandHandler(
        IThreatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(
        RemoveFromWatchlistCommand request,
        CancellationToken cancellationToken)
    {
        var watchlist =
            await _dbContext.ThreatWatchlists
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == request.UserId &&
                        x.ThreatIndicatorId ==
                        request.ThreatIndicatorId &&
                        !x.IsDeleted,
                    cancellationToken);

        if (watchlist is null)
        {
            return false;
        }

        watchlist.IsActive = false;
        watchlist.IsDeleted = true;
        watchlist.DeletedAtUtc =
            DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}