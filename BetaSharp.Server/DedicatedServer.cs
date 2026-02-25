using BetaSharp.Server.Network;
using BetaSharp.Server.Threading;
using java.lang;
using java.net;
using Microsoft.Extensions.Logging;
using Exception = System.Exception;

namespace BetaSharp.Server;

public class DedicatedServer(IServerConfiguration config) : MinecraftServer(config)
{
    private static readonly ILogger<DedicatedServer> s_logger = Log.Instance.For<DedicatedServer>();

    protected override PlayerManager CreatePlayerManager()
    {
        return new DedicatedPlayerManager(this);
    }

    protected override bool Init()
    {
        ConsoleInputThread var1 = new(this);
        var1.setDaemon(true);
        var1.start();

        s_logger.LogInformation("Starting minecraft server version Beta 1.7.3");
        if (Runtime.getRuntime().maxMemory() / 1024L / 1024L < 512L)
        {
            s_logger.LogWarning("**** NOT ENOUGH RAM!");
            s_logger.LogWarning("To start the server with more ram, launch it as \"java -Xmx1024M -Xms1024M -jar minecraft_server.jar\"");
        }

        s_logger.LogInformation("Loading properties");

        string var2 = config.GetServerIp("");
        InetAddress var3 = null;
        if (var2.Length > 0)
        {
            var3 = InetAddress.getByName(var2);
        }

        int var4 = config.GetServerPort(25565);
        s_logger.LogInformation($"Starting Minecraft server on {(var2.Length == 0 ? "*" : var2)}:{var4}");

        try
        {
            connections = new ConnectionListener(this, var3, var4);
        }
        catch (java.io.IOException ex)
        {
            s_logger.LogWarning("**** FAILED TO BIND TO PORT!");
            s_logger.LogWarning($"The exception was: {ex}");
            s_logger.LogWarning("Perhaps a server is already running on that port?");
            return false;
        }

        if (!onlineMode)
        {
            s_logger.LogWarning("**** SERVER IS RUNNING IN OFFLINE/INSECURE MODE!");
            s_logger.LogWarning("The server will make no attempt to authenticate usernames. Beware.");
            s_logger.LogWarning("While this makes the game possible to play without internet access, it also opens up the ability for hackers to connect with any username they choose.");
            s_logger.LogWarning("To change this, set \"online-mode\" to \"true\" in the server.settings file.");
        }

        return base.Init();
    }

    public static void Main(string[] args)
    {
        Log.Instance.Initialize(Directory.GetCurrentDirectory());

        try
        {
            DedicatedServerConfiguration config = new(new java.io.File("server.properties"));
            DedicatedServer server = new(config);

            new RunServerThread(server, "Server thread").start();
        }
        catch (Exception e)
        {
            s_logger.LogError($"Failed to start the minecraft server: {e}");
        }
    }

    public override java.io.File getFile(string path)
    {
        return new java.io.File(path);
    }
}
