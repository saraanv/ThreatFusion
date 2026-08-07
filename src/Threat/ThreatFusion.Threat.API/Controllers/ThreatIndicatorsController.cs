using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThreatFusion.Threat.Application.Features.ThreatIndicators.Create;
using ThreatFusion.Threat.Application.Features.ThreatIndicators.Search;
using Microsoft.AspNetCore.Authorization;

namespace ThreatFusion.Threat.API.Controllers;

[ApiController]
[Route("api/threat-indicators")]
public sealed class ThreatIndicatorsController : ControllerBase
{
    private readonly ISender _sender;

    public ThreatIndicatorsController(
        ISender sender)
    {
        _sender = sender;
    }
    
    [Authorize(Roles = "Analyst,Admin")]
    [HttpPost]
    [Route("CreateThreatIndicator")]
    [ActionName("ثبت شاخص تهدید")]
    public async Task<IActionResult> Create(
        CreateThreatIndicatorCommand command,
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
                result.IndicatorId,
                Message =
                    "Threat indicator created successfully."
            });
    }

    [Authorize]
    [HttpGet]
    [Route("SearchThreatIndicator")]
    [ActionName("جستجوی شاخص تهدید")]
    public async Task<IActionResult> Search(
        [FromQuery] string value,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new SearchThreatIndicatorQuery(value),
            cancellationToken);

        return Ok(result);
    }
}