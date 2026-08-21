using System.Net;

namespace ThreatFusion.Threat.Application.Services;

public sealed class DnsEnrichmentService
{
    public async Task<IReadOnlyCollection<string>>
        ResolveIpAddressesAsync(
            string domain,
            CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(domain))
        {
            return [];
        }

        try
        {
            var addresses =
                await Dns.GetHostAddressesAsync(
                    domain,
                    cancellationToken);

            return addresses
                .Select(x => x.ToString())
                .Distinct()
                .ToList();
        }
        catch
        {
            return [];
        }
    }
}