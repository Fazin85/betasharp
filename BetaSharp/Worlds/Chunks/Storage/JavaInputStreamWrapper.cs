using java.io;

namespace BetaSharp.Worlds.Chunks.Storage;

public class JavaInputStreamWrapper : Stream
{
    private readonly InputStream javaStream;
    private bool disposed;

    public JavaInputStreamWrapper(InputStream javaStream)
    {
        this.javaStream = javaStream ?? throw new ArgumentNullException(nameof(javaStream));
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override void Flush()
    {
        // Input streams don't need to be flushed
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return javaStream.read(buffer, offset, count);
    }

    public override int ReadByte()
    {
        return javaStream.read();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposed)
        {
            disposed = true;
            if (disposing)
            {
                javaStream.close();
            }
        }
        base.Dispose(disposing);
    }
}
