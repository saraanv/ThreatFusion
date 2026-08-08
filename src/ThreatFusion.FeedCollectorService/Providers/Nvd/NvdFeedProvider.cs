using System.Net.Http.Json;
using ThreatFusion.FeedCollectorService.Models;
using ThreatFusion.FeedCollectorService.Providers;

namespace ThreatFusion.FeedCollectorService.Services.Providers.Nvd;

public sealed class NvdFeedProvider : IThreatFeedProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NvdFeedProvider> _logger;

    public NvdFeedProvider(
        HttpClient httpClient,
        ILogger<NvdFeedProvider> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<ThreatIndicatorRequest>>
        GetIndicatorsAsync(
            CancellationToken cancellationToken)
    {
        var response =
            await _httpClient.GetFromJsonAsync<NvdResponse>(
                "/rest/json/cves/2.0?resultsPerPage=10",
                cancellationToken);

        if (response is null)
        {
            return [];
        }

        _logger.LogInformation(
            "NVD returned {Count} vulnerabilities.",
            response.Vulnerabilities.Count);

        return response.Vulnerabilities
            .Select(item =>
            {
                var description =
                    item.Cve.Descriptions
                        .FirstOrDefault(x =>
                            x.Lang == "en")
                        ?.Value;

                return new ThreatIndicatorRequest(
                    Type: 8,
                    Value: item.Cve.Id,
                    Severity: 2,
                    Confidence: 80,
                    SourceName: "NVD",
                    Description: description,
                    FirstSeenUtc: item.Cve.Published,
                    LastSeenUtc: item.Cve.LastModified);
            })
            .ToList();
    }
}