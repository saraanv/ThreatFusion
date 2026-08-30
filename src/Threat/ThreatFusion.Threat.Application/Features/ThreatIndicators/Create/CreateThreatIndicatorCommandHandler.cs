using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;
using ThreatFusion.Threat.Application.Features.ThreatRelations.AutoCorrelate;
using ThreatFusion.Threat.Application.Services;
using ThreatFusion.Threat.Domain.Entities;

namespace ThreatFusion.Threat.Application.Features.ThreatIndicators.Create;

public sealed class CreateThreatIndicatorCommandHandler
    : IRequestHandler<
        CreateThreatIndicatorCommand,
        CreateThreatIndicatorResult>
{
    private readonly IThreatDbContext _dbContext;

    private readonly IValidator<CreateThreatIndicatorCommand>
        _validator;

    private readonly ISender _sender;

    private readonly ThreatAlertService _threatAlertService;

    public CreateThreatIndicatorCommandHandler(
        IThreatDbContext dbContext,
        IValidator<CreateThreatIndicatorCommand> validator,
        ISender sender,
        ThreatAlertService threatAlertService)
    {
        _dbContext = dbContext;
        _validator = validator;
        _sender = sender;
        _threatAlertService = threatAlertService;
    }

    public async Task<CreateThreatIndicatorResult> Handle(
        CreateThreatIndicatorCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult =
            await _validator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            return CreateThreatIndicatorResult.Failure(
                validationResult.Errors
                    .Select(x => x.ErrorMessage)
                    .ToArray());
        }

        var normalizedValue =
            ThreatIndicatorNormalizer.Normalize(
                request.Type,
                request.Value);

        var risk =
            ThreatRiskCalculator.Calculate(
                request.Severity,
                request.Confidence,
                request.CvssScore,
                request.SourceName);

        var existingIndicator =
            await _dbContext.ThreatIndicators
                .FirstOrDefaultAsync(
                    x =>
                        x.Type == request.Type &&
                        x.Value == normalizedValue &&
                        !x.IsDeleted,
                    cancellationToken);

        
        if (existingIndicator is not null)
        {
            
            var oldRiskScore =
                existingIndicator.RiskScore;

            var oldSeverity =
                existingIndicator.Severity;

            var oldConfidence =
                existingIndicator.Confidence;

            var oldCvssScore =
                existingIndicator.CvssScore;

            var oldRiskLevel =
                existingIndicator.RiskLevel;

            var changed =
                false;

            
            if (existingIndicator.Severity !=
                request.Severity)
            {
                existingIndicator.Severity =
                    request.Severity;

                changed = true;
            }

            
            if (existingIndicator.Confidence !=
                request.Confidence)
            {
                existingIndicator.Confidence =
                    request.Confidence;

                changed = true;
            }

            
            
            var normalizedDescription =
                request.Description?.Trim();

            if (existingIndicator.Description !=
                normalizedDescription)
            {
                existingIndicator.Description =
                    normalizedDescription;

                changed = true;
            }

            
            if (existingIndicator.FirstSeenUtc !=
                request.FirstSeenUtc)
            {
                existingIndicator.FirstSeenUtc =
                    request.FirstSeenUtc;

                changed = true;
            }

            
            if (existingIndicator.LastSeenUtc !=
                request.LastSeenUtc)
            {
                existingIndicator.LastSeenUtc =
                    request.LastSeenUtc;

                changed = true;
            }
            
            if (existingIndicator.CvssScore !=
                request.CvssScore)
            {
                existingIndicator.CvssScore =
                    request.CvssScore;

                changed = true;
            }

            
            if (existingIndicator.CvssVersion !=
                request.CvssVersion)
            {
                existingIndicator.CvssVersion =
                    request.CvssVersion;

                changed = true;
            }

            
            if (existingIndicator.CvssVector !=
                request.CvssVector)
            {
                existingIndicator.CvssVector =
                    request.CvssVector;

                changed = true;
            }

            
            if (existingIndicator.CweId !=
                request.CweId)
            {
                existingIndicator.CweId =
                    request.CweId;

                changed = true;
            }

            
            if (existingIndicator.ReferenceUrl !=
                request.ReferenceUrl)
            {
                existingIndicator.ReferenceUrl =
                    request.ReferenceUrl;

                changed = true;
            }
            
            if (existingIndicator.RiskScore !=
                risk.Score)
            {
                existingIndicator.RiskScore =
                    risk.Score;

                changed = true;
            }

            
            if (existingIndicator.RiskLevel !=
                risk.Level)
            {
                existingIndicator.RiskLevel =
                    risk.Level;

                changed = true;
            }

            
            if (!changed)
            {
                await _sender.Send(
                    new AutoCorrelateThreatIndicatorCommand(
                        existingIndicator.Id),
                    cancellationToken);

                return CreateThreatIndicatorResult
                    .Unchanged(
                        existingIndicator.Id);
            }

            
            await _dbContext.SaveChangesAsync(
                cancellationToken);

            
            
            var importantChanges =
                new List<string>();

            if (oldSeverity !=
                existingIndicator.Severity)
            {
                importantChanges.Add(
                    $"Severity changed from " +
                    $"{oldSeverity} to " +
                    $"{existingIndicator.Severity}.");
            }

            if (oldConfidence !=
                existingIndicator.Confidence)
            {
                importantChanges.Add(
                    $"Confidence changed from " +
                    $"{oldConfidence} to " +
                    $"{existingIndicator.Confidence}.");
            }

            if (oldCvssScore !=
                existingIndicator.CvssScore)
            {
                importantChanges.Add(
                    $"CVSS score changed from " +
                    $"{oldCvssScore?.ToString() ?? "null"} " +
                    $"to " +
                    $"{existingIndicator.CvssScore?.ToString() ?? "null"}.");
            }

            if (oldRiskLevel !=
                existingIndicator.RiskLevel)
            {
                importantChanges.Add(
                    $"Risk level changed from " +
                    $"{oldRiskLevel} to " +
                    $"{existingIndicator.RiskLevel}.");
            }

            if (importantChanges.Count > 0)
            {
                await _threatAlertService
                    .CreateIndicatorUpdatedAlertsAsync(
                        existingIndicator.Id,
                        existingIndicator.Value,
                        existingIndicator.Severity,
                        string.Join(
                            " ",
                            importantChanges),
                        cancellationToken);
            }

            
            if (existingIndicator.RiskScore >
                oldRiskScore)
            {
                await _threatAlertService
                    .CreateRiskIncreasedAlertsAsync(
                        existingIndicator.Id,
                        existingIndicator.Value,
                        oldRiskScore,
                        existingIndicator.RiskScore,
                        existingIndicator.Severity,
                        cancellationToken);
            }

            
            await _sender.Send(
                new AutoCorrelateThreatIndicatorCommand(
                    existingIndicator.Id),
                cancellationToken);

            return CreateThreatIndicatorResult
                .Updated(
                    existingIndicator.Id);
        }

        
        var indicator =
            new ThreatIndicator
            {
                Type =
                    request.Type,

                Value =
                    normalizedValue,

                Severity =
                    request.Severity,

                Confidence =
                    request.Confidence,

                RiskScore =
                    risk.Score,

                RiskLevel =
                    risk.Level,

                SourceName =
                    request.SourceName.Trim(),

                Description =
                    request.Description?.Trim(),

                FirstSeenUtc =
                    request.FirstSeenUtc,

                LastSeenUtc =
                    request.LastSeenUtc,

                CvssScore =
                    request.CvssScore,

                CvssVersion =
                    request.CvssVersion,

                CvssVector =
                    request.CvssVector,

                CweId =
                    request.CweId,

                ReferenceUrl =
                    request.ReferenceUrl,

                CreatedAtUtc =
                    DateTime.UtcNow,

                IsActive =
                    true,

                IsDeleted =
                    false
            };

        await _dbContext.ThreatIndicators
            .AddAsync(
                indicator,
                cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        
        
        await _sender.Send(
            new AutoCorrelateThreatIndicatorCommand(
                indicator.Id),
            cancellationToken);

        return CreateThreatIndicatorResult
            .Created(
                indicator.Id);
    }
}