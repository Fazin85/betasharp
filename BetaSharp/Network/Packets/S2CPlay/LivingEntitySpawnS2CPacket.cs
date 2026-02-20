using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using java.io;
using java.util;

namespace BetaSharp.Network.Packets.S2CPlay;

public class LivingEntitySpawnS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(LivingEntitySpawnS2CPacket).TypeHandle);

    public int entityId;
    public sbyte type;
    public int xPosition;
    public int yPosition;
    public int zPosition;
    public sbyte yaw;
    public sbyte pitch;
    private DataWatcher metaData;
    private List receivedMetadata;

    public LivingEntitySpawnS2CPacket()
    {
    }

    public LivingEntitySpawnS2CPacket(EntityLiving ent)
    {
        entityId = ent.id;
        type = (sbyte)EntityRegistry.getRawId(ent);
        xPosition = MathHelper.Floor(ent.x * 32.0D);
        yPosition = MathHelper.Floor(ent.y * 32.0D);
        zPosition = MathHelper.Floor(ent.z * 32.0D);
        yaw = (sbyte)(int)(ent.yaw * 256.0F / 360.0F);
        pitch = (sbyte)(int)(ent.pitch * 256.0F / 360.0F);
        metaData = ent.getDataWatcher();
    }

    public override void read(DataInputStream stream)
    {
        entityId = stream.readInt();
        type = (sbyte)stream.readByte();
        xPosition = stream.readInt();
        yPosition = stream.readInt();
        zPosition = stream.readInt();
        yaw = (sbyte)stream.readByte();
        pitch = (sbyte)stream.readByte();
        receivedMetadata = DataWatcher.readWatchableObjects(stream);
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeInt(entityId);
        stream.writeByte(type);
        stream.writeInt(xPosition);
        stream.writeInt(yPosition);
        stream.writeInt(zPosition);
        stream.writeByte(yaw);
        stream.writeByte(pitch);
        metaData.writeWatchableObjects(stream);
    }

    public override void apply(NetHandler handler)
    {
        handler.onLivingEntitySpawn(this);
    }

    public override int size()
    {
        return 20;
    }

    public List getMetadata()
    {
        return receivedMetadata;
    }
}