using System.Text.Json.Serialization;
using BetaSharp.Launcher.Features.Mojang.Entitlements;
using BetaSharp.Launcher.Features.Mojang.Profile;
using BetaSharp.Launcher.Features.Mojang.Token;

namespace BetaSharp.Launcher.Features.Mojang;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(TokenRequest))]
[JsonSerializable(typeof(TokenResponse))]
[JsonSerializable(typeof(EntitlementsResponse))]
[JsonSerializable(typeof(EntitlementsResponse.Item))]
[JsonSerializable(typeof(ProfileResponse))]
[JsonSerializable(typeof(ProfileResponse.Skin))]
internal sealed partial class MojangSerializerContext : JsonSerializerContext;
