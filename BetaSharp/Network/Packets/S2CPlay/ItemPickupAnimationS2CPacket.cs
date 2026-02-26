using System.Net.Sockets;

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

    public override void Read(NetworkStream stream)
    {
        entityId = stream.ReadInt();
        collectorEntityId = stream.ReadInt();
    }

    public override void Write(NetworkStream stream)
    {
        stream.WriteInt(entityId);
        stream.WriteInt(collectorEntityId);
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
