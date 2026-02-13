using BetaSharp.Blocks.Entities;
using BetaSharp.Entities;
using BetaSharp.Network.Packets;
using BetaSharp.Network.Packets.Play;
using BetaSharp.Network.Packets.S2CPlay;
using BetaSharp.Server.Network;
using BetaSharp.Server.Worlds;
using BetaSharp.Util;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using BetaSharp.Worlds.Dimensions;
using java.util.logging;

namespace BetaSharp.Server;

public class PlayerManager
{
    public static Logger LOGGER = Logger.getLogger("Minecraft");
    public List<ServerPlayerEntity> players = [];
    private readonly MinecraftServer server;
    private readonly ChunkMap[] chunkMaps;
    private readonly int maxPlayerCount;
    protected readonly HashSet<string> bannedPlayers = [];
    protected readonly HashSet<string> bannedIps = [];
    protected readonly HashSet<string> ops = [];
    protected readonly HashSet<string> whitelist = [];
    private PlayerSaveHandler saveHandler;
    private readonly bool whitelistEnabled;

    public PlayerManager(MinecraftServer server)
    {
        chunkMaps = new ChunkMap[2];
        this.server = server;
        int var2 = server.config.GetViewDistance(10);
        chunkMaps[0] = new ChunkMap(server, 0, var2);
        chunkMaps[1] = new ChunkMap(server, -1, var2);
        maxPlayerCount = server.config.GetMaxPlayers(20);
        whitelistEnabled = server.config.GetWhiteList(false);
    }

    public void saveAllPlayers(ServerWorld[] world)
    {
        saveHandler = world[0].getWorldStorage().getPlayerSaveHandler();
    }

    public void updatePlayerAfterDimensionChange(ServerPlayerEntity player)
    {
        chunkMaps[0].removePlayer(player);
        chunkMaps[1].removePlayer(player);
        getChunkMap(player.dimensionId).addPlayer(player);
        ServerWorld var2 = server.getWorld(player.dimensionId);
        var2.chunkCache.loadChunk((int)player.x >> 4, (int)player.z >> 4);
    }

    public int getBlockViewDistance()
    {
        return chunkMaps[0].getBlockViewDistance();
    }

    private ChunkMap getChunkMap(int dimensionId)
    {
        return dimensionId == -1 ? chunkMaps[1] : chunkMaps[0];
    }

    public void loadPlayerData(ServerPlayerEntity player)
    {
        saveHandler.loadPlayerData(player);
    }

    public void addPlayer(ServerPlayerEntity player)
    {
        players.Add(player);
        ServerWorld var2 = server.getWorld(player.dimensionId);
        var2.chunkCache.loadChunk((int)player.x >> 4, (int)player.z >> 4);

        while (var2.getEntityCollisions(player, player.boundingBox).Count != 0)
        {
            player.setPosition(player.x, player.y + 1.0, player.z);
        }

        var2.spawnEntity(player);
        getChunkMap(player.dimensionId).addPlayer(player);
    }

    public void updatePlayerChunks(ServerPlayerEntity player)
    {
        getChunkMap(player.dimensionId).updatePlayerChunks(player);
    }

    public void disconnect(ServerPlayerEntity player)
    {
        saveHandler.savePlayerData(player);
        server.getWorld(player.dimensionId).remove(player);
        players.Remove(player);
        getChunkMap(player.dimensionId).removePlayer(player);
    }

    public ServerPlayerEntity connectPlayer(ServerLoginNetworkHandler loginNetworkHandler, string name)
    {
        if (bannedPlayers.Contains(name.Trim().ToLower()))
        {
            loginNetworkHandler.disconnect("You are banned from this server!");
            return null;
        }
        else if (!isWhitelisted(name))
        {
            loginNetworkHandler.disconnect("You are not white-listed on this server!");
            return null;
        }
        else
        {
            string var3 = loginNetworkHandler.connection.getAddress().toString();
            var3 = var3.Substring(var3.IndexOf("/") + 1);
            var3 = var3.Substring(0, var3.IndexOf(":"));
            if (bannedIps.Contains(var3))
            {
                loginNetworkHandler.disconnect("Your IP address is banned from this server!");
                return null;
            }
            else if (players.Count >= maxPlayerCount)
            {
                loginNetworkHandler.disconnect("The server is full!");
                return null;
            }
            else
            {
                for (int var4 = 0; var4 < players.Count; var4++)
                {
                    ServerPlayerEntity var5 = players[var4];
                    if (var5.name.EqualsIgnoreCase(name))
                    {
                        var5.networkHandler.disconnect("You logged in from another location");
                    }
                }

                return new ServerPlayerEntity(server, server.getWorld(0), name, new ServerPlayerInteractionManager(server.getWorld(0)));
            }
        }
    }

