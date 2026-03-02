using System.Runtime.InteropServices;
using BetaSharp.Client.Guis;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Core.OpenGL;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Guis;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
internal struct GuiVertex
{
    public float X, Y, Z;
    public float U, V;
    public byte R, G, B, A;
}

public class GuiBatch
{
    private const int MaxVertices = 65536;
    private const int VertexSize = 24;

    private readonly GL _gl;
    private readonly GuiShader _shader;
    private readonly uint _vao;
    private readonly uint _vbo;

    private readonly GuiVertex[] _vertices = new GuiVertex[MaxVertices];
    private int _vertexCount;
    private bool _useTexture;
    private uint _currentTextureId;
    private bool _begun;
    private float _screenWidth;
    private float _screenHeight;

    public GuiBatch(GL gl)
    {
        _gl = gl;
        _shader = new GuiShader(gl);

        _vao = _gl.GenVertexArray();
        _vbo = _gl.GenBuffer();

        _gl.BindVertexArray(_vao);
        _gl.BindBuffer(GLEnum.ArrayBuffer, _vbo);
        unsafe
        {
            _gl.BufferData(GLEnum.ArrayBuffer, (nuint)(MaxVertices * VertexSize), (void*)0, GLEnum.DynamicDraw);
            _gl.VertexAttribPointer(0, 3, GLEnum.Float, false, VertexSize, (void*)0);
            _gl.VertexAttribPointer(1, 2, GLEnum.Float, false, VertexSize, (void*)12);
            _gl.VertexAttribPointer(2, 4, GLEnum.UnsignedByte, true, VertexSize, (void*)20);
        }
        _gl.EnableVertexAttribArray(0);
        _gl.EnableVertexAttribArray(1);
        _gl.EnableVertexAttribArray(2);

        _gl.BindVertexArray(0);
    }

    public void Begin(ScaledResolution resolution)
    {
        if (_begun) Flush();
        _begun = true;
        _screenWidth = (float)resolution.ScaledWidthDouble;
        _screenHeight = (float)resolution.ScaledHeightDouble;
        _vertexCount = 0;
        _useTexture = false;
        _currentTextureId = 0;
    }

    /// <summary>Call after BindTexture to ensure the batch flushes when the texture changes. Pass the texture handle's Id.</summary>
    public void SetTexture(uint textureId)
    {
        if (_useTexture && textureId != _currentTextureId && _vertexCount > 0)
            Flush();
        _currentTextureId = textureId;
    }

    public void End()
    {
        if (_begun)
        {
            Flush();
            GLManager.GL.RestoreWorldRenderingState();
            _begun = false;
        }
    }

    private void EnsureCapacity(int additionalVertices, bool useTexture, uint textureId = 0)
    {
        if (_useTexture != useTexture && _vertexCount > 0)
            Flush();
        if (useTexture && textureId != 0 && _useTexture && textureId != _currentTextureId && _vertexCount > 0)
            Flush();
        _useTexture = useTexture;
        if (useTexture && textureId != 0)
            _currentTextureId = textureId;

        if (_vertexCount + additionalVertices > MaxVertices)
        {
            Flush();
        }
    }

    private void AddQuad(float x0, float y0, float x1, float y1, float z,
        float u0, float v0, float u1, float v1,
        byte r, byte g, byte b, byte a)
    {
        int i = _vertexCount;
        _vertices[i++] = new GuiVertex { X = x0, Y = y1, Z = z, U = u0, V = v1, R = r, G = g, B = b, A = a };
        _vertices[i++] = new GuiVertex { X = x1, Y = y1, Z = z, U = u1, V = v1, R = r, G = g, B = b, A = a };
        _vertices[i++] = new GuiVertex { X = x1, Y = y0, Z = z, U = u1, V = v0, R = r, G = g, B = b, A = a };
        _vertices[i++] = new GuiVertex { X = x0, Y = y1, Z = z, U = u0, V = v1, R = r, G = g, B = b, A = a };
        _vertices[i++] = new GuiVertex { X = x1, Y = y0, Z = z, U = u1, V = v0, R = r, G = g, B = b, A = a };
        _vertices[i++] = new GuiVertex { X = x0, Y = y0, Z = z, U = u0, V = v0, R = r, G = g, B = b, A = a };
        _vertexCount = i;
    }

