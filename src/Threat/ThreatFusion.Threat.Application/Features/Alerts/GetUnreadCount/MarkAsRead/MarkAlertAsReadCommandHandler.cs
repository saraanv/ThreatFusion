using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;

namespace ThreatFusion.Threat.Application.Features.Alerts.MarkAsRead;

public sealed class MarkAlertAsReadCommandHandler
    : IRequestHandler<
        MarkAlertAsReadCommand,
        bool>
{
    private readonly IThreatDbContext _dbContext;

    public MarkAlertAsReadCommandHandler(
        IThreatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(
        MarkAlertAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var alert =
            await _dbContext.ThreatAlerts
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == request.AlertId &&
                        x.UserId == request.UserId &&
                        !x.IsDeleted,
                    cancellationToken);

        if (alert is null)
        {
            return false;
        }

        if (!alert.IsRead)
        {
            alert.IsRead = true;
            alert.ReadAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(
                cancellationToken);
        }

        return true;
    }
}