using BetaSharp.Blocks;
using BetaSharp.Blocks.Entities;
using BetaSharp.Entities;
using BetaSharp.Network.Packets;
using BetaSharp.Network.Packets.S2CPlay;
using BetaSharp.Util;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using java.lang;

namespace BetaSharp.Server;

public class ChunkMap
{
    public List<ServerPlayerEntity> players = [];
    private readonly LongObjectHashMap chunkMapping = new();
    private readonly List<TrackedChunk> chunksToUpdate = [];
    public readonly ChunkLoadingQueue loadQueue;
    private MinecraftServer server;
    private readonly int dimensionId;
    private readonly int viewDistance;
    private readonly int[][] DIRECTIONS = [[1, 0], [0, 1], [-1, 0], [0, -1]];

    public ChunkMap(MinecraftServer server, int dimensionId, int viewRadius)
    {
        if (viewRadius > 15)
        {
            throw new IllegalArgumentException("Too big view radius!");
        }
        else if (viewRadius < 3)
        {
            throw new IllegalArgumentException("Too small view radius!");
        }
        else
        {
            viewDistance = viewRadius;
            this.server = server;
            this.dimensionId = dimensionId;
            this.loadQueue = new ChunkLoadingQueue(this);
        }
    }

    public ServerWorld getWorld()
    {
        return server.getWorld(dimensionId);
    }

    public void updateChunks()
    {
        for (int var1 = 0; var1 < chunksToUpdate.Count; var1++)
        {
            chunksToUpdate[var1].updateChunk();
        }

        chunksToUpdate.Clear();
        loadQueue.Tick();
    }

    public static long GetChunkHash(int chunkX, int chunkZ)
    {
        return chunkX + 2147483647L | chunkZ + 2147483647L << 32;
    }

    internal TrackedChunk GetOrCreateChunk(int chunkX, int chunkZ, bool createIfAbsent)
    {
        long var4 = GetChunkHash(chunkX, chunkZ);
        TrackedChunk var6 = (TrackedChunk)chunkMapping.get(var4);
        if (var6 == null && createIfAbsent)
        {
            var6 = new TrackedChunk(this, chunkX, chunkZ);
            chunkMapping.put(var4, var6);
        }

        return var6;
    }

    public void markBlockForUpdate(int x, int y, int z)
    {
        int var4 = x >> 4;
        int var5 = z >> 4;
        TrackedChunk var6 = GetOrCreateChunk(var4, var5, false);
        if (var6 != null)
        {
            var6.updatePlayerChunks(x & 15, y, z & 15);
        }
    }

    public void addPlayer(ServerPlayerEntity player)
    {
        int var2 = (int)player.x >> 4;
        int var3 = (int)player.z >> 4;
        player.lastX = player.x;
        player.lastZ = player.z;
        int var4 = 0;
        int var5 = viewDistance;
        int var6 = 0;
        int var7 = 0;
        if (GetOrCreateChunk(var2, var3, false) is TrackedChunk centerChunk)
        {
            centerChunk.addPlayer(player);
        }
        else
        {
            loadQueue.Add(var2, var3, player);
        }

        for (int var8 = 1; var8 <= var5 * 2; var8++)
        {
            for (int var9 = 0; var9 < 2; var9++)
            {
                int[] var10 = DIRECTIONS[var4++ % 4];

                for (int var11 = 0; var11 < var8; var11++)
                {
                    var6 += var10[0];
                    var7 += var10[1];
                    if (GetOrCreateChunk(var2 + var6, var3 + var7, false) is TrackedChunk chunk)
                    {
                        if (!chunk.HasPlayer(player))
                        {
                            chunk.addPlayer(player);
                        }
                    }
                    else
                    {
                        loadQueue.Add(var2 + var6, var3 + var7, player);
                    }
                }
            }
        }

        var4 %= 4;

        for (int var13 = 0; var13 < var5 * 2; var13++)
        {
            var6 += DIRECTIONS[var4][0];
            var7 += DIRECTIONS[var4][1];
            if (GetOrCreateChunk(var2 + var6, var3 + var7, false) is TrackedChunk chunk)
            {
                if (!chunk.HasPlayer(player))
                {
                    chunk.addPlayer(player);
                }
            }
            else
            {
                loadQueue.Add(var2 + var6, var3 + var7, player);
            }
        }

        players.Add(player);
    }

