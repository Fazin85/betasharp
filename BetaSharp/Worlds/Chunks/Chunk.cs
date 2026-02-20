using BetaSharp.Blocks;
using BetaSharp.Blocks.Entities;
using BetaSharp.Entities;
using BetaSharp.Profiling;
using BetaSharp.Util.Maths;

namespace BetaSharp.Worlds.Chunks;

public class Chunk : java.lang.Object
{
    public static bool hasSkyLight;
    public byte[] blocks;
    public bool loaded;
    public World world;
    public ChunkNibbleArray meta;
    public ChunkNibbleArray skyLight;
    public ChunkNibbleArray blockLight;
    public byte[] heightmap;
    public int minHeightmapValue;
    public readonly int x;
    public readonly int z;
    public Dictionary<BlockPos, BlockEntity> blockEntities;
    public List<Entity>[] entities;
    public bool terrainPopulated;
    public bool dirty;
    public bool empty;
    public bool lastSaveHadEntities;
    public long lastSaveTime;

    public Chunk(World world, int x, int z)
    {
        blockEntities = [];
        entities = new List<Entity>[8];
        terrainPopulated = false;
        dirty = false;
        lastSaveHadEntities = false;
        lastSaveTime = 0L;
        this.world = world;
        this.x = x;
        this.z = z;
        heightmap = new byte[256];

        for (int var4 = 0; var4 < entities.Length; ++var4)
        {
            entities[var4] = [];
        }

    }

    public Chunk(World world, byte[] blocks, int x, int z) : this(world, x, z)
    {
        this.blocks = blocks;
        meta = new ChunkNibbleArray(blocks.Length);
        skyLight = new ChunkNibbleArray(blocks.Length);
        blockLight = new ChunkNibbleArray(blocks.Length);
    }

    public virtual bool chunkPosEquals(int x, int z)
    {
        return x == this.x && z == this.z;
    }

    public virtual int getHeight(int x, int z)
    {
        return heightmap[z << 4 | x] & 255;
    }

    public virtual void populateLight()
    {
    }

    public virtual void populateHeightMapOnly()
    {
        int var1 = 127;

        for (int var2 = 0; var2 < 16; ++var2)
        {
            for (int var3 = 0; var3 < 16; ++var3)
            {
                int var4 = 127;

                for (int var5 = var2 << 11 | var3 << 7; var4 > 0 && Block.BlockLightOpacity[blocks[var5 + var4 - 1] & 255] == 0; --var4)
                {
                }

                heightmap[var3 << 4 | var2] = (byte)var4;
                if (var4 < var1)
                {
                    var1 = var4;
                }
            }
        }

        minHeightmapValue = var1;
        dirty = true;
    }

    public virtual void populateHeightMap()
    {
        int var1 = 127;

        int var2;
        int var3;
        for (var2 = 0; var2 < 16; ++var2)
        {
            for (var3 = 0; var3 < 16; ++var3)
            {
                int var4 = 127;

                int var5;
                for (var5 = var2 << 11 | var3 << 7; var4 > 0 && Block.BlockLightOpacity[blocks[var5 + var4 - 1] & 255] == 0; --var4)
                {
                }

                heightmap[var3 << 4 | var2] = (byte)var4;
                if (var4 < var1)
                {
                    var1 = var4;
                }

                if (!world.dimension.hasCeiling)
                {
                    int var6 = 15;
                    int var7 = 127;

                    do
                    {
                        var6 -= Block.BlockLightOpacity[blocks[var5 + var7] & 255];
                        if (var6 > 0)
                        {
                            skyLight.setNibble(var2, var7, var3, var6);
                        }

                        --var7;
                    } while (var7 > 0 && var6 > 0);
                }
            }
        }

        minHeightmapValue = var1;

        for (var2 = 0; var2 < 16; ++var2)
        {
            for (var3 = 0; var3 < 16; ++var3)
            {
                lightGaps(var2, var3);
            }
        }

        dirty = true;
    }

