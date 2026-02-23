using System;

namespace BetaSharp.Launcher.Features.Accounts;

public sealed class Account
{
    public required string Name { get; init; }

    public required string? Skin { get; init; }

    public required string Token { get; set; }

    public required DateTimeOffset Expiration { get; set; }
}
