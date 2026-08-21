using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;

namespace ThreatFusion.Threat.Application.Features.Alerts.GetMine;

public sealed class GetMyAlertsQueryHandler
    : IRequestHandler<
        GetMyAlertsQuery,
        IReadOnlyCollection<ThreatAlertDto>>
{
    private readonly IThreatDbContext _dbContext;

    public GetMyAlertsQueryHandler(
        IThreatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<ThreatAlertDto>> Handle(
        GetMyAlertsQuery request,
        CancellationToken cancellationToken)
    {
        var alerts =
            await _dbContext.ThreatAlerts
                .AsNoTracking()
                .Where(x =>
                    x.UserId == request.UserId &&
                    !x.IsDeleted)
                .Join(
                    _dbContext.ThreatIndicators
                        .AsNoTracking()
                        .Where(x => !x.IsDeleted),

                    alert =>
                        alert.ThreatIndicatorId,

                    indicator =>
                        indicator.Id,

                    (alert, indicator) =>
                        new
                        {
                            Alert = alert,
                            Indicator = indicator
                        })
                .OrderByDescending(x =>
                    x.Alert.CreatedAtUtc)
                .ToListAsync(
                    cancellationToken);

        return alerts
            .Select(x =>
                new ThreatAlertDto(
                    x.Alert.Id,
                    x.Alert.ThreatIndicatorId,
                    x.Indicator.Value,
                    x.Alert.AlertType.ToString(),
                    x.Alert.Title,
                    x.Alert.Message,
                    x.Alert.Severity.ToString(),
                    x.Alert.IsRead,
                    x.Alert.CreatedAtUtc,
                    x.Alert.ReadAtUtc))
            .ToList();
    }
}