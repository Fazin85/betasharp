using System.Runtime.InteropServices;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Chunks;

/// <summary>
/// Provides a fixed-size queue for building a draw-command list usable with MultiDrawArrays.
/// Ported and simplified from Sodium's MultiDrawBatch.java.
/// </summary>
public unsafe class MultiDrawBatch : IDisposable
{
    private readonly int* _firsts;
    private readonly uint* _counts;
    private readonly int _capacity;
    private bool _disposed;

    public int Size { get; private set; }
    public bool IsFilled { get; set; }

    public MultiDrawBatch(int capacity)
    {
        _capacity = capacity;
        _firsts = (int*)NativeMemory.Alloc((nuint)(capacity * sizeof(int)));
        _counts = (uint*)NativeMemory.Alloc((nuint)(capacity * sizeof(uint)));
    }

    public void Add(int first, uint count)
    {
        if (Size >= _capacity)
        {
            return;
        }

        _firsts[Size] = first;
        _counts[Size] = count;
        Size++;
    }

    public void Clear()
    {
        Size = 0;
        IsFilled = false;
    }

    public void Delete()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        NativeMemory.Free(_firsts);
        NativeMemory.Free(_counts);
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    public int* Firsts => _firsts;
    public uint* Counts => _counts;

    public bool IsEmpty => Size == 0;
}
