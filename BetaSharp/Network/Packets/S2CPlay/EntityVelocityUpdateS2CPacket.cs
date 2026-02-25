using BetaSharp.Entities;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class EntityVelocityUpdateS2CPacket : Packet
{
    public int entityId;
    public int motionX;
    public int motionY;
    public int motionZ;

    public EntityVelocityUpdateS2CPacket()
    {
    }

    public EntityVelocityUpdateS2CPacket(Entity ent) : this(ent.id, ent.velocityX, ent.velocityY, ent.velocityZ)
    {
    }

    public EntityVelocityUpdateS2CPacket(int entityId, double motionX, double motionY, double motionZ)
    {
        this.entityId = entityId;
        double maxvelocity = 3.9D;
        if (motionX < -maxvelocity)
        {
            motionX = -maxvelocity;
        }

        if (motionY < -maxvelocity)
        {
            motionY = -maxvelocity;
        }

        if (motionZ < -maxvelocity)
        {
            motionZ = -maxvelocity;
        }

        if (motionX > maxvelocity)
        {
            motionX = maxvelocity;
        }

        if (motionY > maxvelocity)
        {
            motionY = maxvelocity;
        }

        if (motionZ > maxvelocity)
        {
            motionZ = maxvelocity;
        }

        this.motionX = (int)(motionX * 8000.0D);
        this.motionY = (int)(motionY * 8000.0D);
        this.motionZ = (int)(motionZ * 8000.0D);
    }

    public override void Read(DataInputStream stream)
    {
        entityId = stream.readInt();
        motionX = stream.readShort();
        motionY = stream.readShort();
        motionZ = stream.readShort();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeInt(entityId);
        stream.writeShort(motionX);
        stream.writeShort(motionY);
        stream.writeShort(motionZ);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onEntityVelocityUpdate(this);
    }

    public override int Size()
    {
        return 10;
    }
}