using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class PlayNoteSoundS2CPacket : Packet
{
    public int xLocation;
    public int yLocation;
    public int zLocation;
    public int instrumentType;
    public int pitch;

    public PlayNoteSoundS2CPacket()
    {
    }

    public PlayNoteSoundS2CPacket(int x, int y, int z, int instrument, int pitch)
    {
        xLocation = x;
        yLocation = y;
        zLocation = z;
        instrumentType = instrument;
        this.pitch = pitch;
    }

    public override void Read(DataInputStream stream)
    {
        xLocation = stream.readInt();
        yLocation = stream.readShort();
        zLocation = stream.readInt();
        instrumentType = stream.read();
        pitch = stream.read();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeInt(xLocation);
        stream.writeShort(yLocation);
        stream.writeInt(zLocation);
        stream.write(instrumentType);
        stream.write(pitch);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onPlayNoteSound(this);
    }

    public override int Size()
    {
        return 12;
    }
}