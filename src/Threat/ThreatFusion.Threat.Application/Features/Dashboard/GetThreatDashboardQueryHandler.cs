using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;
using ThreatFusion.Threat.Domain.Enums;

namespace ThreatFusion.Threat.Application.Features.Dashboard;

public sealed class GetThreatDashboardQueryHandler
    : IRequestHandler<GetThreatDashboardQuery, ThreatDashboardDto>
{
    private readonly IThreatDbContext _dbContext;

    public GetThreatDashboardQueryHandler(
        IThreatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ThreatDashboardDto> Handle(
        GetThreatDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var totalIndicators =
            await _dbContext.ThreatIndicators
                .AsNoTracking()
                .CountAsync(cancellationToken);

        var activeIndicators =
            await _dbContext.ThreatIndicators
                .AsNoTracking()
                .CountAsync(
                    x => x.IsActive,
                    cancellationToken);

        var criticalIndicators =
            await _dbContext.ThreatIndicators
                .AsNoTracking()
                .CountAsync(
                    x => x.Severity == ThreatSeverity.Critical,
                    cancellationToken);

        var indicatorsByType =
            await _dbContext.ThreatIndicators
                .AsNoTracking()
                .GroupBy(x => x.Type)
                .Select(group => new IndicatorTypeCountDto(
                    group.Key.ToString(),
                    group.Count()))
                .ToListAsync(cancellationToken);

        var sourceCounts =
            await _dbContext.ThreatIndicators
                .AsNoTracking()
                .GroupBy(x => x.SourceName)
                .Select(group => new
                {
                    Source = group.Key,
                    Count = group.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync(cancellationToken);

        var indicatorsBySource =
            sourceCounts
                .Select(x => new SourceCountDto(
                    x.Source,
                    x.Count))
                .ToList();

        var latestThreats =
            await _dbContext.ThreatIndicators
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(10)
                .Select(x => new LatestThreatDto(
                    x.Id,
                    x.Type.ToString(),
                    x.Value,
                    x.Severity.ToString(),
                    x.Confidence,
                    x.SourceName,
                    x.FirstSeenUtc))
                .ToListAsync(cancellationToken);

        var lastFeedSync =
            await _dbContext.ThreatFeedSyncs
                .AsNoTracking()
                .OrderByDescending(x => x.StartedAtUtc)
                .Select(x => new FeedSyncSummaryDto(
                    x.FeedName,
                    x.StartedAtUtc,
                    x.CompletedAtUtc,
                    x.TotalFetched,
                    x.CreatedCount,
                    x.UpdatedCount,
                    x.UnchangedCount,
                    x.FailedCount,
                    x.IsSuccessful))
                .FirstOrDefaultAsync(cancellationToken);

        return new ThreatDashboardDto(
            totalIndicators,
            activeIndicators,
            criticalIndicators,
            indicatorsByType,
            indicatorsBySource,
            latestThreats,
            lastFeedSync);
    }
}