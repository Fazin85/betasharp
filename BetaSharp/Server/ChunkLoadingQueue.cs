using BetaSharp.Entities;

namespace BetaSharp.Server;

public class ChunkLoadingQueue(ChunkMap chunkMap)
{
    private readonly ChunkMap chunkMap = chunkMap;
    private readonly Dictionary<long, PendingChunk> pendingChunks = [];
    //TODO: MAKE THIS CONFIGURABLE
    private const int MAX_CHUNKS_PER_TICK = 5;

    public void Add(int x, int z, ServerPlayerEntity player)
    {
        long hash = ChunkMap.GetChunkHash(x, z);

        if (pendingChunks.TryGetValue(hash, out var pending))
        {
            if (!pending.Players.Contains(player))
            {
                pending.Players.Add(player);
            }
        }
        else
        {
            var newPending = new PendingChunk(hash, x, z, player);
            pendingChunks.Add(hash, newPending);
        }
    }

    public void RemovePlayer(ServerPlayerEntity player)
    {
        var keysToRemove = new List<long>();
        
        foreach (var kvp in pendingChunks)
        {
            kvp.Value.Players.Remove(player);
            if (kvp.Value.Players.Count == 0)
            {
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            pendingChunks.Remove(key);
        }
    }

    public void Tick()
    {
        if (pendingChunks.Count == 0) return;

        // Convert to list for sorting
        var pendingList = pendingChunks.Values.ToList();
        pendingList.Sort((a, b) =>
        {
            double distA = a.GetMinDistanceSqr();
            double distB = b.GetMinDistanceSqr();
            return distA.CompareTo(distB);
        });

        int chunksLoaded = 0;

        for (int i = 0; i < pendingList.Count && chunksLoaded < MAX_CHUNKS_PER_TICK; i++)
        {
            var chunkToLoad = pendingList[i];
            pendingChunks.Remove(chunkToLoad.Hash);

            var chunk = chunkMap.GetOrCreateChunk(chunkToLoad.X, chunkToLoad.Z, true);

            if (chunk != null)
            {
                foreach (var player in chunkToLoad.Players)
                {
                    if (!chunk.HasPlayer(player))
                    {
                        chunk.addPlayer(player);
                    }
                }
            }
            chunksLoaded++;
        }
    }

    private class PendingChunk
    {
        public long Hash { get; }
        public int X { get; }
        public int Z { get; }
        public List<ServerPlayerEntity> Players { get; } = [];
        private readonly double centerX;
        private readonly double centerZ;

        public PendingChunk(long hash, int x, int z, ServerPlayerEntity initiator)
        {
            Hash = hash;
            X = x;
            Z = z;
            centerX = x * 16 + 8;
            centerZ = z * 16 + 8;
            Players.Add(initiator);
        }

        public double GetMinDistanceSqr()
        {
            double min = double.MaxValue;

            foreach (var p in Players)
            {
                double dx = p.x - centerX;
                double dz = p.z - centerZ;
                double d = dx * dx + dz * dz;
                if (d < min) min = d;
            }
            return min;
        }
    }
}
