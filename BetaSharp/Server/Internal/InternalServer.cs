using BetaSharp.Server.Network;

namespace BetaSharp.Server.Internal;

public class InternalServer : MinecraftServer
{
    private readonly string _worldPath;
    private readonly Lock _difficultyLock = new();
    private int _lastDifficulty = -1;

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

        _lastDifficulty = worlds[0].difficulty;

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

    public void SetDifficulty(int difficulty)
    {
        lock (_difficultyLock)
        {
            if (_lastDifficulty != difficulty)
            {
                _lastDifficulty = difficulty;
                for (int i = 0; i < worlds.Length; ++i)
                {
                    if (worlds[i] != null)
                    {
                        worlds[i].difficulty = difficulty;
                        worlds[i].allowSpawning(difficulty > 0, true);
                    }
                }

                string difficultyName = difficulty switch
                {
                    0 => "Peaceful",
                    1 => "Easy",
                    2 => "Normal",
                    3 => "Hard",
                    _ => "Unknown"
                };

                playerManager?.sendToAll(new BetaSharp.Network.Packets.Play.ChatMessagePacket($"Difficulty set to {difficultyName}"));
            }
        }
    }
}
