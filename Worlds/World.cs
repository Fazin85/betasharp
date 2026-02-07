using betareborn.Biomes;
using betareborn.Blocks;
using betareborn.Chunks;
using betareborn.Entities;
using betareborn.Materials;
using betareborn.NBT;
using betareborn.Profiling;
using betareborn.TileEntities;
using java.lang;
using java.util;
using Silk.NET.Maths;
using System.Runtime.InteropServices;

namespace betareborn.Worlds
{
    public class World : java.lang.Object, BlockView
    {
        public static readonly Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(World).TypeHandle);
        private const int AUTOSAVE_PERIOD = 40;
        public bool scheduledUpdatesAreImmediate;
        private readonly List<MetadataChunkBlock> lightingToUpdate;
        public List<Entity> loadedEntityList;
        private readonly List<Entity> unloadedEntityList;
        private readonly TreeSet scheduledTickTreeSet;
        private readonly Set scheduledTickSet;
        public List<TileEntity> loadedTileEntityList;
        private readonly List<TileEntity> field_30900_E;
        public List playerEntities;
        public List weatherEffects;
        private readonly long field_1019_F;
        public int skylightSubtracted;
        protected int field_9437_g;
        protected readonly int field_9436_h;
        protected float prevRainingStrength;
        protected float rainingStrength;
        protected float prevThunderingStrength;
        protected float thunderingStrength;
        protected int field_27168_F;
        public int field_27172_i;
        public bool editingBlocks;
        private readonly long lockTimestamp;
        protected int autosavePeriod;
        public int difficultySetting;
        public java.util.Random random;
        public bool isNewWorld;
        public readonly WorldProvider dimension;
        protected List<IWorldAccess> worldAccesses;
        protected IChunkProvider chunkProvider;
        protected readonly ISaveHandler saveHandler;
        protected WorldInfo worldInfo;
        public bool findingSpawnPoint;
        private bool allPlayersSleeping;
        public MapStorage field_28108_z;
        private readonly List<Box> collidingBoundingBoxes;
        private bool field_31055_L;
        private int lightingUpdatesCounter;
        private bool spawnHostileMobs;
        private bool spawnPeacefulMobs;
        static int lightingUpdatesScheduled = 0;
        private readonly Set positionsToUpdate;
        private int soundCounter;
        private readonly List<Entity> field_1012_M;
        public bool multiplayerWorld;

        public BiomeSource getBiomeSource()
        {
            return dimension.worldChunkMgr;
        }

        public World(ISaveHandler var1, string var2, WorldProvider var3, long var4)
        {
            scheduledUpdatesAreImmediate = false;
            lightingToUpdate = [];
            loadedEntityList = [];
            unloadedEntityList = [];
            scheduledTickTreeSet = new TreeSet();
            scheduledTickSet = new HashSet();
            loadedTileEntityList = [];
            field_30900_E = [];
            playerEntities = new ArrayList();
            weatherEffects = new ArrayList();
            field_1019_F = 16777215L;
            skylightSubtracted = 0;
            field_9437_g = (new java.util.Random()).nextInt();
            field_9436_h = 1013904223;
            field_27168_F = 0;
            field_27172_i = 0;
            editingBlocks = false;
            lockTimestamp = java.lang.System.currentTimeMillis();
            autosavePeriod = AUTOSAVE_PERIOD;
            random = new();
            isNewWorld = false;
            worldAccesses = [];
            collidingBoundingBoxes = [];
            lightingUpdatesCounter = 0;
            spawnHostileMobs = true;
            spawnPeacefulMobs = true;
            positionsToUpdate = new HashSet();
            soundCounter = random.nextInt(12000);
            field_1012_M = [];
            multiplayerWorld = false;
            saveHandler = var1;
            worldInfo = new WorldInfo(var4, var2);
            dimension = var3;
            field_28108_z = new MapStorage(var1);
            var3.registerWorld(this);
            chunkProvider = getChunkProvider();
            calculateInitialSkylight();
            func_27163_E();
        }

        public World(World var1, WorldProvider var2)
        {
            scheduledUpdatesAreImmediate = false;
            lightingToUpdate = [];
            loadedEntityList = [];
            unloadedEntityList = [];
            scheduledTickTreeSet = new TreeSet();
            scheduledTickSet = new HashSet();
            loadedTileEntityList = [];
            field_30900_E = [];
            playerEntities = new ArrayList();
            weatherEffects = new ArrayList();
            field_1019_F = 16777215L;
            skylightSubtracted = 0;
            field_9437_g = (new java.util.Random()).nextInt();
            field_9436_h = 1013904223;
            field_27168_F = 0;
            field_27172_i = 0;
            editingBlocks = false;
            lockTimestamp = java.lang.System.currentTimeMillis();
            autosavePeriod = AUTOSAVE_PERIOD;
            random = new();
            isNewWorld = false;
            worldAccesses = [];
            collidingBoundingBoxes = [];
            lightingUpdatesCounter = 0;
            spawnHostileMobs = true;
            spawnPeacefulMobs = true;
            positionsToUpdate = new HashSet();
            soundCounter = random.nextInt(12000);
            field_1012_M = [];
            multiplayerWorld = false;
            lockTimestamp = var1.lockTimestamp;
            saveHandler = var1.saveHandler;
            worldInfo = new WorldInfo(var1.worldInfo);
            field_28108_z = new MapStorage(saveHandler);
            dimension = var2;
            var2.registerWorld(this);
            chunkProvider = getChunkProvider();
            calculateInitialSkylight();
            func_27163_E();
        }

        public World(ISaveHandler var1, string var2, long var3) : this(var1, var2, var3, null)
        {
        }

        public World(ISaveHandler var1, string var2, long var3, WorldProvider var5)
        {
            scheduledUpdatesAreImmediate = false;
            lightingToUpdate = [];
            loadedEntityList = [];
            unloadedEntityList = [];
            scheduledTickTreeSet = new TreeSet();
            scheduledTickSet = new HashSet();
            loadedTileEntityList = [];
            field_30900_E = [];
            playerEntities = new ArrayList();
            weatherEffects = new ArrayList();
            field_1019_F = 16777215L;
            skylightSubtracted = 0;
            field_9437_g = (new java.util.Random()).nextInt();
            field_9436_h = 1013904223;
            field_27168_F = 0;
            field_27172_i = 0;
            editingBlocks = false;
            lockTimestamp = java.lang.System.currentTimeMillis();
            autosavePeriod = AUTOSAVE_PERIOD;
            random = new java.util.Random();
            isNewWorld = false;
            worldAccesses = [];
            collidingBoundingBoxes = [];
            lightingUpdatesCounter = 0;
            spawnHostileMobs = true;
            spawnPeacefulMobs = true;
            positionsToUpdate = new HashSet();
            soundCounter = random.nextInt(12000);
            field_1012_M = [];
            multiplayerWorld = false;
            saveHandler = var1;
            field_28108_z = new MapStorage(var1);
            worldInfo = var1.loadWorldInfo();
            isNewWorld = worldInfo == null;
            if (var5 != null)
            {
                dimension = var5;
            }
            else if (worldInfo != null && worldInfo.getDimension() == -1)
            {
                dimension = WorldProvider.getProviderForDimension(-1);
            }
            else
            {
                dimension = WorldProvider.getProviderForDimension(0);
            }

            bool var6 = false;
            if (worldInfo == null)
            {
                worldInfo = new WorldInfo(var3, var2);
                var6 = true;
            }
            else
            {
                worldInfo.setWorldName(var2);
            }

            dimension.registerWorld(this);
            chunkProvider = getChunkProvider();
            if (var6)
            {
                getInitialSpawnLocation();
            }

            calculateInitialSkylight();
            func_27163_E();
        }

        protected virtual IChunkProvider getChunkProvider()
        {
            IChunkLoader var1 = saveHandler.getChunkLoader(dimension);
            return new ChunkProvider(this, (McRegionChunkLoader)var1, dimension.getChunkProvider());
        }

        protected void getInitialSpawnLocation()
        {
            findingSpawnPoint = true;
            int var1 = 0;
            byte var2 = 64;

            int var3;
            for (var3 = 0; !dimension.canCoordinateBeSpawn(var1, var3); var3 += random.nextInt(64) - random.nextInt(64))
            {
                var1 += random.nextInt(64) - random.nextInt(64);
            }

            worldInfo.setSpawn(var1, var2, var3);
            findingSpawnPoint = false;
        }

        public virtual void setSpawnLocation()
        {
            if (worldInfo.getSpawnY() <= 0)
            {
                worldInfo.setSpawnY(64);
            }

            int var1 = worldInfo.getSpawnX();

            int var2;
            for (var2 = worldInfo.getSpawnZ(); getFirstUncoveredBlock(var1, var2) == 0; var2 += random.nextInt(8) - random.nextInt(8))
            {
                var1 += random.nextInt(8) - random.nextInt(8);
            }

            worldInfo.setSpawnX(var1);
            worldInfo.setSpawnZ(var2);
        }

        public int getFirstUncoveredBlock(int var1, int var2)
        {
            int var3;
            for (var3 = 63; !isAir(var1, var3 + 1, var2); ++var3)
            {
            }

            return getBlockId(var1, var3, var2);
        }

        public void emptyMethod1()
        {
        }

        public void spawnPlayerWithLoadedChunks(EntityPlayer var1)
        {
            try
            {
                NBTTagCompound var2 = worldInfo.getPlayerNBTTagCompound();
                if (var2 != null)
                {
                    var1.readFromNBT(var2);
                    worldInfo.setPlayerNBTTagCompound((NBTTagCompound)null);
                }

                if (chunkProvider is ChunkProviderLoadOrGenerate)
                {
                    ChunkProviderLoadOrGenerate var3 = (ChunkProviderLoadOrGenerate)chunkProvider;
                    int var4 = MathHelper.floor_float((float)((int)var1.posX)) >> 4;
                    int var5 = MathHelper.floor_float((float)((int)var1.posZ)) >> 4;
                    var3.setCurrentChunkOver(var4, var5);
                }

                spawnEntity(var1);
            }
            catch (java.lang.Exception var6)
            {
                var6.printStackTrace();
            }

        }

        public void saveWorld(bool var1, IProgressUpdate var2)
        {
            if (chunkProvider.canSave())
            {
                if (var2 != null)
                {
                    var2.func_594_b("Saving level");
                }

                Profiler.PushGroup("saveLevel");
                saveLevel();
                Profiler.PopGroup();
                if (var2 != null)
                {
                    var2.displayLoadingString("Saving chunks");
                }

                Profiler.Start("saveChunks");
                chunkProvider.saveChunks(var1, var2);
                Profiler.Stop("saveChunks");
            }
        }

        private void saveLevel()
        {
            Profiler.Start("checkSessionLock");
            //checkSessionLock();
            Profiler.Stop("checkSessionLock");
            Profiler.Start("saveWorldInfoAndPlayer");
            saveHandler.saveWorldInfoAndPlayer(worldInfo, playerEntities);
            Profiler.Stop("saveWorldInfoAndPlayer");

            Profiler.Start("saveAllData");
            field_28108_z.saveAllData();
            Profiler.Stop("saveAllData");
        }

        public bool func_650_a(int var1)
        {
            if (!chunkProvider.canSave())
            {
                return true;
            }
            else
            {
                if (var1 == 0)
                {
                    saveLevel();
                }

                return chunkProvider.saveChunks(false, (IProgressUpdate)null);
            }
        }

        public int getBlockId(int var1, int var2, int var3)
        {
            return var1 >= -32000000 && var3 >= -32000000 && var1 < 32000000 && var3 <= 32000000 ? (var2 < 0 ? 0 : (var2 >= 128 ? 0 : getChunkFromChunkCoords(var1 >> 4, var3 >> 4).getBlockID(var1 & 15, var2, var3 & 15))) : 0;
        }

