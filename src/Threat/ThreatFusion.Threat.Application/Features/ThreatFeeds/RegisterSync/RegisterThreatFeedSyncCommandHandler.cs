using MediatR;
using ThreatFusion.Threat.Application.Abstractions;
using ThreatFusion.Threat.Domain.Entities;

namespace ThreatFusion.Threat.Application
    .Features.ThreatFeeds.RegisterSync;

public sealed class RegisterThreatFeedSyncCommandHandler
    : IRequestHandler<
        RegisterThreatFeedSyncCommand,
        RegisterThreatFeedSyncResult>
{
    private readonly IThreatDbContext _dbContext;

    public RegisterThreatFeedSyncCommandHandler(
        IThreatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RegisterThreatFeedSyncResult> Handle(
        RegisterThreatFeedSyncCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FeedName))
        {
            return RegisterThreatFeedSyncResult.Failure(
                "Feed name is required.");
        }

        var sync = new ThreatFeedSync
        {
            FeedName = request.FeedName.Trim(),

            StartedAtUtc = request.StartedAtUtc,
            CompletedAtUtc = request.CompletedAtUtc,

            TotalFetched = request.TotalFetched,

            CreatedCount = request.CreatedCount,
            UpdatedCount = request.UpdatedCount,
            UnchangedCount = request.UnchangedCount,
            FailedCount = request.FailedCount,

            IsSuccessful = request.IsSuccessful,
            ErrorMessage = request.ErrorMessage,

            CreatedAtUtc = DateTime.UtcNow,
            IsDeleted = false
        };

        await _dbContext.ThreatFeedSyncs.AddAsync(
            sync,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return RegisterThreatFeedSyncResult.Success(
            sync.Id);
    }
}