    public virtual void populateBlockLight()
    {
    }

    private void lightGaps(int x, int z)
    {
        int var3 = getHeight(x, z);
        int var4 = this.x * 16 + x;
        int var5 = this.z * 16 + z;
        lightGap(var4 - 1, var5, var3);
        lightGap(var4 + 1, var5, var3);
        lightGap(var4, var5 - 1, var3);
        lightGap(var4, var5 + 1, var3);
    }

    private void lightGap(int x, int y, int z)
    {
        int var4 = world.getTopY(x, y);
        if (var4 > z)
        {
            world.queueLightUpdate(LightType.Sky, x, z, y, x, var4, y);
            dirty = true;
        }
        else if (var4 < z)
        {
            world.queueLightUpdate(LightType.Sky, x, var4, y, x, z, y);
            dirty = true;
        }

    }

    private void updateHeightMap(int localX, int y, int localZ)
    {
        int var4 = heightmap[localZ << 4 | localX] & 255;
        int var5 = var4;
        if (y > var4)
        {
            var5 = y;
        }

        for (int var6 = localX << 11 | localZ << 7; var5 > 0 && Block.BlockLightOpacity[blocks[var6 + var5 - 1] & 255] == 0; --var5)
        {
        }

        if (var5 != var4)
        {
            world.setBlocksDirty(localX, localZ, var5, var4);
            heightmap[localZ << 4 | localX] = (byte)var5;
            int var7;
            int var8;
            int var9;
            if (var5 < minHeightmapValue)
            {
                minHeightmapValue = var5;
            }
            else
            {
                var7 = 127;

                for (var8 = 0; var8 < 16; ++var8)
                {
                    for (var9 = 0; var9 < 16; ++var9)
                    {
                        if ((heightmap[var9 << 4 | var8] & 255) < var7)
                        {
                            var7 = heightmap[var9 << 4 | var8] & 255;
                        }
                    }
                }

                minHeightmapValue = var7;
            }

            var7 = x * 16 + localX;
            var8 = z * 16 + localZ;
            if (var5 < var4)
            {
                for (var9 = var5; var9 < var4; ++var9)
                {
                    skyLight.setNibble(localX, var9, localZ, 15);
                }
            }
            else
            {
                world.queueLightUpdate(LightType.Sky, var7, var4, var8, var7, var5, var8);

                for (var9 = var4; var9 < var5; ++var9)
                {
                    skyLight.setNibble(localX, var9, localZ, 0);
                }
            }

            var9 = 15;

            int var10;
            for (var10 = var5; var5 > 0 && var9 > 0; skyLight.setNibble(localX, var5, localZ, var9))
            {
                --var5;
                int var11 = Block.BlockLightOpacity[getBlockId(localX, var5, localZ)];
                if (var11 == 0)
                {
                    var11 = 1;
                }

                var9 -= var11;
                if (var9 < 0)
                {
                    var9 = 0;
                }
            }

            while (var5 > 0 && Block.BlockLightOpacity[getBlockId(localX, var5 - 1, localZ)] == 0)
            {
                --var5;
            }

            if (var5 != var10)
            {
                world.queueLightUpdate(LightType.Sky, var7 - 1, var5, var8 - 1, var7 + 1, var10, var8 + 1);
            }

            dirty = true;
        }
    }

    public virtual int getBlockId(int x, int y, int z)
    {
        return blocks[x << 11 | z << 7 | y] & 255;
    }

