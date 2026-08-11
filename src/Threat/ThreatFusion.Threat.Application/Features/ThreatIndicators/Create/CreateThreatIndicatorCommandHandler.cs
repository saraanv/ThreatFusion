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

        var existingIndicator =
    await _dbContext.ThreatIndicators
        .FirstOrDefaultAsync(
            x =>
                x.Type == request.Type &&
                x.Value == normalizedValue,
            cancellationToken);

if (existingIndicator is not null)
{
    var changed = false;

    if (existingIndicator.Severity != request.Severity)
    {
        existingIndicator.Severity = request.Severity;
        changed = true;
    }

    if (existingIndicator.Confidence != request.Confidence)
    {
        existingIndicator.Confidence = request.Confidence;
        changed = true;
    }

    if (existingIndicator.Description != request.Description)
    {
        existingIndicator.Description = request.Description;
        changed = true;
    }

    if (existingIndicator.FirstSeenUtc != request.FirstSeenUtc)
    {
        existingIndicator.FirstSeenUtc = request.FirstSeenUtc;
        changed = true;
    }

    if (existingIndicator.LastSeenUtc != request.LastSeenUtc)
    {
        existingIndicator.LastSeenUtc = request.LastSeenUtc;
        changed = true;
    }

    if (existingIndicator.CvssScore != request.CvssScore)
    {
        existingIndicator.CvssScore = request.CvssScore;
        changed = true;
    }

    if (existingIndicator.CvssVersion != request.CvssVersion)
    {
        existingIndicator.CvssVersion = request.CvssVersion;
        changed = true;
    }

    if (existingIndicator.CvssVector != request.CvssVector)
    {
        existingIndicator.CvssVector = request.CvssVector;
        changed = true;
    }

    if (existingIndicator.CweId != request.CweId)
    {
        existingIndicator.CweId = request.CweId;
        changed = true;
    }

    if (existingIndicator.ReferenceUrl != request.ReferenceUrl)
    {
        existingIndicator.ReferenceUrl = request.ReferenceUrl;
        changed = true;
    }

    if (!changed)
    {
        return CreateThreatIndicatorResult.Unchanged(
            existingIndicator.Id);
    }

    await _dbContext.SaveChangesAsync(
        cancellationToken);

    return CreateThreatIndicatorResult.Updated(
        existingIndicator.Id);
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

    CvssScore = request.CvssScore,
    CvssVersion = request.CvssVersion,
    CvssVector = request.CvssVector,
    CweId = request.CweId,
    ReferenceUrl = request.ReferenceUrl,

    CreatedAtUtc = DateTime.UtcNow,

    IsActive = true,
    IsDeleted = false
};

await _dbContext.ThreatIndicators.AddAsync(
    indicator,
    cancellationToken);

await _dbContext.SaveChangesAsync(
    cancellationToken);

return CreateThreatIndicatorResult.Created(
    indicator.Id);
    }
}