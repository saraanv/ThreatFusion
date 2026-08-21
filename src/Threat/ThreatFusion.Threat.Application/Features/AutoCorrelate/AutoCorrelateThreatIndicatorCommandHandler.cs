using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;
using ThreatFusion.Threat.Application.Services;
using ThreatFusion.Threat.Domain.Entities;
using ThreatFusion.Threat.Domain.Enums;

namespace ThreatFusion.Threat.Application
    .Features.ThreatRelations.AutoCorrelate;

public sealed class AutoCorrelateThreatIndicatorCommandHandler
    : IRequestHandler<
        AutoCorrelateThreatIndicatorCommand,
        int>
{
    private readonly IThreatDbContext _dbContext;

    public AutoCorrelateThreatIndicatorCommandHandler(
        IThreatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Handle(
        AutoCorrelateThreatIndicatorCommand request,
        CancellationToken cancellationToken)
    {
        var indicator =
            await _dbContext.ThreatIndicators
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == request.IndicatorId &&
                        !x.IsDeleted,
                    cancellationToken);

        if (indicator is null)
        {
            return 0;
        }

        // فعلاً اولین Rule ما فقط برای URL است.
        if (indicator.Type != IndicatorType.Url)
        {
            return 0;
        }

        if (!Uri.TryCreate(
                indicator.Value,
                UriKind.Absolute,
                out var uri))
        {
            return 0;
        }

        var normalizedDomain =
            ThreatIndicatorNormalizer.Normalize(
                IndicatorType.Domain,
                uri.Host);

        if (string.IsNullOrWhiteSpace(normalizedDomain))
        {
            return 0;
        }

        var domainIndicator =
            await _dbContext.ThreatIndicators
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.Type == IndicatorType.Domain &&
                        x.Value == normalizedDomain &&
                        !x.IsDeleted,
                    cancellationToken);

        // اگر Domain هنوز در ThreatFusion وجود ندارد،
        // فعلاً relation نمی‌سازیم.
        if (domainIndicator is null)
        {
            return 0;
        }

        // URL را به خودش وصل نکن.
        if (domainIndicator.Id == indicator.Id)
        {
            return 0;
        }

        var relationExists =
            await _dbContext.ThreatIndicatorRelations
                .AnyAsync(
                    x =>
                        x.SourceIndicatorId == indicator.Id &&
                        x.TargetIndicatorId == domainIndicator.Id &&
                        x.RelationType ==
                            ThreatRelationType.AssociatedWith &&
                        !x.IsDeleted,
                    cancellationToken);

        if (relationExists)
        {
            return 0;
        }

        var relation =
            new ThreatIndicatorRelation
            {
                SourceIndicatorId =
                    indicator.Id,

                TargetIndicatorId =
                    domainIndicator.Id,

                RelationType =
                    ThreatRelationType.AssociatedWith,

                Description =
                    "Automatically correlated from URL host.",

                Confidence = 100,

                IsActive = true,

                CreatedAtUtc =
                    DateTime.UtcNow,

                IsDeleted = false
            };

        await _dbContext.ThreatIndicatorRelations
            .AddAsync(
                relation,
                cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return 1;
    }
}