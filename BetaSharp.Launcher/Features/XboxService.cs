using System;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BetaSharp.Launcher.Features;

internal sealed class XboxService(HttpClient client)
{
    public sealed class XboxUserRequest
    {
        public sealed class UserProperties
        {
            public string AuthMethod => "RPS";

            public string SiteName => "user.auth.xboxlive.com";

            public required string RpsTicket { get; init; }
        }

        public required UserProperties Properties { get; init; }

        public string RelyingParty => "http://auth.xboxlive.com";

        public string TokenType => "JWT";
    }

    public sealed class XboxUserResponse
    {
        public sealed class UserDisplayClaims
        {
            public sealed class UserXui
            {
                [JsonPropertyName("uhs")]
                public required string Uhs { get; init; }
            }

            [JsonPropertyName("xui")]
            public required UserXui[] Xui { get; set; }
        }

        public required string Token { get; init; }

        public required UserDisplayClaims DisplayClaims { get; init; }
    }

    public sealed class XboxTokenRequest
    {
        public sealed class TokenProperties
        {
            public string SandboxId => "RETAIL";

            public required string[] UserTokens { get; init; }
        }

        public required TokenProperties Properties { get; init; }

        public string RelyingParty => "rp://api.minecraftservices.com/";

        public string TokenType => "JWT";
    }

    public sealed class XboxTokenResponse
    {
        public required string Token { get; init; }
    }

    public async Task<(string Token, string Hash)> GetAsync(string microsoft)
    {
        var userResponse = await client.PostAsync<XboxUserRequest, XboxUserResponse>(
            "https://user.auth.xboxlive.com/user/authenticate",
            new XboxUserRequest { Properties = new XboxUserRequest.UserProperties { RpsTicket = $"d={microsoft}" } },
            SourceGenerationContext.Default.XboxUserRequest,
            SourceGenerationContext.Default.XboxUserResponse);

        var tokenResponse = await client.PostAsync<XboxTokenRequest, XboxTokenResponse>(
            "https://xsts.auth.xboxlive.com/xsts/authorize",
            new XboxTokenRequest { Properties = new XboxTokenRequest.TokenProperties { UserTokens = [userResponse.Token] } },
            SourceGenerationContext.Default.XboxTokenRequest,
            SourceGenerationContext.Default.XboxTokenResponse);

        ArgumentNullException.ThrowIfNull(tokenResponse);

        return (tokenResponse.Token, userResponse.DisplayClaims.Xui[0].Uhs);
    }
}
