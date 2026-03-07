using BetaSharp.Entities;

namespace BetaSharp.Server;

internal class ChunkLoadingQueue(ChunkMap chunkMap)
{
    private readonly ChunkMap _chunkMap = chunkMap;
    private readonly Dictionary<long, PendingChunk> _pendingChunks = [];
    private readonly List<ServerPlayerEntity> _uniquePlayersCache = [];
    private readonly List<PendingChunk> _playerChunksCache = [];

    public void Add(int x, int z, ServerPlayerEntity player)
    {
        long hash = ChunkMap.GetChunkHash(x, z);

        if (_pendingChunks.TryGetValue(hash, out PendingChunk? pending))
        {
            if (!pending.Players.Contains(player))
            {
                pending.Players.Add(player);
            }
        }
        else
        {
            _pendingChunks[hash] = new PendingChunk(hash, x, z, player);
        }
    }

    public void RemovePlayer(ServerPlayerEntity player)
    {
        var toRemove = new List<long>();
        foreach ((long hash, PendingChunk? chunk) in _pendingChunks)
        {
            chunk.Players.Remove(player);
            if (chunk.Players.Count == 0)
            {
                toRemove.Add(hash);
            }
        }
        foreach (long hash in toRemove)
        {
            _pendingChunks.Remove(hash);
        }
    }

    public void Tick()
    {
        if (_pendingChunks.Count == 0) return;

        _uniquePlayersCache.Clear();
        foreach (PendingChunk chunk in _pendingChunks.Values)
        {
            foreach (ServerPlayerEntity player in chunk.Players)
            {
                if (!_uniquePlayersCache.Contains(player))
                {
                    _uniquePlayersCache.Add(player);
                }
            }
        }

        if (_uniquePlayersCache.Count == 0) return;

        int budgetPerPlayer = Math.Max(1, Math.Min(5, 20 / _uniquePlayersCache.Count));

        foreach (ServerPlayerEntity player in _uniquePlayersCache)
        {
            _playerChunksCache.Clear();
            foreach (PendingChunk chunk in _pendingChunks.Values)
            {
                if (chunk.Players.Contains(player))
                {
                    _playerChunksCache.Add(chunk);
                }
            }

            _playerChunksCache.Sort((a, b) =>
            {
                double dxA = player.x - a.CenterX;
                double dzA = player.z - a.CenterZ;
                double distA = dxA * dxA + dzA * dzA;

                double dxB = player.x - b.CenterX;
                double dzB = player.z - b.CenterZ;
                double distB = dxB * dxB + dzB * dzB;

                return distA.CompareTo(distB);
            });

            int loadedForPlayer = 0;
            foreach (PendingChunk chunkToLoad in _playerChunksCache)
            {
                if (loadedForPlayer >= budgetPerPlayer) break;
                if (!_pendingChunks.ContainsKey(chunkToLoad.Hash)) continue; // might have been loaded by another player

                _pendingChunks.Remove(chunkToLoad.Hash);

                ChunkMap.TrackedChunk chunk = _chunkMap.GetOrCreateChunk(chunkToLoad.X, chunkToLoad.Z, true);
                if (chunk != null)
                {
                    foreach (ServerPlayerEntity p in chunkToLoad.Players)
                    {
                        if (!chunk.HasPlayer(p))
                        {
                            chunk.addPlayer(p);
                        }
                    }
                }
                loadedForPlayer++;
            }
        }
    }

    private class PendingChunk
    {
        public long Hash { get; }
        public int X { get; }
        public int Z { get; }
        public List<ServerPlayerEntity> Players { get; } = [];

        public double CenterX { get; }
        public double CenterZ { get; }

        public PendingChunk(long hash, int x, int z, ServerPlayerEntity initiator)
        {
            Hash = hash;
            X = x;
            Z = z;
            CenterX = x * 16 + 8;
            CenterZ = z * 16 + 8;
            Players.Add(initiator);
        }
    }
}
