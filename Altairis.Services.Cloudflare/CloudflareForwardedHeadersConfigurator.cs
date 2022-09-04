using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;

namespace Altairis.Services.Cloudflare;

public static class CloudflareForwardedHeadersConfigurator {
    private const string CloudflareIPv4ListUrl = "https://www.cloudflare.com/ips-v4";
    private const string CloudflareIPv6ListUrl = "https://www.cloudflare.com/ips-v6";
    private const string CloudflareConnectingIpHeaderName = "CF-Connecting-IP";

    public static async Task<ForwardedHeadersOptions> GetForwardedHeadersOptionsForCloudflare() {
        var options = new ForwardedHeadersOptions {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor,
            ForwardedForHeaderName = CloudflareConnectingIpHeaderName,
        };

        // Add IPv4 headers
        var v4 = await DownloadIpList(CloudflareIPv4ListUrl);
        foreach (var item in v4) options.KnownNetworks.Add(item);

        // Add IPv6 headers
        var v6 = await DownloadIpList(CloudflareIPv6ListUrl);
        foreach (var item in v6) options.KnownNetworks.Add(item);

        return options;
    }

    private static async Task<IEnumerable<IPNetwork>> DownloadIpList(string url) {
        // Download list of IP networks
        var http = new HttpClient();
        var response = await http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var s = await response.Content.ReadAsStringAsync();

        // Split to lines
        var lines = s.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        // Parse as CIDR networks
        return lines.Select(s => {
            var data = s.Split('/');
            var ip = IPAddress.Parse(data[0]);
            var prefixLength = int.Parse(data[1]);
            return new IPNetwork(ip, prefixLength);
        });
    }

    public static IApplicationBuilder UseCloudflare(this IApplicationBuilder app) => app.UseForwardedHeaders(GetForwardedHeadersOptionsForCloudflare().Result);

}
