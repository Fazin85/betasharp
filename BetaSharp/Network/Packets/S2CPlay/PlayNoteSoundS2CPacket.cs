using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class PlayNoteSoundS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(PlayNoteSoundS2CPacket).TypeHandle);

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

    public override void read(DataInputStream stream)
    {
        xLocation = stream.readInt();
        yLocation = stream.readShort();
        zLocation = stream.readInt();
        instrumentType = stream.read();
        pitch = stream.read();
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeInt(xLocation);
        stream.writeShort(yLocation);
        stream.writeInt(zLocation);
        stream.write(instrumentType);
        stream.write(pitch);
    }

    public override void apply(NetHandler handler)
    {
        handler.onPlayNoteSound(this);
    }

    public override int size()
    {
        return 12;
    }
}