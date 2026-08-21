using MediatR;

namespace ThreatFusion.Threat.Application.Features.Alerts.GetUnreadCount;

public sealed record GetUnreadAlertCountQuery(
    long UserId)
    : IRequest<int>;