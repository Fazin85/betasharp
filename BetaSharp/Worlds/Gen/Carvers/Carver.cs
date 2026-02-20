using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Chunks;

namespace BetaSharp.Worlds.Gen.Carvers;

public class Carver
{
    protected int radius = 8;
    protected JavaRandom rand = new();

    /// <summary>
    /// Attempts to generate a cave in the current chunk
    /// </summary>
    /// <param name="source">The chunk source</param>
    /// <param name="world">The world this cave is being generated in</param>
    /// <param name="chunkX">X-Coordinate of the chunk</param>
    /// <param name="chunkZ">Z-Coordinate of the chunk</param>
    /// <param name="blocks">1D Array of Blocks within this chunk</param>
    public virtual void carve(ChunkSource source, World world, int chunkX, int chunkZ, byte[] blocks)
    {
        rand.SetSeed(world.getSeed());
        long xOffset = rand.NextLong() / 2L * 2L + 1L;
        long yOffset = rand.NextLong() / 2L * 2L + 1L;

        for (int currentX = chunkX - radius; currentX <= chunkX + radius; ++currentX)
        {
            for (int currentZ = chunkZ - radius; currentZ <= chunkZ + radius; ++currentZ)
            {
                rand.SetSeed(currentX * xOffset + currentZ * yOffset ^ world.getSeed());
                CarveCaves(world, currentX, currentZ, chunkX, chunkZ, blocks);
            }
        }

    }

    protected virtual void CarveCaves(World world, int chunkX, int chunkZ, int centerChunkX, int centerChunkZ, byte[] blocks)
    {
    }
}
