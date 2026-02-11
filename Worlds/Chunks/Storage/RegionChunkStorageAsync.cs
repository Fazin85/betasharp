using betareborn.NBT;

namespace betareborn.Worlds.Chunks.Storage
{
    public class RegionChunkStorageAsync : ChunkStorage
    {

        public readonly java.io.File worldDir;

        public RegionChunkStorageAsync(java.io.File dir)
        {
            worldDir = dir;
        }

        public Chunk loadChunk(World world, int chunkX, int chunkZ)
        {
            NbtTagCompound? var4 = Region.RegionCache.readChunkNBT(worldDir, chunkX, chunkZ);
            if (var4 != null)
            {
                if (!var4.HasKey("Level"))
                {
                    java.lang.System.@out.println("Chunk file at " + chunkX + "," + chunkZ + " is missing level data, skipping");
                    return null;
                }
                else if (!var4.GetCompoundTag("Level").HasKey("Blocks"))
                {
                    java.lang.System.@out.println("Chunk file at " + chunkX + "," + chunkZ + " is missing block data, skipping");
                    return null;
                }
                else
                {
                    Chunk var6 = RegionChunkStorage.loadChunkFromNbt(world, var4.GetCompoundTag("Level"));
                    if (!var6.chunkPosEquals(chunkX, chunkZ))
                    {
                        java.lang.System.@out.println("Chunk file at " + chunkX + "," + chunkZ + " is in the wrong location; relocating. (Expected " + chunkX + ", " + chunkZ + ", got " + var6.x + ", " + var6.z + ")");
                        var4.SetInteger("xPos", chunkX);
                        var4.SetInteger("zPos", chunkZ);
                        var6 = RegionChunkStorage.loadChunkFromNbt(world, var4.GetCompoundTag("Level"));
                    }

                    var6.fill();
                    return var6;
                }
            }
            else
            {
                return null;
            }
        }

        public void saveChunk(World world, Chunk chunk, Action onSave, long _)
        {
            NbtTagCompound var4 = new();
            NbtTagCompound var5 = new();
            var4.SetTag("Level", var5);
            RegionChunkStorage.storeChunkInCompound(chunk, world, var5);
            Region.RegionCache.writeChunkNBT(worldDir, chunk.x, chunk.z, var4);
        }

        public void saveEntities(World world, Chunk chunk)
        {
        }

        public void tick()
        {
        }

        public void flush()
        {
        }

        public void flushToDisk()
        {
            Region.RegionCache.unloadAllRegions(worldDir);
            Region.RegionCache.resetLoadedCounters();
        }
    }
}