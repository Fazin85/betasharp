using java.io;

namespace BetaSharp.Network.Packets;

public class HandshakePacket : Packet
{
    public string username;

    public HandshakePacket()
    {
    }

    public HandshakePacket(string username)
    {
        this.username = username;
    }

    public override void Read(DataInputStream stream)
    {
        username = ReadString(stream, 32);
    }

    public override void Write(DataOutputStream stream)
    {
        WriteString(username, stream);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onHandshake(this);
    }

    public override int Size()
    {
        return 4 + username.Length + 4;
    }
}
