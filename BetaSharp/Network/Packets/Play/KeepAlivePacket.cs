using java.io;

namespace BetaSharp.Network.Packets.Play;

public class KeepAlivePacket : Packet
{
    public override void Apply(NetHandler handler)
    {
    }

    public override void Read(DataInputStream stream)
    {
    }

    public override void Write(DataOutputStream stream)
    {
    }

    public override int Size()
    {
        return 0;
    }
}
