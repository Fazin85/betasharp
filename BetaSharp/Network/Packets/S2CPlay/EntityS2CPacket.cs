using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityS2CPacket : Packet
{
    public int id;
    public sbyte deltaX;
    public sbyte deltaY;
    public sbyte deltaZ;
    public sbyte yaw;
    public sbyte pitch;
    public bool rotate = false;
    public EntityS2CPacket(int entityId)
    {
        id = entityId;
    }

    public EntityS2CPacket()
    {
    }

    public override void Read(DataInputStream stream)
    {
        id = stream.readInt();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeInt(id);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onEntity(this);
    }

    public override int Size()
    {
        return 4;
    }
}