using Microsoft.Extensions.Logging;

namespace BetaSharp.Server;

internal class DedicatedPlayerManager : PlayerManager
{
    private readonly ILogger<DedicatedPlayerManager> _logger = Log.Instance.For<DedicatedPlayerManager>();
    private readonly FileStream _bannedPlayersFile;
    private readonly FileStream _bannedIpsFile;
    private readonly FileStream _operatorsFile;
    private readonly FileStream _whitelistFile;

    public DedicatedPlayerManager(BetaSharpServer server) : base(server)
    {
        _bannedPlayersFile = server.getFile("banned-players.txt");
        _bannedIpsFile = server.getFile("banned-ips.txt");
        _operatorsFile = server.getFile("ops.txt");
        _whitelistFile = server.getFile("white-list.txt");

        loadBannedPlayers();
        loadBannedIps();
        loadOperators();
        loadWhitelist();
        saveBannedPlayers();
        saveBannedIps();
        saveOperators();
        saveWhitelist();
    }

    protected override void loadBannedPlayers()
    {
        try
        {
            bannedPlayers.Clear();
            using StreamReader reader = new(_bannedPlayersFile);

            while (reader.ReadLine() is { } line)
            {
                bannedPlayers.Add(line.Trim().ToLower());
            }
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Failed to load ban list: {exception}");
        }
    }

    protected override void saveBannedPlayers()
    {
        try
        {
            using StreamWriter writer = new(_bannedPlayersFile);

            foreach (string player in bannedPlayers)
            {
                writer.WriteLine(player);
            }
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Failed to save ban list: {exception}");
        }
    }

    protected override void loadBannedIps()
    {
        try
        {
            bannedIps.Clear();
            using StreamReader reader = new(_bannedIpsFile);

            while (reader.ReadLine() is { } line)
            {
                bannedIps.Add(line.Trim().ToLower());
            }
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Failed to load ip ban list: {exception}");
        }
    }

    protected override void saveBannedIps()
    {
        try
        {
            using StreamWriter writer = new(_bannedIpsFile);

            foreach (string ip in bannedIps)
            {
                writer.WriteLine(ip);
            }
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Failed to save ip ban list: {exception}");
        }
    }

    protected override void loadOperators()
    {
        try
        {
            ops.Clear();
            using StreamReader reader = new(_operatorsFile);

            while (reader.ReadLine() is { } line)
            {
                ops.Add(line.Trim().ToLower());
            }
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Failed to load ip ban list: {exception}");
        }
    }

    protected override void saveOperators()
    {
        try
        {
            using StreamWriter writer = new(_operatorsFile);

            foreach (string op in ops)
            {
                writer.WriteLine(op);
            }
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Failed to save ip ban list: {exception}");
        }
    }

    protected override void loadWhitelist()
    {
        try
        {
            whitelist.Clear();
            using StreamReader reader = new(_whitelistFile);

            while (reader.ReadLine() is { } line)
            {
                whitelist.Add(line.Trim().ToLower());
            }
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Failed to load white-list: {exception}");
        }
    }

    protected override void saveWhitelist()
    {
        try
        {
            using StreamWriter writer = new(_whitelistFile);

            foreach (string name in whitelist)
            {
                writer.WriteLine(name);
            }
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Failed to save white-list: {exception}");
        }
    }
}
