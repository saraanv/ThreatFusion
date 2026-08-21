using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;

namespace ThreatFusion.Threat.Application.Features.ThreatRelations.GetGraph;

public sealed class GetThreatGraphQueryHandler
    : IRequestHandler<
        GetThreatGraphQuery,
        ThreatGraphDto>
{
    private readonly IThreatDbContext _dbContext;

    public GetThreatGraphQueryHandler(
        IThreatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ThreatGraphDto> Handle(
        GetThreatGraphQuery request,
        CancellationToken cancellationToken)
    {
        var relations =
            await _dbContext.ThreatIndicatorRelations
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.IsActive &&
                    (
                        x.SourceIndicatorId == request.IndicatorId ||
                        x.TargetIndicatorId == request.IndicatorId
                    ))
                .ToListAsync(
                    cancellationToken);

        var indicatorIds =
            relations
                .SelectMany(x => new[]
                {
                    x.SourceIndicatorId,
                    x.TargetIndicatorId
                })
                .Append(request.IndicatorId)
                .Distinct()
                .ToList();

        var indicators =
            await _dbContext.ThreatIndicators
                .AsNoTracking()
                .Where(x =>
                    indicatorIds.Contains(x.Id) &&
                    !x.IsDeleted)
                .ToListAsync(
                    cancellationToken);

        var nodes =
            indicators
                .Select(x =>
                    new ThreatGraphNodeDto(
                        x.Id,
                        x.Type.ToString(),
                        x.Value,
                        x.Severity.ToString(),
                        x.RiskScore,
                        x.RiskLevel.ToString(),
                        x.SourceName))
                .ToList();

        var edges =
            relations
                .Select(x =>
                    new ThreatGraphEdgeDto(
                        x.Id,
                        x.SourceIndicatorId,
                        x.TargetIndicatorId,
                        x.RelationType.ToString(),
                        x.Confidence,
                        x.Description))
                .ToList();

        return new ThreatGraphDto(
            nodes,
            edges);
    }
}