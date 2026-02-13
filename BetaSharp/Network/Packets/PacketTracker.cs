namespace BetaSharp.Network.Packets;

public class PacketTracker
{
    private int count = 0;
    private long size = 0;

    public void update(int size)
    {
        ++count;
        this.size += size;
    }

    public PacketTracker()
    {
    }
}