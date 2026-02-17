namespace BetaSharp.Worlds.Chunks.Storage;
using java.io;

public class ChunkDataStream(Stream stream, byte compressionType) : IDisposable
{
    public Stream Stream => stream;
    public byte CompressionType => compressionType;

    public void Dispose()
    {
        stream.Dispose();
    }
}