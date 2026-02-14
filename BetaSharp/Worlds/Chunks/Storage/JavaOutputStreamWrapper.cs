using java.io;

namespace BetaSharp.Worlds.Chunks.Storage;

public class JavaOutputStreamWrapper : Stream
{
    private readonly OutputStream javaStream;
    private bool disposed;

    public JavaOutputStreamWrapper(OutputStream javaStream)
    {
        this.javaStream = javaStream ?? throw new ArgumentNullException(nameof(javaStream));
    }

    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override void Flush()
    {
        javaStream.flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
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
        javaStream.write(buffer, offset, count);
    }

    public override void WriteByte(byte value)
    {
        javaStream.write(value);
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
