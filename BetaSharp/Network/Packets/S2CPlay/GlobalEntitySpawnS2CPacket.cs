using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class GlobalEntitySpawnS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(GlobalEntitySpawnS2CPacket).TypeHandle);

    public int id;
    public int x;
    public int y;
    public int z;
    public int type;

    public GlobalEntitySpawnS2CPacket()
    {
    }

    public GlobalEntitySpawnS2CPacket(Entity ent)
    {
        id = ent.id;
        x = MathHelper.Floor(ent.x * 32.0D);
        y = MathHelper.Floor(ent.y * 32.0D);
        z = MathHelper.Floor(ent.z * 32.0D);
        if (ent is EntityLightningBolt)
        {
            type = 1;
        }

    }

    public override void read(DataInputStream stream)
    {
        id = stream.readInt();
        type = (sbyte)stream.readByte();
        x = stream.readInt();
        y = stream.readInt();
        z = stream.readInt();
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeInt(id);
        stream.writeByte(type);
        stream.writeInt(x);
        stream.writeInt(y);
        stream.writeInt(z);
    }

    public override void apply(NetHandler handler)
    {
        handler.onLightningEntitySpawn(this);
    }

    public override int size()
    {
        return 17;
    }
}