using MediatR;

namespace ThreatFusion.Threat.Application.Features.Alerts.GetMine;

public sealed record GetMyAlertsQuery(
    long UserId)
    : IRequest<IReadOnlyCollection<ThreatAlertDto>>;