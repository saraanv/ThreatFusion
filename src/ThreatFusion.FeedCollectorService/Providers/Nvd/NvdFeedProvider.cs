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

    var allIndicators =
        new List<ThreatIndicatorRequest>();

    var startIndex = 0;
    var totalResults = int.MaxValue;

    while (startIndex < totalResults)
    {
        var url =
            $"/rest/json/cves/2.0" +
            $"?pubStartDate={Uri.EscapeDataString(startDateText)}" +
            $"&pubEndDate={Uri.EscapeDataString(endDateText)}" +
            $"&resultsPerPage={resultsPerPage}" +
            $"&startIndex={startIndex}";

        _logger.LogInformation(
            "Fetching NVD page. StartIndex: {StartIndex}",
            startIndex);

        var response =
            await _httpClient.GetFromJsonAsync<NvdResponse>(
                url,
                cancellationToken);

        if (response is null)
        {
            break;
        }

        totalResults =
            response.TotalResults;

        _logger.LogInformation(
            "NVD page returned {Count}. Total results: {TotalResults}.",
            response.Vulnerabilities.Count,
            totalResults);

        var indicators =
            response.Vulnerabilities
                .Select(item =>
                {
                    var description =
                        item.Cve.Descriptions
                            .FirstOrDefault(x =>
                                x.Lang == "en")
                            ?.Value;

                    var cvss =
                        item.Cve.Metrics.CvssMetricV31
                            .FirstOrDefault()
                            ?.CvssData;

                    var cwe =
                        item.Cve.Weaknesses
                            .SelectMany(x => x.Description)
                            .FirstOrDefault(x => x.Lang == "en")
                            ?.Value;

                    var referenceUrl =
                        item.Cve.References
                            .FirstOrDefault()
                            ?.Url;
                    
                    var severity = cvss?.BaseSeverity switch
                    {
                        "LOW" => 1,
                        "MEDIUM" => 2,
                        "HIGH" => 3,
                        "CRITICAL" => 4,
                        _ => 0
                    };
                    
                    return new ThreatIndicatorRequest(
                        Type: 8,
                        Value: item.Cve.Id,
                        Severity: severity,
                        Confidence: 80,
                        SourceName: "NVD",
                        Description: description,
                        FirstSeenUtc: item.Cve.Published,
                        LastSeenUtc: item.Cve.LastModified,
                        CvssScore: cvss?.BaseScore,
                        CvssVersion: cvss?.Version,
                        CvssVector: cvss?.VectorString,
                        CweId: cwe,
                        ReferenceUrl: referenceUrl);
                });

        allIndicators.AddRange(indicators);

        if (response.Vulnerabilities.Count == 0)
        {
            break;
        }

        startIndex += response.ResultsPerPage;

        if (startIndex < totalResults)
        {
            await Task.Delay(
                TimeSpan.FromSeconds(6),
                cancellationToken);
        }
    }

    _logger.LogInformation(
        "NVD collection completed. Total collected: {Count}",
        allIndicators.Count);

    return allIndicators;
}
}