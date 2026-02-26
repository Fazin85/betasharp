using System.Net.Sockets;

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

    public override void Read(NetworkStream stream)
    {
        id = stream.ReadInt();
    }

    public override void Write(NetworkStream stream)
    {
        stream.WriteInt(id);
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
