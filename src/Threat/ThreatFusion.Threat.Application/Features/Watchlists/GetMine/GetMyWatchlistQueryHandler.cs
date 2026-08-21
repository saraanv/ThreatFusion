using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;

namespace ThreatFusion.Threat.Application.Features.Watchlists.GetMine;

public sealed class GetMyWatchlistQueryHandler
    : IRequestHandler<
        GetMyWatchlistQuery,
        IReadOnlyCollection<WatchlistItemDto>>
{
    private readonly IThreatDbContext _dbContext;

    public GetMyWatchlistQueryHandler(
        IThreatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<WatchlistItemDto>> Handle(
        GetMyWatchlistQuery request,
        CancellationToken cancellationToken)
    {
        var items =
            await _dbContext.ThreatWatchlists
                .AsNoTracking()
                .Where(x =>
                    x.UserId == request.UserId &&
                    x.IsActive &&
                    !x.IsDeleted)
                .Join(
                    _dbContext.ThreatIndicators
                        .AsNoTracking()
                        .Where(x => !x.IsDeleted),

                    watchlist =>
                        watchlist.ThreatIndicatorId,

                    indicator =>
                        indicator.Id,

                    (watchlist, indicator) =>
                        new
                        {
                            Watchlist = watchlist,
                            Indicator = indicator
                        })
                .OrderByDescending(x =>
                    x.Watchlist.CreatedAtUtc)
                .ToListAsync(
                    cancellationToken);

        var result =
            items
                .Select(x =>
                    new WatchlistItemDto(
                        x.Watchlist.Id,
                        x.Indicator.Id,
                        x.Indicator.Type.ToString(),
                        x.Indicator.Value,
                        x.Indicator.Severity.ToString(),
                        x.Indicator.RiskScore,
                        x.Indicator.RiskLevel.ToString(),
                        x.Indicator.SourceName,
                        x.Watchlist.Note,
                        x.Watchlist.CreatedAtUtc))
                .ToList();

        return result;
    }
}