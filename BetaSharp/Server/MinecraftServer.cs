using BetaSharp.Network.Packets.S2CPlay;
using BetaSharp.Server.Commands;
using BetaSharp.Server.Entities;
using BetaSharp.Server.Internal;
using BetaSharp.Server.Network;
using BetaSharp.Server.Worlds;
using BetaSharp.Util;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using BetaSharp.Worlds.Storage;
using java.lang;
using java.util;

namespace BetaSharp.Server;

public abstract class MinecraftServer : Runnable, CommandOutput
{
    public HashMap GIVE_COMMANDS_COOLDOWNS = [];
    public ConnectionListener connections;
    public IServerConfiguration config;
    public ServerWorld[] worlds;
    public PlayerManager playerManager;
    private ServerCommandHandler commandHandler;
    public bool running = true;
    public bool stopped;
    private int ticks;
    public string progressMessage;
    public int progress;
    private List tickables = new ArrayList();
    private List pendingCommands = Collections.synchronizedList(new ArrayList());
    public EntityTracker[] entityTrackers = new EntityTracker[2];
    public bool onlineMode;
    public bool spawnAnimals;
    public bool pvpEnabled;
    public bool flightEnabled;
    protected bool logHelp = true;

    private readonly Lock _tpsLock = new();
    private long _lastTpsTime;
    private int _ticksThisSecond;
    private float _currentTps;

    private volatile bool _isPaused;

    public float Tps
    {
        get
        {
            lock (_tpsLock)
            {
                return _currentTps;
            }
        }
    }

    public void SetPaused(bool paused)
    {
        _isPaused = paused;
    }

    public MinecraftServer(IServerConfiguration config)
    {
        this.config = config;
    }

    protected virtual bool Init()
    {
        commandHandler = new ServerCommandHandler(this);

        onlineMode = config.GetOnlineMode(true);
        spawnAnimals = config.GetSpawnAnimals(true);
        pvpEnabled = config.GetPvpEnabled(true);
        flightEnabled = config.GetAllowFlight(false);

        playerManager = CreatePlayerManager();
        entityTrackers[0] = new EntityTracker(this, 0);
        entityTrackers[1] = new EntityTracker(this, -1);
        long startTime = java.lang.System.nanoTime();
        string worldName = config.GetLevelName("world");
        string seedString = config.GetLevelSeed("");
        long seed = new java.util.Random().nextLong();
        if (seedString.Length > 0)
        {
            try
            {
                seed = Long.parseLong(seedString);
            }
            catch (NumberFormatException)
            {
                // Java based string hashing
                int hash = 0;
                foreach (char c in seedString)
                {
                    hash = 31 * hash + c;
                }
                seed = hash;
            }
        }

        Log.Info($"Preparing level \"{worldName}\"");
        loadWorld(new RegionWorldStorageSource(getFile(".")), worldName, seed);

        if (logHelp)
        {
            Log.Info($"Done ({java.lang.System.nanoTime() - startTime}ns)! For help, type \"help\" or \"?\"");
        }

        return true;
    }

    private void loadWorld(WorldStorageSource storageSource, string worldDir, long seed)
    {
        worlds = new ServerWorld[2];
        RegionWorldStorage worldStorage = new RegionWorldStorage(getFile("."), worldDir, true);

        for (int i = 0; i < worlds.Length; i++)
        {
            if (i == 0)
            {
                worlds[i] = new ServerWorld(this, worldStorage, worldDir, i == 0 ? 0 : -1, seed);
            }
            else
            {
                worlds[i] = new ReadOnlyServerWorld(this, worldStorage, worldDir, i == 0 ? 0 : -1, seed, worlds[0]);
            }

            worlds[i].addWorldAccess(new ServerWorldEventListener(this, worlds[i]));
            worlds[i].difficulty = config.GetSpawnMonsters(true) ? 1 : 0;
            worlds[i].allowSpawning(config.GetSpawnMonsters(true), spawnAnimals);
            playerManager.saveAllPlayers(worlds);
        }

        short startRegionSize = 196;
        long lastTimeLogged = java.lang.System.currentTimeMillis();

        for (int i = 0; i < worlds.Length; i++)
        {
            Log.Info($"Preparing start region for level {i}");
            if (i == 0 || config.GetAllowNether(true))
            {
                ServerWorld world = worlds[i];
                Vec3i spawnPos = world.getSpawnPos();

                for (int x = -startRegionSize; x <= startRegionSize && running; x += 16)
                {
                    for (int z = -startRegionSize; z <= startRegionSize && running; z += 16)
                    {
                        long currentTime = java.lang.System.currentTimeMillis();
                        if (currentTime < lastTimeLogged)
                        {
                            lastTimeLogged = currentTime;
                        }

                        if (currentTime > lastTimeLogged + 1000L)
                        {
                            int total = (startRegionSize * 2 + 1) * (startRegionSize * 2 + 1);
                            int complete = (x + startRegionSize) * (startRegionSize * 2 + 1) + z + 1;
                            logProgress("Preparing spawn area", complete * 100 / total);
                            lastTimeLogged = currentTime;
                        }

                        world.chunkCache.loadChunk(spawnPos.x + x >> 4, spawnPos.z + z >> 4);

                        while (world.doLightingUpdates() && running)
                        {
                        }
                    }
                }
            }
        }

        clearProgress();
    }

    private void logProgress(string progressType, int progress)
    {
        progressMessage = progressType;
        this.progress = progress;
        Log.Info($"{progressType}: {progress}%");
    }

    private void clearProgress()
    {
        progressMessage = null;
        progress = 0;
    }

    private void saveWorlds()
    {
        Log.Info("Saving chunks");

        foreach (ServerWorld world in worlds)
        {
            world.saveWithLoadingDisplay(true, null);
            world.forceSave();
        }
    }

