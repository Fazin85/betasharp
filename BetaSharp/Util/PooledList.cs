using System.Buffers;
using System.Runtime.CompilerServices;

namespace BetaSharp.Util;

public sealed class PooledList<T>(int initialCapacity = 16) : IDisposable where T : unmanaged
{
    public int Count { get; private set; }

    public T[] Buffer { get; private set; } = ArrayPool<T>.Shared.Rent(initialCapacity);

    public Span<T> Span => Buffer.AsSpan(0, Count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T value)
    {
        if (Count == Buffer.Length)
            Grow(Count + 1);

        Buffer[Count++] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddRange(ReadOnlySpan<T> values)
    {
        if (Count + values.Length > Buffer.Length)
            Grow(Count + values.Length);

        values.CopyTo(Buffer.AsSpan(Count));
        Count += values.Length;
    }

    private void Grow(int minCapacity)
    {
        int newSize = Buffer.Length * 2;
        if (newSize < minCapacity)
            newSize = minCapacity;

        var newBuffer = ArrayPool<T>.Shared.Rent(newSize);
        Array.Copy(Buffer, newBuffer, Count);
        ArrayPool<T>.Shared.Return(Buffer, clearArray: false);
        Buffer = newBuffer;
    }

    public void Dispose()
    {
        if (Buffer != null)
        {
            ArrayPool<T>.Shared.Return(Buffer, clearArray: false);
            Buffer = null!;
            Count = 0;
        }
    }
}
