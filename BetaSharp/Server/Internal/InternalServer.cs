using BetaSharp.Server.Network;

namespace BetaSharp.Server.Internal;

public class InternalServer : MinecraftServer
{
    private readonly string _worldPath;

    public InternalServer(string worldPath, string levelName, string seed, int viewDistance) : base(new InternalServerConfiguration(levelName, seed, viewDistance))
    {
        _worldPath = worldPath;
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
        connections = new ConnectionListener(this);

        LOGGER.info($"Starting internal server");

        bool result = base.Init();
        if (result)
        {
            isReady = true;
        }
        return result;
    }

    public override java.io.File getFile(string path)
    {
        return new(System.IO.Path.Combine(_worldPath, path));
    }
}