    private void shutdown()
    {
        if (stopped)
        {
            return;
        }

        Log.Info("Stopping server");

        if (playerManager != null)
        {
            playerManager.savePlayers();
        }

        foreach (ServerWorld world in worlds)
        {
            if (world != null)
            {
                saveWorlds();
            }
        }
    }

    public void stop()
    {
        running = false;
    }

    public void run()
    {
        try
        {
            if (Init())
            {
                long lastTime = java.lang.System.currentTimeMillis();
                long accumulatedTime = 0L;
                _lastTpsTime = lastTime;
                _ticksThisSecond = 0;

                while (running)
                {
                    long currentTime = java.lang.System.currentTimeMillis();
                    long tickLength = currentTime - lastTime;
                    if (tickLength > 2000L)
                    {
                        Log.Warn("Can't keep up! Did the system time change, or is the server overloaded?");
                        tickLength = 2000L;
                    }

                    if (tickLength < 0L)
                    {
                        Log.Warn("Time ran backwards! Did the system time change?");
                        tickLength = 0L;
                    }

                    accumulatedTime += tickLength;
                    lastTime = currentTime;

                    if (_isPaused)
                    {
                        accumulatedTime = 0L;
                        lock (_tpsLock)
                        {
                            _currentTps = 0.0f;
                        }
                        continue;
                    }

                    if (worlds[0].canSkipNight())
                    {
                        tick();
                        _ticksThisSecond++;
                        accumulatedTime = 0L;
                    }
                    else
                    {
                        while (accumulatedTime > 50L)
                        {
                            accumulatedTime -= 50L;
                            tick();
                            _ticksThisSecond++;
                        }
                    }

                    long tpsNow = java.lang.System.currentTimeMillis();
                    long tpsElapsed = tpsNow - _lastTpsTime;
                    if (tpsElapsed >= 1000L)
                    {
                        lock (_tpsLock)
                        {
                            _currentTps = _ticksThisSecond * 1000.0f / tpsElapsed;
                        }
                        _ticksThisSecond = 0;
                        _lastTpsTime = tpsNow;
                    }

                    java.lang.Thread.sleep(1L);
                }
            }
            else
            {
                while (running)
                {
                    runPendingCommands();

                    try
                    {
                        java.lang.Thread.sleep(10L);
                    }
                    catch (InterruptedException ex)
                    {
                        ex.printStackTrace();
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Log.Error(ex);
            Log.Error("Unexpected exception");

            while (running)
            {
                runPendingCommands();

                try
                {
                    java.lang.Thread.sleep(10L);
                }
                catch (InterruptedException interruptedEx)
                {
                    interruptedEx.printStackTrace();
                }
            }
        }
        finally
        {
            try
            {
                shutdown();
                stopped = true;
            }
            catch (Throwable ex)
            {
                ex.printStackTrace();
            }
            finally
            {
                if (this is not InternalServer)
                {
                    Environment.Exit(0);
                }
            }
        }
    }

    private void tick()
    {
        ArrayList completeCooldowns = [];

        var keys = GIVE_COMMANDS_COOLDOWNS.keySet();
        var iter = keys.iterator();
        while (iter.hasNext())
        {
            string key = (string)iter.next();
            int cooldown = (int)GIVE_COMMANDS_COOLDOWNS.get(key);
            if (cooldown > 0)
            {
                GIVE_COMMANDS_COOLDOWNS.put(key, cooldown - 1);
            }
            else
            {
                completeCooldowns.add(key);
            }
        }

        for (int i = 0; i < completeCooldowns.size(); i++)
        {
            GIVE_COMMANDS_COOLDOWNS.remove(completeCooldowns.get(i));
        }

        ticks++;

        for (int i = 0; i < worlds.Length; i++)
        {
            if (i == 0 || config.GetAllowNether(true))
            {
                ServerWorld world = worlds[i];
                if (ticks % 20 == 0)
                {
                    playerManager.sendToDimension(new WorldTimeUpdateS2CPacket(world.getTime()), world.dimension.id);
                }

                world.Tick();

                while (world.doLightingUpdates())
                {
                }

                world.tickEntities();
            }
        }

        if (connections != null)
        {
            connections.Tick();
        }
        playerManager.updateAllChunks();

        foreach (EntityTracker t in entityTrackers)
        {
            t.tick();
        }

        for (int i = 0; i < tickables.size(); i++)
        {
            ((Tickable)tickables.get(i)).tick();
        }

        try
        {
            runPendingCommands();
        }
        catch (java.lang.Exception ex)
        {
            Log.Warn($"Unexpected exception while parsing console command: {ex}");
        }
    }

    public void queueCommands(string str, CommandOutput cmd)
    {
        pendingCommands.add(new Command(str, cmd));
    }

    public void runPendingCommands()
    {
        while (pendingCommands.size() > 0)
        {
            commandHandler.ExecuteCommand((Command)pendingCommands.remove(0));
        }
    }

    public void addTickable(Tickable tickable)
    {
        tickables.add(tickable);
    }

    public abstract java.io.File getFile(string path);

    public void SendMessage(string message)
    {
        Log.Info(message);
    }

    public void Warn(string message)
    {
        Log.Warn(message);
    }

    public string GetName()
    {
        return "CONSOLE";
    }

    public ServerWorld getWorld(int dimensionId)
    {
        return dimensionId == -1 ? worlds[1] : worlds[0];
    }

    public EntityTracker getEntityTracker(int dimensionId)
    {
        return dimensionId == -1 ? entityTrackers[1] : entityTrackers[0];
    }
    protected virtual PlayerManager CreatePlayerManager()
    {
        return new PlayerManager(this);
    }

}
