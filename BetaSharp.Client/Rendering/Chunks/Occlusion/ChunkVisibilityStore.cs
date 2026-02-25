using Silk.NET.Maths;

namespace BetaSharp.Client.Rendering.Chunks.Occlusion;

public struct ChunkVisibilityStore
{
    // A 64-bit mask where each bit represents visibility between two faces.
    // There are 6 faces, so 6*6 = 36 possible paths.
    // Bit index = (from * 8) + to
    private long _data;

    public void SetVisible(ChunkDirection from, ChunkDirection to)
    {
        _data |= 1L << GetBit(from, to);
    }

    public readonly bool IsVisible(ChunkDirection from, ChunkDirection to)
    {
        return (_data & (1L << GetBit(from, to))) != 0;
    }

    public readonly ChunkDirectionMask GetVisibleFrom(ChunkDirectionMask incoming, Vector3D<double> viewPos, SubChunkRenderer renderer)
    {
        if (incoming == ChunkDirectionMask.None)
            return FoldOutgoing(_data); // If no incoming (camera inside), all are potentially valid

        long visibilityData = _data;
        
        // Angle occlusion masking (Sodium's optimization)
        visibilityData &= GetAngleMask(viewPos, renderer);

        long mask = CreateMask((int)incoming);
        return FoldOutgoing(visibilityData & mask);
    }

    private static long GetAngleMask(Vector3D<double> viewPos, SubChunkRenderer renderer)
    {
        var center = renderer.PositionPlus;
        double dx = Math.Abs(viewPos.X - center.X);
        double dy = Math.Abs(viewPos.Y - center.Y);
        double dz = Math.Abs(viewPos.Z - center.Z);

        long mask = 0;
        if (dx > dy || dz > dy) mask |= GetUpDownOccluded();
        if (dx > dz || dy > dz) mask |= GetNorthSouthOccluded();
        if (dy > dx || dz > dx) mask |= GetWestEastOccluded();

        return ~mask;
    }

    private static long GetUpDownOccluded() => (1L << GetBit(ChunkDirection.Down, ChunkDirection.Up)) | (1L << GetBit(ChunkDirection.Up, ChunkDirection.Down));
    private static long GetNorthSouthOccluded() => (1L << GetBit(ChunkDirection.North, ChunkDirection.South)) | (1L << GetBit(ChunkDirection.South, ChunkDirection.North));
    private static long GetWestEastOccluded() => (1L << GetBit(ChunkDirection.West, ChunkDirection.East)) | (1L << GetBit(ChunkDirection.East, ChunkDirection.West));

    public readonly ChunkDirectionMask GetVisibleAll()
    {
        return FoldOutgoing(_data);
    }

    private static int GetBit(ChunkDirection from, ChunkDirection to)
    {
        return ((int)from << 3) | (int)to;
    }

    private static long CreateMask(int incoming)
    {
        // This spreads the 6 mask bits across 6 bytes, putting bit 'i' at bit 0 of byte 'i'
        // Multiplier has bits at 0, 7, 14, 21, 28, 35
        const long multiplier = 0x810204081L;
        long expanded = multiplier * (uint)incoming;
        return (expanded & 0x010101010101L) * 0xFF;
    }

    private static ChunkDirectionMask FoldOutgoing(long data)
    {
        long folded = data;
        folded |= folded >> 32;
        folded |= folded >> 16;
        folded |= folded >> 8;
        return (ChunkDirectionMask)(folded & (int)ChunkDirectionMask.All);
    }
}
