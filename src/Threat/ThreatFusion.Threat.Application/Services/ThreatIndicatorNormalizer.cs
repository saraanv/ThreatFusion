using System.Net;
using ThreatFusion.Threat.Domain.Enums;

namespace ThreatFusion.Threat.Application.Services;

public static class ThreatIndicatorNormalizer
{
    public static string Normalize(
        IndicatorType type,
        string value)
    {
        var trimmedValue =
            value.Trim();

        return type switch
        {
            IndicatorType.IpAddress =>
                NormalizeIpAddress(
                    trimmedValue),

            IndicatorType.Domain =>
                trimmedValue
                    .TrimEnd('.')
                    .ToLowerInvariant(),

            IndicatorType.Url =>
                NormalizeUrl(
                    trimmedValue),

            IndicatorType.Email =>
                trimmedValue
                    .ToLowerInvariant(),

            IndicatorType.FileHash =>
                trimmedValue
                    .ToLowerInvariant(),

            IndicatorType.Cve =>
                trimmedValue
                    .ToUpperInvariant(),

            _ =>
                trimmedValue
        };
    }

    private static string NormalizeIpAddress(
        string value)
    {
        if (!IPAddress.TryParse(
                value,
                out var ipAddress))
        {
            return value;
        }

        return ipAddress.ToString();
    }

    private static string NormalizeUrl(
        string value)
    {
        if (!Uri.TryCreate(
                value,
                UriKind.Absolute,
                out var uri))
        {
            return value;
        }

        var builder =
            new UriBuilder(uri)
            {
                Scheme =
                    uri.Scheme.ToLowerInvariant(),

                Host =
                    uri.Host.ToLowerInvariant()
            };

        if (
            (builder.Scheme == Uri.UriSchemeHttp &&
             builder.Port == 80)
            ||
            (builder.Scheme == Uri.UriSchemeHttps &&
             builder.Port == 443)
        )
        {
            builder.Port = -1;
        }

        return builder.Uri
            .AbsoluteUri;
    }
}