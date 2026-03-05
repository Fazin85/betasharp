namespace Beta3D;

public record struct BufferSlice(Buffer Buffer, long Offset, long Length)
{
    public readonly BufferSlice Slice(long offset, long length)
    {
        if (offset >= 0L && length >= 0L && offset + length <= Length)
        {
            return new BufferSlice(Buffer, Offset + offset, length);
        }
        else
        {
            throw new ArgumentOutOfRangeException($"Offset of {offset} and length {length} would put new slice outside existing slice's range (of {Offset}, {Length})");
        }
    }
}
