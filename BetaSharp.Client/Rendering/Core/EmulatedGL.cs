using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Core;

public unsafe class EmulatedGL : LegacyGL
{
    private readonly MatrixStack _modelViewStack = new();
    private readonly MatrixStack _projectionStack = new();
    private readonly MatrixStack _textureStack = new();

    private GLEnum _currentMatrixMode = GLEnum.Modelview;

    private readonly FixedFunctionShader _shader;
    private bool _useTexture = false;
    private uint _currentProgram = 0;
    private float _alphaThreshold = 0.1f;

    private readonly DisplayListCompiler _displayLists;

    public EmulatedGL(GL gl) : base(gl)
    {
        _shader = new FixedFunctionShader(gl);
        _shader.Use();
        _shader.SetTexture0(0);
        _displayLists = new DisplayListCompiler(gl);
    }

    private MatrixStack ActiveStack => _currentMatrixMode switch
    {
        GLEnum.Modelview => _modelViewStack,
        GLEnum.Projection => _projectionStack,
        GLEnum.Texture => _textureStack,
        _ => _modelViewStack
    };

    private void ActivateShader()
    {
        SilkGL.UseProgram(_shader.Program);
        _shader.SetModelView(_modelViewStack.Top);
        _shader.SetProjection(_projectionStack.Top);
        _shader.SetTextureMatrix(_textureStack.Top);
        _shader.SetUseTexture(_useTexture);
        _shader.SetAlphaThreshold(_alphaThreshold);
    }

    public override void AlphaFunc(GLEnum func, float refValue)
    {
        _alphaThreshold = refValue;
    }

    public override uint GenLists(uint range) => _displayLists.GenLists(range);

    public override void NewList(uint list, GLEnum mode)
    {
        if (mode == GLEnum.Compile || mode == GLEnum.CompileAndExecute)
            _displayLists.BeginList(list);
    }

    public override void EndList() => _displayLists.EndList();

    public override void DeleteLists(uint list, uint range) => _displayLists.DeleteLists(list, range);

    public override void CallList(uint list)
    {
        if (_displayLists.IsCompiling) return;

        _displayLists.Execute(list,
            onDraw: chunk =>
            {
                ActivateShader();
                SilkGL.BindVertexArray(chunk.Vao);
                SilkGL.DrawArrays(chunk.DrawMode, 0, (uint)chunk.VertexCount);
                SilkGL.BindVertexArray(0);
            },
            onTranslate: t => ActiveStack.Translate(t.X, t.Y, t.Z),
            onColor: c => SilkGL.VertexAttrib4(1, c.R, c.G, c.B, c.A));
    }

    public override void CallLists(uint n, GLEnum type, void* lists)
    {
        if (_displayLists.IsCompiling) return;

        if (type == GLEnum.UnsignedInt)
        {
            uint* ids = (uint*)lists;
            for (int i = 0; i < (int)n; i++)
                CallList(ids[i]);
        }
    }

    // ---- BufferData interception ----

    public override void BufferData(GLEnum target, nuint size, void* data, GLEnum usage)
    {
        if (_displayLists.IsCompiling && target == GLEnum.ArrayBuffer && data != null)
            _displayLists.CaptureVertexData((byte*)data, (int)size);

        // Always upload to GPU (keeps Tessellator VBO state valid)
        SilkGL.BufferData(target, size, data, usage);
    }

    // ---- Matrix operations ----

    public override void MatrixMode(GLEnum mode)
    {
        _currentMatrixMode = mode;
        if (!_displayLists.IsCompiling) base.MatrixMode(mode);
    }

    public override void LoadIdentity()
    {
        if (_displayLists.IsCompiling) return;
        ActiveStack.LoadIdentity();
    }

    public override void PushMatrix()
    {
        if (_displayLists.IsCompiling) return;
        ActiveStack.Push();
        base.PushMatrix();
    }

    public override void PopMatrix()
    {
        if (_displayLists.IsCompiling) return;
        ActiveStack.Pop();
        base.PopMatrix();
    }

    public override void Translate(float x, float y, float z)
    {
        if (_displayLists.IsCompiling) { _displayLists.RecordTranslate(x, y, z); return; }
        ActiveStack.Translate(x, y, z);
    }

    public override void Translate(double x, double y, double z)
    {
        if (_displayLists.IsCompiling) { _displayLists.RecordTranslate((float)x, (float)y, (float)z); return; }
        ActiveStack.Translate((float)x, (float)y, (float)z);
    }

    public override void Rotate(float angle, float x, float y, float z)
    {
        if (_displayLists.IsCompiling) return;
        ActiveStack.Rotate(angle, x, y, z);
    }

    public override void Scale(float x, float y, float z)
    {
        if (_displayLists.IsCompiling) return;
        ActiveStack.Scale(x, y, z);
    }

    public override void Scale(double x, double y, double z)
    {
        if (_displayLists.IsCompiling) return;
        ActiveStack.Scale((float)x, (float)y, (float)z);
    }

    public override void Ortho(double left, double right, double bottom, double top, double zNear, double zFar)
    {
        if (_displayLists.IsCompiling) return;
        ActiveStack.Ortho(left, right, bottom, top, zNear, zFar);
    }

    public override void Frustum(double left, double right, double bottom, double top, double zNear, double zFar)
    {
        if (_displayLists.IsCompiling) return;
        ActiveStack.Frustum(left, right, bottom, top, zNear, zFar);
    }

