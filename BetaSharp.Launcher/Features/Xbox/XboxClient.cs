using System;
using System.Net.Http;
using System.Threading.Tasks;
using BetaSharp.Launcher.Features.Xbox.Profile;
using BetaSharp.Launcher.Features.Xbox.Token;

namespace BetaSharp.Launcher.Features.Xbox;

internal sealed class XboxClient(IHttpClientFactory clientFactory)
{
    public async Task<UserResponse> GetProfileAsync(string token)
    {
        var client = clientFactory.CreateClient(nameof(XboxClient));

        var instance = await client.PostAsync(
            "https://user.auth.xboxlive.com/user/authenticate",
            new ProfileRequest { Properties = new ProfileRequest.ProfileProperties { RpsTicket = $"d={token}" } },
            XboxSerializerContext.Default.ProfileRequest,
            XboxSerializerContext.Default.UserResponse);

        ArgumentNullException.ThrowIfNull(instance);

        return instance;
    }

    public async Task<TokenResponse> GetTokenAsync(string token)
    {
        var client = clientFactory.CreateClient(nameof(XboxClient));

        var instance = await client.PostAsync(
            "https://xsts.auth.xboxlive.com/xsts/authorize",
            new TokenRequest { Properties = new TokenRequest.TokenProperties { UserTokens = [token] } },
            XboxSerializerContext.Default.TokenRequest,
            XboxSerializerContext.Default.TokenResponse);

        ArgumentNullException.ThrowIfNull(instance);

        return instance;
    }
}
