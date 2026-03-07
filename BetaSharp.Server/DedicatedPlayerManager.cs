using Microsoft.Extensions.Logging;

namespace BetaSharp.Server;

internal class DedicatedPlayerManager : PlayerManager
{
    private readonly ILogger<DedicatedPlayerManager> _logger = Log.Instance.For<DedicatedPlayerManager>();
    private readonly string BANNED_PLAYERS_FILE;
    private readonly string BANNED_IPS_FILE;
    private readonly string OPERATORS_FILE;
    private readonly string WHITELIST_FILE;

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
            if (File.Exists(BANNED_PLAYERS_FILE))
            {
                foreach (string line in File.ReadAllLines(BANNED_PLAYERS_FILE))
                {
                    string trimmed = line.Trim().ToLower();
                    if (trimmed.Length > 0) bannedPlayers.Add(trimmed);
                }
            }
        }
        catch (Exception var3)
        {
            _logger.LogWarning($"Failed to load ban list: {var3}");
        }
    }

    protected override void saveBannedPlayers()
    {
        try
        {
            File.WriteAllLines(BANNED_PLAYERS_FILE, bannedPlayers);
        }
        catch (Exception var4)
        {
            _logger.LogWarning($"Failed to save ban list: {var4}");
        }
    }

    protected override void loadBannedIps()
    {
        try
        {
            bannedIps.Clear();
            if (File.Exists(BANNED_IPS_FILE))
            {
                foreach (string line in File.ReadAllLines(BANNED_IPS_FILE))
                {
                    string trimmed = line.Trim().ToLower();
                    if (trimmed.Length > 0) bannedIps.Add(trimmed);
                }
            }
        }
        catch (Exception var3)
        {
            _logger.LogWarning($"Failed to load ip ban list: {var3}");
        }
    }

    protected override void saveBannedIps()
    {
        try
        {
            File.WriteAllLines(BANNED_IPS_FILE, bannedIps);
        }
        catch (Exception var4)
        {
            _logger.LogWarning($"Failed to save ip ban list: {var4}");
        }
    }

    protected override void loadOperators()
    {
        try
        {
            ops.Clear();
            if (File.Exists(OPERATORS_FILE))
            {
                foreach (string line in File.ReadAllLines(OPERATORS_FILE))
                {
                    string trimmed = line.Trim().ToLower();
                    if (trimmed.Length > 0) ops.Add(trimmed);
                }
            }
        }
        catch (Exception var3)
        {
            _logger.LogWarning($"Failed to load operators list: {var3}");
        }
    }

    protected override void saveOperators()
    {
        try
        {
            File.WriteAllLines(OPERATORS_FILE, ops);
        }
        catch (Exception var4)
        {
            _logger.LogWarning($"Failed to save operators list: {var4}");
        }
    }

    protected override void loadWhitelist()
    {
        try
        {
            whitelist.Clear();
            if (File.Exists(WHITELIST_FILE))
            {
                foreach (string line in File.ReadAllLines(WHITELIST_FILE))
                {
                    string trimmed = line.Trim().ToLower();
                    if (trimmed.Length > 0) whitelist.Add(trimmed);
                }
            }
        }
        catch (Exception var3)
        {
            _logger.LogWarning($"Failed to load white-list: {var3}");
        }
    }

    protected override void saveWhitelist()
    {
        try
        {
            File.WriteAllLines(WHITELIST_FILE, whitelist);
        }
        catch (Exception var4)
        {
            _logger.LogWarning($"Failed to save white-list: {var4}");
        }
    }
}
