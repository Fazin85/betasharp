using BetaSharp.Server.Network;

namespace BetaSharp.Server.Internal;

public class InternalServer : MinecraftServer
{
    private readonly string _worldPath;
    private readonly Lock _difficultyLock = new();
    private readonly int _initialDifficulty;
    private int _lastDifficulty;

    public InternalServer(string worldPath, string levelName, string seed, int viewDistance, int initialDifficulty) : base(new InternalServerConfiguration(levelName, seed, viewDistance))
    {
        _worldPath = worldPath;
        logHelp = false;
        _initialDifficulty = initialDifficulty;
        _lastDifficulty = _initialDifficulty;
    }

    public void SetViewDistance(int viewDistanceChunks)
    {
        InternalServerConfiguration serverConfiguration = (InternalServerConfiguration)config;
        serverConfiguration.SetViewDistance(viewDistanceChunks);
    }

    public volatile bool isReady;

    protected override bool Init()
    {
        connections = new ConnectionListener(this);

        Log.Info("Starting internal server");

        bool result = base.Init();

        if (result)
        {
            for (int i = 0; i < worlds.Length; ++i)
            {
                if (worlds[i] != null)
                {
                    worlds[i].difficulty = _initialDifficulty;
                    worlds[i].allowSpawning(_initialDifficulty > 0, true);
                }
            }

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
