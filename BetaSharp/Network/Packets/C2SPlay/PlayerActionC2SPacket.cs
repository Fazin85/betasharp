using java.io;

namespace BetaSharp.Network.Packets.C2SPlay;

public class PlayerActionC2SPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(PlayerActionC2SPacket).TypeHandle);

    public int x;
    public int y;
    public int z;
    public int direction;
    public int action;

    public PlayerActionC2SPacket()
    {
    }

    public PlayerActionC2SPacket(int action, int x, int y, int z, int direction)
    {
        this.action = action;
        this.x = x;
        this.y = y;
        this.z = z;
        this.direction = direction;
    }

    public override void read(DataInputStream stream)
    {
        action = stream.read();
        x = stream.readInt();
        y = stream.read();
        z = stream.readInt();
        direction = stream.read();
    }

    public override void write(DataOutputStream stream)
    {
        stream.write(action);
        stream.writeInt(x);
        stream.write(y);
        stream.writeInt(z);
        stream.write(direction);
    }

    public override void apply(NetHandler handler)
    {
        handler.handlePlayerAction(this);
    }

    public override int size()
    {
        return 11;
    }
}