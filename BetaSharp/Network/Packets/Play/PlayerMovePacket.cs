using java.io;

namespace BetaSharp.Network.Packets.Play;

public class PlayerMovePacket : Packet
{
    public double x;
    public double y;
    public double z;
    public double eyeHeight;
    public float yaw;
    public float pitch;
    public bool onGround;
    public bool changePosition;
    public bool changeLook;

    public PlayerMovePacket()
    {
    }

    public PlayerMovePacket(bool onGround)
    {
        this.onGround = onGround;
    }

    public override void Apply(NetHandler handler)
    {
        handler.onPlayerMove(this);
    }

    public override void Read(DataInputStream stream)
    {
        onGround = stream.read() != 0;
    }

    public override void Write(DataOutputStream stream)
    {
        stream.write(onGround ? 1 : 0);
    }

    public override int Size()
    {
        return 1;
    }
}