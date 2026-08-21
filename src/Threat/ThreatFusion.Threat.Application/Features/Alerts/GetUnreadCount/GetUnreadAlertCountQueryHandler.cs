using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;

namespace ThreatFusion.Threat.Application.Features.Alerts.GetUnreadCount;

public sealed class GetUnreadAlertCountQueryHandler
    : IRequestHandler<
        GetUnreadAlertCountQuery,
        int>
{
    private readonly IThreatDbContext _dbContext;

    public GetUnreadAlertCountQueryHandler(
        IThreatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Handle(
        GetUnreadAlertCountQuery request,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ThreatAlerts
            .AsNoTracking()
            .CountAsync(
                x =>
                    x.UserId == request.UserId &&
                    !x.IsRead &&
                    !x.IsDeleted,
                cancellationToken);
    }
}