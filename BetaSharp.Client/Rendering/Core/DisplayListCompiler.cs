using BetaSharp.Util;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Core;

public unsafe class DisplayListCompiler
{
    public enum DLCommandType : byte
    {
        DrawChunk,
        Translate,
        Color
    }

    public struct DLCommand
    {
        public DLCommandType Type;
        public uint Vao;
        public uint Vbo;
        public int VertexCount;
        public GLEnum DrawMode;
        public float X_R, Y_G, Z_B, W_A;
    }

    private class DisplayList
    {
        public PooledList<DLCommand> Commands = new(16);
    }

    private readonly GL _gl;
    private uint _nextListId = 1;
    private readonly Dictionary<uint, DisplayList> _emulatedLists = [];
    private uint _compilingListId;

    private byte[] _stagingBuffer = new byte[16384];
    private int _stagingBufferCount = 0;

    private GLEnum _lastDrawMode;
    private bool _compiledHasTexture;
    private bool _compiledHasColor;
    private bool _compiledHasNormals;
    private uint _compiledStride = 32;

    public bool IsCompiling { get; private set; }

    public DisplayListCompiler(GL gl)
    {
        _gl = gl;
    }

    public uint GenLists(uint range)
    {
        uint baseId = _nextListId;
        for (uint i = 0; i < range; i++)
        {
            _emulatedLists[_nextListId] = new DisplayList();
            _nextListId++;
        }
        return baseId;
    }

    public void DeleteLists(uint list, uint range)
    {
        for (uint i = 0; i < range; i++)
        {
            uint id = list + i;
            if (_emulatedLists.TryGetValue(id, out DisplayList? dl))
            {
                FreeGpuResources(dl);
                _emulatedLists.Remove(id);
            }
        }
    }

    public void BeginList(uint list)
    {
        IsCompiling = true;
        _compilingListId = list;
        _stagingBufferCount = 0;
        _compiledHasTexture = false;
        _compiledHasColor = false;
        _compiledHasNormals = false;
        _compiledStride = 32;

        if (_emulatedLists.TryGetValue(list, out DisplayList? existing))
        {
            FreeGpuResources(existing);
            existing.Commands.Clear();
        }
        else
        {
            _emulatedLists[list] = new DisplayList();
        }
    }

    public void EndList()
    {
        IsCompiling = false;
        _stagingBufferCount = 0;
    }

    public void CaptureVertexData(byte* data, int byteCount)
    {
        if (!IsCompiling) return;

        EnsureCapacity(_stagingBufferCount + byteCount);
        fixed (byte* dst = &_stagingBuffer[0])
        {
            System.Buffer.MemoryCopy(data, dst + _stagingBufferCount, byteCount, byteCount);
        }
        _stagingBufferCount += byteCount;
    }

    private void EnsureCapacity(int requiredSize)
    {
        if (_stagingBuffer.Length < requiredSize)
        {
            int newSize = Math.Max(_stagingBuffer.Length * 2, requiredSize);
            Array.Resize(ref _stagingBuffer, newSize);
        }
    }

    public void SetStride(uint stride) => _compiledStride = stride;

    public void EnableAttribute(GLEnum clientState)
    {
        switch (clientState)
        {
            case GLEnum.TextureCoordArray: _compiledHasTexture = true; break;
            case GLEnum.ColorArray: _compiledHasColor = true; break;
            case GLEnum.NormalArray: _compiledHasNormals = true; break;
        }
    }

    public void RecordDraw(GLEnum mode, int vertexCount)
    {
        _lastDrawMode = mode;

        if (_stagingBufferCount == 0 || vertexCount == 0) return;

        uint vao = _gl.GenVertexArray();
        uint vbo = _gl.GenBuffer();

        _gl.BindVertexArray(vao);
        _gl.BindBuffer(GLEnum.ArrayBuffer, vbo);

        fixed (byte* ptr = &_stagingBuffer[0])
        {
            _gl.BufferData(GLEnum.ArrayBuffer, (nuint)_stagingBufferCount, ptr, GLEnum.StaticDraw);
        }

        uint stride = _compiledStride;

        _gl.EnableVertexAttribArray(0);
        _gl.VertexAttribPointer(0, 3, GLEnum.Float, false, stride, (void*)0);

        if (_compiledHasColor)
        {
            _gl.EnableVertexAttribArray(1);
            _gl.VertexAttribPointer(1, 4, GLEnum.UnsignedByte, true, stride, (void*)20);
        }
        else
        {
            _gl.DisableVertexAttribArray(1);
            _gl.VertexAttrib4(1, 1.0f, 1.0f, 1.0f, 1.0f);
        }

        if (_compiledHasTexture)
        {
            _gl.EnableVertexAttribArray(2);
            _gl.VertexAttribPointer(2, 2, GLEnum.Float, false, stride, (void*)12);
        }
        else
        {
            _gl.DisableVertexAttribArray(2);
        }

        if (_compiledHasNormals)
        {
            _gl.EnableVertexAttribArray(3);
            _gl.VertexAttribPointer(3, 3, GLEnum.Byte, true, stride, (void*)24);
        }
        else
        {
            _gl.DisableVertexAttribArray(3);
        }

        _gl.BindVertexArray(0);

        _emulatedLists[_compilingListId].Commands.Add(new DLCommand
        {
            Type = DLCommandType.DrawChunk,
            Vao = vao,
            Vbo = vbo,
            VertexCount = vertexCount,
            DrawMode = _lastDrawMode,
        });

        _stagingBufferCount = 0;
    }

    public void RecordTranslate(float x, float y, float z)
    {
        _emulatedLists[_compilingListId].Commands.Add(new DLCommand { Type = DLCommandType.Translate, X_R = x, Y_G = y, Z_B = z });
    }

    public void RecordColor(float r, float g, float b, float a)
    {
        _emulatedLists[_compilingListId].Commands.Add(new DLCommand { Type = DLCommandType.Color, X_R = r, Y_G = g, Z_B = b, W_A = a });
    }

    public delegate void DrawCallback(ref DLCommand cmd);
    public delegate void TranslateCallback(ref DLCommand cmd);
    public delegate void ColorCallback(ref DLCommand cmd);

    public void Execute(uint list, DrawCallback onDraw, TranslateCallback onTranslate, ColorCallback onColor)
    {
        if (!_emulatedLists.TryGetValue(list, out DisplayList? dl) || dl.Commands.Count == 0) return;

        Span<DLCommand> span = dl.Commands.Span;
        for (int i = 0; i < span.Length; i++)
        {
            ref DLCommand cmd = ref span[i];
            switch (cmd.Type)
            {
                case DLCommandType.DrawChunk: onDraw(ref cmd); break;
                case DLCommandType.Translate: onTranslate(ref cmd); break;
                case DLCommandType.Color: onColor(ref cmd); break;
            }
        }
    }

    private void FreeGpuResources(DisplayList dl)
    {
        foreach (DLCommand cmd in dl.Commands.Span)
        {
            if (cmd.Type == DLCommandType.DrawChunk)
            {
                _gl.DeleteBuffer(cmd.Vbo);
                _gl.DeleteVertexArray(cmd.Vao);
            }
        }
    }
}
