using ThreatFusion.FeedCollectorService.Models;

namespace ThreatFusion.FeedCollectorService.Providers;

public interface IThreatFeedProvider
{
    Task<IReadOnlyCollection<ThreatIndicatorRequest>> GetIndicatorsAsync(
        CancellationToken cancellationToken);
}