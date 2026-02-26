using System.Net.Sockets;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityMoveRelativeS2CPacket : EntityS2CPacket
{
    public EntityMoveRelativeS2CPacket()
    {
    }

    public EntityMoveRelativeS2CPacket(int entityId, byte deltaX, byte deltaY, byte deltaZ) : base(entityId)
    {
        this.deltaX = (sbyte)deltaX;
        this.deltaY = (sbyte)deltaY;
        this.deltaZ = (sbyte)deltaZ;
    }

    public override void Read(NetworkStream stream)
    {
        base.Read(stream);
        deltaX = (sbyte)stream.ReadByte();
        deltaY = (sbyte)stream.ReadByte();
        deltaZ = (sbyte)stream.ReadByte();
    }

    public override void Write(NetworkStream stream)
    {
        base.Write(stream);
        stream.WriteByte((byte)deltaX);
        stream.WriteByte((byte)deltaY);
        stream.WriteByte((byte)deltaZ);
    }

    public override int Size()
    {
        return 7;
    }
}
