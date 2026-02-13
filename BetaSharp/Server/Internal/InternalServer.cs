using BetaSharp.Server.Network;
using java.net;
using java.util.logging;

namespace BetaSharp.Server.Internal;

public class InternalServer : MinecraftServer
{
    private readonly string worldPath;

    public int Port
    {
        get
        {
            lock (portLock)
            {
                return port;
            }
        }
    }

    private int port;
    private readonly Lock portLock = new();

    public InternalServer(string worldPath, string levelName, string seed, int viewDistance) : base(new InternalServerConfiguration(levelName, seed, viewDistance))
    {
        this.worldPath = worldPath;
        logHelp = false;
    }

    public void SetViewDistance(int viewDistanceChunks)
    {
        InternalServerConfiguration serverConfiguration = (InternalServerConfiguration)config;
        serverConfiguration.SetViewDistance(viewDistanceChunks);
    }

    public volatile bool isReady = false;

    protected override bool Init()
    {
        try
        {
            connections = new ConnectionListener(this, InetAddress.getByName("127.0.0.1"), 0);
            lock (portLock)
            {
                port = connections.port;
            }
        }
        catch (java.io.IOException ex)
        {
            LOGGER.warning("**** FAILED TO BIND TO PORT!");
            LOGGER.log(Level.WARNING, "The exception was: " + ex.toString());
            LOGGER.warning("Perhaps a server is already running on that port?");
            return false;
        }

        LOGGER.info($"Starting internal server on port {port}");

        bool result = base.Init();
        if (result)
        {
            isReady = true;
        }
        return result;
    }

    public override java.io.File getFile(string path)
    {
        return new(System.IO.Path.Combine(worldPath, path));
    }
}