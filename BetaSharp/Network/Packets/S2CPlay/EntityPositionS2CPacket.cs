using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityPositionS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityPositionS2CPacket).TypeHandle);

    public int id;
    public int x;
    public int y;
    public int z;
    public sbyte yaw;
    public sbyte pitch;

    public EntityPositionS2CPacket()
    {
    }

    public EntityPositionS2CPacket(int entityId, int x, int y, int z, byte yaw, byte pitch)
    {
        this.id = entityId;
        this.x = x;
        this.y = y;
        this.z = z;
        this.yaw = (sbyte)yaw;
        this.pitch = (sbyte)pitch;
    }

    public EntityPositionS2CPacket(Entity var1)
    {
        id = var1.id;
        x = MathHelper.Floor(var1.x * 32.0D);
        y = MathHelper.Floor(var1.y * 32.0D);
        z = MathHelper.Floor(var1.z * 32.0D);
        yaw = (sbyte)(int)(var1.yaw * 256.0F / 360.0F);
        pitch = (sbyte)(int)(var1.pitch * 256.0F / 360.0F);
    }

    public override void read(DataInputStream stream)
    {
        id = stream.readInt();
        x = stream.readInt();
        y = stream.readInt();
        z = stream.readInt();
        yaw = (sbyte)stream.read();
        pitch = (sbyte)stream.read();
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeInt(id);
        stream.writeInt(x);
        stream.writeInt(y);
        stream.writeInt(z);
        stream.write(yaw);
        stream.write(pitch);
    }

    public override void apply(NetHandler handler)
    {
        handler.onEntityPosition(this);
    }

    public override int size()
    {
        return 34;
    }
}