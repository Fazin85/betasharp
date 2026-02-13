using BetaSharp.Entities;
using java.io;

namespace BetaSharp.Network.Packets.Play;

public class EntityAnimationPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityAnimationPacket).TypeHandle);

    public int id;
    public int animationId;

    public EntityAnimationPacket()
    {
    }

    public EntityAnimationPacket(Entity ent, int animationId)
    {
        id = ent.id;
        this.animationId = animationId;
    }

    public override void read(DataInputStream stream)
    {
        id = stream.readInt();
        animationId = (sbyte)stream.readByte();
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeInt(id);
        stream.writeByte(animationId);
    }

    public override void apply(NetHandler handler)
    {
        handler.onEntityAnimation(this);
    }

    public override int size()
    {
        return 5;
    }
}