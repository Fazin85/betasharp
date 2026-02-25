using BetaSharp.Entities;
using java.io;

namespace BetaSharp.Network.Packets.C2SPlay;

public class ClientCommandC2SPacket : Packet
{
    public int entityId;
    public int mode;

    public ClientCommandC2SPacket()
    {
    }

    public ClientCommandC2SPacket(Entity ent, int mode)
    {
        entityId = ent.id;
        this.mode = mode;
    }

    public override void Read(DataInputStream stream)
    {
        entityId = stream.readInt();
        mode = (sbyte)stream.readByte();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeInt(entityId);
        stream.writeByte(mode);
    }

    public override void Apply(NetHandler handler)
    {
        handler.handleClientCommand(this);
    }

    public override int Size()
    {
        return 5;
    }
}