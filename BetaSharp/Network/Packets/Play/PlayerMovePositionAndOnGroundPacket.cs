using java.io;

namespace BetaSharp.Network.Packets.Play;

public class PlayerMovePositionAndOnGroundPacket : PlayerMovePacket
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(PlayerMovePositionAndOnGroundPacket).TypeHandle);

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

    public override void read(DataInputStream stream)
    {
        x = stream.readDouble();
        y = stream.readDouble();
        eyeHeight = stream.readDouble();
        z = stream.readDouble();
        base.read(stream);
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeDouble(x);
        stream.writeDouble(y);
        stream.writeDouble(eyeHeight);
        stream.writeDouble(z);
        base.write(stream);
    }

    public override int size()
    {
        return 33;
    }
}