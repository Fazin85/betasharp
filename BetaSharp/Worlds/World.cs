using System.Runtime.InteropServices;
using BetaSharp.Blocks;
using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.NBT;
using BetaSharp.Profiling;
using BetaSharp.Rules;
using BetaSharp.Util.Hit;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Biomes;
using BetaSharp.Worlds.Biomes.Source;
using BetaSharp.Worlds.Chunks;
using BetaSharp.Worlds.Chunks.Light;
using BetaSharp.Worlds.Dimensions;
using BetaSharp.Worlds.Storage;
using java.lang;
using java.util;
using Silk.NET.Maths;

namespace BetaSharp.Worlds;

public abstract class World : java.lang.Object, BlockView
{
    public static readonly Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(World).TypeHandle);
    private const int AUTOSAVE_PERIOD = 40;
    public bool instantBlockUpdateEnabled;
    private readonly List<LightUpdate> lightingQueue;
    public List<Entity> entities;
    private readonly List<Entity> entitiesToUnload;
    private readonly TreeSet scheduledUpdates;
    private readonly Set scheduledUpdateSet;
    public List<BlockEntity> blockEntities;
    private readonly List<BlockEntity> blockEntityUpdateQueue;
    public List<EntityPlayer> players;
    public List globalEntities;
    private readonly long worldTimeMask;
    public int ambientDarkness;
    protected int lcgBlockSeed;
    protected readonly int lcgBlockSeedIncrement;
    protected float prevRainingStrength;
    protected float rainingStrength;
    protected float prevThunderingStrength;
    protected float thunderingStrength;
    protected int ticksSinceLightning;
    public int lightningTicksLeft;
    public bool pauseTicking;
    private readonly long lockTimestamp;
    protected int autosavePeriod;
    public int difficulty;
    public JavaRandom random;
    public bool isNewWorld;
    public readonly Dimension dimension;
    protected List<IWorldAccess> eventListeners;
    protected ChunkSource chunkSource;
    protected readonly WorldStorage storage;
    protected WorldProperties properties;
    public bool eventProcessingEnabled;
    private bool allPlayersSleeping;
    public PersistentStateManager persistentStateManager;
    private readonly List<Box> collidingBoundingBoxes;
    private bool processingDeferred;
    private int lightingUpdatesCounter;
    private bool spawnHostileMobs;
    private bool spawnPeacefulMobs;
    private int lightingUpdatesScheduled;
    private readonly HashSet<ChunkPos> activeChunks;
    private int soundCounter;
    private readonly List<Entity> tempEntityList;
    public bool isRemote;
    public RuleSet Rules { get; protected set; }

    public BiomeSource getBiomeSource()
    {
        return dimension.biomeSource;
    }

    public WorldStorage getWorldStorage()
    {
        return storage;
    }


    public World(WorldStorage var1, string var2, Dimension var3, long var4)
    {
        instantBlockUpdateEnabled = false;
        lightingQueue = [];
        entities = [];
        entitiesToUnload = [];
        scheduledUpdates = new TreeSet();
        scheduledUpdateSet = new HashSet();
        blockEntities = [];
        blockEntityUpdateQueue = [];
        players = [];
        globalEntities = new ArrayList();
        worldTimeMask = 0xFFFFFFL;
        ambientDarkness = 0;
        lcgBlockSeed = (new JavaRandom()).NextInt();
        lcgBlockSeedIncrement = 1013904223;
        ticksSinceLightning = 0;
        lightningTicksLeft = 0;
        pauseTicking = false;
        lockTimestamp = java.lang.System.currentTimeMillis();
        autosavePeriod = AUTOSAVE_PERIOD;
        random = new();
        isNewWorld = false;
        eventListeners = [];
        collidingBoundingBoxes = [];
        lightingUpdatesCounter = 0;
        spawnHostileMobs = true;
        spawnPeacefulMobs = true;
        activeChunks = new HashSet<ChunkPos>();
        soundCounter = random.NextInt(12000);
        tempEntityList = [];
        isRemote = false;
        storage = var1;
        properties = new WorldProperties(var4, var2);
        dimension = var3;
        persistentStateManager = new PersistentStateManager(var1);
        var3.setWorld(this);
        chunkSource = CreateChunkCache();
        Rules = properties.RulesTag != null
            ? RuleSet.FromNBT(RuleRegistry.Instance, properties.RulesTag)
            : new RuleSet(RuleRegistry.Instance);
        updateSkyBrightness();
        prepareWeather();
    }

    public World(World var1, Dimension var2)
    {
        instantBlockUpdateEnabled = false;
        lightingQueue = [];
        entities = [];
        entitiesToUnload = [];
        scheduledUpdates = new TreeSet();
        scheduledUpdateSet = new HashSet();
        blockEntities = [];
        blockEntityUpdateQueue = [];
        players = [];
        globalEntities = new ArrayList();
        worldTimeMask = 0xFFFFFFL;
        ambientDarkness = 0;
        lcgBlockSeed = (new JavaRandom()).NextInt();
        lcgBlockSeedIncrement = 1013904223;
        ticksSinceLightning = 0;
        lightningTicksLeft = 0;
        pauseTicking = false;
        lockTimestamp = java.lang.System.currentTimeMillis();
        autosavePeriod = AUTOSAVE_PERIOD;
        random = new();
        isNewWorld = false;
        eventListeners = [];
        collidingBoundingBoxes = [];
        lightingUpdatesCounter = 0;
        spawnHostileMobs = true;
        spawnPeacefulMobs = true;
        activeChunks = new HashSet<ChunkPos>();
        soundCounter = random.NextInt(12000);
        tempEntityList = [];
        isRemote = false;
        lockTimestamp = var1.lockTimestamp;
        storage = var1.storage;
        properties = new WorldProperties(var1.properties);
        persistentStateManager = new PersistentStateManager(storage);
        dimension = var2;
        var2.setWorld(this);
        chunkSource = CreateChunkCache();
        Rules = properties.RulesTag != null
            ? RuleSet.FromNBT(RuleRegistry.Instance, properties.RulesTag)
            : new RuleSet(RuleRegistry.Instance);
        updateSkyBrightness();
        prepareWeather();
    }

    public World(WorldStorage var1, string var2, long var3) : this(var1, var2, var3, null)
    {
    }

    public World(WorldStorage var1, string var2, long var3, Dimension var5)
    {
        instantBlockUpdateEnabled = false;
        lightingQueue = [];
        entities = [];
        entitiesToUnload = [];
        scheduledUpdates = new TreeSet();
        scheduledUpdateSet = new HashSet();
        blockEntities = [];
        blockEntityUpdateQueue = [];
        players = [];
        globalEntities = new ArrayList();
        worldTimeMask = 0xFFFFFFL;
        ambientDarkness = 0;
        lcgBlockSeed = (new JavaRandom()).NextInt();
        lcgBlockSeedIncrement = 1013904223;
        ticksSinceLightning = 0;
        lightningTicksLeft = 0;
        pauseTicking = false;
        lockTimestamp = java.lang.System.currentTimeMillis();
        autosavePeriod = AUTOSAVE_PERIOD;
        random = new JavaRandom();
        isNewWorld = false;
        eventListeners = [];
        collidingBoundingBoxes = [];
        lightingUpdatesCounter = 0;
        spawnHostileMobs = true;
        spawnPeacefulMobs = true;
        activeChunks = new HashSet<ChunkPos>();
        soundCounter = random.NextInt(12000);
        tempEntityList = [];
        isRemote = false;
        storage = var1;
        persistentStateManager = new PersistentStateManager(var1);
        properties = var1.loadProperties();
        isNewWorld = properties == null;
        if (var5 != null)
        {
            dimension = var5;
        }
        else if (properties != null && properties.Dimension == -1)
        {
            dimension = Dimension.fromId(-1);
        }
        else
        {
            dimension = Dimension.fromId(0);
        }

        bool var6 = false;
        if (properties == null)
        {
            properties = new WorldProperties(var3, var2);
            var6 = true;
        }
        else
        {
            properties.LevelName = var2;
        }

        dimension.setWorld(this);
        chunkSource = CreateChunkCache();
        Rules = properties.RulesTag != null
            ? RuleSet.FromNBT(RuleRegistry.Instance, properties.RulesTag)
            : new RuleSet(RuleRegistry.Instance);

        if (var6)
        {
            initializeSpawnPoint();
        }

        updateSkyBrightness();
        prepareWeather();
    }

    protected abstract ChunkSource CreateChunkCache();

    protected void initializeSpawnPoint()
    {
        eventProcessingEnabled = true;
        int var1 = 0;
        byte var2 = 64;

        int var3;
        for (var3 = 0; !dimension.isValidSpawnPoint(var1, var3); var3 += random.NextInt(64) - random.NextInt(64))
        {
            var1 += random.NextInt(64) - random.NextInt(64);
        }

        properties.SetSpawn(var1, var2, var3);
        eventProcessingEnabled = false;
    }

    public virtual void UpdateSpawnPosition()
    {
        if (properties.SpawnY <= 0)
        {
            properties.SpawnY = 64;
        }

        int var1 = properties.SpawnX;

        int var2;
        for (var2 = properties.SpawnZ; getSpawnBlockId(var1, var2) == 0; var2 += random.NextInt(8) - random.NextInt(8))
        {
            var1 += random.NextInt(8) - random.NextInt(8);
        }

        properties.SpawnX = var1;
        properties.SpawnZ = var2;
    }

    public int getSpawnBlockId(int var1, int var2)
    {
        int var3;
        for (var3 = 63; !isAir(var1, var3 + 1, var2); ++var3)
        {
        }

        return getBlockId(var1, var3, var2);
    }

    public void saveWorldData()
    {
    }

    public void addPlayer(EntityPlayer player)
    {
        try
        {
            NBTTagCompound? var2 = properties.PlayerTag;
            if (var2 != null)
            {
                player.read(var2);
                properties.PlayerTag = null;
            }

            SpawnEntity(player);
        }
        catch (java.lang.Exception ex)
        {
            ex.printStackTrace();
        }

    }

    public void saveWithLoadingDisplay(bool saveEntities, LoadingDisplay loadingDisplay)
    {
        if (chunkSource.canSave())
        {
            if (loadingDisplay != null)
            {
                loadingDisplay.progressStartNoAbort("Saving level");
            }

            Profiler.PushGroup("saveLevel");
            save();
            Profiler.PopGroup();
            if (loadingDisplay != null)
            {
                loadingDisplay.progressStage("Saving chunks");
            }

            Profiler.Start("saveChunks");
            chunkSource.save(saveEntities, loadingDisplay);
            Profiler.Stop("saveChunks");
        }
    }

    private void save()
    {
        Profiler.Start("checkSessionLock");
        //checkSessionLock();
        Profiler.Stop("checkSessionLock");
        Profiler.Start("saveWorldInfoAndPlayer");

        properties.RulesTag = new NBTTagCompound();
        Rules.WriteToNBT(properties.RulesTag);

        storage.save(properties, players.Cast<EntityPlayer>().ToList());
        Profiler.Stop("saveWorldInfoAndPlayer");

        Profiler.Start("saveAllData");
        persistentStateManager.saveAllData();
        Profiler.Stop("saveAllData");
    }

    public bool attemptSaving(int i)
    {
        if (!chunkSource.canSave())
        {
            return true;
        }
        else
        {
            if (i == 0)
            {
                save();
            }

            return chunkSource.save(false, (LoadingDisplay)null);
        }
    }

    public int getBlockId(int x, int y, int z)
    {
        return x >= -32000000 && z >= -32000000 && x < 32000000 && z <= 32000000 ? (y < 0 ? 0 : (y >= 128 ? 0 : getChunk(x >> 4, z >> 4).getBlockId(x & 15, y, z & 15))) : 0;
    }

    public bool isAir(int x, int y, int z)
    {
        return getBlockId(x, y, z) == 0;
    }

    public bool isPosLoaded(int x, int y, int z)
    {
        return y >= 0 && y < 128 ? hasChunk(x >> 4, z >> 4) : false;
    }

    public bool isRegionLoaded(int x, int y, int z, int range)
    {
        return isRegionLoaded(x - range, y - range, z - range, x + range, y + range, z + range);
    }

    public bool isRegionLoaded(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
    {
        if (maxY >= 0 && minY < 128)
        {
            minX >>= 4;
            minY >>= 4;
            minZ >>= 4;
            maxX >>= 4;
            maxY >>= 4;
            maxZ >>= 4;

            for (int var7 = minX; var7 <= maxX; ++var7)
            {
                for (int var8 = minZ; var8 <= maxZ; ++var8)
                {
                    if (!hasChunk(var7, var8))
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

    private bool hasChunk(int x, int z)
    {
        return chunkSource.isChunkLoaded(x, z);
    }

    public Chunk getChunkFromPos(int x, int z)
    {
        return getChunk(x >> 4, z >> 4);
    }

    public Chunk getChunk(int chunkX, int chunkZ)
    {
        return chunkSource.getChunk(chunkX, chunkZ);
    }

    public virtual bool SetBlockWithoutNotifyingNeighbors(int x, int y, int z, int blockId, int meta)
    {
        if (x >= -32000000 && z >= -32000000 && x < 32000000 && z <= 32000000)
        {
            if (y < 0)
            {
                return false;
            }
            else if (y >= 128)
            {
                return false;
            }
            else
            {
                Chunk var6 = getChunk(x >> 4, z >> 4);
                return var6.setBlock(x & 15, y, z & 15, blockId, meta);
            }
        }
        else
        {
            return false;
        }
    }

    public virtual bool SetBlockWithoutNotifyingNeighbors(int x, int y, int z, int blockId)
    {
        if (x >= -32000000 && z >= -32000000 && x < 32000000 && z <= 32000000)
        {
            if (y < 0)
            {
                return false;
            }
            else if (y >= 128)
            {
                return false;
            }
            else
            {
                Chunk var5 = getChunk(x >> 4, z >> 4);
                return var5.setBlock(x & 15, y, z & 15, blockId);
            }
        }
        else
        {
            return false;
        }
    }

    public Material getMaterial(int x, int y, int z)
    {
        int var4 = getBlockId(x, y, z);
        return var4 == 0 ? Material.Air : Block.Blocks[var4].material;
    }

    public int getBlockMeta(int x, int y, int z)
    {
        if (x >= -32000000 && z >= -32000000 && x < 32000000 && z <= 32000000)
        {
            if (y < 0)
            {
                return 0;
            }
            else if (y >= 128)
            {
                return 0;
            }
            else
            {
                Chunk var4 = getChunk(x >> 4, z >> 4);
                x &= 15;
                z &= 15;
                return var4.getBlockMeta(x, y, z);
            }
        }
        else
        {
            return 0;
        }
    }

    public void setBlockMeta(int x, int y, int z, int meta)
    {
        if (SetBlockMetaWithoutNotifyingNeighbors(x, y, z, meta))
        {
            int var5 = getBlockId(x, y, z);
            if (Block.BlocksIngoreMetaUpdate[var5 & 255])
            {
                blockUpdate(x, y, z, var5);
            }
            else
            {
                notifyNeighbors(x, y, z, var5);
            }
        }

    }

    public virtual bool SetBlockMetaWithoutNotifyingNeighbors(int x, int y, int z, int meta)
    {
        if (x >= -32000000 && z >= -32000000 && x < 32000000 && z <= 32000000)
        {
            if (y < 0)
            {
                return false;
            }
            else if (y >= 128)
            {
                return false;
            }
            else
            {
                Chunk var5 = getChunk(x >> 4, z >> 4);
                x &= 15;
                z &= 15;
                var5.setBlockMeta(x, y, z, meta);
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    public bool setBlock(int x, int y, int z, int blockId)
    {
        if (SetBlockWithoutNotifyingNeighbors(x, y, z, blockId))
        {
            blockUpdate(x, y, z, blockId);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool setBlock(int x, int y, int z, int blockId, int meta)
    {
        if (SetBlockWithoutNotifyingNeighbors(x, y, z, blockId, meta))
        {
            blockUpdate(x, y, z, blockId);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void blockUpdateEvent(int x, int y, int z)
    {
        for (int var4 = 0; var4 < eventListeners.Count; ++var4)
        {
            eventListeners[var4].blockUpdate(x, y, z);
        }

    }

    protected void blockUpdate(int x, int y, int z, int blockId)
    {
        blockUpdateEvent(x, y, z);
        notifyNeighbors(x, y, z, blockId);
    }

    public void setBlocksDirty(int x, int z, int minY, int maxY)
    {
        if (minY > maxY)
        {
            (maxY, minY) = (minY, maxY);
        }

        setBlocksDirty(x, minY, z, x, maxY, z);
    }

    public void setBlocksDirty(int x, int y, int z)
    {
        for (int var4 = 0; var4 < eventListeners.Count; ++var4)
        {
            eventListeners[var4].setBlocksDirty(x, y, z, x, y, z);
        }

    }

    public void setBlocksDirty(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
    {
        for (int var7 = 0; var7 < eventListeners.Count; ++var7)
        {
            eventListeners[var7].setBlocksDirty(minX, minY, minZ, maxX, maxY, maxZ);
        }

    }

    public void notifyNeighbors(int x, int y, int z, int blockId)
    {
        notifyUpdate(x - 1, y, z, blockId);
        notifyUpdate(x + 1, y, z, blockId);
        notifyUpdate(x, y - 1, z, blockId);
        notifyUpdate(x, y + 1, z, blockId);
        notifyUpdate(x, y, z - 1, blockId);
        notifyUpdate(x, y, z + 1, blockId);
    }

    private void notifyUpdate(int x, int y, int z, int blockId)
    {
        if (!pauseTicking && !isRemote)
        {
            Block var5 = Block.Blocks[getBlockId(x, y, z)];
            if (var5 != null)
            {
                var5.neighborUpdate(this, x, y, z, blockId);
            }

        }
    }

    public bool hasSkyLight(int x, int y, int z)
    {
        return getChunk(x >> 4, z >> 4).isAboveMaxHeight(x & 15, y, z & 15);
    }

    public int getBrightness(int x, int y, int z)
    {
        if (y < 0)
        {
            return 0;
        }
        else
        {
            if (y >= 128)
            {
                y = 127;
            }

            return getChunk(x >> 4, z >> 4).getLight(x & 15, y, z & 15, 0);
        }
    }

    public int getLightLevel(int x, int y, int z)
    {
        return getLightLevel(x, y, z, true);
    }

    public int getLightLevel(int x, int y, int z, bool bl)
    {
        if (x >= -32000000 && z >= -32000000 && x < 32000000 && z <= 32000000)
        {
            if (bl)
            {
                int var5 = getBlockId(x, y, z);
                if (var5 == Block.Slab.id || var5 == Block.Farmland.id || var5 == Block.CobblestoneStairs.id || var5 == Block.WoodenStairs.id)
                {
                    int var6 = getLightLevel(x, y + 1, z, false);
                    int var7 = getLightLevel(x + 1, y, z, false);
                    int var8 = getLightLevel(x - 1, y, z, false);
                    int var9 = getLightLevel(x, y, z + 1, false);
                    int var10 = getLightLevel(x, y, z - 1, false);
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

            if (y < 0)
            {
                return 0;
            }
            else
            {
                if (y >= 128)
                {
                    y = 127;
                }

                Chunk var11 = getChunk(x >> 4, z >> 4);
                x &= 15;
                z &= 15;
                return var11.getLight(x, y, z, ambientDarkness);
            }
        }
        else
        {
            return 15;
        }
    }

    public bool isTopY(int x, int y, int z)
    {
        if (x >= -32000000 && z >= -32000000 && x < 32000000 && z <= 32000000)
        {
            if (y < 0)
            {
                return false;
            }
            else if (y >= 128)
            {
                return true;
            }
            else if (!hasChunk(x >> 4, z >> 4))
            {
                return false;
            }
            else
            {
                Chunk var4 = getChunk(x >> 4, z >> 4);
                x &= 15;
                z &= 15;
                return var4.isAboveMaxHeight(x, y, z);
            }
        }
        else
        {
            return false;
        }
    }

    public int getTopY(int x, int z)
    {
        if (x >= -32000000 && z >= -32000000 && x < 32000000 && z <= 32000000)
        {
            if (!hasChunk(x >> 4, z >> 4))
            {
                return 0;
            }
            else
            {
                Chunk var3 = getChunk(x >> 4, z >> 4);
                return var3.getHeight(x & 15, z & 15);
            }
        }
        else
        {
            return 0;
        }
    }

    public void updateLight(LightType lightType, int x, int y, int z, int l)
    {
        if (!dimension.hasCeiling || lightType != LightType.Sky)
        {
            if (isPosLoaded(x, y, z))
            {
                if (lightType == LightType.Sky)
                {
                    if (isTopY(x, y, z))
                    {
                        l = 15;
                    }
                }
                else if (lightType == LightType.Block)
                {
                    int var6 = getBlockId(x, y, z);
                    if (Block.BlocksLightLuminance[var6] > l)
                    {
                        l = Block.BlocksLightLuminance[var6];
                    }
                }

                if (getBrightness(lightType, x, y, z) != l)
                {
                    queueLightUpdate(lightType, x, y, z, x, y, z);
                }

            }
        }
    }

    public int getBrightness(LightType type, int x, int y, int z)
    {
        if (y < 0)
        {
            y = 0;
        }

        if (y >= 128)
        {
            y = 127;
        }

        if (y >= 0 && y < 128 && x >= -32000000 && z >= -32000000 && x < 32000000 && z <= 32000000)
        {
            int var5 = x >> 4;
            int var6 = z >> 4;
            if (!hasChunk(var5, var6))
            {
                return 0;
            }
            else
            {
                Chunk var7 = getChunk(var5, var6);
                return var7.getLight(type, x & 15, y, z & 15);
            }
        }
        else
        {
            return type.lightValue;
        }
    }

    public void setLight(LightType lightType, int x, int y, int z, int value)
    {
        if (x >= -32000000 && z >= -32000000 && x < 32000000 && z <= 32000000)
        {
            if (y >= 0)
            {
                if (y < 128)
                {
                    if (hasChunk(x >> 4, z >> 4))
                    {
                        Chunk var6 = getChunk(x >> 4, z >> 4);
                        var6.setLight(lightType, x & 15, y, z & 15, value);

                        for (int var7 = 0; var7 < eventListeners.Count; ++var7)
                        {
                            eventListeners[var7].blockUpdate(x, y, z);
                        }

                    }
                }
            }
        }
    }

    public float getNaturalBrightness(int x, int y, int z, int blockLight)
    {
        int var5 = getLightLevel(x, y, z);
        if (var5 < blockLight)
        {
            var5 = blockLight;
        }

        return dimension.lightLevelToLuminance[var5];
    }

    public float getLuminance(int x, int y, int z)
    {
        return dimension.lightLevelToLuminance[getLightLevel(x, y, z)];
    }

    public bool canMonsterSpawn()
    {
        return ambientDarkness < 4;
    }

    public HitResult raycast(Vec3D start, Vec3D end)
    {
        return raycast(start, end, false, false);
    }

    public HitResult raycast(Vec3D start, Vec3D end, bool bl)
    {
        return raycast(start, end, bl, false);
    }

    public HitResult raycast(Vec3D start, Vec3D pos, bool bl, bool bl2)
    {
        if (!java.lang.Double.isNaN(start.x) && !java.lang.Double.isNaN(start.y) && !java.lang.Double.isNaN(start.z))
        {
            if (!java.lang.Double.isNaN(pos.x) && !java.lang.Double.isNaN(pos.y) && !java.lang.Double.isNaN(pos.z))
            {
                int var5 = MathHelper.Floor(pos.x);
                int var6 = MathHelper.Floor(pos.y);
                int var7 = MathHelper.Floor(pos.z);
                int var8 = MathHelper.Floor(start.x);
                int var9 = MathHelper.Floor(start.y);
                int var10 = MathHelper.Floor(start.z);
                int var11 = getBlockId(var8, var9, var10);
                int var12 = getBlockMeta(var8, var9, var10);
                Block var13 = Block.Blocks[var11];
                if ((!bl2 || var13 == null || var13.getCollisionShape(this, var8, var9, var10) != null) && var11 > 0 && var13.hasCollision(var12, bl))
                {
                    HitResult var14 = var13.raycast(this, var8, var9, var10, start, pos);
                    if (var14 != null)
                    {
                        return var14;
                    }
                }

                var11 = 200;

                while (var11-- >= 0)
                {
                    if (java.lang.Double.isNaN(start.x) || java.lang.Double.isNaN(start.y) || java.lang.Double.isNaN(start.z))
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
                    double var27 = pos.x - start.x;
                    double var29 = pos.y - start.y;
                    double var31 = pos.z - start.z;
                    if (var39)
                    {
                        var21 = (var15 - start.x) / var27;
                    }

                    if (var40)
                    {
                        var23 = (var17 - start.y) / var29;
                    }

                    if (var41)
                    {
                        var25 = (var19 - start.z) / var31;
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

                        start.x = var15;
                        start.y += var29 * var21;
                        start.z += var31 * var21;
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

                        start.x += var27 * var23;
                        start.y = var17;
                        start.z += var31 * var23;
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

                        start.x += var27 * var25;
                        start.y += var29 * var25;
                        start.z = var19;
                    }

                    Vec3D var34 = new Vec3D(start.x, start.y, start.z);
                    var8 = (int)(var34.x = (double)MathHelper.Floor(start.x));
                    if (var42 == 5)
                    {
                        --var8;
                        ++var34.x;
                    }

                    var9 = (int)(var34.y = (double)MathHelper.Floor(start.y));
                    if (var42 == 1)
                    {
                        --var9;
                        ++var34.y;
                    }

                    var10 = (int)(var34.z = (double)MathHelper.Floor(start.z));
                    if (var42 == 3)
                    {
                        --var10;
                        ++var34.z;
                    }

                    int var35 = getBlockId(var8, var9, var10);
                    int var36 = getBlockMeta(var8, var9, var10);
                    Block var37 = Block.Blocks[var35];
                    if ((!bl2 || var37 == null || var37.getCollisionShape(this, var8, var9, var10) != null) && var35 > 0 && var37.hasCollision(var36, bl))
                    {
                        HitResult var38 = var37.raycast(this, var8, var9, var10, start, pos);
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

    public void playSound(Entity entity, string sound, float volume, float pitch)
    {
        for (int var5 = 0; var5 < eventListeners.Count; ++var5)
        {
            eventListeners[var5].playSound(sound, entity.x, entity.y - (double)entity.standingEyeHeight, entity.z, volume, pitch);
        }

    }

    public void playSound(double x, double y, double z, string sound, float volume, float pitch)
    {
        for (int var10 = 0; var10 < eventListeners.Count; ++var10)
        {
            eventListeners[var10].playSound(sound, x, y, z, volume, pitch);
        }

    }

    public void playStreaming(string music, int x, int y, int z)
    {
        for (int var5 = 0; var5 < eventListeners.Count; ++var5)
        {
            eventListeners[var5].playStreaming(music, x, y, z);
        }

    }

    public void addParticle(string particle, double x, double y, double z, double velocityX, double velocityY, double velocityZ)
    {
        for (int var14 = 0; var14 < eventListeners.Count; ++var14)
        {
            eventListeners[var14].spawnParticle(particle, x, y, z, velocityX, velocityY, velocityZ);
        }

    }

    public virtual bool spawnGlobalEntity(Entity entity)
    {
        globalEntities.add(entity);
        return true;
    }

    public virtual bool SpawnEntity(Entity entity)
    {
        int var2 = MathHelper.Floor(entity.x / 16.0D);
        int var3 = MathHelper.Floor(entity.z / 16.0D);
        bool var4 = false;
        if (entity is EntityPlayer)
        {
            var4 = true;
        }

        if (!var4 && !hasChunk(var2, var3))
        {
            return false;
        }
        else
        {
            if (entity is EntityPlayer)
            {
                EntityPlayer var5 = (EntityPlayer)entity;
                players.Add(var5);
                updateSleepingPlayers();
            }

            getChunk(var2, var3).addEntity(entity);
            entities.Add(entity);
            NotifyEntityAdded(entity);
            return true;
        }
    }

    protected virtual void NotifyEntityAdded(Entity entity)
    {
        for (int var2 = 0; var2 < eventListeners.Count; ++var2)
        {
            eventListeners[var2].notifyEntityAdded(entity);
        }

    }

    protected virtual void NotifyEntityRemoved(Entity entity)
    {
        for (int var2 = 0; var2 < eventListeners.Count; ++var2)
        {
            eventListeners[var2].notifyEntityRemoved(entity);
        }

    }

    public virtual void Remove(Entity entity)
    {
        if (entity.passenger != null)
        {
            entity.passenger.setVehicle((Entity)null);
        }

        if (entity.vehicle != null)
        {
            entity.setVehicle((Entity)null);
        }

        entity.markDead();
        if (entity is EntityPlayer)
        {
            players.Remove((EntityPlayer)entity);
            updateSleepingPlayers();
        }

    }

    public void serverRemove(Entity entity)
    {
        entity.markDead();
        if (entity is EntityPlayer player)
        {
            players.Remove(player);
            this.updateSleepingPlayers();
        }

        int var2 = entity.chunkX;
        int var3 = entity.chunkZ;
        if (entity.isPersistent && hasChunk(var2, var3))
        {
            getChunk(var2, var3).removeEntity(entity);
        }

        entities.Remove(entity);
        NotifyEntityRemoved(entity);
    }

    public void addWorldAccess(IWorldAccess worldAccess)
    {
        eventListeners.Add(worldAccess);
    }

    public void removeWorldAccess(IWorldAccess worldAccess)
    {
        eventListeners.Remove(worldAccess);
    }

    public List<Box> getEntityCollisions(Entity entity, Box box)
    {
        collidingBoundingBoxes.Clear();
        int var3 = MathHelper.Floor(box.minX);
        int var4 = MathHelper.Floor(box.maxX + 1.0D);
        int var5 = MathHelper.Floor(box.minY);
        int var6 = MathHelper.Floor(box.maxY + 1.0D);
        int var7 = MathHelper.Floor(box.minZ);
        int var8 = MathHelper.Floor(box.maxZ + 1.0D);

        for (int var9 = var3; var9 < var4; ++var9)
        {
            for (int var10 = var7; var10 < var8; ++var10)
            {
                if (isPosLoaded(var9, 64, var10))
                {
                    for (int var11 = var5 - 1; var11 < var6; ++var11)
                    {
                        Block var12 = Block.Blocks[getBlockId(var9, var11, var10)];
                        if (var12 != null)
                        {
                            var12.addIntersectingBoundingBox(this, var9, var11, var10, box, collidingBoundingBoxes);
                        }
                    }
                }
            }
        }

        double var14 = 0.25D;
        List<Entity> var15 = getEntities(entity, box.expand(var14, var14, var14));

        for (int var16 = 0; var16 < var15.Count; ++var16)
        {
            Box? var13 = var15[var16].getBoundingBox();
            if (var13 != null && var13.Value.intersects(box))
            {
                collidingBoundingBoxes.Add(var13.Value);
            }

            var13 = entity.getCollisionAgainstShape(var15[var16]);
            if (var13 != null && var13.Value.intersects(box))
            {
                collidingBoundingBoxes.Add(var13.Value);
            }
        }

        return collidingBoundingBoxes;
    }

    public int getAmbientDarkness(float partialTicks)
    {
        float var2 = getTime(partialTicks);
        float var3 = 1.0F - (MathHelper.Cos(var2 * (float)java.lang.Math.PI * 2.0F) * 2.0F + 0.5F);
        if (var3 < 0.0F)
        {
            var3 = 0.0F;
        }

        if (var3 > 1.0F)
        {
            var3 = 1.0F;
        }

        var3 = 1.0F - var3;
        var3 = (float)((double)var3 * (1.0D - (double)(getRainGradient(partialTicks) * 5.0F) / 16.0D));
        var3 = (float)((double)var3 * (1.0D - (double)(getThunderGradient(partialTicks) * 5.0F) / 16.0D));
        var3 = 1.0F - var3;
        return (int)(var3 * 11.0F);
    }

    public Vector3D<double> getSkyColor(Entity entity, float partialTicks)
    {
        float var3 = getTime(partialTicks);
        float var4 = MathHelper.Cos(var3 * (float)java.lang.Math.PI * 2.0F) * 2.0F + 0.5F;
        if (var4 < 0.0F)
        {
            var4 = 0.0F;
        }

        if (var4 > 1.0F)
        {
            var4 = 1.0F;
        }

        int var5 = MathHelper.Floor(entity.x);
        int var6 = MathHelper.Floor(entity.z);
        float var7 = (float)getBiomeSource().GetTemperature(var5, var6);
        int var8 = getBiomeSource().GetBiome(var5, var6).GetSkyColorByTemp(var7);
        float var9 = (float)(var8 >> 16 & 255) / 255.0F;
        float var10 = (float)(var8 >> 8 & 255) / 255.0F;
        float var11 = (float)(var8 & 255) / 255.0F;
        var9 *= var4;
        var10 *= var4;
        var11 *= var4;
        float var12 = getRainGradient(partialTicks);
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

        var13 = getThunderGradient(partialTicks);
        if (var13 > 0.0F)
        {
            var14 = (var9 * 0.3F + var10 * 0.59F + var11 * 0.11F) * 0.2F;
            float var15 = 1.0F - var13 * (12.0F / 16.0F);
            var9 = var9 * var15 + var14 * (1.0F - var15);
            var10 = var10 * var15 + var14 * (1.0F - var15);
            var11 = var11 * var15 + var14 * (1.0F - var15);
        }

        if (lightningTicksLeft > 0)
        {
            var14 = (float)lightningTicksLeft - partialTicks;
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

    public float getTime(float var1)
    {
        return dimension.getTimeOfDay(properties.WorldTime, var1);
    }

    public Vector3D<double> getCloudColor(float partialTicks)
    {
        float var2 = getTime(partialTicks);
        float var3 = MathHelper.Cos(var2 * (float)java.lang.Math.PI * 2.0F) * 2.0F + 0.5F;
        if (var3 < 0.0F)
        {
            var3 = 0.0F;
        }

        if (var3 > 1.0F)
        {
            var3 = 1.0F;
        }

        float var4 = (float)(worldTimeMask >> 16 & 255L) / 255.0F;
        float var5 = (float)(worldTimeMask >> 8 & 255L) / 255.0F;
        float var6 = (float)(worldTimeMask & 255L) / 255.0F;
        float var7 = getRainGradient(partialTicks);
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
        var8 = getThunderGradient(partialTicks);
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
        float var2 = getTime(var1);
        return dimension.getFogColor(var2, var1);
    }

    public int getTopSolidBlockY(int x, int z)
    {
        Chunk var3 = getChunkFromPos(x, z);
        int var4 = 127;
        x &= 15;

        for (z &= 15; var4 > 0; --var4)
        {
            int var5 = var3.getBlockId(x, var4, z);
            Material var6 = var5 == 0 ? Material.Air : Block.Blocks[var5].material;
            if (var6.BlocksMovement || var6.IsFluid)
            {
                return var4 + 1;
            }
        }

        return -1;
    }

    public float calcualteSkyLightIntensity(float partialTicks)
    {
        float var2 = getTime(partialTicks);
        float var3 = 1.0F - (MathHelper.Cos(var2 * (float)java.lang.Math.PI * 2.0F) * 2.0F + 12.0F / 16.0F);
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

    public int getSpawnPositionValidityY(int x, int z)
    {
        Chunk var3 = getChunkFromPos(x, z);
        int var4 = 127;
        x &= 15;

        for (int var7 = z & 15; var4 > 0; var4--)
        {
            int var5 = var3.getBlockId(x, var4, var7);
            if (var5 != 0 && Block.Blocks[var5].material.BlocksMovement)
            {
                return var4 + 1;
            }
        }

        return -1;
    }

    public virtual void ScheduleBlockUpdate(int x, int y, int z, int id, int tickRate)
    {
        BlockEvent var6 = new(x, y, z, id);
        byte var7 = 8;
        if (instantBlockUpdateEnabled)
        {
            if (isRegionLoaded(var6.x - var7, var6.y - var7, var6.z - var7, var6.x + var7, var6.y + var7, var6.z + var7))
            {
                int var8 = getBlockId(var6.x, var6.y, var6.z);
                if (var8 == var6.blockId && var8 > 0)
                {
                    Block.Blocks[var8].onTick(this, var6.x, var6.y, var6.z, random);
                }
            }

        }
        else
        {
            if (isRegionLoaded(x - var7, y - var7, z - var7, x + var7, y + var7, z + var7))
            {
                if (id > 0)
                {
                    var6.setScheduledTime((long)tickRate + properties.WorldTime);
                }

                if (!scheduledUpdateSet.contains(var6))
                {
                    scheduledUpdateSet.add(var6);
                    scheduledUpdates.add(var6);
                }
            }

        }
    }

    public void tickEntities()
    {
        Profiler.Start("updateEntites.updateWeatherEffects");

        int var1;
        Entity var2;
        for (var1 = 0; var1 < globalEntities.size(); ++var1)
        {
            var2 = (Entity)globalEntities.get(var1);
            var2.tick();
            if (var2.dead)
            {
                globalEntities.remove(var1--);
            }
        }
        Profiler.Stop("updateEntites.updateWeatherEffects");

        foreach (var entity in entitiesToUnload)
        {
            entities.Remove(entity);
        }

        Profiler.Start("updateEntites.clearUnloadedEntities");

        int var3;
        int var4;
        for (var1 = 0; var1 < entitiesToUnload.Count; ++var1)
        {
            var2 = entitiesToUnload[var1];
            var3 = var2.chunkX;
            var4 = var2.chunkZ;
            if (var2.isPersistent && hasChunk(var3, var4))
            {
                getChunk(var3, var4).removeEntity(var2);
            }
        }

        for (var1 = 0; var1 < entitiesToUnload.Count; ++var1)
        {
            NotifyEntityRemoved(entitiesToUnload[var1]);
        }

        entitiesToUnload.Clear();

        Profiler.Stop("updateEntites.clearUnloadedEntities");

        Profiler.Start("updateEntites.updateLoadedEntities");

        for (var1 = 0; var1 < entities.Count; ++var1)
        {
            var2 = entities[var1];
            if (var2.vehicle != null)
            {
                if (!var2.vehicle.dead && var2.vehicle.passenger == var2)
                {
                    continue;
                }

                var2.vehicle.passenger = null;
                var2.vehicle = null;
            }

            if (!var2.dead)
            {
                updateEntity(var2);
            }

            if (var2.dead)
            {
                var3 = var2.chunkX;
                var4 = var2.chunkZ;
                if (var2.isPersistent && hasChunk(var3, var4))
                {
                    getChunk(var3, var4).removeEntity(var2);
                }

                entities.RemoveAt(var1--);
                NotifyEntityRemoved(var2);
            }
        }
        Profiler.Stop("updateEntites.updateLoadedEntities");

        processingDeferred = true;

        Profiler.Start("updateEntites.updateLoadedTileEntities");

        for (int i = blockEntities.Count - 1; i >= 0; i--)
        {
            BlockEntity var5 = blockEntities[i];
            if (!var5.isRemoved())
            {
                var5.tick();
            }
            if (var5.isRemoved())
            {
                blockEntities.RemoveAt(i);
                Chunk var7 = getChunk(var5.x >> 4, var5.z >> 4);
                if (var7 != null)
                {
                    var7.removeBlockEntityAt(var5.x & 15, var5.y, var5.z & 15);
                }
            }
        }

        processingDeferred = false;
        if (blockEntityUpdateQueue.Count > 0)
        {
            foreach (BlockEntity var8 in blockEntityUpdateQueue)
            {
                if (!var8.isRemoved())
                {
                    if (!blockEntities.Contains(var8))
                    {
                        blockEntities.Add(var8);
                    }
                    Chunk var9 = getChunk(var8.x >> 4, var8.z >> 4);
                    if (var9 != null)
                    {
                        var9.setBlockEntity(var8.x & 15, var8.y, var8.z & 15, var8);
                    }
                    blockUpdateEvent(var8.x, var8.y, var8.z);
                }
            }
            blockEntityUpdateQueue.Clear();
        }
        Profiler.Stop("updateEntites.updateLoadedTileEntities");

    }

    public void processBlockUpdates(IEnumerable<BlockEntity> blockUpdates)
    {
        if (processingDeferred)
        {
            blockEntityUpdateQueue.AddRange(blockUpdates);
        }
        else
        {
            blockEntities.AddRange(blockUpdates);
        }

    }

    public void updateEntity(Entity entity)
    {
        updateEntity(entity, true);
    }

    public virtual void updateEntity(Entity entity, bool requireLoaded)
    {
        int var3 = MathHelper.Floor(entity.x);
        int var4 = MathHelper.Floor(entity.z);
        byte var5 = 32;
        if (!requireLoaded || isRegionLoaded(var3 - var5, 0, var4 - var5, var3 + var5, 128, var4 + var5))
        {
            entity.lastTickX = entity.x;
            entity.lastTickY = entity.y;
            entity.lastTickZ = entity.z;
            entity.prevYaw = entity.yaw;
            entity.prevPitch = entity.pitch;
            if (requireLoaded && entity.isPersistent)
            {
                if (entity.vehicle != null)
                {
                    entity.tickRiding();
                }
                else
                {
                    entity.tick();
                }
            }

            if (java.lang.Double.isNaN(entity.x) || java.lang.Double.isInfinite(entity.x))
            {
                entity.x = entity.lastTickX;
            }

            if (java.lang.Double.isNaN(entity.y) || java.lang.Double.isInfinite(entity.y))
            {
                entity.y = entity.lastTickY;
            }

            if (java.lang.Double.isNaN(entity.z) || java.lang.Double.isInfinite(entity.z))
            {
                entity.z = entity.lastTickZ;
            }

            if (java.lang.Double.isNaN((double)entity.pitch) || java.lang.Double.isInfinite((double)entity.pitch))
            {
                entity.pitch = entity.prevPitch;
            }

            if (java.lang.Double.isNaN((double)entity.yaw) || java.lang.Double.isInfinite((double)entity.yaw))
            {
                entity.yaw = entity.prevYaw;
            }

            int var6 = MathHelper.Floor(entity.x / 16.0D);
            int var7 = MathHelper.Floor(entity.y / 16.0D);
            int var8 = MathHelper.Floor(entity.z / 16.0D);
            if (!entity.isPersistent || entity.chunkX != var6 || entity.chunkSlice != var7 || entity.chunkZ != var8)
            {
                if (entity.isPersistent && hasChunk(entity.chunkX, entity.chunkZ))
                {
                    getChunk(entity.chunkX, entity.chunkZ).removeEntity(entity, entity.chunkSlice);
                }

                if (hasChunk(var6, var8))
                {
                    entity.isPersistent = true;
                    getChunk(var6, var8).addEntity(entity);
                }
                else
                {
                    entity.isPersistent = false;
                }
            }

            if (requireLoaded && entity.isPersistent && entity.passenger != null)
            {
                if (!entity.passenger.dead && entity.passenger.vehicle == entity)
                {
                    updateEntity(entity.passenger);
                }
                else
                {
                    entity.passenger.vehicle = null;
                    entity.passenger = null;
                }
            }

        }
    }

    public bool canSpawnEntity(Box box)
    {
        List<Entity> var2 = getEntities((Entity)null, box);

        for (int var3 = 0; var3 < var2.Count; ++var3)
        {
            Entity var4 = var2[var3];
            if (!var4.dead && var4.preventEntitySpawning)
            {
                return false;
            }
        }

        return true;
    }

    public bool isAnyBlockInBox(Box box)
    {
        int var2 = MathHelper.Floor(box.minX);
        int var3 = MathHelper.Floor(box.maxX + 1.0);
        int var4 = MathHelper.Floor(box.minY);
        int var5 = MathHelper.Floor(box.maxY + 1.0);
        int var6 = MathHelper.Floor(box.minZ);
        int var7 = MathHelper.Floor(box.maxZ + 1.0);
        if (box.minX < 0.0)
        {
            var2--;
        }

        if (box.minY < 0.0)
        {
            var4--;
        }

        if (box.minZ < 0.0)
        {
            var6--;
        }

        for (int var8 = var2; var8 < var3; var8++)
        {
            for (int var9 = var4; var9 < var5; var9++)
            {
                for (int var10 = var6; var10 < var7; var10++)
                {
                    Block var11 = Block.Blocks[getBlockId(var8, var9, var10)];
                    if (var11 != null)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool isBoxSubmergedInFluid(Box box)
    {
        int var2 = MathHelper.Floor(box.minX);
        int var3 = MathHelper.Floor(box.maxX + 1.0D);
        int var4 = MathHelper.Floor(box.minY);
        int var5 = MathHelper.Floor(box.maxY + 1.0D);
        int var6 = MathHelper.Floor(box.minZ);
        int var7 = MathHelper.Floor(box.maxZ + 1.0D);
        if (box.minX < 0.0D)
        {
            --var2;
        }

        if (box.minY < 0.0D)
        {
            --var4;
        }

        if (box.minZ < 0.0D)
        {
            --var6;
        }

        for (int var8 = var2; var8 < var3; ++var8)
        {
            for (int var9 = var4; var9 < var5; ++var9)
            {
                for (int var10 = var6; var10 < var7; ++var10)
                {
                    Block var11 = Block.Blocks[getBlockId(var8, var9, var10)];
                    if (var11 != null && var11.material.IsFluid)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool isFireOrLavaInBox(Box box)
    {
        int var2 = MathHelper.Floor(box.minX);
        int var3 = MathHelper.Floor(box.maxX + 1.0D);
        int var4 = MathHelper.Floor(box.minY);
        int var5 = MathHelper.Floor(box.maxY + 1.0D);
        int var6 = MathHelper.Floor(box.minZ);
        int var7 = MathHelper.Floor(box.maxZ + 1.0D);
        if (isRegionLoaded(var2, var4, var6, var3, var5, var7))
        {
            for (int var8 = var2; var8 < var3; ++var8)
            {
                for (int var9 = var4; var9 < var5; ++var9)
                {
                    for (int var10 = var6; var10 < var7; ++var10)
                    {
                        int var11 = getBlockId(var8, var9, var10);
                        if (var11 == Block.Fire.id || var11 == Block.FlowingLava.id || var11 == Block.Lava.id)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public bool updateMovementInFluid(Box entityBox, Material fluidMaterial, Entity entity)
    {
        int var4 = MathHelper.Floor(entityBox.minX);
        int var5 = MathHelper.Floor(entityBox.maxX + 1.0D);
        int var6 = MathHelper.Floor(entityBox.minY);
        int var7 = MathHelper.Floor(entityBox.maxY + 1.0D);
        int var8 = MathHelper.Floor(entityBox.minZ);
        int var9 = MathHelper.Floor(entityBox.maxZ + 1.0D);
        if (!isRegionLoaded(var4, var6, var8, var5, var7, var9))
        {
            return false;
        }
        else
        {
            bool var10 = false;
            Vec3D var11 = new Vec3D(0.0D, 0.0D, 0.0D);

            for (int var12 = var4; var12 < var5; ++var12)
            {
                for (int var13 = var6; var13 < var7; ++var13)
                {
                    for (int var14 = var8; var14 < var9; ++var14)
                    {
                        Block var15 = Block.Blocks[getBlockId(var12, var13, var14)];
                        if (var15 != null && var15.material == fluidMaterial)
                        {
                            double var16 = (double)((float)(var13 + 1) - BlockFluid.getFluidHeightFromMeta(getBlockMeta(var12, var13, var14)));
                            if ((double)var7 >= var16)
                            {
                                var10 = true;
                                var15.applyVelocity(this, var12, var13, var14, entity, var11);
                            }
                        }
                    }
                }
            }

            if (var11.magnitude() > 0.0D)
            {
                var11 = var11.normalize();
                double var18 = 0.014D;
                entity.velocityX += var11.x * var18;
                entity.velocityY += var11.y * var18;
                entity.velocityZ += var11.z * var18;
            }

            return var10;
        }
    }

    public bool isMaterialInBox(Box box, Material material)
    {
        int var3 = MathHelper.Floor(box.minX);
        int var4 = MathHelper.Floor(box.maxX + 1.0D);
        int var5 = MathHelper.Floor(box.minY);
        int var6 = MathHelper.Floor(box.maxY + 1.0D);
        int var7 = MathHelper.Floor(box.minZ);
        int var8 = MathHelper.Floor(box.maxZ + 1.0D);

        for (int var9 = var3; var9 < var4; ++var9)
        {
            for (int var10 = var5; var10 < var6; ++var10)
            {
                for (int var11 = var7; var11 < var8; ++var11)
                {
                    Block var12 = Block.Blocks[getBlockId(var9, var10, var11)];
                    if (var12 != null && var12.material == material)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool isFluidInBox(Box box, Material fluid)
    {
        int var3 = MathHelper.Floor(box.minX);
        int var4 = MathHelper.Floor(box.maxX + 1.0D);
        int var5 = MathHelper.Floor(box.minY);
        int var6 = MathHelper.Floor(box.maxY + 1.0D);
        int var7 = MathHelper.Floor(box.minZ);
        int var8 = MathHelper.Floor(box.maxZ + 1.0D);

        for (int var9 = var3; var9 < var4; ++var9)
        {
            for (int var10 = var5; var10 < var6; ++var10)
            {
                for (int var11 = var7; var11 < var8; ++var11)
                {
                    Block var12 = Block.Blocks[getBlockId(var9, var10, var11)];
                    if (var12 != null && var12.material == fluid)
                    {
                        int var13 = getBlockMeta(var9, var10, var11);
                        double var14 = (double)(var10 + 1);
                        if (var13 < 8)
                        {
                            var14 = (double)(var10 + 1) - (double)var13 / 8.0D;
                        }

                        if (var14 >= box.minY)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public Explosion createExplosion(Entity source, double x, double y, double z, float power)
    {
        return createExplosion(source, x, y, z, power, false);
    }

    public virtual Explosion createExplosion(Entity source, double x, double y, double z, float power, bool fire)
    {
        Explosion var10 = new(this, source, x, y, z, power);
        var10.isFlaming = fire;
        var10.doExplosionA();
        var10.doExplosionB(true);
        return var10;
    }

    public float getVisibilityRatio(Vec3D vec, Box box)
    {
        double var3 = 1.0D / ((box.maxX - box.minX) * 2.0D + 1.0D);
        double var5 = 1.0D / ((box.maxY - box.minY) * 2.0D + 1.0D);
        double var7 = 1.0D / ((box.maxZ - box.minZ) * 2.0D + 1.0D);
        int var9 = 0;
        int var10 = 0;

        for (float var11 = 0.0F; var11 <= 1.0F; var11 = (float)((double)var11 + var3))
        {
            for (float var12 = 0.0F; var12 <= 1.0F; var12 = (float)((double)var12 + var5))
            {
                for (float var13 = 0.0F; var13 <= 1.0F; var13 = (float)((double)var13 + var7))
                {
                    double var14 = box.minX + (box.maxX - box.minX) * (double)var11;
                    double var16 = box.minY + (box.maxY - box.minY) * (double)var12;
                    double var18 = box.minZ + (box.maxZ - box.minZ) * (double)var13;
                    if (raycast(new Vec3D(var14, var16, var18), vec) == null)
                    {
                        ++var9;
                    }

                    ++var10;
                }
            }
        }

        return (float)var9 / (float)var10;
    }

    public void extinguishFire(EntityPlayer player, int x, int y, int z, int direction)
    {
        if (direction == 0)
        {
            --y;
        }

        if (direction == 1)
        {
            ++y;
        }

        if (direction == 2)
        {
            --z;
        }

        if (direction == 3)
        {
            ++z;
        }

        if (direction == 4)
        {
            --x;
        }

        if (direction == 5)
        {
            ++x;
        }

        if (getBlockId(x, y, z) == Block.Fire.id)
        {
            worldEvent(player, 1004, x, y, z, 0);
            setBlock(x, y, z, 0);
        }

    }

    public Entity getPlayerForProxy(java.lang.Class var1)
    {
        return null;
    }

    public string getEntityCount()
    {
        return "All: " + entities.Count;
    }

    public string getDebugInfo()
    {
        return chunkSource.getDebugInfo();
    }

    public BlockEntity getBlockEntity(int x, int y, int z)
    {
        Chunk var4 = getChunk(x >> 4, z >> 4);
        return var4 != null ? var4.getBlockEntity(x & 15, y, z & 15) : null;
    }

    public void setBlockEntity(int x, int y, int z, BlockEntity blockEntity)
    {
        if (!blockEntity.isRemoved())
        {
            if (processingDeferred)
            {
                blockEntity.x = x;
                blockEntity.y = y;
                blockEntity.z = z;
                blockEntityUpdateQueue.Add(blockEntity);
            }
            else
            {
                blockEntities.Add(blockEntity);
                Chunk var5 = getChunk(x >> 4, z >> 4);
                if (var5 != null)
                {
                    var5.setBlockEntity(x & 15, y, z & 15, blockEntity);
                }
            }
        }

    }

    public void removeBlockEntity(int x, int y, int z)
    {
        BlockEntity var4 = getBlockEntity(x, y, z);
        if (var4 != null && processingDeferred)
        {
            var4.markRemoved();
        }
        else
        {
            if (var4 != null)
            {
                blockEntities.Remove(var4);
            }

            Chunk var5 = getChunk(x >> 4, z >> 4);
            if (var5 != null)
            {
                var5.removeBlockEntityAt(x & 15, y, z & 15);
            }
        }

    }

    public bool isOpaque(int x, int y, int z)
    {
        Block var4 = Block.Blocks[getBlockId(x, y, z)];
        return var4 == null ? false : var4.isOpaque();
    }

    public bool shouldSuffocate(int x, int y, int z)
    {
        Block var4 = Block.Blocks[getBlockId(x, y, z)];
        return var4 == null ? false : var4.material.Suffocates && var4.isFullCube();
    }

    public void savingProgress(LoadingDisplay display)
    {
        saveWithLoadingDisplay(true, display);
    }

    public bool doLightingUpdates()
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

                while (lightingQueue.Count > 0)
                {
                    --var1;
                    if (var1 <= 0)
                    {
                        var2 = true;
                        return var2;
                    }

                    int lastIndex = lightingQueue.Count - 1;
                    LightUpdate mcb = lightingQueue[lastIndex];

                    lightingQueue.RemoveAt(lastIndex);
                    mcb.updateLight(this);
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

    public void queueLightUpdate(LightType type, int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
    {
        queueLightUpdate(type, minX, minY, minZ, maxX, maxY, maxZ, true);
    }

    public void queueLightUpdate(LightType type, int minX, int minY, int minZ, int maxX, int maxY, int maxZ, bool bl)
    {
        if (!dimension.hasCeiling || type != LightType.Sky)
        {
            ++lightingUpdatesScheduled;

            try
            {
                if (lightingUpdatesScheduled == 50)
                {
                    return;
                }

                int var9 = (maxX + minX) / 2;
                int var10 = (maxZ + minZ) / 2;
                if (isPosLoaded(var9, 64, var10))
                {
                    if (getChunkFromPos(var9, var10).isEmpty())
                    {
                        return;
                    }

                    int var11 = lightingQueue.Count;
                    int var12;
                    var span = CollectionsMarshal.AsSpan(lightingQueue);

                    if (bl)
                    {
                        var12 = 5;
                        if (var12 > var11)
                        {
                            var12 = var11;
                        }

                        for (int var13 = 0; var13 < var12; ++var13)
                        {
                            ref LightUpdate var14 = ref span[lightingQueue.Count - var13 - 1];
                            if (var14.lightType == type && var14.expand(minX, minY, minZ, maxX, maxY, maxZ))
                            {
                                return;
                            }
                        }
                    }

                    lightingQueue.Add(new LightUpdate(type, minX, minY, minZ, maxX, maxY, maxZ));
                    var12 = 1000000;
                    if (lightingQueue.Count > 1000000)
                    {
                        Log.Info($"More than {var12} updates, aborting lighting updates");
                        lightingQueue.Clear();
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

    public void updateSkyBrightness()
    {
        int var1 = getAmbientDarkness(1.0F);
        if (var1 != ambientDarkness)
        {
            ambientDarkness = var1;
        }

    }

    public void allowSpawning(bool allowMonsterSpawning, bool allowMobSpawning)
    {
        spawnHostileMobs = allowMonsterSpawning;
        spawnPeacefulMobs = allowMobSpawning;
    }

    public virtual void Tick()
    {
        UpdateWeatherCycles();
        long var2;
        if (canSkipNight())
        {
            bool var1 = false;
            if (spawnHostileMobs && difficulty >= 1)
            {
                var1 = NaturalSpawner.spawnMonstersAndWakePlayers(this, players);
            }

            if (!var1)
            {
                var2 = properties.WorldTime + 24000L;
                properties.WorldTime = var2 - var2 % 24000L;
                afterSkipNight();
            }
        }
        Profiler.Start("performSpawning");
        NaturalSpawner.performSpawning(this, spawnHostileMobs, spawnPeacefulMobs);
        Profiler.Stop("performSpawning");
        Profiler.Start("unload100OldestChunks");
        chunkSource.tick();
        Profiler.Stop("unload100OldestChunks");

        Profiler.Start("updateSkylightSubtracted");
        int var4 = getAmbientDarkness(1.0F);
        if (var4 != ambientDarkness)
        {
            ambientDarkness = var4;

            for (int var5 = 0; var5 < eventListeners.Count; ++var5)
            {
                eventListeners[var5].notifyAmbientDarknessChanged();
            }
        }
        Profiler.Stop("updateSkylightSubtracted");

        var2 = properties.WorldTime + 1L;
        if (var2 % (long)autosavePeriod == 0L)
        {
            Profiler.PushGroup("autosave");
            saveWithLoadingDisplay(false, (LoadingDisplay)null);
            Profiler.PopGroup();
        }

        properties.WorldTime = var2;
        Profiler.Start("tickUpdates");
        ProcessScheduledTicks(false);
        Profiler.Stop("tickUpdates");
        ManageChunkUpdatesAndEvents();
    }

    private void prepareWeather()
    {
        if (properties.IsRaining)
        {
            rainingStrength = 1.0F;
            if (properties.IsThundering)
            {
                thunderingStrength = 1.0F;
            }
        }

    }

    protected virtual void UpdateWeatherCycles()
    {
        if (!dimension.hasCeiling)
        {
            if (ticksSinceLightning > 0)
            {
                --ticksSinceLightning;
            }

            int var1 = properties.ThunderTime;
            if (var1 <= 0)
            {
                if (properties.IsThundering)
                {
                    properties.ThunderTime = random.NextInt(12000) + 3600;
                }
                else
                {
                    properties.ThunderTime = random.NextInt(168000) + 12000;
                }
            }
            else
            {
                --var1;
                properties.ThunderTime = var1;
                if (var1 <= 0)
                {
                    properties.IsThundering = !properties.IsThundering;
                }
            }

            int var2 = properties.RainTime;
            if (var2 <= 0)
            {
                if (properties.IsRaining)
                {
                    properties.RainTime = random.NextInt(12000) + 12000;
                }
                else
                {
                    properties.RainTime = random.NextInt(168000) + 12000;
                }
            }
            else
            {
                --var2;
                properties.RainTime = var2;
                if (var2 <= 0)
                {
                    properties.IsRaining = !properties.IsRaining;
                }
            }

            prevRainingStrength = rainingStrength;
            if (properties.IsRaining)
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
            if (properties.IsThundering)
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

    private void clearWeather()
    {
        properties.RainTime = 0;
        properties.IsRaining = false;
        properties.ThunderTime = 0;
        properties.IsThundering = false;
    }

    protected virtual void ManageChunkUpdatesAndEvents()
    {
        activeChunks.Clear();
        int var3;
        int var4;
        int var6;
        int var7;
        for (int var1 = 0; var1 < players.Count; ++var1)
        {
            EntityPlayer var2 = players[var1];
            var3 = MathHelper.Floor(var2.x / 16.0D);
            var4 = MathHelper.Floor(var2.z / 16.0D);
            byte var5 = 9;

            for (var6 = -var5; var6 <= var5; ++var6)
            {
                for (var7 = -var5; var7 <= var5; ++var7)
                {
                    activeChunks.Add(new ChunkPos(var6 + var3, var7 + var4));
                }
            }
        }

        if (soundCounter > 0)
        {
            --soundCounter;
        }

        foreach (var p in activeChunks)
        {
            var3 = p.x * 16;
            var4 = p.z * 16;
            Chunk var14 = getChunk(p.x, p.z);
            int var8;
            int var9;
            int var10;
            if (soundCounter == 0)
            {
                lcgBlockSeed = lcgBlockSeed * 3 + 1013904223;
                var6 = lcgBlockSeed >> 2;
                var7 = var6 & 15;
                var8 = var6 >> 8 & 15;
                var9 = var6 >> 16 & 127;
                var10 = var14.getBlockId(var7, var9, var8);
                var7 += var3;
                var8 += var4;
                if (var10 == 0 && getBrightness(var7, var9, var8) <= random.NextInt(8) && getBrightness(LightType.Sky, var7, var9, var8) <= 0)
                {
                    EntityPlayer var11 = getClosestPlayer((double)var7 + 0.5D, (double)var9 + 0.5D, (double)var8 + 0.5D, 8.0D);
                    if (var11 != null && var11.getSquaredDistance((double)var7 + 0.5D, (double)var9 + 0.5D, (double)var8 + 0.5D) > 4.0D)
                    {
                        playSound((double)var7 + 0.5D, (double)var9 + 0.5D, (double)var8 + 0.5D, "ambient.cave.cave", 0.7F, 0.8F + random.NextFloat() * 0.2F);
                        soundCounter = random.NextInt(12000) + 6000;
                    }
                }
            }

            if (random.NextInt(100000) == 0 && isRaining() && isThundering())
            {
                lcgBlockSeed = lcgBlockSeed * 3 + 1013904223;
                var6 = lcgBlockSeed >> 2;
                var7 = var3 + (var6 & 15);
                var8 = var4 + (var6 >> 8 & 15);
                var9 = getTopSolidBlockY(var7, var8);
                if (isRaining(var7, var9, var8))
                {
                    spawnGlobalEntity(new EntityLightningBolt(this, (double)var7, (double)var9, (double)var8));
                    ticksSinceLightning = 2;
                }
            }

            int var15;
            if (random.NextInt(16) == 0)
            {
                lcgBlockSeed = lcgBlockSeed * 3 + 1013904223;
                var6 = lcgBlockSeed >> 2;
                var7 = var6 & 15;
                var8 = var6 >> 8 & 15;
                var9 = getTopSolidBlockY(var7 + var3, var8 + var4);
                if (getBiomeSource().GetBiome(var7 + var3, var8 + var4).GetEnableSnow() && var9 >= 0 && var9 < 128 && var14.getLight(LightType.Block, var7, var9, var8) < 10)
                {
                    var10 = var14.getBlockId(var7, var9 - 1, var8);
                    var15 = var14.getBlockId(var7, var9, var8);
                    if (isRaining() && var15 == 0 && Block.Snow.canPlaceAt(this, var7 + var3, var9, var8 + var4) && var10 != 0 && var10 != Block.Ice.id && Block.Blocks[var10].material.BlocksMovement)
                    {
                        setBlock(var7 + var3, var9, var8 + var4, Block.Snow.id);
                    }

                    if (var10 == Block.Water.id && var14.getBlockMeta(var7, var9 - 1, var8) == 0)
                    {
                        setBlock(var7 + var3, var9 - 1, var8 + var4, Block.Ice.id);
                    }
                }
            }

            for (var6 = 0; var6 < 80; ++var6)
            {
                lcgBlockSeed = lcgBlockSeed * 3 + 1013904223;
                var7 = lcgBlockSeed >> 2;
                var8 = var7 & 15;
                var9 = var7 >> 8 & 15;
                var10 = var7 >> 16 & 127;
                var15 = var14.blocks[var8 << 11 | var9 << 7 | var10] & 255;
                if (Block.BlocksRandomTick[var15])
                {
                    Block.Blocks[var15].onTick(this, var8 + var3, var10, var9 + var4, random);
                }
            }
        }

    }

    public virtual bool ProcessScheduledTicks(bool flush)
    {
        int var2 = scheduledUpdates.size();
        if (var2 != scheduledUpdateSet.size())
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
                BlockEvent var4 = (BlockEvent)scheduledUpdates.first();
                if (!flush && var4.ticks > properties.WorldTime)
                {
                    break;
                }

                scheduledUpdates.remove(var4);
                scheduledUpdateSet.remove(var4);
                byte var5 = 8;
                if (isRegionLoaded(var4.x - var5, var4.y - var5, var4.z - var5, var4.x + var5, var4.y + var5, var4.z + var5))
                {
                    int var6 = getBlockId(var4.x, var4.y, var4.z);
                    if (var6 == var4.blockId && var6 > 0)
                    {
                        Block.Blocks[var6].onTick(this, var4.x, var4.y, var4.z, random);
                    }
                }
            }

            return scheduledUpdates.size() != 0;
        }
    }

    public void displayTick(int x, int y, int z)
    {
        byte var4 = 16;
        JavaRandom var5 = new();

        for (int var6 = 0; var6 < 1000; ++var6)
        {
            int var7 = x + random.NextInt(var4) - random.NextInt(var4);
            int var8 = y + random.NextInt(var4) - random.NextInt(var4);
            int var9 = z + random.NextInt(var4) - random.NextInt(var4);
            int var10 = getBlockId(var7, var8, var9);
            if (var10 > 0)
            {
                Block.Blocks[var10].randomDisplayTick(this, var7, var8, var9, var5);
            }
        }

    }

    public List<Entity> getEntities(Entity entity, Box box)
    {
        tempEntityList.Clear();
        int var3 = MathHelper.Floor((box.minX - 2.0D) / 16.0D);
        int var4 = MathHelper.Floor((box.maxX + 2.0D) / 16.0D);
        int var5 = MathHelper.Floor((box.minZ - 2.0D) / 16.0D);
        int var6 = MathHelper.Floor((box.maxZ + 2.0D) / 16.0D);

        for (int var7 = var3; var7 <= var4; ++var7)
        {
            for (int var8 = var5; var8 <= var6; ++var8)
            {
                if (hasChunk(var7, var8))
                {
                    getChunk(var7, var8).collectOtherEntities(entity, box, tempEntityList);
                }
            }
        }

        return tempEntityList;
    }

    public List<Entity> collectEntitiesByClass(Class clazz, Box box)
    {
        int var3 = MathHelper.Floor((box.minX - 2.0D) / 16.0D);
        int var4 = MathHelper.Floor((box.maxX + 2.0D) / 16.0D);
        int var5 = MathHelper.Floor((box.minZ - 2.0D) / 16.0D);
        int var6 = MathHelper.Floor((box.maxZ + 2.0D) / 16.0D);
        List<Entity> var7 = new();

        for (int var8 = var3; var8 <= var4; ++var8)
        {
            for (int var9 = var5; var9 <= var6; ++var9)
            {
                if (hasChunk(var8, var9))
                {
                    getChunk(var8, var9).collectEntitiesByClass(clazz, box, var7);
                }
            }
        }

        return var7;
    }

    public List<Entity> getEntities()
    {
        return entities;
    }

    public void updateBlockEntity(int x, int y, int z, BlockEntity blockEntity)
    {
        if (isPosLoaded(x, y, z))
        {
            getChunkFromPos(x, z).markDirty();
        }

        for (int var5 = 0; var5 < eventListeners.Count; ++var5)
        {
            eventListeners[var5].updateBlockEntity(x, y, z, blockEntity);
        }

    }

    public int countEntities(Class entityClass)
    {
        int var2 = 0;

        for (int var3 = 0; var3 < entities.Count; ++var3)
        {
            Entity var4 = entities[var3];
            if (entityClass.isAssignableFrom(var4.getClass()))
            {
                ++var2;
            }
        }

        return var2;
    }

    public void addEntities(List<Entity> entities)
    {
        this.entities.AddRange(entities);

        for (int var2 = 0; var2 < entities.Count; ++var2)
        {
            NotifyEntityAdded(entities[var2]);
        }

    }

    public void unloadEntities(List<Entity> entities)
    {
        entitiesToUnload.AddRange(entities);
    }

    public void tickChunks()
    {
        while (chunkSource.tick())
        {
        }

    }

    public bool canPlace(int blockId, int x, int y, int z, bool fallingBlock, int side)
    {
        int var7 = getBlockId(x, y, z);
        Block var8 = Block.Blocks[var7];
        Block var9 = Block.Blocks[blockId];
        Box? var10 = var9.getCollisionShape(this, x, y, z);
        if (fallingBlock)
        {
            var10 = null;
        }

        if (var10 != null && !canSpawnEntity(var10.Value))
        {
            return false;
        }
        else
        {
            if (var8 == Block.FlowingWater || var8 == Block.Water || var8 == Block.FlowingLava || var8 == Block.Lava || var8 == Block.Fire || var8 == Block.Snow)
            {
                var8 = null;
            }

            return blockId > 0 && var8 == null && var9.canPlaceAt(this, x, y, z, side);
        }
    }

    public PathEntity findPath(Entity entity, Entity target, float range)
    {
        int var4 = MathHelper.Floor(entity.x);
        int var5 = MathHelper.Floor(entity.y);
        int var6 = MathHelper.Floor(entity.z);
        int var7 = (int)(range + 16.0F);
        int var8 = var4 - var7;
        int var9 = var5 - var7;
        int var10 = var6 - var7;
        int var11 = var4 + var7;
        int var12 = var5 + var7;
        int var13 = var6 + var7;
        WorldRegion var14 = new(this, var8, var9, var10, var11, var12, var13);
        return (new Pathfinder(var14)).createEntityPathTo(entity, target, range);
    }

    public PathEntity findPath(Entity entity, int x, int y, int z, float range)
    {
        int var6 = MathHelper.Floor(entity.x);
        int var7 = MathHelper.Floor(entity.y);
        int var8 = MathHelper.Floor(entity.z);
        int var9 = (int)(range + 8.0F);
        int var10 = var6 - var9;
        int var11 = var7 - var9;
        int var12 = var8 - var9;
        int var13 = var6 + var9;
        int var14 = var7 + var9;
        int var15 = var8 + var9;
        WorldRegion var16 = new(this, var10, var11, var12, var13, var14, var15);
        return (new Pathfinder(var16)).createEntityPathTo(entity, x, y, z, range);
    }

    public bool isStrongPoweringSide(int x, int y, int z, int side)
    {
        int var5 = getBlockId(x, y, z);
        return var5 == 0 ? false : Block.Blocks[var5].isStrongPoweringSide(this, x, y, z, side);
    }

    public bool isStrongPowered(int x, int y, int z)
    {
        return isStrongPoweringSide(x, y - 1, z, 0) ? true : (isStrongPoweringSide(x, y + 1, z, 1) ? true : (isStrongPoweringSide(x, y, z - 1, 2) ? true : (isStrongPoweringSide(x, y, z + 1, 3) ? true : (isStrongPoweringSide(x - 1, y, z, 4) ? true : isStrongPoweringSide(x + 1, y, z, 5)))));
    }

    public bool isPoweringSide(int x, int y, int z, int side)
    {
        if (shouldSuffocate(x, y, z))
        {
            return isStrongPowered(x, y, z);
        }
        else
        {
            int var5 = getBlockId(x, y, z);
            return var5 == 0 ? false : Block.Blocks[var5].isPoweringSide(this, x, y, z, side);
        }
    }

    public bool isPowered(int x, int y, int z)
    {
        return isPoweringSide(x, y - 1, z, 0) ? true : (isPoweringSide(x, y + 1, z, 1) ? true : (isPoweringSide(x, y, z - 1, 2) ? true : (isPoweringSide(x, y, z + 1, 3) ? true : (isPoweringSide(x - 1, y, z, 4) ? true : isPoweringSide(x + 1, y, z, 5)))));
    }

    public EntityPlayer getClosestPlayer(Entity entity, double range)
    {
        return getClosestPlayer(entity.x, entity.y, entity.z, range);
    }

    public EntityPlayer getClosestPlayer(double x, double y, double z, double range)
    {
        double var9 = -1.0D;
        EntityPlayer var11 = null;

        for (int var12 = 0; var12 < players.Count; ++var12)
        {
            EntityPlayer var13 = players[var12];
            double var14 = var13.getSquaredDistance(x, y, z);
            if ((range < 0.0D || var14 < range * range) && (var9 == -1.0D || var14 < var9))
            {
                var9 = var14;
                var11 = var13;
            }
        }

        return var11;
    }

    public EntityPlayer getPlayer(string name)
    {
        for (int var2 = 0; var2 < players.Count; ++var2)
        {
            if (name.Equals(players[var2].name))
            {
                return players[var2];
            }
        }

        return null;
    }

    public void handleChunkDataUpdate(int x, int y, int z, int sideX, int sideY, int sideZ, byte[] chunkData)
    {
        int var8 = x >> 4;
        int var9 = z >> 4;
        int var10 = x + sideX - 1 >> 4;
        int var11 = z + sideZ - 1 >> 4;
        int var12 = 0;
        int var13 = y;
        int var14 = y + sideY;
        if (y < 0)
        {
            var13 = 0;
        }

        if (var14 > 128)
        {
            var14 = 128;
        }

        for (int var15 = var8; var15 <= var10; ++var15)
        {
            int var16 = x - var15 * 16;
            int var17 = x + sideX - var15 * 16;
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
                int var19 = z - var18 * 16;
                int var20 = z + sideZ - var18 * 16;
                if (var19 < 0)
                {
                    var19 = 0;
                }

                if (var20 > 16)
                {
                    var20 = 16;
                }

                var12 = getChunk(var15, var18).loadFromPacket(chunkData, var16, var13, var19, var17, var14, var20, var12);
                setBlocksDirty(var15 * 16 + var16, var13, var18 * 16 + var19, var15 * 16 + var17, var14, var18 * 16 + var20);
            }
        }

    }

    public virtual void Disconnect()
    {
    }

    public byte[] getChunkData(int x, int y, int z, int sizeX, int sizeY, int sizeZ)
    {
        byte[] var7 = new byte[sizeX * sizeY * sizeZ * 5 / 2];
        int var8 = x >> 4;
        int var9 = z >> 4;
        int var10 = x + sizeX - 1 >> 4;
        int var11 = z + sizeZ - 1 >> 4;
        int var12 = 0;
        int var13 = y;
        int var14 = y + sizeY;
        if (y < 0)
        {
            var13 = 0;
        }

        if (var14 > 128)
        {
            var14 = 128;
        }

        for (int var15 = var8; var15 <= var10; var15++)
        {
            int var16 = x - var15 * 16;
            int var17 = x + sizeX - var15 * 16;
            if (var16 < 0)
            {
                var16 = 0;
            }

            if (var17 > 16)
            {
                var17 = 16;
            }

            for (int var18 = var9; var18 <= var11; var18++)
            {
                int var19 = z - var18 * 16;
                int var20 = z + sizeZ - var18 * 16;
                if (var19 < 0)
                {
                    var19 = 0;
                }

                if (var20 > 16)
                {
                    var20 = 16;
                }

                var12 = getChunk(var15, var18).toPacket(var7, var16, var13, var19, var17, var14, var20, var12);
            }
        }

        return var7;
    }

    public void checkSessionLock()
    {
        storage.checkSessionLock();
    }

    public void setTime(long time)
    {
        properties.WorldTime = time;
    }

    public void synchronizeTimeAndUpdates(long time)
    {
        long var3 = time - properties.WorldTime;

        var iter = scheduledUpdateSet.iterator();
        while (iter.hasNext())
        {
            var obj = (BlockEvent)iter.next();
            obj.ticks += var3;
        }

        setTime(time);
    }

    public long getSeed()
    {
        return properties.RandomSeed;
    }

    public long getTime()
    {
        return properties.WorldTime;
    }

    public Vec3i getSpawnPos()
    {
        return new Vec3i(properties.SpawnX, properties.SpawnY, properties.SpawnZ);
    }

    public void setSpawnPos(Vec3i pos)
    {
        properties.SetSpawn(pos.x, pos.y, pos.z);
    }

    public void loadChunksNearEntity(Entity entity)
    {
        int var2 = MathHelper.Floor(entity.x / 16.0D);
        int var3 = MathHelper.Floor(entity.z / 16.0D);
        byte var4 = 2;

        for (int var5 = var2 - var4; var5 <= var2 + var4; ++var5)
        {
            for (int var6 = var3 - var4; var6 <= var3 + var4; ++var6)
            {
                getChunk(var5, var6);
            }
        }

        if (!entities.Contains(entity))
        {
            entities.Add(entity);
        }

    }

    public virtual bool canInteract(EntityPlayer player, int x, int y, int z)
    {
        return true;
    }

    public virtual void broadcastEntityEvent(Entity entity, byte @event)
    {
    }

    public void updateEntityLists()
    {
        foreach (var entity in entitiesToUnload)
        {
            entities.Remove(entity);
        }

        int var1;
        Entity var2;
        int var3;
        int var4;
        for (var1 = 0; var1 < entitiesToUnload.Count; ++var1)
        {
            var2 = entitiesToUnload[var1];
            var3 = var2.chunkX;
            var4 = var2.chunkZ;
            if (var2.isPersistent && hasChunk(var3, var4))
            {
                getChunk(var3, var4).removeEntity(var2);
            }
        }

        for (var1 = 0; var1 < entitiesToUnload.Count; ++var1)
        {
            NotifyEntityRemoved(entitiesToUnload[var1]);
        }

        entitiesToUnload.Clear();

        for (var1 = 0; var1 < entities.Count; ++var1)
        {
            var2 = entities[var1];
            if (var2.vehicle != null)
            {
                if (!var2.vehicle.dead && var2.vehicle.passenger == var2)
                {
                    continue;
                }

                var2.vehicle.passenger = null;
                var2.vehicle = null;
            }

            if (var2.dead)
            {
                var3 = var2.chunkX;
                var4 = var2.chunkZ;
                if (var2.isPersistent && hasChunk(var3, var4))
                {
                    getChunk(var3, var4).removeEntity(var2);
                }

                entities.RemoveAt(var1--);
                NotifyEntityRemoved(var2);
            }
        }

    }

    public ChunkSource getChunkSource()
    {
        return chunkSource;
    }

    public virtual void playNoteBlockActionAt(int x, int y, int z, int soundType, int pitch)
    {
        int var6 = getBlockId(x, y, z);
        if (var6 > 0)
        {
            Block.Blocks[var6].onBlockAction(this, x, y, z, soundType, pitch);
        }

    }

    public WorldProperties getProperties()
    {
        return properties;
    }

    public void updateSleepingPlayers()
    {
        allPlayersSleeping = players.Count > 0;
        foreach (var player in players)
        {
            if (!player.isSleeping())
            {
                allPlayersSleeping = false;
                break;
            }
        }

    }

    protected void afterSkipNight()
    {
        allPlayersSleeping = false;
        foreach (var player in players)
        {
            if (player.isSleeping())
            {
                player.wakeUp(false, false, true);
            }
        }

        clearWeather();
    }

    public bool canSkipNight()
    {
        if (!allPlayersSleeping || isRemote)
        {
            return false;
        }
        return players.All(player => player.isPlayerFullyAsleep());
    }

    public float getThunderGradient(float delta)
    {
        return (prevThunderingStrength + (thunderingStrength - prevThunderingStrength) * delta) * getRainGradient(delta);
    }

    public float getRainGradient(float delta)
    {
        return prevRainingStrength + (rainingStrength - prevRainingStrength) * delta;
    }

    public void setRainGradient(float rainGradient)
    {
        prevRainingStrength = rainGradient;
        rainingStrength = rainGradient;
    }

    public bool isThundering()
    {
        return (double)getThunderGradient(1.0F) > 0.9D;
    }

    public bool isRaining()
    {
        return (double)getRainGradient(1.0F) > 0.2D;
    }

    public bool isRaining(int x, int y, int z)
    {
        if (!isRaining())
        {
            return false;
        }
        else if (!hasSkyLight(x, y, z))
        {
            return false;
        }
        else if (getTopSolidBlockY(x, z) > y)
        {
            return false;
        }
        else
        {
            Biome var4 = getBiomeSource().GetBiome(x, z);
            return var4.GetEnableSnow() ? false : var4.CanSpawnLightningBolt();
        }
    }

    public void setState(string id, PersistentState state)
    {
        persistentStateManager.setData(id, state);
    }

    public PersistentState getOrCreateState(Class @class, string id)
    {
        return persistentStateManager.loadData(@class, id);
    }

    public int getIdCount(string id)
    {
        return persistentStateManager.getUniqueDataId(id);
    }

    public void worldEvent(int @event, int x, int y, int z, int data)
    {
        worldEvent(null, @event, x, y, z, data);
    }

    public void worldEvent(EntityPlayer player, int @event, int x, int y, int z, int data)
    {
        for (int var7 = 0; var7 < eventListeners.Count; ++var7)
        {
            eventListeners[var7].worldEvent(player, @event, x, y, z, data);
        }

    }
}
