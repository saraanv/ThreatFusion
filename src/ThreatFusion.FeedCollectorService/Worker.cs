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
            var startedAtUtc = DateTime.UtcNow;

            var createdCount = 0;
            var updatedCount = 0;
            var unchangedCount = 0;
            var failedCount = 0;

            IReadOnlyCollection<ThreatIndicatorRequest> indicators = [];

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
                        var status =
                            await _threatApiClient.SendIndicatorAsync(
                                indicator,
                                stoppingToken);

                        switch (status)
                        {
                            case "Created":
                                createdCount++;

                                _logger.LogInformation(
                                    "Indicator created: {Value}",
                                    indicator.Value);

                                break;

                            case "Updated":
                                updatedCount++;

                                _logger.LogInformation(
                                    "Indicator updated: {Value}",
                                    indicator.Value);

                                break;

                            case "Unchanged":
                                unchangedCount++;

                                _logger.LogInformation(
                                    "Indicator unchanged: {Value}",
                                    indicator.Value);

                                break;

                            default:
                                failedCount++;

                                _logger.LogWarning(
                                    "Unknown write status '{Status}' for indicator: {Value}",
                                    status,
                                    indicator.Value);

                                break;
                        }
                    }
                    catch (Exception exception)
                    {
                        failedCount++;

                        _logger.LogError(
                            exception,
                            "Failed to process indicator: {Value}",
                            indicator.Value);
                    }
                }
            }
            catch (Exception exception)
            {
                isSuccessful = false;

                failedCount++;

                errorMessage = exception.Message;

                _logger.LogError(
                    exception,
                    "Threat feed collection failed.");
            }

            var completedAtUtc = DateTime.UtcNow;

            try
            {
                var syncRequest =
                    new ThreatFeedSyncRequest(
                        FeedName: "NVD",
                        StartedAtUtc: startedAtUtc,
                        CompletedAtUtc: completedAtUtc,
                        TotalFetched: indicators.Count,

                        CreatedCount: createdCount,
                        UpdatedCount: updatedCount,
                        UnchangedCount: unchangedCount,
                        FailedCount: failedCount,

                        IsSuccessful:
                        isSuccessful &&
                        failedCount == 0,

                        ErrorMessage:
                        errorMessage);

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
                "Fetched: {Fetched}, " +
                "Created: {Created}, " +
                "Updated: {Updated}, " +
                "Unchanged: {Unchanged}, " +
                "Failed: {Failed}",
                indicators.Count,
                createdCount,
                updatedCount,
                unchangedCount,
                failedCount);

            try
            {
                await Task.Delay(
                    TimeSpan.FromMinutes(5),
                    stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Worker is shutting down normally.
                break;
            }
        }
    }
}