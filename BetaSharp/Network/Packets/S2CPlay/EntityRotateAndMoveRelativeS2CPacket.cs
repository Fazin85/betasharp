using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityRotateAndMoveRelativeS2CPacket : EntityS2CPacket
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityRotateAndMoveRelativeS2CPacket).TypeHandle);

    public EntityRotateAndMoveRelativeS2CPacket()
    {
        rotate = true;
    }

    public EntityRotateAndMoveRelativeS2CPacket(int entityId, byte deltaX, byte deltaY, byte deltaZ, byte yaw, byte pitch) : base(entityId)
    {
        this.deltaX = (sbyte)deltaX;
        this.deltaY = (sbyte)deltaY;
        this.deltaZ = (sbyte)deltaZ;
        this.yaw = (sbyte)yaw;
        this.pitch = (sbyte)pitch;
        rotate = true;
    }

    public override void read(DataInputStream stream)
    {
        base.read(stream);
        deltaX = (sbyte)stream.readByte();
        deltaY = (sbyte)stream.readByte();
        deltaZ = (sbyte)stream.readByte();
        yaw = (sbyte)stream.readByte();
        pitch = (sbyte)stream.readByte();
    }

    public override void write(DataOutputStream stream)
    {
        base.write(stream);
        stream.writeByte(deltaX);
        stream.writeByte(deltaY);
        stream.writeByte(deltaZ);
        stream.writeByte(yaw);
        stream.writeByte(pitch);
    }

    public override int size()
    {
        return 9;
    }
}