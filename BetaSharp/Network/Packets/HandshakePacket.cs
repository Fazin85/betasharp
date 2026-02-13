using java.io;

namespace BetaSharp.Network.Packets;

public class HandshakePacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(HandshakePacket).TypeHandle);

    public string username;

    public HandshakePacket()
    {
    }

    public HandshakePacket(string username)
    {
        this.username = username;
    }

    public override void read(DataInputStream stream)
    {
        username = readString(stream, 32);
    }

    public override void write(DataOutputStream stream)
    {
        writeString(username, stream);
    }

    public override void apply(NetHandler handler)
    {
        handler.onHandshake(this);
    }

    public override int size()
    {
        return 4 + username.Length + 4;
    }
}