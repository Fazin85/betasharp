using BetaSharp.Worlds;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class BlockUpdateS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(BlockUpdateS2CPacket).TypeHandle);

    public int x;
    public int y;
    public int z;
    public int blockRawId;
    public int blockMetadata;

    public BlockUpdateS2CPacket()
    {
        worldPacket = false;
    }

    public BlockUpdateS2CPacket(int x, int y, int z, World world)
    {
        worldPacket = false;
        this.x = x;
        this.y = y;
        this.z = z;
        blockRawId = world.getBlockId(x, y, z);
        blockMetadata = world.getBlockMeta(x, y, z);
    }

    public override void read(DataInputStream stream)
    {
        x = stream.readInt();
        y = stream.read();
        z = stream.readInt();
        blockRawId = stream.read();
        blockMetadata = stream.read();
    }

    public override void write(DataOutputStream stream)
    {
        stream.writeInt(x);
        stream.write(y);
        stream.writeInt(z);
        stream.write(blockRawId);
        stream.write(blockMetadata);
    }

    public override void apply(NetHandler handler)
    {
        handler.onBlockUpdate(this);
    }

    public override int size()
    {
        return 11;
    }
}