using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class WorldEventS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(WorldEventS2CPacket).TypeHandle);

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

    public override void read(DataInputStream stream)
    {
        eventId = stream.readInt();
        x = stream.readInt();
        y = (sbyte)stream.readByte();
        z = stream.readInt();
        data = stream.readInt();
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeInt(eventId);
        stream.writeInt(x);
        stream.writeByte(y);
        stream.writeInt(z);
        stream.writeInt(data);
    }

    public override void apply(NetHandler handler)
    {
        handler.onWorldEvent(this);
    }

    public override int size()
    {
        return 20;
    }
}