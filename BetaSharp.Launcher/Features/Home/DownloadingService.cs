using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BetaSharp.Launcher.Features.Home;

internal sealed class DownloadingService(HttpClient client)
{
    private const string ExpectedHash = "af1fa04b8006d3ef78c7e24f8de4aa56f439a74d7f314827529062d5bab6db4c";

    private readonly string _path = Path.Combine(Directory.GetCurrentDirectory(), "b1.7.3.jar");

    public async Task DownloadAsync()
    {
        if (File.Exists(_path) && await ValidateAsync())
        {
            return;
        }

        await using var stream = await client.GetStreamAsync("https://launcher.mojang.com/v1/objects/43db9b498cb67058d2e12d394e6507722e71bb45/client.jar");
        await using var file = File.OpenWrite(_path);

        await stream.CopyToAsync(file);
    }

    private async Task<bool> ValidateAsync()
    {
        using var sha256 = SHA256.Create();
        await using var stream = File.OpenRead(_path);
        byte[] hashBytes = await sha256.ComputeHashAsync(stream);
        string actualHash = Convert.ToHexStringLower(hashBytes);

        return actualHash.Equals(ExpectedHash, StringComparison.OrdinalIgnoreCase);
    }
}
