using java.io;

namespace BetaSharp.Server;

public class DedicatedPlayerManager : PlayerManager
{
    private readonly java.io.File BannedPlayersFile;
    private readonly java.io.File BANNED_IPS_FILE;
    private readonly java.io.File OPERATORS_FILE;
    private readonly java.io.File WHITELIST_FILE;

    public DedicatedPlayerManager(MinecraftServer server) : base(server)
    {
        BannedPlayersFile = server.getFile("banned-players.txt");
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
            BufferedReader reader = new(new FileReader(BannedPlayersFile));
            string var2 = "";

            while ((var2 = reader.readLine()) != null)
            {
                bannedPlayers.Add(var2.Trim().ToLower());
            }

            reader.close();
        }
        catch (Exception exception)
        {
            LOGGER.warning("Failed to load ban list: " + exception);
        }
    }

    protected override void saveBannedPlayers()
    {
        try
        {
            PrintWriter writer = new(new FileWriter(BannedPlayersFile, false));

            foreach (string var3 in bannedPlayers)
            {
                writer.println(var3);
            }

            writer.close();
        }
        catch (Exception exception)
        {
            LOGGER.warning("Failed to save ban list: " + exception);
        }
    }

    protected override void loadBannedIps()
    {
        try
        {
            bannedIps.Clear();
            BufferedReader reader = new(new FileReader(BANNED_IPS_FILE));
            string var2 = "";

            while ((var2 = reader.readLine()) != null)
            {
                bannedIps.Add(var2.Trim().ToLower());
            }

            reader.close();
        }
        catch (Exception exception)
        {
            LOGGER.warning("Failed to load ip ban list: " + exception);
        }
    }

    protected override void saveBannedIps()
    {
        try
        {
            PrintWriter writer = new(new FileWriter(BANNED_IPS_FILE, false));

            foreach (string var3 in bannedIps)
            {
                writer.println(var3);
            }

            writer.close();
        }
        catch (Exception exception)
        {
            LOGGER.warning("Failed to save ip ban list: " + exception);
        }
    }

    protected override void loadOperators()
    {
        try
        {
            ops.Clear();
            BufferedReader reader = new(new FileReader(OPERATORS_FILE));
            string var2 = "";

            while ((var2 = reader.readLine()) != null)
            {
                ops.Add(var2.Trim().ToLower());
            }

            reader.close();
        }
        catch (Exception exception)
        {
            LOGGER.warning("Failed to load operators list: " + exception);
        }
    }

    protected override void saveOperators()
    {
        try
        {
            PrintWriter writer = new(new FileWriter(OPERATORS_FILE, false));

            foreach (string var3 in ops)
            {
                writer.println(var3);
            }

            writer.close();
        }
        catch (Exception exception)
        {
            LOGGER.warning("Failed to save operators list: " + exception);
        }
    }

    protected override void loadWhitelist()
    {
        try
        {
            whitelist.Clear();
            BufferedReader reader = new(new FileReader(WHITELIST_FILE));
            string var2 = "";

            while ((var2 = reader.readLine()) != null)
            {
                whitelist.Add(var2.Trim().ToLower());
            }

            reader.close();
        }
        catch (Exception var3)
        {
            LOGGER.warning("Failed to load white-list: " + var3);
        }
    }

    protected override void saveWhitelist()
    {
        try
        {
            PrintWriter writer = new(new FileWriter(WHITELIST_FILE, false));

            foreach (string var3 in whitelist)
            {
                writer.println(var3);
            }

            writer.close();
        }
        catch (Exception exception)
        {
            LOGGER.warning("Failed to save white-list: " + exception);
        }
    }
}