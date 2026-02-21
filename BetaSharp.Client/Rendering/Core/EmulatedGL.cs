using Silk.NET.Maths;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Core;

public unsafe class EmulatedGL : LegacyGL
{
    private readonly MatrixStack _modelViewStack = new();
    private readonly MatrixStack _projectionStack = new();
    private readonly MatrixStack _textureStack = new();

    private GLEnum _currentMatrixMode = GLEnum.Modelview;
    private bool _isCompilingList = false;

    // Shader Emulation
    private FixedFunctionShader _shader;
    private bool _useTexture = false;

    public EmulatedGL(GL gl) : base(gl)
    {
        _shader = new FixedFunctionShader(gl);
        _shader.Use();
        _shader.SetTexture0(0);
    }

    private MatrixStack ActiveStack
    {
        get
        {
            return _currentMatrixMode switch
            {
                GLEnum.Modelview => _modelViewStack,
                GLEnum.Projection => _projectionStack,
                GLEnum.Texture => _textureStack,
                _ => _modelViewStack
            };
        }
    }

    public override void MatrixMode(GLEnum mode)
    {
        if (_isCompilingList)
        {
            base.MatrixMode(mode);
            return;
        }
        _currentMatrixMode = mode;
        base.MatrixMode(mode);
    }

    public override void NewList(uint list, GLEnum mode)
    {
        if (mode == GLEnum.Compile || mode == GLEnum.CompileAndExecute)
        {
            _isCompilingList = true;
        }
        base.NewList(list, mode);
    }

    public override void EndList()
    {
        _isCompilingList = false;
        base.EndList();
    }

    public override void LoadIdentity()
    {
        if (_isCompilingList)
        {
            base.LoadIdentity();
            return;
        }
        ActiveStack.LoadIdentity();
    }

    public override void PushMatrix()
    {
        if (_isCompilingList)
        {
            base.PushMatrix();
            return;
        }
        ActiveStack.Push();
        base.PushMatrix();
    }

    public override void PopMatrix()
    {
        if (_isCompilingList)
        {
            base.PopMatrix();
            return;
        }
        ActiveStack.Pop();
        base.PopMatrix();
    }

    public override void Translate(float x, float y, float z)
    {
        if (_isCompilingList)
        {
            base.Translate(x, y, z);
            return;
        }
        ActiveStack.Translate(x, y, z);
    }

    public override void Translate(double x, double y, double z)
    {
        if (_isCompilingList)
        {
            base.Translate(x, y, z);
            return;
        }
        ActiveStack.Translate((float)x, (float)y, (float)z);
    }

    public override void Rotate(float angle, float x, float y, float z)
    {
        if (_isCompilingList)
        {
            base.Rotate(angle, x, y, z);
            return;
        }
        ActiveStack.Rotate(angle, x, y, z);
    }

    public override void Scale(float x, float y, float z)
    {
        if (_isCompilingList)
        {
            base.Scale(x, y, z);
            return;
        }
        ActiveStack.Scale(x, y, z);
    }

    public override void Scale(double x, double y, double z)
    {
        if (_isCompilingList)
        {
            base.Scale(x, y, z);
            return;
        }
        ActiveStack.Scale((float)x, (float)y, (float)z);
    }

    public override void Ortho(double left, double right, double bottom, double top, double zNear, double zFar)
    {
        if (_isCompilingList)
        {
            base.Ortho(left, right, bottom, top, zNear, zFar);
            return;
        }
        ActiveStack.Ortho(left, right, bottom, top, zNear, zFar);
    }

    public override void Frustum(double left, double right, double bottom, double top, double zNear, double zFar)
    {
        if (_isCompilingList)
        {
            base.Frustum(left, right, bottom, top, zNear, zFar);
            return;
        }
        ActiveStack.Frustum(left, right, bottom, top, zNear, zFar);
    }

    // --- Phase 3: Immediate Mode / Array overriding ---

    public override void Color3(float red, float green, float blue)
    {
        if (_isCompilingList)
        {
            base.Color3(red, green, blue);
            return;
        }
        SilkGL.VertexAttrib4(1, red, green, blue, 1.0f);
        base.Color3(red, green, blue);
    }

