using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThreatFusion.Threat.API.Models;
using ThreatFusion.Threat.API.Services;
using ThreatFusion.Threat.Application.Features.Watchlists.Add;
using ThreatFusion.Threat.Application.Features.Watchlists.GetMine;
using ThreatFusion.Threat.Application.Features.Watchlists.Remove;

namespace ThreatFusion.Threat.API.Controllers;

[ApiController]
[Route("api/watchlists")]
[Authorize]
public sealed class WatchlistsController
    : ControllerBase
{
    private readonly ISender _sender;
    private readonly CurrentUserService _currentUserService;

    public WatchlistsController(
        ISender sender,
        CurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    [Route("AddToWatchlist")]
    public async Task<IActionResult> AddToWatchlist(
        [FromBody] AddToWatchlistRequest request,
        CancellationToken cancellationToken)
    {
        var userId =
            _currentUserService.GetUserId();

        var result =
            await _sender.Send(
                new AddToWatchlistCommand(
                    userId,
                    request.ThreatIndicatorId,
                    request.Note),
                cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(
                new ApiErrorResponse(
                    "ValidationError",
                    "Failed to add indicator to watchlist.",
                    result.Errors,
                    HttpContext.TraceIdentifier));
        }

        return Ok(new
        {
            result.WatchlistId
        });
    }

    [HttpDelete]
    [Route("RemoveFromWatchlist")]
    public async Task<IActionResult> RemoveFromWatchlist(
        [FromQuery] long threatIndicatorId,
        CancellationToken cancellationToken)
    {
        var userId =
            _currentUserService.GetUserId();

        var removed =
            await _sender.Send(
                new RemoveFromWatchlistCommand(
                    userId,
                    threatIndicatorId),
                cancellationToken);

        if (!removed)
        {
            return NotFound(
                new ApiErrorResponse(
                    "NotFound",
                    "Watchlist item was not found.",
                    null,
                    HttpContext.TraceIdentifier));
        }

        return NoContent();
    }

    [HttpGet]
    [Route("GetMyWatchlist")]
    public async Task<IActionResult> GetMyWatchlist(
        CancellationToken cancellationToken)
    {
        var userId =
            _currentUserService.GetUserId();

        var result =
            await _sender.Send(
                new GetMyWatchlistQuery(
                    userId),
                cancellationToken);

        return Ok(result);
    }

    public sealed record AddToWatchlistRequest(
        long ThreatIndicatorId,
        string? Note);
}