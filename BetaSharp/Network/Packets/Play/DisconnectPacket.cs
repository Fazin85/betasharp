using java.io;

namespace BetaSharp.Network.Packets.Play;

public class DisconnectPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(DisconnectPacket).TypeHandle);

    public string reason;

    public DisconnectPacket()
    {
    }

    public DisconnectPacket(string reason)
    {
        this.reason = reason;
    }

    public override void read(DataInputStream stream)
    {
        reason = readString(stream, 100);
    }

    public override void write(DataOutputStream stream)
    {
        writeString(reason, stream);
    }

    public override void apply(NetHandler handler)
    {
        handler.onDisconnect(this);
    }

    public override int size()
    {
        return reason.Length;
    }
}