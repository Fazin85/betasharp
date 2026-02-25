using java.io;

namespace BetaSharp.Network.Packets.Play;

public class UpdateSignPacket : Packet
{
    public int x;
    public int y;
    public int z;
    public string[] text;

    public UpdateSignPacket()
    {
        WorldPacket = true;
    }

    public UpdateSignPacket(int x, int y, int z, string[] text)
    {
        WorldPacket = true;
        this.x = x;
        this.y = y;
        this.z = z;
        this.text = text;
    }

    public override void Read(DataInputStream stream)
    {
        x = stream.readInt();
        y = stream.readShort();
        z = stream.readInt();
        text = new string[4];

        for (int i = 0; i < 4; ++i)
        {

            text[i] = ReadString(stream, 15);
        }

    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeInt(x);
        stream.writeShort(y);
        stream.writeInt(z);

        for (int i = 0; i < 4; ++i)
        {
            WriteString(text[i], stream);
        }

    }

    public override void Apply(NetHandler networkHandler)
    {
        networkHandler.handleUpdateSign(this);
    }

    public override int Size()
    {
        int size = 0;

        for (int i = 0; i < 4; ++i)
        {
            size += text[i].Length;
        }

        return size;
    }
}