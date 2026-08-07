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
            try
            {
                _logger.LogInformation(
                    "Threat feed collection started at {Time}",
                    DateTime.UtcNow);

                var indicators =
                    await _feedProvider.GetIndicatorsAsync(
                        stoppingToken);

                foreach (var indicator in indicators)
                {
                    var created =
                        await _threatApiClient
                            .SendIndicatorAsync(
                                indicator,
                                stoppingToken);

                    if (created)
                    {
                        _logger.LogInformation(
                            "Indicator imported: {Value}",
                            indicator.Value);
                    }
                    else
                    {
                        _logger.LogInformation(
                            "Indicator skipped: {Value}",
                            indicator.Value);
                    }
                }

                _logger.LogInformation(
                    "Threat feed collection completed.");
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Threat feed collection failed.");
            }

            await Task.Delay(
                TimeSpan.FromMinutes(5),
                stoppingToken);
        }
    }
}