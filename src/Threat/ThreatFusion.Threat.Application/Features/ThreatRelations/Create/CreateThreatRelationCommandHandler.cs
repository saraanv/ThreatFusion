using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;
using ThreatFusion.Threat.Domain.Entities;

namespace ThreatFusion.Threat.Application.Features.ThreatRelations.Create;

public sealed class CreateThreatRelationCommandHandler
    : IRequestHandler<
        CreateThreatRelationCommand,
        CreateThreatRelationResult>
{
    private readonly IThreatDbContext _dbContext;
    private readonly IValidator<CreateThreatRelationCommand> _validator;

    public CreateThreatRelationCommandHandler(
        IThreatDbContext dbContext,
        IValidator<CreateThreatRelationCommand> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
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

        var sourceIndicatorExists =
            await _dbContext.ThreatIndicators
                .AnyAsync(
                    x =>
                        x.Id == request.SourceIndicatorId &&
                        !x.IsDeleted,
                    cancellationToken);

        if (!sourceIndicatorExists)
        {
            return CreateThreatRelationResult.Failure(
                "Source indicator was not found.");
        }

        var targetIndicatorExists =
            await _dbContext.ThreatIndicators
                .AnyAsync(
                    x =>
                        x.Id == request.TargetIndicatorId &&
                        !x.IsDeleted,
                    cancellationToken);

        if (!targetIndicatorExists)
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

                IsDeleted = false
            };

        await _dbContext.ThreatIndicatorRelations
            .AddAsync(
                relation,
                cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return CreateThreatRelationResult.Success(
            relation.Id);
    }
}