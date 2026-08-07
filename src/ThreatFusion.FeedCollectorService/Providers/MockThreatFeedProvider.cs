using ThreatFusion.FeedCollectorService.Models;

namespace ThreatFusion.FeedCollectorService.Providers;

public sealed class MockThreatFeedProvider
    : IThreatFeedProvider
{
    public Task<IReadOnlyCollection<ThreatIndicatorRequest>>
        GetIndicatorsAsync(
            CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        IReadOnlyCollection<ThreatIndicatorRequest> indicators =
        [
            new ThreatIndicatorRequest(
                Type: 3,
                Value: "malicious-example.com",
                Severity: 3,
                Confidence: 90,
                SourceName: "MockFeed",
                Description: "Mock malicious domain",
                FirstSeenUtc: now.AddDays(-2),
                LastSeenUtc: now),

            new ThreatIndicatorRequest(
                Type: 4,
                Value: "http://malicious-example.com/payload.exe",
                Severity: 4,
                Confidence: 95,
                SourceName: "MockFeed",
                Description: "Mock malicious URL",
                FirstSeenUtc: now.AddDays(-1),
                LastSeenUtc: now)
        ];

        return Task.FromResult(indicators);
    }
}