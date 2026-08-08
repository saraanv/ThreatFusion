using System.Net.Http.Json;
using ThreatFusion.FeedCollectorService.Models;
using ThreatFusion.FeedCollectorService.Providers;

namespace ThreatFusion.FeedCollectorService.Providers.CisaKev;

public sealed class CisaKevFeedProvider : IThreatFeedProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CisaKevFeedProvider> _logger;

    public CisaKevFeedProvider(
        HttpClient httpClient,
        ILogger<CisaKevFeedProvider> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<ThreatIndicatorRequest>>
        GetIndicatorsAsync(
            CancellationToken cancellationToken)
    {
        using var response =
            await _httpClient.GetAsync(
                "/sites/default/files/feeds/known_exploited_vulnerabilities.json",
                cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body =
                await response.Content.ReadAsStringAsync(
                    cancellationToken);

            _logger.LogError(
                "CISA KEV request failed. Status: {StatusCode}, Body: {Body}",
                response.StatusCode,
                body);

            response.EnsureSuccessStatusCode();
        }

        var cisaResponse =
            await response.Content
                .ReadFromJsonAsync<CisaKevResponse>(
                    cancellationToken: cancellationToken);

        if (cisaResponse is null)
        {
            return [];
        }

        _logger.LogInformation(
            "CISA KEV returned {Count} vulnerabilities.",
            cisaResponse.Vulnerabilities.Count);

        return cisaResponse.Vulnerabilities
            .OrderByDescending(x => x.DateAdded)
            .Take(10)
            .Select(vulnerability =>
                new ThreatIndicatorRequest(
                    Type: 8,
                    Value: vulnerability.CveId,
                    Severity: 4,
                    Confidence: 100,
                    SourceName: "CISA-KEV",
                    Description:
                    BuildDescription(vulnerability),
                    FirstSeenUtc:
                    vulnerability.DateAdded,
                    LastSeenUtc:
                    cisaResponse.DateReleased))
            .ToList();
    }

    private static string BuildDescription(
        CisaKevVulnerability vulnerability)
    {
        return
            $"{vulnerability.VendorProject} - " +
            $"{vulnerability.Product}: " +
            $"{vulnerability.ShortDescription}";
    }
    
}