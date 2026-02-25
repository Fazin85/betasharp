using java.io;

namespace BetaSharp.Network.Packets.Play;

public class PlayerMovePositionAndOnGroundPacket : PlayerMovePacket
{
    public PlayerMovePositionAndOnGroundPacket()
    {
        changePosition = true;
    }

    public PlayerMovePositionAndOnGroundPacket(double x, double y, double eyeHeight, double z, bool onGround)
    {
        base.x = x;
        base.y = y;
        base.eyeHeight = eyeHeight;
        base.z = z;
        base.onGround = onGround;
        changePosition = true;
    }

    public override void Read(DataInputStream stream)
    {
        x = stream.readDouble();
        y = stream.readDouble();
        eyeHeight = stream.readDouble();
        z = stream.readDouble();
        base.Read(stream);
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeDouble(x);
        stream.writeDouble(y);
        stream.writeDouble(eyeHeight);
        stream.writeDouble(z);
        base.Write(stream);
    }

    public override int Size()
    {
        return 33;
    }
}