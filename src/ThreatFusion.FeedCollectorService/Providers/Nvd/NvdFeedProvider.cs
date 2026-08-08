using System.Net.Http.Json;
using ThreatFusion.FeedCollectorService.Models;
using ThreatFusion.FeedCollectorService.Providers;

namespace ThreatFusion.FeedCollectorService.Services.Providers.Nvd;

public sealed class NvdFeedProvider : IThreatFeedProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NvdFeedProvider> _logger;
    private readonly IConfiguration _configuration;

    public NvdFeedProvider(
        HttpClient httpClient,
        ILogger<NvdFeedProvider> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<IReadOnlyCollection<ThreatIndicatorRequest>>
        GetIndicatorsAsync(
            CancellationToken cancellationToken)
    {
        var lookbackHours =
            _configuration.GetValue<int>(
                "Nvd:LookbackHours");

        var resultsPerPage =
            _configuration.GetValue<int>(
                "Nvd:ResultsPerPage");

        if (lookbackHours <= 0)
        {
            lookbackHours = 24;
        }

        if (resultsPerPage <= 0)
        {
            resultsPerPage = 20;
        }

        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddHours(-lookbackHours);

        var startDateText =
            startDate.ToString(
                "yyyy-MM-ddTHH:mm:ss.fff");

        var endDateText =
            endDate.ToString(
                "yyyy-MM-ddTHH:mm:ss.fff");

        var url =
            $"/rest/json/cves/2.0" +
            $"?pubStartDate={Uri.EscapeDataString(startDateText)}" +
            $"&pubEndDate={Uri.EscapeDataString(endDateText)}" +
            $"&resultsPerPage={resultsPerPage}";

        _logger.LogInformation(
            "Fetching NVD CVEs from {StartDate} to {EndDate}.",
            startDate,
            endDate);

        var response =
            await _httpClient.GetFromJsonAsync<NvdResponse>(
                url,
                cancellationToken);

        if (response is null)
        {
            return [];
        }

        _logger.LogInformation(
            "NVD returned {Count} vulnerabilities out of {TotalResults}.",
            response.Vulnerabilities.Count,
            response.TotalResults);

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