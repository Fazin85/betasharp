using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class PlayerSpawnPositionS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(PlayerSpawnPositionS2CPacket).TypeHandle);

    public int x;
    public int y;
    public int z;

    public PlayerSpawnPositionS2CPacket()
    {
    }

    public PlayerSpawnPositionS2CPacket(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public override void read(DataInputStream stream)
    {
        x = stream.readInt();
        y = stream.readInt();
        z = stream.readInt();
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeInt(x);
        stream.writeInt(y);
        stream.writeInt(z);
    }

    public override void apply(NetHandler handler)
    {
        handler.onPlayerSpawnPosition(this);
    }

    public override int size()
    {
        return 12;
    }
}