    public virtual bool setBlock(int x, int y, int z, int rawId, int meta)
    {
        byte var6 = (byte)rawId;
        int var7 = heightmap[z << 4 | x] & 255;
        int var8 = blocks[x << 11 | z << 7 | y] & 255;
        if (var8 == rawId && this.meta.getNibble(x, y, z) == meta)
        {
            return false;
        }
        else
        {
            int var9 = this.x * 16 + x;
            int var10 = this.z * 16 + z;
            blocks[x << 11 | z << 7 | y] = (byte)(var6 & 255);
            if (var8 != 0 && !world.isRemote)
            {
                Block.Blocks[var8].onBreak(world, var9, y, var10);
            }

            this.meta.setNibble(x, y, z, meta);
            if (!world.dimension.hasCeiling)
            {
                if (Block.BlockLightOpacity[var6 & 255] != 0)
                {
                    if (y >= var7)
                    {
                        updateHeightMap(x, y + 1, z);
                    }
                }
                else if (y == var7 - 1)
                {
                    updateHeightMap(x, y, z);
                }

                world.queueLightUpdate(LightType.Sky, var9, y, var10, var9, y, var10);
            }

            world.queueLightUpdate(LightType.Block, var9, y, var10, var9, y, var10);
            lightGaps(x, z);
            this.meta.setNibble(x, y, z, meta);
            if (rawId != 0)
            {
                Block.Blocks[rawId].onPlaced(world, var9, y, var10);
            }

            dirty = true;
            return true;
        }
    }

    public virtual bool setBlock(int x, int y, int z, int rawId)
    {
        byte var5 = (byte)rawId;
        int var6 = heightmap[z << 4 | x] & 255;
        int var7 = blocks[x << 11 | z << 7 | y] & 255;
        if (var7 == rawId)
        {
            return false;
        }
        else
        {
            int var8 = this.x * 16 + x;
            int var9 = this.z * 16 + z;
            blocks[x << 11 | z << 7 | y] = (byte)(var5 & 255);
            if (var7 != 0)
            {
                Block.Blocks[var7].onBreak(world, var8, y, var9);
            }

            meta.setNibble(x, y, z, 0);
            if (Block.BlockLightOpacity[var5 & 255] != 0)
            {
                if (y >= var6)
                {
                    updateHeightMap(x, y + 1, z);
                }
            }
            else if (y == var6 - 1)
            {
                updateHeightMap(x, y, z);
            }

            world.queueLightUpdate(LightType.Sky, var8, y, var9, var8, y, var9);
            world.queueLightUpdate(LightType.Block, var8, y, var9, var8, y, var9);
            lightGaps(x, z);
            if (rawId != 0 && !world.isRemote)
            {
                Block.Blocks[rawId].onPlaced(world, var8, y, var9);
            }

            dirty = true;
            return true;
        }
    }

    public virtual int getBlockMeta(int x, int y, int z)
    {
        return meta.getNibble(x, y, z);
    }

    public virtual void setBlockMeta(int x, int y, int z, int meta)
    {
        dirty = true;
        this.meta.setNibble(x, y, z, meta);
    }

    public virtual int getLight(LightType lightType, int x, int y, int z)
    {
        return lightType == LightType.Sky ? skyLight.getNibble(x, y, z) : lightType == LightType.Block ? blockLight.getNibble(x, y, z) : 0;
    }

    public virtual void setLight(LightType lightType, int x, int y, int z, int value)
    {
        dirty = true;
        if (lightType == LightType.Sky)
        {
            skyLight.setNibble(x, y, z, value);
        }
        else
        {
            if (lightType != LightType.Block)
            {
                return;
            }

            blockLight.setNibble(x, y, z, value);
        }

    }

    public virtual int getLight(int x, int y, int z, int ambientDarkness)
    {
        int var5 = skyLight.getNibble(x, y, z);
        if (var5 > 0)
        {
            hasSkyLight = true;
        }

        var5 -= ambientDarkness;
        int var6 = blockLight.getNibble(x, y, z);
        if (var6 > var5)
        {
            var5 = var6;
        }

        return var5;
    }

