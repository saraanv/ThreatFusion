namespace ThreatFusion.FeedCollectorService.Models;

public sealed record LoginRequest(
    string Email,
    string Password);