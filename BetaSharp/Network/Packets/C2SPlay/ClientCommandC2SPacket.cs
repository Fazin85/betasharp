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

    public override void read(DataInputStream stream)
    {
        entityId = stream.readInt();
        mode = (sbyte)stream.readByte();
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeInt(entityId);
        stream.writeByte(mode);
    }

    public override void apply(NetHandler handler)
    {
        handler.handleClientCommand(this);
    }

    public override int size()
    {
        return 5;
    }
}