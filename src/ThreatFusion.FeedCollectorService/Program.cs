using ThreatFusion.FeedCollectorService;
using ThreatFusion.FeedCollectorService.Providers;
using ThreatFusion.FeedCollectorService.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IThreatFeedProvider, MockThreatFeedProvider>();

builder.Services.AddHttpClient<ThreatApiClient>((serviceProvider, client) =>
{
    var configuration =
        serviceProvider.GetRequiredService<IConfiguration>();

    var baseUrl =
        configuration["ThreatApi:BaseUrl"]
        ?? throw new InvalidOperationException(
            "ThreatApi BaseUrl is not configured.");

    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();