using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityRotateS2CPacket : EntityS2CPacket
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityRotateS2CPacket).TypeHandle);

    public EntityRotateS2CPacket()
    {
        rotate = true;
    }

    public EntityRotateS2CPacket(int entityId, byte yaw, byte pitch) : base(entityId)
    {
        this.yaw = (sbyte)yaw;
        this.pitch = (sbyte)pitch;
        rotate = true;
    }

    public override void read(DataInputStream stream)
    {
        base.read(stream);
        yaw = (sbyte)stream.readByte();
        pitch = (sbyte)stream.readByte();
    }

    public override void write(DataOutputStream stream)
    {
        base.write(stream);
        stream.writeByte(yaw);
        stream.writeByte(pitch);
    }

    public override int size()
    {
        return 6;
    }
}