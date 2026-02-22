using BetaSharp.Worlds;
using BetaSharp.Worlds.Storage;

namespace BetaSharp.Server.Worlds;

public class ReadOnlyServerWorld : ServerWorld
{
    public ReadOnlyServerWorld(MinecraftServer server, IWorldStorage storage, string saveName, int dimension, long seed, ServerWorld del) : base(server, storage, saveName, dimension, seed)
    {
        persistentStateManager = del.persistentStateManager;
        properties = new DerivingWorldProperties(del.getProperties());
        Rules = del.Rules;
    }
}
