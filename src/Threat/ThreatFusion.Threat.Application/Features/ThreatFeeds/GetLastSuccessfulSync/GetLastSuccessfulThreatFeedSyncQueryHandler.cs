using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;

namespace ThreatFusion.Threat.Application
    .Features.ThreatFeeds.GetLastSuccessfulSync;

public sealed class GetLastSuccessfulThreatFeedSyncQueryHandler
    : IRequestHandler<
        GetLastSuccessfulThreatFeedSyncQuery,
        LastThreatFeedSyncDto?>
{
    private readonly IThreatDbContext _dbContext;

    public GetLastSuccessfulThreatFeedSyncQueryHandler(
        IThreatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LastThreatFeedSyncDto?> Handle(
        GetLastSuccessfulThreatFeedSyncQuery request,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ThreatFeedSyncs
            .AsNoTracking()
            .Where(x =>
                x.FeedName == request.FeedName &&
                x.IsSuccessful)
            .OrderByDescending(x => x.CompletedAtUtc)
            .Select(x => new LastThreatFeedSyncDto(
                x.FeedName,
                x.StartedAtUtc,
                x.CompletedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);
    }
}