    public ServerPlayerEntity respawnPlayer(ServerPlayerEntity player, int dimensionId)
    {
        server.getEntityTracker(player.dimensionId).removeListener(player);
        server.getEntityTracker(player.dimensionId).onEntityRemoved(player);
        getChunkMap(player.dimensionId).removePlayer(player);
        players.Remove(player);
        server.getWorld(player.dimensionId).serverRemove(player);
        Vec3i var3 = player.getSpawnPos();
        player.dimensionId = dimensionId;
        ServerPlayerEntity var4 = new(
            server, server.getWorld(player.dimensionId), player.name, new ServerPlayerInteractionManager(server.getWorld(player.dimensionId))
        )
        {
            id = player.id,
            networkHandler = player.networkHandler
        };
        ServerWorld var5 = server.getWorld(player.dimensionId);
        if (var3 != null)
        {
            Vec3i var6 = EntityPlayer.findRespawnPosition(server.getWorld(player.dimensionId), var3);
            if (var6 != null)
            {
                var4.setPositionAndAnglesKeepPrevAngles(var6.x + 0.5F, var6.y + 0.1F, var6.z + 0.5F, 0.0F, 0.0F);
                var4.setSpawnPos(var3);
            }
            else
            {
                var4.networkHandler.sendPacket(new GameStateChangeS2CPacket(0));
            }
        }

        var5.chunkCache.loadChunk((int)var4.x >> 4, (int)var4.z >> 4);

        while (var5.getEntityCollisions(var4, var4.boundingBox).Count != 0)
        {
            var4.setPosition(var4.x, var4.y + 1.0, var4.z);
        }

        var4.networkHandler.sendPacket(new PlayerRespawnPacket((sbyte)var4.dimensionId));
        var4.networkHandler.teleport(var4.x, var4.y, var4.z, var4.yaw, var4.pitch);
        sendWorldInfo(var4, var5);
        getChunkMap(var4.dimensionId).addPlayer(var4);
        var5.spawnEntity(var4);
        players.Add(var4);
        var4.initScreenHandler();
        var4.m_41544513();
        return var4;
    }

    public void changePlayerDimension(ServerPlayerEntity player)
    {
        ServerWorld var2 = server.getWorld(player.dimensionId);
        sbyte var3 = 0;
        if (player.dimensionId == -1)
        {
            var3 = 0;
        }
        else
        {
            var3 = -1;
        }

        player.dimensionId = var3;
        ServerWorld var4 = server.getWorld(player.dimensionId);
        player.networkHandler.sendPacket(new PlayerRespawnPacket((sbyte)player.dimensionId));
        var2.serverRemove(player);
        player.dead = false;
        double var5 = player.x;
        double var7 = player.z;
        double var9 = 8.0;
        if (player.dimensionId == -1)
        {
            var5 /= var9;
            var7 /= var9;
            player.setPositionAndAnglesKeepPrevAngles(var5, player.y, var7, player.yaw, player.pitch);
            if (player.isAlive())
            {
                var2.updateEntity(player, false);
            }
        }
        else
        {
            var5 *= var9;
            var7 *= var9;
            player.setPositionAndAnglesKeepPrevAngles(var5, player.y, var7, player.yaw, player.pitch);
            if (player.isAlive())
            {
                var2.updateEntity(player, false);
            }
        }

        if (player.isAlive())
        {
            var4.spawnEntity(player);
            player.setPositionAndAnglesKeepPrevAngles(var5, player.y, var7, player.yaw, player.pitch);
            var4.updateEntity(player, false);
            var4.chunkCache.forceLoad = true;
            new PortalForcer().moveToPortal(var4, player);
            var4.chunkCache.forceLoad = false;
        }

        updatePlayerAfterDimensionChange(player);
        player.networkHandler.teleport(player.x, player.y, player.z, player.yaw, player.pitch);
        player.setWorld(var4);
        sendWorldInfo(player, var4);
        sendPlayerStatus(player);
    }

    public void updateAllChunks()
    {
        for (int var1 = 0; var1 < chunkMaps.Length; var1++)
        {
            chunkMaps[var1].updateChunks();
        }
    }

    public void markDirty(int x, int y, int z, int dimensionId)
    {
        getChunkMap(dimensionId).markBlockForUpdate(x, y, z);
    }

    public void sendToAll(Packet packet)
    {
        for (int var2 = 0; var2 < players.Count; var2++)
        {
            ServerPlayerEntity var3 = players[var2];
            var3.networkHandler.sendPacket(packet);
        }
    }

    public void sendToDimension(Packet packet, int dimensionId)
    {
        for (int var3 = 0; var3 < players.Count; var3++)
        {
            ServerPlayerEntity var4 = players[var3];
            if (var4.dimensionId == dimensionId)
            {
                var4.networkHandler.sendPacket(packet);
            }
        }
    }

