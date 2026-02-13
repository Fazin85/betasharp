using java.io;

namespace BetaSharp.Network.Packets.Play;

public class UpdateSignPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(UpdateSignPacket).TypeHandle);

    public int x;
    public int y;
    public int z;
    public string[] text;

    public UpdateSignPacket()
    {
        worldPacket = true;
    }

    public UpdateSignPacket(int x, int y, int z, string[] text)
    {
        worldPacket = true;
        this.x = x;
        this.y = y;
        this.z = z;
        this.text = text;
    }

    public override void read(DataInputStream stream)
    {
        x = stream.readInt();
        y = stream.readShort();
        z = stream.readInt();
        text = new string[4];

        for (int i = 0; i < 4; ++i)
        {

            text[i] = readString(stream, 15);
        }

    }

    public override void write(DataOutputStream stream)
    {
        stream.writeInt(x);
        stream.writeShort(y);
        stream.writeInt(z);

        for (int i = 0; i < 4; ++i)
        {
            writeString(text[i], stream);
        }

    }

    public override void apply(NetHandler networkHandler)
    {
        networkHandler.handleUpdateSign(this);
    }

    public override int size()
    {
        int size = 0;

        for (int i = 0; i < 4; ++i)
        {
            size += text[i].Length;
        }

        return size;
    }
}