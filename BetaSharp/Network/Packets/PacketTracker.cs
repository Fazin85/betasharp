namespace BetaSharp.Network.Packets;

public class PacketTracker
{
    private int count;
    private long size;

    public void update(int size)
    {
        ++count;
        this.size += size;
    }

    public PacketTracker()
    {
    }
}