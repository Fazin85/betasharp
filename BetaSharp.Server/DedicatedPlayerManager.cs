using Microsoft.Extensions.Logging;

namespace BetaSharp.Server;

internal class DedicatedPlayerManager : PlayerManager
{
    private readonly ILogger<DedicatedPlayerManager> _logger = Log.Instance.For<DedicatedPlayerManager>();
    private readonly FileStream BANNED_PLAYERS_FILE;
    private readonly FileStream BANNED_IPS_FILE;
    private readonly FileStream OPERATORS_FILE;
    private readonly FileStream WHITELIST_FILE;

    public DedicatedPlayerManager(BetaSharpServer server) : base(server)
    {
        BANNED_PLAYERS_FILE = server.getFile("banned-players.txt");
        BANNED_IPS_FILE = server.getFile("banned-ips.txt");
        OPERATORS_FILE = server.getFile("ops.txt");
        WHITELIST_FILE = server.getFile("white-list.txt");

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
            using StreamReader reader = new(BANNED_PLAYERS_FILE);
            string? line = "";

            while ((line = reader.ReadLine()) != null)
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
            using StreamWriter writer = new(BANNED_PLAYERS_FILE);

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
            using StreamReader reader = new(BANNED_IPS_FILE);
            string? line = "";

            while ((line = reader.ReadLine()) != null)
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
            using StreamWriter writer = new(BANNED_IPS_FILE);

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
            using StreamReader reader = new(OPERATORS_FILE);
            string? line = "";

            while ((line = reader.ReadLine()) != null)
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
            using StreamWriter writer = new(OPERATORS_FILE);

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
            using StreamReader reader = new(WHITELIST_FILE);
            string? line = "";

            while ((line = reader.ReadLine()) != null)
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
            using StreamWriter writer = new(WHITELIST_FILE);

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
