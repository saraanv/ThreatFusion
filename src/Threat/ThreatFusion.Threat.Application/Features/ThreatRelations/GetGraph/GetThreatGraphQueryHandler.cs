using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;
using ThreatFusion.Threat.Domain.Entities;
using ThreatFusion.Threat.Domain.Enums;

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

            var relationQuery =
                _dbContext.ThreatIndicatorRelations
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
                        ));

            if (request.RelationType.HasValue)
            {
                relationQuery =
                    relationQuery.Where(x =>
                        x.RelationType ==
                        request.RelationType.Value);
            }

            if (request.IsAutomatic.HasValue)
            {
                relationQuery =
                    relationQuery.Where(x =>
                        x.IsAutomatic ==
                        request.IsAutomatic.Value);
            }

            var relations =
                await relationQuery
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

        var indicatorQuery =
            _dbContext.ThreatIndicators
                .AsNoTracking()
                .Where(x =>
                    indicatorIds.Contains(x.Id) &&
                    !x.IsDeleted);

        if (request.MinRiskScore.HasValue)
        {
            indicatorQuery =
                indicatorQuery.Where(x =>
                    x.RiskScore >=
                    request.MinRiskScore.Value);
        }

        var indicators =
            await indicatorQuery
                .ToListAsync(
                    cancellationToken);

        var allowedIndicatorIds =
            indicators
                .Select(x => x.Id)
                .ToHashSet();

        var filteredRelations =
            allRelations.Values
                .Where(x =>
                    allowedIndicatorIds.Contains(
                        x.SourceIndicatorId) &&
                    allowedIndicatorIds.Contains(
                        x.TargetIndicatorId))
                .ToList();

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
            filteredRelations
                .Select(x =>
                    new ThreatGraphEdgeDto(
                        x.Id,
                        x.SourceIndicatorId,
                        x.TargetIndicatorId,
                        x.RelationType.ToString(),
                        x.Confidence,
                        x.Description,
                        x.SourceName,
                        x.IsAutomatic,
                        x.DiscoveredAtUtc))
                .ToList();

        var highestRiskIndicator =
            indicators
                .OrderByDescending(x =>
                    x.RiskScore)
                .FirstOrDefault();

        var summary =
            new ThreatGraphSummaryDto(
                NodeCount:
                    nodes.Count,

                EdgeCount:
                    edges.Count,

                CriticalNodeCount:
                    indicators.Count(x =>
                        x.RiskLevel ==
                        ThreatRiskLevel.Critical),

                HighRiskNodeCount:
                    indicators.Count(x =>
                        x.RiskLevel ==
                        ThreatRiskLevel.High),

                AutomaticRelationCount:
                    filteredRelations.Count(x =>
                        x.IsAutomatic),

                ManualRelationCount:
                    filteredRelations.Count(x =>
                        !x.IsAutomatic),

                AverageRiskScore:
                    indicators.Count == 0
                        ? 0
                        : Math.Round(
                            indicators.Average(x =>
                                x.RiskScore),
                            2),

                HighestRiskIndicatorId:
                    highestRiskIndicator?.Id,

                HighestRiskIndicatorValue:
                    highestRiskIndicator?.Value,

                HighestRiskScore:
                    highestRiskIndicator?.RiskScore);

        return new ThreatGraphDto(
            nodes,
            edges,
            summary);
    }
}