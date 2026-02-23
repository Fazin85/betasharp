using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BetaSharp.Launcher.Features.Mojang;
using BetaSharp.Launcher.Features.Mojang.Token;
using BetaSharp.Launcher.Features.Xbox;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Launcher.Features.Accounts;

// Add support for multiple accounts.
internal sealed class AccountsService(
    ILogger<AccountsService> logger,
    AuthenticationService authenticationService,
    XboxClient xboxClient,
    MojangClient mojangClient)
{
    private readonly string _path = Path.Combine(App.Folder, "account.json");

    private Account? _account;

    public Task InitializeAsync()
    {
        return authenticationService.InitializeAsync();
    }

    public async Task<TokenResponse?> AuthenticateAsync()
    {
        string microsoft = await authenticationService.AuthenticateAsync();

        var profile = await xboxClient.GetProfileAsync(microsoft);
        var xbox = await xboxClient.GetTokenAsync(profile.Token);

        var mojang = await mojangClient.GetTokenAsync(xbox.Value, profile.DisplayClaims.Xui[0].Uhs);

        var entitlements = await mojangClient.GetEntitlementsAsync(mojang.Value);

        if (!entitlements.Items.Any(item => item.Name is "product_minecraft" or "game_minecraft"))
        {
            logger.LogInformation("Account does not own Minecraft Java edition");
            return null;
        }

        return mojang;
    }

    public async Task<Account?> GetAsync()
    {
        if (_account is null)
        {
            if (!File.Exists(_path))
            {
                logger.LogInformation("Failed to get account's file");
                return null;
            }

            await using var stream = File.OpenRead(_path);

            _account = await JsonSerializer.DeserializeAsync(stream, AccountSerializerContext.Default.Account);

            ArgumentNullException.ThrowIfNull(_account);
        }

        if (DateTimeOffset.Now.AddMinutes(1) > _account.Expiration)
        {
            logger.LogInformation("Account's token expired");
            return null;
        }

        return _account;
    }

    public async Task RefreshAsync(string token, DateTimeOffset expiration)
    {
        var profile = await mojangClient.GetProfileAsync(token);

        _account = new Account { Name = profile.Name, Skin = profile.Skins.FirstOrDefault()?.Url, Token = token, Expiration = expiration };

        await using var stream = File.OpenWrite(_path);
        await JsonSerializer.SerializeAsync(stream, _account, AccountSerializerContext.Default.Account);
    }

    public async Task DeleteAsync()
    {
        _account = null;

        File.Delete(_path);
        await authenticationService.SignOutAsync();
    }
}
