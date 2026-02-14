namespace BetaSharp.Worlds.Chunks.Storage;

public class ChunkDataStream(Stream stream, byte compressionType)
{
    private readonly Stream stream = stream;
    private readonly byte compressionType = compressionType;

    public Stream getInputStream() => stream;
    public byte getCompressionType() => compressionType;
}