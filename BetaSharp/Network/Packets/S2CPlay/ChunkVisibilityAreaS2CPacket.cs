using System.Net.Sockets;

namespace BetaSharp.Network.Packets.S2CPlay;

public class ChunkVisibilityAreaS2CPacket() : ExtendedProtocolPacket(PacketId.ChunkVisibilityArea)
{
    public int MinX;
    public int MaxX;
    public int MinZ;
    public int MaxZ;

    public ChunkVisibilityAreaS2CPacket(int minX, int maxX, int minZ, int maxZ) : this()
    {
        MinX = minX;
        MaxX = maxX;
        MinZ = minZ;
        MaxZ = maxZ;
    }

    public override void Read(NetworkStream stream)
    {
        MinX = stream.ReadInt();
        MaxX = stream.ReadInt();
        MinZ = stream.ReadInt();
        MaxZ = stream.ReadInt();
    }

    public override void Write(NetworkStream stream)
    {
        stream.WriteInt(MinX);
        stream.WriteInt(MaxX);
        stream.WriteInt(MinZ);
        stream.WriteInt(MaxZ);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onChunkVisibilityArea(this);
    }

    public override int Size()
    {
        return 16;
    }
}
