namespace BetaSharp.Launcher.Features.Xbox.Token;

internal sealed class TokenRequest
{
    internal sealed class TokenProperties
    {
        public string SandboxId => "RETAIL";

        public required string[] UserTokens { get; init; }
    }

    public required TokenProperties Properties { get; init; }

    public string RelyingParty => "rp://api.minecraftservices.com/";

    public string TokenType => "JWT";
}
