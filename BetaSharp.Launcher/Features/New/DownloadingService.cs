using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BetaSharp.Launcher.Features.New;

internal sealed class DownloadingService(IHttpClientFactory httpClientFactory)
{
    public async Task DownloadMinecraftAsync()
    {
        var resource = await RequestClientUrlAsync();
        var client = httpClientFactory.CreateClient();

        await using var stream = await client.GetStreamAsync(resource);
        await using var file = File.OpenWrite("b1.7.3.jar");

        await stream.CopyToAsync(file);
    }

    private async Task<string> RequestVersionUrlAsync()
    {
        var client = httpClientFactory.CreateClient();

        await using var stream = await client.GetStreamAsync("https://piston-meta.mojang.com/mc/game/version_manifest_v2.json");

        var node = await JsonNode.ParseAsync(stream);
        var versions = node?["versions"]?.AsArray();

        ArgumentNullException.ThrowIfNull(versions);

        var version = versions.FirstOrDefault(version => version?["id"]?.GetValue<string>() is "b1.7.3");
        var url = version?["url"]?.GetValue<string>();

        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        return url;
    }

    private async Task<string> RequestClientUrlAsync()
    {
        var resource = await RequestVersionUrlAsync();
        var client = httpClientFactory.CreateClient();

        await using var stream = await client.GetStreamAsync(resource);

        var node = await JsonNode.ParseAsync(stream);
        var url = node?["downloads"]?["client"]?["url"]?.GetValue<string>();
        
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        return url;
    }
}