    public void DrawRect(int x1, int y1, int x2, int y2, Color color, float z = 0f)
    {
        if (x1 > x2) (x1, x2) = (x2, x1);
        if (y1 > y2) (y1, y2) = (y2, y1);

        EnsureCapacity(6, false, 0);

        byte r = (byte)color.R;
        byte g = (byte)color.G;
        byte b = (byte)color.B;
        byte a = (byte)color.A;

        AddQuad(x1, y1, x2, y2, z, 0, 0, 1, 1, r, g, b, a);
    }

    public void DrawGradientRect(int left, int right, int top, int bottom, Color topColor, Color bottomColor, float z = 0f)
    {
        EnsureCapacity(6, false, 0);

        byte tr = (byte)topColor.R, tg = (byte)topColor.G, tb = (byte)topColor.B, ta = (byte)topColor.A;
        byte br = (byte)bottomColor.R, bg = (byte)bottomColor.G, bb = (byte)bottomColor.B, ba = (byte)bottomColor.A;

        int i = _vertexCount;
        _vertices[i++] = new GuiVertex { X = left, Y = bottom, Z = z, U = 0, V = 0, R = br, G = bg, B = bb, A = ba };
        _vertices[i++] = new GuiVertex { X = right, Y = bottom, Z = z, U = 0, V = 0, R = br, G = bg, B = bb, A = ba };
        _vertices[i++] = new GuiVertex { X = right, Y = top, Z = z, U = 0, V = 0, R = tr, G = tg, B = tb, A = ta };
        _vertices[i++] = new GuiVertex { X = left, Y = bottom, Z = z, U = 0, V = 0, R = br, G = bg, B = bb, A = ba };
        _vertices[i++] = new GuiVertex { X = right, Y = top, Z = z, U = 0, V = 0, R = tr, G = tg, B = tb, A = ta };
        _vertices[i++] = new GuiVertex { X = left, Y = top, Z = z, U = 0, V = 0, R = tr, G = tg, B = tb, A = ta };
        _vertexCount = i;
    }

    public void DrawTexturedQuad(int x, int y, int w, int h, float u0, float v0, float u1, float v1, Color color, float z = 0f, uint textureId = 0)
    {
        EnsureCapacity(6, true, textureId);

        byte r = (byte)color.R, g = (byte)color.G, b = (byte)color.B, a = (byte)color.A;
        AddQuad(x, y, x + w, y + h, z, u0, v0, u1, v1, r, g, b, a);
    }

    public void Flush()
    {
        if (_vertexCount == 0) return;

        var legacyGl = (LegacyGL)GLManager.GL;
        var silkGl = legacyGl.SilkGL;

        silkGl.ActiveTexture(TextureUnit.Texture0);
        if (_useTexture && _currentTextureId != 0)
            silkGl.BindTexture(GLEnum.Texture2D, _currentTextureId);
        silkGl.UseProgram(_shader.Program);
        _shader.SetScreenSize(_screenWidth, _screenHeight);
        _shader.SetUseTexture(_useTexture);
        _shader.SetTexture0(0);
        _shader.SetAlphaThreshold(-1f);

        silkGl.BindVertexArray(_vao);
        silkGl.BindBuffer(GLEnum.ArrayBuffer, _vbo);

        unsafe
        {
            fixed (GuiVertex* ptr = _vertices)
            {
                silkGl.BufferSubData(GLEnum.ArrayBuffer, 0, (nuint)(_vertexCount * VertexSize), ptr);
            }
        }

        silkGl.DrawArrays(GLEnum.Triangles, 0, (uint)_vertexCount);

        silkGl.BindVertexArray(0);

        _vertexCount = 0;
    }
}
