using MediatR;

namespace ThreatFusion.Threat.Application.Features.Dashboard.GetOverview;

public sealed record GetDashboardOverviewQuery(
    long UserId)
    : IRequest<DashboardOverviewDto>;