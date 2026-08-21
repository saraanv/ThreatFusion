namespace ThreatFusion.Threat.Application.Features.Watchlists.Add;

public sealed record AddToWatchlistResult(
    bool IsSuccess,
    long? WatchlistId,
    IReadOnlyCollection<string> Errors)
{
    public static AddToWatchlistResult Success(
        long watchlistId) =>
        new(
            true,
            watchlistId,
            Array.Empty<string>());

    public static AddToWatchlistResult Failure(
        params string[] errors) =>
        new(
            false,
            null,
            errors);
}