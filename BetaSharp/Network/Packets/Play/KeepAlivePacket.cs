using java.io;

namespace BetaSharp.Network.Packets.Play;

public class KeepAlivePacket : Packet
{
    public override void apply(NetHandler handler)
    {
    }

    public override void read(DataInputStream stream)
    {
    }

    public override void write(DataOutputStream stream)
    {
    }

    public override int size()
    {
        return 0;
    }
}