using betareborn.Network.Packets.S2CPlay;
using betareborn.Server.Commands;
using betareborn.Server.Entities;
using betareborn.Server.Internal;
using betareborn.Server.Network;
using betareborn.Server.Worlds;
using betareborn.Util;
using betareborn.Util.Maths;
using betareborn.Worlds;
using betareborn.Worlds.Storage;
using java.lang;
using java.util;
using java.util.logging;

namespace betareborn.Server
{
    public abstract class MinecraftServer : Runnable, CommandOutput
    {
        public static Logger LOGGER = Logger.getLogger("Minecraft");
        public HashMap GIVE_COMMANDS_COOLDOWNS = [];
        public ConnectionListener connections;
        public IServerConfiguration config;
        public ServerWorld[] worlds;
        public PlayerManager playerManager;
        private ServerCommandHandler commandHandler;
        public bool running = true;
        public bool stopped = false;
        private int ticks = 0;
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

        public MinecraftServer(IServerConfiguration config)
        {
            this.config = config;
        }

        protected virtual bool Init()
        {
            commandHandler = new ServerCommandHandler(this);
            ServerLog.init();

            onlineMode = config.GetOnlineMode(true);
            spawnAnimals = config.GetSpawnAnimals(true);
            pvpEnabled = config.GetPvpEnabled(true);
            flightEnabled = config.GetAllowFlight(false);

            playerManager = CreatePlayerManager();
            entityTrackers[0] = new EntityTracker(this, 0);
            entityTrackers[1] = new EntityTracker(this, -1);
            long var5 = java.lang.System.nanoTime();
            string var7 = config.GetLevelName("world");
            string var8 = config.GetLevelSeed("");
            long var9 = new java.util.Random().nextLong();
            if (var8.Length > 0)
            {
                try
                {
                    var9 = Long.parseLong(var8);
                }
                catch (NumberFormatException)
                {
                    var9 = var8.GetHashCode();
                }
            }

            LOGGER.info("Preparing level \"" + var7 + "\"");
            loadWorld(new RegionWorldStorageSource(getFile(".")), var7, var9);

            if (logHelp)
            {
                LOGGER.info("Done (" + (java.lang.System.nanoTime() - var5) + "ns)! For help, type \"help\" or \"?\"");
            }

            return true;
        }

