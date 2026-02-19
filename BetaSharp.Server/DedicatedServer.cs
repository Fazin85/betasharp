using BetaSharp.Launcher;
using BetaSharp.Server.Network;
using BetaSharp.Server.Threading;
using java.lang;
using java.net;
using java.util.logging;

namespace BetaSharp.Server;

public class DedicatedServer(IServerConfiguration config) : MinecraftServer(config)
{
    protected override PlayerManager CreatePlayerManager()
    {
        return new DedicatedPlayerManager(this);
    }

    protected override bool Init()
    {
        ConsoleInputThread var1 = new(this);
        var1.setDaemon(true);
        var1.start();

        Log.Info("Starting minecraft server version Beta 1.7.3");
        if (Runtime.getRuntime().maxMemory() / 1024L / 1024L < 512L)
        {
            Log.Warn("**** NOT ENOUGH RAM!");
            Log.Warn("To start the server with more ram, launch it as \"java -Xmx1024M -Xms1024M -jar minecraft_server.jar\"");
        }

        Log.Info("Loading properties");

        string var2 = config.GetServerIp("");
        InetAddress var3 = null;
        if (var2.Length > 0)
        {
            var3 = InetAddress.getByName(var2);
        }

        int var4 = config.GetServerPort(25565);
        Log.Info($"Starting Minecraft server on {(var2.Length == 0 ? "*" : var2)}:{var4}");

        try
        {
            connections = new ConnectionListener(this, var3, var4);
        }
        catch (java.io.IOException ex)
        {
            Log.Warn("**** FAILED TO BIND TO PORT!");
            Log.Warn($"The exception was: {ex}");
            Log.Warn("Perhaps a server is already running on that port?");
            return false;
        }

        if (!onlineMode)
        {
            Log.Warn("**** SERVER IS RUNNING IN OFFLINE/INSECURE MODE!");
            Log.Warn("The server will make no attempt to authenticate usernames. Beware.");
            Log.Warn("While this makes the game possible to play without internet access, it also opens up the ability for hackers to connect with any username they choose.");
            Log.Warn("To change this, set \"online-mode\" to \"true\" in the server.settings file.");
        }

        return base.Init();
    }

    public static void Main(string[] args)
    {
        Log.Initialize(new LogOptions(IsServer: true));
        Log.AddCrashHandlers();

        try
        {
            if (!JarValidator.ValidateJar("b1.7.3.jar"))
            {
                Log.Info("Downloading b1.7.3.jar");
                var task = MinecraftDownloader.DownloadBeta173Async();
                task.Wait();

                if (task.Result)
                {
                    Log.Info("Successfully downloaded b1.7.3.jar");
                }
                else
                {
                    Log.Error("Failed to download b1.7.3.jar");
                    return;
                }
            }

            DedicatedServerConfiguration config = new(new java.io.File("server.properties"));
            DedicatedServer server = new(config);

            new RunServerThread(server, "Server thread").start();
        }
        catch (java.lang.Exception ex)
        {
            Log.Error($"Failed to start the minecraft server: {ex}");
        }
    }

    public override java.io.File getFile(string path)
    {
        return new java.io.File(path);
    }
}