    public virtual void addEntity(Entity entity)
    {
        lastSaveHadEntities = true;
        int var2 = MathHelper.Floor(entity.x / 16.0D);
        int var3 = MathHelper.Floor(entity.z / 16.0D);
        if (var2 != x || var3 != z)
        {
            Log.Info($"Wrong location! {entity}");
            java.lang.Thread.dumpStack();
        }

        int var4 = MathHelper.Floor(entity.y / 16.0D);
        if (var4 < 0)
        {
            var4 = 0;
        }

        if (var4 >= entities.Length)
        {
            var4 = entities.Length - 1;
        }

        entity.isPersistent = true;
        entity.chunkX = x;
        entity.chunkSlice = var4;
        entity.chunkZ = z;
        entities[var4].Add(entity);
    }

    public virtual void removeEntity(Entity entity)
    {
        removeEntity(entity, entity.chunkSlice);
    }

    public virtual void removeEntity(Entity entity, int chunkSlice)
    {
        if (chunkSlice < 0)
        {
            chunkSlice = 0;
        }

        if (chunkSlice >= entities.Length)
        {
            chunkSlice = entities.Length - 1;
        }

        entities[chunkSlice].Remove(entity);
    }

    public virtual bool isAboveMaxHeight(int x, int y, int z)
    {
        return y >= (heightmap[z << 4 | x] & 255);
    }

    public virtual BlockEntity getBlockEntity(int x, int y, int z)
    {
        BlockPos var4 = new(x, y, z);
        blockEntities.TryGetValue(var4, out BlockEntity? var5);
        if (var5 == null)
        {
            int var6 = getBlockId(x, y, z);
            if (!Block.BlocksWithEntity[var6])
            {
                return null;
            }

            BlockWithEntity var7 = (BlockWithEntity)Block.Blocks[var6];
            var7.onPlaced(world, this.x * 16 + x, y, this.z * 16 + z);
            blockEntities.TryGetValue(var4, out var5);
        }

        if (var5 != null && var5.isRemoved())
        {
            blockEntities.Remove(var4);
            return null;
        }
        else
        {
            return var5;
        }
    }

    public virtual void addBlockEntity(BlockEntity blockEntity)
    {
        int var2 = blockEntity.x - x * 16;
        int var3 = blockEntity.y;
        int var4 = blockEntity.z - z * 16;
        setBlockEntity(var2, var3, var4, blockEntity);
        if (loaded)
        {
            world.blockEntities.Add(blockEntity);
        }

    }

    public virtual void setBlockEntity(int x, int y, int z, BlockEntity blockEntity)
    {
        BlockPos var5 = new(x, y, z);
        blockEntity.world = world;
        blockEntity.x = this.x * 16 + x;
        blockEntity.y = y;
        blockEntity.z = this.z * 16 + z;
        if (getBlockId(x, y, z) != 0 && Block.Blocks[getBlockId(x, y, z)] is BlockWithEntity)
        {
            blockEntity.cancelRemoval();
            blockEntities[var5] = blockEntity;
        }
        else
        {
            Log.Info("Attempted to place a tile entity where there was no entity tile!");
        }
    }

    public virtual void removeBlockEntityAt(int localX, int y, int localZ)
    {
        BlockPos var4 = new(localX, y, localZ);
        if (loaded)
        {
            blockEntities.TryGetValue(var4, out BlockEntity? var5);
            if (var5 != null)
            {
                blockEntities.Remove(var4);
                var5.markRemoved();
            }
        }

    }

    public virtual void load()
    {
        loaded = true;
        world.processBlockUpdates(blockEntities.Values);

        for (int var1 = 0; var1 < entities.Length; ++var1)
        {
            world.addEntities(entities[var1]);
        }

    }

    public virtual void unload()
    {
        loaded = false;

        foreach (var var2 in blockEntities.Values)
        {
            var2.markRemoved();
        }

        for (int var3 = 0; var3 < entities.Length; ++var3)
        {
            world.unloadEntities(entities[var3]);
        }

    }

