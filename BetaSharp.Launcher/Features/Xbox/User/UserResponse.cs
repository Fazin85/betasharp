namespace BetaSharp.Launcher.Features.Xbox.User;

internal sealed class UserResponse
{
    public sealed class UserDisplayClaims
    {
        public sealed class UserXui
        {
            public required string Uhs { get; init; }
        }

        public required UserXui[] Xui { get; set; }
    }

    public required string Token { get; init; }

    public required UserDisplayClaims DisplayClaims { get; init; }
}
