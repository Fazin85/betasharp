using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class ChunkStatusUpdateS2CPacket : Packet
{
    public int x;
    public int z;
    public bool load;

    public ChunkStatusUpdateS2CPacket()
    {
        WorldPacket = false;
    }

    public ChunkStatusUpdateS2CPacket(int x, int z, bool load)
    {
        WorldPacket = false;
        this.x = x;
        this.z = z;
        this.load = load;
    }

    public override void Read(DataInputStream stream)
    {
        x = stream.readInt();
        z = stream.readInt();
        load = stream.read() != 0;
    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeInt(x);
        stream.writeInt(z);
        stream.write(load ? 1 : 0);
    }

    public override void Apply(NetHandler handler)
    {
        handler.onChunkStatusUpdate(this);
    }

    public override int Size()
    {
        return 9;
    }
}