    public virtual void markDirty()
    {
        dirty = true;
    }

    public virtual void collectOtherEntities(Entity except, Box box, List<Entity> result)
    {
        int var4 = MathHelper.Floor((box.minY - 2.0D) / 16.0D);
        int var5 = MathHelper.Floor((box.maxY + 2.0D) / 16.0D);
        if (var4 < 0)
        {
            var4 = 0;
        }

        if (var5 >= entities.Length)
        {
            var5 = entities.Length - 1;
        }

        for (int var6 = var4; var6 <= var5; ++var6)
        {
            List<Entity> var7 = entities[var6];

            for (int var8 = 0; var8 < var7.Count; ++var8)
            {
                Entity var9 = var7[var8];
                if (var9 != except && var9.boundingBox.intersects(box))
                {
                    result.Add(var9);
                }
            }
        }

    }

    public virtual void collectEntitiesByClass(java.lang.Class entityClass, Box box, List<Entity> result)
    {
        int var4 = MathHelper.Floor((box.minY - 2.0D) / 16.0D);
        int var5 = MathHelper.Floor((box.maxY + 2.0D) / 16.0D);
        if (var4 < 0)
        {
            var4 = 0;
        }

        if (var5 >= entities.Length)
        {
            var5 = entities.Length - 1;
        }

        for (int var6 = var4; var6 <= var5; ++var6)
        {
            List<Entity> var7 = entities[var6];

            for (int var8 = 0; var8 < var7.Count; ++var8)
            {
                Entity var9 = var7[var8];
                if (entityClass.isAssignableFrom(var9.getClass()) && var9.boundingBox.intersects(box))
                {
                    result.Add(var9);
                }
            }
        }

    }

    public virtual bool shouldSave(bool saveEntities)
    {
        if (empty)
        {
            return false;
        }
        else
        {
            if (saveEntities)
            {
                if (lastSaveHadEntities && world.getTime() != lastSaveTime)
                {
                    return true;
                }
            }
            else if (lastSaveHadEntities && world.getTime() >= lastSaveTime + 600L)
            {
                return true;
            }

            return dirty;
        }
    }

    public virtual int loadFromPacket(byte[] bytes, int minX, int minY, int minZ, int maxX, int maxY, int maxZ, int offset)
    {
        int sizeX = maxX - minX;
        int sizeY = maxY - minY;
        int sizeZ = maxZ - minZ;

        if (sizeX == 16 && sizeY == 128 && sizeZ == 16)
        {
            Profiler.Start("loadFromPacketFull");
        }
        else
        {
            Profiler.Start("loadFromPacketSmall");
        }

        int var9;
        int var10;
        int var11;
        int var12;
        for (var9 = minX; var9 < maxX; ++var9)
        {
            for (var10 = minZ; var10 < maxZ; ++var10)
            {
                var11 = var9 << 11 | var10 << 7 | minY;
                var12 = maxY - minY;
                Buffer.BlockCopy(bytes, offset, blocks, var11, var12);
                offset += var12;
            }
        }

        populateHeightMapOnly();

        for (var9 = minX; var9 < maxX; ++var9)
        {
            for (var10 = minZ; var10 < maxZ; ++var10)
            {
                var11 = (var9 << 11 | var10 << 7 | minY) >> 1;
                var12 = (maxY - minY) / 2;
                Buffer.BlockCopy(bytes, offset, meta.bytes, var11, var12);
                offset += var12;
            }
        }

        for (var9 = minX; var9 < maxX; ++var9)
        {
            for (var10 = minZ; var10 < maxZ; ++var10)
            {
                var11 = (var9 << 11 | var10 << 7 | minY) >> 1;
                var12 = (maxY - minY) / 2;
                Buffer.BlockCopy(bytes, offset, blockLight.bytes, var11, var12);
                offset += var12;
            }
        }

        for (var9 = minX; var9 < maxX; ++var9)
        {
            for (var10 = minZ; var10 < maxZ; ++var10)
            {
                var11 = (var9 << 11 | var10 << 7 | minY) >> 1;
                var12 = (maxY - minY) / 2;
                Buffer.BlockCopy(bytes, offset, skyLight.bytes, var11, var12);
                offset += var12;
            }
        }

        for (var9 = minX; var9 < maxX; ++var9)
        {
            for (var10 = minZ; var10 < maxZ; ++var10)
            {
                for (int y = minY; y < maxY; y++)
                {
                    int id = getBlockId(var9, y, var10);
                    if (id > 0 && Block.BlocksWithEntity[id])
                    {
                        getBlockEntity(var9, y, var10);
                    }
                }
            }
        }

        if (sizeX == 16 && sizeY == 128 && sizeZ == 16)
        {
            Profiler.Stop("loadFromPacketFull");
        }
        else
        {
            Profiler.Stop("loadFromPacketSmall");
        }

        return offset;
    }

