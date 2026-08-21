using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThreatFusion.Threat.Application.Features.ThreatRelations.Create;
using ThreatFusion.Threat.Application.Features.ThreatRelations.GetByIndicator;

namespace ThreatFusion.Threat.API.Controllers;

[ApiController]
[Route("api/threat-relations")]
public sealed class ThreatRelationsController
    : ControllerBase
{
    private readonly ISender _sender;

    public ThreatRelationsController(
        ISender sender)
    {
        _sender = sender;
    }

    [Authorize(Roles = "Analyst,Admin")]
    [HttpPost]
    [Route("CreateRelation")]
    [ActionName("ثبت ارتباط بین شاخص‌های تهدید")]
    public async Task<IActionResult> CreateRelation(
        CreateThreatRelationCommand command,
        CancellationToken cancellationToken)
    {
        var result =
            await _sender.Send(
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
                result.RelationId,
                Message =
                    "Threat relation created successfully."
            });
    }
    
    [Authorize]
    [HttpGet]
    [Route("GetRelationsByIndicator")]
    [ActionName("دریافت ارتباطات شاخص تهدید")]
    public async Task<IActionResult> GetRelationsByIndicator(
        [FromQuery] long indicatorId,
        CancellationToken cancellationToken)
    {
        var result =
            await _sender.Send(
                new GetThreatRelationsByIndicatorQuery(
                    indicatorId),
                cancellationToken);

        return Ok(result);
    }
}