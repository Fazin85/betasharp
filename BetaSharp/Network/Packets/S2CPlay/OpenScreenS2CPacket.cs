using System.Net.Sockets;

namespace BetaSharp.Network.Packets.S2CPlay;

public class OpenScreenS2CPacket : Packet
{
    public int syncId;
    public int screenHandlerId;
    public string name;
    public int slotsCount;

    public OpenScreenS2CPacket()
    {
    }

    public OpenScreenS2CPacket(int syncId, int screenHandlerId, String name, int size)
    {
        this.syncId = syncId;
        this.screenHandlerId = screenHandlerId;
        this.name = name;
        slotsCount = size;
    }

    public override void Apply(NetHandler handler)
    {
        handler.onOpenScreen(this);
    }

    public override void Read(NetworkStream stream)
    {
        syncId = (sbyte)stream.ReadByte();
        screenHandlerId = (sbyte)stream.ReadByte();
        name = stream.ReadString();
        slotsCount = (sbyte)stream.ReadByte();
    }

    public override void Write(NetworkStream stream)
    {
        stream.WriteByte((byte)syncId);
        stream.WriteByte((byte)screenHandlerId);
        stream.WriteString(name);
        stream.WriteByte((byte)slotsCount);
    }

    public override int Size()
    {
        return 3 + name.Length;
    }
}
