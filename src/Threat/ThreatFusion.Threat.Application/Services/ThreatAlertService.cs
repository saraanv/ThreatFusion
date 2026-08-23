using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;
using ThreatFusion.Threat.Domain.Entities;
using ThreatFusion.Threat.Domain.Enums;

namespace ThreatFusion.Threat.Application.Services;

public sealed class ThreatAlertService
{
    private readonly IThreatDbContext _dbContext;

    public ThreatAlertService(
        IThreatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateNewRelationAlertsAsync(
        long indicatorId,
        string indicatorValue,
        ThreatSeverity severity,
        string relatedIndicatorValue,
        ThreatRelationType relationType,
        CancellationToken cancellationToken)
    {
        var watcherUserIds =
            await GetWatcherUserIdsAsync(
                indicatorId,
                cancellationToken);

        if (watcherUserIds.Count == 0)
        {
            return;
        }

        foreach (var userId in watcherUserIds)
        {
            var duplicateExists =
                await HasRecentDuplicateAsync(
                    userId,
                    indicatorId,
                    ThreatAlertType.NewRelation,
                    TimeSpan.FromMinutes(10),
                    cancellationToken);

            if (duplicateExists)
            {
                continue;
            }

            var alert =
                new ThreatAlert
                {
                    UserId = userId,

                    ThreatIndicatorId =
                        indicatorId,

                    AlertType =
                        ThreatAlertType.NewRelation,

                    Title =
                        "New threat relation discovered",

                    Message =
                        $"A new {relationType} relation was discovered " +
                        $"for '{indicatorValue}' with '{relatedIndicatorValue}'.",

                    Severity =
                        severity,

                    IsRead =
                        false,

                    ReadAtUtc =
                        null,

                    CreatedAtUtc =
                        DateTime.UtcNow,

                    IsDeleted =
                        false
                };

            await _dbContext.ThreatAlerts
                .AddAsync(
                    alert,
                    cancellationToken);
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task CreateRiskIncreasedAlertsAsync(
        long indicatorId,
        string indicatorValue,
        double oldRiskScore,
        double newRiskScore,
        ThreatSeverity severity,
        CancellationToken cancellationToken)
    {
        if (newRiskScore <= oldRiskScore)
        {
            return;
        }

        var watcherUserIds =
            await GetWatcherUserIdsAsync(
                indicatorId,
                cancellationToken);

        if (watcherUserIds.Count == 0)
        {
            return;
        }

        foreach (var userId in watcherUserIds)
        {
            var duplicateExists =
                await HasRecentDuplicateAsync(
                    userId,
                    indicatorId,
                    ThreatAlertType.RiskIncreased,
                    TimeSpan.FromMinutes(10),
                    cancellationToken);

            if (duplicateExists)
            {
                continue;
            }

            var alert =
                new ThreatAlert
                {
                    UserId = userId,

                    ThreatIndicatorId =
                        indicatorId,

                    AlertType =
                        ThreatAlertType.RiskIncreased,

                    Title =
                        "Threat risk increased",

                    Message =
                        $"Risk score for '{indicatorValue}' " +
                        $"increased from {oldRiskScore} to {newRiskScore}.",

                    Severity =
                        severity,

                    IsRead =
                        false,

                    ReadAtUtc =
                        null,

                    CreatedAtUtc =
                        DateTime.UtcNow,

                    IsDeleted =
                        false
                };

            await _dbContext.ThreatAlerts
                .AddAsync(
                    alert,
                    cancellationToken);
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    private async Task<List<long>> GetWatcherUserIdsAsync(
        long indicatorId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ThreatWatchlists
            .AsNoTracking()
            .Where(x =>
                x.ThreatIndicatorId ==
                    indicatorId &&
                x.IsActive &&
                !x.IsDeleted)
            .Select(x =>
                x.UserId)
            .Distinct()
            .ToListAsync(
                cancellationToken);
    }

    private async Task<bool> HasRecentDuplicateAsync(
        long userId,
        long indicatorId,
        ThreatAlertType alertType,
        TimeSpan timeWindow,
        CancellationToken cancellationToken)
    {
        var threshold =
            DateTime.UtcNow.Subtract(
                timeWindow);

        return await _dbContext.ThreatAlerts
            .AsNoTracking()
            .AnyAsync(
                x =>
                    x.UserId == userId &&
                    x.ThreatIndicatorId ==
                        indicatorId &&
                    x.AlertType ==
                        alertType &&
                    !x.IsDeleted &&
                    x.CreatedAtUtc >=
                        threshold,
                cancellationToken);
    }
}