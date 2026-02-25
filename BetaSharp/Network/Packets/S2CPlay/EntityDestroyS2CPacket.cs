using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityDestroyS2CPacket : Packet
{
    public int entityId;

    public EntityDestroyS2CPacket()
    {
    }

    public EntityDestroyS2CPacket(int id)
    {
        entityId = id;
    }

    public override void Read(DataInputStream stream)
    {
        entityId = stream.readInt();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeInt(entityId);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onEntityDestroy(this);
    }

    public override int Size()
    {
        return 4;
    }
}