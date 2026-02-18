using BetaSharp.Worlds.Chunks;

namespace BetaSharp.Worlds.Gen.Carvers;

public class Carver
{
    protected int radius = 8;
    protected java.util.Random rand = new();

    public virtual void carve(ChunkSource source, World world, int chunkX, int chunkZ, byte[] blocks)
    {
        rand.setSeed(world.getSeed());
        long rand1 = rand.nextLong() / 2L * 2L + 1L;
        long rand2 = rand.nextLong() / 2L * 2L + 1L;

        for (int currentX = chunkX - radius; currentX <= chunkX + radius; ++currentX)
        {
            for (int currentZ = chunkZ - radius; currentZ <= chunkZ + radius; ++currentZ)
            {
                rand.setSeed(currentX * rand1 + currentZ * rand2 ^ world.getSeed());
                func_868_a(world, currentX, currentZ, chunkX, chunkZ, blocks);
            }
        }

    }

    protected virtual void func_868_a(World world, int chunkX, int chunkZ, int centerChunkX, int centerChunkZ, byte[] blocks)
    {
    }
}
