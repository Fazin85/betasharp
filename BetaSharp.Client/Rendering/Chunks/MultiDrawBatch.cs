using System.Runtime.InteropServices;

namespace BetaSharp.Client.Rendering.Chunks;

/// <summary>
/// Provides a fixed-size queue for building a draw-command list usable with MultiDrawArrays.
/// </summary>
public unsafe class MultiDrawBatch : IDisposable
{
    private readonly int _capacity;
    private bool _disposed;

    public int Size { get; private set; }
    public bool IsFilled { get; set; }

    public MultiDrawBatch(int capacity)
    {
        _capacity = capacity;
        Firsts = (int*)NativeMemory.Alloc((nuint)(capacity * sizeof(int)));
        Counts = (uint*)NativeMemory.Alloc((nuint)(capacity * sizeof(uint)));
    }

    public void Add(int first, uint count)
    {
        if (Size >= _capacity)
        {
            return;
        }

        Firsts[Size] = first;
        Counts[Size] = count;
        Size++;
    }

    public void Clear()
    {
        Size = 0;
        IsFilled = false;
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        NativeMemory.Free(Firsts);
        NativeMemory.Free(Counts);
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    public int* Firsts { get; }
    public uint* Counts { get; }

    public bool IsEmpty => Size == 0;
}
