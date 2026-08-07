namespace ThreatFusion.FeedCollectorService.Providers.UrlHaus;

public sealed class UrlHausOptions
{
    public const string SectionName = "UrlHaus";

    public string AuthKey { get; set; } = string.Empty;

    public string BaseUrl { get; set; } = "https://urlhaus-api.abuse.ch";
}