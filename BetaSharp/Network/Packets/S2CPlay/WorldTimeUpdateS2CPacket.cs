using System.Net.Sockets;

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

    public override void Read(NetworkStream stream)
    {
        time = stream.ReadLong();
    }

    public override void Write(NetworkStream stream)
    {
        stream.WriteLong(time);
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
