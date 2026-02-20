using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntitySpawnS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntitySpawnS2CPacket).TypeHandle);

    public int id;
    public int x;
    public int y;
    public int z;
    public int velocityX;
    public int velocityY;
    public int velocityZ;
    public int entityType;
    public int entityData;

    public EntitySpawnS2CPacket()
    {
    }

    public EntitySpawnS2CPacket(Entity entity, int entityType) : this(entity, entityType, 0)
    {
    }

    public EntitySpawnS2CPacket(Entity entity, int entityType, int entityData)
    {
        id = entity.id;
        x = MathHelper.Floor(entity.x * 32.0);
        y = MathHelper.Floor(entity.y * 32.0);
        z = MathHelper.Floor(entity.z * 32.0);
        this.entityType = entityType;
        this.entityData = entityData;
        if (entityData > 0)
        {
            double velocityX = entity.velocityX;
            double velocityY = entity.velocityY;
            double velocityZ = entity.velocityZ;
            double maxVelocity = 3.9;
            if (velocityX < -maxVelocity)
            {
                velocityX = -maxVelocity;
            }

            if (velocityY < -maxVelocity)
            {
                velocityY = -maxVelocity;
            }

            if (velocityZ < -maxVelocity)
            {
                velocityZ = -maxVelocity;
            }

            if (velocityX > maxVelocity)
            {
                velocityX = maxVelocity;
            }

            if (velocityY > maxVelocity)
            {
                velocityY = maxVelocity;
            }

            if (velocityZ > maxVelocity)
            {
                velocityZ = maxVelocity;
            }

            this.velocityX = (int)(velocityX * 8000.0);
            this.velocityY = (int)(velocityY * 8000.0);
            this.velocityZ = (int)(velocityZ * 8000.0);
        }
    }

    public override void read(DataInputStream stream)
    {
        id = stream.readInt();
        entityType = (sbyte)stream.readByte();
        x = stream.readInt();
        y = stream.readInt();
        z = stream.readInt();
        entityData = stream.readInt();
        if (entityData > 0)
        {
            velocityX = stream.readShort();
            velocityY = stream.readShort();
            velocityZ = stream.readShort();
        }

    }

    public override void write(DataOutputStream stream)
    {
        stream.writeInt(id);
        stream.writeByte(entityType);
        stream.writeInt(x);
        stream.writeInt(y);
        stream.writeInt(z);
        stream.writeInt(entityData);
        if (entityData > 0)
        {
            stream.writeShort(velocityX);
            stream.writeShort(velocityY);
            stream.writeShort(velocityZ);
        }

    }

    public override void apply(NetHandler handler)
    {
        handler.onEntitySpawn(this);
    }

    public override int size()
    {
        return 21 + entityData > 0 ? 6 : 0;
    }
}