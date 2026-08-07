using System.Net.Http.Headers;
using System.Net.Http.Json;
using ThreatFusion.FeedCollectorService.Models;

namespace ThreatFusion.FeedCollectorService.Services;

public sealed class ThreatApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ThreatApiClient(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task SendIndicatorAsync(
        ThreatIndicatorRequest indicator,
        CancellationToken cancellationToken)
    {
        var token = _configuration["ThreatApi:Token"];

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException(
                "Threat API token is not configured.");
        }

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var response =
            await _httpClient.PostAsJsonAsync(
                "/api/threat-indicators/CreateThreatIndicator",
                indicator,
                cancellationToken);

        if (response.StatusCode ==
            System.Net.HttpStatusCode.BadRequest)
        {
            return;
        }

        response.EnsureSuccessStatusCode();
    }
}