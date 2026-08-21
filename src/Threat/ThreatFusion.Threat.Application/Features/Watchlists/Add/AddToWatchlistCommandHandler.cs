using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;
using ThreatFusion.Threat.Domain.Entities;

namespace ThreatFusion.Threat.Application.Features.Watchlists.Add;

public sealed class AddToWatchlistCommandHandler
    : IRequestHandler<
        AddToWatchlistCommand,
        AddToWatchlistResult>
{
    private readonly IThreatDbContext _dbContext;

    public AddToWatchlistCommandHandler(
        IThreatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AddToWatchlistResult> Handle(
        AddToWatchlistCommand request,
        CancellationToken cancellationToken)
    {
        var indicatorExists =
            await _dbContext.ThreatIndicators
                .AnyAsync(
                    x =>
                        x.Id == request.ThreatIndicatorId &&
                        !x.IsDeleted,
                    cancellationToken);

        if (!indicatorExists)
        {
            return AddToWatchlistResult.Failure(
                "Threat indicator was not found.");
        }

        var existingWatchlist =
            await _dbContext.ThreatWatchlists
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == request.UserId &&
                        x.ThreatIndicatorId ==
                            request.ThreatIndicatorId,
                    cancellationToken);

        if (existingWatchlist is not null)
        {
            if (!existingWatchlist.IsDeleted &&
                existingWatchlist.IsActive)
            {
                return AddToWatchlistResult.Failure(
                    "Indicator is already in your watchlist.");
            }

            existingWatchlist.IsDeleted = false;
            existingWatchlist.IsActive = true;
            existingWatchlist.DeletedAtUtc = null;
            existingWatchlist.Note =
                request.Note?.Trim();

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            return AddToWatchlistResult.Success(
                existingWatchlist.Id);
        }

        var watchlist =
            new ThreatWatchlist
            {
                UserId =
                    request.UserId,

                ThreatIndicatorId =
                    request.ThreatIndicatorId,

                Note =
                    request.Note?.Trim(),

                IsActive = true,

                CreatedAtUtc =
                    DateTime.UtcNow,

                IsDeleted = false
            };

        await _dbContext.ThreatWatchlists
            .AddAsync(
                watchlist,
                cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return AddToWatchlistResult.Success(
            watchlist.Id);
    }
}