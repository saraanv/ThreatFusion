namespace ThreatFusion.FeedCollectorService.Models;

public sealed record LoginResponse(
    string AccessToken,
    DateTime ExpiresAtUtc);