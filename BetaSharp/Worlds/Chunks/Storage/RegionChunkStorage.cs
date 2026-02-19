using BetaSharp.Blocks.Entities;
using BetaSharp.Entities;
using BetaSharp.NBT;
using System;
using System.IO;

namespace BetaSharp.Worlds.Chunks.Storage;

public class RegionChunkStorage : IChunkStorage
{
    private readonly string _dir;

    public RegionChunkStorage(string dir)
    {
        _dir = dir;
    }

    public Chunk LoadChunk(World world, int chunkX, int chunkZ)
    {
        using ChunkDataStream s = RegionIo.GetChunkInputStream(_dir, chunkX, chunkZ);
        if (s == null)
        {
            return null;
        }

        Stream stream = s.Stream;

        if (stream != null)
        {
            NBTTagCompound root = NbtIo.Read(stream);
            if (!root.HasKey("Level"))
            {
                Log.Info($"Chunk file at {chunkX},{chunkZ} is missing level data, skipping");
                return null;
            }

            NBTTagCompound levelTag = root.GetCompoundTag("Level");
            if (!levelTag.HasKey("Blocks"))
            {
                Log.Info($"Chunk file at {chunkX},{chunkZ} is missing block data, skipping");
                return null;
            }

            Chunk chunk = LoadChunkFromNbt(world, levelTag);

            if (!chunk.chunkPosEquals(chunkX, chunkZ))
            {
                Log.Info($"Chunk file at {chunkX},{chunkZ} is in the wrong location; relocating. (Expected {chunkX}, {chunkZ}, got {chunk.x}, {chunk.z})");
                levelTag.SetInteger("xPos", chunkX);
                levelTag.SetInteger("zPos", chunkZ);
                chunk = LoadChunkFromNbt(world, levelTag);
            }

            chunk.fill();
            return chunk;
        }

        return null;
    }

    public void SaveChunk(World world, Chunk chunk, Action unused1, long unused2)
    {
        try
        {
            using Stream stream = RegionIo.GetChunkOutputStream(_dir, chunk.x, chunk.z);
            NBTTagCompound root = new();
            NBTTagCompound levelTag = new();

            root.SetTag("Level", levelTag);
            StoreChunkInCompound(chunk, world, levelTag);

            NbtIo.Write(root, stream);

            WorldProperties props = world.getProperties();
            props.SizeOnDisk += (long)RegionIo.GetSizeDelta(_dir, chunk.x, chunk.z);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    public static void StoreChunkInCompound(Chunk chunk, World world, NBTTagCompound nbt)
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
        NBTTagList entityList = new();

        for (int i = 0; i < chunk.entities.Length; ++i)
        {
            foreach (Entity entity in chunk.entities[i])
            {
                chunk.lastSaveHadEntities = true;
                NBTTagCompound entityTag = new();
                if (entity.saveSelfNbt(entityTag))
                {
                    entityList.SetTag(entityTag);
                }
            }
        }
        nbt.SetTag("Entities", entityList);

        NBTTagList tileEntityList = new();
        foreach (BlockEntity tileEntity in chunk.blockEntities.Values)
        {
            NBTTagCompound tileTag = new();
            tileEntity.writeNbt(tileTag);
            tileEntityList.SetTag(tileTag);
        }
        nbt.SetTag("TileEntities", tileEntityList);
    }

    public static Chunk LoadChunkFromNbt(World world, NBTTagCompound nbt)
    {
        int x = nbt.GetInteger("xPos");
        int z = nbt.GetInteger("zPos");

        Chunk chunk = new(world, x, z);
        chunk.blocks = nbt.GetByteArray("Blocks");
        chunk.meta = new ChunkNibbleArray(nbt.GetByteArray("Data"));
        chunk.skyLight = new ChunkNibbleArray(nbt.GetByteArray("SkyLight"));
        chunk.blockLight = new ChunkNibbleArray(nbt.GetByteArray("BlockLight"));
        chunk.heightmap = nbt.GetByteArray("HeightMap");
        chunk.terrainPopulated = nbt.GetBoolean("TerrainPopulated");

        if (!chunk.meta.isArrayInitialized())
        {
            chunk.meta = new ChunkNibbleArray(chunk.blocks.Length);
        }

        if (chunk.heightmap == null || !chunk.skyLight.isArrayInitialized())
        {
            chunk.heightmap = new byte[256];
            chunk.skyLight = new ChunkNibbleArray(chunk.blocks.Length);
            chunk.populateHeightMap();
        }

        if (!chunk.blockLight.isArrayInitialized())
        {
            chunk.blockLight = new ChunkNibbleArray(chunk.blocks.Length);
            chunk.populateLight();
        }

        NBTTagList entityList = nbt.GetTagList("Entities");
        if (entityList != null)
        {
            for (int i = 0; i < entityList.TagCount(); ++i)
            {
                NBTTagCompound entityTag = (NBTTagCompound)entityList.TagAt(i);
                Entity entity = EntityRegistry.getEntityFromNbt(entityTag, world);
                chunk.lastSaveHadEntities = true;
                if (entity != null)
                {
                    chunk.addEntity(entity);
                }
            }
        }

        NBTTagList tileEntityList = nbt.GetTagList("TileEntities");
        if (tileEntityList != null)
        {
            for (int i = 0; i < tileEntityList.TagCount(); ++i)
            {
                NBTTagCompound tileTag = (NBTTagCompound)tileEntityList.TagAt(i);
                BlockEntity tileEntity = BlockEntity.createFromNbt(tileTag);
                if (tileEntity != null)
                {
                    chunk.addBlockEntity(tileEntity);
                }
            }
        }

        return chunk;
    }

    public void SaveEntities(World world, Chunk chunk) { }
    public void Tick() { }
    public void Flush() { }
    public void FlushToDisk() { }
}
