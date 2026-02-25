using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class WorldTimeUpdateS2CPacket : Packet
{
    public long time;

    public WorldTimeUpdateS2CPacket()
    {
    }

    public WorldTimeUpdateS2CPacket(long time)
    {
        this.time = time;
    }

    public override void Read(DataInputStream stream)
    {
        time = stream.readLong();
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeLong(time);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onWorldTimeUpdate(this);
    }

    public override int Size()
    {
        return 8;
    }
}