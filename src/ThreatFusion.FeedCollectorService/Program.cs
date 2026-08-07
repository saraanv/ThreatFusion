using ThreatFusion.FeedCollectorService;
using ThreatFusion.FeedCollectorService.Providers;
using ThreatFusion.FeedCollectorService.Providers.CisaKev;
using ThreatFusion.FeedCollectorService.Services;

var builder =
    Host.CreateApplicationBuilder(args);

var gatewayBaseUrl =
    builder.Configuration["Gateway:BaseUrl"]
    ?? throw new InvalidOperationException(
        "Gateway BaseUrl is not configured.");

builder.Services.AddHttpClient<
    IThreatFeedProvider,
    CisaKevFeedProvider>(
    client =>
    {
        client.BaseAddress =
            new Uri("https://www.cisa.gov");
    });

builder.Services.AddHttpClient<IdentityApiClient>(
    client =>
    {
        client.BaseAddress =
            new Uri(gatewayBaseUrl);
    });

builder.Services.AddHttpClient<ThreatApiClient>(
    client =>
    {
        client.BaseAddress =
            new Uri(gatewayBaseUrl);
    });

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();