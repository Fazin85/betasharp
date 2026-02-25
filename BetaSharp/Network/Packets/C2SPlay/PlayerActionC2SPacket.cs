using java.io;

namespace BetaSharp.Network.Packets.C2SPlay;

public class PlayerActionC2SPacket : Packet
{
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

    public override void Read(DataInputStream stream)
    {
        action = stream.read();
        x = stream.readInt();
        y = stream.read();
        z = stream.readInt();
        direction = stream.read();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.write(action);
        stream.writeInt(x);
        stream.write(y);
        stream.writeInt(z);
        stream.write(direction);
    }

    public override void Apply(NetHandler handler)
    {
        handler.handlePlayerAction(this);
    }

    public override int Size()
    {
        return 11;
    }
}