using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;
using ThreatFusion.Threat.Application.Services;
using ThreatFusion.Threat.Domain.Entities;

namespace ThreatFusion.Threat.Application.Features.ThreatRelations.Create;

public sealed class CreateThreatRelationCommandHandler
    : IRequestHandler<
        CreateThreatRelationCommand,
        CreateThreatRelationResult>
{
    private readonly IThreatDbContext _dbContext;
    private readonly IValidator<CreateThreatRelationCommand> _validator;
    private readonly ThreatAlertService _threatAlertService;

    public CreateThreatRelationCommandHandler(
        IThreatDbContext dbContext,
        IValidator<CreateThreatRelationCommand> validator,
        ThreatAlertService threatAlertService)
    {
        _dbContext = dbContext;
        _validator = validator;
        _threatAlertService = threatAlertService;
    }

    public async Task<CreateThreatRelationResult> Handle(
        CreateThreatRelationCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult =
            await _validator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            return CreateThreatRelationResult.Failure(
                validationResult.Errors
                    .Select(x => x.ErrorMessage)
                    .ToArray());
        }

        
        var sourceIndicator =
            await _dbContext.ThreatIndicators
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == request.SourceIndicatorId &&
                        !x.IsDeleted,
                    cancellationToken);

        if (sourceIndicator is null)
        {
            return CreateThreatRelationResult.Failure(
                "Source indicator was not found.");
        }

        
        var targetIndicator =
            await _dbContext.ThreatIndicators
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == request.TargetIndicatorId &&
                        !x.IsDeleted,
                    cancellationToken);

        if (targetIndicator is null)
        {
            return CreateThreatRelationResult.Failure(
                "Target indicator was not found.");
        }

        
        var relationExists =
            await _dbContext.ThreatIndicatorRelations
                .AnyAsync(
                    x =>
                        x.SourceIndicatorId ==
                            request.SourceIndicatorId &&
                        x.TargetIndicatorId ==
                            request.TargetIndicatorId &&
                        x.RelationType ==
                            request.RelationType &&
                        !x.IsDeleted,
                    cancellationToken);

        if (relationExists)
        {
            return CreateThreatRelationResult.Failure(
                "This threat relation already exists.");
        }

        
        var relation =
            new ThreatIndicatorRelation
            {
                SourceIndicatorId =
                    request.SourceIndicatorId,

                TargetIndicatorId =
                    request.TargetIndicatorId,

                RelationType =
                    request.RelationType,

                Description =
                    request.Description?.Trim(),

                Confidence =
                    request.Confidence,

                IsActive = true,

                CreatedAtUtc =
                    DateTime.UtcNow,

                IsDeleted = false,

                SourceName = "Manual",

                IsAutomatic = false,

                DiscoveredAtUtc =
                    DateTime.UtcNow,
            };

        await _dbContext.ThreatIndicatorRelations
            .AddAsync(
                relation,
                cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        
        await _threatAlertService
            .CreateNewRelationAlertsAsync(
                indicatorId:
                    sourceIndicator.Id,

                indicatorValue:
                    sourceIndicator.Value,

                severity:
                    sourceIndicator.Severity,

                relatedIndicatorValue:
                    targetIndicator.Value,

                relationType:
                    request.RelationType,

                cancellationToken:
                    cancellationToken);

        return CreateThreatRelationResult.Success(
            relation.Id);
    }
}