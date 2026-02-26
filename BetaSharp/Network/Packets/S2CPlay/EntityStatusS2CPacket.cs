using System.Net.Sockets;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityStatusS2CPacket : Packet
{
    public int entityId;
    public sbyte entityStatus;

    public EntityStatusS2CPacket()
    {
    }

    public EntityStatusS2CPacket(int entityId, byte status)
    {
        this.entityId = entityId;
        entityStatus = (sbyte)status;
    }

    public override void Read(NetworkStream stream)
    {
        entityId = stream.ReadInt();
        entityStatus = (sbyte)stream.ReadByte();
    }

    public override void Write(NetworkStream stream)
    {
        stream.WriteInt(entityId);
        stream.WriteByte((byte)entityStatus);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onEntityStatus(this);
    }

    public override int Size()
    {
        return 5;
    }
}
