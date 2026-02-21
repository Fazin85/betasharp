using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Core;

/// <summary>
/// Software display list compiler that captures vertex data and GL state commands
/// during NewList/EndList compilation, then replays them on CallList.
/// 
/// Native OpenGL display lists cannot record vertex array commands (glVertexPointer,
/// glDrawArrays, etc.), so this class emulates the feature by intercepting those
/// calls and storing the geometry in persistent VAO/VBO objects.
/// </summary>
public unsafe class DisplayListCompiler
{
    // ---- Recorded command types ----

    public abstract class DLCommand;

    public class DLDrawChunk : DLCommand
    {
        public uint Vao;
        public uint Vbo;
        public int VertexCount;
        public GLEnum DrawMode;
    }

    public class DLTranslate : DLCommand
    {
        public float X, Y, Z;
    }

    public class DLColor : DLCommand
    {
        public float R, G, B, A;
    }

    private class EmulatedDisplayList
    {
        public List<DLCommand> Commands = [];
    }

    // ---- State ----

    private readonly GL _gl;
    private uint _nextListId = 1;
    private readonly Dictionary<uint, EmulatedDisplayList> _emulatedLists = [];

    private bool _isCompiling;
    private uint _compilingListId;

    // Staging data accumulated during compilation
    private List<byte>? _stagingBuffer;
    private GLEnum _lastDrawMode;
    private bool _compiledHasTexture;
    private bool _compiledHasColor;
    private bool _compiledHasNormals;
    private uint _compiledStride = 32; // Tessellator default

    public bool IsCompiling => _isCompiling;

    public DisplayListCompiler(GL gl)
    {
        _gl = gl;
    }

    // ---- ID allocation ----

    public uint GenLists(uint range)
    {
        uint baseId = _nextListId;
        for (uint i = 0; i < range; i++)
        {
            _emulatedLists[_nextListId] = new EmulatedDisplayList();
            _nextListId++;
        }
        return baseId;
    }

    public void DeleteLists(uint list, uint range)
    {
        for (uint i = 0; i < range; i++)
        {
            uint id = list + i;
            if (_emulatedLists.TryGetValue(id, out EmulatedDisplayList? dl))
            {
                FreeGpuResources(dl);
                _emulatedLists.Remove(id);
            }
        }
    }

    // ---- Compilation ----

    public void BeginList(uint list)
    {
        _isCompiling = true;
        _compilingListId = list;
        _stagingBuffer = new List<byte>(4096);
        _compiledHasTexture = false;
        _compiledHasColor = false;
        _compiledHasNormals = false;
        _compiledStride = 32;

        if (_emulatedLists.TryGetValue(list, out EmulatedDisplayList? existing))
        {
            FreeGpuResources(existing);
            existing.Commands.Clear();
        }
        else
        {
            _emulatedLists[list] = new EmulatedDisplayList();
        }
    }

    public void EndList()
    {
        _isCompiling = false;
        _stagingBuffer = null;
    }

    /// <summary>
    /// Capture vertex data being uploaded via BufferData during compilation.
    /// </summary>
    public void CaptureVertexData(byte* data, int byteCount)
    {
        if (_stagingBuffer == null) return;
        var span = new ReadOnlySpan<byte>(data, byteCount);
        _stagingBuffer.AddRange(span.ToArray());
    }

    /// <summary>
    /// Record the current vertex stride (from VertexPointer).
    /// </summary>
    public void SetStride(uint stride) => _compiledStride = stride;

    /// <summary>
    /// Track which vertex attributes are enabled during compilation.
    /// </summary>
    public void EnableAttribute(GLEnum clientState)
    {
        switch (clientState)
        {
            case GLEnum.TextureCoordArray: _compiledHasTexture = true; break;
            case GLEnum.ColorArray: _compiledHasColor = true; break;
            case GLEnum.NormalArray: _compiledHasNormals = true; break;
        }
    }

    /// <summary>
    /// Called when DrawArrays fires during compilation.
    /// Flushes staged vertex data into a persistent VAO/VBO chunk.
    /// </summary>
    public void RecordDraw(GLEnum mode, int vertexCount)
    {
        _lastDrawMode = mode;

        if (_stagingBuffer == null || _stagingBuffer.Count == 0 || vertexCount == 0) return;

        uint vao = _gl.GenVertexArray();
        uint vbo = _gl.GenBuffer();

        _gl.BindVertexArray(vao);
        _gl.BindBuffer(GLEnum.ArrayBuffer, vbo);

        byte[] data = _stagingBuffer.ToArray();
        fixed (byte* ptr = data)
        {
            _gl.BufferData(GLEnum.ArrayBuffer, (nuint)data.Length, ptr, GLEnum.StaticDraw);
        }

        uint stride = _compiledStride;

        // Attrib 0: Position (3 floats at offset 0)
        _gl.EnableVertexAttribArray(0);
        _gl.VertexAttribPointer(0, 3, GLEnum.Float, false, stride, (void*)0);

        // Attrib 1: Color (4 UByte at offset 20)
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

        // Attrib 2: TexCoord (2 floats at offset 12)
        if (_compiledHasTexture)
        {
            _gl.EnableVertexAttribArray(2);
            _gl.VertexAttribPointer(2, 2, GLEnum.Float, false, stride, (void*)12);
        }
        else
        {
            _gl.DisableVertexAttribArray(2);
        }

        // Attrib 3: Normal (3 bytes at offset 24)
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

        _emulatedLists[_compilingListId].Commands.Add(new DLDrawChunk
        {
            Vao = vao,
            Vbo = vbo,
            VertexCount = vertexCount,
            DrawMode = _lastDrawMode,
        });

        _stagingBuffer.Clear();
    }

    /// <summary>
    /// Record a translate command into the current display list.
    /// </summary>
    public void RecordTranslate(float x, float y, float z)
    {
        _emulatedLists[_compilingListId].Commands.Add(new DLTranslate { X = x, Y = y, Z = z });
    }

    /// <summary>
    /// Record a color command into the current display list.
    /// </summary>
    public void RecordColor(float r, float g, float b, float a)
    {
        _emulatedLists[_compilingListId].Commands.Add(new DLColor { R = r, G = g, B = b, A = a });
    }

    // ---- Playback ----

    /// <summary>
    /// Execute a display list, calling the provided callbacks for each command type.
    /// </summary>
    public void Execute(uint list, Action<DLDrawChunk> onDraw, Action<DLTranslate> onTranslate, Action<DLColor> onColor)
    {
        if (!_emulatedLists.TryGetValue(list, out EmulatedDisplayList? dl) || dl.Commands.Count == 0) return;

        foreach (DLCommand cmd in dl.Commands)
        {
            switch (cmd)
            {
                case DLDrawChunk chunk: onDraw(chunk); break;
                case DLTranslate t: onTranslate(t); break;
                case DLColor c: onColor(c); break;
            }
        }
    }

    // ---- Cleanup ----

    private void FreeGpuResources(EmulatedDisplayList dl)
    {
        foreach (DLCommand cmd in dl.Commands)
        {
            if (cmd is DLDrawChunk chunk)
            {
                _gl.DeleteBuffer(chunk.Vbo);
                _gl.DeleteVertexArray(chunk.Vao);
            }
        }
    }
}
