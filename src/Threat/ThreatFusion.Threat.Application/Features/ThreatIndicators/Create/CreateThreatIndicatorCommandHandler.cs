using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;
using ThreatFusion.Threat.Domain.Entities;

namespace ThreatFusion.Threat.Application.Features.ThreatIndicators.Create;

public sealed class CreateThreatIndicatorCommandHandler
    : IRequestHandler<
        CreateThreatIndicatorCommand,
        CreateThreatIndicatorResult>
{
    private readonly IThreatDbContext _dbContext;
    private readonly IValidator<CreateThreatIndicatorCommand> _validator;

    public CreateThreatIndicatorCommandHandler(
        IThreatDbContext dbContext,
        IValidator<CreateThreatIndicatorCommand> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
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
            request.Value.Trim().ToLowerInvariant();

        var exists =
            await _dbContext.ThreatIndicators
                .AnyAsync(
                    x =>
                        x.Type == request.Type &&
                        x.Value == normalizedValue,
                    cancellationToken);

        if (exists)
        {
            return CreateThreatIndicatorResult.Failure(
                "This threat indicator already exists.");
        }

        var indicator = new ThreatIndicator
        {
            Type = request.Type,
            Value = normalizedValue,

            Severity = request.Severity,
            Confidence = request.Confidence,

            SourceName = request.SourceName.Trim(),
            Description = request.Description?.Trim(),

            FirstSeenUtc = request.FirstSeenUtc,
            LastSeenUtc = request.LastSeenUtc,

            CreatedAtUtc = DateTime.UtcNow,

            IsActive = true,
            IsDeleted = false
        };

        await _dbContext.ThreatIndicators.AddAsync(
            indicator,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return CreateThreatIndicatorResult.Success(
            indicator.Id);
    }
}