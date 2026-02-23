using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using BetaSharp.Launcher.Features.Mojang.Entitlements;
using BetaSharp.Launcher.Features.Mojang.Profile;
using BetaSharp.Launcher.Features.Mojang.Token;

namespace BetaSharp.Launcher.Features.Mojang;

internal sealed class MojangClient(IHttpClientFactory clientFactory)
{
    private const string Base = "https://api.minecraftservices.com";

    public async Task<TokenResponse> GetTokenAsync(string token, string hash)
    {
        var client = clientFactory.CreateClient(nameof(MojangClient));

        var response = await client.PostAsync(
            $"{Base}/authentication/login_with_xbox",
            JsonContent.Create(new TokenRequest { Value = $"XBL3.0 x={hash};{token}" }, MojangSerializerContext.Default.TokenRequest));

        await using var stream = await response.Content.ReadAsStreamAsync();

        var instance = JsonSerializer.Deserialize<TokenResponse>(stream, MojangSerializerContext.Default.TokenResponse);

        ArgumentNullException.ThrowIfNull(instance);

        return instance;
    }

    public async Task<EntitlementsResponse> GetEntitlementsAsync(string token)
    {
        var client = clientFactory.CreateClient(nameof(MojangClient));

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await client.GetAsync($"{Base}/entitlements");

        await using var stream = await response.Content.ReadAsStreamAsync();

        var instance = JsonSerializer.Deserialize<EntitlementsResponse>(stream, MojangSerializerContext.Default.EntitlementsResponse);

        ArgumentNullException.ThrowIfNull(instance);

        return instance;
    }

    public async Task<ProfileResponse> GetProfileAsync(string token)
    {
        var client = clientFactory.CreateClient(nameof(MojangClient));

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await client.GetAsync($"{Base}/minecraft/profile");

        await using var stream = await response.Content.ReadAsStreamAsync();

        var instance = JsonSerializer.Deserialize<ProfileResponse>(stream, MojangSerializerContext.Default.ProfileResponse);

        ArgumentNullException.ThrowIfNull(instance);

        return instance;
    }
}