        private void loadWorld(WorldStorageSource storageSource, string worldDir, long seed)
        {
            worlds = new ServerWorld[2];
            RegionWorldStorage var5 = new RegionWorldStorage(getFile("."), worldDir, true);

            for (int var6 = 0; var6 < worlds.Length; var6++)
            {
                if (var6 == 0)
                {
                    worlds[var6] = new ServerWorld(this, var5, worldDir, var6 == 0 ? 0 : -1, seed);
                }
                else
                {
                    worlds[var6] = new ReadOnlyServerWorld(this, var5, worldDir, var6 == 0 ? 0 : -1, seed, worlds[0]);
                }

                worlds[var6].addWorldAccess(new ServerWorldEventListener(this, worlds[var6]));
                worlds[var6].difficulty = config.GetSpawnMonsters(true) ? 1 : 0;
                worlds[var6].allowSpawning(config.GetSpawnMonsters(true), spawnAnimals);
                playerManager.saveAllPlayers(worlds);
            }

            short var18 = 196;
            long var7 = java.lang.System.currentTimeMillis();

            for (int var9 = 0; var9 < worlds.Length; var9++)
            {
                LOGGER.info("Preparing start region for level " + var9);
                if (var9 == 0 || config.GetAllowNether(true))
                {
                    ServerWorld var10 = worlds[var9];
                    Vec3i var11 = var10.getSpawnPos();

                    for (int var12 = -var18; var12 <= var18 && running; var12 += 16)
                    {
                        for (int var13 = -var18; var13 <= var18 && running; var13 += 16)
                        {
                            long var14 = java.lang.System.currentTimeMillis();
                            if (var14 < var7)
                            {
                                var7 = var14;
                            }

                            if (var14 > var7 + 1000L)
                            {
                                int var16 = (var18 * 2 + 1) * (var18 * 2 + 1);
                                int var17 = (var12 + var18) * (var18 * 2 + 1) + var13 + 1;
                                logProgress("Preparing spawn area", var17 * 100 / var16);
                                var7 = var14;
                            }

                            var10.chunkCache.loadChunk(var11.x + var12 >> 4, var11.z + var13 >> 4);

                            while (var10.doLightingUpdates() && running)
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
            LOGGER.info(progressType + ": " + progress + "%");
        }

        private void clearProgress()
        {
            progressMessage = null;
            progress = 0;
        }

        private void saveWorlds()
        {
            LOGGER.info("Saving chunks");

            for (int var1 = 0; var1 < worlds.Length; var1++)
            {
                ServerWorld var2 = worlds[var1];
                var2.saveWithLoadingDisplay(true, null);
                var2.forceSave();
            }
        }

        private void shutdown()
        {
            if (stopped)
            {
                return;
            }

            LOGGER.info("Stopping server");

            if (playerManager != null)
            {
                playerManager.savePlayers();
            }

            for (int var1 = 0; var1 < worlds.Length; var1++)
            {
                ServerWorld var2 = worlds[var1];
                if (var2 != null)
                {
                    saveWorlds();
                }
            }

            while (AsyncIO.isBlocked())
            {
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
                    long var1 = java.lang.System.currentTimeMillis();
                    _lastTpsTime = var1;
                    _ticksThisSecond = 0;

                    for (long var3 = 0L; running; java.lang.Thread.sleep(1L))
                    {
                        long var5 = java.lang.System.currentTimeMillis();
                        long var7 = var5 - var1;
                        if (var7 > 2000L)
                        {
                            LOGGER.warning("Can't keep up! Did the system time change, or is the server overloaded?");
                            var7 = 2000L;
                        }

                        if (var7 < 0L)
                        {
                            LOGGER.warning("Time ran backwards! Did the system time change?");
                            var7 = 0L;
                        }

                        var3 += var7;
                        var1 = var5;
                        if (worlds[0].canSkipNight())
                        {
                            tick();
                            _ticksThisSecond++;
                            var3 = 0L;
                        }
                        else
                        {
                            while (var3 > 50L)
                            {
                                var3 -= 50L;
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
                        catch (InterruptedException var57)
                        {
                            var57.printStackTrace();
                        }
                    }
                }
            }
            catch (System.Exception var58)
            {
                Console.WriteLine(var58);
                LOGGER.log(Level.SEVERE, "Unexpected exception", var58);

                while (running)
                {
                    runPendingCommands();

                    try
                    {
                        java.lang.Thread.sleep(10L);
                    }
                    catch (InterruptedException var56)
                    {
                        var56.printStackTrace();
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
                catch (Throwable var54)
                {
                    var54.printStackTrace();
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
            ArrayList var1 = [];

            var keys = GIVE_COMMANDS_COOLDOWNS.keySet();
            var iter = keys.iterator();
            while (iter.hasNext())
            {
                string var3 = (string)iter.next();
                int var4 = (int)GIVE_COMMANDS_COOLDOWNS.get(var3);
                if (var4 > 0)
                {
                    GIVE_COMMANDS_COOLDOWNS.put(var3, var4 - 1);
                }
                else
                {
                    var1.add(var3);
                }
            }

            for (int var6 = 0; var6 < var1.size(); var6++)
            {
                GIVE_COMMANDS_COOLDOWNS.remove(var1.get(var6));
            }

            AsyncIO.tick();
            ticks++;

            for (int var7 = 0; var7 < worlds.Length; var7++)
            {
                if (var7 == 0 || config.GetAllowNether(true))
                {
                    ServerWorld var10 = worlds[var7];
                    if (ticks % 20 == 0)
                    {
                        playerManager.sendToDimension(new WorldTimeUpdateS2CPacket(var10.getTime()), var10.dimension.id);
                    }

                    var10.tick(-1);

                    while (var10.doLightingUpdates())
                    {
                    }

                    var10.tickEntities();
                }
            }

            if (connections != null)
            {
                connections.tick();
            }
            playerManager.updateAllChunks();

            for (int var8 = 0; var8 < entityTrackers.Length; var8++)
            {
                entityTrackers[var8].tick();
            }

            for (int var9 = 0; var9 < tickables.size(); var9++)
            {
                ((Tickable)tickables.get(var9)).tick();
            }

            try
            {
                runPendingCommands();
            }
            catch (java.lang.Exception var5)
            {
                LOGGER.log(Level.WARNING, "Unexpected exception while parsing console command", (Throwable)var5);
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
                Command var1 = (Command)pendingCommands.remove(0);
                commandHandler.ExecuteCommand(var1);
            }
        }

        public void addTickable(Tickable tickable)
        {
            tickables.add(tickable);
        }

        public abstract java.io.File getFile(string path);

        public void SendMessage(string message)
        {
            LOGGER.info(message);
        }

        public void Warn(string message)
        {
            LOGGER.warning(message);
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
}
