using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;

namespace ThreatFusion.Threat.Application.Features.ThreatRelations.GetByIndicator;

public sealed class GetThreatRelationsByIndicatorQueryHandler
    : IRequestHandler<
        GetThreatRelationsByIndicatorQuery,
        IReadOnlyCollection<ThreatRelationDto>>
{
    private readonly IThreatDbContext _dbContext;

    public GetThreatRelationsByIndicatorQueryHandler(
        IThreatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<ThreatRelationDto>> Handle(
        GetThreatRelationsByIndicatorQuery request,
        CancellationToken cancellationToken)
    {
        var relations =
            await _dbContext.ThreatIndicatorRelations
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    (
                        x.SourceIndicatorId == request.IndicatorId ||
                        x.TargetIndicatorId == request.IndicatorId
                    ))
                .Join(
                    _dbContext.ThreatIndicators,
                    relation => relation.SourceIndicatorId,
                    source => source.Id,
                    (relation, source) => new
                    {
                        relation,
                        source
                    })
                .Join(
                    _dbContext.ThreatIndicators,
                    x => x.relation.TargetIndicatorId,
                    target => target.Id,
                    (x, target) => new ThreatRelationDto(
                        x.relation.Id,
                        x.relation.SourceIndicatorId,
                        x.source.Value,
                        x.relation.TargetIndicatorId,
                        target.Value,
                        x.relation.RelationType.ToString(),
                        x.relation.Description,
                        x.relation.Confidence,
                        x.relation.IsActive))
                .ToListAsync(
                    cancellationToken);

        return relations;
    }
}