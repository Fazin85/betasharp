using System.Runtime.InteropServices;
using BetaSharp.Util;
using BetaSharp.Util.Maths;
using Silk.NET.Maths;
using Silk.NET.OpenGL.Legacy;
using BetaSharp.Client.Rendering.Core;
using Microsoft.Extensions.Logging;
using System.Collections;
using VertexArray = BetaSharp.Client.Rendering.Core.VertexArray;

namespace BetaSharp.Client.Rendering.Chunks;

public class RenderRegion : IDisposable
{
    private readonly BitArray[] _visibilityMasks = new BitArray[2]; // [pass][sectionIndex]
    private readonly long[] _sectionVersions = new long[SectionCount];
    public const int Width = 8;
    public const int Height = 4;
    public const int Depth = 8;
    public const int SectionCount = Width * Height * Depth;

    public Vector3D<int> Origin { get; }
    public Box BoundingBox { get; }
    
    private readonly GlBufferArena[] _arenas = new GlBufferArena[2];
    private readonly VertexArray[] _vaos = new VertexArray[2];
    private readonly MultiDrawBatch[] _batches = new MultiDrawBatch[2];
    private readonly int[] _arenaVersions = new int[2];
    
    private readonly GlBufferArena.Segment[,] _segments = new GlBufferArena.Segment[2, SectionCount];
    private readonly int[,] _counts = new int[2, SectionCount];

    private bool _isDirtySolid = true;
    private bool _isDirtyTranslucent = true;

    private bool _disposed;

    public RenderRegion(Vector3D<int> origin)
    {
        Origin = origin;
        BoundingBox = new Box(origin.X, origin.Y, origin.Z, origin.X + Width * 16, origin.Y + Height * 16, origin.Z + Depth * 16);

        _arenas[0] = new GlBufferArena(524288, 16);
        _arenas[1] = new GlBufferArena(131072, 16);

        _vaos[0] = new VertexArray();
        SetupVao(_vaos[0], _arenas[0]);

        _vaos[1] = new VertexArray();
        SetupVao(_vaos[1], _arenas[1]);

        _batches[0] = new MultiDrawBatch(SectionCount);
        _batches[1] = new MultiDrawBatch(SectionCount);

        _visibilityMasks[0] = new BitArray(SectionCount);
        _visibilityMasks[1] = new BitArray(SectionCount);
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

    public void UploadSection(int sectionIndex, int pass, Span<ChunkVertex> data, long version)
    {
        if (sectionIndex < 0 || sectionIndex >= SectionCount) return;

        if (_segments[pass, sectionIndex] != null)
        {
            _arenas[pass].Free(_segments[pass, sectionIndex]);
            _segments[pass, sectionIndex] = null;
            _counts[pass, sectionIndex] = 0;
        }

        if (data.Length > 0)
        {
            GlBufferArena.Segment segment = _arenas[pass].Allocate(data.Length);
            if (segment != null)
            {
                _arenas[pass].Upload(segment, data);
                _segments[pass, sectionIndex] = segment;
                _counts[pass, sectionIndex] = data.Length;
            }
        }

        _sectionVersions[sectionIndex] = version;
        if (pass == 0) _isDirtySolid = true;
        else _isDirtyTranslucent = true;
    }

    public bool UpdateBatch(int pass, List<SubChunkRenderer> visibleSections, bool forceRebuild = false)
    {
        bool changed = forceRebuild;
        if (!changed)
        {
            if (pass == 0 && _isDirtySolid) changed = true;
            if (pass == 1 && _isDirtyTranslucent) changed = true;

            BitArray currentMask = new BitArray(SectionCount);
            foreach (SubChunkRenderer renderer in visibleSections)
            {
                int idx = GetSectionIndex(renderer.Position);
                currentMask.Set(idx, true);
            }

            if (!changed)
            {
                for (int i = 0; i < SectionCount; i++)
                {
                    if (currentMask.Get(i) != _visibilityMasks[pass].Get(i))
                    {
                        changed = true;
                        break;
                    }
                }
            }

            _visibilityMasks[pass] = currentMask;
        }

        if (changed)
        {
            MultiDrawBatch batch = _batches[pass];
            batch.Clear();
            foreach (SubChunkRenderer renderer in visibleSections)
            {
                int idx = GetSectionIndex(renderer.Position);
                GlBufferArena.Segment segment = _segments[pass, idx];
                int count = _counts[pass, idx];
                if (segment != null && count > 0)
                {
                    batch.Add(segment.Offset, (uint)count);
                }
            }
            
            if (pass == 0) _isDirtySolid = false;
            else _isDirtyTranslucent = false;
        }

        return changed;
    }


    private int GetSectionIndex(Vector3D<int> pos)
    {
        int sx = (pos.X / 16) % Width;
        int sy = (pos.Y / 16) % Height;
        int sz = (pos.Z / 16) % Depth;
        
        if (sx < 0) sx += Width;
        if (sy < 0) sy += Height;
        if (sz < 0) sz += Depth;
        
        return sx + sy * Width + sz * (Width * Height);
    }

    public void Bind(int pass)
    {
        if (_arenas[pass].Version != _arenaVersions[pass])
        {
            Log.Instance.For<RenderRegion>().LogInformation($"Refreshing VAO and Batch for region at {Origin}, pass {pass} (Arena Version: {_arenas[pass].Version})");
            SetupVao(_vaos[pass], _arenas[pass]);
            _batches[pass].Clear();
            _arenaVersions[pass] = _arenas[pass].Version;
            
            if (pass == 0) _isDirtySolid = true;
            else _isDirtyTranslucent = true;
        }
        _vaos[pass].Bind();
    }

    public static void Unbind()
    {
        VertexArray.Unbind();
    }

    public int GetOffset(int pass, int sectionIndex) => _segments[pass, sectionIndex]?.Offset ?? -1;
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
