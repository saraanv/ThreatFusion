using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;
using ThreatFusion.Threat.Domain.Entities;

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
        var depth =
            request.Depth <= 0
                ? 1
                : Math.Min(request.Depth, 3);

        var visitedIndicatorIds =
            new HashSet<long>
            {
                request.IndicatorId
            };

        var currentLevel =
            new HashSet<long>
            {
                request.IndicatorId
            };

        var allRelations =
            new Dictionary<long, ThreatIndicatorRelation>();

        for (var level = 0;
             level < depth;
             level++)
        {
            if (currentLevel.Count == 0)
            {
                break;
            }

            var levelIds =
                currentLevel.ToList();

            var relations =
                await _dbContext.ThreatIndicatorRelations
                    .AsNoTracking()
                    .Where(x =>
                        !x.IsDeleted &&
                        x.IsActive &&
                        (
                            levelIds.Contains(
                                x.SourceIndicatorId)
                            ||
                            levelIds.Contains(
                                x.TargetIndicatorId)
                        ))
                    .ToListAsync(
                        cancellationToken);

            var nextLevel =
                new HashSet<long>();

            foreach (var relation in relations)
            {
                allRelations.TryAdd(
                    relation.Id,
                    relation);

                if (visitedIndicatorIds.Add(
                        relation.SourceIndicatorId))
                {
                    nextLevel.Add(
                        relation.SourceIndicatorId);
                }

                if (visitedIndicatorIds.Add(
                        relation.TargetIndicatorId))
                {
                    nextLevel.Add(
                        relation.TargetIndicatorId);
                }
            }

            currentLevel =
                nextLevel;
        }

        var indicatorIds =
            visitedIndicatorIds.ToList();

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
            allRelations.Values
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