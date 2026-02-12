using betareborn.Worlds;
using java.io;

namespace betareborn.Network.Packets.S2CPlay
{
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
            worldPacket = true;
        }

        public BlockUpdateS2CPacket(int x, int y, int z, World world)
        {
            worldPacket = true;
            this.x = x;
            this.y = y;
            this.z = z;
            blockRawId = world.getBlockId(x, y, z);
            blockMetadata = world.getBlockMeta(x, y, z);
        }

        public override void read(DataInputStream var1)
        {
            x = var1.readInt();
            y = var1.read();
            z = var1.readInt();
            blockRawId = var1.read();
            blockMetadata = var1.read();
        }

        public override void write(DataOutputStream var1)
        {
            var1.writeInt(x);
            var1.write(y);
            var1.writeInt(z);
            var1.write(blockRawId);
            var1.write(blockMetadata);
        }

        public override void apply(NetHandler var1)
        {
            var1.onBlockUpdate(this);
        }

        public override int size()
        {
            return 11;
        }
    }

}