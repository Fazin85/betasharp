using betareborn.Blocks.Entities;
using betareborn.Entities;
using betareborn.NBT;
using java.io;

namespace betareborn.Worlds.Chunks.Storage
{
    public class RegionChunkStorage : ChunkStorage
    {
        private readonly java.io.File dir;

        public RegionChunkStorage(java.io.File dir)
        {
            this.dir = dir;
        }

        public Chunk loadChunk(World world, int chunkX, int chunkZ)
        {
            var s = RegionIo.getChunkInputStream(dir, chunkX, chunkZ);
            if (s == null)
            {
                return null;
            }

            var var4 = s.getInputStream();

            if (var4 != null)
            {
                NBTTagCompound var5 = NbtIo.read((DataInput)var4);
                if (!var5.hasKey("Level"))
                {
                    java.lang.System.@out.println("Chunk file at " + chunkX + "," + chunkZ + " is missing level data, skipping");
                    return null;
                }
                else if (!var5.getCompoundTag("Level").hasKey("Blocks"))
                {
                    java.lang.System.@out.println("Chunk file at " + chunkX + "," + chunkZ + " is missing block data, skipping");
                    return null;
                }
                else
                {
                    Chunk var6 = loadChunkFromNbt(world, var5.getCompoundTag("Level"));
                    if (!var6.chunkPosEquals(chunkX, chunkZ))
                    {
                        java.lang.System.@out.println("Chunk file at " + chunkX + "," + chunkZ + " is in the wrong location; relocating. (Expected " + chunkX + ", " + chunkZ + ", got " + var6.x + ", " + var6.z + ")");
                        var5.setInteger("xPos", chunkX);
                        var5.setInteger("zPos", chunkZ);
                        var6 = loadChunkFromNbt(world, var5.getCompoundTag("Level"));
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
                DataOutputStream var3 = RegionIo.getChunkOutputStream(dir, chunk.x, chunk.z);
                NBTTagCompound var4 = new();
                NBTTagCompound var5 = new();
                var4.setTag("Level", var5);
                storeChunkInCompound(chunk, world, var5);
                NbtIo.write(var4, var3);
                var3.close();
                WorldProperties var6 = world.getProperties();
                var6.setSizeOnDisk(var6.getSizeOnDisk() + (long)RegionIo.getSizeDelta(dir, chunk.x, chunk.z));
            }
            catch (Exception var7)
            {
                System.Console.WriteLine(var7);
            }
        }

        public static void storeChunkInCompound(Chunk chunk, World world, NBTTagCompound nbt)
        {
            nbt.setInteger("xPos", chunk.x);
            nbt.setInteger("zPos", chunk.z);
            nbt.setLong("LastUpdate", world.getTime());
            nbt.setByteArray("Blocks", chunk.blocks);
            nbt.setByteArray("Data", chunk.meta.bytes);
            nbt.setByteArray("SkyLight", chunk.skyLight.bytes);
            nbt.setByteArray("BlockLight", chunk.blockLight.bytes);
            nbt.setByteArray("HeightMap", chunk.heightmap);
            nbt.setBoolean("TerrainPopulated", chunk.terrainPopulated);
            chunk.lastSaveHadEntities = false;
            NBTTagList var3 = new();

            NBTTagCompound var7;
            for (int var4 = 0; var4 < chunk.entities.Length; ++var4)
            {
                foreach (var var6 in chunk.entities[var4])
                {
                    chunk.lastSaveHadEntities = true;
                    var7 = new NBTTagCompound();
                    if (var6.saveSelfNbt(var7))
                    {
                        var3.setTag(var7);
                    }
                }
            }

            nbt.setTag("Entities", var3);
            NBTTagList var8 = new();

            foreach (var var9 in chunk.blockEntities.Values)
            {
                var7 = new NBTTagCompound();
                var9.writeNbt(var7);
                var8.setTag(var7);
            }

            nbt.setTag("TileEntities", var8);
        }

        public static Chunk loadChunkFromNbt(World world, NBTTagCompound nbt)
        {
            int var2 = nbt.getInteger("xPos");
            int var3 = nbt.getInteger("zPos");
            Chunk var4 = new(world, var2, var3);
            var4.blocks = nbt.getByteArray("Blocks");
            var4.meta = new ChunkNibbleArray(nbt.getByteArray("Data"));
            var4.skyLight = new ChunkNibbleArray(nbt.getByteArray("SkyLight"));
            var4.blockLight = new ChunkNibbleArray(nbt.getByteArray("BlockLight"));
            var4.heightmap = nbt.getByteArray("HeightMap");
            var4.terrainPopulated = nbt.getBoolean("TerrainPopulated");
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

            NBTTagList var5 = nbt.getTagList("Entities");
            if (var5 != null)
            {
                for (int var6 = 0; var6 < var5.tagCount(); ++var6)
                {
                    NBTTagCompound var7 = (NBTTagCompound)var5.tagAt(var6);
                    Entity var8 = EntityRegistry.getEntityFromNbt(var7, world);
                    var4.lastSaveHadEntities = true;
                    if (var8 != null)
                    {
                        var4.addEntity(var8);
                    }
                }
            }

            NBTTagList var10 = nbt.getTagList("TileEntities");
            if (var10 != null)
            {
                for (int var11 = 0; var11 < var10.tagCount(); ++var11)
                {
                    NBTTagCompound var12 = (NBTTagCompound)var10.tagAt(var11);
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
}
