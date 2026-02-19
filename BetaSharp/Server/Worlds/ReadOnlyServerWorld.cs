using BetaSharp.Worlds;
using BetaSharp.Worlds.Storage;

namespace BetaSharp.Server.Worlds;

public class ReadOnlyServerWorld : ServerWorld
{
    public ReadOnlyServerWorld(MinecraftServer server, WorldStorage storage, String saveName, int dimension, long seed, ServerWorld del) : base(server, storage, saveName, dimension, seed)
    {
        persistentStateManager = del.persistentStateManager;
        properties = new DerivingWorldProperties(del.getProperties());
    }
}
