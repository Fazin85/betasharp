using System.Text.Json.Serialization;

namespace BetaSharp.Launcher.Features.Accounts;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(Account))]
internal sealed partial class AccountSerializerContext : JsonSerializerContext;
