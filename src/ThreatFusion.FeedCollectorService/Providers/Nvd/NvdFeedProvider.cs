using System.Text.Json;
using ThreatFusion.FeedCollectorService.Models;
using ThreatFusion.FeedCollectorService.Providers;

namespace ThreatFusion.FeedCollectorService.Services.Providers.Nvd;

public sealed class NvdFeedProvider : IThreatFeedProvider
{
    private const int MaxRetries = 3;

    private readonly HttpClient _httpClient;
    private readonly ILogger<NvdFeedProvider> _logger;
    private readonly IConfiguration _configuration;
    private readonly ThreatFeedSyncClient _syncClient;

    public NvdFeedProvider(
        HttpClient httpClient,
        ILogger<NvdFeedProvider> logger,
        IConfiguration configuration,
        ThreatFeedSyncClient syncClient)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
        _syncClient = syncClient;
    }

    public async Task<IReadOnlyCollection<ThreatIndicatorRequest>>
        GetIndicatorsAsync(
            CancellationToken cancellationToken)
    {
        var lookbackHours =
            _configuration.GetValue<int>(
                "Nvd:LookbackHours");

        var resultsPerPage =
            _configuration.GetValue<int>(
                "Nvd:ResultsPerPage");

        if (lookbackHours <= 0)
        {
            lookbackHours = 24;
        }

        if (resultsPerPage <= 0)
        {
            resultsPerPage = 100;
        }

        var endDate = DateTime.UtcNow;

        var lastSuccessfulSync =
            await _syncClient.GetLastSuccessfulSyncAsync(
                "NVD",
                cancellationToken);

        var fallbackStartDate =
            endDate.AddHours(-lookbackHours);

        DateTime startDate;

        if (lastSuccessfulSync.HasValue)
        {
            var incrementalStartDate =
                lastSuccessfulSync.Value
                    .AddMinutes(-5);

            // در Development اجازه نمی‌دهیم یک Sync قدیمی
            // چند هزار رکورد را دوباره بکشد.
            startDate =
                incrementalStartDate > fallbackStartDate
                    ? incrementalStartDate
                    : fallbackStartDate;

            _logger.LogInformation(
                "Incremental NVD sync. " +
                "Last successful sync: {LastSync}. " +
                "Effective start date: {StartDate}.",
                lastSuccessfulSync.Value,
                startDate);
        }
        else
        {
            startDate =
                fallbackStartDate;

            _logger.LogInformation(
                "No previous successful NVD sync found. " +
                "Using lookback period of {LookbackHours} hours.",
                lookbackHours);
        }

        var startDateText =
            startDate.ToString(
                "yyyy-MM-ddTHH:mm:ss.fff");

        var endDateText =
            endDate.ToString(
                "yyyy-MM-ddTHH:mm:ss.fff");

        var allIndicators =
            new List<ThreatIndicatorRequest>();

        var startIndex = 0;
        var totalResults = int.MaxValue;

        while (startIndex < totalResults)
        {
            var url =
                "/rest/json/cves/2.0" +
                $"?lastModStartDate={Uri.EscapeDataString(startDateText)}" +
                $"&lastModEndDate={Uri.EscapeDataString(endDateText)}" +
                $"&resultsPerPage={resultsPerPage}" +
                $"&startIndex={startIndex}";

            var nvdResponse =
                await GetPageWithRetryAsync(
                    url,
                    startIndex,
                    cancellationToken);

            if (nvdResponse is null)
            {
                throw new InvalidOperationException(
                    $"NVD returned an invalid response for StartIndex {startIndex}.");
            }

            totalResults =
                nvdResponse.TotalResults;

            _logger.LogInformation(
                "NVD page returned {Count}. Total results: {TotalResults}.",
                nvdResponse.Vulnerabilities.Count,
                totalResults);

            if (nvdResponse.Vulnerabilities.Count == 0)
            {
                break;
            }

            var indicators =
                nvdResponse.Vulnerabilities
                    .Select(MapToThreatIndicator)
                    .ToList();

            allIndicators.AddRange(
                indicators);

            /*
             * معمولاً همان resultsPerPage برگشتی را داریم.
             * ولی اگر NVD مقدار غیرمنتظره صفر بدهد،
             * از مقدار config استفاده می‌کنیم تا loop بی‌نهایت نشود.
             */
            var pageSize =
                nvdResponse.ResultsPerPage > 0
                    ? nvdResponse.ResultsPerPage
                    : resultsPerPage;

            startIndex += pageSize;

            if (startIndex < totalResults)
            {
                // برای فشار نیاوردن به NVD
                await Task.Delay(
                    TimeSpan.FromSeconds(7),
                    cancellationToken);
            }
        }

        _logger.LogInformation(
            "NVD collection completed. Total collected: {Count}",
            allIndicators.Count);

        return allIndicators;
    }

    private async Task<NvdResponse?> GetPageWithRetryAsync(
        string url,
        int startIndex,
        CancellationToken cancellationToken)
    {
        for (var attempt = 1;
             attempt <= MaxRetries;
             attempt++)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching NVD page. StartIndex: {StartIndex}, Attempt: {Attempt}/{MaxRetries}",
                    startIndex,
                    attempt,
                    MaxRetries);

                using var httpResponse =
                    await _httpClient.GetAsync(
                        url,
                        HttpCompletionOption.ResponseHeadersRead,
                        cancellationToken);

                httpResponse.EnsureSuccessStatusCode();

                await using var responseStream =
                    await httpResponse.Content
                        .ReadAsStreamAsync(
                            cancellationToken);

                var nvdResponse =
                    await JsonSerializer
                        .DeserializeAsync<NvdResponse>(
                            responseStream,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            },
                            cancellationToken);

                return nvdResponse;
            }
            catch (HttpRequestException exception)
                when (attempt < MaxRetries)
            {
                var delay =
                    TimeSpan.FromSeconds(
                        attempt * 10);

                _logger.LogWarning(
                    exception,
                    "NVD network request failed for StartIndex {StartIndex}. " +
                    "Retrying in {DelaySeconds} seconds.",
                    startIndex,
                    delay.TotalSeconds);

                await Task.Delay(
                    delay,
                    cancellationToken);
            }
            catch (TaskCanceledException exception)
                when (!cancellationToken.IsCancellationRequested &&
                      attempt < MaxRetries)
            {
                var delay =
                    TimeSpan.FromSeconds(
                        attempt * 10);

                _logger.LogWarning(
                    exception,
                    "NVD request timed out for StartIndex {StartIndex}. " +
                    "Retrying in {DelaySeconds} seconds.",
                    startIndex,
                    delay.TotalSeconds);

                await Task.Delay(
                    delay,
                    cancellationToken);
            }catch (IOException exception)
                when (attempt < MaxRetries)
            {
                var delay =
                    TimeSpan.FromSeconds(
                        attempt * 10);

                _logger.LogWarning(
                    exception,
                    "NVD response stream failed for StartIndex {StartIndex}. " +
                    "Retrying in {DelaySeconds} seconds.",
                    startIndex,
                    delay.TotalSeconds);

                await Task.Delay(
                    delay,
                    cancellationToken);
            }
        }

        throw new HttpRequestException(
            $"NVD request failed after {MaxRetries} attempts. " +
            $"StartIndex: {startIndex}.");
    }

    private static ThreatIndicatorRequest MapToThreatIndicator(
        NvdVulnerabilityItem item)
    {
        var description =
            Truncate(
                item.Cve.Descriptions
                    .FirstOrDefault(x =>
                        string.Equals(
                            x.Lang,
                            "en",
                            StringComparison.OrdinalIgnoreCase))
                    ?.Value,
                2000);

        if (!string.IsNullOrWhiteSpace(description) &&
            description.Length > 2000)
        {
            description =
                description[..2000];
        }

        var cvss =
            item.Cve.Metrics.CvssMetricV31
                .FirstOrDefault()
                ?.CvssData
            ??
            item.Cve.Metrics.CvssMetricV30
                .FirstOrDefault()
                ?.CvssData
            ??
            item.Cve.Metrics.CvssMetricV2
                .FirstOrDefault()
                ?.CvssData;

        var cwe =
            item.Cve.Weaknesses
                .SelectMany(x =>
                    x.Description)
                .FirstOrDefault(x =>
                    string.Equals(
                        x.Lang,
                        "en",
                        StringComparison.OrdinalIgnoreCase))
                ?.Value;

        var referenceUrl =
            item.Cve.References
                .FirstOrDefault()
                ?.Url;

        var severity =
            cvss?.BaseSeverity
                ?.ToUpperInvariant() switch
            {
                "LOW" => 1,
                "MEDIUM" => 2,
                "HIGH" => 3,
                "CRITICAL" => 4,

                // اگر هنوز NVD severity نداده باشد.
                _ => 0
            };

        return new ThreatIndicatorRequest(
            Type: 8,
            Value: item.Cve.Id,
            Severity: severity,
            Confidence: 80,
            SourceName: "NVD",
            Description: description,
            FirstSeenUtc: item.Cve.Published,
            LastSeenUtc: item.Cve.LastModified,
            CvssScore: cvss?.BaseScore,
            CvssVersion: cvss?.Version,
            CvssVector: cvss?.VectorString,
            CweId: cwe,
            ReferenceUrl: referenceUrl);
    }private static string? Truncate(
        string? value,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return value.Length <= maxLength
            ? value
            : value[..maxLength];
    }
}
