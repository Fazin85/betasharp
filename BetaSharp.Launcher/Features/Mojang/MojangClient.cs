using System.Net.Http;
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

        return await client.PostAsync(
            $"{Base}/authentication/login_with_xbox",
            new TokenRequest { Value = $"XBL3.0 x={hash};{token}" },
            MojangSerializerContext.Default.TokenRequest,
            MojangSerializerContext.Default.TokenResponse);
    }

    public async Task<EntitlementsResponse> GetEntitlementsAsync(string token)
    {
        var client = clientFactory.CreateClient(nameof(MojangClient));

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        return await client.GetAsync($"{Base}/entitlements", MojangSerializerContext.Default.EntitlementsResponse);
    }

    public async Task<ProfileResponse> GetProfileAsync(string token)
    {
        var client = clientFactory.CreateClient(nameof(MojangClient));

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        return await client.GetAsync($"{Base}/minecraft/profile", MojangSerializerContext.Default.ProfileResponse);
    }
}