    public void removePlayer(ServerPlayerEntity player)
    {
        int var2 = (int)player.lastX >> 4;
        int var3 = (int)player.lastZ >> 4;

        for (int var4 = var2 - viewDistance; var4 <= var2 + viewDistance; var4++)
        {
            for (int var5 = var3 - viewDistance; var5 <= var3 + viewDistance; var5++)
            {
                TrackedChunk var6 = GetOrCreateChunk(var4, var5, false);
                var6?.removePlayer(player);
            }
        }

        players.Remove(player);
        loadQueue.RemovePlayer(player);
    }

    private bool isWithinViewDistance(int chunkX, int chunkZ, int centerX, int centerZ)
    {
        int var5 = chunkX - centerX;
        int var6 = chunkZ - centerZ;
        return var5 >= -viewDistance && var5 <= viewDistance && var6 >= -viewDistance && var6 <= viewDistance;
    }

    public void updatePlayerChunks(ServerPlayerEntity player)
    {
        int var2 = (int)player.x >> 4;
        int var3 = (int)player.z >> 4;
        double var4 = player.lastX - player.x;
        double var6 = player.lastZ - player.z;
        double var8 = var4 * var4 + var6 * var6;
        if (!(var8 < 64.0))
        {
            int var10 = (int)player.lastX >> 4;
            int var11 = (int)player.lastZ >> 4;
            int var12 = var2 - var10;
            int var13 = var3 - var11;
            if (var12 != 0 || var13 != 0)
            {
                for (int var14 = var2 - viewDistance; var14 <= var2 + viewDistance; var14++)
                {
                    for (int var15 = var3 - viewDistance; var15 <= var3 + viewDistance; var15++)
                    {
                        if (!isWithinViewDistance(var14, var15, var10, var11))
                        {
                            if (GetOrCreateChunk(var14, var15, false) is TrackedChunk chunk)
                            {
                                if (!chunk.HasPlayer(player))
                                {
                                    chunk.addPlayer(player);
                                }
                            }
                            else
                            {
                                loadQueue.Add(var14, var15, player);
                            }
                        }

                        if (!isWithinViewDistance(var14 - var12, var15 - var13, var2, var3))
                        {
                            TrackedChunk var16 = GetOrCreateChunk(var14 - var12, var15 - var13, false);
                            var16?.removePlayer(player);
                        }
                    }
                }

                player.lastX = player.x;
                player.lastZ = player.z;
            }
        }
    }

    public int getBlockViewDistance()
    {
        return viewDistance * 16 - 16;
    }

    internal class TrackedChunk
    {
        private readonly ChunkMap chunkMap;
        private readonly List<ServerPlayerEntity> players;
        private readonly int chunkX;
        private readonly int chunkZ;
        private readonly ChunkPos chunkPos;
        private readonly short[] dirtyBlocks;
        private int dirtyBlockCount;
        private int minX;
        private int minY;
        private int minZ;
        private int maxX;
        private int maxY;
        private int maxZ;

        public TrackedChunk(ChunkMap chunkMap, int chunkX, int chunkZ)
        {
            this.chunkMap = chunkMap;
            players = [];
            dirtyBlocks = new short[10];
            dirtyBlockCount = 0;
            this.chunkX = chunkX;
            this.chunkZ = chunkZ;
            chunkPos = new ChunkPos(chunkX, chunkZ);
            chunkMap.getWorld().chunkCache.loadChunk(chunkX, chunkZ);
        }

        public bool HasPlayer(ServerPlayerEntity player) => players.Contains(player);

        public void addPlayer(ServerPlayerEntity player)
        {
            if (players.Contains(player))
            {
                return;
            }

            if (player.activeChunks.Add(chunkPos))
            {
                player.networkHandler.sendPacket(new ChunkStatusUpdateS2CPacket(chunkPos.x, chunkPos.z, true));
            }

            players.Add(player);
            player.pendingChunkUpdates.add(chunkPos);
        }

