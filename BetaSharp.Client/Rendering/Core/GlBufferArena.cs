using System.Collections.Generic;
using Silk.NET.OpenGL.Legacy;
using BetaSharp.Client.Rendering.Core;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Client.Rendering.Core;

public unsafe class GlBufferArena : IDisposable
{
    private class Segment
    {
        public int Offset;
        public int Length;
        public bool Free;
        public Segment Prev;
        public Segment Next;

        public int End => Offset + Length;
    }

    private uint _id;
    private int _capacity;
    private int _used;
    private readonly int _stride;
    private Segment _head;
    private bool _disposed;
    private int _version;

    public int Version => _version;

    public GlBufferArena(int initialCapacity, int stride)
    {
        _stride = stride;
        _capacity = initialCapacity;
        _id = GLManager.GL.GenBuffer();
        GLManager.GL.BindBuffer(GLEnum.ArrayBuffer, _id);
        GLManager.GL.BufferData(GLEnum.ArrayBuffer, (nuint)(initialCapacity * stride), (void*)0, GLEnum.StaticDraw);

        _head = new Segment
        {
            Offset = 0,
            Length = initialCapacity,
            Free = true
        };
    }

    public uint BufferId => _id;

    public int Allocate(int size)
    {
        Segment entry = FindFree(size);
        if (entry == null)
        {
            Grow(size);
            entry = FindFree(size);
            if (entry == null) return -1; // Should not happen after grow
        }

        if (entry.Length == size)
        {
            entry.Free = false;
        }
        else
        {
            Segment newSegment = new Segment
            {
                Offset = entry.Offset,
                Length = size,
                Free = false,
                Prev = entry.Prev,
                Next = entry
            };

            if (entry.Prev != null)
                entry.Prev.Next = newSegment;
            else
                _head = newSegment;

            entry.Prev = newSegment;
            entry.Offset += size;
            entry.Length -= size;
            
            entry = newSegment;
        }

        _used += size;
        return entry.Offset;
    }

    private void Grow(int minRequiredSize)
    {
        int newCapacity = System.Math.Max(_capacity * 2, _used + minRequiredSize);
        uint newId = GLManager.GL.GenBuffer();
        
        GLManager.GL.BindBuffer(GLEnum.ArrayBuffer, newId);
        GLManager.GL.BufferData(GLEnum.ArrayBuffer, (nuint)(newCapacity * _stride), (void*)0, GLEnum.StaticDraw);

        if (_id != 0)
        {
            GLManager.GL.BindBuffer(GLEnum.CopyReadBuffer, _id);
            GLManager.GL.BindBuffer(GLEnum.CopyWriteBuffer, newId);
            GLManager.GL.CopyBufferSubData(GLEnum.CopyReadBuffer, GLEnum.CopyWriteBuffer, 0, 0, (nuint)(_capacity * _stride));
            GLManager.GL.DeleteBuffer(_id);
        }

        // Add a new free segment or extend the last one
        Segment curr = _head;
        while (curr.Next != null) curr = curr.Next;

        if (curr.Free)
        {
            curr.Length += (newCapacity - _capacity);
        }
        else
        {
            Segment newFree = new Segment
            {
                Offset = curr.End,
                Length = newCapacity - _capacity,
                Free = true,
                Prev = curr,
                Next = null
            };
            curr.Next = newFree;
        }

        _id = newId;
        _capacity = newCapacity;
        _version++;
        
        Log.Instance.For<GlBufferArena>().LogInformation($"GlBufferArena growing to {_capacity} vertices ({_capacity * _stride / 1024 / 1024} MB), Version: {_version}");
    }

    private Segment FindFree(int size)
    {
        Segment curr = _head;
        while (curr != null)
        {
            if (curr.Free && curr.Length >= size)
                return curr;
            curr = curr.Next;
        }
        return null;
    }

    public void Free(int offset)
    {
        Segment curr = _head;
        while (curr != null)
        {
            if (curr.Offset == offset)
            {
                if (curr.Free) return;

                curr.Free = true;
                _used -= curr.Length;

                // Merge with next
                if (curr.Next != null && curr.Next.Free)
                {
                    curr.Length += curr.Next.Length;
                    curr.Next = curr.Next.Next;
                    if (curr.Next != null)
                        curr.Next.Prev = curr;
                }

                // Merge with prev
                if (curr.Prev != null && curr.Prev.Free)
                {
                    Segment prev = curr.Prev;
                    prev.Length += curr.Length;
                    prev.Next = curr.Next;
                    if (curr.Next != null)
                        curr.Next.Prev = prev;
                }
                return;
            }
            curr = curr.Next;
        }
    }

    public void Upload<T>(int offset, Span<T> data) where T : unmanaged
    {
        GLManager.GL.BindBuffer(GLEnum.ArrayBuffer, _id);
        fixed (T* ptr = data)
        {
            GLManager.GL.BufferSubData(GLEnum.ArrayBuffer, (nint)(offset * _stride), (nuint)(data.Length * sizeof(T)), ptr);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        if (_id != 0)
        {
            GLManager.GL.DeleteBuffer(_id);
            _id = 0;
        }
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
