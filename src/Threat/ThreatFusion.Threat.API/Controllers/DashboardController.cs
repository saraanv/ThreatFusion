using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThreatFusion.Threat.API.Services;
using ThreatFusion.Threat.Application.Features.Dashboard;
using ThreatFusion.Threat.Application.Features.Dashboard.GetOverview;

namespace ThreatFusion.Threat.API.Controllers;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly ISender _sender;
    private readonly CurrentUserService _currentUserService;
    public DashboardController(
        ISender sender,
        CurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [Authorize]
    [HttpGet]
    [Route("GetDashboard")]
    [ActionName("دریافت اطلاعات داشبورد")]
    public async Task<IActionResult> GetDashboard(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetThreatDashboardQuery(),
            cancellationToken);

        return Ok(result);
    }
    [Authorize]
    [HttpGet]
    [Route("GetOverview")]
    [ActionName("دریافت خلاصه داشبورد")]
    public async Task<IActionResult> GetOverview(
        CancellationToken cancellationToken)
    {
        var userId =
            _currentUserService.GetUserId();

        var result =
            await _sender.Send(
                new GetDashboardOverviewQuery(
                    userId),
                cancellationToken);

        return Ok(result);
    }
}