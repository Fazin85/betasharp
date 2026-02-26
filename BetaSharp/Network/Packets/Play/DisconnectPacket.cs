using System.Net.Sockets;

namespace BetaSharp.Network.Packets.Play;

public class DisconnectPacket : Packet
{
    public string reason;

    public DisconnectPacket()
    {
    }

    public DisconnectPacket(string reason)
    {
        this.reason = reason;
    }

    public override void Read(NetworkStream stream)
    {
        reason = stream.ReadLongString(100);
    }

    public override void Write(NetworkStream stream)
    {
        stream.WriteLongString(reason);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onDisconnect(this);
    }

    public override int Size()
    {
        return reason.Length;
    }
}
