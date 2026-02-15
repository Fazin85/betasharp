using BetaSharp.Server.Network;
using BetaSharp.Server.Threading;
using java.lang;
using java.net;
using java.util.logging;

namespace BetaSharp.Server.Dedicated;

public class DedicatedServer : MinecraftServer
{
    public DedicatedServer(IServerConfiguration config) : base(config)
    {
    }

    protected override PlayerManager CreatePlayerManager()
    {
        return new DedicatedPlayerManager(this);
    }

    protected override bool Init()
    {
        ConsoleInputThread var1 = new(this);
        var1.setDaemon(true);
        var1.start();

        LOGGER.info("Starting minecraft server version Beta 1.7.3");
        if (Runtime.getRuntime().maxMemory() / 1024L / 1024L < 512L)
        {
            LOGGER.warning("**** NOT ENOUGH RAM!");
            LOGGER.warning("To start the server with more ram, launch it as \"java -Xmx1024M -Xms1024M -jar minecraft_server.jar\"");
        }

        LOGGER.info("Loading properties");

        string var2 = config.GetServerIp("");
        InetAddress var3 = null;
        if (var2.Length > 0)
        {
            var3 = InetAddress.getByName(var2);
        }

        int var4 = config.GetServerPort(25565);
        LOGGER.info("Starting Minecraft server on " + (var2.Length == 0 ? "*" : var2) + ":" + var4);

        try
        {
            connections = new ConnectionListener(this, var3, var4);
        }
        catch (java.io.IOException var13)
        {
            LOGGER.warning("**** FAILED TO BIND TO PORT!");
            LOGGER.log(Level.WARNING, "The exception was: " + var13.toString());
            LOGGER.warning("Perhaps a server is already running on that port?");
            return false;
        }

        if (!onlineMode)
        {
            LOGGER.warning("**** SERVER IS RUNNING IN OFFLINE/INSECURE MODE!");
            LOGGER.warning("The server will make no attempt to authenticate usernames. Beware.");
            LOGGER.warning(
                "While this makes the game possible to play without internet access, it also opens up the ability for hackers to connect with any username they choose."
            );
            LOGGER.warning("To change this, set \"online-mode\" to \"true\" in the server.settings file.");
        }

        return base.Init();
    }

    public static void Main(string[] args)
    {
        try
        {
            if (!JarValidator.ValidateJar("b1.7.3.jar"))
            {
                Console.WriteLine("Downloading b1.7.3.jar");
                var task = MinecraftDownloader.DownloadBeta173Async();
                task.Wait();

                if (task.Result)
                {
                    Console.WriteLine("Successfully downloaded b1.7.3.jar");
                }
                else
                {
                    Console.WriteLine("Failed to download b1.7.3.jar");
                    return;
                }
            }

            DedicatedServerConfiguration config = new(new java.io.File("server.properties"));
            DedicatedServer server = new(config);

            new RunServerThread(server, "Server thread").start();
        }
        catch (java.lang.Exception var2)
        {
            LOGGER.log(Level.SEVERE, "Failed to start the minecraft server", (Throwable)var2);
        }
    }

    public override java.io.File getFile(string path)
    {
        return new java.io.File(path);
    }
}