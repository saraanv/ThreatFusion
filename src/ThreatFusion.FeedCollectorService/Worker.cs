using ThreatFusion.FeedCollectorService.Providers;
using ThreatFusion.FeedCollectorService.Services;

namespace ThreatFusion.FeedCollectorService;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IThreatFeedProvider _provider;
    private readonly ThreatApiClient _threatApiClient;

    public Worker(
        ILogger<Worker> logger,
        IThreatFeedProvider provider,
        ThreatApiClient threatApiClient)
    {
        _logger = logger;
        _provider = provider;
        _threatApiClient = threatApiClient;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var indicators =
                await _provider.GetIndicatorsAsync(
                    stoppingToken);

            foreach (var indicator in indicators)
            {
                await _threatApiClient.SendIndicatorAsync(
                    indicator,
                    stoppingToken);

                _logger.LogInformation(
                    "Indicator sent: {Value}",
                    indicator.Value);
            }

            await Task.Delay(
                TimeSpan.FromMinutes(5),
                stoppingToken);
        }
    }
}