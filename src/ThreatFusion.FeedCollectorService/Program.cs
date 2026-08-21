using ThreatFusion.FeedCollectorService;
using ThreatFusion.FeedCollectorService.Providers;
using ThreatFusion.FeedCollectorService.Services;
using ThreatFusion.FeedCollectorService.Services.Providers.Nvd;

var builder =
    Host.CreateApplicationBuilder(args);

var gatewayBaseUrl =
    builder.Configuration["Gateway:BaseUrl"]
    ?? throw new InvalidOperationException(
        "Gateway BaseUrl is not configured.");

builder.Services.AddHttpClient<
    IThreatFeedProvider,
    NvdFeedProvider>(
    client =>
    {
        client.BaseAddress =
            new Uri(
                "https://services.nvd.nist.gov");

        client.Timeout =
            TimeSpan.FromMinutes(5);

        client.DefaultRequestHeaders
            .UserAgent
            .ParseAdd(
                "ThreatFusion/1.0");

        client.DefaultRequestHeaders
            .Accept
            .ParseAdd(
                "application/json");
    });

builder.Services.AddHttpClient<
    IdentityApiClient>(
    client =>
    {
        client.BaseAddress =
            new Uri(
                gatewayBaseUrl);
    });

builder.Services.AddHttpClient<
    ThreatFeedSyncClient>(
    client =>
    {
        client.BaseAddress =
            new Uri(
                gatewayBaseUrl);
    });

builder.Services.AddHttpClient<
    ThreatApiClient>(
    client =>
    {
        client.BaseAddress =
            new Uri(
                gatewayBaseUrl);
    });

builder.Services.AddHostedService<Worker>();

var host =
    builder.Build();

host.Run();