using java.io;

namespace BetaSharp.Network.Packets.Play;

public class PlayerMoveFullPacket : PlayerMovePacket
{
    public PlayerMoveFullPacket()
    {
        changeLook = true;
        changePosition = true;
    }

    public PlayerMoveFullPacket(double x, double y, double eyeHeight, double z, float yaw, float pitch, bool onGround)
    {
        base.x = x;
        base.y = y;
        base.z = z;
        base.eyeHeight = eyeHeight;
        base.yaw = yaw;
        base.pitch = pitch;
        base.onGround = onGround;
        changeLook = true;
        changePosition = true;
    }

    public override void Read(DataInputStream stream)
    {
        x = stream.readDouble();
        y = stream.readDouble();
        eyeHeight = stream.readDouble();
        z = stream.readDouble();
        yaw = stream.readFloat();
        pitch = stream.readFloat();
        base.Read(stream);
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeDouble(x);
        stream.writeDouble(y);
        stream.writeDouble(eyeHeight);
        stream.writeDouble(z);
        stream.writeFloat(yaw);
        stream.writeFloat(pitch);
        base.Write(stream);
    }

    public override int Size()
    {
        return 41;
    }
}