using System.Text.Json.Serialization;
using BetaSharp.Launcher.Features.Xbox.Token;
using BetaSharp.Launcher.Features.Xbox.User;

namespace BetaSharp.Launcher.Features.Xbox;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(UserRequest))]
[JsonSerializable(typeof(UserRequest.UserProperties))]
[JsonSerializable(typeof(UserResponse))]
[JsonSerializable(typeof(UserResponse.UserDisplayClaims))]
[JsonSerializable(typeof(UserResponse.UserDisplayClaims.UserXui))]
[JsonSerializable(typeof(TokenRequest))]
[JsonSerializable(typeof(TokenRequest.TokenProperties))]
[JsonSerializable(typeof(TokenResponse))]
internal sealed partial class XboxSerializerContext : JsonSerializerContext;
