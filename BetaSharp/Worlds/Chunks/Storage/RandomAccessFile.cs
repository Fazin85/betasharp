namespace BetaSharp.Worlds.Chunks.Storage;

/// <summary>
/// C# equivalent of Java's java.io.RandomAccessFile.
/// Supports random access reads and writes with big-endian byte order (matching Java's DataOutputStream).
/// </summary>
internal sealed class RandomAccessFile : IDisposable
{
    private readonly FileStream _stream;

    public RandomAccessFile(string path, string mode)
    {
        FileAccess access = mode == "r" ? FileAccess.Read : FileAccess.ReadWrite;
        FileMode fileMode = mode == "r" ? FileMode.OpenOrCreate : FileMode.OpenOrCreate;
        _stream = new FileStream(path, fileMode, access);
    }

    public long length() => _stream.Length;

    public void seek(long pos) => _stream.Seek(pos, SeekOrigin.Begin);

    public void write(int b) => _stream.WriteByte((byte)b);

    public void write(byte[] b) => _stream.Write(b, 0, b.Length);

    public void write(byte[] b, int off, int len) => _stream.Write(b, off, len);

    public void writeInt(int v)
    {
        _stream.WriteByte((byte)(v >> 24));
        _stream.WriteByte((byte)(v >> 16));
        _stream.WriteByte((byte)(v >> 8));
        _stream.WriteByte((byte)v);
    }

    public void writeByte(byte b) => _stream.WriteByte(b);

    public int readInt()
    {
        int b1 = _stream.ReadByte();
        int b2 = _stream.ReadByte();
        int b3 = _stream.ReadByte();
        int b4 = _stream.ReadByte();
        return (b1 << 24) | (b2 << 16) | (b3 << 8) | b4;
    }

    public byte readByte()
    {
        int b = _stream.ReadByte();
        if (b < 0) throw new EndOfStreamException();
        return (byte)b;
    }

    public int read(byte[] b) => _stream.Read(b, 0, b.Length);

    public void close() => _stream.Close();

    public void Dispose() => _stream.Dispose();
}
