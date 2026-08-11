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
    [JsonPropertyName("metrics")]
    public NvdMetrics Metrics { get; set; } = new();

    [JsonPropertyName("weaknesses")]
    public List<NvdWeakness> Weaknesses { get; set; } = [];

    [JsonPropertyName("references")]
    public List<NvdReference> References { get; set; } = [];
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
    [JsonPropertyName("metrics")]
    public NvdMetrics Metrics { get; set; } = new();

    [JsonPropertyName("weaknesses")]
    public List<NvdWeakness> Weaknesses { get; set; } = [];

    [JsonPropertyName("references")]
    public List<NvdReference> References { get; set; } = [];
}

public sealed class NvdDescription
{
    [JsonPropertyName("lang")]
    public string Lang { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}
public sealed class NvdMetrics
{
    [JsonPropertyName("cvssMetricV31")]
    public List<NvdCvssMetricV31> CvssMetricV31 { get; set; } = [];
}

public sealed class NvdCvssMetricV31
{
    [JsonPropertyName("cvssData")]
    public NvdCvssData CvssData { get; set; } = new();
}

public sealed class NvdCvssData
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("vectorString")]
    public string VectorString { get; set; } = string.Empty;

    [JsonPropertyName("baseScore")]
    public double BaseScore { get; set; }

    [JsonPropertyName("baseSeverity")]
    public string BaseSeverity { get; set; } = string.Empty;
}
public sealed class NvdWeakness
{
    [JsonPropertyName("description")]
    public List<NvdWeaknessDescription> Description { get; set; } = [];
}

public sealed class NvdWeaknessDescription
{
    [JsonPropertyName("lang")]
    public string Lang { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}

public sealed class NvdReference
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}