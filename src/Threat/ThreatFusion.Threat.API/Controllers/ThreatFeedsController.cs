using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThreatFusion.Threat.Application
    .Features.ThreatFeeds.RegisterSync;

namespace ThreatFusion.Threat.API.Controllers;

[ApiController]
[Route("api/threat-feeds")]
public sealed class ThreatFeedsController : ControllerBase
{
    private readonly ISender _sender;

    public ThreatFeedsController(
        ISender sender)
    {
        _sender = sender;
    }

    [Authorize(Roles = "Analyst,Admin")]
    [HttpPost]
    [Route("RegisterSync")]
    [ActionName("ثبت نتیجه همگام سازی منبع تهدید")]
    public async Task<IActionResult> RegisterSync(
        RegisterThreatFeedSyncCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                Errors = result.Errors
            });
        }

        return StatusCode(
            StatusCodes.Status201Created,
            new
            {
                result.SyncId,
                Message =
                    "Threat feed synchronization registered successfully."
            });
    }
}