using ThreatFusion.FeedCollectorService.Models;
using ThreatFusion.FeedCollectorService.Providers;
using ThreatFusion.FeedCollectorService.Services;

namespace ThreatFusion.FeedCollectorService;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly IThreatFeedProvider _feedProvider;

    private readonly ThreatApiClient _threatApiClient;

    public Worker(
        ILogger<Worker> logger,
        IThreatFeedProvider feedProvider,
        ThreatApiClient threatApiClient)
    {
        _logger = logger;
        _feedProvider = feedProvider;
        _threatApiClient = threatApiClient;
    }

   protected override async Task ExecuteAsync(
    CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        var startedAtUtc =
            DateTime.UtcNow;

        var importedCount = 0;
        var skippedCount = 0;
        var failedCount = 0;

        IReadOnlyCollection<ThreatIndicatorRequest>
            indicators = [];

        string? errorMessage = null;

        var isSuccessful = true;

        try
        {
            _logger.LogInformation(
                "Threat feed collection started at {Time}",
                startedAtUtc);

            indicators =
                await _feedProvider.GetIndicatorsAsync(
                    stoppingToken);

            foreach (var indicator in indicators)
            {
                try
                {
                    var created =
                        await _threatApiClient
                            .SendIndicatorAsync(
                                indicator,
                                stoppingToken);

                    if (created)
                    {
                        importedCount++;

                        _logger.LogInformation(
                            "Indicator imported: {Value}",
                            indicator.Value);
                    }
                    else
                    {
                        skippedCount++;

                        _logger.LogInformation(
                            "Indicator skipped: {Value}",
                            indicator.Value);
                    }
                }
                catch (Exception exception)
                {
                    failedCount++;

                    _logger.LogError(
                        exception,
                        "Failed to import indicator: {Value}",
                        indicator.Value);
                }
            }
        }
        catch (Exception exception)
        {
            isSuccessful = false;
            failedCount = 1;
            errorMessage = exception.Message;

            _logger.LogError(
                exception,
                "Threat feed collection failed.");
        }

        var completedAtUtc =
            DateTime.UtcNow;

        try
        {
            var syncRequest =
                new ThreatFeedSyncRequest(
                    FeedName: "NVD",
                    StartedAtUtc: startedAtUtc,
                    CompletedAtUtc: completedAtUtc,
                    TotalFetched: indicators.Count,
                    ImportedCount: importedCount,
                    SkippedCount: skippedCount,
                    FailedCount: failedCount,
                    IsSuccessful:
                        isSuccessful &&
                        failedCount == 0,
                    ErrorMessage: errorMessage);

            await _threatApiClient.RegisterSyncAsync(
                syncRequest,
                stoppingToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to register threat feed synchronization.");
        }

        _logger.LogInformation(
            "Threat feed collection completed. " +
            "Fetched: {Fetched}, Imported: {Imported}, " +
            "Skipped: {Skipped}, Failed: {Failed}",
            indicators.Count,
            importedCount,
            skippedCount,
            failedCount);

        await Task.Delay(
            TimeSpan.FromMinutes(5),
            stoppingToken);
    }
}
}