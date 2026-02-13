using BetaSharp.Worlds;
using java.io;
using java.util.zip;

namespace BetaSharp.Network.Packets.S2CPlay;

public class ChunkDataS2CPacket : Packet
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(ChunkDataS2CPacket).TypeHandle);

    public int x;
    public int y;
    public int z;
    public int sizeX;
    public int sizeY;
    public int sizeZ;
    public byte[] chunkData;
    private int chunkDataSize;

    public ChunkDataS2CPacket()
    {
        worldPacket = true;
    }

    public ChunkDataS2CPacket(int x, int y, int z, int sizeX, int sizeY, int sizeZ, World world)
    {
        worldPacket = true;
        this.x = x;
        this.y = y;
        this.z = z;
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.sizeZ = sizeZ;
        byte[] chunkData = world.getChunkData(x, y, z, sizeX, sizeY, sizeZ);
        Deflater deflater = new(1);

        try
        {
            deflater.setInput(chunkData);
            deflater.finish();
            this.chunkData = new byte[sizeX * sizeY * sizeZ * 5 / 2];
            chunkDataSize = deflater.deflate(this.chunkData);
        }
        finally
        {
            deflater.end();
        }
    }

    public override void read(DataInputStream stream)
    {
        x = stream.readInt();
        y = stream.readShort();
        z = stream.readInt();
        sizeX = stream.read() + 1;
        sizeY = stream.read() + 1;
        sizeZ = stream.read() + 1;
        chunkDataSize = stream.readInt();
        byte[]
            chunkData = new byte[chunkDataSize];
        stream.readFully(chunkData);

        this.chunkData = new byte[sizeX * sizeY * sizeZ * 5 / 2];
        Inflater inflater = new Inflater();
        inflater.setInput(chunkData);

        try
        {
            inflater.inflate(this.chunkData);
        }
        catch (DataFormatException ex)
        {
            throw new java.io.IOException("Bad compressed data format");
        }
        finally
        {
            inflater.end();
        }

    }

    public override void write(DataOutputStream stream)
    {
        stream.writeInt(x);
        stream.writeShort(y);
        stream.writeInt(z);
        stream.write(sizeX - 1);
        stream.write(sizeY - 1);
        stream.write(sizeZ - 1);
        stream.writeInt(chunkDataSize);
        stream.write(chunkData, 0, chunkDataSize);
    }

    public override void apply(NetHandler handler)
    {
        handler.handleChunkData(this);
    }

    public override int size()
    {
        return 17 + chunkDataSize;
    }
}