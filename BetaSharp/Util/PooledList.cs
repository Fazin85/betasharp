using System.Buffers;
using System.Runtime.CompilerServices;

namespace BetaSharp.Util;

public sealed class PooledList<T>(int initialCapacity = 16) : IDisposable where T : unmanaged
{
    private T[] _buffer = ArrayPool<T>.Shared.Rent(initialCapacity);
    private int _count = 0;

    public int Count => _count;
    public T[] Buffer => _buffer;
    public Span<T> Span => _buffer.AsSpan(0, _count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T value)
    {
        if (_count == _buffer.Length)
            Grow(_count + 1);

        _buffer[_count++] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddRange(ReadOnlySpan<T> values)
    {
        if (_count + values.Length > _buffer.Length)
            Grow(_count + values.Length);

        values.CopyTo(_buffer.AsSpan(_count));
        _count += values.Length;
    }

    private void Grow(int minCapacity)
    {
        int newSize = _buffer.Length * 2;
        if (newSize < minCapacity)
            newSize = minCapacity;

        var newBuffer = ArrayPool<T>.Shared.Rent(newSize);
        Array.Copy(_buffer, newBuffer, _count);
        ArrayPool<T>.Shared.Return(_buffer, clearArray: false);
        _buffer = newBuffer;
    }

    public void Dispose()
    {
        if (_buffer != null)
        {
            ArrayPool<T>.Shared.Return(_buffer, clearArray: false);
            _buffer = null!;
            _count = 0;
        }
    }
}