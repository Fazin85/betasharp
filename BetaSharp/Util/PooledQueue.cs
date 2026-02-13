using System.Buffers;
using System.Runtime.CompilerServices;

namespace BetaSharp.Util;

public sealed class PooledQueue<T> : IDisposable/* where T : unmanaged*/
{
    private T[] _buffer;
    private int _head; // index of first element
    private int _tail; // index after last element
    private int _count;

    public int Count => _count;
    public bool IsEmpty => _count == 0;

    public PooledQueue(int initialCapacity = 16)
    {
        _buffer = ArrayPool<T>.Shared.Rent(initialCapacity);
        _head = 0;
        _tail = 0;
        _count = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(T item)
    {
        if (_count == _buffer.Length)
            Grow();

        _buffer[_tail] = item;
        _tail = (_tail + 1) % _buffer.Length;
        _count++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Dequeue()
    {
        if (_count == 0) throw new InvalidOperationException("Queue is empty");

        T item = _buffer[_head];
        _head = (_head + 1) % _buffer.Length;
        _count--;
        return item;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Peek()
    {
        if (_count == 0) throw new InvalidOperationException("Queue is empty");
        return _buffer[_head];
    }

    private void Grow()
    {
        int newSize = _buffer.Length * 2;
        T[] newBuffer = ArrayPool<T>.Shared.Rent(newSize);

        if (_head < _tail)
        {
            // contiguous
            Array.Copy(_buffer, _head, newBuffer, 0, _count);
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
        _tail = _count;
    }

    public void Dispose()
    {
        ArrayPool<T>.Shared.Return(_buffer, clearArray: false);
        _buffer = null!;
        _head = _tail = _count = 0;
    }
}