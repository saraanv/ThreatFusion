using MediatR;

namespace ThreatFusion.Threat.Application.Features.Alerts.MarkAsRead;

public sealed record MarkAlertAsReadCommand(
    long UserId,
    long AlertId)
    : IRequest<bool>;