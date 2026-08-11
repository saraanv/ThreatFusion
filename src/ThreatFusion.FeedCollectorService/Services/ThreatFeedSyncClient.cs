using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ThreatFusion.FeedCollectorService.Services;

public sealed class ThreatFeedSyncClient
{
    private readonly HttpClient _httpClient;
    private readonly IdentityApiClient _identityApiClient;


    public ThreatFeedSyncClient(
        HttpClient httpClient,
        IdentityApiClient identityApiClient)
    {
        _httpClient = httpClient;
        _identityApiClient = identityApiClient;
    }


    public async Task<DateTime?> GetLastSuccessfulSyncAsync(
        string feedName,
        CancellationToken cancellationToken)
    {
        var token =
            await _identityApiClient.GetAccessTokenAsync(
                cancellationToken);


        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);


        var response =
            await _httpClient.GetFromJsonAsync<LastSyncResponse>(
                $"/threat/api/threat-feeds/GetLastSuccessfulSync?feedName={feedName}",
                cancellationToken);


        return response?.CompletedAtUtc;
    }


    private sealed record LastSyncResponse(
        string FeedName,
        DateTime StartedAtUtc,
        DateTime? CompletedAtUtc);
}