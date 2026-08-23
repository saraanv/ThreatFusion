using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;
using ThreatFusion.Threat.Domain.Enums;

namespace ThreatFusion.Threat.Application.Features.Dashboard.GetOverview;

public sealed class GetDashboardOverviewQueryHandler
    : IRequestHandler<
        GetDashboardOverviewQuery,
        DashboardOverviewDto>
{
    private readonly IThreatDbContext _dbContext;

    public GetDashboardOverviewQueryHandler(
        IThreatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardOverviewDto> Handle(
        GetDashboardOverviewQuery request,
        CancellationToken cancellationToken)
    {
        /*
         * ==========================================
         * INDICATOR STATISTICS
         * ==========================================
         */

        var indicatorStats =
            await _dbContext.ThreatIndicators
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.IsActive)
                .GroupBy(x => 1)
                .Select(g => new
                {
                    Total =
                        g.Count(),

                    Critical =
                        g.Count(x =>
                            x.RiskLevel ==
                            ThreatRiskLevel.Critical),

                    High =
                        g.Count(x =>
                            x.RiskLevel ==
                            ThreatRiskLevel.High)
                })
                .FirstOrDefaultAsync(
                    cancellationToken);

        var totalIndicators =
            indicatorStats?.Total ?? 0;

        var criticalIndicators =
            indicatorStats?.Critical ?? 0;

        var highRiskIndicators =
            indicatorStats?.High ?? 0;

        /*
         * ==========================================
         * USER WATCHLIST
         * ==========================================
         */

        var watchedIndicators =
            await _dbContext.ThreatWatchlists
                .AsNoTracking()
                .CountAsync(
                    x =>
                        x.UserId == request.UserId &&
                        x.IsActive &&
                        !x.IsDeleted,
                    cancellationToken);

        /*
         * ==========================================
         * USER ALERTS
         * ==========================================
         */

        var unreadAlerts =
            await _dbContext.ThreatAlerts
                .AsNoTracking()
                .CountAsync(
                    x =>
                        x.UserId == request.UserId &&
                        !x.IsRead &&
                        !x.IsDeleted,
                    cancellationToken);

        /*
         * ==========================================
         * RELATION STATISTICS
         * ==========================================
         */

        var relationStats =
            await _dbContext.ThreatIndicatorRelations
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.IsActive)
                .GroupBy(x => 1)
                .Select(g => new
                {
                    Automatic =
                        g.Count(x =>
                            x.IsAutomatic),

                    Manual =
                        g.Count(x =>
                            !x.IsAutomatic)
                })
                .FirstOrDefaultAsync(
                    cancellationToken);

        var automaticRelations =
            relationStats?.Automatic ?? 0;

        var manualRelations =
            relationStats?.Manual ?? 0;

        /*
         * ==========================================
         * TOP RISKY INDICATORS
         * ==========================================
         */

        var riskyIndicatorRows =
            await _dbContext.ThreatIndicators
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.IsActive)
                .OrderByDescending(x =>
                    x.RiskScore)
                .ThenByDescending(x =>
                    x.Id)
                .Take(5)
                .Select(x => new
                {
                    x.Id,
                    x.Type,
                    x.Value,
                    x.Severity,
                    x.RiskScore,
                    x.RiskLevel,
                    x.SourceName
                })
                .ToListAsync(
                    cancellationToken);

        var topRiskyIndicators =
            riskyIndicatorRows
                .Select(x =>
                    new DashboardRiskyIndicatorDto(
                        x.Id,
                        x.Type.ToString(),
                        x.Value,
                        x.Severity.ToString(),
                        x.RiskScore,
                        x.RiskLevel.ToString(),
                        x.SourceName))
                .ToList();

        /*
         * ==========================================
         * RECENT ALERTS
         * ==========================================
         *
         * عمداً Join مستقیم نمی‌زنیم.
         * اول فقط 5 Alert آخر را می‌گیریم،
         * بعد Indicatorهای مرتبط را جدا می‌خوانیم.
         */

        var recentAlertsRaw =
            await _dbContext.ThreatAlerts
                .AsNoTracking()
                .Where(x =>
                    x.UserId == request.UserId &&
                    !x.IsDeleted)
                .OrderByDescending(x =>
                    x.CreatedAtUtc)
                .ThenByDescending(x =>
                    x.Id)
                .Take(5)
                .ToListAsync(
                    cancellationToken);

        var recentIndicatorIds =
            recentAlertsRaw
                .Select(x =>
                    x.ThreatIndicatorId)
                .Distinct()
                .ToList();

        var recentIndicators =
            recentIndicatorIds.Count == 0
                ? new Dictionary<long, string>()
                : await _dbContext.ThreatIndicators
                    .AsNoTracking()
                    .Where(x =>
                        recentIndicatorIds.Contains(
                            x.Id) &&
                        !x.IsDeleted)
                    .Select(x => new
                    {
                        x.Id,
                        x.Value
                    })
                    .ToDictionaryAsync(
                        x => x.Id,
                        x => x.Value,
                        cancellationToken);

        var recentAlerts =
            recentAlertsRaw
                .Where(x =>
                    recentIndicators.ContainsKey(
                        x.ThreatIndicatorId))
                .Select(x =>
                    new DashboardRecentAlertDto(
                        x.Id,
                        x.ThreatIndicatorId,
                        recentIndicators[
                            x.ThreatIndicatorId],
                        x.AlertType.ToString(),
                        x.Title,
                        x.Severity.ToString(),
                        x.IsRead,
                        x.CreatedAtUtc))
                .ToList();

        /*
         * ==========================================
         * RESULT
         * ==========================================
         */

        return new DashboardOverviewDto(
            totalIndicators,
            criticalIndicators,
            highRiskIndicators,
            watchedIndicators,
            unreadAlerts,
            automaticRelations,
            manualRelations,
            topRiskyIndicators,
            recentAlerts);
    }
}