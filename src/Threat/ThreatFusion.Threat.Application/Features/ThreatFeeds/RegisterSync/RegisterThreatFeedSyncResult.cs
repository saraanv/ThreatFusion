namespace ThreatFusion.Threat.Application
    .Features.ThreatFeeds.RegisterSync;

public sealed record RegisterThreatFeedSyncResult(
    bool IsSuccess,
    long? SyncId,
    IReadOnlyCollection<string> Errors)
{
    public static RegisterThreatFeedSyncResult Success(
        long syncId) =>
        new(
            true,
            syncId,
            Array.Empty<string>());

    public static RegisterThreatFeedSyncResult Failure(
        params string[] errors) =>
        new(
            false,
            null,
            errors);
}