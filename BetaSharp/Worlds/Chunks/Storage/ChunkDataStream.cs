using java.io;

namespace BetaSharp.Worlds.Chunks.Storage;

public class ChunkDataStream(DataInputStream stream, byte compressionType)
{
    private readonly DataInputStream stream = stream;
    private readonly byte compressionType = compressionType;

    public DataInputStream getInputStream() => stream;
    public byte getCompressionType() => compressionType;
}