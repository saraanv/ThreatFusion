namespace ThreatFusion.Identity.Application.Common.Models;

public sealed record TokenResult(
    string AccessToken,
    DateTime ExpiresAtUtc);