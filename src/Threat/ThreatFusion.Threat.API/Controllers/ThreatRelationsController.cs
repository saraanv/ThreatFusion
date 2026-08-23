using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThreatFusion.Threat.API.Models;
using ThreatFusion.Threat.Application.Features.ThreatRelations.Create;
using ThreatFusion.Threat.Application.Features.ThreatRelations.GetByIndicator;
using ThreatFusion.Threat.Application.Features.ThreatRelations.GetGraph;
using ThreatFusion.Threat.Domain.Enums;

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
            return BadRequest(
                new ApiErrorResponse(
                    "ValidationError",
                    "Failed to create or update threat indicator.",
                    result.Errors,
                    HttpContext.TraceIdentifier));
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
    [Authorize]
    [HttpGet]
    [Route("GetThreatGraph")]
    [ActionName("دریافت گراف تهدید")]
    public async Task<IActionResult> GetThreatGraph(
        [FromQuery] long indicatorId,
        [FromQuery] int depth = 1,
        [FromQuery] ThreatRelationType? relationType = null,
        [FromQuery] bool? isAutomatic = null,
        [FromQuery] double? minRiskScore = null,
        CancellationToken cancellationToken = default)
    {
        var result =
            await _sender.Send(
                new GetThreatGraphQuery(
                    indicatorId,
                    depth,
                    relationType,
                    isAutomatic,
                    minRiskScore),
                cancellationToken);

        return Ok(result);
    }
}