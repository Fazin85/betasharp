using System.Text.Json.Serialization;

namespace BetaSharp.Launcher.Features.Xbox.Token;

internal sealed class TokenResponse
{
    [JsonPropertyName("token")]
    public required string Value { get; init; }
}
