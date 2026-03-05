namespace Beta3D;

public abstract class Buffer(Buffer.BufferUsage usage, long size) : IDisposable
{
    [Flags]
    public enum BufferUsage : uint
    {
        MapRead = 1,
        MapWrite = 2,
        HintClientStorage = 4,
        CopyDst = 8,
        CopySrc = 16,
        Vertex = 32,
        Index = 64,
        Uniform = 128,
        UniformTexelBuffer = 256
    }

    public BufferUsage Usage { get; } = usage;
    public long Size { get; } = size;

    public abstract bool IsDisposed { get; }

    public BufferSlice Slice(long offset, long length)
    {
        if (offset >= 0L && length >= 0L && offset + length <= Size)
        {
            return new BufferSlice(this, offset, length);
        }
        else
        {
            throw new ArgumentException("Offset of " + offset + " and length " + length + " would put new slice outside buffer's range (of 0," + Size + ")");
        }
    }

    public BufferSlice Slice()
    {
        return new(this, 0, Size);
    }

    public abstract void Dispose();
}
