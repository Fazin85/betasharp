using System.Buffers.Binary;
using BetaSharp.Util;

namespace BetaSharp.NBT;

internal static class StreamExtensions
{
    public static void WriteShort(this Stream stream, short value)
    {
        Span<byte> span = stackalloc byte[sizeof(short)];
        BinaryPrimitives.WriteInt16BigEndian(span, value);
        stream.Write(span);
    }

    public static void WriteUShort(this Stream stream, ushort value)
    {
        Span<byte> span = stackalloc byte[sizeof(ushort)];
        BinaryPrimitives.WriteUInt16BigEndian(span, value);
        stream.Write(span);
    }

    public static void WriteInt(this Stream stream, int value)
    {
        Span<byte> span = stackalloc byte[sizeof(int)];
        BinaryPrimitives.WriteInt32BigEndian(span, value);
        stream.Write(span);
    }

    public static void WriteFloat(this Stream stream, float value)
    {
        Span<byte> span = stackalloc byte[sizeof(float)];
        BinaryPrimitives.WriteSingleBigEndian(span, value);
        stream.Write(span);
    }

    public static void WriteDouble(this Stream stream, double value)
    {
        Span<byte> span = stackalloc byte[sizeof(double)];
        BinaryPrimitives.WriteDoubleBigEndian(span, value);
        stream.Write(span);
    }

    public static void WriteLong(this Stream stream, long value)
    {
        Span<byte> span = stackalloc byte[sizeof(long)];
        BinaryPrimitives.WriteInt64BigEndian(span, value);
        stream.Write(span);
    }

    public static void WriteString(this Stream stream, string value)
    {
        byte[] buffer = ModifiedUtf8.GetBytes(value);

        stream.WriteUShort((ushort)buffer.Length);
        stream.Write(buffer);
    }

    public static short ReadShort(this Stream stream)
    {
        Span<byte> span = stackalloc byte[sizeof(short)];
        stream.ReadExactly(span);

        return BinaryPrimitives.ReadInt16BigEndian(span);
    }

    public static ushort ReadUShort(this Stream stream)
    {
        Span<byte> span = stackalloc byte[sizeof(ushort)];
        stream.ReadExactly(span);

        return BinaryPrimitives.ReadUInt16BigEndian(span);
    }

    public static int ReadInt(this Stream stream)
    {
        Span<byte> span = stackalloc byte[sizeof(int)];
        stream.ReadExactly(span);

        return BinaryPrimitives.ReadInt32BigEndian(span);
    }

    public static float ReadFloat(this Stream stream)
    {
        Span<byte> span = stackalloc byte[sizeof(float)];
        stream.ReadExactly(span);

        return BinaryPrimitives.ReadSingleBigEndian(span);
    }

    public static double ReadDouble(this Stream stream)
    {
        Span<byte> span = stackalloc byte[sizeof(double)];
        stream.ReadExactly(span);

        return BinaryPrimitives.ReadDoubleBigEndian(span);
    }

    public static long ReadLong(this Stream stream)
    {
        Span<byte> span = stackalloc byte[sizeof(long)];
        stream.ReadExactly(span);

        return BinaryPrimitives.ReadInt64BigEndian(span);
    }

    public static string ReadString(this Stream stream)
    {
        ushort length = stream.ReadUShort();
        byte[] buffer = new byte[length];

        stream.ReadExactly(buffer);

        return ModifiedUtf8.GetString(buffer);
    }
}
