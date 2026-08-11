namespace ThreatFusion.FeedCollectorService.Models;

public sealed record ThreatIndicatorWriteResponse(
    long IndicatorId,
    string Status);