using MediatR;

namespace ThreatFusion.Threat.Application.Features.Dashboard;

public sealed record GetThreatDashboardQuery
    : IRequest<ThreatDashboardDto>;