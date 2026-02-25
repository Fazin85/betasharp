using BetaSharp.Blocks;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Chunks.Occlusion;

public static class ChunkVisibilityComputer
{
    public static ChunkVisibilityStore Compute(WorldRegionSnapshot cache, int minX, int minY, int minZ)
    {
        int size = SubChunkRenderer.Size;
        int totalBlocks = size * size * size;
        ChunkVisibilityStore store = new();
        
        Span<uint> visited = stackalloc uint[(totalBlocks + 31) / 32];
        
        for (int f = 0; f < ChunkDirectionExtensions.Count; f++)
        {
            ChunkDirection startFace = (ChunkDirection)f;
            visited.Clear();
            
            ChunkDirectionMask reachable = FloodFill(cache, minX, minY, minZ, startFace, visited, size);
            
            for (int t = 0; t < ChunkDirectionExtensions.Count; t++)
            {
                if ((reachable & (ChunkDirectionMask)(1 << t)) != 0)
                {
                    store.SetVisible(startFace, (ChunkDirection)t);
                }
            }
        }

        return store;
    }

    private static ChunkDirectionMask FloodFill(
        WorldRegionSnapshot cache, 
        int minX, int minY, int minZ, 
        ChunkDirection startFace, 
        Span<uint> visited,
        int size)
    {
        ChunkDirectionMask reachable = ChunkDirectionMask.None;
        int totalBlocks = size * size * size;
        
        Span<ushort> queue = stackalloc ushort[totalBlocks];
        int head = 0, tail = 0;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int lx = 0, ly = 0, lz = 0;
                switch (startFace)
                {
                    case ChunkDirection.Down: lx = i; ly = 0; lz = j; break;
                    case ChunkDirection.Up: lx = i; ly = size - 1; lz = j; break;
                    case ChunkDirection.North: lx = i; ly = j; lz = 0; break;
                    case ChunkDirection.South: lx = i; ly = j; lz = size - 1; break;
                    case ChunkDirection.West: lx = 0; ly = i; lz = j; break;
                    case ChunkDirection.East: lx = size - 1; ly = i; lz = j; break;
                }

                if (IsAir(cache, minX + lx, minY + ly, minZ + lz))
                {
                    int idx = GetIndex(lx, ly, lz, size);
                    if (!IsVisited(visited, idx))
                    {
                        MarkVisited(visited, idx);
                        queue[tail++] = (ushort)idx;
                    }
                }
            }
        }

        while (head < tail)
        {
            ushort idx = queue[head++];
            int lx, ly, lz;
            if (size == 16)
            {
                lx = idx & 0xF;
                ly = (idx >> 4) & 0xF;
                lz = (idx >> 8) & 0xF;
            }
            else
            {
                lx = idx & 0x1F;
                ly = (idx >> 5) & 0x1F;
                lz = (idx >> 10) & 0x1F;
            }

            if (lx == 0) reachable |= ChunkDirectionMask.West;
            if (lx == size - 1) reachable |= ChunkDirectionMask.East;
            if (ly == 0) reachable |= ChunkDirectionMask.Down;
            if (ly == size - 1) reachable |= ChunkDirectionMask.Up;
            if (lz == 0) reachable |= ChunkDirectionMask.North;
            if (lz == size - 1) reachable |= ChunkDirectionMask.South;

            TryVisit(cache, minX, minY, minZ, lx - 1, ly, lz, visited, queue, ref tail, size);
            TryVisit(cache, minX, minY, minZ, lx + 1, ly, lz, visited, queue, ref tail, size);
            TryVisit(cache, minX, minY, minZ, lx, ly - 1, lz, visited, queue, ref tail, size);
            TryVisit(cache, minX, minY, minZ, lx, ly + 1, lz, visited, queue, ref tail, size);
            TryVisit(cache, minX, minY, minZ, lx, ly, lz - 1, visited, queue, ref tail, size);
            TryVisit(cache, minX, minY, minZ, lx, ly, lz + 1, visited, queue, ref tail, size);
        }

        return reachable;
    }

    private static void TryVisit(
        WorldRegionSnapshot cache, 
        int minX, int minY, int minZ, 
        int lx, int ly, int lz, 
        Span<uint> visited, 
        Span<ushort> queue, 
        ref int tail,
        int size)
    {
        if (lx < 0 || lx >= size || ly < 0 || ly >= size || lz < 0 || lz >= size) return;

        int idx = GetIndex(lx, ly, lz, size);
        if (IsVisited(visited, idx)) return;

        if (IsAir(cache, minX + lx, minY + ly, minZ + lz))
        {
            MarkVisited(visited, idx);
            queue[tail++] = (ushort)idx;
        }
    }

    private static bool IsAir(WorldRegionSnapshot cache, int x, int y, int z)
    {
        int id = cache.getBlockId(x, y, z);
        if (id <= 0) return true;
        return !Block.BlocksOpaque[id];
    }

    private static int GetIndex(int x, int y, int z, int size)
    {
        return size == 16 ? (x | (y << 4) | (z << 8)) : (x | (y << 5) | (z << 10));
    }

    private static bool IsVisited(Span<uint> visited, int idx) => (visited[idx >> 5] & (1u << (idx & 31))) != 0;
    private static void MarkVisited(Span<uint> visited, int idx) => visited[idx >> 5] |= (1u << (idx & 31));
}
