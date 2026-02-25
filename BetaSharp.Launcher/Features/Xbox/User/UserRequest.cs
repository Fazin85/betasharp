namespace BetaSharp.Launcher.Features.Xbox.User;

internal sealed class UserRequest
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
