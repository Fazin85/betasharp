using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Chunks;

namespace BetaSharp.Worlds.Gen.Carvers;

public class Carver
{
    protected int radius = 8;
    protected JavaRandom rand = new();

    public virtual void carve(ChunkSource source, World world, int chunkX, int chunkZ, byte[] blocks)
    {
        rand.SetSeed(world.getSeed());
        long rand1 = rand.NextLong() / 2L * 2L + 1L;
        long rand2 = rand.NextLong() / 2L * 2L + 1L;

        for (int currentX = chunkX - radius; currentX <= chunkX + radius; ++currentX)
        {
            for (int currentZ = chunkZ - radius; currentZ <= chunkZ + radius; ++currentZ)
            {
                rand.SetSeed(currentX * rand1 + currentZ * rand2 ^ world.getSeed());
                func_868_a(world, currentX, currentZ, chunkX, chunkZ, blocks);
            }
        }

    }

    protected virtual void func_868_a(World world, int chunkX, int chunkZ, int centerChunkX, int centerChunkZ, byte[] blocks)
    {
    }
}