    public override void Color3(float red, float green, float blue)
    {
        if (_displayLists.IsCompiling) { _displayLists.RecordColor(red, green, blue, 1.0f); return; }
        SilkGL.VertexAttrib4(1, red, green, blue, 1.0f);
        base.Color3(red, green, blue);
    }

    public override void Color3(byte red, byte green, byte blue)
    {
        float r = red / 255.0f, g = green / 255.0f, b = blue / 255.0f;
        if (_displayLists.IsCompiling) { _displayLists.RecordColor(r, g, b, 1.0f); return; }
        SilkGL.VertexAttrib4(1, r, g, b, 1.0f);
        base.Color3(red, green, blue);
    }

    public override void Color4(float red, float green, float blue, float alpha)
    {
        if (_displayLists.IsCompiling) { _displayLists.RecordColor(red, green, blue, alpha); return; }
        SilkGL.VertexAttrib4(1, red, green, blue, alpha);
        base.Color4(red, green, blue, alpha);
    }

    public override void VertexPointer(int size, GLEnum type, uint stride, void* pointer)
    {
        if (_displayLists.IsCompiling) { _displayLists.SetStride(stride); return; }
        SilkGL.VertexAttribPointer(0, size, type, false, stride, pointer);
    }

    public override void ColorPointer(int size, ColorPointerType type, uint stride, void* pointer)
    {
        if (_displayLists.IsCompiling) return;
        SilkGL.VertexAttribPointer(1, size, (GLEnum)type, type == ColorPointerType.UnsignedByte, stride, pointer);
    }

    public override void TexCoordPointer(int size, GLEnum type, uint stride, void* pointer)
    {
        if (_displayLists.IsCompiling) return;
        SilkGL.VertexAttribPointer(2, size, type, false, stride, pointer);
    }

    public override void NormalPointer(NormalPointerType type, uint stride, void* pointer)
    {
        if (_displayLists.IsCompiling) return;
        SilkGL.VertexAttribPointer(3, 3, (GLEnum)type, true, stride, pointer);
    }

    public override void EnableClientState(GLEnum array)
    {
        if (_displayLists.IsCompiling) { _displayLists.EnableAttribute(array); return; }
        switch (array)
        {
            case GLEnum.VertexArray: SilkGL.EnableVertexAttribArray(0); break;
            case GLEnum.ColorArray: SilkGL.EnableVertexAttribArray(1); break;
            case GLEnum.TextureCoordArray: SilkGL.EnableVertexAttribArray(2); break;
            case GLEnum.NormalArray: SilkGL.EnableVertexAttribArray(3); break;
            default: base.EnableClientState(array); break;
        }
    }

    public override void DisableClientState(GLEnum array)
    {
        if (_displayLists.IsCompiling) return;
        switch (array)
        {
            case GLEnum.VertexArray: SilkGL.DisableVertexAttribArray(0); break;
            case GLEnum.ColorArray: SilkGL.DisableVertexAttribArray(1); break;
            case GLEnum.TextureCoordArray: SilkGL.DisableVertexAttribArray(2); break;
            case GLEnum.NormalArray: SilkGL.DisableVertexAttribArray(3); break;
            default: base.DisableClientState(array); break;
        }
    }

    public override void Enable(GLEnum cap)
    {
        if (cap == GLEnum.Texture2D && !_displayLists.IsCompiling)
            _useTexture = true;
        base.Enable(cap);
    }

    public override void Disable(GLEnum cap)
    {
        if (cap == GLEnum.Texture2D && !_displayLists.IsCompiling)
            _useTexture = false;
        base.Disable(cap);
    }

    public override void DrawArrays(GLEnum mode, int first, uint count)
    {
        if (_displayLists.IsCompiling)
        {
            _displayLists.RecordDraw(mode, (int)count);
            return;
        }

        if (_currentProgram == 0 || _currentProgram == _shader.Program)
            ActivateShader();

        SilkGL.DrawArrays(mode, first, count);
    }

    public override void UseProgram(uint program)
    {
        if (_displayLists.IsCompiling) return;
        _currentProgram = program;
        base.UseProgram(program);
    }

    public override void GetFloat(GLEnum pname, float* data)
    {
        if (pname == GLEnum.ModelviewMatrix)
        {
            Matrix4X4<float> m = _modelViewStack.Top;
            System.Buffer.MemoryCopy(&m, data, 64, 64);
        }
        else if (pname == GLEnum.ProjectionMatrix)
        {
            Matrix4X4<float> m = _projectionStack.Top;
            System.Buffer.MemoryCopy(&m, data, 64, 64);
        }
        else
        {
            base.GetFloat(pname, data);
        }
    }

    public override void GetFloat(GLEnum pname, Span<float> data)
    {
        if (pname == GLEnum.ModelviewMatrix)
        {
            Matrix4X4<float> m = _modelViewStack.Top;
            fixed (float* dst = data)
                System.Buffer.MemoryCopy(&m, dst, 64, 64);
        }
        else if (pname == GLEnum.ProjectionMatrix)
        {
            Matrix4X4<float> m = _projectionStack.Top;
            fixed (float* dst = data)
                System.Buffer.MemoryCopy(&m, dst, 64, 64);
        }
        else
        {
            base.GetFloat(pname, data);
        }
    }
}
