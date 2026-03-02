using System.Runtime.InteropServices;
using Silk.NET.OpenGL.Legacy;
using VkBuffer = Vuldrid.DeviceBuffer;

namespace BetaSharp.Client.Rendering.Core.OpenGL;

/// <summary>
/// Vertex format used by display lists and the Tessellator.
/// Matches the vertex layout expected by the fixed-function pipeline shader.
/// Layout: Position (12 bytes) + TexCoord (8 bytes) + Color (4 bytes) + Normal (4 bytes) = 28 bytes
/// Padded to 32 bytes to match the pipeline vertex layout.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EmulatedVertex
{
    public float PosX, PosY, PosZ;     // 12 bytes, offset 0
    public float TexU, TexV;           // 8 bytes, offset 12
    public byte ColorR, ColorG, ColorB, ColorA; // 4 bytes, offset 20
    public sbyte NormalX, NormalY, NormalZ, NormalW; // 4 bytes, offset 24
    public uint _padding;              // 4 bytes, offset 28 (pad to 32)
}

/// <summary>
/// Recorded display list command types.
/// </summary>
public enum DLCommandType : byte
{
    Draw,
    Translate,
    Color,
    BindTexture,
    EnableTexture,
    DisableTexture,
    CallList,
}

/// <summary>
/// A single recorded display list command.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct DLCommand
{
    [FieldOffset(0)] public DLCommandType Type;

    // Draw command data
    [FieldOffset(4)] public GLEnum DrawMode;
    [FieldOffset(8)] public int DrawOffset;   // byte offset into vertex data
    [FieldOffset(12)] public int DrawCount;   // vertex count

    // Translate data
    [FieldOffset(4)] public float TransX;
    [FieldOffset(8)] public float TransY;
    [FieldOffset(12)] public float TransZ;

    // Color data
    [FieldOffset(4)] public float ColorR;
    [FieldOffset(8)] public float ColorG;
    [FieldOffset(12)] public float ColorB;
    [FieldOffset(16)] public float ColorA;

    // Texture data
    [FieldOffset(4)] public uint TextureId;

    // CallList data
    [FieldOffset(4)] public uint ListId;
}

/// <summary>
/// Compiles and executes display lists using Vuldrid buffers.
/// </summary>
public class DisplayListCompiler
{
    private readonly EmulatedGL _gl;

    // Display list storage
    private readonly Dictionary<uint, CompiledDisplayList> _lists = [];
    private uint _nextListId = 1;

    // Recording state
    private uint _recordingListId;
    private List<DLCommand>? _recordingCommands;
    private List<EmulatedVertex>? _recordingVertices;

    public bool IsRecording => _recordingCommands != null;

    public DisplayListCompiler(EmulatedGL gl)
    {
        _gl = gl;
    }

    public uint AllocateLists(uint range)
    {
        uint first = _nextListId;
        _nextListId += range;
        return first;
    }

    public void BeginList(uint listId)
    {
        _recordingListId = listId;
        _recordingCommands = [];
        _recordingVertices = [];
    }

    public void EndList()
    {
        if (_recordingCommands == null || _recordingVertices == null)
            return;

        // Free old list if exists
        if (_lists.TryGetValue(_recordingListId, out CompiledDisplayList? old))
        {
            old.Dispose();
        }

        var compiled = new CompiledDisplayList(
            _recordingCommands.ToArray(),
            _recordingVertices.ToArray(),
            GLManager.Device);

        _lists[_recordingListId] = compiled;

        _recordingCommands = null;
        _recordingVertices = null;
    }

    /// <summary>
    /// Record a draw command into the current display list.
    /// </summary>
    public void RecordDraw(GLEnum mode, ReadOnlySpan<EmulatedVertex> vertices)
    {
        if (_recordingCommands == null || _recordingVertices == null) return;

        int offset = _recordingVertices.Count * 32; // 32 bytes per vertex
        _recordingCommands.Add(new DLCommand
        {
            Type = DLCommandType.Draw,
            DrawMode = mode,
            DrawOffset = offset,
            DrawCount = vertices.Length,
        });

        for (int i = 0; i < vertices.Length; i++)
            _recordingVertices.Add(vertices[i]);
    }

    public void RecordTranslate(float x, float y, float z)
    {
        if (_recordingCommands == null) return;
        _recordingCommands.Add(new DLCommand
        {
            Type = DLCommandType.Translate,
            TransX = x,
            TransY = y,
            TransZ = z
        });
    }

    public void RecordColor(float r, float g, float b, float a)
    {
        if (_recordingCommands == null) return;
        _recordingCommands.Add(new DLCommand
        {
            Type = DLCommandType.Color,
            ColorR = r,
            ColorG = g,
            ColorB = b,
            ColorA = a
        });
    }

    public void RecordBindTexture(uint textureId)
    {
        if (_recordingCommands == null) return;
        _recordingCommands.Add(new DLCommand
        {
            Type = DLCommandType.BindTexture,
            TextureId = textureId
        });
    }

    public void RecordCallList(uint listId)
    {
        if (_recordingCommands == null) return;
        _recordingCommands.Add(new DLCommand
        {
            Type = DLCommandType.CallList,
            ListId = listId
        });
    }

    public void Execute(uint listId)
    {
        if (!_lists.TryGetValue(listId, out CompiledDisplayList? list)) return;

        foreach (ref readonly DLCommand cmd in list.Commands.AsSpan())
        {
            switch (cmd.Type)
            {
                case DLCommandType.Draw:
                    if (list.VertexBuffer != null)
                    {
                        _gl.DrawWithBuffer(cmd.DrawMode, list.VertexBuffer, (uint)cmd.DrawCount, (uint)(cmd.DrawOffset / 32));
                    }
                    break;

                case DLCommandType.Translate:
                    _gl.Translate(cmd.TransX, cmd.TransY, cmd.TransZ);
                    break;

                case DLCommandType.Color:
                    _gl.Color4(cmd.ColorR, cmd.ColorG, cmd.ColorB, cmd.ColorA);
                    break;

                case DLCommandType.BindTexture:
                    _gl.BindTexture(GLEnum.Texture2D, cmd.TextureId);
                    break;

                case DLCommandType.CallList:
                    Execute(cmd.ListId);
                    break;
            }
        }
    }

    public void DeleteLists(uint first, uint range)
    {
        for (uint i = first; i < first + range; i++)
        {
            if (_lists.Remove(i, out CompiledDisplayList? list))
            {
                list.Dispose();
            }
        }
    }

    public void Dispose()
    {
        foreach (CompiledDisplayList list in _lists.Values)
            list.Dispose();
        _lists.Clear();
    }
}

/// <summary>
/// A compiled display list with a GPU vertex buffer and command array.
/// </summary>
public class CompiledDisplayList : IDisposable
{
    public DLCommand[] Commands { get; }
    public VkBuffer? VertexBuffer { get; private set; }

    public CompiledDisplayList(DLCommand[] commands, EmulatedVertex[] vertices, global::Vuldrid.GraphicsDevice device)
    {
        Commands = commands;

        if (vertices.Length > 0)
        {
            uint size = (uint)(vertices.Length * 32);
            VertexBuffer = device.ResourceFactory.CreateBuffer(
                new global::Vuldrid.BufferDescription(size, global::Vuldrid.BufferUsage.VertexBuffer));
            device.UpdateBuffer(VertexBuffer, 0, vertices);
        }
    }

    public void Dispose()
    {
        VertexBuffer?.Dispose();
        VertexBuffer = null;
    }
}
