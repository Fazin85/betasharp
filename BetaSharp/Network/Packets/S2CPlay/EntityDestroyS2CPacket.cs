using System.Net.Sockets;

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

    public override void Read(NetworkStream stream)
    {
        entityId = stream.ReadInt();
    }

    public override void Write(NetworkStream stream)
    {
        stream.WriteInt(entityId);
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
