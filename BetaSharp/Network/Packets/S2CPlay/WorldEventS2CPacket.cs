using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class WorldEventS2CPacket : Packet
{
    public int eventId;
    public int data;
    public int x;
    public int y;
    public int z;

    public WorldEventS2CPacket()
    {
    }

    public WorldEventS2CPacket(int eventId, int x, int y, int z, int data)
    {
        this.eventId = eventId;
        this.x = x;
        this.y = y;
        this.z = z;
        this.data = data;
    }

    public override void Read(DataInputStream stream)
    {
        eventId = stream.readInt();
        x = stream.readInt();
        y = (sbyte)stream.readByte();
        z = stream.readInt();
        data = stream.readInt();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeInt(eventId);
        stream.writeInt(x);
        stream.writeByte(y);
        stream.writeInt(z);
        stream.writeInt(data);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onWorldEvent(this);
    }

    public override int Size()
    {
        return 20;
    }
}