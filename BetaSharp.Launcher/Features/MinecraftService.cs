using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;

namespace BetaSharp.Launcher.Features;

internal sealed class MinecraftService(HttpClient client)
{
    public sealed class MinecraftTokenRequest
    {
        [JsonPropertyName("identityToken")]
        public required string IdentityToken { get; init; }
    }

    public sealed class MinecraftTokenResponse
    {
        [JsonPropertyName("access_token")]
        public required string AccessToken { get; init; }

        [JsonPropertyName("expires_in")]
        public required int ExpiresIn { get; init; }
    }

    public sealed class MinecraftProfileResponse
    {
        public sealed class Skin
        {
            [JsonPropertyName("url")]
            public required string Url { get; init; }

            [JsonPropertyName("state")]
            public required string State { get; init; }
        }

        [JsonPropertyName("name")]
        public required string Name { get; init; }

        [JsonPropertyName("skins")]
        public required Skin[] Skins { get; init; }
    }

    public async Task<(string Token, DateTimeOffset Expiration)> GetTokenAsync(string token, string hash)
    {
        var response = await client.PostAsync<MinecraftTokenRequest, MinecraftTokenResponse>(
            "https://api.minecraftservices.com/authentication/login_with_xbox",
            new MinecraftTokenRequest { IdentityToken = $"XBL3.0 x={hash};{token}" },
            SourceGenerationContext.Default.MinecraftTokenRequest,
            SourceGenerationContext.Default.MinecraftTokenResponse);

        ArgumentNullException.ThrowIfNull(response);

        return (response.AccessToken, DateTimeOffset.Now.AddSeconds(response.ExpiresIn));
    }

    public async Task<(string Name, string? Skin)> GetProfileAsync(string token)
    {
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await client.GetFromJsonAsync<MinecraftProfileResponse>(
            "https://api.minecraftservices.com/minecraft/profile",
            SourceGenerationContext.Default.MinecraftProfileResponse);

        ArgumentNullException.ThrowIfNull(response);

        return (response.Name, response.Skins.FirstOrDefault(skin => skin.State.Equals("active", StringComparison.InvariantCultureIgnoreCase))?.Url);
    }

    public async Task<CroppedBitmap> GetFaceAsync(string skin)
    {
        await using var stream = await client.GetStreamAsync(skin);

        var memory = new MemoryStream();

        await stream.CopyToAsync(memory);

        memory.Position = 0;

        return new CroppedBitmap(new Bitmap(memory), new PixelRect(8, 8, 8, 8));
    }
}
