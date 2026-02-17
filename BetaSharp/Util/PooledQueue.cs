using System.Buffers;
using System.Runtime.CompilerServices;

namespace BetaSharp.Util;

public sealed class PooledQueue<T> : IDisposable/* where T : unmanaged*/
{
    private T[] _buffer;
    private int _head; // index of first element
    private int _tail; // index after last element

    public int Count { get; private set; }

    public bool IsEmpty => Count == 0;

    public PooledQueue(int initialCapacity = 16)
    {
        _buffer = ArrayPool<T>.Shared.Rent(initialCapacity);
        _head = 0;
        _tail = 0;
        Count = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(T item)
    {
        if (Count == _buffer.Length)
            Grow();

        _buffer[_tail] = item;
        _tail = (_tail + 1) % _buffer.Length;
        Count++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Dequeue()
    {
        if (Count == 0) throw new InvalidOperationException("Queue is empty");

        T item = _buffer[_head];
        _head = (_head + 1) % _buffer.Length;
        Count--;
        return item;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Peek()
    {
        if (Count == 0) throw new InvalidOperationException("Queue is empty");
        return _buffer[_head];
    }

    private void Grow()
    {
        int newSize = _buffer.Length * 2;
        T[] newBuffer = ArrayPool<T>.Shared.Rent(newSize);

        if (_head < _tail)
        {
            // contiguous
            Array.Copy(_buffer, _head, newBuffer, 0, Count);
        }
        else
        {
            // wrap-around
            int rightCount = _buffer.Length - _head;
            Array.Copy(_buffer, _head, newBuffer, 0, rightCount);
            Array.Copy(_buffer, 0, newBuffer, rightCount, _tail);
        }

        ArrayPool<T>.Shared.Return(_buffer, clearArray: false);
        _buffer = newBuffer;
        _head = 0;
        _tail = Count;
    }

    public void Dispose()
    {
        ArrayPool<T>.Shared.Return(_buffer, clearArray: false);
        _buffer = null!;
        _head = _tail = Count = 0;
    }
}