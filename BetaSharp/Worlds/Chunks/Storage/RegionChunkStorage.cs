using BetaSharp.Blocks.Entities;
using BetaSharp.Entities;
using BetaSharp.NBT;

namespace BetaSharp.Worlds.Chunks.Storage;

public class RegionChunkStorage : ChunkStorage
{
    private readonly java.io.File dir;

    public RegionChunkStorage(java.io.File dir)
    {
        this.dir = dir;
    }

    public Chunk loadChunk(World world, int chunkX, int chunkZ)
    {
        using ChunkDataStream s = RegionIo.getChunkInputStream(dir, chunkX, chunkZ);
        if (s == null)
        {
            return null;
        }

        Stream var4 = s.Stream;

        if (var4 != null)
        {
            NBTTagCompound var5 = NbtIo.Read(var4);
            if (!var5.HasKey("Level"))
            {
                Log.Info($"Chunk file at {chunkX},{chunkZ} is missing level data, skipping");
                return null;
            }
            else if (!var5.GetCompoundTag("Level").HasKey("Blocks"))
            {
                Log.Info($"Chunk file at {chunkX},{chunkZ} is missing block data, skipping");
                return null;
            }
            else
            {
                Chunk var6 = loadChunkFromNbt(world, var5.GetCompoundTag("Level"));
                if (!var6.chunkPosEquals(chunkX, chunkZ))
                {
                    Log.Info($"Chunk file at {chunkX},{chunkZ} is in the wrong location; relocating. (Expected {chunkX}, {chunkZ}, got {var6.x}, {var6.z})");
                    var5.SetInteger("xPos", chunkX);
                    var5.SetInteger("zPos", chunkZ);
                    var6 = loadChunkFromNbt(world, var5.GetCompoundTag("Level"));
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

    public void saveChunk(World world, Chunk chunk, Action unused1, long unused2)
    {
        try
        {
            using Stream stream = RegionIo.getChunkOutputStream(dir, chunk.x, chunk.z);
            NBTTagCompound tag = new();
            NBTTagCompound var5 = new();
            tag.SetTag("Level", var5);
            storeChunkInCompound(chunk, world, var5);
            NbtIo.Write(tag, stream);
            WorldProperties var6 = world.getProperties();
            var6.SizeOnDisk = var6.SizeOnDisk + (long)RegionIo.getSizeDelta(dir, chunk.x, chunk.z);
        }
        catch (Exception var7)
        {
            Log.Error(var7);
        }
    }

    public static void storeChunkInCompound(Chunk chunk, World world, NBTTagCompound nbt)
    {
        nbt.SetInteger("xPos", chunk.x);
        nbt.SetInteger("zPos", chunk.z);
        nbt.SetLong("LastUpdate", world.getTime());
        nbt.SetByteArray("Blocks", chunk.blocks);
        nbt.SetByteArray("Data", chunk.meta.bytes);
        nbt.SetByteArray("SkyLight", chunk.skyLight.bytes);
        nbt.SetByteArray("BlockLight", chunk.blockLight.bytes);
        nbt.SetByteArray("HeightMap", chunk.heightmap);
        nbt.SetBoolean("TerrainPopulated", chunk.terrainPopulated);
        chunk.lastSaveHadEntities = false;
        NBTTagList var3 = new();

        NBTTagCompound var7;
        for (int var4 = 0; var4 < chunk.entities.Length; ++var4)
        {
            foreach (Entity var6 in chunk.entities[var4])
            {
                chunk.lastSaveHadEntities = true;
                var7 = new NBTTagCompound();
                if (var6.saveSelfNbt(var7))
                {
                    var3.SetTag(var7);
                }
            }
        }

        nbt.SetTag("Entities", var3);
        NBTTagList var8 = new();

        foreach (BlockEntity var9 in chunk.blockEntities.Values)
        {
            var7 = new NBTTagCompound();
            var9.writeNbt(var7);
            var8.SetTag(var7);
        }

        nbt.SetTag("TileEntities", var8);
    }

    public static Chunk loadChunkFromNbt(World world, NBTTagCompound nbt)
    {
        int var2 = nbt.GetInteger("xPos");
        int var3 = nbt.GetInteger("zPos");
        Chunk var4 = new(world, var2, var3);
        var4.blocks = nbt.GetByteArray("Blocks");
        var4.meta = new ChunkNibbleArray(nbt.GetByteArray("Data"));
        var4.skyLight = new ChunkNibbleArray(nbt.GetByteArray("SkyLight"));
        var4.blockLight = new ChunkNibbleArray(nbt.GetByteArray("BlockLight"));
        var4.heightmap = nbt.GetByteArray("HeightMap");
        var4.terrainPopulated = nbt.GetBoolean("TerrainPopulated");
        if (!var4.meta.isArrayInitialized())
        {
            var4.meta = new ChunkNibbleArray(var4.blocks.Length);
        }

        if (var4.heightmap == null || !var4.skyLight.isArrayInitialized())
        {
            var4.heightmap = new byte[256];
            var4.skyLight = new ChunkNibbleArray(var4.blocks.Length);
            var4.populateHeightMap();
        }

        if (!var4.blockLight.isArrayInitialized())
        {
            var4.blockLight = new ChunkNibbleArray(var4.blocks.Length);
            var4.populateLight();
        }

        NBTTagList var5 = nbt.GetTagList("Entities");
        if (var5 != null)
        {
            for (int var6 = 0; var6 < var5.TagCount(); ++var6)
            {
                NBTTagCompound var7 = (NBTTagCompound)var5.TagAt(var6);
                Entity var8 = EntityRegistry.getEntityFromNbt(var7, world);
                var4.lastSaveHadEntities = true;
                if (var8 != null)
                {
                    var4.addEntity(var8);
                }
            }
        }

        NBTTagList var10 = nbt.GetTagList("TileEntities");
        if (var10 != null)
        {
            for (int var11 = 0; var11 < var10.TagCount(); ++var11)
            {
                NBTTagCompound var12 = (NBTTagCompound)var10.TagAt(var11);
                BlockEntity var9 = BlockEntity.createFromNbt(var12);
                if (var9 != null)
                {
                    var4.addBlockEntity(var9);
                }
            }
        }

        return var4;
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
    }
}
