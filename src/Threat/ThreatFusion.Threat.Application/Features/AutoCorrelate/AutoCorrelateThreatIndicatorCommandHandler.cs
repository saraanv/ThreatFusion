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
    private readonly DnsEnrichmentService _dnsEnrichmentService;

    public AutoCorrelateThreatIndicatorCommandHandler(
        IThreatDbContext dbContext,
        DnsEnrichmentService dnsEnrichmentService)
    {
        _dbContext = dbContext;
        _dnsEnrichmentService = dnsEnrichmentService;
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

        if (indicator.Type == IndicatorType.Domain)
        {
            return await CorrelateDomainWithIpAsync(
                indicator,
                cancellationToken);
        }
        
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
    
    private async Task<int> CorrelateDomainWithIpAsync(
    ThreatIndicator indicator,
    CancellationToken cancellationToken)
{
    var resolvedIpAddresses =
        await _dnsEnrichmentService.ResolveIpAddressesAsync(
            indicator.Value,
            cancellationToken);

    if (resolvedIpAddresses.Count == 0)
    {
        return 0;
    }

    var createdRelations = 0;

    foreach (var ipAddress in resolvedIpAddresses)
    {
        var normalizedIp =
            ThreatIndicatorNormalizer.Normalize(
                IndicatorType.IpAddress,
                ipAddress);

        var ipIndicator =
            await _dbContext.ThreatIndicators
                .FirstOrDefaultAsync(
                    x =>
                        x.Type == IndicatorType.IpAddress &&
                        x.Value == normalizedIp &&
                        !x.IsDeleted,
                    cancellationToken);

        if (ipIndicator is null)
        {
            ipIndicator =
                new ThreatIndicator
                {
                    Type =
                        IndicatorType.IpAddress,

                    Value =
                        normalizedIp,

                    Severity =
                        indicator.Severity,

                    Confidence =
                        indicator.Confidence,

                    RiskScore =
                        indicator.RiskScore,

                    RiskLevel =
                        indicator.RiskLevel,

                    SourceName =
                        "DNS-Enrichment",

                    Description =
                        $"Resolved automatically from domain {indicator.Value}.",

                    FirstSeenUtc =
                        DateTime.UtcNow,

                    LastSeenUtc =
                        DateTime.UtcNow,

                    CreatedAtUtc =
                        DateTime.UtcNow,

                    IsActive = true,

                    IsDeleted = false
                };

            await _dbContext.ThreatIndicators.AddAsync(
                ipIndicator,
                cancellationToken);

            await _dbContext.SaveChangesAsync(
                cancellationToken);
        }

        var relationExists =
            await _dbContext.ThreatIndicatorRelations
                .AnyAsync(
                    x =>
                        x.SourceIndicatorId == indicator.Id &&
                        x.TargetIndicatorId == ipIndicator.Id &&
                        x.RelationType ==
                            ThreatRelationType.ResolvesTo &&
                        !x.IsDeleted,
                    cancellationToken);

        if (relationExists)
        {
            continue;
        }

        var relation =
            new ThreatIndicatorRelation
            {
                SourceIndicatorId =
                    indicator.Id,

                TargetIndicatorId =
                    ipIndicator.Id,

                RelationType =
                    ThreatRelationType.ResolvesTo,

                Description =
                    "Automatically resolved through DNS enrichment.",

                Confidence = 100,

                IsActive = true,

                CreatedAtUtc =
                    DateTime.UtcNow,

                IsDeleted = false
            };

        await _dbContext.ThreatIndicatorRelations.AddAsync(
            relation,
            cancellationToken);

        createdRelations++;
    }

    if (createdRelations > 0)
    {
        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    return createdRelations;
}
}