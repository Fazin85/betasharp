using BetaSharp.Entities;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class PlayerSleepUpdateS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(PlayerSleepUpdateS2CPacket).TypeHandle);

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

    public override void read(DataInputStream stream)
    {
        id = stream.readInt();
        status = (sbyte)stream.readByte();
        x = stream.readInt();
        y = (sbyte)stream.readByte();
        z = stream.readInt();
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeInt(id);
        stream.writeByte(status);
        stream.writeInt(x);
        stream.writeByte(y);
        stream.writeInt(z);
    }

    public override void apply(NetHandler handler)
    {
        handler.onPlayerSleepUpdate(this);
    }

    public override int size()
    {
        return 14;
    }
}