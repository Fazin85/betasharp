using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Launcher.Features;

// Add support for multiple accounts.
internal sealed class AccountService(ILogger<AccountService> logger)
{
    public sealed class Account(string name, string? skin, string token, DateTimeOffset expiration)
    {
        public string Name => name;

        public string? Skin => skin;

        public string Token => token;

        public DateTimeOffset Expiration => expiration;
    }

    private readonly string _path = Path.Combine(App.Folder, "account.json");

    private Account? _account;

    public async Task UpdateAsync(string name, string? skin, string token, DateTimeOffset expiration)
    {
        _account = new Account(name, skin, token, expiration);

        await using var stream = File.OpenWrite(_path);
        await JsonSerializer.SerializeAsync<Account>(stream, _account, SourceGenerationContext.Default.Account);
    }

    public async Task<Account?> GetAsync()
    {
        if (_account is not null)
        {
            return _account;
        }

        try
        {
            await using var stream = File.OpenRead(_path);

            _account = await JsonSerializer.DeserializeAsync<Account>(stream, SourceGenerationContext.Default.Account);

            ArgumentNullException.ThrowIfNull(_account);

            return _account;
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    public void Delete()
    {
        _account = null;
        File.Delete(_path);
    }
}
