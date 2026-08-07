using System.Net.Http.Json;
using ThreatFusion.FeedCollectorService.Models;

namespace ThreatFusion.FeedCollectorService.Services;

public sealed class IdentityApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    private string? _accessToken;
    private DateTime _expiresAtUtc;

    public IdentityApiClient(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GetAccessTokenAsync(
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_accessToken) &&
            _expiresAtUtc >
            DateTime.UtcNow.AddMinutes(2))
        {
            return _accessToken;
        }

        var email =
            _configuration["FeedCollectorAccount:Email"]
            ?? throw new InvalidOperationException(
                "FeedCollector email is not configured.");

        var password =
            _configuration["FeedCollectorAccount:Password"]
            ?? throw new InvalidOperationException(
                "FeedCollector password is not configured.");

        var request =
            new LoginRequest(
                email,
                password);

        var response =
            await _httpClient.PostAsJsonAsync(
                "/identity/api/auth/LoginUser",
                request,
                cancellationToken);

        response.EnsureSuccessStatusCode();

        var loginResponse =
            await response.Content
                .ReadFromJsonAsync<LoginResponse>(
                    cancellationToken);

        if (loginResponse is null)
        {
            throw new InvalidOperationException(
                "Identity API returned an invalid login response.");
        }

        _accessToken =
            loginResponse.AccessToken;

        _expiresAtUtc =
            loginResponse.ExpiresAtUtc;

        return _accessToken;
    }
}