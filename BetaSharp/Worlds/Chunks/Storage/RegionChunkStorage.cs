using BetaSharp.Blocks.Entities;
using BetaSharp.Entities;
using BetaSharp.NBT;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Worlds.Chunks.Storage;

internal class RegionChunkStorage : IChunkStorage
{
    private readonly ILogger<RegionChunkStorage> _logger = Log.Instance.For<RegionChunkStorage>();
    private readonly java.io.File dir;

    public RegionChunkStorage(string dir)
    {
        this.dir = new java.io.File(dir);
    }

    public Chunk LoadChunk(World world, int chunkX, int chunkZ)
    {
        using ChunkDataStream s = RegionIo.GetChunkInputStream(dir, chunkX, chunkZ);
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
                _logger.LogInformation($"Chunk file at {chunkX},{chunkZ} is missing level data, skipping");
                return null;
            }
            else if (!var5.GetCompoundTag("Level").HasKey("Blocks"))
            {
                _logger.LogInformation($"Chunk file at {chunkX},{chunkZ} is missing block data, skipping");
                return null;
            }
            else
            {
                Chunk var6 = LoadChunkFromNbt(world, var5.GetCompoundTag("Level"));
                if (!var6.ChunkPosEquals(chunkX, chunkZ))
                {
                    _logger.LogInformation($"Chunk file at {chunkX},{chunkZ} is in the wrong location; relocating. (Expected {chunkX}, {chunkZ}, got {var6.X}, {var6.Z})");
                    var5.SetInteger("xPos", chunkX);
                    var5.SetInteger("zPos", chunkZ);
                    var6 = LoadChunkFromNbt(world, var5.GetCompoundTag("Level"));
                }

                var6.Fill();
                return var6;
            }
        }
        else
        {
            return null;
        }
    }

    public void SaveChunk(World world, Chunk chunk, Action unused1, long unused2)
    {
        try
        {
            using Stream stream = RegionIo.GetChunkOutputStream(dir, chunk.X, chunk.Z);
            NBTTagCompound tag = new();
            NBTTagCompound var5 = new();
            tag.SetTag("Level", var5);
            storeChunkInCompound(chunk, world, var5);
            NbtIo.Write(tag, stream);
            WorldProperties var6 = world.getProperties();
            var6.SizeOnDisk = var6.SizeOnDisk + (long)RegionIo.getSizeDelta(dir, chunk.X, chunk.Z);
        }
        catch (Exception var7)
        {
            _logger.LogError(var7, "Exception");
        }
    }

    public static void storeChunkInCompound(Chunk chunk, World world, NBTTagCompound nbt)
    {
        nbt.SetInteger("xPos", chunk.X);
        nbt.SetInteger("zPos", chunk.Z);
        nbt.SetLong("LastUpdate", world.getTime());
        nbt.SetByteArray("Blocks", chunk.Blocks);
        nbt.SetByteArray("Data", chunk.Meta.Bytes);
        nbt.SetByteArray("SkyLight", chunk.SkyLight.Bytes);
        nbt.SetByteArray("BlockLight", chunk.BlockLight.Bytes);
        nbt.SetByteArray("HeightMap", chunk.HeightMap);
        nbt.SetBoolean("TerrainPopulated", chunk.TerrainPopulated);
        chunk.LastSaveHadEntities = false;
        NBTTagList var3 = new();

        NBTTagCompound var7;
        for (int var4 = 0; var4 < chunk.Entities.Length; ++var4)
        {
            foreach (Entity var6 in chunk.Entities[var4])
            {
                chunk.LastSaveHadEntities = true;
                var7 = new NBTTagCompound();
                if (var6.saveSelfNbt(var7))
                {
                    var3.SetTag(var7);
                }
            }
        }

        nbt.SetTag("Entities", var3);
        NBTTagList var8 = new();

        foreach (BlockEntity var9 in chunk.BlockEntities.Values)
        {
            var7 = new NBTTagCompound();
            var9.writeNbt(var7);
            var8.SetTag(var7);
        }

        nbt.SetTag("TileEntities", var8);
    }

    public static Chunk LoadChunkFromNbt(World world, NBTTagCompound nbt)
    {
        int x = nbt.GetInteger("xPos");
        int y = nbt.GetInteger("zPos");
        byte[] blocks = nbt.GetByteArray("Blocks");
        ChunkNibbleArray meta = new ChunkNibbleArray(nbt.GetByteArray("Data"));
        ChunkNibbleArray skyLight = new ChunkNibbleArray(nbt.GetByteArray("SkyLight"));
        ChunkNibbleArray blockLight = new ChunkNibbleArray(nbt.GetByteArray("BlockLight"));
        byte[] heightMap = nbt.GetByteArray("HeightMap");
        bool terrainPopulated = nbt.GetBoolean("TerrainPopulated");

        Chunk chunk = new(world, x, y,blocks, meta,skyLight, blockLight, heightMap, terrainPopulated);

        if (terrainPopulated)
        {
            chunk.MarkReadyForNetwork();
        }

        if (!chunk.Meta.IsInitialized)
        {
            chunk.Meta = new ChunkNibbleArray(chunk.Blocks.Length);
        }

        if (chunk.HeightMap == null || !chunk.SkyLight.IsInitialized)
        {
            chunk.HeightMap = new byte[256];
            chunk.SkyLight = new ChunkNibbleArray(chunk.Blocks.Length);
            chunk.PopulateHeightMap();
        }

        if (!chunk.BlockLight.IsInitialized)
        {
            chunk.BlockLight = new ChunkNibbleArray(chunk.Blocks.Length);
            chunk.PopulateLight();
        }

        NBTTagList var5 = nbt.GetTagList("Entities");
        if (var5 != null)
        {
            for (int var6 = 0; var6 < var5.TagCount(); ++var6)
            {
                NBTTagCompound var7 = (NBTTagCompound)var5.TagAt(var6);
                Entity var8 = EntityRegistry.getEntityFromNbt(var7, world);
                chunk.LastSaveHadEntities = true;
                if (var8 != null)
                {
                    chunk.AddEntity(var8);
                }
            }
        }

        NBTTagList var10 = nbt.GetTagList("TileEntities");
        if (var10 != null)
        {
            for (int var11 = 0; var11 < var10.TagCount(); ++var11)
            {
                NBTTagCompound var12 = (NBTTagCompound)var10.TagAt(var11);
                BlockEntity var9 = BlockEntity.CreateFromNbt(var12);
                if (var9 != null)
                {
                    chunk.AddBlockEntity(var9);
                }
            }
        }

        return chunk;
    }

    public void SaveEntities(World world, Chunk chunk)
    {
    }

    public void Tick()
    {
    }

    public void Flush()
    {
    }

    public void FlushToDisk()
    {
    }
}
