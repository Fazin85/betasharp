using System.Runtime.InteropServices;
using BetaSharp.Util;
using BetaSharp.Util.Maths;
using Silk.NET.Maths;
using Silk.NET.OpenGL.Legacy;
using BetaSharp.Client.Rendering.Core;
using Microsoft.Extensions.Logging;
using VertexArray = BetaSharp.Client.Rendering.Core.VertexArray;

namespace BetaSharp.Client.Rendering.Chunks;

public class RenderRegion : IDisposable
{
    public const int Width = 8;
    public const int Height = 4;
    public const int Depth = 8;
    public const int SectionCount = Width * Height * Depth;

    public Vector3D<int> Origin { get; }
    
    private readonly GlBufferArena[] _arenas = new GlBufferArena[2];
    private readonly VertexArray[] _vaos = new VertexArray[2];
    private readonly MultiDrawBatch[] _batches = new MultiDrawBatch[2];
    private readonly int[] _arenaVersions = new int[2];
    
    // Offsets and counts for each section in each pass [pass][sectionIndex]
    private readonly int[,] _offsets = new int[2, SectionCount];
    private readonly int[,] _counts = new int[2, SectionCount];

    private bool _disposed;

    public RenderRegion(Vector3D<int> origin)
    {
        Origin = origin;

        // Initialize arenas with larger capacities for 8x4x8 regions
        // Growth will handle overflow.
        _arenas[0] = new GlBufferArena(524288, 16);
        _arenas[1] = new GlBufferArena(131072, 16);

        _vaos[0] = new VertexArray();
        SetupVao(_vaos[0], _arenas[0]);

        _vaos[1] = new VertexArray();
        SetupVao(_vaos[1], _arenas[1]);

        _batches[0] = new MultiDrawBatch(SectionCount);
        _batches[1] = new MultiDrawBatch(SectionCount);

        for (int i = 0; i < SectionCount; i++)
        {
            _offsets[0, i] = -1;
            _offsets[1, i] = -1;
        }
    }

    private unsafe void SetupVao(VertexArray vao, GlBufferArena arena)
    {
        vao.Bind();
        GLManager.GL.BindBuffer(GLEnum.ArrayBuffer, arena.BufferId);

        const uint stride = 16;

        GLManager.GL.EnableVertexAttribArray(0);
        GLManager.GL.VertexAttribPointer(0, 3, GLEnum.Short, false, stride, (void*)4);

        GLManager.GL.EnableVertexAttribArray(1);
        GLManager.GL.VertexAttribIPointer(1, 2, GLEnum.UnsignedShort, stride, (void*)10);

        GLManager.GL.EnableVertexAttribArray(2);
        GLManager.GL.VertexAttribPointer(2, 4, GLEnum.UnsignedByte, true, stride, (void*)0);

        GLManager.GL.EnableVertexAttribArray(3);
        GLManager.GL.VertexAttribIPointer(3, 1, GLEnum.UnsignedByte, stride, (void*)14);

        GLManager.GL.EnableVertexAttribArray(4);
        GLManager.GL.VertexAttribIPointer(4, 1, GLEnum.UnsignedByte, stride, (void*)15);

        VertexArray.Unbind();
    }

    public void UploadSection(int sectionIndex, int pass, Span<ChunkVertex> data)
    {
        if (sectionIndex < 0 || sectionIndex >= SectionCount) return;

        // Free previous allocation if it exists
        if (_offsets[pass, sectionIndex] != -1)
        {
            _arenas[pass].Free(_offsets[pass, sectionIndex]);
            _offsets[pass, sectionIndex] = -1;
            _counts[pass, sectionIndex] = 0;
        }

        if (data.Length > 0)
        {
            int offset = _arenas[pass].Allocate(data.Length);
            if (offset != -1)
            {
                _arenas[pass].Upload(offset, data);
                _offsets[pass, sectionIndex] = offset;
                _counts[pass, sectionIndex] = data.Length;
            }
        }
    }

    public void Bind(int pass)
    {
        if (_arenas[pass].Version != _arenaVersions[pass])
        {
            Log.Instance.For<RenderRegion>().LogInformation($"Refreshing VAO for region at {Origin}, pass {pass} (Arena Version: {_arenas[pass].Version})");
            SetupVao(_vaos[pass], _arenas[pass]);
            _arenaVersions[pass] = _arenas[pass].Version;
        }
        _vaos[pass].Bind();
    }

    public static void Unbind()
    {
        VertexArray.Unbind();
    }

    public int GetOffset(int pass, int sectionIndex) => _offsets[pass, sectionIndex];
    public int GetCount(int pass, int sectionIndex) => _counts[pass, sectionIndex];

    public MultiDrawBatch GetBatch(int pass) => _batches[pass];

    public void Dispose()
    {
        if (_disposed) return;

        _arenas[0].Dispose();
        _arenas[1].Dispose();
        _vaos[0].Dispose();
        _vaos[1].Dispose();
        _batches[0].Dispose();
        _batches[1].Dispose();

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
