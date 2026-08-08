using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThreatFusion.Threat.Application.Features.Dashboard;

namespace ThreatFusion.Threat.API.Controllers;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly ISender _sender;

    public DashboardController(ISender sender)
    {
        _sender = sender;
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
}