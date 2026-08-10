using System.Text.Json.Serialization;

namespace ThreatFusion.FeedCollectorService.Services.Providers.Nvd;

public sealed class NvdResponse
{
    [JsonPropertyName("resultsPerPage")]
    public int ResultsPerPage { get; set; }

    [JsonPropertyName("startIndex")]
    public int StartIndex { get; set; }

    [JsonPropertyName("totalResults")]
    public int TotalResults { get; set; }

    [JsonPropertyName("vulnerabilities")]
    public List<NvdVulnerabilityItem> Vulnerabilities { get; set; } = [];
}

public sealed class NvdVulnerabilityItem
{
    [JsonPropertyName("cve")]
    public NvdCve Cve { get; set; } = new();
}

public sealed class NvdCve
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("published")]
    public DateTime? Published { get; set; }

    [JsonPropertyName("lastModified")]
    public DateTime? LastModified { get; set; }

    [JsonPropertyName("descriptions")]
    public List<NvdDescription> Descriptions { get; set; } = [];
}

public sealed class NvdDescription
{
    [JsonPropertyName("lang")]
    public string Lang { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}