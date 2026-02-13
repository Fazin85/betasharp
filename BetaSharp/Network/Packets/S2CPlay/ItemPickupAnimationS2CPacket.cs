using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class ItemPickupAnimationS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(ItemPickupAnimationS2CPacket).TypeHandle);

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

    public override void read(DataInputStream stream)
    {
        entityId = stream.readInt();
        collectorEntityId = stream.readInt();
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeInt(entityId);
        stream.writeInt(collectorEntityId);
    }

    public override void apply(NetHandler handler)
    {
        handler.onItemPickupAnimation(this);
    }

    public override int size()
    {
        return 8;
    }
}