        public void removePlayer(ServerPlayerEntity player)
        {
            if (players.Contains(player))
            {
                players.Remove(player);
                if (players.Count == 0)
                {
                    long var2 = chunkX + 2147483647L | chunkZ + 2147483647L << 32;
                    chunkMap.chunkMapping.remove(var2);
                    if (dirtyBlockCount > 0)
                    {
                        chunkMap.chunksToUpdate.Remove(this);
                    }

                    chunkMap.getWorld().chunkCache.isLoaded(chunkX, chunkZ);
                }

                player.pendingChunkUpdates.remove(chunkPos);
                if (player.activeChunks.Remove(chunkPos))
                {
                    player.networkHandler.sendPacket(new ChunkStatusUpdateS2CPacket(chunkX, chunkZ, false));
                }
            }
        }

        public void updatePlayerChunks(int x, int y, int z)
        {
            if (dirtyBlockCount == 0)
            {
                chunkMap.chunksToUpdate.Add(this);
                minX = minY = x;
                minZ = maxX = y;
                maxY = maxZ = z;
            }

            if (minX > x)
            {
                minX = x;
            }

            if (minY < x)
            {
                minY = x;
            }

            if (minZ > y)
            {
                minZ = y;
            }

            if (maxX < y)
            {
                maxX = y;
            }

            if (maxY > z)
            {
                maxY = z;
            }

            if (maxZ < z)
            {
                maxZ = z;
            }

            if (dirtyBlockCount < 10)
            {
                short var4 = (short)(x << 12 | z << 8 | y);

                for (int var5 = 0; var5 < dirtyBlockCount; var5++)
                {
                    if (dirtyBlocks[var5] == var4)
                    {
                        return;
                    }
                }

                dirtyBlocks[dirtyBlockCount++] = var4;
            }
        }

        public void sendPacketToPlayers(Packet packet)
        {
            for (int var2 = 0; var2 < players.Count; var2++)
            {
                ServerPlayerEntity var3 = players[var2];
                if (var3.activeChunks.Contains(chunkPos))
                {
                    var3.networkHandler.sendPacket(packet);
                }
            }
        }

        public void updateChunk()
        {
            ServerWorld var1 = chunkMap.getWorld();
            if (dirtyBlockCount != 0)
            {
                if (dirtyBlockCount == 1)
                {
                    int var2 = chunkX * 16 + minX;
                    int var3 = minZ;
                    int var4 = chunkZ * 16 + maxY;
                    sendPacketToPlayers(new BlockUpdateS2CPacket(var2, var3, var4, var1));
                    if (Block.BlocksWithEntity[var1.getBlockId(var2, var3, var4)])
                    {
                        sendBlockEntityUpdate(var1.getBlockEntity(var2, var3, var4));
                    }
                }
                else if (dirtyBlockCount == 10)
                {
                    minZ = minZ / 2 * 2;
                    maxX = (maxX / 2 + 1) * 2;
                    int var10 = minX + chunkX * 16;
                    int var12 = minZ;
                    int var14 = maxY + chunkZ * 16;
                    int var5 = minY - minX + 1;
                    int var6 = maxX - minZ + 2;
                    int var7 = maxZ - maxY + 1;
                    sendPacketToPlayers(new ChunkDataS2CPacket(var10, var12, var14, var5, var6, var7, var1));
                    var var8 = var1.getBlockEntities(var10, var12, var14, var10 + var5, var12 + var6, var14 + var7);

                    for (int var9 = 0; var9 < var8.Count; var9++)
                    {
                        sendBlockEntityUpdate(var8[var9]);
                    }
                }
                else
                {
                    sendPacketToPlayers(new ChunkDeltaUpdateS2CPacket(chunkX, chunkZ, dirtyBlocks, dirtyBlockCount, var1));

                    for (int var11 = 0; var11 < dirtyBlockCount; var11++)
                    {
                        int var13 = chunkX * 16 + (dirtyBlockCount >> 12 & 15);
                        int var15 = dirtyBlockCount & 0xFF;
                        int var16 = chunkZ * 16 + (dirtyBlockCount >> 8 & 15);
                        if (Block.BlocksWithEntity[var1.getBlockId(var13, var15, var16)])
                        {
                            Log.Info("Sending!");
                            sendBlockEntityUpdate(var1.getBlockEntity(var13, var15, var16));
                        }
                    }
                }

                dirtyBlockCount = 0;
            }
        }

        private void sendBlockEntityUpdate(BlockEntity blockentity)
        {
            if (blockentity != null)
            {
                Packet var2 = blockentity.createUpdatePacket();
                if (var2 != null)
                {
                    sendPacketToPlayers(var2);
                }
            }
        }
    }
}
