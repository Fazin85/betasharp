using BetaSharp.Entities;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class PlayerSleepUpdateS2CPacket : Packet
{
    public int id;
    public int x;
    public int y;
    public int z;
    public int status;

    public PlayerSleepUpdateS2CPacket()
    {
    }

    public PlayerSleepUpdateS2CPacket(Entity player, int status, int x, int y, int z)
    {
        this.status = status;
        this.x = x;
        this.y = y;
        this.z = z;
        this.id = player.id;
    }

    public override void Read(DataInputStream stream)
    {
        id = stream.readInt();
        status = (sbyte)stream.readByte();
        x = stream.readInt();
        y = (sbyte)stream.readByte();
        z = stream.readInt();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeInt(id);
        stream.writeByte(status);
        stream.writeInt(x);
        stream.writeByte(y);
        stream.writeInt(z);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onPlayerSleepUpdate(this);
    }

    public override int Size()
    {
        return 14;
    }
}