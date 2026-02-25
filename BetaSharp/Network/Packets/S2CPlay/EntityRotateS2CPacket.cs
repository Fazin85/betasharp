using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityRotateS2CPacket : EntityS2CPacket
{
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

    public override void Read(DataInputStream stream)
    {
        base.Read(stream);
        yaw = (sbyte)stream.readByte();
        pitch = (sbyte)stream.readByte();
    }

    public override void Write(DataOutputStream stream)
    {
        base.Write(stream);
        stream.writeByte(yaw);
        stream.writeByte(pitch);
    }

    public override int Size()
    {
        return 6;
    }
}