using System.Runtime.InteropServices;
using BetaSharp.Blocks;
using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.NBT;
using BetaSharp.Profiling;
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
    public java.util.Random random;
    public bool isNewWorld;
    public readonly Dimension Dimension;
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
    private int lightingUpdatesScheduled = 0;
    private readonly HashSet<ChunkPos> activeChunks;
    private int soundCounter;
    private readonly List<Entity> tempEntityList;
    public bool isRemote;

    public BiomeSource getBiomeSource()
    {
        return Dimension.biomeSource;
    }

    public WorldStorage getWorldStorage()
    {
        return storage;
    }


    public World(WorldStorage WorldStorage, string LevelName, Dimension Dimension, long Seed)
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
        worldTimeMask = 0x00FFFFFFL;
        ambientDarkness = 0;
        lcgBlockSeed = (new java.util.Random()).nextInt();
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
        soundCounter = random.nextInt(12000);
        tempEntityList = [];
        isRemote = false;
        this.storage = storage;
        properties = new WorldProperties(Seed, LevelName);
        this.Dimension = Dimension;
        persistentStateManager = new PersistentStateManager(storage);
        Dimension.setWorld(this);
        chunkSource = CreateChunkCache();
        updateSkyBrightness();
        prepareWeather();
    }

    public World(World World, Dimension Dimension)
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
        worldTimeMask = 0x00FFFFFFL;
        ambientDarkness = 0;
        lcgBlockSeed = (new java.util.Random()).nextInt();
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
        soundCounter = random.nextInt(12000);
        tempEntityList = [];
        isRemote = false;
        lockTimestamp = World.lockTimestamp;
        storage = World.storage;
        properties = new WorldProperties(World.properties);
        persistentStateManager = new PersistentStateManager(storage);
        this.Dimension = Dimension;
        Dimension.setWorld(this);
        chunkSource = CreateChunkCache();
        updateSkyBrightness();
        prepareWeather();
    }

    public World(WorldStorage Storage, string LevelName, long Seed) : this(Storage, LevelName, Seed, null)
    {
    }

    public World(WorldStorage Storage, string LevelName, long Seed, Dimension ProvidedDimension)
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
        worldTimeMask = 0x00FFFFFFL;
        ambientDarkness = 0;
        lcgBlockSeed = (new java.util.Random()).nextInt();
        lcgBlockSeedIncrement = 1013904223;
        ticksSinceLightning = 0;
        lightningTicksLeft = 0;
        pauseTicking = false;
        lockTimestamp = java.lang.System.currentTimeMillis();
        autosavePeriod = AUTOSAVE_PERIOD;
        random = new java.util.Random();
        isNewWorld = false;
        eventListeners = [];
        collidingBoundingBoxes = [];
        lightingUpdatesCounter = 0;
        spawnHostileMobs = true;
        spawnPeacefulMobs = true;
        activeChunks = new HashSet<ChunkPos>();
        soundCounter = random.nextInt(12000);
        tempEntityList = [];
        isRemote = false;
        storage = Storage;
        persistentStateManager = new PersistentStateManager(Storage);
        properties = Storage.loadProperties();
        isNewWorld = properties == null;
        Dimension = (ProvidedDimension, properties?.Dimension) switch
        {
            (not null, _) => ProvidedDimension,
            (null, -1) => Dimension.fromId(-1),
            _ => Dimension.fromId(0),
        };

        bool ShouldInitializeSpawn = false;
        if (properties == null)
        {
            properties = new WorldProperties(Seed, LevelName);
            ShouldInitializeSpawn = true;
        }
        else
        {
            properties.LevelName = LevelName;
        }

        Dimension.setWorld(this);
        chunkSource = CreateChunkCache();
        if (ShouldInitializeSpawn)
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
        int SpawnX = 0;
        byte SpawnY = 64;

        int SpawnZ;
        for (SpawnZ = 0; !Dimension.isValidSpawnPoint(SpawnX, SpawnZ); SpawnZ += random.nextInt(64) - random.nextInt(64))
        {
            SpawnX += random.nextInt(64) - random.nextInt(64);
        }

        properties.SetSpawn(SpawnX, SpawnY, SpawnZ);
        eventProcessingEnabled = false;
    }

    public virtual void UpdateSpawnPosition()
    {
        if (properties.SpawnY <= 0)
        {
            properties.SpawnY = 64;
        }

        int SpawnX = properties.SpawnX;

        int SpawnZ;
        for (SpawnZ = properties.SpawnZ; getSpawnBlockId(SpawnX, SpawnZ) == 0; SpawnZ += random.nextInt(8) - random.nextInt(8))
        {
            SpawnX += random.nextInt(8) - random.nextInt(8);
        }

        properties.SpawnX = SpawnX;
        properties.SpawnZ = SpawnZ;
    }

    public int getSpawnBlockId(int X, int Z)
    {
        int Y;
        for (Y = 63; !isAir(X, Y + 1, Z); ++Y)
        {
        }

        return getBlockId(X, Y, Z);
    }

    public void saveWorldData()
    {
    }

    public void addPlayer(EntityPlayer Player)
    {
        try
        {
            NBTTagCompound? PlayerNbtData = properties.PlayerTag;
            if (PlayerNbtData != null)
            {
                Player.read(PlayerNbtData);
                properties.PlayerTag = null;
            }

            SpawnEntity(Player);
        }
        catch (java.lang.Exception LoadException)
        {
            LoadException.printStackTrace();
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

    private bool IsWithinWorldBounds(int x, int z)
    {
        return x >= -32000000 && z >= -32000000 && x < 32000000 && z <= 32000000;
    }

    public int getBlockId(int X, int Y, int Z)
    {
        return IsWithinWorldBounds(X, Z) ? (Y < 0 ? 0 : (Y >= 128 ? 0 : getChunk(X >> 4, Z >> 4).getBlockId(X & 15, Y, Z & 15))) : 0;
    }

    public bool isAir(int X, int Y, int Z)
    {
        return getBlockId(X, Y, Z) == 0;
    }

    public bool isPosLoaded(int X, int Y, int Z)
    {
        return Y >= 0 && Y < 128 ? hasChunk(X >> 4, Z >> 4) : false;
    }

    public bool isRegionLoaded(int X, int Y, int Z, int Range)
    {
        return isRegionLoaded(X - Range, Y - Range, Z - Range, X + Range, Y + Range, Z + Range);
    }

    public bool isRegionLoaded(int MinX, int MinY, int MinZ, int MaxX, int MaxY, int MaxZ)
    {
        if (MaxY >= 0 && MinY < 128)
        {
            MinX >>= 4;
            MinY >>= 4;
            MinZ >>= 4;
            MaxX >>= 4;
            MaxY >>= 4;
            MaxZ >>= 4;

            for (int i = MinX; i <= MaxX; ++i)
            {
                for (int j = MinZ; j <= MaxZ; ++j)
                {
                    if (!hasChunk(i, j))
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

    public virtual bool SetBlockWithoutNotifyingNeighbors(int X, int Y, int Z, int blockId, int meta)
    {
        if (IsWithinWorldBounds(X, Z))
        {
            switch (Y)
            {
                case < 0:
                case >= 128:
                    return false;
                default:
                    Chunk TargetChunk = getChunk(X >> 4, Z >> 4);
                    return TargetChunk.setBlock(X & 15, Y, Z & 15, blockId, meta);
            }
        }
        else
        {
            return false;
        }
    }

    public virtual bool SetBlockWithoutNotifyingNeighbors(int X, int Y, int Z, int blockId)
    {
        if (IsWithinWorldBounds(X, Z))
        {
            switch (Y)
            {
                case < 0:
                case >= 128:
                    return false;
                default:
                    Chunk TargetChunk = getChunk(X >> 4, Z >> 4);
                    return TargetChunk.setBlock(X & 15, Y, Z & 15, blockId);
            }
        }
        else
        {
            return false;
        }
    }

    public Material getMaterial(int X, int Y, int Z)
    {
        int BlockId = getBlockId(X, Y, Z);
        return BlockId == 0 ? Material.Air : Block.Blocks[BlockId].material;
    }

    public int getBlockMeta(int X, int Y, int Z)
    {
        if (IsWithinWorldBounds(X, Z))
        {
            switch (Y)
            {
                case < 0:
                case >= 128:
                    return 0;
                default:
                    Chunk TargetChunk = getChunk(X >> 4, Z >> 4);
                    X &= 15;
                    Z &= 15;
                    return TargetChunk.getBlockMeta(X, Y, Z);
            }
        }
        else
        {
            return 0;
        }
    }

    public void setBlockMeta(int X, int Y, int Z, int meta)
    {
        if (SetBlockMetaWithoutNotifyingNeighbors(X, Y, Z, meta))
        {
            int CurrentBlockId = getBlockId(X, Y, Z);
            if (Block.BlocksIngoreMetaUpdate[CurrentBlockId & 255])
            {
                blockUpdate(X, Y, Z, CurrentBlockId);
            }
            else
            {
                notifyNeighbors(X, Y, Z, CurrentBlockId);
            }
        }

    }

    public virtual bool SetBlockMetaWithoutNotifyingNeighbors(int X, int Y, int Z, int Meta)
    {
        if (IsWithinWorldBounds(X, Z))
        {
            switch (Y)
            {
                case < 0:
                case >= 128:
                    return false;
                default:
                    Chunk TargetChunk = getChunk(X >> 4, Z >> 4);
                    X &= 15;
                    Z &= 15;
                    TargetChunk.setBlockMeta(X, Y, Z, Meta);
                    return true;
            }
        }
        else
        {
            return false;
        }
    }

    public bool setBlock(int X, int Y, int Z, int blockId)
    {
        if (SetBlockWithoutNotifyingNeighbors(X, Y, Z, blockId))
        {
            blockUpdate(X, Y, Z, blockId);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool setBlock(int X, int Y, int Z, int blockId, int meta)
    {
        if (SetBlockWithoutNotifyingNeighbors(X, Y, Z, blockId, meta))
        {
            blockUpdate(X, Y, Z, blockId);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void blockUpdateEvent(int X, int Y, int Z)
    {
        for (int ListenerIndex = 0; ListenerIndex < eventListeners.Count; ++ListenerIndex)
        {
            eventListeners[ListenerIndex].blockUpdate(X, Y, Z);
        }

    }

    protected void blockUpdate(int X, int Y, int Z, int blockId)
    {
        blockUpdateEvent(X, Y, Z);
        notifyNeighbors(X, Y, Z, blockId);
    }

    public void setBlocksDirty(int x, int z, int minY, int maxY)
    {
        if (minY > maxY)
        {
            (maxY, minY) = (minY, maxY);
        }

        setBlocksDirty(x, minY, z, x, maxY, z);
    }

    public void setBlocksDirty(int X, int Y, int Z)
    {
        for (int ListenerIndex = 0; ListenerIndex < eventListeners.Count; ++ListenerIndex)
        {
            eventListeners[ListenerIndex].setBlocksDirty(X, Y, Z, X, Y, Z);
        }

    }

    public void setBlocksDirty(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
    {
        for (int ListenerIndex = 0; ListenerIndex < eventListeners.Count; ++ListenerIndex)
        {
            eventListeners[ListenerIndex].setBlocksDirty(minX, minY, minZ, maxX, maxY, maxZ);
        }

    }

    public void notifyNeighbors(int X, int Y, int Z, int BlockId)
    {
        notifyUpdate(X - 1, Y, Z, BlockId);
        notifyUpdate(X + 1, Y, Z, BlockId);
        notifyUpdate(X, Y - 1, Z, BlockId);
        notifyUpdate(X, Y + 1, Z, BlockId);
        notifyUpdate(X, Y, Z - 1, BlockId);
        notifyUpdate(X, Y, Z + 1, BlockId);
    }

    private void notifyUpdate(int X, int Y, int Z, int blockId)
    {
        if (!pauseTicking && !isRemote)
        {
            Block TargetBlock = Block.Blocks[getBlockId(X, Y, Z)];
            if (TargetBlock != null)
            {
                TargetBlock.neighborUpdate(this, X, Y, Z, blockId);
            }

        }
    }

    public bool hasSkyLight(int X, int Y, int Z)
    {
        return getChunk(X >> 4, Z >> 4).isAboveMaxHeight(X & 15, Y, Z & 15);
    }

    public int getBrightness(int X, int Y, int Z)
    {
        if (Y < 0)
        {
            return 0;
        }
        else
        {
            if (Y >= 128)
            {
                Y = 127;
            }

            return getChunk(X >> 4, Z >> 4).getLight(X & 15, Y, Z & 15, 0);
        }
    }

    public int getLightLevel(int X, int Y, int Z)
    {
        return getLightLevel(X, Y, Z, true);
    }

    public int getLightLevel(int X, int Y, int Z, bool bl)
    {
        if (IsWithinWorldBounds(X, Z))
        {
            if (bl)
            {
                int BlockIdAt = getBlockId(X, Y, Z);
                if (BlockIdAt == Block.Slab.id || BlockIdAt == Block.Farmland.id || BlockIdAt == Block.CobblestoneStairs.id || BlockIdAt == Block.WoodenStairs.id)
                {
                    int UpLight = getLightLevel(X, Y + 1, Z, false);
                    int EastLight = getLightLevel(X + 1, Y, Z, false);
                    int WestLight = getLightLevel(X - 1, Y, Z, false);
                    int SouthLight = getLightLevel(X, Y, Z + 1, false);
                    int NorthLight = getLightLevel(X, Y, Z - 1, false);
                    if (EastLight > UpLight)
                    {
                        UpLight = EastLight;
                    }

                    if (WestLight > UpLight)
                    {
                        UpLight = WestLight;
                    }

                    if (SouthLight > UpLight)
                    {
                        UpLight = SouthLight;
                    }

                    if (NorthLight > UpLight)
                    {
                        UpLight = NorthLight;
                    }

                    return UpLight;
                }
            }

            if (Y < 0)
            {
                return 0;
            }
            else
            {
                if (Y >= 128)
                {
                    Y = 127;
                }

                Chunk TargetChunk = getChunk(X >> 4, Z >> 4);
                X &= 15;
                Z &= 15;
                return TargetChunk.getLight(X, Y, Z, ambientDarkness);
            }
        }
        else
        {
            return 15;
        }
    }

    public bool isTopY(int X, int Y, int Z)
    {
        if (!IsWithinWorldBounds(X, Z))
            return false;
        
        if (Y < 0)
            return false;
        
        if (Y >= 128)
            return true;
        
        if (!hasChunk(X >> 4, Z >> 4))
            return false;
        
        Chunk TargetChunk = getChunk(X >> 4, Z >> 4);
        X &= 15;
        Z &= 15;
        return TargetChunk.isAboveMaxHeight(X, Y, Z);
    }

    public int getTopY(int x, int z)
    {
        if (IsWithinWorldBounds(x, z))
        {
            if (!hasChunk(x >> 4, z >> 4))
            {
                return 0;
            }
            else
            {
                Chunk TargetChunk = getChunk(x >> 4, z >> 4);
                return TargetChunk.getHeight(x & 15, z & 15);
            }
        }
        else
        {
            return 0;
        }
    }

    public void updateLight(LightType lightType, int X, int Y, int Z, int l)
    {
        if (!Dimension.hasCeiling || lightType != LightType.Sky)
        {
            if (isPosLoaded(X, Y, Z))
            {
                if (lightType == LightType.Sky)
                {
                    if (isTopY(X, Y, Z))
                    {
                        l = 15;
                    }
                }
                else if (lightType == LightType.Block)
                {
                    int BlockIdAt = getBlockId(X, Y, Z);
                    if (Block.BlocksLightLuminance[BlockIdAt] > l)
                    {
                        l = Block.BlocksLightLuminance[BlockIdAt];
                    }
                }

                if (getBrightness(lightType, X, Y, Z) != l)
                {
                    queueLightUpdate(lightType, X, Y, Z, X, Y, Z);
                }

            }
        }
    }

    public int getBrightness(LightType LightType, int X, int Y, int Z)
    {
        // Clamp Y to valid range [0, 127]
        if (Y < 0) Y = 0;
        if (Y >= 128) Y = 127;
        
        if (!IsWithinWorldBounds(X, Z))
            return LightType.lightValue;
        
        int chunkX = X >> 4;
        int chunkZ = Z >> 4;
        
        if (!hasChunk(chunkX, chunkZ))
            return 0;
        
        Chunk TargetChunk = getChunk(chunkX, chunkZ);
        return TargetChunk.getLight(LightType, X & 15, Y, Z & 15);
    }

    public void setLight(LightType LightType, int X, int Y, int Z, int Value)
    {
        if (!IsWithinWorldBounds(X, Z) || Y < 0 || Y >= 128)
            return;
        
        if (!hasChunk(X >> 4, Z >> 4))
            return;
        
        Chunk TargetChunk = getChunk(X >> 4, Z >> 4);
        TargetChunk.setLight(LightType, X & 15, Y, Z & 15, Value);

        for (int ListenerIndex = 0; ListenerIndex < eventListeners.Count; ++ListenerIndex)
        {
            eventListeners[ListenerIndex].blockUpdate(X, Y, Z);
        }
    }

    public float getNaturalBrightness(int X, int Y, int Z, int BlockLight)
    {
        int CurrentLightLevel = getLightLevel(X, Y, Z);
        if (CurrentLightLevel < BlockLight)
        {
            CurrentLightLevel = BlockLight;
        }

        return Dimension.lightLevelToLuminance[CurrentLightLevel];
    }

    public float getLuminance(int X, int Y, int Z)
    {
        return Dimension.lightLevelToLuminance[getLightLevel(X, Y, Z)];
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
        if (java.lang.Double.isNaN(start.x) || java.lang.Double.isNaN(start.y) || java.lang.Double.isNaN(start.z))
        {
            return null;
        }

        if (java.lang.Double.isNaN(pos.x) || java.lang.Double.isNaN(pos.y) || java.lang.Double.isNaN(pos.z))
        {
            return null;
        }

        int endX = MathHelper.floor_double(pos.x);
        int endY = MathHelper.floor_double(pos.y);
        int endZ = MathHelper.floor_double(pos.z);
        int startX = MathHelper.floor_double(start.x);
        int startY = MathHelper.floor_double(start.y);
        int startZ = MathHelper.floor_double(start.z);

        int initialBlockId = getBlockId(startX, startY, startZ);
        int initialMeta = getBlockMeta(startX, startY, startZ);
        Block initialBlock = Block.Blocks[initialBlockId];
        if ((!bl2 || initialBlock == null || initialBlock.getCollisionShape(this, startX, startY, startZ) != null) && initialBlockId > 0 && initialBlock.hasCollision(initialMeta, bl))
        {
            HitResult hit = initialBlock.raycast(this, startX, startY, startZ, start, pos);
            if (hit != null)
            {
                return hit;
            }
        }

        int stepsRemaining = 200;
        while (stepsRemaining-- >= 0)
        {
            if (java.lang.Double.isNaN(start.x) || java.lang.Double.isNaN(start.y) || java.lang.Double.isNaN(start.z))
            {
                return null;
            }

            if (startX == endX && startY == endY && startZ == endZ)
            {
                return null;
            }

            bool stepX = true;
            bool stepY = true;
            bool stepZ = true;
            double nextPlaneX = 999.0D;
            double nextPlaneY = 999.0D;
            double nextPlaneZ = 999.0D;

            if (endX > startX)
            {
                nextPlaneX = (double)startX + 1.0D;
            }
            else if (endX < startX)
            {
                nextPlaneX = (double)startX + 0.0D;
            }
            else
            {
                stepX = false;
            }

            if (endY > startY)
            {
                nextPlaneY = (double)startY + 1.0D;
            }
            else if (endY < startY)
            {
                nextPlaneY = (double)startY + 0.0D;
            }
            else
            {
                stepY = false;
            }

            if (endZ > startZ)
            {
                nextPlaneZ = (double)startZ + 1.0D;
            }
            else if (endZ < startZ)
            {
                nextPlaneZ = (double)startZ + 0.0D;
            }
            else
            {
                stepZ = false;
            }

            double tX = 999.0D;
            double tY = 999.0D;
            double tZ = 999.0D;
            double deltaX = pos.x - start.x;
            double deltaY = pos.y - start.y;
            double deltaZ = pos.z - start.z;
            if (stepX)
            {
                tX = (nextPlaneX - start.x) / deltaX;
            }

            if (stepY)
            {
                tY = (nextPlaneY - start.y) / deltaY;
            }

            if (stepZ)
            {
                tZ = (nextPlaneZ - start.z) / deltaZ;
            }

            byte side;
            if (tX < tY && tX < tZ)
            {
                side = (byte)(endX > startX ? 4 : 5);
                start.x = nextPlaneX;
                start.y += deltaY * tX;
                start.z += deltaZ * tX;
            }
            else if (tY < tZ)
            {
                side = (byte)(endY > startY ? 0 : 1);
                start.x += deltaX * tY;
                start.y = nextPlaneY;
                start.z += deltaZ * tY;
            }
            else
            {
                side = (byte)(endZ > startZ ? 2 : 3);
                start.x += deltaX * tZ;
                start.y += deltaY * tZ;
                start.z = nextPlaneZ;
            }

            Vec3D hitVec = new Vec3D(start.x, start.y, start.z);
            startX = (int)(hitVec.x = (double)MathHelper.floor_double(start.x));
            if (side == 5)
            {
                --startX;
                ++hitVec.x;
            }

            startY = (int)(hitVec.y = (double)MathHelper.floor_double(start.y));
            if (side == 1)
            {
                --startY;
                ++hitVec.y;
            }

            startZ = (int)(hitVec.z = (double)MathHelper.floor_double(start.z));
            if (side == 3)
            {
                --startZ;
                ++hitVec.z;
            }

            int blockIdAt = getBlockId(startX, startY, startZ);
            int blockMetaAt = getBlockMeta(startX, startY, startZ);
            Block blockAt = Block.Blocks[blockIdAt];
            if ((!bl2 || blockAt == null || blockAt.getCollisionShape(this, startX, startY, startZ) != null) && blockIdAt > 0 && blockAt.hasCollision(blockMetaAt, bl))
            {
                HitResult hit = blockAt.raycast(this, startX, startY, startZ, start, pos);
                if (hit != null)
                {
                    return hit;
                }
            }
        }

        return null;
    }

    public void playSound(Entity Entity, string Sound, float Volume, float Pitch)
    {
        for (int i = 0; i < eventListeners.Count; ++i)
        {
            eventListeners[i].playSound(Sound, Entity.x, Entity.y - (double)Entity.standingEyeHeight, Entity.z, Volume, Pitch);
        }

    }

    public void playSound(double x, double y, double z, string sound, float volume, float pitch)
    {
        for (int i = 0; i < eventListeners.Count; ++i)
        {
            eventListeners[i].playSound(sound, x, y, z, volume, pitch);
        }

    }

    public void playStreaming(string music, int X, int Y, int Z)
    {
        for (int i = 0; i < eventListeners.Count; ++i)
        {
            eventListeners[i].playStreaming(music, X, Y, Z);
        }

    }

    public void addParticle(string Particle, double X, double Y, double Z, double VelocityX, double VelocityY, double VelocityZ)
    {
        for (int i = 0; i < eventListeners.Count; ++i)
        {
            eventListeners[i].spawnParticle(Particle, X, Y, Z, VelocityX, VelocityY, VelocityZ);
        }

    }

    public virtual bool spawnGlobalEntity(Entity Entity)
    {
        globalEntities.add(Entity);
        return true;
    }

    public virtual bool SpawnEntity(Entity Entity)
    {
        int chunkX = MathHelper.floor_double(Entity.x / 16.0D);
        int chunkZ = MathHelper.floor_double(Entity.z / 16.0D);
        bool isPlayerEntity = false;
        if (Entity is EntityPlayer) isPlayerEntity = true;

        if (!isPlayerEntity && !hasChunk(chunkX, chunkZ))
        {
            return false;
        }
        else
        {
            if (Entity is EntityPlayer)
            {
                EntityPlayer Player = (EntityPlayer)Entity;
                players.Add(Player);
                updateSleepingPlayers();
            }

            getChunk(chunkX, chunkZ).addEntity(Entity);
            entities.Add(Entity);
            NotifyEntityAdded(Entity);
            return true;
        }
    }

    protected virtual void NotifyEntityAdded(Entity Entity)
    {
        for (int ListenerIndex = 0; ListenerIndex < eventListeners.Count; ++ListenerIndex)
        {
            eventListeners[ListenerIndex].notifyEntityAdded(Entity);
        }

    }

    protected virtual void NotifyEntityRemoved(Entity Entity)
    {
        for (int ListenerIndex = 0; ListenerIndex < eventListeners.Count; ++ListenerIndex)
        {
            eventListeners[ListenerIndex].notifyEntityRemoved(Entity);
        }

    }

    public virtual void Remove(Entity Entity)
    {
        Entity.passenger?.setVehicle(null);
        Entity.vehicle?.setVehicle(null);

        Entity.markDead();
        
        if (Entity is EntityPlayer player)
        {
            players.Remove(player);
            updateSleepingPlayers();
        }
    }

    public void serverRemove(Entity Entity)
    {
        Entity.markDead();
        if (Entity is EntityPlayer Player)
        {
            players.Remove(Player);
            this.updateSleepingPlayers();
        }

        int chunkX = Entity.chunkX;
        int chunkZ = Entity.chunkZ;
        if (Entity.isPersistent && hasChunk(chunkX, chunkZ))
        {
            getChunk(chunkX, chunkZ).removeEntity(Entity);
        }

        entities.Remove(Entity);
        NotifyEntityRemoved(Entity);
    }

    public void addWorldAccess(IWorldAccess WorldAccess)
    {
        eventListeners.Add(WorldAccess);
    }

    public void removeWorldAccess(IWorldAccess WorldAccess)
    {
        eventListeners.Remove(WorldAccess);
    }

    public List<Box> getEntityCollisions(Entity entity, Box box)
    {
        collidingBoundingBoxes.Clear();
        int minX = MathHelper.floor_double(box.minX);
        int maxX = MathHelper.floor_double(box.maxX + 1.0D);
        int minY = MathHelper.floor_double(box.minY);
        int maxY = MathHelper.floor_double(box.maxY + 1.0D);
        int minZ = MathHelper.floor_double(box.minZ);
        int maxZ = MathHelper.floor_double(box.maxZ + 1.0D);

        for (int x = minX; x < maxX; ++x)
        {
            for (int z = minZ; z < maxZ; ++z)
            {
                if (isPosLoaded(x, 64, z))
                {
                    for (int y = minY - 1; y < maxY; ++y)
                    {
                        Block block = Block.Blocks[getBlockId(x, y, z)];
                        if (block != null)
                        {
                            block.addIntersectingBoundingBox(this, x, y, z, box, collidingBoundingBoxes);
                        }
                    }
                }
            }
        }

        double expand = 0.25D;
        List<Entity> nearbyEntities = getEntities(entity, box.expand(expand, expand, expand));

        for (int idx = 0; idx < nearbyEntities.Count; ++idx)
        {
            Box? entityBox = nearbyEntities[idx].getBoundingBox();
            if (entityBox != null && entityBox.Value.intersects(box))
            {
                collidingBoundingBoxes.Add(entityBox.Value);
            }

            entityBox = entity.getCollisionAgainstShape(nearbyEntities[idx]);
            if (entityBox != null && entityBox.Value.intersects(box))
            {
                collidingBoundingBoxes.Add(entityBox.Value);
            }
        }

        return collidingBoundingBoxes;
    }

    public int getAmbientDarkness(float partialTicks)
    {
        float timeOfDay = getTime(partialTicks);
        float timeFactor = 1.0F - (MathHelper.cos(timeOfDay * (float)java.lang.Math.PI * 2.0F) * 2.0F + 0.5F);
        if (timeFactor < 0.0F)
        {
            timeFactor = 0.0F;
        }

        if (timeFactor > 1.0F)
        {
            timeFactor = 1.0F;
        }

        timeFactor = 1.0F - timeFactor;
        timeFactor = (float)((double)timeFactor * (1.0D - (double)(getRainGradient(partialTicks) * 5.0F) / 16.0D));
        timeFactor = (float)((double)timeFactor * (1.0D - (double)(getThunderGradient(partialTicks) * 5.0F) / 16.0D));
        timeFactor = 1.0F - timeFactor;
        return (int)(timeFactor * 11.0F);
    }

    public Vector3D<double> getSkyColor(Entity entity, float partialTicks)
    {
        float timeOfDay = getTime(partialTicks);
        float brightnessFactor = MathHelper.cos(timeOfDay * (float)java.lang.Math.PI * 2.0F) * 2.0F + 0.5F;
        if (brightnessFactor < 0.0F)
        {
            brightnessFactor = 0.0F;
        }

        if (brightnessFactor > 1.0F)
        {
            brightnessFactor = 1.0F;
        }

        int posX = MathHelper.floor_double(entity.x);
        int posZ = MathHelper.floor_double(entity.z);
        float temperature = (float)getBiomeSource().GetTemperature(posX, posZ);
        int skyColor = getBiomeSource().GetBiome(posX, posZ).GetSkyColorByTemp(temperature);
        float r = (float)(skyColor >> 16 & 255) / 255.0F;
        float g = (float)(skyColor >> 8 & 255) / 255.0F;
        float b = (float)(skyColor & 255) / 255.0F;
        r *= brightnessFactor;
        g *= brightnessFactor;
        b *= brightnessFactor;
        float rainGradient = getRainGradient(partialTicks);
        float mixA;
        float mixB;
        if (rainGradient > 0.0F)
        {
            mixA = (r * 0.3F + g * 0.59F + b * 0.11F) * 0.6F;
            mixB = 1.0F - rainGradient * (12.0F / 16.0F);
            r = r * mixB + mixA * (1.0F - mixB);
            g = g * mixB + mixA * (1.0F - mixB);
            b = b * mixB + mixA * (1.0F - mixB);
        }

        mixA = getThunderGradient(partialTicks);
        if (mixA > 0.0F)
        {
            mixB = (r * 0.3F + g * 0.59F + b * 0.11F) * 0.2F;
            float thunderBlend = 1.0F - mixA * (12.0F / 16.0F);
            r = r * thunderBlend + mixB * (1.0F - thunderBlend);
            g = g * thunderBlend + mixB * (1.0F - thunderBlend);
            b = b * thunderBlend + mixB * (1.0F - thunderBlend);
        }

        if (lightningTicksLeft > 0)
        {
            float lightningFactor = (float)lightningTicksLeft - partialTicks;
            if (lightningFactor > 1.0F)
            {
                lightningFactor = 1.0F;
            }

            lightningFactor *= 0.45F;
            r = r * (1.0F - lightningFactor) + 0.8F * lightningFactor;
            g = g * (1.0F - lightningFactor) + 0.8F * lightningFactor;
            b = b * (1.0F - lightningFactor) + 1.0F * lightningFactor;
        }

        return new((double)r, (double)g, (double)b);
    }

    public float getTime(float partialTicks)
    {
        return Dimension.getTimeOfDay(properties.WorldTime, partialTicks);
    }

    public Vector3D<double> getCloudColor(float partialTicks)
    {
        float timeOfDay = getTime(partialTicks);
        float cloudFactor = MathHelper.cos(timeOfDay * (float)java.lang.Math.PI * 2.0F) * 2.0F + 0.5F;
        if (cloudFactor < 0.0F)
        {
            cloudFactor = 0.0F;
        }

        if (cloudFactor > 1.0F)
        {
            cloudFactor = 1.0F;
        }

        float r = (float)(worldTimeMask >> 16 & 255L) / 255.0F;
        float g = (float)(worldTimeMask >> 8 & 255L) / 255.0F;
        float b = (float)(worldTimeMask & 255L) / 255.0F;
        float rainGrad = getRainGradient(partialTicks);
        float mixA;
        float mixB;
        if (rainGrad > 0.0F)
        {
            mixA = (r * 0.3F + g * 0.59F + b * 0.11F) * 0.6F;
            mixB = 1.0F - rainGrad * 0.95F;
            r = r * mixB + mixA * (1.0F - mixB);
            g = g * mixB + mixA * (1.0F - mixB);
            b = b * mixB + mixA * (1.0F - mixB);
        }

        r *= cloudFactor * 0.9F + 0.1F;
        g *= cloudFactor * 0.9F + 0.1F;
        b *= cloudFactor * 0.85F + 0.15F;
        mixA = getThunderGradient(partialTicks);
        if (mixA > 0.0F)
        {
            mixB = (r * 0.3F + g * 0.59F + b * 0.11F) * 0.2F;
            float thunderBlend = 1.0F - mixA * 0.95F;
            r = r * thunderBlend + mixB * (1.0F - thunderBlend);
            g = g * thunderBlend + mixB * (1.0F - thunderBlend);
            b = b * thunderBlend + mixB * (1.0F - thunderBlend);
        }

        return new((double)r, (double)g, (double)b);
    }

    public Vector3D<double> getFogColor(float partialTicks)
    {
        float timeOfDay = getTime(partialTicks);
        return Dimension.getFogColor(timeOfDay, partialTicks);
    }

    public int getTopSolidBlockY(int x, int z)
    {
        Chunk chunk = getChunkFromPos(x, z);
        int y = 127;
        x &= 15;

        for (z &= 15; y > 0; --y)
        {
            int blockId = chunk.getBlockId(x, y, z);
            Material material = blockId == 0 ? Material.Air : Block.Blocks[blockId].material;
            if (material.BlocksMovement || material.IsFluid)
            {
                return y + 1;
            }
        }

        return -1;
    }

    public float calcualteSkyLightIntensity(float partialTicks)
    {
        float timeOfDay = getTime(partialTicks);
        float intensityFactor = 1.0F - (MathHelper.cos(timeOfDay * (float)java.lang.Math.PI * 2.0F) * 2.0F + 12.0F / 16.0F);
        if (intensityFactor < 0.0F)
        {
            intensityFactor = 0.0F;
        }

        if (intensityFactor > 1.0F)
        {
            intensityFactor = 1.0F;
        }

        return intensityFactor * intensityFactor * 0.5F;
    }

    public int getSpawnPositionValidityY(int x, int z)
    {
        Chunk chunk = getChunkFromPos(x, z);
        int y = 127;
        x &= 15;

        for (int zLocal = z & 15; y > 0; y--)
        {
            int blockId = chunk.getBlockId(x, y, zLocal);
            if (blockId != 0 && Block.Blocks[blockId].material.BlocksMovement)
            {
                return y + 1;
            }
        }

        return -1;
    }

    public virtual void ScheduleBlockUpdate(int X, int Y, int Z, int Id, int TickRate)
    {
        BlockEvent blockEvent = new(X, Y, Z, Id);
        byte range = 8;
        if (instantBlockUpdateEnabled)
        {
            if (isRegionLoaded(blockEvent.x - range, blockEvent.y - range, blockEvent.z - range, blockEvent.x + range, blockEvent.y + range, blockEvent.z + range))
            {
                int currentBlockId = getBlockId(blockEvent.x, blockEvent.y, blockEvent.z);
                if (currentBlockId == blockEvent.blockId && currentBlockId > 0)
                {
                    Block.Blocks[currentBlockId].onTick(this, blockEvent.x, blockEvent.y, blockEvent.z, random);
                }
            }

        }
        else
        {
            if (isRegionLoaded(X - range, Y - range, Z - range, X + range, Y + range, Z + range))
            {
                if (Id > 0)
                {
                    blockEvent.setScheduledTime((long)TickRate + properties.WorldTime);
                }

                if (!scheduledUpdateSet.contains(blockEvent))
                {
                    scheduledUpdateSet.add(blockEvent);
                    scheduledUpdates.add(blockEvent);
                }
            }

        }
    }

    public void tickEntities()
    {
        Profiler.Start("updateEntites.updateWeatherEffects");

        int globalIndex;
        Entity currentEntity;
        for (globalIndex = 0; globalIndex < globalEntities.size(); ++globalIndex)
        {
            currentEntity = (Entity)globalEntities.get(globalIndex);
            currentEntity.tick();
            if (currentEntity.dead)
            {
                globalEntities.remove(globalIndex--);
            }
        }
        Profiler.Stop("updateEntites.updateWeatherEffects");

        foreach (Entity UnloadedEntity in entitiesToUnload)
        {
            entities.Remove(UnloadedEntity);
        }

        Profiler.Start("updateEntites.clearUnloadedEntities");

        int chunkX;
        int chunkZ;
        for (globalIndex = 0; globalIndex < entitiesToUnload.Count; ++globalIndex)
        {
            currentEntity = entitiesToUnload[globalIndex];
            chunkX = currentEntity.chunkX;
            chunkZ = currentEntity.chunkZ;
            if (currentEntity.isPersistent && hasChunk(chunkX, chunkZ))
            {
                getChunk(chunkX, chunkZ).removeEntity(currentEntity);
            }
        }

        for (globalIndex = 0; globalIndex < entitiesToUnload.Count; ++globalIndex)
        {
            NotifyEntityRemoved(entitiesToUnload[globalIndex]);
        }

        entitiesToUnload.Clear();

        Profiler.Stop("updateEntites.clearUnloadedEntities");

        Profiler.Start("updateEntites.updateLoadedEntities");

        for (globalIndex = 0; globalIndex < entities.Count; ++globalIndex)
        {
            currentEntity = entities[globalIndex];
            if (currentEntity.vehicle != null)
            {
                if (!currentEntity.vehicle.dead && currentEntity.vehicle.passenger == currentEntity)
                {
                    continue;
                }

                currentEntity.vehicle.passenger = null;
                currentEntity.vehicle = null;
            }

            if (!currentEntity.dead)
            {
                updateEntity(currentEntity);
            }

            if (currentEntity.dead)
            {
                chunkX = currentEntity.chunkX;
                chunkZ = currentEntity.chunkZ;
                if (currentEntity.isPersistent && hasChunk(chunkX, chunkZ))
                {
                    getChunk(chunkX, chunkZ).removeEntity(currentEntity);
                }

                entities.RemoveAt(globalIndex--);
                NotifyEntityRemoved(currentEntity);
            }
        }
        Profiler.Stop("updateEntites.updateLoadedEntities");

        processingDeferred = true;

        Profiler.Start("updateEntites.updateLoadedTileEntities");

        for (int i = blockEntities.Count - 1; i >= 0; i--)
        {
            BlockEntity blockEntity = blockEntities[i];
            if (!blockEntity.isRemoved())
            {
                blockEntity.tick();
            }
            if (blockEntity.isRemoved())
            {
                blockEntities.RemoveAt(i);
                Chunk chunk = getChunk(blockEntity.x >> 4, blockEntity.z >> 4);
                if (chunk != null)
                {
                    chunk.removeBlockEntityAt(blockEntity.x & 15, blockEntity.y, blockEntity.z & 15);
                }
            }
        }

        processingDeferred = false;
        if (blockEntityUpdateQueue.Count > 0)
        {
            foreach (BlockEntity queuedBlockEntity in blockEntityUpdateQueue)
            {
                if (!queuedBlockEntity.isRemoved())
                {
                    if (!blockEntities.Contains(queuedBlockEntity))
                    {
                        blockEntities.Add(queuedBlockEntity);
                    }
                    Chunk targetChunk = getChunk(queuedBlockEntity.x >> 4, queuedBlockEntity.z >> 4);
                    if (targetChunk != null)
                    {
                        targetChunk.setBlockEntity(queuedBlockEntity.x & 15, queuedBlockEntity.y, queuedBlockEntity.z & 15, queuedBlockEntity);
                    }
                    blockUpdateEvent(queuedBlockEntity.x, queuedBlockEntity.y, queuedBlockEntity.z);
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
        int posX = MathHelper.floor_double(entity.x);
        int posZ = MathHelper.floor_double(entity.z);
        byte range = 32;
        if (!requireLoaded || isRegionLoaded(posX - range, 0, posZ - range, posX + range, 128, posZ + range))
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

            int chunkX = MathHelper.floor_double(entity.x / 16.0D);
            int chunkSlice = MathHelper.floor_double(entity.y / 16.0D);
            int chunkZ = MathHelper.floor_double(entity.z / 16.0D);
            if (!entity.isPersistent || entity.chunkX != chunkX || entity.chunkSlice != chunkSlice || entity.chunkZ != chunkZ)
            {
                if (entity.isPersistent && hasChunk(entity.chunkX, entity.chunkZ))
                {
                    getChunk(entity.chunkX, entity.chunkZ).removeEntity(entity, entity.chunkSlice);
                }

                if (hasChunk(chunkX, chunkZ))
                {
                    entity.isPersistent = true;
                    getChunk(chunkX, chunkZ).addEntity(entity);
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
        List<Entity> nearby = getEntities((Entity)null, box);

        for (int i = 0; i < nearby.Count; ++i)
        {
            Entity e = nearby[i];
            if (!e.dead && e.preventEntitySpawning)
            {
                return false;
            }
        }

        return true;
    }

    public bool isAnyBlockInBox(Box box)
    {
        int minX = MathHelper.floor(box.minX);
        int maxX = MathHelper.floor(box.maxX + 1.0);
        int minY = MathHelper.floor(box.minY);
        int maxY = MathHelper.floor(box.maxY + 1.0);
        int minZ = MathHelper.floor(box.minZ);
        int maxZ = MathHelper.floor(box.maxZ + 1.0);
        if (box.minX < 0.0)
        {
            minX--;
        }

        if (box.minY < 0.0)
        {
            minY--;
        }

        if (box.minZ < 0.0)
        {
            minZ--;
        }

        for (int x = minX; x < maxX; x++)
        {
            for (int y = minY; y < maxY; y++)
            {
                for (int z = minZ; z < maxZ; z++)
                {
                    Block block = Block.Blocks[getBlockId(x, y, z)];
                    if (block != null)
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
        int minX = MathHelper.floor_double(box.minX);
        int maxX = MathHelper.floor_double(box.maxX + 1.0D);
        int minY = MathHelper.floor_double(box.minY);
        int maxY = MathHelper.floor_double(box.maxY + 1.0D);
        int minZ = MathHelper.floor_double(box.minZ);
        int maxZ = MathHelper.floor_double(box.maxZ + 1.0D);
        if (box.minX < 0.0D)
        {
            --minX;
        }

        if (box.minY < 0.0D)
        {
            --minY;
        }

        if (box.minZ < 0.0D)
        {
            --minZ;
        }

        for (int x = minX; x < maxX; ++x)
        {
            for (int y = minY; y < maxY; ++y)
            {
                for (int z = minZ; z < maxZ; ++z)
                {
                    Block block = Block.Blocks[getBlockId(x, y, z)];
                    if (block != null && block.material.IsFluid)
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
        int MinX = MathHelper.floor_double(box.minX);
        int MaxX = MathHelper.floor_double(box.maxX + 1.0D);
        int MinY = MathHelper.floor_double(box.minY);
        int MaxY = MathHelper.floor_double(box.maxY + 1.0D);
        int MinZ = MathHelper.floor_double(box.minZ);
        int MaxZ = MathHelper.floor_double(box.maxZ + 1.0D);
        if (isRegionLoaded(MinX, MinY, MinZ, MaxX, MaxY, MaxZ))
        {
            for (int X = MinX; X < MaxX; ++X)
            {
                for (int Y = MinY; Y < MaxY; ++Y)
                {
                    for (int Z = MinZ; Z < MaxZ; ++Z)
                    {
                        int blockId = getBlockId(X, Y, Z);
                        if (blockId == Block.Fire.id || blockId == Block.FlowingLava.id || blockId == Block.Lava.id)
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
        int MinX = MathHelper.floor_double(entityBox.minX);
        int MaxX = MathHelper.floor_double(entityBox.maxX + 1.0D);
        int MinY = MathHelper.floor_double(entityBox.minY);
        int MaxY = MathHelper.floor_double(entityBox.maxY + 1.0D);
        int MinZ = MathHelper.floor_double(entityBox.minZ);
        int MaxZ = MathHelper.floor_double(entityBox.maxZ + 1.0D);
        if (!isRegionLoaded(MinX, MinY, MinZ, MaxX, MaxY, MaxZ))
        {
            return false;
        }
        else
        {
            bool FoundFluid = false;
            Vec3D FlowVector = new Vec3D(0.0D, 0.0D, 0.0D);

            for (int X = MinX; X < MaxX; ++X)
            {
                for (int Y = MinY; Y < MaxY; ++Y)
                {
                    for (int Z = MinZ; Z < MaxZ; ++Z)
                    {
                        Block BlockAt = Block.Blocks[getBlockId(X, Y, Z)];
                        if (BlockAt != null && BlockAt.material == fluidMaterial)
                        {
                            double FluidHeight = (double)((float)(Y + 1) - BlockFluid.getFluidHeightFromMeta(getBlockMeta(X, Y, Z)));
                            if ((double)MaxY >= FluidHeight)
                            {
                                FoundFluid = true;
                                BlockAt.applyVelocity(this, X, Y, Z, entity, FlowVector);
                            }
                        }
                    }
                }
            }

            if (FlowVector.magnitude() > 0.0D)
            {
                FlowVector = FlowVector.normalize();
                double PushFactor = 0.014D;
                entity.velocityX += FlowVector.x * PushFactor;
                entity.velocityY += FlowVector.y * PushFactor;
                entity.velocityZ += FlowVector.z * PushFactor;
            }

            return FoundFluid;
        }
    }

    public bool isMaterialInBox(Box box, Material material)
    {
        int MinX = MathHelper.floor_double(box.minX);
        int MaxX = MathHelper.floor_double(box.maxX + 1.0D);
        int MinY = MathHelper.floor_double(box.minY);
        int MaxY = MathHelper.floor_double(box.maxY + 1.0D);
        int MinZ = MathHelper.floor_double(box.minZ);
        int MaxZ = MathHelper.floor_double(box.maxZ + 1.0D);

        for (int X = MinX; X < MaxX; ++X)
        {
            for (int Y = MinY; Y < MaxY; ++Y)
            {
                for (int Z = MinZ; Z < MaxZ; ++Z)
                {
                    Block BlockAt = Block.Blocks[getBlockId(X, Y, Z)];
                    if (BlockAt != null && BlockAt.material == material)
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
        int MinX = MathHelper.floor_double(box.minX);
        int MaxX = MathHelper.floor_double(box.maxX + 1.0D);
        int MinY = MathHelper.floor_double(box.minY);
        int MaxY = MathHelper.floor_double(box.maxY + 1.0D);
        int MinZ = MathHelper.floor_double(box.minZ);
        int MaxZ = MathHelper.floor_double(box.maxZ + 1.0D);

        for (int X = MinX; X < MaxX; ++X)
        {
            for (int Y = MinY; Y < MaxY; ++Y)
            {
                for (int Z = MinZ; Z < MaxZ; ++Z)
                {
                    Block BlockAt = Block.Blocks[getBlockId(X, Y, Z)];
                    if (BlockAt != null && BlockAt.material == fluid)
                    {
                        int Meta = getBlockMeta(X, Y, Z);
                        double FluidTop = (double)(Y + 1);
                        if (Meta < 8)
                        {
                            FluidTop = (double)(Y + 1) - (double)Meta / 8.0D;
                        }

                        if (FluidTop >= box.minY)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public Explosion createExplosion(Entity EntitySource, double X, double Y, double Z, float Power)
    {
        return createExplosion(EntitySource, X, Y, Z, Power, false);
    }

    public virtual Explosion createExplosion(Entity source, double X, double Y, double Z, float Power, bool IsFire)
    {
        Explosion Explosion = new(this, source, X, Y, Z, Power);
        Explosion.isFlaming = IsFire;
        Explosion.doExplosionA();
        Explosion.doExplosionB(true);
        return Explosion;
    }

    public float getVisibilityRatio(Vec3D vec, Box box)
    {
        double StepX = 1.0D / ((box.maxX - box.minX) * 2.0D + 1.0D);
        double StepY = 1.0D / ((box.maxY - box.minY) * 2.0D + 1.0D);
        double StepZ = 1.0D / ((box.maxZ - box.minZ) * 2.0D + 1.0D);
        int VisibleCount = 0;
        int TotalCount = 0;
        for (float sampleX = 0.0F; sampleX <= 1.0F; sampleX = (float)((double)sampleX + StepX))
        {
            for (float sampleY = 0.0F; sampleY <= 1.0F; sampleY = (float)((double)sampleY + StepY))
            {
                for (float sampleZ = 0.0F; sampleZ <= 1.0F; sampleZ = (float)((double)sampleZ + StepZ))
                {
                    double sampleXCoord = box.minX + (box.maxX - box.minX) * (double)sampleX;
                    double sampleYCoord = box.minY + (box.maxY - box.minY) * (double)sampleY;
                    double sampleZCoord = box.minZ + (box.maxZ - box.minZ) * (double)sampleZ;
                    if (raycast(new Vec3D(sampleXCoord, sampleYCoord, sampleZCoord), vec) == null)
                    {
                        ++VisibleCount;
                    }

                    ++TotalCount;
                }
            }
        }

        if (TotalCount == 0)
        {
            return 0.0F;
        }

        return (float)VisibleCount / (float)TotalCount;
    }

    public void extinguishFire(EntityPlayer player, int x, int y, int z, int direction)
    {
        switch (direction)
        {
            case 0:
                --y;
                break;
            case 1:
                ++y;
                break;
            case 2:
                --z;
                break;
            case 3:
                ++z;
                break;
            case 4:
                --x;
                break;
            case 5:
                ++x;
                break;
            default:
                break;
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

    public BlockEntity getBlockEntity(int X, int Y, int Z)
    {
        Chunk chunk = getChunk(X >> 4, Z >> 4);
        return chunk != null ? chunk.getBlockEntity(X & 15, Y, Z & 15) : null;
    }

    public void setBlockEntity(int X, int Y, int Z, BlockEntity blockEntity)
    {
        if (!blockEntity.isRemoved())
        {
            if (processingDeferred)
            {
                blockEntity.x = X;
                blockEntity.y = Y;
                blockEntity.z = Z;
                blockEntityUpdateQueue.Add(blockEntity);
            }
            else
            {
                blockEntities.Add(blockEntity);
                Chunk chunk = getChunk(X >> 4, Z >> 4);
                if (chunk != null)
                {
                    chunk.setBlockEntity(X & 15, Y, Z & 15, blockEntity);
                }
            }
        }

    }

    public void removeBlockEntity(int X, int Y, int Z)
    {
        BlockEntity blockEntityAt = getBlockEntity(X, Y, Z);
        if (blockEntityAt != null && processingDeferred)
        {
            blockEntityAt.markRemoved();
        }
        else
        {
            if (blockEntityAt != null)
            {
                blockEntities.Remove(blockEntityAt);
            }

            Chunk chunk = getChunk(X >> 4, Z >> 4);
            if (chunk != null)
            {
                chunk.removeBlockEntityAt(X & 15, Y, Z & 15);
            }
        }

    }

    public bool isOpaque(int X, int Y, int Z)
    {
        Block block = Block.Blocks[getBlockId(X, Y, Z)];
        return block == null ? false : block.isOpaque();
    }

    public bool shouldSuffocate(int X, int Y, int Z)
    {
        Block Block = Block.Blocks[getBlockId(X, Y, Z)];
        return Block == null ? false : Block.material.Suffocates && Block.isFullCube();
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

            bool Result;
            try
            {
                int iterations = 500;

                while (lightingQueue.Count > 0)
                {
                    --iterations;
                    if (iterations <= 0)
                    {
                        Result = true;
                        return Result;
                    }

                    int lastIndex = lightingQueue.Count - 1;
                    LightUpdate mcb = lightingQueue[lastIndex];

                    lightingQueue.RemoveAt(lastIndex);
                    mcb.updateLight(this);
                }

                Result = false;
            }
            finally
            {
                --lightingUpdatesCounter;
            }

            return Result;
        }
    }

    public void queueLightUpdate(LightType type, int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
    {
        queueLightUpdate(type, minX, minY, minZ, maxX, maxY, maxZ, true);
    }

    public void queueLightUpdate(LightType type, int minX, int minY, int minZ, int maxX, int maxY, int maxZ, bool bl)
    {
        if (!Dimension.hasCeiling || type != LightType.Sky)
        {
            ++lightingUpdatesScheduled;

            try
            {
                if (lightingUpdatesScheduled == 50)
                {
                    return;
                }

                int centerX = (maxX + minX) / 2;
                int centerZ = (maxZ + minZ) / 2;
                if (isPosLoaded(centerX, 64, centerZ))
                {
                    if (getChunkFromPos(centerX, centerZ).isEmpty())
                    {
                        return;
                    }

                    int queueCount = lightingQueue.Count;
                    int spanCount;
                    Span<LightUpdate> LightingQueueSpan = CollectionsMarshal.AsSpan(lightingQueue);

                    if (bl)
                    {
                        spanCount = 5;
                        if (spanCount > queueCount)
                        {
                            spanCount = queueCount;
                        }

                        for (int i = 0; i < spanCount; ++i)
                        {
                            ref LightUpdate updateRef = ref LightingQueueSpan[lightingQueue.Count - i - 1];
                            if (updateRef.lightType == type && updateRef.expand(minX, minY, minZ, maxX, maxY, maxZ))
                            {
                                return;
                            }
                        }
                    }

                    lightingQueue.Add(new LightUpdate(type, minX, minY, minZ, maxX, maxY, maxZ));
                    spanCount = 1000000;
                    if (lightingQueue.Count > 1000000)
                    {
                        java.lang.System.@out.println("More than " + spanCount + " updates, aborting lighting updates");
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
        int ambient = getAmbientDarkness(1.0F);
        if (ambient != ambientDarkness)
        {
            ambientDarkness = ambient;
        }

    }

    public void allowSpawning(bool allowMonsterSpawning, bool allowMobSpawning)
    {
        spawnHostileMobs = allowMonsterSpawning;
        spawnPeacefulMobs = allowMobSpawning;
    }

    public virtual void Tick(int renderDistance)
    {
        UpdateWeatherCycles();
        long newWorldTime;
        if (canSkipNight())
        {
            bool monstersSpawned = false;
            if (spawnHostileMobs && difficulty >= 1)
            {
                monstersSpawned = NaturalSpawner.spawnMonstersAndWakePlayers(this, players);
            }

            if (!monstersSpawned)
            {
                newWorldTime = properties.WorldTime + 24000L;
                properties.WorldTime = newWorldTime - newWorldTime % 24000L;
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
        int newAmbient = getAmbientDarkness(1.0F);
        if (newAmbient != ambientDarkness)
        {
            ambientDarkness = newAmbient;

            for (int listenerIndex = 0; listenerIndex < eventListeners.Count; ++listenerIndex)
            {
                eventListeners[listenerIndex].notifyAmbientDarknessChanged();
            }
        }
        Profiler.Stop("updateSkylightSubtracted");

        newWorldTime = properties.WorldTime + 1L;
        if (newWorldTime % (long)autosavePeriod == 0L)
        {
            Profiler.PushGroup("autosave");
            saveWithLoadingDisplay(false, (LoadingDisplay)null);
            Profiler.PopGroup();

            chunkSource.markChunksForUnload(renderDistance);
        }

        properties.WorldTime = newWorldTime;
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
        if (!Dimension.hasCeiling)
        {
            if (ticksSinceLightning > 0)
            {
                --ticksSinceLightning;
            }

            int thunderTimeRemaining = properties.ThunderTime;
            if (thunderTimeRemaining <= 0)
            {
                if (properties.IsThundering)
                {
                    properties.ThunderTime = random.nextInt(12000) + 3600;
                }
                else
                {
                    properties.ThunderTime = random.nextInt(168000) + 12000;
                }
            }
            else
            {
                --thunderTimeRemaining;
                properties.ThunderTime = thunderTimeRemaining;
                if (thunderTimeRemaining <= 0)
                {
                    properties.IsThundering = !properties.IsThundering;
                }
            }

            int rainTimeRemaining = properties.RainTime;
            if (rainTimeRemaining <= 0)
            {
                if (properties.IsRaining)
                {
                    properties.RainTime = random.nextInt(12000) + 12000;
                }
                else
                {
                    properties.RainTime = random.nextInt(168000) + 12000;
                }
            }
            else
            {
                --rainTimeRemaining;
                properties.RainTime = rainTimeRemaining;
                if (rainTimeRemaining <= 0)
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
        for (int i = 0; i < players.Count; ++i)
        {
            EntityPlayer Player = players[i];
            var3 = MathHelper.floor_double(Player.x / 16.0D);
            var4 = MathHelper.floor_double(Player.z / 16.0D);
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
                if (var10 == 0 && getBrightness(var7, var9, var8) <= random.nextInt(8) && getBrightness(LightType.Sky, var7, var9, var8) <= 0)
                {
                    EntityPlayer var11 = getClosestPlayer((double)var7 + 0.5D, (double)var9 + 0.5D, (double)var8 + 0.5D, 8.0D);
                    if (var11 != null && var11.getSquaredDistance((double)var7 + 0.5D, (double)var9 + 0.5D, (double)var8 + 0.5D) > 4.0D)
                    {
                        playSound((double)var7 + 0.5D, (double)var9 + 0.5D, (double)var8 + 0.5D, "ambient.cave.cave", 0.7F, 0.8F + random.nextFloat() * 0.2F);
                        soundCounter = random.nextInt(12000) + 6000;
                    }
                }
            }

            if (random.nextInt(100000) == 0 && isRaining() && isThundering())
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
            if (random.nextInt(16) == 0)
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

    public void displayTick(int X, int Y, int Z)
    {
        byte var4 = 16;
        java.util.Random var5 = new();

        for (int var6 = 0; var6 < 1000; ++var6)
        {
            int var7 = X + random.nextInt(var4) - random.nextInt(var4);
            int var8 = Y + random.nextInt(var4) - random.nextInt(var4);
            int var9 = Z + random.nextInt(var4) - random.nextInt(var4);
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
        int var3 = MathHelper.floor_double((box.minX - 2.0D) / 16.0D);
        int var4 = MathHelper.floor_double((box.maxX + 2.0D) / 16.0D);
        int var5 = MathHelper.floor_double((box.minZ - 2.0D) / 16.0D);
        int var6 = MathHelper.floor_double((box.maxZ + 2.0D) / 16.0D);

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
        int var3 = MathHelper.floor_double((box.minX - 2.0D) / 16.0D);
        int var4 = MathHelper.floor_double((box.maxX + 2.0D) / 16.0D);
        int var5 = MathHelper.floor_double((box.minZ - 2.0D) / 16.0D);
        int var6 = MathHelper.floor_double((box.maxZ + 2.0D) / 16.0D);
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

    public void updateBlockEntity(int X, int Y, int Z, BlockEntity blockEntity)
    {
        if (isPosLoaded(X, Y, Z))
        {
            getChunkFromPos(X, Z).markDirty();
        }

        for (int var5 = 0; var5 < eventListeners.Count; ++var5)
        {
            eventListeners[var5].updateBlockEntity(X, Y, Z, blockEntity);
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

    public bool canPlace(int blockId, int X, int Y, int Z, bool fallingBlock, int side)
    {
        int ExistingBlockId = getBlockId(X, Y, Z);
        Block ExistingBlock = Block.Blocks[ExistingBlockId];
        Block BlockToPlace = Block.Blocks[blockId];
        Box? CollisionShape = BlockToPlace.getCollisionShape(this, X, Y, Z);
        if (fallingBlock)
        {
            CollisionShape = null;
        }

        if (CollisionShape != null && !canSpawnEntity(CollisionShape.Value))
        {
            return false;
        }
        else
        {
            if (ExistingBlock == Block.FlowingWater || ExistingBlock == Block.Water || ExistingBlock == Block.FlowingLava || ExistingBlock == Block.Lava || ExistingBlock == Block.Fire || ExistingBlock == Block.Snow)
            {
                ExistingBlock = null;
            }

            return blockId > 0 && ExistingBlock == null && BlockToPlace.canPlaceAt(this, X, Y, Z, side);
        }
    }

    public PathEntity findPath(Entity entity, Entity target, float Range)
    {
        int EntityX = MathHelper.floor_double(entity.x);
        int EntityY = MathHelper.floor_double(entity.y);
        int EntityZ = MathHelper.floor_double(entity.z);
        int RangeMargin = (int)(Range + 16.0F);
        int MinX = EntityX - RangeMargin;
        int MinY = EntityY - RangeMargin;
        int MinZ = EntityZ - RangeMargin;
        int MaxX = EntityX + RangeMargin;
        int MaxY = EntityY + RangeMargin;
        int MaxZ = EntityZ + RangeMargin;
        WorldRegion SearchRegion = new(this, MinX, MinY, MinZ, MaxX, MaxY, MaxZ);
        return (new Pathfinder(SearchRegion)).createEntityPathTo(entity, target, Range);
    }

    public PathEntity findPath(Entity entity, int X, int Y, int Z, float Range)
    {
        int EntityX = MathHelper.floor_double(entity.x);
        int EntityY = MathHelper.floor_double(entity.y);
        int EntityZ = MathHelper.floor_double(entity.z);
        int RangeMargin = (int)(Range + 8.0F);
        int MinX = EntityX - RangeMargin;
        int MinY = EntityY - RangeMargin;
        int MinZ = EntityZ - RangeMargin;
        int MaxX = EntityX + RangeMargin;
        int MaxY = EntityY + RangeMargin;
        int MaxZ = EntityZ + RangeMargin;
        WorldRegion SearchRegion = new(this, MinX, MinY, MinZ, MaxX, MaxY, MaxZ);
        return (new Pathfinder(SearchRegion)).createEntityPathTo(entity, X, Y, Z, Range);
    }

    public bool isStrongPoweringSide(int X, int Y, int Z, int Side)
    {
        int BlockId = getBlockId(X, Y, Z);
        return BlockId == 0 ? false : Block.Blocks[BlockId].isStrongPoweringSide(this, X, Y, Z, Side);
    }

    public bool isStrongPowered(int X, int Y, int Z)
    {
        return isStrongPoweringSide(X, Y - 1, Z, 0) ? true : (isStrongPoweringSide(X, Y + 1, Z, 1) ? true : (isStrongPoweringSide(X, Y, Z - 1, 2) ? true : (isStrongPoweringSide(X, Y, Z + 1, 3) ? true : (isStrongPoweringSide(X - 1, Y, Z, 4) ? true : isStrongPoweringSide(X + 1, Y, Z, 5)))));
    }

    public bool isPoweringSide(int X, int Y, int Z, int Side)
    {
        if (shouldSuffocate(X, Y, Z))
        {
            return isStrongPowered(X, Y, Z);
        }
        else
        {
            int BlockId = getBlockId(X, Y, Z);
            return BlockId == 0 ? false : Block.Blocks[BlockId].isPoweringSide(this, X, Y, Z, Side);
        }
    }

    public bool isPowered(int X, int Y, int Z)
    {
        return isPoweringSide(X, Y - 1, Z, 0) ? true : (isPoweringSide(X, Y + 1, Z, 1) ? true : (isPoweringSide(X, Y, Z - 1, 2) ? true : (isPoweringSide(X, Y, Z + 1, 3) ? true : (isPoweringSide(X - 1, Y, Z, 4) ? true : isPoweringSide(X + 1, Y, Z, 5)))));
    }

    public EntityPlayer getClosestPlayer(Entity entity, double Range)
    {
        return getClosestPlayer(entity.x, entity.y, entity.z, Range);
    }

    public EntityPlayer getClosestPlayer(double X, double Y, double Z, double Range)
    {
        double MinDistance = -1.0D;
        EntityPlayer ClosestPlayer = null;

        for (int i = 0; i < players.Count; ++i)
        {
            EntityPlayer CurrentPlayer = players[i];
            double SquaredDistance = CurrentPlayer.getSquaredDistance(X, Y, Z);
            if ((Range < 0.0D || SquaredDistance < Range * Range) && (MinDistance == -1.0D || SquaredDistance < MinDistance))
            {
                MinDistance = SquaredDistance;
                ClosestPlayer = CurrentPlayer;
            }
        }

        return ClosestPlayer;
    }

    public EntityPlayer getPlayer(string Name)
    {
        for (int PlayerIndex = 0; PlayerIndex < players.Count; ++PlayerIndex)
        {
            if (Name.Equals(players[PlayerIndex].name))
            {
                return players[PlayerIndex];
            }
        }

        return null;
    }

    public void handleChunkDataUpdate(int X, int Y, int Z, int SideX, int SideY, int SideZ, byte[] ChunkData)
    {
        int StartChunkX = X >> 4;
        int StartChunkZ = Z >> 4;
        int EndChunkX = X + SideX - 1 >> 4;
        int EndChunkZ = Z + SideZ - 1 >> 4;
        int DataOffset = 0;
        int MinY = Y;
        int MaxY = Y + SideY;
        if (Y < 0)
        {
            MinY = 0;
        }

        if (MaxY > 128)
        {
            MaxY = 128;
        }

        for (int CurrentChunkX = StartChunkX; CurrentChunkX <= EndChunkX; ++CurrentChunkX)
        {
            int MinLocalX = X - CurrentChunkX * 16;
            int MaxLocalX = X + SideX - CurrentChunkX * 16;
            if (MinLocalX < 0)
            {
                MinLocalX = 0;
            }

            if (MaxLocalX > 16)
            {
                MaxLocalX = 16;
            }

            for (int CurrentChunkZ = StartChunkZ; CurrentChunkZ <= EndChunkZ; ++CurrentChunkZ)
            {
                int MinLocalZ = Z - CurrentChunkZ * 16;
                int MaxLocalZ = Z + SideZ - CurrentChunkZ * 16;
                if (MinLocalZ < 0)
                {
                    MinLocalZ = 0;
                }

                if (MaxLocalZ > 16)
                {
                    MaxLocalZ = 16;
                }

                DataOffset = getChunk(CurrentChunkX, CurrentChunkZ).loadFromPacket(ChunkData, MinLocalX, MinY, MinLocalZ, MaxLocalX, MaxY, MaxLocalZ, DataOffset);
                setBlocksDirty(CurrentChunkX * 16 + MinLocalX, MinY, CurrentChunkZ * 16 + MinLocalZ, CurrentChunkX * 16 + MaxLocalX, MaxY, CurrentChunkZ * 16 + MaxLocalZ);
            }
        }

    }

    public virtual void Disconnect()
    {
    }

    public byte[] getChunkData(int X, int Y, int Z, int sizeX, int SizeY, int SizeZ)
    {
        byte[] ChunkDataBuffer = new byte[sizeX * SizeY * SizeZ * 5 / 2];
        int StartChunkX = X >> 4;
        int StartChunkZ = Z >> 4;
        int EndChunkX = X + sizeX - 1 >> 4;
        int EndChunkZ = Z + SizeZ - 1 >> 4;
        int DataOffset = 0;
        int AdjustedMinY = Y;
        int AdjustedMaxY = Y + SizeY;
        if (Y < 0)
        {
            AdjustedMinY = 0;
        }

        if (AdjustedMaxY > 128)
        {
            AdjustedMaxY = 128;
        }

        for (int CurrentChunkX = StartChunkX; CurrentChunkX <= EndChunkX; CurrentChunkX++)
        {
            int AdjustedMinLocalX = X - CurrentChunkX * 16;
            int AdjustedMaxLocalX = X + sizeX - CurrentChunkX * 16;
            if (AdjustedMinLocalX < 0)
            {
                AdjustedMinLocalX = 0;
            }

            if (AdjustedMaxLocalX > 16)
            {
                AdjustedMaxLocalX = 16;
            }

            for (int CurrentChunkZ = StartChunkZ; CurrentChunkZ <= EndChunkZ; CurrentChunkZ++)
            {
                int AdjustedMinLocalZ = Z - CurrentChunkZ * 16;
                int AdjustedMaxLocalZ = Z + SizeZ - CurrentChunkZ * 16;
                if (AdjustedMinLocalZ < 0)
                {
                    AdjustedMinLocalZ = 0;
                }

                if (AdjustedMaxLocalZ > 16)
                {
                    AdjustedMaxLocalZ = 16;
                }

                DataOffset = getChunk(CurrentChunkX, CurrentChunkZ).toPacket(ChunkDataBuffer, AdjustedMinLocalX, AdjustedMinY, AdjustedMinLocalZ, AdjustedMaxLocalX, AdjustedMaxY, AdjustedMaxLocalZ, DataOffset);
            }
        }

        return ChunkDataBuffer;
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
        long TimeDifference = time - properties.WorldTime;

        Iterator ScheduledUpdateIterator = scheduledUpdateSet.iterator();
        while (ScheduledUpdateIterator.hasNext())
        {
            BlockEvent ScheduledBlockEvent = (BlockEvent)ScheduledUpdateIterator.next();
            ScheduledBlockEvent.ticks += TimeDifference;
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
        int EntityChunkX = MathHelper.floor_double(entity.x / 16.0D);
        int EntityChunkZ = MathHelper.floor_double(entity.z / 16.0D);
        byte LoadRadius = 2;

        for (int CurrentChunkX = EntityChunkX - LoadRadius; CurrentChunkX <= EntityChunkX + LoadRadius; ++CurrentChunkX)
        {
            for (int CurrentChunkZ = EntityChunkZ - LoadRadius; CurrentChunkZ <= EntityChunkZ + LoadRadius; ++CurrentChunkZ)
            {
                getChunk(CurrentChunkX, CurrentChunkZ);
            }
        }

        if (!entities.Contains(entity))
        {
            entities.Add(entity);
        }

    }

    public virtual bool canInteract(EntityPlayer player, int X, int Y, int Z)
    {
        return true;
    }

    public virtual void broadcastEntityEvent(Entity entity, byte @event)
    {
    }

    public void updateEntityLists()
    {
        foreach (Entity EntityToUnload in entitiesToUnload)
        {
            entities.Remove(EntityToUnload);
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

    public virtual void playNoteBlockActionAt(int X, int Y, int Z, int SoundType, int Pitch)
    {
        int var6 = getBlockId(X, Y, Z);
        if (var6 > 0)
        {
            Block.Blocks[var6].onBlockAction(this, X, Y, Z, SoundType, Pitch);
        }

    }

    public WorldProperties getProperties()
    {
        return properties;
    }

    public void updateSleepingPlayers()
    {
        allPlayersSleeping = players.Count > 0;
        foreach (EntityPlayer CurrentPlayer in players) {
            if (!CurrentPlayer.isSleeping())
            {
                allPlayersSleeping = false;
                break;
            }
        }

    }

    protected void afterSkipNight()
    {
        allPlayersSleeping = false;
        foreach (EntityPlayer SleepingPlayer in players) {
            if (SleepingPlayer.isSleeping())
            {
                SleepingPlayer.wakeUp(false, false, true);
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

    public float getThunderGradient(float Delta)
    {
        return (prevThunderingStrength + (thunderingStrength - prevThunderingStrength) * Delta) * getRainGradient(Delta);
    }

    public float getRainGradient(float Delta)
    {
        return prevRainingStrength + (rainingStrength - prevRainingStrength) * Delta;
    }

    public void setRainGradient(float RainGradient)
    {
        prevRainingStrength = RainGradient;
        rainingStrength = RainGradient;
    }

    public bool isThundering()
    {
        return (double)getThunderGradient(1.0F) > 0.9D;
    }

    public bool isRaining()
    {
        return (double)getRainGradient(1.0F) > 0.2D;
    }

    public bool isRaining(int X, int Y, int Z)
    {
        if (!isRaining())
        {
            return false;
        }
        else if (!hasSkyLight(X, Y, Z))
        {
            return false;
        }
        else if (getTopSolidBlockY(X, Z) > Y)
        {
            return false;
        }
        else
        {
            Biome var4 = getBiomeSource().GetBiome(X, Z);
            return var4.GetEnableSnow() ? false : var4.CanSpawnLightningBolt();
        }
    }

    public void setState(string Id, PersistentState State)
    {
        persistentStateManager.setData(Id, State);
    }

    public PersistentState getOrCreateState(Class @class, string Id)
    {
        return persistentStateManager.loadData(@class, Id);
    }

    public int getIdCount(string Id)
    {
        return persistentStateManager.getUniqueDataId(Id);
    }

    public void worldEvent(int @event, int X, int Y, int Z, int Data)
    {
        worldEvent(null, @event, X, Y, Z, Data);
    }

    public void worldEvent(EntityPlayer player, int @event, int X, int Y, int Z, int Data)
    {
        for (int var7 = 0; var7 < eventListeners.Count; ++var7)
        {
            eventListeners[var7].worldEvent(player, @event, X, Y, Z, Data);
        }

    }
}