using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ThreatFusion.FeedCollectorService.Models;

namespace ThreatFusion.FeedCollectorService.Services;

public sealed class ThreatApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IdentityApiClient _identityApiClient;

    public ThreatApiClient(
        HttpClient httpClient,
        IdentityApiClient identityApiClient)
    {
        _httpClient = httpClient;
        _identityApiClient = identityApiClient;
    }

    public async Task<string> SendIndicatorAsync(
        ThreatIndicatorRequest indicator,
        CancellationToken cancellationToken)
    {
        var accessToken =
            await _identityApiClient
                .GetAccessTokenAsync(
                    cancellationToken);

        using var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                "/threat/api/threat-indicators/CreateThreatIndicator");

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken);

        request.Content =
            JsonContent.Create(indicator);

        var response =
            await _httpClient.SendAsync(
                request,
                cancellationToken);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content
                .ReadFromJsonAsync<ThreatIndicatorWriteResponse>(
                    cancellationToken);

        if (result is null)
        {
            throw new InvalidOperationException(
                "Threat API returned an invalid response.");
        }

        return result.Status;
    }
    
    public async Task RegisterSyncAsync(
        ThreatFeedSyncRequest sync,
        CancellationToken cancellationToken)
    {
        var accessToken =
            await _identityApiClient
                .GetAccessTokenAsync(
                    cancellationToken);

        using var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                "/threat/api/threat-feeds/RegisterSync");

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken);

        request.Content =
            JsonContent.Create(sync);

        var response =
            await _httpClient.SendAsync(
                request,
                cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}