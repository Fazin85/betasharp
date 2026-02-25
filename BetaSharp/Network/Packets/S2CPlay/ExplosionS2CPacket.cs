using BetaSharp.Util.Maths;
using java.io;

namespace BetaSharp.Network.Packets.S2CPlay;

public class ExplosionS2CPacket : Packet
{
    public double explosionX;
    public double explosionY;
    public double explosionZ;
    public float explosionSize;
    public HashSet<BlockPos> destroyedBlockPositions;

    public ExplosionS2CPacket()
    {
    }

    public ExplosionS2CPacket(double x, double y, double z, float radius, HashSet<BlockPos> affectedBlocks)
    {
        explosionX = x;
        explosionY = y;
        explosionZ = z;
        explosionSize = radius;
        destroyedBlockPositions = new HashSet<BlockPos>(affectedBlocks);
    }

    public override void Read(DataInputStream stream)
    {
        explosionX = stream.readDouble();
        explosionY = stream.readDouble();
        explosionZ = stream.readDouble();
        explosionSize = stream.readFloat();
        int blockCount = stream.readInt();
        destroyedBlockPositions = new HashSet<BlockPos>();
        int x = (int)explosionX;
        int y = (int)explosionY;
        int z = (int)explosionZ;

        for (int _ = 0; _ < blockCount; ++_)
        {
            int xOffset = (sbyte)stream.readByte() + x;
            int yOffset = (sbyte)stream.readByte() + y;
            int zOffset = (sbyte)stream.readByte() + z;

            destroyedBlockPositions.Add(new BlockPos(xOffset, yOffset, zOffset));
        }

    }

    public override void Write(DataOutputStream stream)
    {
        stream.writeDouble(explosionX);
        stream.writeDouble(explosionY);
        stream.writeDouble(explosionZ);
        stream.writeFloat(explosionSize);
        stream.writeInt(destroyedBlockPositions.Count);
        int x = (int)explosionX;
        int y = (int)explosionY;
        int z = (int)explosionZ;
        foreach (var pos in destroyedBlockPositions)
        {
            int xOffset = pos.x - x;
            int yOffset = pos.y - y;
            int zOffset = pos.z - z;
            stream.writeByte(xOffset);
            stream.writeByte(yOffset);
            stream.writeByte(zOffset);
        }
    }

    public override void Apply(NetHandler handler)
    {
        handler.onExplosion(this);
    }

    public override int Size()
    {
        return 32 + destroyedBlockPositions.Count * 3;
    }
}