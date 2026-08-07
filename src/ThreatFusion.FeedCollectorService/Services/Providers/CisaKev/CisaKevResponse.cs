using System.Text.Json.Serialization;

namespace ThreatFusion.FeedCollectorService.Providers.CisaKev;

public sealed class CisaKevResponse
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("catalogVersion")]
    public string CatalogVersion { get; set; } = string.Empty;

    [JsonPropertyName("dateReleased")]
    public DateTime? DateReleased { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("vulnerabilities")]
    public List<CisaKevVulnerability> Vulnerabilities { get; set; } = [];
}

public sealed class CisaKevVulnerability
{
    [JsonPropertyName("cveID")]
    public string CveId { get; set; } = string.Empty;

    [JsonPropertyName("vendorProject")]
    public string VendorProject { get; set; } = string.Empty;

    [JsonPropertyName("product")]
    public string Product { get; set; } = string.Empty;

    [JsonPropertyName("vulnerabilityName")]
    public string VulnerabilityName { get; set; } = string.Empty;

    [JsonPropertyName("dateAdded")]
    public DateTime? DateAdded { get; set; }

    [JsonPropertyName("shortDescription")]
    public string ShortDescription { get; set; } = string.Empty;

    [JsonPropertyName("requiredAction")]
    public string RequiredAction { get; set; } = string.Empty;

    [JsonPropertyName("dueDate")]
    public DateTime? DueDate { get; set; }

    [JsonPropertyName("knownRansomwareCampaignUse")]
    public string? KnownRansomwareCampaignUse { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}