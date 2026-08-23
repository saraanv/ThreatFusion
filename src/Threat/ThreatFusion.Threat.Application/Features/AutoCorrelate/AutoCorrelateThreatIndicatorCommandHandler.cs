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
    private readonly ThreatAlertService _threatAlertService;

    public AutoCorrelateThreatIndicatorCommandHandler(
        IThreatDbContext dbContext,
        DnsEnrichmentService dnsEnrichmentService,
        ThreatAlertService threatAlertService)
    {
        _dbContext = dbContext;
        _dnsEnrichmentService = dnsEnrichmentService;
        _threatAlertService = threatAlertService;
    }

    public async Task<int> Handle(
        AutoCorrelateThreatIndicatorCommand request,
        CancellationToken cancellationToken)
    {
        var indicator =
            await _dbContext.ThreatIndicators
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == request.IndicatorId &&
                        !x.IsDeleted,
                    cancellationToken);

        if (indicator is null)
        {
            return 0;
        }

        if (indicator.Type == IndicatorType.Url)
        {
            return await CorrelateUrlAsync(
                indicator,
                cancellationToken);
        }

        if (indicator.Type == IndicatorType.Domain)
        {
            return await CorrelateDomainWithIpAsync(
                indicator,
                cancellationToken);
        }

        return 0;
    }

    private async Task<int> CorrelateUrlAsync(
        ThreatIndicator urlIndicator,
        CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(
                urlIndicator.Value,
                UriKind.Absolute,
                out var uri))
        {
            return 0;
        }

        var normalizedDomain =
            ThreatIndicatorNormalizer.Normalize(
                IndicatorType.Domain,
                uri.Host);

        if (string.IsNullOrWhiteSpace(
                normalizedDomain))
        {
            return 0;
        }

        var domainIndicator =
            await _dbContext.ThreatIndicators
                .FirstOrDefaultAsync(
                    x =>
                        x.Type ==
                            IndicatorType.Domain &&
                        x.Value ==
                            normalizedDomain &&
                        !x.IsDeleted,
                    cancellationToken);

        if (domainIndicator is null)
        {
            var domainRisk =
                ThreatRiskCalculator.Calculate(
                    urlIndicator.Severity,
                    urlIndicator.Confidence,
                    null,
                    "URL-Enrichment");

            domainIndicator =
                new ThreatIndicator
                {
                    Type =
                        IndicatorType.Domain,

                    Value =
                        normalizedDomain,

                    Severity =
                        urlIndicator.Severity,

                    Confidence =
                        urlIndicator.Confidence,

                    RiskScore =
                        domainRisk.Score,

                    RiskLevel =
                        domainRisk.Level,

                    SourceName =
                        "URL-Enrichment",

                    Description =
                        $"Extracted automatically from URL {urlIndicator.Value}.",

                    FirstSeenUtc =
                        DateTime.UtcNow,

                    LastSeenUtc =
                        DateTime.UtcNow,

                    CreatedAtUtc =
                        DateTime.UtcNow,

                    IsActive = true,
                    IsDeleted = false
                };

            await _dbContext.ThreatIndicators
                .AddAsync(
                    domainIndicator,
                    cancellationToken);

            await _dbContext.SaveChangesAsync(
                cancellationToken);
        }

        var createdRelations = 0;

        var urlDomainRelationExists =
            await _dbContext
                .ThreatIndicatorRelations
                .AnyAsync(
                    x =>
                        x.SourceIndicatorId ==
                            urlIndicator.Id &&
                        x.TargetIndicatorId ==
                            domainIndicator.Id &&
                        x.RelationType ==
                            ThreatRelationType
                                .AssociatedWith &&
                        !x.IsDeleted,
                    cancellationToken);

        if (!urlDomainRelationExists)
        {
            var relation =
                new ThreatIndicatorRelation
                {
                    SourceIndicatorId =
                        urlIndicator.Id,

                    TargetIndicatorId =
                        domainIndicator.Id,

                    RelationType =
                        ThreatRelationType
                            .AssociatedWith,

                    Description =
                        "Automatically correlated from URL host.",

                    Confidence = 100,

                    SourceName =
                        "URL-Enrichment",

                    IsAutomatic =
                        true,

                    DiscoveredAtUtc =
                        DateTime.UtcNow,

                    IsActive = true,

                    CreatedAtUtc =
                        DateTime.UtcNow,

                    IsDeleted = false
                };

            await _dbContext
                .ThreatIndicatorRelations
                .AddAsync(
                    relation,
                    cancellationToken);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            createdRelations++;

            /*
             * Alert برای هر دو سمت relation.
             *
             * اگر URL watch شده باشد:
             * URL → Domain alert
             */
            await _threatAlertService
                .CreateNewRelationAlertsAsync(
                    urlIndicator.Id,
                    urlIndicator.Value,
                    urlIndicator.Severity,
                    domainIndicator.Value,
                    ThreatRelationType.AssociatedWith,
                    cancellationToken);

            /*
             * اگر Domain watch شده باشد:
             * Domain ← URL alert
             */
            await _threatAlertService
                .CreateNewRelationAlertsAsync(
                    domainIndicator.Id,
                    domainIndicator.Value,
                    domainIndicator.Severity,
                    urlIndicator.Value,
                    ThreatRelationType.AssociatedWith,
                    cancellationToken);
        }

        /*
         * حالا Domain را DNS enrich می‌کنیم.
         * این مرحله می‌تواند Domain → IP relation بسازد.
         */
        createdRelations +=
            await CorrelateDomainWithIpAsync(
                domainIndicator,
                cancellationToken);

        return createdRelations;
    }

    private async Task<int> CorrelateDomainWithIpAsync(
        ThreatIndicator domainIndicator,
        CancellationToken cancellationToken)
    {
        var resolvedIpAddresses =
            await _dnsEnrichmentService
                .ResolveIpAddressesAsync(
                    domainIndicator.Value,
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
                            x.Type ==
                                IndicatorType.IpAddress &&
                            x.Value ==
                                normalizedIp &&
                            !x.IsDeleted,
                        cancellationToken);

            if (ipIndicator is null)
            {
                var ipRisk =
                    ThreatRiskCalculator.Calculate(
                        domainIndicator.Severity,
                        domainIndicator.Confidence,
                        null,
                        "DNS-Enrichment");

                ipIndicator =
                    new ThreatIndicator
                    {
                        Type =
                            IndicatorType.IpAddress,

                        Value =
                            normalizedIp,

                        Severity =
                            domainIndicator.Severity,

                        Confidence =
                            domainIndicator.Confidence,

                        RiskScore =
                            ipRisk.Score,

                        RiskLevel =
                            ipRisk.Level,

                        SourceName =
                            "DNS-Enrichment",

                        Description =
                            $"Resolved automatically from domain {domainIndicator.Value}.",

                        FirstSeenUtc =
                            DateTime.UtcNow,

                        LastSeenUtc =
                            DateTime.UtcNow,

                        CreatedAtUtc =
                            DateTime.UtcNow,

                        IsActive = true,
                        IsDeleted = false
                    };

                await _dbContext
                    .ThreatIndicators
                    .AddAsync(
                        ipIndicator,
                        cancellationToken);

                await _dbContext.SaveChangesAsync(
                    cancellationToken);
            }

            var relationExists =
                await _dbContext
                    .ThreatIndicatorRelations
                    .AnyAsync(
                        x =>
                            x.SourceIndicatorId ==
                                domainIndicator.Id &&
                            x.TargetIndicatorId ==
                                ipIndicator.Id &&
                            x.RelationType ==
                                ThreatRelationType
                                    .ResolvesTo &&
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
                        domainIndicator.Id,

                    TargetIndicatorId =
                        ipIndicator.Id,

                    RelationType =
                        ThreatRelationType.ResolvesTo,

                    Description =
                        "Automatically resolved through DNS enrichment.",

                    Confidence = 100,

                    SourceName =
                        "DNS-Enrichment",

                    IsAutomatic =
                        true,

                    DiscoveredAtUtc =
                        DateTime.UtcNow,

                    IsActive = true,

                    CreatedAtUtc =
                        DateTime.UtcNow,

                    IsDeleted = false
                };

            await _dbContext
                .ThreatIndicatorRelations
                .AddAsync(
                    relation,
                    cancellationToken);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            createdRelations++;

            /*
             * Domain روی watchlist باشد:
             * Domain → IP
             */
            await _threatAlertService
                .CreateNewRelationAlertsAsync(
                    domainIndicator.Id,
                    domainIndicator.Value,
                    domainIndicator.Severity,
                    ipIndicator.Value,
                    ThreatRelationType.ResolvesTo,
                    cancellationToken);

            /*
             * IP روی watchlist باشد:
             * IP ← Domain
             */
            await _threatAlertService
                .CreateNewRelationAlertsAsync(
                    ipIndicator.Id,
                    ipIndicator.Value,
                    ipIndicator.Severity,
                    domainIndicator.Value,
                    ThreatRelationType.ResolvesTo,
                    cancellationToken);
        }

        return createdRelations;
    }
}