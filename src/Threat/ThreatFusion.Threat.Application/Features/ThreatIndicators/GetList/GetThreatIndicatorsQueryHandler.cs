using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;
using ThreatFusion.Threat.Application.Common.Models;

namespace ThreatFusion.Threat.Application.Features.ThreatIndicators.GetList;

public sealed class GetThreatIndicatorsQueryHandler
    : IRequestHandler<
        GetThreatIndicatorsQuery,
        PagedResult<ThreatIndicatorListItemDto>>
{
    private readonly IThreatDbContext _dbContext;

    public GetThreatIndicatorsQueryHandler(
        IThreatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<ThreatIndicatorListItemDto>> Handle(
        GetThreatIndicatorsQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber =
            request.PageNumber <= 0
                ? 1
                : request.PageNumber;

        var pageSize =
            request.PageSize <= 0
                ? 20
                : Math.Min(request.PageSize, 100);

        var query =
            _dbContext.ThreatIndicators
                .AsNoTracking()
                .AsQueryable();

        if (request.Type.HasValue)
        {
            query =
                query.Where(x =>
                    x.Type == request.Type.Value);
        }

        if (request.Severity.HasValue)
        {
            query =
                query.Where(x =>
                    x.Severity == request.Severity.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Source))
        {
            var source =
                request.Source.Trim();

            query =
                query.Where(x =>
                    x.SourceName == source);
        }

        if (request.MinConfidence.HasValue)
        {
            query =
                query.Where(x =>
                    x.Confidence >=
                    request.MinConfidence.Value);
        }

        if (request.FromDateUtc.HasValue)
        {
            query =
                query.Where(x =>
                    x.CreatedAtUtc >=
                    request.FromDateUtc.Value);
        }

        if (request.ToDateUtc.HasValue)
        {
            query =
                query.Where(x =>
                    x.CreatedAtUtc <=
                    request.ToDateUtc.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm =
                request.SearchTerm.Trim();

            query =
                query.Where(x =>
                    x.Value.Contains(searchTerm) ||
                    (x.Description != null &&
                     x.Description.Contains(searchTerm)));
        }

        if (request.MinRiskScore.HasValue)
        {
            query =
                query.Where(x =>
                    x.RiskScore >=
                    request.MinRiskScore.Value);
        }

        if (request.MaxRiskScore.HasValue)
        {
            query =
                query.Where(x =>
                    x.RiskScore <=
                    request.MaxRiskScore.Value);
        }

        if (request.RiskLevel.HasValue)
        {
            query =
                query.Where(x =>
                    x.RiskLevel ==
                    request.RiskLevel.Value);
        }

        if (request.IsActive.HasValue)
        {
            query =
                query.Where(x =>
                    x.IsActive ==
                    request.IsActive.Value);
        }

        var totalCount =
            await query.CountAsync(
                cancellationToken);

        query =
            ApplySorting(
                query,
                request.SortBy,
                request.SortDescending);

        var entities =
            await query
                .Skip(
                    (pageNumber - 1) *
                    pageSize)
                .Take(pageSize)
                .ToListAsync(
                    cancellationToken);

        var items =
            entities
                .Select(x =>
                    new ThreatIndicatorListItemDto(
                        x.Id,
                        x.Type.ToString(),
                        x.Value,
                        x.Severity.ToString(),
                        x.Confidence,
                        x.RiskScore,
                        x.RiskLevel.ToString(),
                        x.SourceName,
                        x.Description,
                        x.FirstSeenUtc,
                        x.LastSeenUtc,
                        x.CvssScore,
                        x.CvssVersion,
                        x.CvssVector,
                        x.CweId,
                        x.ReferenceUrl,
                        x.IsActive))
                .ToList();

        var totalPages =
            (int)Math.Ceiling(
                totalCount /
                (double)pageSize);

        return new PagedResult<ThreatIndicatorListItemDto>(
            items,
            pageNumber,
            pageSize,
            totalCount,
            totalPages);
    }

    private static IQueryable<Domain.Entities.ThreatIndicator>
        ApplySorting(
            IQueryable<Domain.Entities.ThreatIndicator> query,
            string? sortBy,
            bool sortDescending)
    {
        var normalizedSortBy =
            sortBy?.Trim().ToLowerInvariant();

        return normalizedSortBy switch
        {
            "riskscore" =>
                sortDescending
                    ? query.OrderByDescending(x => x.RiskScore)
                    : query.OrderBy(x => x.RiskScore),

            "severity" =>
                sortDescending
                    ? query.OrderByDescending(x => x.Severity)
                    : query.OrderBy(x => x.Severity),

            "confidence" =>
                sortDescending
                    ? query.OrderByDescending(x => x.Confidence)
                    : query.OrderBy(x => x.Confidence),

            "firstseenutc" =>
                sortDescending
                    ? query.OrderByDescending(x => x.FirstSeenUtc)
                    : query.OrderBy(x => x.FirstSeenUtc),

            "lastseenutc" =>
                sortDescending
                    ? query.OrderByDescending(x => x.LastSeenUtc)
                    : query.OrderBy(x => x.LastSeenUtc),

            _ =>
                sortDescending
                    ? query.OrderByDescending(x => x.CreatedAtUtc)
                    : query.OrderBy(x => x.CreatedAtUtc)
        };
    }
}