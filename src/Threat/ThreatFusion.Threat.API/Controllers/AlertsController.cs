using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThreatFusion.Threat.API.Services;
using ThreatFusion.Threat.Application.Features.Alerts.GetMine;
using ThreatFusion.Threat.Application.Features.Alerts.GetUnreadCount;
using ThreatFusion.Threat.Application.Features.Alerts.MarkAsRead;

namespace ThreatFusion.Threat.API.Controllers;

[ApiController]
[Route("api/alerts")]
[Authorize]
public sealed class AlertsController
    : ControllerBase
{
    private readonly ISender _sender;
    private readonly CurrentUserService _currentUserService;

    public AlertsController(
        ISender sender,
        CurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [Route("GetMyAlerts")]
    public async Task<IActionResult> GetMyAlerts(
        CancellationToken cancellationToken)
    {
        var userId =
            _currentUserService.GetUserId();

        var result =
            await _sender.Send(
                new GetMyAlertsQuery(
                    userId),
                cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    [Route("GetUnreadAlertCount")]
    public async Task<IActionResult> GetUnreadAlertCount(
        CancellationToken cancellationToken)
    {
        var userId =
            _currentUserService.GetUserId();

        var count =
            await _sender.Send(
                new GetUnreadAlertCountQuery(
                    userId),
                cancellationToken);

        return Ok(new
        {
            Count = count
        });
    }

    [HttpPatch]
    [Route("MarkAlertAsRead")]
    public async Task<IActionResult> MarkAlertAsRead(
        [FromQuery] long alertId,
        CancellationToken cancellationToken)
    {
        var userId =
            _currentUserService.GetUserId();

        var result =
            await _sender.Send(
                new MarkAlertAsReadCommand(
                    userId,
                    alertId),
                cancellationToken);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}