    public int toPacket(byte[] bytes, int minX, int minY, int minZ, int maxX, int maxY, int maxZ, int offset)
    {
        int var9 = maxX - minX;
        int var10 = maxY - minY;
        int var11 = maxZ - minZ;
        if (var9 * var10 * var11 == blocks.Length)
        {
            Buffer.BlockCopy(blocks, 0, bytes, offset, blocks.Length);
            offset += blocks.Length;
            Buffer.BlockCopy(meta.bytes, 0, bytes, offset, meta.bytes.Length);
            offset += meta.bytes.Length;
            Buffer.BlockCopy(blockLight.bytes, 0, bytes, offset, blockLight.bytes.Length);
            offset += blockLight.bytes.Length;
            Buffer.BlockCopy(skyLight.bytes, 0, bytes, offset, skyLight.bytes.Length);
            return offset + skyLight.bytes.Length;
        }
        else
        {
            for (int var12 = minX; var12 < maxX; var12++)
            {
                for (int var13 = minZ; var13 < maxZ; var13++)
                {
                    int var14 = var12 << 11 | var13 << 7 | minY;
                    int var15 = maxY - minY;
                    Buffer.BlockCopy(blocks, var14, bytes, offset, var15);
                    offset += var15;
                }
            }

            for (int var19 = minX; var19 < maxX; var19++)
            {
                for (int var22 = minZ; var22 < maxZ; var22++)
                {
                    int var25 = (var19 << 11 | var22 << 7 | minY) >> 1;
                    int var28 = (maxY - minY) / 2;
                    Buffer.BlockCopy(meta.bytes, var25, bytes, offset, var28);
                    offset += var28;
                }
            }

            for (int var20 = minX; var20 < maxX; var20++)
            {
                for (int var23 = minZ; var23 < maxZ; var23++)
                {
                    int var26 = (var20 << 11 | var23 << 7 | minY) >> 1;
                    int var29 = (maxY - minY) / 2;
                    Buffer.BlockCopy(blockLight.bytes, var26, bytes, offset, var29);
                    offset += var29;
                }
            }

            for (int var21 = minX; var21 < maxX; var21++)
            {
                for (int var24 = minZ; var24 < maxZ; var24++)
                {
                    int var27 = (var21 << 11 | var24 << 7 | minY) >> 1;
                    int var30 = (maxY - minY) / 2;
                    Buffer.BlockCopy(skyLight.bytes, var27, bytes, offset, var30);
                    offset += var30;
                }
            }

            return offset;
        }
    }

    public virtual JavaRandom getSlimeRandom(long scrambler)
    {
        return new JavaRandom(world.getSeed() + x * x * 4987142 + x * 5947611 + z * z * 4392871L + z * 389711 ^ scrambler);
    }

    public virtual bool isEmpty()
    {
        return false;
    }

    public void fill()
    {
        BlockSource.fill(blocks);
    }
}
