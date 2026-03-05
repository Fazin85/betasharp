using Silk.NET.Maths;

namespace Beta3D;

public class Std140Builder
{
    private readonly byte[] _buffer;
    private int _position;
    private readonly int _start;

    private Std140Builder(byte[] buffer)
    {
        _buffer = buffer;
        _start = 0;
        _position = 0;
    }

    public static Std140Builder IntoBuffer(byte[] buffer) => new(buffer);
    public static Std140Builder WithSize(int size) => new(new byte[size]);

    public ReadOnlySpan<byte> AsSpan => _buffer.AsSpan(0, _position);

    private Std140Builder Align(int alignment)
    {
        int relative = _position - _start;
        int aligned = (relative + alignment - 1) & ~(alignment - 1);
        _position = _start + aligned;
        return this;
    }

    private void WriteBytes(ReadOnlySpan<byte> bytes)
    {
        bytes.CopyTo(_buffer.AsSpan(_position));
        _position += bytes.Length;
    }

    public Std140Builder Float(float value)
    {
        Align(4);
        WriteBytes(BitConverter.GetBytes(value));
        return this;
    }

    public Std140Builder Int(int value)
    {
        Align(4);
        WriteBytes(BitConverter.GetBytes(value));
        return this;
    }

    public Std140Builder Vec2(float x, float y)
    {
        Align(8);
        WriteBytes(BitConverter.GetBytes(x));
        WriteBytes(BitConverter.GetBytes(y));
        return this;
    }

    public Std140Builder IVec2(int x, int y)
    {
        Align(8);
        WriteBytes(BitConverter.GetBytes(x));
        WriteBytes(BitConverter.GetBytes(y));
        return this;
    }

    public Std140Builder Vec3(float x, float y, float z)
    {
        Align(16);
        WriteBytes(BitConverter.GetBytes(x));
        WriteBytes(BitConverter.GetBytes(y));
        WriteBytes(BitConverter.GetBytes(z));
        _position += 4; // std140 padding
        return this;
    }

    public Std140Builder IVec3(int x, int y, int z)
    {
        Align(16);
        WriteBytes(BitConverter.GetBytes(x));
        WriteBytes(BitConverter.GetBytes(y));
        WriteBytes(BitConverter.GetBytes(z));
        _position += 4; // std140 padding
        return this;
    }

    public Std140Builder Vec4(float x, float y, float z, float w)
    {
        Align(16);
        WriteBytes(BitConverter.GetBytes(x));
        WriteBytes(BitConverter.GetBytes(y));
        WriteBytes(BitConverter.GetBytes(z));
        WriteBytes(BitConverter.GetBytes(w));
        return this;
    }

    public Std140Builder Vec4(Vector4D<float> vec)
    {
        return Vec4(vec.X, vec.Y, vec.Z, vec.W);
    }

    public Std140Builder IVec4(int x, int y, int z, int w)
    {
        Align(16);
        WriteBytes(BitConverter.GetBytes(x));
        WriteBytes(BitConverter.GetBytes(y));
        WriteBytes(BitConverter.GetBytes(z));
        WriteBytes(BitConverter.GetBytes(w));
        return this;
    }

    public Std140Builder Mat4(Matrix4X4<float> mat)
    {
        Align(16);
        Vec4(mat.Column1);
        Vec4(mat.Column2);
        Vec4(mat.Column3);
        Vec4(mat.Column4);
        return this;
    }
}
