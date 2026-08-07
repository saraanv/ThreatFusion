using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;

namespace ThreatFusion.Threat.Application.Features.ThreatIndicators.Search;

public sealed class SearchThreatIndicatorQueryHandler
    : IRequestHandler<
        SearchThreatIndicatorQuery,
        IReadOnlyCollection<ThreatIndicatorDto>>
{
    private readonly IThreatDbContext _dbContext;

    public SearchThreatIndicatorQueryHandler(
        IThreatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<ThreatIndicatorDto>> Handle(
        SearchThreatIndicatorQuery request,
        CancellationToken cancellationToken)
    {
        var value = request.Value
            .Trim()
            .ToLowerInvariant();

        return await _dbContext.ThreatIndicators
            .AsNoTracking()
            .Where(x => x.Value.Contains(value))
            .OrderByDescending(x => x.Confidence)
            .Select(x => new ThreatIndicatorDto(
                x.Id,
                x.Type,
                x.Value,
                x.Severity,
                x.Confidence,
                x.SourceName,
                x.Description,
                x.FirstSeenUtc,
                x.LastSeenUtc,
                x.IsActive))
            .ToListAsync(cancellationToken);
    }
}