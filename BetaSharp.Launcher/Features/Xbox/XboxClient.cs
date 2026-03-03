using System.Net.Http;
using System.Threading.Tasks;
using BetaSharp.Launcher.Features.Xbox.Token;
using BetaSharp.Launcher.Features.Xbox.User;

namespace BetaSharp.Launcher.Features.Xbox;

internal sealed class XboxClient(IHttpClientFactory clientFactory)
{
    public async Task<UserResponse> GetUserAsync(string token)
    {
        var client = clientFactory.CreateClient(nameof(XboxClient));

        return await client.PostAsync(
            "https://user.auth.xboxlive.com/user/authenticate",
            new UserRequest { Properties = new UserRequest.UserProperties { RpsTicket = $"d={token}" } },
            XboxSerializerContext.Default.UserRequest,
            XboxSerializerContext.Default.UserResponse);
    }

    public async Task<TokenResponse> GetTokenAsync(string token)
    {
        var client = clientFactory.CreateClient(nameof(XboxClient));

        return await client.PostAsync(
            "https://xsts.auth.xboxlive.com/xsts/authorize",
            new TokenRequest { Properties = new TokenRequest.TokenProperties { UserTokens = [token] } },
            XboxSerializerContext.Default.TokenRequest,
            XboxSerializerContext.Default.TokenResponse);
    }
}
