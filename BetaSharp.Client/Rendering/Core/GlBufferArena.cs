using System.Collections.Generic;
using Silk.NET.OpenGL.Legacy;
using BetaSharp.Client.Rendering.Core;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Client.Rendering.Core;

public unsafe class GlBufferArena : IDisposable
{
    public class Segment
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
    public int Capacity => _capacity;
    public int Used => _used;
    public uint BufferId => _id;

    public bool IsEmpty => _used == 0;

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

    public Segment Allocate(int size)
    {
        Segment entry = FindFree(size);
        if (entry == null)
        {
            // If we have enough total free space, try compacting first
            if (_capacity - _used >= size)
            {
                Compact();
                entry = FindFree(size);
            }
            
            // If still no entry, we must grow
            if (entry == null)
            {
                Grow(size);
                entry = FindFree(size);
            }

            if (entry == null) return null; // Should definitely not happen now
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
        return entry;
    }

    private void Grow(int minRequiredSize)
    {
        int newCapacity = System.Math.Max(_capacity * 2, _used + minRequiredSize);
        Log.Instance.For<GlBufferArena>().LogInformation($"Growing GlBufferArena to {newCapacity} vertices");
        Compact(newCapacity);
    }

    public void Defragment() => Compact(); // Keeping name for compatibility but redirecting to Compact

    private void Compact(int newCapacity = -1)
    {
        if (_id == 0) return;
        if (newCapacity == -1) newCapacity = _capacity;

        uint newId = GLManager.GL.GenBuffer();
        GLManager.GL.BindBuffer(GLEnum.ArrayBuffer, newId);
        GLManager.GL.BufferData(GLEnum.ArrayBuffer, (nuint)(newCapacity * _stride), (void*)0, GLEnum.StaticDraw);

        if (_used > 0)
        {
            GLManager.GL.BindBuffer(GLEnum.CopyReadBuffer, _id);
            GLManager.GL.BindBuffer(GLEnum.CopyWriteBuffer, newId);

            int nextPackedOffset = 0;
            var usedSegments = new List<Segment>();
            Segment curr = _head;
            while (curr != null)
            {
                if (!curr.Free) usedSegments.Add(curr);
                curr = curr.Next;
            }

            int i = 0;
            while (i < usedSegments.Count)
            {
                Segment batchStart = usedSegments[i];
                int batchLength = batchStart.Length;
                int j = i + 1;
                
                while (j < usedSegments.Count && usedSegments[j].Offset == usedSegments[j - 1].End)
                {
                    batchLength += usedSegments[j].Length;
                    j++;
                }

                GLManager.GL.CopyBufferSubData(GLEnum.CopyReadBuffer, GLEnum.CopyWriteBuffer, 
                    (nint)(batchStart.Offset * _stride), 
                    (nint)(nextPackedOffset * _stride), 
                    (nuint)(batchLength * _stride));

                int offsetInBatch = 0;
                for (int k = i; k < j; k++)
                {
                    usedSegments[k].Offset = nextPackedOffset + offsetInBatch;
                    offsetInBatch += usedSegments[k].Length;
                }

                nextPackedOffset += batchLength;
                i = j;
            }

            // Sync the linked list
            if (usedSegments.Count > 0)
            {
                _head = usedSegments[0];
                _head.Prev = null;
                for (int k = 0; k < usedSegments.Count - 1; k++)
                {
                    usedSegments[k].Next = usedSegments[k + 1];
                    usedSegments[k + 1].Prev = usedSegments[k];
                }
                
                Segment last = usedSegments[^1];
                if (nextPackedOffset < newCapacity)
                {
                    last.Next = new Segment
                    {
                        Offset = nextPackedOffset,
                        Length = newCapacity - nextPackedOffset,
                        Free = true,
                        Prev = last,
                        Next = null
                    };
                }
                else
                {
                    last.Next = null;
                }
            }
            else
            {
                _head = new Segment { Offset = 0, Length = newCapacity, Free = true, Prev = null, Next = null };
            }
        }
        else
        {
            _head = new Segment { Offset = 0, Length = newCapacity, Free = true, Prev = null, Next = null };
        }

        GLManager.GL.DeleteBuffer(_id);
        _id = newId;
        _capacity = newCapacity;
        _version++;
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

    public void Free(Segment segment)
    {
        if (segment == null || segment.Free) return;

        segment.Free = true;
        _used -= segment.Length;

        // Merge with next
        if (segment.Next != null && segment.Next.Free)
        {
            segment.Length += segment.Next.Length;
            segment.Next = segment.Next.Next;
            if (segment.Next != null)
                segment.Next.Prev = segment;
        }

        // Merge with prev
        if (segment.Prev != null && segment.Prev.Free)
        {
            Segment prev = segment.Prev;
            prev.Length += segment.Length;
            prev.Next = segment.Next;
            if (segment.Next != null)
                segment.Next.Prev = prev;
        }
    }

    public void Upload<T>(Segment segment, Span<T> data) where T : unmanaged
    {
        GLManager.GL.BindBuffer(GLEnum.ArrayBuffer, _id);
        fixed (T* ptr = data)
        {
            GLManager.GL.BufferSubData(GLEnum.ArrayBuffer, (nint)(segment.Offset * _stride), (nuint)(data.Length * sizeof(T)), ptr);
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
