using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityMoveRelativeS2CPacket : EntityS2CPacket
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityMoveRelativeS2CPacket).TypeHandle);

    public EntityMoveRelativeS2CPacket()
    {
    }

    public EntityMoveRelativeS2CPacket(int entityId, byte deltaX, byte deltaY, byte deltaZ) : base(entityId)
    {
        this.deltaX = (sbyte)deltaX;
        this.deltaY = (sbyte)deltaY;
        this.deltaZ = (sbyte)deltaZ;
    }

    public override void read(DataInputStream stream)
    {
        base.read(stream);
        deltaX = (sbyte)stream.readByte();
        deltaY = (sbyte)stream.readByte();
        deltaZ = (sbyte)stream.readByte();
    }

    public override void write(DataOutputStream stream)
    {
        base.write(stream);
        stream.writeByte(deltaX);
        stream.writeByte(deltaY);
        stream.writeByte(deltaZ);
    }

    public override int size()
    {
        return 7;
    }
}