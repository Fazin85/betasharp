using System.Net.Sockets;
using BetaSharp.Entities;

namespace BetaSharp.Network.Packets.Play;

public class EntityAnimationPacket : Packet
{
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

    public override void Read(NetworkStream stream)
    {
        id = stream.ReadInt();
        animationId = (sbyte)stream.ReadByte();
    }

    public override void Write(NetworkStream stream)
    {
        stream.WriteInt(id);
        stream.WriteByte((byte)animationId);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onEntityAnimation(this);
    }

    public override int Size()
    {
        return 5;
    }
}
