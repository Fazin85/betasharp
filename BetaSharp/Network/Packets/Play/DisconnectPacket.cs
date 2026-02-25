using java.io;

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

    public override void Read(DataInputStream stream)
    {
        reason = ReadString(stream, 100);
    }

    public override void Write(DataOutputStream stream)
    {
        WriteString(reason, stream);
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