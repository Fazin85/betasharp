using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using BetaSharp.Launcher.Features.Mojang.Entitlements;
using BetaSharp.Launcher.Features.Mojang.Profile;
using BetaSharp.Launcher.Features.Mojang.Token;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Launcher.Features.Mojang;

internal sealed class MojangClient(ILogger<MojangClient> logger, IHttpClientFactory clientFactory)
{
    public async Task<TokenResponse> GetTokenAsync(TokenRequest request)
    {
        var client = clientFactory.CreateClient(nameof(MojangClient));

        var response = await client.PostAsync(
            "https://api.minecraftservices.com/authentication/login_with_xbox",
            JsonContent.Create(request, SourceGenerationContext.Default.TokenRequest));

        await using var stream = await response.Content.ReadAsStreamAsync();

        var instance = JsonSerializer.Deserialize<TokenResponse>(stream, SourceGenerationContext.Default.TokenResponse);

        ArgumentNullException.ThrowIfNull(instance);

        return instance;
    }

    public async Task<EntitlementsResponse> GetEntitlementsAsync(string token)
    {
        var client = clientFactory.CreateClient(nameof(MojangClient));

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await client.GetAsync("https://api.minecraftservices.com/entitlements");

        await using var stream = await response.Content.ReadAsStreamAsync();

        var instance = JsonSerializer.Deserialize<EntitlementsResponse>(stream, SourceGenerationContext.Default.EntitlementsResponse);

        ArgumentNullException.ThrowIfNull(instance);

        return instance;
    }

    public async Task<ProfileResponse> GetProfileAsync(string token)
    {
        var client = clientFactory.CreateClient(nameof(MojangClient));

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await client.GetAsync("https://api.minecraftservices.com/minecraft/profile");

        await using var stream = await response.Content.ReadAsStreamAsync();

        var instance = JsonSerializer.Deserialize<ProfileResponse>(stream, SourceGenerationContext.Default.ProfileResponse);

        ArgumentNullException.ThrowIfNull(instance);

        return instance;
    }
}

