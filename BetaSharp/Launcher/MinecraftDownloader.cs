using System.Text.Json;

namespace BetaSharp.Launcher;

public class MinecraftDownloader
{
    private static readonly HttpClient _httpClient = new();
    private const string VersionManifestUrl = "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";

    public static async Task<bool> DownloadBeta173Async(IProgress<double>? progress = null)
    {
        try
        {
            var manifestResponse = await _httpClient.GetStringAsync(VersionManifestUrl);
            var manifest = JsonDocument.Parse(manifestResponse);

            string? versionUrl = null;
            foreach (var version in manifest.RootElement.GetProperty("versions").EnumerateArray())
            {
                if (version.GetProperty("id").GetString() == "b1.7.3")
                {
                    versionUrl = version.GetProperty("url").GetString();
                    break;
                }
            }

            if (versionUrl == null)
            {
                throw new Exception("Beta 1.7.3 not found in version manifest");
            }

            var versionResponse = await _httpClient.GetStringAsync(versionUrl);
            var versionData = JsonDocument.Parse(versionResponse);

            var clientUrl = versionData.RootElement
                .GetProperty("downloads")
                .GetProperty("client")
                .GetProperty("url")
                .GetString();

            if (clientUrl == null)
            {
                throw new Exception("Client download URL not found");
            }

            using var response = await _httpClient.GetAsync(clientUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? 0;
            var downloadedBytes = 0L;

            using var contentStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream("b1.7.3.jar", FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

            var buffer = new byte[8192];
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                downloadedBytes += bytesRead;

                if (totalBytes > 0)
                {
                    progress?.Report((double)downloadedBytes / totalBytes * 100);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Download error: {ex.Message}");
            return false;
        }
    }
}