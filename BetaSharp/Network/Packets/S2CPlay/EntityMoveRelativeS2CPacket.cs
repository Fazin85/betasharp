using java.io;

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

    public override void Read(DataInputStream stream)
    {
        base.Read(stream);
        deltaX = (sbyte)stream.readByte();
        deltaY = (sbyte)stream.readByte();
        deltaZ = (sbyte)stream.readByte();
    }

    public override void Write(DataOutputStream stream)
    {
        base.Write(stream);
        stream.writeByte(deltaX);
        stream.writeByte(deltaY);
        stream.writeByte(deltaZ);
    }

    public override int Size()
    {
        return 7;
    }
}