        public bool isAir(int var1, int var2, int var3)
        {
            return getBlockId(var1, var2, var3) == 0;
        }

        public bool blockExists(int var1, int var2, int var3)
        {
            return var2 >= 0 && var2 < 128 ? chunkExists(var1 >> 4, var3 >> 4) : false;
        }

        public bool doChunksNearChunkExist(int var1, int var2, int var3, int var4)
        {
            return checkChunksExist(var1 - var4, var2 - var4, var3 - var4, var1 + var4, var2 + var4, var3 + var4);
        }

        public bool checkChunksExist(int var1, int var2, int var3, int var4, int var5, int var6)
        {
            if (var5 >= 0 && var2 < 128)
            {
                var1 >>= 4;
                var2 >>= 4;
                var3 >>= 4;
                var4 >>= 4;
                var5 >>= 4;
                var6 >>= 4;

                for (int var7 = var1; var7 <= var4; ++var7)
                {
                    for (int var8 = var3; var8 <= var6; ++var8)
                    {
                        if (!chunkExists(var7, var8))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool chunkExists(int var1, int var2)
        {
            return chunkProvider.chunkExists(var1, var2);
        }

        public Chunk getChunkFromBlockCoords(int var1, int var2)
        {
            return getChunkFromChunkCoords(var1 >> 4, var2 >> 4);
        }

        public Chunk getChunkFromChunkCoords(int var1, int var2)
        {
            return chunkProvider.provideChunk(var1, var2);
        }

        public virtual bool setBlockAndMetadata(int var1, int var2, int var3, int var4, int var5)
        {
            if (var1 >= -32000000 && var3 >= -32000000 && var1 < 32000000 && var3 <= 32000000)
            {
                if (var2 < 0)
                {
                    return false;
                }
                else if (var2 >= 128)
                {
                    return false;
                }
                else
                {
                    Chunk var6 = getChunkFromChunkCoords(var1 >> 4, var3 >> 4);
                    return var6.setBlockIDWithMetadata(var1 & 15, var2, var3 & 15, var4, var5);
                }
            }
            else
            {
                return false;
            }
        }

        public virtual bool setBlock(int var1, int var2, int var3, int var4)
        {
            if (var1 >= -32000000 && var3 >= -32000000 && var1 < 32000000 && var3 <= 32000000)
            {
                if (var2 < 0)
                {
                    return false;
                }
                else if (var2 >= 128)
                {
                    return false;
                }
                else
                {
                    Chunk var5 = getChunkFromChunkCoords(var1 >> 4, var3 >> 4);
                    return var5.setBlockID(var1 & 15, var2, var3 & 15, var4);
                }
            }
            else
            {
                return false;
            }
        }

        public Material getMaterial(int var1, int var2, int var3)
        {
            int var4 = getBlockId(var1, var2, var3);
            return var4 == 0 ? Material.AIR : Block.BLOCKS[var4].material;
        }

        public int getBlockMeta(int var1, int var2, int var3)
        {
            if (var1 >= -32000000 && var3 >= -32000000 && var1 < 32000000 && var3 <= 32000000)
            {
                if (var2 < 0)
                {
                    return 0;
                }
                else if (var2 >= 128)
                {
                    return 0;
                }
                else
                {
                    Chunk var4 = getChunkFromChunkCoords(var1 >> 4, var3 >> 4);
                    var1 &= 15;
                    var3 &= 15;
                    return var4.getBlockMetadata(var1, var2, var3);
                }
            }
            else
            {
                return 0;
            }
        }

        public void setBlockMeta(int var1, int var2, int var3, int var4)
        {
            if (setBlockMetadata(var1, var2, var3, var4))
            {
                int var5 = getBlockId(var1, var2, var3);
                if (Block.BLOCKS_IGNORE_META_UPDATE[var5 & 255])
                {
                    notifyBlockChange(var1, var2, var3, var5);
                }
                else
                {
                    notifyBlocksOfNeighborChange(var1, var2, var3, var5);
                }
            }

        }

        public virtual bool setBlockMetadata(int var1, int var2, int var3, int var4)
        {
            if (var1 >= -32000000 && var3 >= -32000000 && var1 < 32000000 && var3 <= 32000000)
            {
                if (var2 < 0)
                {
                    return false;
                }
                else if (var2 >= 128)
                {
                    return false;
                }
                else
                {
                    Chunk var5 = getChunkFromChunkCoords(var1 >> 4, var3 >> 4);
                    var1 &= 15;
                    var3 &= 15;
                    var5.setBlockMetadata(var1, var2, var3, var4);
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public bool setBlockWithNotify(int var1, int var2, int var3, int var4)
        {
            if (setBlock(var1, var2, var3, var4))
            {
                notifyBlockChange(var1, var2, var3, var4);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool setBlockAndMetadataWithNotify(int var1, int var2, int var3, int var4, int var5)
        {
            if (setBlockAndMetadata(var1, var2, var3, var4, var5))
            {
                notifyBlockChange(var1, var2, var3, var4);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void markBlockNeedsUpdate(int var1, int var2, int var3)
        {
            for (int var4 = 0; var4 < worldAccesses.Count; ++var4)
            {
                worldAccesses[var4].markBlockAndNeighborsNeedsUpdate(var1, var2, var3);
            }

        }

        protected void notifyBlockChange(int var1, int var2, int var3, int var4)
        {
            markBlockNeedsUpdate(var1, var2, var3);
            notifyBlocksOfNeighborChange(var1, var2, var3, var4);
        }

        public void markBlocksDirtyVertical(int var1, int var2, int var3, int var4)
        {
            if (var3 > var4)
            {
                int var5 = var4;
                var4 = var3;
                var3 = var5;
            }

            markBlocksDirty(var1, var3, var2, var1, var4, var2);
        }

        public void markBlockAsNeedsUpdate(int var1, int var2, int var3)
        {
            for (int var4 = 0; var4 < worldAccesses.Count; ++var4)
            {
                worldAccesses[var4].markBlockRangeNeedsUpdate(var1, var2, var3, var1, var2, var3);
            }

        }

        public void markBlocksDirty(int var1, int var2, int var3, int var4, int var5, int var6)
        {
            for (int var7 = 0; var7 < worldAccesses.Count; ++var7)
            {
                worldAccesses[var7].markBlockRangeNeedsUpdate(var1, var2, var3, var4, var5, var6);
            }

        }

        public void notifyBlocksOfNeighborChange(int var1, int var2, int var3, int var4)
        {
            notifyBlockOfNeighborChange(var1 - 1, var2, var3, var4);
            notifyBlockOfNeighborChange(var1 + 1, var2, var3, var4);
            notifyBlockOfNeighborChange(var1, var2 - 1, var3, var4);
            notifyBlockOfNeighborChange(var1, var2 + 1, var3, var4);
            notifyBlockOfNeighborChange(var1, var2, var3 - 1, var4);
            notifyBlockOfNeighborChange(var1, var2, var3 + 1, var4);
        }

        private void notifyBlockOfNeighborChange(int var1, int var2, int var3, int var4)
        {
            if (!editingBlocks && !multiplayerWorld)
            {
                Block var5 = Block.BLOCKS[getBlockId(var1, var2, var3)];
                if (var5 != null)
                {
                    var5.neighborUpdate(this, var1, var2, var3, var4);
                }

            }
        }

        public bool canBlockSeeTheSky(int var1, int var2, int var3)
        {
            return getChunkFromChunkCoords(var1 >> 4, var3 >> 4).canBlockSeeTheSky(var1 & 15, var2, var3 & 15);
        }

        public int getFullBlockLightValue(int var1, int var2, int var3)
        {
            if (var2 < 0)
            {
                return 0;
            }
            else
            {
                if (var2 >= 128)
                {
                    var2 = 127;
                }

                return getChunkFromChunkCoords(var1 >> 4, var3 >> 4).getBlockLightValue(var1 & 15, var2, var3 & 15, 0);
            }
        }

        public int getBlockLightValue(int var1, int var2, int var3)
        {
            return getBlockLightValue_do(var1, var2, var3, true);
        }

        public int getBlockLightValue_do(int var1, int var2, int var3, bool var4)
        {
            if (var1 >= -32000000 && var3 >= -32000000 && var1 < 32000000 && var3 <= 32000000)
            {
                if (var4)
                {
                    int var5 = getBlockId(var1, var2, var3);
                    if (var5 == Block.SLAB.id || var5 == Block.FARMLAND.id || var5 == Block.COBBLESTONE_STAIRS.id || var5 == Block.WOODEN_STAIRS.id)
                    {
                        int var6 = getBlockLightValue_do(var1, var2 + 1, var3, false);
                        int var7 = getBlockLightValue_do(var1 + 1, var2, var3, false);
                        int var8 = getBlockLightValue_do(var1 - 1, var2, var3, false);
                        int var9 = getBlockLightValue_do(var1, var2, var3 + 1, false);
                        int var10 = getBlockLightValue_do(var1, var2, var3 - 1, false);
                        if (var7 > var6)
                        {
                            var6 = var7;
                        }

                        if (var8 > var6)
                        {
                            var6 = var8;
                        }

                        if (var9 > var6)
                        {
                            var6 = var9;
                        }

                        if (var10 > var6)
                        {
                            var6 = var10;
                        }

                        return var6;
                    }
                }

                if (var2 < 0)
                {
                    return 0;
                }
                else
                {
                    if (var2 >= 128)
                    {
                        var2 = 127;
                    }

                    Chunk var11 = getChunkFromChunkCoords(var1 >> 4, var3 >> 4);
                    var1 &= 15;
                    var3 &= 15;
                    return var11.getBlockLightValue(var1, var2, var3, skylightSubtracted);
                }
            }
            else
            {
                return 15;
            }
        }

        public bool canExistingBlockSeeTheSky(int var1, int var2, int var3)
        {
            if (var1 >= -32000000 && var3 >= -32000000 && var1 < 32000000 && var3 <= 32000000)
            {
                if (var2 < 0)
                {
                    return false;
                }
                else if (var2 >= 128)
                {
                    return true;
                }
                else if (!chunkExists(var1 >> 4, var3 >> 4))
                {
                    return false;
                }
                else
                {
                    Chunk var4 = getChunkFromChunkCoords(var1 >> 4, var3 >> 4);
                    var1 &= 15;
                    var3 &= 15;
                    return var4.canBlockSeeTheSky(var1, var2, var3);
                }
            }
            else
            {
                return false;
            }
        }

        public int getHeightValue(int var1, int var2)
        {
            if (var1 >= -32000000 && var2 >= -32000000 && var1 < 32000000 && var2 <= 32000000)
            {
                if (!chunkExists(var1 >> 4, var2 >> 4))
                {
                    return 0;
                }
                else
                {
                    Chunk var3 = getChunkFromChunkCoords(var1 >> 4, var2 >> 4);
                    return var3.getHeightValue(var1 & 15, var2 & 15);
                }
            }
            else
            {
                return 0;
            }
        }

        public void neighborLightPropagationChanged(EnumSkyBlock var1, int var2, int var3, int var4, int var5)
        {
            if (!dimension.hasNoSky || var1 != EnumSkyBlock.Sky)
            {
                if (blockExists(var2, var3, var4))
                {
                    if (var1 == EnumSkyBlock.Sky)
                    {
                        if (canExistingBlockSeeTheSky(var2, var3, var4))
                        {
                            var5 = 15;
                        }
                    }
                    else if (var1 == EnumSkyBlock.Block)
                    {
                        int var6 = getBlockId(var2, var3, var4);
                        if (Block.BLOCKS_LIGHT_LUMINANCE[var6] > var5)
                        {
                            var5 = Block.BLOCKS_LIGHT_LUMINANCE[var6];
                        }
                    }

                    if (getSavedLightValue(var1, var2, var3, var4) != var5)
                    {
                        scheduleLightingUpdate(var1, var2, var3, var4, var2, var3, var4);
                    }

                }
            }
        }

        public int getSavedLightValue(EnumSkyBlock var1, int var2, int var3, int var4)
        {
            if (var3 < 0)
            {
                var3 = 0;
            }

            if (var3 >= 128)
            {
                var3 = 127;
            }

            if (var3 >= 0 && var3 < 128 && var2 >= -32000000 && var4 >= -32000000 && var2 < 32000000 && var4 <= 32000000)
            {
                int var5 = var2 >> 4;
                int var6 = var4 >> 4;
                if (!chunkExists(var5, var6))
                {
                    return 0;
                }
                else
                {
                    Chunk var7 = getChunkFromChunkCoords(var5, var6);
                    return var7.getSavedLightValue(var1, var2 & 15, var3, var4 & 15);
                }
            }
            else
            {
                return var1.field_1722_c;
            }
        }

        public void setLightValue(EnumSkyBlock var1, int var2, int var3, int var4, int var5)
        {
            if (var2 >= -32000000 && var4 >= -32000000 && var2 < 32000000 && var4 <= 32000000)
            {
                if (var3 >= 0)
                {
                    if (var3 < 128)
                    {
                        if (chunkExists(var2 >> 4, var4 >> 4))
                        {
                            Chunk var6 = getChunkFromChunkCoords(var2 >> 4, var4 >> 4);
                            var6.setLightValue(var1, var2 & 15, var3, var4 & 15, var5);

                            for (int var7 = 0; var7 < worldAccesses.Count; ++var7)
                            {
                                worldAccesses[var7].markBlockAndNeighborsNeedsUpdate(var2, var3, var4);
                            }

                        }
                    }
                }
            }
        }

        public float getNaturalBrightness(int var1, int var2, int var3, int var4)
        {
            int var5 = getBlockLightValue(var1, var2, var3);
            if (var5 < var4)
            {
                var5 = var4;
            }

            return dimension.lightBrightnessTable[var5];
        }

        public float getLuminance(int var1, int var2, int var3)
        {
            return dimension.lightBrightnessTable[getBlockLightValue(var1, var2, var3)];
        }

        public bool isDaytime()
        {
            return skylightSubtracted < 4;
        }

        public MovingObjectPosition rayTraceBlocks(Vec3D var1, Vec3D var2)
        {
            return func_28105_a(var1, var2, false, false);
        }

        public MovingObjectPosition rayTraceBlocks_do(Vec3D var1, Vec3D var2, bool var3)
        {
            return func_28105_a(var1, var2, var3, false);
        }

        public MovingObjectPosition func_28105_a(Vec3D var1, Vec3D var2, bool var3, bool var4)
        {
            if (!java.lang.Double.isNaN(var1.xCoord) && !java.lang.Double.isNaN(var1.yCoord) && !java.lang.Double.isNaN(var1.zCoord))
            {
                if (!java.lang.Double.isNaN(var2.xCoord) && !java.lang.Double.isNaN(var2.yCoord) && !java.lang.Double.isNaN(var2.zCoord))
                {
                    int var5 = MathHelper.floor_double(var2.xCoord);
                    int var6 = MathHelper.floor_double(var2.yCoord);
                    int var7 = MathHelper.floor_double(var2.zCoord);
                    int var8 = MathHelper.floor_double(var1.xCoord);
                    int var9 = MathHelper.floor_double(var1.yCoord);
                    int var10 = MathHelper.floor_double(var1.zCoord);
                    int var11 = getBlockId(var8, var9, var10);
                    int var12 = getBlockMeta(var8, var9, var10);
                    Block var13 = Block.BLOCKS[var11];
                    if ((!var4 || var13 == null || var13.getCollisionShape(this, var8, var9, var10) != null) && var11 > 0 && var13.hasCollision(var12, var3))
                    {
                        MovingObjectPosition var14 = var13.collisionRayTrace(this, var8, var9, var10, var1, var2);
                        if (var14 != null)
                        {
                            return var14;
                        }
                    }

                    var11 = 200;

                    while (var11-- >= 0)
                    {
                        if (java.lang.Double.isNaN(var1.xCoord) || java.lang.Double.isNaN(var1.yCoord) || java.lang.Double.isNaN(var1.zCoord))
                        {
                            return null;
                        }

                        if (var8 == var5 && var9 == var6 && var10 == var7)
                        {
                            return null;
                        }

                        bool var39 = true;
                        bool var40 = true;
                        bool var41 = true;
                        double var15 = 999.0D;
                        double var17 = 999.0D;
                        double var19 = 999.0D;
                        if (var5 > var8)
                        {
                            var15 = (double)var8 + 1.0D;
                        }
                        else if (var5 < var8)
                        {
                            var15 = (double)var8 + 0.0D;
                        }
                        else
                        {
                            var39 = false;
                        }

                        if (var6 > var9)
                        {
                            var17 = (double)var9 + 1.0D;
                        }
                        else if (var6 < var9)
                        {
                            var17 = (double)var9 + 0.0D;
                        }
                        else
                        {
                            var40 = false;
                        }

                        if (var7 > var10)
                        {
                            var19 = (double)var10 + 1.0D;
                        }
                        else if (var7 < var10)
                        {
                            var19 = (double)var10 + 0.0D;
                        }
                        else
                        {
                            var41 = false;
                        }

                        double var21 = 999.0D;
                        double var23 = 999.0D;
                        double var25 = 999.0D;
                        double var27 = var2.xCoord - var1.xCoord;
                        double var29 = var2.yCoord - var1.yCoord;
                        double var31 = var2.zCoord - var1.zCoord;
                        if (var39)
                        {
                            var21 = (var15 - var1.xCoord) / var27;
                        }

                        if (var40)
                        {
                            var23 = (var17 - var1.yCoord) / var29;
                        }

                        if (var41)
                        {
                            var25 = (var19 - var1.zCoord) / var31;
                        }

                        bool var33 = false;
                        byte var42;
                        if (var21 < var23 && var21 < var25)
                        {
                            if (var5 > var8)
                            {
                                var42 = 4;
                            }
                            else
                            {
                                var42 = 5;
                            }

                            var1.xCoord = var15;
                            var1.yCoord += var29 * var21;
                            var1.zCoord += var31 * var21;
                        }
                        else if (var23 < var25)
                        {
                            if (var6 > var9)
                            {
                                var42 = 0;
                            }
                            else
                            {
                                var42 = 1;
                            }

                            var1.xCoord += var27 * var23;
                            var1.yCoord = var17;
                            var1.zCoord += var31 * var23;
                        }
                        else
                        {
                            if (var7 > var10)
                            {
                                var42 = 2;
                            }
                            else
                            {
                                var42 = 3;
                            }

                            var1.xCoord += var27 * var25;
                            var1.yCoord += var29 * var25;
                            var1.zCoord = var19;
                        }

                        Vec3D var34 = Vec3D.createVector(var1.xCoord, var1.yCoord, var1.zCoord);
                        var8 = (int)(var34.xCoord = (double)MathHelper.floor_double(var1.xCoord));
                        if (var42 == 5)
                        {
                            --var8;
                            ++var34.xCoord;
                        }

                        var9 = (int)(var34.yCoord = (double)MathHelper.floor_double(var1.yCoord));
                        if (var42 == 1)
                        {
                            --var9;
                            ++var34.yCoord;
                        }

                        var10 = (int)(var34.zCoord = (double)MathHelper.floor_double(var1.zCoord));
                        if (var42 == 3)
                        {
                            --var10;
                            ++var34.zCoord;
                        }

                        int var35 = getBlockId(var8, var9, var10);
                        int var36 = getBlockMeta(var8, var9, var10);
                        Block var37 = Block.BLOCKS[var35];
                        if ((!var4 || var37 == null || var37.getCollisionShape(this, var8, var9, var10) != null) && var35 > 0 && var37.hasCollision(var36, var3))
                        {
                            MovingObjectPosition var38 = var37.collisionRayTrace(this, var8, var9, var10, var1, var2);
                            if (var38 != null)
                            {
                                return var38;
                            }
                        }
                    }

                    return null;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public void playSoundAtEntity(Entity var1, string var2, float var3, float var4)
        {
            for (int var5 = 0; var5 < worldAccesses.Count; ++var5)
            {
                worldAccesses[var5].playSound(var2, var1.posX, var1.posY - (double)var1.yOffset, var1.posZ, var3, var4);
            }

        }

        public void playSoundEffect(double var1, double var3, double var5, string var7, float var8, float var9)
        {
            for (int var10 = 0; var10 < worldAccesses.Count; ++var10)
            {
                worldAccesses[var10].playSound(var7, var1, var3, var5, var8, var9);
            }

        }

        public void playRecord(string var1, int var2, int var3, int var4)
        {
            for (int var5 = 0; var5 < worldAccesses.Count; ++var5)
            {
                worldAccesses[var5].playRecord(var1, var2, var3, var4);
            }

        }

        public void addParticle(string var1, double var2, double var4, double var6, double var8, double var10, double var12)
        {
            for (int var14 = 0; var14 < worldAccesses.Count; ++var14)
            {
                worldAccesses[var14].spawnParticle(var1, var2, var4, var6, var8, var10, var12);
            }

        }

        public bool addWeatherEffect(Entity var1)
        {
            weatherEffects.add(var1);
            return true;
        }

        public virtual bool spawnEntity(Entity var1)
        {
            int var2 = MathHelper.floor_double(var1.posX / 16.0D);
            int var3 = MathHelper.floor_double(var1.posZ / 16.0D);
            bool var4 = false;
            if (var1 is EntityPlayer)
            {
                var4 = true;
            }

            if (!var4 && !chunkExists(var2, var3))
            {
                return false;
            }
            else
            {
                if (var1 is EntityPlayer)
                {
                    EntityPlayer var5 = (EntityPlayer)var1;
                    playerEntities.add(var5);
                    updateAllPlayersSleepingFlag();
                }

                getChunkFromChunkCoords(var2, var3).addEntity(var1);
                loadedEntityList.Add(var1);
                obtainEntitySkin(var1);
                return true;
            }
        }

        protected virtual void obtainEntitySkin(Entity var1)
        {
            for (int var2 = 0; var2 < worldAccesses.Count; ++var2)
            {
                worldAccesses[var2].obtainEntitySkin(var1);
            }

        }

        protected virtual void releaseEntitySkin(Entity var1)
        {
            for (int var2 = 0; var2 < worldAccesses.Count; ++var2)
            {
                worldAccesses[var2].releaseEntitySkin(var1);
            }

        }

        public virtual void setEntityDead(Entity var1)
        {
            if (var1.riddenByEntity != null)
            {
                var1.riddenByEntity.mountEntity((Entity)null);
            }

            if (var1.ridingEntity != null)
            {
                var1.mountEntity((Entity)null);
            }

            var1.setEntityDead();
            if (var1 is EntityPlayer)
            {
                playerEntities.remove((EntityPlayer)var1);
                updateAllPlayersSleepingFlag();
            }

        }

        public void addWorldAccess(IWorldAccess var1)
        {
            worldAccesses.Add(var1);
        }

        public void removeWorldAccess(IWorldAccess var1)
        {
            worldAccesses.Remove(var1);
        }

        public List<Box> getCollidingBoundingBoxes(Entity var1, Box var2)
        {
            collidingBoundingBoxes.Clear();
            int var3 = MathHelper.floor_double(var2.minX);
            int var4 = MathHelper.floor_double(var2.maxX + 1.0D);
            int var5 = MathHelper.floor_double(var2.minY);
            int var6 = MathHelper.floor_double(var2.maxY + 1.0D);
            int var7 = MathHelper.floor_double(var2.minZ);
            int var8 = MathHelper.floor_double(var2.maxZ + 1.0D);

            for (int var9 = var3; var9 < var4; ++var9)
            {
                for (int var10 = var7; var10 < var8; ++var10)
                {
                    if (blockExists(var9, 64, var10))
                    {
                        for (int var11 = var5 - 1; var11 < var6; ++var11)
                        {
                            Block var12 = Block.BLOCKS[getBlockId(var9, var11, var10)];
                            if (var12 != null)
                            {
                                var12.addIntersectingBoundingBox(this, var9, var11, var10, var2, collidingBoundingBoxes);
                            }
                        }
                    }
                }
            }

            double var14 = 0.25D;
            List<Entity> var15 = getEntitiesWithinAABBExcludingEntity(var1, var2.expand(var14, var14, var14));

            for (int var16 = 0; var16 < var15.Count; ++var16)
            {
                Box var13 = var15[var16].getBoundingBox();
                if (var13 != null && var13.intersects(var2))
                {
                    collidingBoundingBoxes.Add(var13);
                }

                var13 = var1.getCollisionBox(var15[var16]);
                if (var13 != null && var13.intersects(var2))
                {
                    collidingBoundingBoxes.Add(var13);
                }
            }

            return collidingBoundingBoxes;
        }

        public int calculateSkylightSubtracted(float var1)
        {
            float var2 = getCelestialAngle(var1);
            float var3 = 1.0F - (MathHelper.cos(var2 * (float)java.lang.Math.PI * 2.0F) * 2.0F + 0.5F);
            if (var3 < 0.0F)
            {
                var3 = 0.0F;
            }

            if (var3 > 1.0F)
            {
                var3 = 1.0F;
            }

            var3 = 1.0F - var3;
            var3 = (float)((double)var3 * (1.0D - (double)(func_27162_g(var1) * 5.0F) / 16.0D));
            var3 = (float)((double)var3 * (1.0D - (double)(func_27166_f(var1) * 5.0F) / 16.0D));
            var3 = 1.0F - var3;
            return (int)(var3 * 11.0F);
        }

        public Vector3D<double> func_4079_a(Entity var1, float var2)
        {
            float var3 = getCelestialAngle(var2);
            float var4 = MathHelper.cos(var3 * (float)java.lang.Math.PI * 2.0F) * 2.0F + 0.5F;
            if (var4 < 0.0F)
            {
                var4 = 0.0F;
            }

            if (var4 > 1.0F)
            {
                var4 = 1.0F;
            }

            int var5 = MathHelper.floor_double(var1.posX);
            int var6 = MathHelper.floor_double(var1.posZ);
            float var7 = (float)getBiomeSource().getTemperature(var5, var6);
            int var8 = getBiomeSource().getBiome(var5, var6).getSkyColorByTemp(var7);
            float var9 = (float)(var8 >> 16 & 255) / 255.0F;
            float var10 = (float)(var8 >> 8 & 255) / 255.0F;
            float var11 = (float)(var8 & 255) / 255.0F;
            var9 *= var4;
            var10 *= var4;
            var11 *= var4;
            float var12 = func_27162_g(var2);
            float var13;
            float var14;
            if (var12 > 0.0F)
            {
                var13 = (var9 * 0.3F + var10 * 0.59F + var11 * 0.11F) * 0.6F;
                var14 = 1.0F - var12 * (12.0F / 16.0F);
                var9 = var9 * var14 + var13 * (1.0F - var14);
                var10 = var10 * var14 + var13 * (1.0F - var14);
                var11 = var11 * var14 + var13 * (1.0F - var14);
            }

            var13 = func_27166_f(var2);
            if (var13 > 0.0F)
            {
                var14 = (var9 * 0.3F + var10 * 0.59F + var11 * 0.11F) * 0.2F;
                float var15 = 1.0F - var13 * (12.0F / 16.0F);
                var9 = var9 * var15 + var14 * (1.0F - var15);
                var10 = var10 * var15 + var14 * (1.0F - var15);
                var11 = var11 * var15 + var14 * (1.0F - var15);
            }

            if (field_27172_i > 0)
            {
                var14 = (float)field_27172_i - var2;
                if (var14 > 1.0F)
                {
                    var14 = 1.0F;
                }

                var14 *= 0.45F;
                var9 = var9 * (1.0F - var14) + 0.8F * var14;
                var10 = var10 * (1.0F - var14) + 0.8F * var14;
                var11 = var11 * (1.0F - var14) + 1.0F * var14;
            }

            return new((double)var9, (double)var10, (double)var11);
        }

        public float getCelestialAngle(float var1)
        {
            return dimension.calculateCelestialAngle(worldInfo.getWorldTime(), var1);
        }

        public Vector3D<double> func_628_d(float var1)
        {
            float var2 = getCelestialAngle(var1);
            float var3 = MathHelper.cos(var2 * (float)java.lang.Math.PI * 2.0F) * 2.0F + 0.5F;
            if (var3 < 0.0F)
            {
                var3 = 0.0F;
            }

            if (var3 > 1.0F)
            {
                var3 = 1.0F;
            }

            float var4 = (float)(field_1019_F >> 16 & 255L) / 255.0F;
            float var5 = (float)(field_1019_F >> 8 & 255L) / 255.0F;
            float var6 = (float)(field_1019_F & 255L) / 255.0F;
            float var7 = func_27162_g(var1);
            float var8;
            float var9;
            if (var7 > 0.0F)
            {
                var8 = (var4 * 0.3F + var5 * 0.59F + var6 * 0.11F) * 0.6F;
                var9 = 1.0F - var7 * 0.95F;
                var4 = var4 * var9 + var8 * (1.0F - var9);
                var5 = var5 * var9 + var8 * (1.0F - var9);
                var6 = var6 * var9 + var8 * (1.0F - var9);
            }

            var4 *= var3 * 0.9F + 0.1F;
            var5 *= var3 * 0.9F + 0.1F;
            var6 *= var3 * 0.85F + 0.15F;
            var8 = func_27166_f(var1);
            if (var8 > 0.0F)
            {
                var9 = (var4 * 0.3F + var5 * 0.59F + var6 * 0.11F) * 0.2F;
                float var10 = 1.0F - var8 * 0.95F;
                var4 = var4 * var10 + var9 * (1.0F - var10);
                var5 = var5 * var10 + var9 * (1.0F - var10);
                var6 = var6 * var10 + var9 * (1.0F - var10);
            }

            return new((double)var4, (double)var5, (double)var6);
        }

        public Vector3D<double> getFogColor(float var1)
        {
            float var2 = getCelestialAngle(var1);
            return dimension.func_4096_a(var2, var1);
        }

        public int findTopSolidBlock(int var1, int var2)
        {
            Chunk var3 = getChunkFromBlockCoords(var1, var2);
            int var4 = 127;
            var1 &= 15;

            for (var2 &= 15; var4 > 0; --var4)
            {
                int var5 = var3.getBlockID(var1, var4, var2);
                Material var6 = var5 == 0 ? Material.AIR : Block.BLOCKS[var5].material;
                if (var6.blocksMovement() || var6.isFluid())
                {
                    return var4 + 1;
                }
            }

            return -1;
        }

        public float getStarBrightness(float var1)
        {
            float var2 = getCelestialAngle(var1);
            float var3 = 1.0F - (MathHelper.cos(var2 * (float)java.lang.Math.PI * 2.0F) * 2.0F + 12.0F / 16.0F);
            if (var3 < 0.0F)
            {
                var3 = 0.0F;
            }

            if (var3 > 1.0F)
            {
                var3 = 1.0F;
            }

            return var3 * var3 * 0.5F;
        }

        public virtual void scheduleBlockUpdate(int var1, int var2, int var3, int var4, int var5)
        {
            NextTickListEntry var6 = new(var1, var2, var3, var4);
            byte var7 = 8;
            if (scheduledUpdatesAreImmediate)
            {
                if (checkChunksExist(var6.xCoord - var7, var6.yCoord - var7, var6.zCoord - var7, var6.xCoord + var7, var6.yCoord + var7, var6.zCoord + var7))
                {
                    int var8 = getBlockId(var6.xCoord, var6.yCoord, var6.zCoord);
                    if (var8 == var6.blockID && var8 > 0)
                    {
                        Block.BLOCKS[var8].onTick(this, var6.xCoord, var6.yCoord, var6.zCoord, random);
                    }
                }

            }
            else
            {
                if (checkChunksExist(var1 - var7, var2 - var7, var3 - var7, var1 + var7, var2 + var7, var3 + var7))
                {
                    if (var4 > 0)
                    {
                        var6.setScheduledTime((long)var5 + worldInfo.getWorldTime());
                    }

                    if (!scheduledTickSet.contains(var6))
                    {
                        scheduledTickSet.add(var6);
                        scheduledTickTreeSet.add(var6);
                    }
                }

            }
        }

        public void updateEntities()
        {
            Profiler.Start("updateEntites.updateWeatherEffects");

            int var1;
            Entity var2;
            for (var1 = 0; var1 < weatherEffects.size(); ++var1)
            {
                var2 = (Entity)weatherEffects.get(var1);
                var2.onUpdate();
                if (var2.isDead)
                {
                    weatherEffects.remove(var1--);
                }
            }
            Profiler.Stop("updateEntites.updateWeatherEffects");

            foreach (var entity in unloadedEntityList)
            {
                loadedEntityList.Remove(entity);
            }

            Profiler.Start("updateEntites.clearUnloadedEntities");

            int var3;
            int var4;
            for (var1 = 0; var1 < unloadedEntityList.Count; ++var1)
            {
                var2 = unloadedEntityList[var1];
                var3 = var2.chunkCoordX;
                var4 = var2.chunkCoordZ;
                if (var2.addedToChunk && chunkExists(var3, var4))
                {
                    getChunkFromChunkCoords(var3, var4).removeEntity(var2);
                }
            }

            for (var1 = 0; var1 < unloadedEntityList.Count; ++var1)
            {
                releaseEntitySkin(unloadedEntityList[var1]);
            }

            unloadedEntityList.Clear();

            Profiler.Stop("updateEntites.clearUnloadedEntities");

            Profiler.Start("updateEntites.updateLoadedEntities");

            for (var1 = 0; var1 < loadedEntityList.Count; ++var1)
            {
                var2 = loadedEntityList[var1];
                if (var2.ridingEntity != null)
                {
                    if (!var2.ridingEntity.isDead && var2.ridingEntity.riddenByEntity == var2)
                    {
                        continue;
                    }

                    var2.ridingEntity.riddenByEntity = null;
                    var2.ridingEntity = null;
                }

                if (!var2.isDead)
                {
                    updateEntity(var2);
                }

                if (var2.isDead)
                {
                    var3 = var2.chunkCoordX;
                    var4 = var2.chunkCoordZ;
                    if (var2.addedToChunk && chunkExists(var3, var4))
                    {
                        getChunkFromChunkCoords(var3, var4).removeEntity(var2);
                    }

                    loadedEntityList.RemoveAt(var1--);
                    releaseEntitySkin(var2);
                }
            }
            Profiler.Stop("updateEntites.updateLoadedEntities");

            field_31055_L = true;

            Profiler.Start("updateEntites.updateLoadedTileEntities");

            for (int i = loadedTileEntityList.Count - 1; i >= 0; i--)
            {
                TileEntity var5 = loadedTileEntityList[i];
                if (!var5.isRemoved())
                {
                    var5.tick();
                }
                if (var5.isRemoved())
                {
                    loadedTileEntityList.RemoveAt(i);
                    Chunk var7 = getChunkFromChunkCoords(var5.x >> 4, var5.z >> 4);
                    if (var7 != null)
                    {
                        var7.removeChunkBlockTileEntity(var5.x & 15, var5.y, var5.z & 15);
                    }
                }
            }

            field_31055_L = false;
            if (field_30900_E.Count > 0)
            {
                foreach (TileEntity var8 in field_30900_E)
                {
                    if (!var8.isRemoved())
                    {
                        if (!loadedTileEntityList.Contains(var8))
                        {
                            loadedTileEntityList.Add(var8);
                        }
                        Chunk var9 = getChunkFromChunkCoords(var8.x >> 4, var8.z >> 4);
                        if (var9 != null)
                        {
                            var9.setChunkBlockTileEntity(var8.x & 15, var8.y, var8.z & 15, var8);
                        }
                        markBlockNeedsUpdate(var8.x, var8.y, var8.z);
                    }
                }
                field_30900_E.Clear();
            }
            Profiler.Stop("updateEntites.updateLoadedTileEntities");

        }

        public void func_31054_a(IEnumerable<TileEntity> var1)
        {
            if (field_31055_L)
            {
                field_30900_E.AddRange(var1);
            }
            else
            {
                loadedTileEntityList.AddRange(var1);
            }

        }

        public void updateEntity(Entity var1)
        {
            updateEntityWithOptionalForce(var1, true);
        }

        public void updateEntityWithOptionalForce(Entity var1, bool var2)
        {
            int var3 = MathHelper.floor_double(var1.posX);
            int var4 = MathHelper.floor_double(var1.posZ);
            byte var5 = 32;
            if (!var2 || checkChunksExist(var3 - var5, 0, var4 - var5, var3 + var5, 128, var4 + var5))
            {
                var1.lastTickPosX = var1.posX;
                var1.lastTickPosY = var1.posY;
                var1.lastTickPosZ = var1.posZ;
                var1.prevRotationYaw = var1.rotationYaw;
                var1.prevRotationPitch = var1.rotationPitch;
                if (var2 && var1.addedToChunk)
                {
                    if (var1.ridingEntity != null)
                    {
                        var1.updateRidden();
                    }
                    else
                    {
                        var1.onUpdate();
                    }
                }

                if (java.lang.Double.isNaN(var1.posX) || java.lang.Double.isInfinite(var1.posX))
                {
                    var1.posX = var1.lastTickPosX;
                }

                if (java.lang.Double.isNaN(var1.posY) || java.lang.Double.isInfinite(var1.posY))
                {
                    var1.posY = var1.lastTickPosY;
                }

                if (java.lang.Double.isNaN(var1.posZ) || java.lang.Double.isInfinite(var1.posZ))
                {
                    var1.posZ = var1.lastTickPosZ;
                }

                if (java.lang.Double.isNaN((double)var1.rotationPitch) || java.lang.Double.isInfinite((double)var1.rotationPitch))
                {
                    var1.rotationPitch = var1.prevRotationPitch;
                }

                if (java.lang.Double.isNaN((double)var1.rotationYaw) || java.lang.Double.isInfinite((double)var1.rotationYaw))
                {
                    var1.rotationYaw = var1.prevRotationYaw;
                }

                int var6 = MathHelper.floor_double(var1.posX / 16.0D);
                int var7 = MathHelper.floor_double(var1.posY / 16.0D);
                int var8 = MathHelper.floor_double(var1.posZ / 16.0D);
                if (!var1.addedToChunk || var1.chunkCoordX != var6 || var1.chunkCoordY != var7 || var1.chunkCoordZ != var8)
                {
                    if (var1.addedToChunk && chunkExists(var1.chunkCoordX, var1.chunkCoordZ))
                    {
                        getChunkFromChunkCoords(var1.chunkCoordX, var1.chunkCoordZ).removeEntityAtIndex(var1, var1.chunkCoordY);
                    }

                    if (chunkExists(var6, var8))
                    {
                        var1.addedToChunk = true;
                        getChunkFromChunkCoords(var6, var8).addEntity(var1);
                    }
                    else
                    {
                        var1.addedToChunk = false;
                    }
                }

                if (var2 && var1.addedToChunk && var1.riddenByEntity != null)
                {
                    if (!var1.riddenByEntity.isDead && var1.riddenByEntity.ridingEntity == var1)
                    {
                        updateEntity(var1.riddenByEntity);
                    }
                    else
                    {
                        var1.riddenByEntity.ridingEntity = null;
                        var1.riddenByEntity = null;
                    }
                }

            }
        }

        public bool checkIfAABBIsClear(Box var1)
        {
            List<Entity> var2 = getEntitiesWithinAABBExcludingEntity((Entity)null, var1);

            for (int var3 = 0; var3 < var2.Count; ++var3)
            {
                Entity var4 = var2[var3];
                if (!var4.isDead && var4.preventEntitySpawning)
                {
                    return false;
                }
            }

            return true;
        }

        public bool getIsAnyLiquid(Box var1)
        {
            int var2 = MathHelper.floor_double(var1.minX);
            int var3 = MathHelper.floor_double(var1.maxX + 1.0D);
            int var4 = MathHelper.floor_double(var1.minY);
            int var5 = MathHelper.floor_double(var1.maxY + 1.0D);
            int var6 = MathHelper.floor_double(var1.minZ);
            int var7 = MathHelper.floor_double(var1.maxZ + 1.0D);
            if (var1.minX < 0.0D)
            {
                --var2;
            }

            if (var1.minY < 0.0D)
            {
                --var4;
            }

            if (var1.minZ < 0.0D)
            {
                --var6;
            }

            for (int var8 = var2; var8 < var3; ++var8)
            {
                for (int var9 = var4; var9 < var5; ++var9)
                {
                    for (int var10 = var6; var10 < var7; ++var10)
                    {
                        Block var11 = Block.BLOCKS[getBlockId(var8, var9, var10)];
                        if (var11 != null && var11.material.isFluid())
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool isBoundingBoxBurning(Box var1)
        {
            int var2 = MathHelper.floor_double(var1.minX);
            int var3 = MathHelper.floor_double(var1.maxX + 1.0D);
            int var4 = MathHelper.floor_double(var1.minY);
            int var5 = MathHelper.floor_double(var1.maxY + 1.0D);
            int var6 = MathHelper.floor_double(var1.minZ);
            int var7 = MathHelper.floor_double(var1.maxZ + 1.0D);
            if (checkChunksExist(var2, var4, var6, var3, var5, var7))
            {
                for (int var8 = var2; var8 < var3; ++var8)
                {
                    for (int var9 = var4; var9 < var5; ++var9)
                    {
                        for (int var10 = var6; var10 < var7; ++var10)
                        {
                            int var11 = getBlockId(var8, var9, var10);
                            if (var11 == Block.FIRE.id || var11 == Block.FLOWING_LAVA.id || var11 == Block.LAVA.id)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public bool handleMaterialAcceleration(Box var1, Material var2, Entity var3)
        {
            int var4 = MathHelper.floor_double(var1.minX);
            int var5 = MathHelper.floor_double(var1.maxX + 1.0D);
            int var6 = MathHelper.floor_double(var1.minY);
            int var7 = MathHelper.floor_double(var1.maxY + 1.0D);
            int var8 = MathHelper.floor_double(var1.minZ);
            int var9 = MathHelper.floor_double(var1.maxZ + 1.0D);
            if (!checkChunksExist(var4, var6, var8, var5, var7, var9))
            {
                return false;
            }
            else
            {
                bool var10 = false;
                Vec3D var11 = Vec3D.createVector(0.0D, 0.0D, 0.0D);

                for (int var12 = var4; var12 < var5; ++var12)
                {
                    for (int var13 = var6; var13 < var7; ++var13)
                    {
                        for (int var14 = var8; var14 < var9; ++var14)
                        {
                            Block var15 = Block.BLOCKS[getBlockId(var12, var13, var14)];
                            if (var15 != null && var15.material == var2)
                            {
                                double var16 = (double)((float)(var13 + 1) - BlockFluid.getPercentAir(getBlockMeta(var12, var13, var14)));
                                if ((double)var7 >= var16)
                                {
                                    var10 = true;
                                    var15.velocityToAddToEntity(this, var12, var13, var14, var3, var11);
                                }
                            }
                        }
                    }
                }

                if (var11.lengthVector() > 0.0D)
                {
                    var11 = var11.normalize();
                    double var18 = 0.014D;
                    var3.motionX += var11.xCoord * var18;
                    var3.motionY += var11.yCoord * var18;
                    var3.motionZ += var11.zCoord * var18;
                }

                return var10;
            }
        }

        public bool isMaterialInBB(Box var1, Material var2)
        {
            int var3 = MathHelper.floor_double(var1.minX);
            int var4 = MathHelper.floor_double(var1.maxX + 1.0D);
            int var5 = MathHelper.floor_double(var1.minY);
            int var6 = MathHelper.floor_double(var1.maxY + 1.0D);
            int var7 = MathHelper.floor_double(var1.minZ);
            int var8 = MathHelper.floor_double(var1.maxZ + 1.0D);

            for (int var9 = var3; var9 < var4; ++var9)
            {
                for (int var10 = var5; var10 < var6; ++var10)
                {
                    for (int var11 = var7; var11 < var8; ++var11)
                    {
                        Block var12 = Block.BLOCKS[getBlockId(var9, var10, var11)];
                        if (var12 != null && var12.material == var2)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool isAABBInMaterial(Box var1, Material var2)
        {
            int var3 = MathHelper.floor_double(var1.minX);
            int var4 = MathHelper.floor_double(var1.maxX + 1.0D);
            int var5 = MathHelper.floor_double(var1.minY);
            int var6 = MathHelper.floor_double(var1.maxY + 1.0D);
            int var7 = MathHelper.floor_double(var1.minZ);
            int var8 = MathHelper.floor_double(var1.maxZ + 1.0D);

            for (int var9 = var3; var9 < var4; ++var9)
            {
                for (int var10 = var5; var10 < var6; ++var10)
                {
                    for (int var11 = var7; var11 < var8; ++var11)
                    {
                        Block var12 = Block.BLOCKS[getBlockId(var9, var10, var11)];
                        if (var12 != null && var12.material == var2)
                        {
                            int var13 = getBlockMeta(var9, var10, var11);
                            double var14 = (double)(var10 + 1);
                            if (var13 < 8)
                            {
                                var14 = (double)(var10 + 1) - (double)var13 / 8.0D;
                            }

                            if (var14 >= var1.minY)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public Explosion createExplosion(Entity var1, double var2, double var4, double var6, float var8)
        {
            return newExplosion(var1, var2, var4, var6, var8, false);
        }

        public Explosion newExplosion(Entity var1, double var2, double var4, double var6, float var8, bool var9)
        {
            Explosion var10 = new(this, var1, var2, var4, var6, var8);
            var10.isFlaming = var9;
            var10.doExplosionA();
            var10.doExplosionB(true);
            return var10;
        }

        public float func_675_a(Vec3D var1, Box var2)
        {
            double var3 = 1.0D / ((var2.maxX - var2.minX) * 2.0D + 1.0D);
            double var5 = 1.0D / ((var2.maxY - var2.minY) * 2.0D + 1.0D);
            double var7 = 1.0D / ((var2.maxZ - var2.minZ) * 2.0D + 1.0D);
            int var9 = 0;
            int var10 = 0;

            for (float var11 = 0.0F; var11 <= 1.0F; var11 = (float)((double)var11 + var3))
            {
                for (float var12 = 0.0F; var12 <= 1.0F; var12 = (float)((double)var12 + var5))
                {
                    for (float var13 = 0.0F; var13 <= 1.0F; var13 = (float)((double)var13 + var7))
                    {
                        double var14 = var2.minX + (var2.maxX - var2.minX) * (double)var11;
                        double var16 = var2.minY + (var2.maxY - var2.minY) * (double)var12;
                        double var18 = var2.minZ + (var2.maxZ - var2.minZ) * (double)var13;
                        if (rayTraceBlocks(Vec3D.createVector(var14, var16, var18), var1) == null)
                        {
                            ++var9;
                        }

                        ++var10;
                    }
                }
            }

            return (float)var9 / (float)var10;
        }

        public void onBlockHit(EntityPlayer var1, int var2, int var3, int var4, int var5)
        {
            if (var5 == 0)
            {
                --var3;
            }

            if (var5 == 1)
            {
                ++var3;
            }

            if (var5 == 2)
            {
                --var4;
            }

            if (var5 == 3)
            {
                ++var4;
            }

            if (var5 == 4)
            {
                --var2;
            }

            if (var5 == 5)
            {
                ++var2;
            }

            if (getBlockId(var2, var3, var4) == Block.FIRE.id)
            {
                func_28107_a(var1, 1004, var2, var3, var4, 0);
                setBlockWithNotify(var2, var3, var4, 0);
            }

        }

        public Entity func_4085_a(java.lang.Class var1)
        {
            return null;
        }

        public string func_687_d()
        {
            return "All: " + loadedEntityList.Count;
        }

        public string func_21119_g()
        {
            return chunkProvider.makeString();
        }

        public TileEntity getBlockTileEntity(int var1, int var2, int var3)
        {
            Chunk var4 = getChunkFromChunkCoords(var1 >> 4, var3 >> 4);
            return var4 != null ? var4.getChunkBlockTileEntity(var1 & 15, var2, var3 & 15) : null;
        }

        public void setBlockTileEntity(int var1, int var2, int var3, TileEntity var4)
        {
            if (!var4.isRemoved())
            {
                if (field_31055_L)
                {
                    var4.x = var1;
                    var4.y = var2;
                    var4.z = var3;
                    field_30900_E.Add(var4);
                }
                else
                {
                    loadedTileEntityList.Add(var4);
                    Chunk var5 = getChunkFromChunkCoords(var1 >> 4, var3 >> 4);
                    if (var5 != null)
                    {
                        var5.setChunkBlockTileEntity(var1 & 15, var2, var3 & 15, var4);
                    }
                }
            }

        }

        public void removeBlockTileEntity(int var1, int var2, int var3)
        {
            TileEntity var4 = getBlockTileEntity(var1, var2, var3);
            if (var4 != null && field_31055_L)
            {
                var4.markRemoved();
            }
            else
            {
                if (var4 != null)
                {
                    loadedTileEntityList.Remove(var4);
                }

                Chunk var5 = getChunkFromChunkCoords(var1 >> 4, var3 >> 4);
                if (var5 != null)
                {
                    var5.removeChunkBlockTileEntity(var1 & 15, var2, var3 & 15);
                }
            }

        }

        public bool isOpaque(int var1, int var2, int var3)
        {
            Block var4 = Block.BLOCKS[getBlockId(var1, var2, var3)];
            return var4 == null ? false : var4.isOpaque();
        }

        public bool shouldSuffocate(int var1, int var2, int var3)
        {
            Block var4 = Block.BLOCKS[getBlockId(var1, var2, var3)];
            return var4 == null ? false : var4.material.suffocates() && var4.isFullCube();
        }

        public void saveWorldIndirectly(IProgressUpdate var1)
        {
            saveWorld(true, var1);
        }

        public bool updatingLighting()
        {
            if (lightingUpdatesCounter >= 50)
            {
                return false;
            }
            else
            {
                ++lightingUpdatesCounter;

                bool var2;
                try
                {
                    int var1 = 500;

                    while (lightingToUpdate.Count > 0)
                    {
                        --var1;
                        if (var1 <= 0)
                        {
                            var2 = true;
                            return var2;
                        }

                        int lastIndex = lightingToUpdate.Count - 1;
                        MetadataChunkBlock mcb = lightingToUpdate[lastIndex];

                        lightingToUpdate.RemoveAt(lastIndex);
                        mcb.func_4127_a(this);
                    }

                    var2 = false;
                }
                finally
                {
                    --lightingUpdatesCounter;
                }

                return var2;
            }
        }

        public void scheduleLightingUpdate(EnumSkyBlock var1, int var2, int var3, int var4, int var5, int var6, int var7)
        {
            scheduleLightingUpdate_do(var1, var2, var3, var4, var5, var6, var7, true);
        }

        public void scheduleLightingUpdate_do(EnumSkyBlock var1, int var2, int var3, int var4, int var5, int var6, int var7, bool var8)
        {
            if (!dimension.hasNoSky || var1 != EnumSkyBlock.Sky)
            {
                ++lightingUpdatesScheduled;

                try
                {
                    if (lightingUpdatesScheduled == 50)
                    {
                        return;
                    }

                    int var9 = (var5 + var2) / 2;
                    int var10 = (var7 + var4) / 2;
                    if (blockExists(var9, 64, var10))
                    {
                        if (getChunkFromBlockCoords(var9, var10).func_21167_h())
                        {
                            return;
                        }

                        int var11 = lightingToUpdate.Count;
                        int var12;
                        var span = CollectionsMarshal.AsSpan(lightingToUpdate);

                        if (var8)
                        {
                            var12 = 5;
                            if (var12 > var11)
                            {
                                var12 = var11;
                            }

                            for (int var13 = 0; var13 < var12; ++var13)
                            {
                                ref MetadataChunkBlock var14 = ref span[lightingToUpdate.Count - var13 - 1];
                                if (var14.field_1299_a == var1 && var14.func_866_a(var2, var3, var4, var5, var6, var7))
                                {
                                    return;
                                }
                            }
                        }

                        lightingToUpdate.Add(new MetadataChunkBlock(var1, var2, var3, var4, var5, var6, var7));
                        var12 = 1000000;
                        if (lightingToUpdate.Count > 1000000)
                        {
                            java.lang.System.@out.println("More than " + var12 + " updates, aborting lighting updates");
                            lightingToUpdate.Clear();
                        }

                        return;
                    }
                }
                finally
                {
                    --lightingUpdatesScheduled;
                }

            }
        }

        public void calculateInitialSkylight()
        {
            int var1 = calculateSkylightSubtracted(1.0F);
            if (var1 != skylightSubtracted)
            {
                skylightSubtracted = var1;
            }

        }

        public void setAllowedMobSpawns(bool var1, bool var2)
        {
            spawnHostileMobs = var1;
            spawnPeacefulMobs = var2;
        }

        public virtual void tick(int renderDistance)
        {
            updateWeather();
            long var2;
            if (isAllPlayersFullyAsleep())
            {
                bool var1 = false;
                if (spawnHostileMobs && difficultySetting >= 1)
                {
                    var1 = SpawnerAnimals.performSleepSpawning(this, playerEntities);
                }

                if (!var1)
                {
                    var2 = worldInfo.getWorldTime() + 24000L;
                    worldInfo.setWorldTime(var2 - var2 % 24000L);
                    wakeUpAllPlayers();
                }
            }
            Profiler.Start("performSpawning");
            SpawnerAnimals.performSpawning(this, spawnHostileMobs, spawnPeacefulMobs);
            Profiler.Stop("performSpawning");
            Profiler.Start("unload100OldestChunks");
            chunkProvider.unload100OldestChunks();
            Profiler.Stop("unload100OldestChunks");

            Profiler.Start("updateSkylightSubtracted");
            int var4 = calculateSkylightSubtracted(1.0F);
            if (var4 != skylightSubtracted)
            {
                skylightSubtracted = var4;

                for (int var5 = 0; var5 < worldAccesses.Count; ++var5)
                {
                    worldAccesses[var5].updateAllRenderers();
                }
            }
            Profiler.Stop("updateSkylightSubtracted");

            var2 = worldInfo.getWorldTime() + 1L;
            if (var2 % (long)autosavePeriod == 0L)
            {
                Profiler.PushGroup("autosave");
                saveWorld(false, (IProgressUpdate)null);
                Profiler.PopGroup();

                chunkProvider.markChunksForUnload(renderDistance);
            }

            worldInfo.setWorldTime(var2);
            Profiler.Start("tickUpdates");
            TickUpdates(false);
            Profiler.Stop("tickUpdates");
            updateBlocksAndPlayCaveSounds();
        }

        private void func_27163_E()
        {
            if (worldInfo.getRaining())
            {
                rainingStrength = 1.0F;
                if (worldInfo.getThundering())
                {
                    thunderingStrength = 1.0F;
                }
            }

        }

        protected virtual void updateWeather()
        {
            if (!dimension.hasNoSky)
            {
                if (field_27168_F > 0)
                {
                    --field_27168_F;
                }

                int var1 = worldInfo.getThunderTime();
                if (var1 <= 0)
                {
                    if (worldInfo.getThundering())
                    {
                        worldInfo.setThunderTime(random.nextInt(12000) + 3600);
                    }
                    else
                    {
                        worldInfo.setThunderTime(random.nextInt(168000) + 12000);
                    }
                }
                else
                {
                    --var1;
                    worldInfo.setThunderTime(var1);
                    if (var1 <= 0)
                    {
                        worldInfo.setThundering(!worldInfo.getThundering());
                    }
                }

                int var2 = worldInfo.getRainTime();
                if (var2 <= 0)
                {
                    if (worldInfo.getRaining())
                    {
                        worldInfo.setRainTime(random.nextInt(12000) + 12000);
                    }
                    else
                    {
                        worldInfo.setRainTime(random.nextInt(168000) + 12000);
                    }
                }
                else
                {
                    --var2;
                    worldInfo.setRainTime(var2);
                    if (var2 <= 0)
                    {
                        worldInfo.setRaining(!worldInfo.getRaining());
                    }
                }

                prevRainingStrength = rainingStrength;
                if (worldInfo.getRaining())
                {
                    rainingStrength = (float)((double)rainingStrength + 0.01D);
                }
                else
                {
                    rainingStrength = (float)((double)rainingStrength - 0.01D);
                }

                if (rainingStrength < 0.0F)
                {
                    rainingStrength = 0.0F;
                }

                if (rainingStrength > 1.0F)
                {
                    rainingStrength = 1.0F;
                }

                prevThunderingStrength = thunderingStrength;
                if (worldInfo.getThundering())
                {
                    thunderingStrength = (float)((double)thunderingStrength + 0.01D);
                }
                else
                {
                    thunderingStrength = (float)((double)thunderingStrength - 0.01D);
                }

                if (thunderingStrength < 0.0F)
                {
                    thunderingStrength = 0.0F;
                }

                if (thunderingStrength > 1.0F)
                {
                    thunderingStrength = 1.0F;
                }

            }
        }

        private void stopPrecipitation()
        {
            worldInfo.setRainTime(0);
            worldInfo.setRaining(false);
            worldInfo.setThunderTime(0);
            worldInfo.setThundering(false);
        }

        protected virtual void updateBlocksAndPlayCaveSounds()
        {
            positionsToUpdate.clear();

            int var3;
            int var4;
            int var6;
            int var7;
            for (int var1 = 0; var1 < playerEntities.size(); ++var1)
            {
                EntityPlayer var2 = (EntityPlayer)playerEntities.get(var1);
                var3 = MathHelper.floor_double(var2.posX / 16.0D);
                var4 = MathHelper.floor_double(var2.posZ / 16.0D);
                byte var5 = 9;

                for (var6 = -var5; var6 <= var5; ++var6)
                {
                    for (var7 = -var5; var7 <= var5; ++var7)
                    {
                        positionsToUpdate.add(new ChunkPos(var6 + var3, var7 + var4));
                    }
                }
            }

            if (soundCounter > 0)
            {
                --soundCounter;
            }

            Iterator var12 = positionsToUpdate.iterator();

            while (var12.hasNext())
            {
                ChunkPos var13 = (ChunkPos)var12.next();
                var3 = var13.x * 16;
                var4 = var13.z * 16;
                Chunk var14 = getChunkFromChunkCoords(var13.x, var13.z);
                int var8;
                int var9;
                int var10;
                if (soundCounter == 0)
                {
                    field_9437_g = field_9437_g * 3 + 1013904223;
                    var6 = field_9437_g >> 2;
                    var7 = var6 & 15;
                    var8 = var6 >> 8 & 15;
                    var9 = var6 >> 16 & 127;
                    var10 = var14.getBlockID(var7, var9, var8);
                    var7 += var3;
                    var8 += var4;
                    if (var10 == 0 && getFullBlockLightValue(var7, var9, var8) <= random.nextInt(8) && getSavedLightValue(EnumSkyBlock.Sky, var7, var9, var8) <= 0)
                    {
                        EntityPlayer var11 = getClosestPlayer((double)var7 + 0.5D, (double)var9 + 0.5D, (double)var8 + 0.5D, 8.0D);
                        if (var11 != null && var11.getSquaredDistance((double)var7 + 0.5D, (double)var9 + 0.5D, (double)var8 + 0.5D) > 4.0D)
                        {
                            playSoundEffect((double)var7 + 0.5D, (double)var9 + 0.5D, (double)var8 + 0.5D, "ambient.cave.cave", 0.7F, 0.8F + random.nextFloat() * 0.2F);
                            soundCounter = random.nextInt(12000) + 6000;
                        }
                    }
                }

                if (random.nextInt(100000) == 0 && func_27161_C() && func_27160_B())
                {
                    field_9437_g = field_9437_g * 3 + 1013904223;
                    var6 = field_9437_g >> 2;
                    var7 = var3 + (var6 & 15);
                    var8 = var4 + (var6 >> 8 & 15);
                    var9 = findTopSolidBlock(var7, var8);
                    if (canBlockBeRainedOn(var7, var9, var8))
                    {
                        addWeatherEffect(new EntityLightningBolt(this, (double)var7, (double)var9, (double)var8));
                        field_27168_F = 2;
                    }
                }

                int var15;
                if (random.nextInt(16) == 0)
                {
                    field_9437_g = field_9437_g * 3 + 1013904223;
                    var6 = field_9437_g >> 2;
                    var7 = var6 & 15;
                    var8 = var6 >> 8 & 15;
                    var9 = findTopSolidBlock(var7 + var3, var8 + var4);
                    if (getBiomeSource().getBiome(var7 + var3, var8 + var4).getEnableSnow() && var9 >= 0 && var9 < 128 && var14.getSavedLightValue(EnumSkyBlock.Block, var7, var9, var8) < 10)
                    {
                        var10 = var14.getBlockID(var7, var9 - 1, var8);
                        var15 = var14.getBlockID(var7, var9, var8);
                        if (func_27161_C() && var15 == 0 && Block.SNOW.canPlaceBlockAt(this, var7 + var3, var9, var8 + var4) && var10 != 0 && var10 != Block.ICE.id && Block.BLOCKS[var10].material.blocksMovement())
                        {
                            setBlockWithNotify(var7 + var3, var9, var8 + var4, Block.SNOW.id);
                        }

                        if (var10 == Block.WATER.id && var14.getBlockMetadata(var7, var9 - 1, var8) == 0)
                        {
                            setBlockWithNotify(var7 + var3, var9 - 1, var8 + var4, Block.ICE.id);
                        }
                    }
                }

                for (var6 = 0; var6 < 80; ++var6)
                {
                    field_9437_g = field_9437_g * 3 + 1013904223;
                    var7 = field_9437_g >> 2;
                    var8 = var7 & 15;
                    var9 = var7 >> 8 & 15;
                    var10 = var7 >> 16 & 127;
                    var15 = var14.blocks[var8 << 11 | var9 << 7 | var10] & 255;
                    if (Block.BLOCKS_RANDOM_TICK[var15])
                    {
                        Block.BLOCKS[var15].onTick(this, var8 + var3, var10, var9 + var4, random);
                    }
                }
            }

        }

        public virtual bool TickUpdates(bool var1)
        {
            int var2 = scheduledTickTreeSet.size();
            if (var2 != scheduledTickSet.size())
            {
                throw new IllegalStateException("TickNextTick list out of synch");
            }
            else
            {
                if (var2 > 1000)
                {
                    var2 = 1000;
                }

                for (int var3 = 0; var3 < var2; ++var3)
                {
                    NextTickListEntry var4 = (NextTickListEntry)scheduledTickTreeSet.first();
                    if (!var1 && var4.scheduledTime > worldInfo.getWorldTime())
                    {
                        break;
                    }

                    scheduledTickTreeSet.remove(var4);
                    scheduledTickSet.remove(var4);
                    byte var5 = 8;
                    if (checkChunksExist(var4.xCoord - var5, var4.yCoord - var5, var4.zCoord - var5, var4.xCoord + var5, var4.yCoord + var5, var4.zCoord + var5))
                    {
                        int var6 = getBlockId(var4.xCoord, var4.yCoord, var4.zCoord);
                        if (var6 == var4.blockID && var6 > 0)
                        {
                            Block.BLOCKS[var6].onTick(this, var4.xCoord, var4.yCoord, var4.zCoord, random);
                        }
                    }
                }

                return scheduledTickTreeSet.size() != 0;
            }
        }

        public void randomDisplayUpdates(int var1, int var2, int var3)
        {
            byte var4 = 16;
            java.util.Random var5 = new();

            for (int var6 = 0; var6 < 1000; ++var6)
            {
                int var7 = var1 + random.nextInt(var4) - random.nextInt(var4);
                int var8 = var2 + random.nextInt(var4) - random.nextInt(var4);
                int var9 = var3 + random.nextInt(var4) - random.nextInt(var4);
                int var10 = getBlockId(var7, var8, var9);
                if (var10 > 0)
                {
                    Block.BLOCKS[var10].randomDisplayTick(this, var7, var8, var9, var5);
                }
            }

        }

        public List<Entity> getEntitiesWithinAABBExcludingEntity(Entity var1, Box var2)
        {
            field_1012_M.Clear();
            int var3 = MathHelper.floor_double((var2.minX - 2.0D) / 16.0D);
            int var4 = MathHelper.floor_double((var2.maxX + 2.0D) / 16.0D);
            int var5 = MathHelper.floor_double((var2.minZ - 2.0D) / 16.0D);
            int var6 = MathHelper.floor_double((var2.maxZ + 2.0D) / 16.0D);

            for (int var7 = var3; var7 <= var4; ++var7)
            {
                for (int var8 = var5; var8 <= var6; ++var8)
                {
                    if (chunkExists(var7, var8))
                    {
                        getChunkFromChunkCoords(var7, var8).getEntitiesWithinAABBForEntity(var1, var2, field_1012_M);
                    }
                }
            }

            return field_1012_M;
        }

        public List<Entity> getEntitiesWithinAABB(Class var1, Box var2)
        {
            int var3 = MathHelper.floor_double((var2.minX - 2.0D) / 16.0D);
            int var4 = MathHelper.floor_double((var2.maxX + 2.0D) / 16.0D);
            int var5 = MathHelper.floor_double((var2.minZ - 2.0D) / 16.0D);
            int var6 = MathHelper.floor_double((var2.maxZ + 2.0D) / 16.0D);
            List<Entity> var7 = new();

            for (int var8 = var3; var8 <= var4; ++var8)
            {
                for (int var9 = var5; var9 <= var6; ++var9)
                {
                    if (chunkExists(var8, var9))
                    {
                        getChunkFromChunkCoords(var8, var9).getEntitiesOfTypeWithinAAAB(var1, var2, var7);
                    }
                }
            }

            return var7;
        }

        public List<Entity> getLoadedEntityList()
        {
            return loadedEntityList;
        }

        public void updateBlockEntity(int var1, int var2, int var3, TileEntity var4)
        {
            if (blockExists(var1, var2, var3))
            {
                getChunkFromBlockCoords(var1, var3).setChunkModified();
            }

            for (int var5 = 0; var5 < worldAccesses.Count; ++var5)
            {
                worldAccesses[var5].doNothingWithTileEntity(var1, var2, var3, var4);
            }

        }

        public int countEntities(Class var1)
        {
            int var2 = 0;

            for (int var3 = 0; var3 < loadedEntityList.Count; ++var3)
            {
                Entity var4 = loadedEntityList[var3];
                if (var1.isAssignableFrom(var4.getClass()))
                {
                    ++var2;
                }
            }

            return var2;
        }

        public void func_636_a(List<Entity> var1)
        {
            loadedEntityList.AddRange(var1);

            for (int var2 = 0; var2 < var1.Count; ++var2)
            {
                obtainEntitySkin(var1[var2]);
            }

        }

        public void func_632_b(List<Entity> var1)
        {
            unloadedEntityList.AddRange(var1);
        }

        public void func_656_j()
        {
            while (chunkProvider.unload100OldestChunks())
            {
            }

        }

        public bool canBlockBePlacedAt(int var1, int var2, int var3, int var4, bool var5, int var6)
        {
            int var7 = getBlockId(var2, var3, var4);
            Block var8 = Block.BLOCKS[var7];
            Block var9 = Block.BLOCKS[var1];
            Box var10 = var9.getCollisionShape(this, var2, var3, var4);
            if (var5)
            {
                var10 = null;
            }

            if (var10 != null && !checkIfAABBIsClear(var10))
            {
                return false;
            }
            else
            {
                if (var8 == Block.FLOWING_WATER || var8 == Block.WATER || var8 == Block.FLOWING_LAVA || var8 == Block.LAVA || var8 == Block.FIRE || var8 == Block.SNOW)
                {
                    var8 = null;
                }

                return var1 > 0 && var8 == null && var9.canPlaceBlockOnSide(this, var2, var3, var4, var6);
            }
        }

        public PathEntity getPathToEntity(Entity var1, Entity var2, float var3)
        {
            int var4 = MathHelper.floor_double(var1.posX);
            int var5 = MathHelper.floor_double(var1.posY);
            int var6 = MathHelper.floor_double(var1.posZ);
            int var7 = (int)(var3 + 16.0F);
            int var8 = var4 - var7;
            int var9 = var5 - var7;
            int var10 = var6 - var7;
            int var11 = var4 + var7;
            int var12 = var5 + var7;
            int var13 = var6 + var7;
            ChunkCache var14 = new(this, var8, var9, var10, var11, var12, var13);
            return (new Pathfinder(var14)).createEntityPathTo(var1, var2, var3);
        }

        public PathEntity getEntityPathToXYZ(Entity var1, int var2, int var3, int var4, float var5)
        {
            int var6 = MathHelper.floor_double(var1.posX);
            int var7 = MathHelper.floor_double(var1.posY);
            int var8 = MathHelper.floor_double(var1.posZ);
            int var9 = (int)(var5 + 8.0F);
            int var10 = var6 - var9;
            int var11 = var7 - var9;
            int var12 = var8 - var9;
            int var13 = var6 + var9;
            int var14 = var7 + var9;
            int var15 = var8 + var9;
            ChunkCache var16 = new(this, var10, var11, var12, var13, var14, var15);
            return (new Pathfinder(var16)).createEntityPathTo(var1, var2, var3, var4, var5);
        }

        public bool isBlockProvidingPowerTo(int var1, int var2, int var3, int var4)
        {
            int var5 = getBlockId(var1, var2, var3);
            return var5 == 0 ? false : Block.BLOCKS[var5].isIndirectlyPoweringTo(this, var1, var2, var3, var4);
        }

        public bool isBlockGettingPowered(int var1, int var2, int var3)
        {
            return isBlockProvidingPowerTo(var1, var2 - 1, var3, 0) ? true : (isBlockProvidingPowerTo(var1, var2 + 1, var3, 1) ? true : (isBlockProvidingPowerTo(var1, var2, var3 - 1, 2) ? true : (isBlockProvidingPowerTo(var1, var2, var3 + 1, 3) ? true : (isBlockProvidingPowerTo(var1 - 1, var2, var3, 4) ? true : isBlockProvidingPowerTo(var1 + 1, var2, var3, 5)))));
        }

        public bool isBlockIndirectlyProvidingPowerTo(int var1, int var2, int var3, int var4)
        {
            if (shouldSuffocate(var1, var2, var3))
            {
                return isBlockGettingPowered(var1, var2, var3);
            }
            else
            {
                int var5 = getBlockId(var1, var2, var3);
                return var5 == 0 ? false : Block.BLOCKS[var5].isPoweringTo(this, var1, var2, var3, var4);
            }
        }

        public bool isBlockIndirectlyGettingPowered(int var1, int var2, int var3)
        {
            return isBlockIndirectlyProvidingPowerTo(var1, var2 - 1, var3, 0) ? true : (isBlockIndirectlyProvidingPowerTo(var1, var2 + 1, var3, 1) ? true : (isBlockIndirectlyProvidingPowerTo(var1, var2, var3 - 1, 2) ? true : (isBlockIndirectlyProvidingPowerTo(var1, var2, var3 + 1, 3) ? true : (isBlockIndirectlyProvidingPowerTo(var1 - 1, var2, var3, 4) ? true : isBlockIndirectlyProvidingPowerTo(var1 + 1, var2, var3, 5)))));
        }

        public EntityPlayer getClosestPlayerToEntity(Entity var1, double var2)
        {
            return getClosestPlayer(var1.posX, var1.posY, var1.posZ, var2);
        }

        public EntityPlayer getClosestPlayer(double var1, double var3, double var5, double var7)
        {
            double var9 = -1.0D;
            EntityPlayer var11 = null;

            for (int var12 = 0; var12 < playerEntities.size(); ++var12)
            {
                EntityPlayer var13 = (EntityPlayer)playerEntities.get(var12);
                double var14 = var13.getSquaredDistance(var1, var3, var5);
                if ((var7 < 0.0D || var14 < var7 * var7) && (var9 == -1.0D || var14 < var9))
                {
                    var9 = var14;
                    var11 = var13;
                }
            }

            return var11;
        }

        public EntityPlayer getPlayerEntityByName(string var1)
        {
            for (int var2 = 0; var2 < playerEntities.size(); ++var2)
            {
                if (var1.Equals(((EntityPlayer)playerEntities.get(var2)).username))
                {
                    return (EntityPlayer)playerEntities.get(var2);
                }
            }

            return null;
        }

        public void setChunkData(int var1, int var2, int var3, int var4, int var5, int var6, byte[] var7)
        {
            int var8 = var1 >> 4;
            int var9 = var3 >> 4;
            int var10 = var1 + var4 - 1 >> 4;
            int var11 = var3 + var6 - 1 >> 4;
            int var12 = 0;
            int var13 = var2;
            int var14 = var2 + var5;
            if (var2 < 0)
            {
                var13 = 0;
            }

            if (var14 > 128)
            {
                var14 = 128;
            }

            for (int var15 = var8; var15 <= var10; ++var15)
            {
                int var16 = var1 - var15 * 16;
                int var17 = var1 + var4 - var15 * 16;
                if (var16 < 0)
                {
                    var16 = 0;
                }

                if (var17 > 16)
                {
                    var17 = 16;
                }

                for (int var18 = var9; var18 <= var11; ++var18)
                {
                    int var19 = var3 - var18 * 16;
                    int var20 = var3 + var6 - var18 * 16;
                    if (var19 < 0)
                    {
                        var19 = 0;
                    }

                    if (var20 > 16)
                    {
                        var20 = 16;
                    }

                    var12 = getChunkFromChunkCoords(var15, var18).setChunkData(var7, var16, var13, var19, var17, var14, var20, var12);
                    markBlocksDirty(var15 * 16 + var16, var13, var18 * 16 + var19, var15 * 16 + var17, var14, var18 * 16 + var20);
                }
            }

        }

        public virtual void sendQuittingDisconnectingPacket()
        {
        }

        public void checkSessionLock()
        {
            saveHandler.func_22150_b();
        }

        public void setWorldTime(long var1)
        {
            worldInfo.setWorldTime(var1);
        }

        public long getRandomSeed()
        {
            return worldInfo.getRandomSeed();
        }

        public long getWorldTime()
        {
            return worldInfo.getWorldTime();
        }

        public Vec3i getSpawnPoint()
        {
            return new Vec3i(worldInfo.getSpawnX(), worldInfo.getSpawnY(), worldInfo.getSpawnZ());
        }

        public void setSpawnPoint(Vec3i var1)
        {
            worldInfo.setSpawn(var1.x, var1.y, var1.z);
        }

        public void joinEntityInSurroundings(Entity var1)
        {
            int var2 = MathHelper.floor_double(var1.posX / 16.0D);
            int var3 = MathHelper.floor_double(var1.posZ / 16.0D);
            byte var4 = 2;

            for (int var5 = var2 - var4; var5 <= var2 + var4; ++var5)
            {
                for (int var6 = var3 - var4; var6 <= var3 + var4; ++var6)
                {
                    getChunkFromChunkCoords(var5, var6);
                }
            }

            if (!loadedEntityList.Contains(var1))
            {
                loadedEntityList.Add(var1);
            }

        }

        public bool func_6466_a(EntityPlayer var1, int var2, int var3, int var4)
        {
            return true;
        }

        public void func_9425_a(Entity var1, byte var2)
        {
        }

        public void updateEntityList()
        {
            foreach (var entity in unloadedEntityList)
            {
                loadedEntityList.Remove(entity);
            }

            int var1;
            Entity var2;
            int var3;
            int var4;
            for (var1 = 0; var1 < unloadedEntityList.Count; ++var1)
            {
                var2 = unloadedEntityList[var1];
                var3 = var2.chunkCoordX;
                var4 = var2.chunkCoordZ;
                if (var2.addedToChunk && chunkExists(var3, var4))
                {
                    getChunkFromChunkCoords(var3, var4).removeEntity(var2);
                }
            }

            for (var1 = 0; var1 < unloadedEntityList.Count; ++var1)
            {
                releaseEntitySkin(unloadedEntityList[var1]);
            }

            unloadedEntityList.Clear();

            for (var1 = 0; var1 < loadedEntityList.Count; ++var1)
            {
                var2 = loadedEntityList[var1];
                if (var2.ridingEntity != null)
                {
                    if (!var2.ridingEntity.isDead && var2.ridingEntity.riddenByEntity == var2)
                    {
                        continue;
                    }

                    var2.ridingEntity.riddenByEntity = null;
                    var2.ridingEntity = null;
                }

                if (var2.isDead)
                {
                    var3 = var2.chunkCoordX;
                    var4 = var2.chunkCoordZ;
                    if (var2.addedToChunk && chunkExists(var3, var4))
                    {
                        getChunkFromChunkCoords(var3, var4).removeEntity(var2);
                    }

                    loadedEntityList.RemoveAt(var1--);
                    releaseEntitySkin(var2);
                }
            }

        }

        public IChunkProvider getIChunkProvider()
        {
            return chunkProvider;
        }

        public void playNoteBlockActionAt(int var1, int var2, int var3, int var4, int var5)
        {
            int var6 = getBlockId(var1, var2, var3);
            if (var6 > 0)
            {
                Block.BLOCKS[var6].playBlock(this, var1, var2, var3, var4, var5);
            }

        }

        public WorldInfo getWorldInfo()
        {
            return worldInfo;
        }

        public void updateAllPlayersSleepingFlag()
        {
            allPlayersSleeping = !playerEntities.isEmpty();
            Iterator var1 = playerEntities.iterator();

            while (var1.hasNext())
            {
                EntityPlayer var2 = (EntityPlayer)var1.next();
                if (!var2.isPlayerSleeping())
                {
                    allPlayersSleeping = false;
                    break;
                }
            }

        }

        protected void wakeUpAllPlayers()
        {
            allPlayersSleeping = false;
            Iterator var1 = playerEntities.iterator();

            while (var1.hasNext())
            {
                EntityPlayer var2 = (EntityPlayer)var1.next();
                if (var2.isPlayerSleeping())
                {
                    var2.wakeUpPlayer(false, false, true);
                }
            }

            stopPrecipitation();
        }

        public bool isAllPlayersFullyAsleep()
        {
            if (allPlayersSleeping && !multiplayerWorld)
            {
                Iterator var1 = playerEntities.iterator();

                EntityPlayer var2;
                do
                {
                    if (!var1.hasNext())
                    {
                        return true;
                    }

                    var2 = (EntityPlayer)var1.next();
                } while (var2.isPlayerFullyAsleep());

                return false;
            }
            else
            {
                return false;
            }
        }

        public float func_27166_f(float var1)
        {
            return (prevThunderingStrength + (thunderingStrength - prevThunderingStrength) * var1) * func_27162_g(var1);
        }

        public float func_27162_g(float var1)
        {
            return prevRainingStrength + (rainingStrength - prevRainingStrength) * var1;
        }

        public void func_27158_h(float var1)
        {
            prevRainingStrength = var1;
            rainingStrength = var1;
        }

        public bool func_27160_B()
        {
            return (double)func_27166_f(1.0F) > 0.9D;
        }

        public bool func_27161_C()
        {
            return (double)func_27162_g(1.0F) > 0.2D;
        }

        public bool canBlockBeRainedOn(int var1, int var2, int var3)
        {
            if (!func_27161_C())
            {
                return false;
            }
            else if (!canBlockSeeTheSky(var1, var2, var3))
            {
                return false;
            }
            else if (findTopSolidBlock(var1, var3) > var2)
            {
                return false;
            }
            else
            {
                Biome var4 = getBiomeSource().getBiome(var1, var3);
                return var4.getEnableSnow() ? false : var4.canSpawnLightningBolt();
            }
        }

        public void setItemData(string var1, MapDataBase var2)
        {
            field_28108_z.setData(var1, var2);
        }

        public MapDataBase loadItemData(Class var1, string var2)
        {
            return field_28108_z.loadData(var1, new(var2));
        }

        public int getUniqueDataId(string var1)
        {
            return field_28108_z.getUniqueDataId(var1);
        }

        public void func_28106_e(int var1, int var2, int var3, int var4, int var5)
        {
            func_28107_a((EntityPlayer)null, var1, var2, var3, var4, var5);
        }

        public void func_28107_a(EntityPlayer var1, int var2, int var3, int var4, int var5, int var6)
        {
            for (int var7 = 0; var7 < worldAccesses.Count; ++var7)
            {
                worldAccesses[var7].func_28136_a(var1, var2, var3, var4, var5, var6);
            }

        }
    }

}