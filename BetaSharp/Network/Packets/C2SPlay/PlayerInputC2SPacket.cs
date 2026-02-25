using java.io;

namespace BetaSharp.Network.Packets.C2SPlay;

public class PlayerInputC2SPacket : Packet
{
    private float sideways;
    private float forward;
    private bool jumping;
    private bool sneaking;
    private float pitch;
    private float yaw;

    public override void Read(DataInputStream stream)
    {
        sideways = stream.readFloat();
        forward = stream.readFloat();
        pitch = stream.readFloat();
        yaw = stream.readFloat();
        jumping = stream.readBoolean();
        sneaking = stream.readBoolean();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeFloat(sideways);
        stream.writeFloat(forward);
        stream.writeFloat(pitch);
        stream.writeFloat(yaw);
        stream.writeBoolean(jumping);
        stream.writeBoolean(sneaking);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onPlayerInput(this);
    }

    public override int Size()
    {
        return 18;
    }

    public float getSideways()
    {
        return sideways;
    }

    public float getPitch()
    {
        return pitch;
    }

    public float getForward()
    {
        return forward;
    }

    public float getYaw()
    {
        return yaw;
    }

    public bool isJumping()
    {
        return jumping;
    }

    public bool isSneaking()
    {
        return sneaking;
    }
}