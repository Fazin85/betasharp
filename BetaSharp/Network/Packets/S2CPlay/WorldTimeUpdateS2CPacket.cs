using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class WorldTimeUpdateS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(WorldTimeUpdateS2CPacket).TypeHandle);

    public long time;

    public WorldTimeUpdateS2CPacket()
    {
    }

    public WorldTimeUpdateS2CPacket(long time)
    {
        this.time = time;
    }

    public override void read(DataInputStream stream)
    {
        time = stream.readLong();
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeLong(time);
    }

    public override void apply(NetHandler handler)
    {
        handler.onWorldTimeUpdate(this);
    }

    public override int size()
    {
        return 8;
    }
}