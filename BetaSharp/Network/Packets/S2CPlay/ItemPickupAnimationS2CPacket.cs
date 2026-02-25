using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class ItemPickupAnimationS2CPacket : Packet
{
    public int entityId;
    public int collectorEntityId;

    public ItemPickupAnimationS2CPacket()
    {
    }

    public ItemPickupAnimationS2CPacket(int entityId, int collectorId)
    {
        this.entityId = entityId;
        collectorEntityId = collectorId;
    }

    public override void Read(DataInputStream stream)
    {
        entityId = stream.readInt();
        collectorEntityId = stream.readInt();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeInt(entityId);
        stream.writeInt(collectorEntityId);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onItemPickupAnimation(this);
    }

    public override int Size()
    {
        return 8;
    }
}