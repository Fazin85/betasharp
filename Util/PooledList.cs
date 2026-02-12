using System.Buffers;
using System.Runtime.CompilerServices;

namespace betareborn.Util
{
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
                Grow();

            _buffer[_count++] = value;
        }

        private void Grow()
        {
            var newBuffer = ArrayPool<T>.Shared.Rent(_buffer.Length * 2);
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
}
