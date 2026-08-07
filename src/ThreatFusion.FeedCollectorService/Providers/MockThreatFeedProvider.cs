using ThreatFusion.FeedCollectorService.Models;

namespace ThreatFusion.FeedCollectorService.Providers;

public sealed class MockThreatFeedProvider : IThreatFeedProvider
{
    public Task<IReadOnlyCollection<ThreatIndicatorRequest>> GetIndicatorsAsync(
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<ThreatIndicatorRequest> indicators =
        [
            new(
                Type: 3,
                Value: "malicious-example.com",
                Severity: 3,
                Confidence: 90,
                SourceName: "MockFeed",
                Description: "Mock malicious domain",
                FirstSeenUtc: DateTime.UtcNow.AddDays(-2),
                LastSeenUtc: DateTime.UtcNow),

            new(
                Type: 4,
                Value: "http://malicious-example.com/payload.exe",
                Severity: 4,
                Confidence: 95,
                SourceName: "MockFeed",
                Description: "Mock malicious URL",
                FirstSeenUtc: DateTime.UtcNow.AddDays(-1),
                LastSeenUtc: DateTime.UtcNow)
        ];

        return Task.FromResult(indicators);
    }
}