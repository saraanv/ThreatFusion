using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThreatFusion.Threat.Application.Features.ThreatIndicators.Create;
using ThreatFusion.Threat.Application.Features.ThreatIndicators.Search;
using Microsoft.AspNetCore.Authorization;
using ThreatFusion.Threat.Application.Features.ThreatIndicators.GetById;
using ThreatFusion.Threat.Application.Features.ThreatIndicators.GetList;

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
    
        return Ok(new
        {
            result.IndicatorId,
            Status = result.Status?.ToString()
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
    
    [Authorize]
    [HttpGet]
    [Route("GetThreatIndicators")]
    [ActionName("دریافت لیست شاخص‌های تهدید")]
    public async Task<IActionResult> GetThreatIndicators(
        [FromQuery] GetThreatIndicatorsQuery query,
        CancellationToken cancellationToken)
    {
        var result =
            await _sender.Send(
                query,
                cancellationToken);

        return Ok(result);
    }
    
    [Authorize]
    [HttpGet]
    [Route("GetThreatIndicatorById")]
    [ActionName("دریافت جزئیات شاخص تهدید")]
    public async Task<IActionResult> GetById(
        [FromQuery] long id,
        CancellationToken cancellationToken)
    {
        var result =
            await _sender.Send(
                new GetThreatIndicatorByIdQuery(id),
                cancellationToken);


        if (result == null)
        {
            return NotFound();
        }


        return Ok(result);
    }
}