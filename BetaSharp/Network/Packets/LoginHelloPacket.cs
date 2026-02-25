using java.io;

namespace BetaSharp.Network.Packets;

public class LoginHelloPacket : Packet
{
    public const long BETASHARP_CLIENT_SIGNATURE = 0x627368617270; // "bsharp" in hex. Used to identify BetaSharp clients for future protocol extensions without breaking vanilla compatibility.

    public int protocolVersion;
    public string username;
    public long worldSeed;
    public sbyte dimensionId;

    public LoginHelloPacket()
    {
    }

    public LoginHelloPacket(string username, int protocolVersion, long worldSeed, sbyte dimensionId)
    {
        this.username = username;
        this.protocolVersion = protocolVersion;
        this.worldSeed = worldSeed;
        this.dimensionId = dimensionId;
    }

    public LoginHelloPacket(string username, int protocolVersion)
    {
        this.username = username;
        this.protocolVersion = protocolVersion;
    }

    public override void Read(DataInputStream stream)
    {
        protocolVersion = stream.readInt();
        username = ReadString(stream, 16);
        worldSeed = stream.readLong();
        dimensionId = (sbyte)stream.readByte();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeInt(protocolVersion);
        WriteString(username, stream);
        stream.writeLong(worldSeed);
        stream.writeByte(dimensionId);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onHello(this);
    }

    public override int Size()
    {
        return 4 + username.Length + 4 + 5;
    }
}