    public string getPlayerList()
    {
        string var1 = "";

        for (int var2 = 0; var2 < players.Count; var2++)
        {
            if (var2 > 0)
            {
                var1 += ", ";
            }

            var1 += players[var2].name;
        }

        return var1;
    }

    public void banPlayer(string name)
    {
        bannedPlayers.Add(name.ToLower());
        saveBannedPlayers();
    }

    public void unbanPlayer(string name)
    {
        bannedPlayers.Remove(name.ToLower());
        saveBannedPlayers();
    }

    protected virtual void loadBannedPlayers()
    {
    }

    protected virtual void saveBannedPlayers()
    {
    }

    public void banIp(string ip)
    {
        bannedIps.Add(ip.ToLower());
        saveBannedIps();
    }

    public void unbanIp(string ip)
    {
        bannedIps.Remove(ip.ToLower());
        saveBannedIps();
    }

    protected virtual void loadBannedIps()
    {
    }

    protected virtual void saveBannedIps()
    {
    }

    public void addToOperators(string name)
    {
        ops.Add(name.ToLower());
        saveOperators();
    }

    public void removeFromOperators(string name)
    {
        ops.Remove(name.ToLower());
        saveOperators();
    }

    protected virtual void loadOperators()
    {
    }

    protected virtual void saveOperators()
    {
    }

    protected virtual void loadWhitelist()
    {
    }

    protected virtual void saveWhitelist()
    {
    }

    public bool isWhitelisted(string name)
    {
        name = name.Trim().ToLower();
        return !whitelistEnabled || ops.Contains(name) || whitelist.Contains(name);
    }

    public bool isOperator(string name)
    {
        return ops.Contains(name.Trim().ToLower());
    }

    public ServerPlayerEntity getPlayer(string name)
    {
        for (int var2 = 0; var2 < players.Count; var2++)
        {
            ServerPlayerEntity var3 = players[var2];
            if (var3.name.EqualsIgnoreCase(name))
            {
                return var3;
            }
        }

        return null;
    }

    public void messagePlayer(string name, string message)
    {
        ServerPlayerEntity var3 = getPlayer(name);
        if (var3 != null)
        {
            var3.networkHandler.sendPacket(new ChatMessagePacket(message));
        }
    }

    public void sendToAround(double x, double y, double z, double range, int dimensionId, Packet packet)
    {
        sendToAround(null, x, y, z, range, dimensionId, packet);
    }

    public void sendToAround(EntityPlayer player, double x, double y, double z, double range, int dimensionId, Packet packet)
    {
        for (int var12 = 0; var12 < players.Count; var12++)
        {
            ServerPlayerEntity var13 = players[var12];
            if (var13 != player && var13.dimensionId == dimensionId)
            {
                double var14 = x - var13.x;
                double var16 = y - var13.y;
                double var18 = z - var13.z;
                if (var14 * var14 + var16 * var16 + var18 * var18 < range * range)
                {
                    var13.networkHandler.sendPacket(packet);
                }
            }
        }
    }

    public void broadcast(string message)
    {
        ChatMessagePacket var2 = new(message);

        for (int var3 = 0; var3 < players.Count; var3++)
        {
            ServerPlayerEntity var4 = players[var3];
            if (isOperator(var4.name))
            {
                var4.networkHandler.sendPacket(var2);
            }
        }
    }

    public bool sendPacket(string player, Packet packet)
    {
        ServerPlayerEntity var3 = getPlayer(player);
        if (var3 != null)
        {
            var3.networkHandler.sendPacket(packet);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void savePlayers()
    {
        for (int var1 = 0; var1 < players.Count; var1++)
        {
            saveHandler.savePlayerData(players[var1]);
        }
    }

    public void updateBlockEntity(int x, int y, int z, BlockEntity blockentity)
    {
    }

    public void addToWhitelist(string name)
    {
        whitelist.Add(name);
        saveWhitelist();
    }

    public void removeFromWhitelist(string name)
    {
        whitelist.Remove(name);
        saveWhitelist();
    }

    public HashSet<string> getWhitelist()
    {
        return whitelist;
    }

    public void reloadWhitelist()
    {
        loadWhitelist();
    }

    public void sendWorldInfo(ServerPlayerEntity player, ServerWorld world)
    {
        player.networkHandler.sendPacket(new WorldTimeUpdateS2CPacket(world.getTime()));
        if (world.isRaining())
        {
            player.networkHandler.sendPacket(new GameStateChangeS2CPacket(1));
        }
    }

    public void sendPlayerStatus(ServerPlayerEntity player)
    {
        player.onContentsUpdate(player.playerScreenHandler);
        player.markHealthDirty();
    }
}