    public override void Color3(byte red, byte green, byte blue)
    {
        if (_isCompilingList)
        {
            base.Color3(red, green, blue);
            return;
        }
        SilkGL.VertexAttrib4(1, red / 255.0f, green / 255.0f, blue / 255.0f, 1.0f);
        base.Color3(red, green, blue);
    }

    public override void Color4(float red, float green, float blue, float alpha)
    {
        if (_isCompilingList)
        {
            base.Color4(red, green, blue, alpha);
            return;
        }
        SilkGL.VertexAttrib4(1, red, green, blue, alpha);
        base.Color4(red, green, blue, alpha);
    }

    public override void Color4(byte red, byte green, byte blue, byte alpha)
    {
        if (_isCompilingList)
        {
            base.Color4(red, green, blue, alpha);
            return;
        }
        SilkGL.VertexAttrib4(1, red / 255.0f, green / 255.0f, blue / 255.0f, alpha / 255.0f);
        base.Color4(red, green, blue, alpha);
    }

    public override unsafe void VertexPointer(int size, GLEnum type, uint stride, void* pointer)
    {
        if (_isCompilingList)
        {
            base.VertexPointer(size, type, stride, pointer);
            return;
        }
        SilkGL.VertexAttribPointer(0, size, type, false, stride, pointer);
    }

    public override unsafe void ColorPointer(int size, ColorPointerType type, uint stride, void* pointer)
    {
        if (_isCompilingList)
        {
            base.ColorPointer(size, type, stride, pointer);
            return;
        }
        SilkGL.VertexAttribPointer(1, size, (GLEnum)type, type == ColorPointerType.UnsignedByte, stride, pointer);
    }

    public override unsafe void TexCoordPointer(int size, GLEnum type, uint stride, void* pointer)
    {
        if (_isCompilingList)
        {
            base.TexCoordPointer(size, type, stride, pointer);
            return;
        }
        SilkGL.VertexAttribPointer(2, size, type, false, stride, pointer);
    }

    public override unsafe void NormalPointer(NormalPointerType type, uint stride, void* pointer)
    {
        if (_isCompilingList)
        {
            base.NormalPointer(type, stride, pointer);
            return;
        }
        SilkGL.VertexAttribPointer(3, 3, (GLEnum)type, true, stride, pointer);
    }

    public override void EnableClientState(GLEnum array)
    {
        if (_isCompilingList)
        {
            base.EnableClientState(array);
            return;
        }
        switch (array)
        {
            case GLEnum.VertexArray: SilkGL.EnableVertexAttribArray(0); break;
            case GLEnum.ColorArray: SilkGL.EnableVertexAttribArray(1); break;
            case GLEnum.TextureCoordArray: SilkGL.EnableVertexAttribArray(2); break;
            case GLEnum.NormalArray: SilkGL.EnableVertexAttribArray(3); break;
            default: base.EnableClientState(array); break; // fallback just in case
        }
    }

    public override void DisableClientState(GLEnum array)
    {
        if (_isCompilingList)
        {
            base.DisableClientState(array);
            return;
        }
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
        if (_isCompilingList)
        {
            base.Enable(cap);
            return;
        }
        if (cap == GLEnum.Texture2D)
            _useTexture = true;

        base.Enable(cap);
    }

    public override void Disable(GLEnum cap)
    {
        if (_isCompilingList)
        {
            base.Disable(cap);
            return;
        }
        if (cap == GLEnum.Texture2D)
            _useTexture = false;

        base.Disable(cap);
    }

    public override void DrawArrays(GLEnum mode, int first, uint count)
    {
        if (_isCompilingList)
        {
            base.DrawArrays(mode, first, count);
            return;
        }

        // Apply shader state right before drawing
        _shader.Use();
        _shader.SetModelView(_modelViewStack.Top);
        _shader.SetProjection(_projectionStack.Top);
        _shader.SetTextureMatrix(_textureStack.Top);
        _shader.SetUseTexture(_useTexture);

        SilkGL.DrawArrays(mode, first, count);
    }
}
