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

    // Lighting state
    private bool _lightingEnabled = false;
    private float _light0DirX, _light0DirY, _light0DirZ;
    private float _light0DiffR, _light0DiffG, _light0DiffB;
    private float _light1DirX, _light1DirY, _light1DirZ;
    private float _light1DiffR, _light1DiffG, _light1DiffB;
    private float _ambientR = 0.2f, _ambientG = 0.2f, _ambientB = 0.2f;

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
        _shader.SetEnableLighting(_lightingEnabled);

        if (_lightingEnabled)
        {
            _shader.SetLight0(_light0DirX, _light0DirY, _light0DirZ, _light0DiffR, _light0DiffG, _light0DiffB);
            _shader.SetLight1(_light1DirX, _light1DirY, _light1DirZ, _light1DiffR, _light1DiffG, _light1DiffB);
            _shader.SetAmbientLight(_ambientR, _ambientG, _ambientB);

            // Normal matrix = transpose of inverse of upper-left 3x3 of modelview
            var mv = _modelViewStack.Top;
            if (Matrix4X4.Invert(mv, out var invMv))
            {
                // Transpose the inverse, then extract upper-left 3x3
                var t = Matrix4X4.Transpose(invMv);
                var normalMatrix = new Matrix3X3<float>(
                    t.M11, t.M12, t.M13,
                    t.M21, t.M22, t.M23,
                    t.M31, t.M32, t.M33);
                _shader.SetNormalMatrix(normalMatrix);
            }
            else
            {
                _shader.SetNormalMatrix(Matrix3X3<float>.Identity);
            }
        }
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
        if (_displayLists.IsCompiling) { base.Enable(cap); return; }
        switch (cap)
        {
            case GLEnum.Texture2D: _useTexture = true; break;
            case GLEnum.Lighting: _lightingEnabled = true; return;
            case GLEnum.Light0: return;
            case GLEnum.Light1: return;
            case GLEnum.ColorMaterial: return;
        }
        base.Enable(cap);
    }

    public override void Disable(GLEnum cap)
    {
        if (_displayLists.IsCompiling) { base.Disable(cap); return; }
        switch (cap)
        {
            case GLEnum.Texture2D: _useTexture = false; break;
            case GLEnum.Lighting: _lightingEnabled = false; return;
            case GLEnum.Light0: return;
            case GLEnum.Light1: return;
            case GLEnum.ColorMaterial: return;
        }
        base.Disable(cap);
    }

    public override void Light(GLEnum light, GLEnum pname, float* params_)
    {
        if (pname == GLEnum.Position)
        {
            // w=0 means directional; store normalized direction
            float x = params_[0], y = params_[1], z = params_[2];
            float len = MathF.Sqrt(x * x + y * y + z * z);
            if (len > 0) { x /= len; y /= len; z /= len; }

            if (light == GLEnum.Light0) { _light0DirX = x; _light0DirY = y; _light0DirZ = z; }
            else if (light == GLEnum.Light1) { _light1DirX = x; _light1DirY = y; _light1DirZ = z; }
        }
        else if (pname == GLEnum.Diffuse)
        {
            if (light == GLEnum.Light0) { _light0DiffR = params_[0]; _light0DiffG = params_[1]; _light0DiffB = params_[2]; }
            else if (light == GLEnum.Light1) { _light1DiffR = params_[0]; _light1DiffG = params_[1]; _light1DiffB = params_[2]; }
        }
        // Ambient and Specular are ignored (specular=0 in game, per-light ambient=0)
    }

    public override void LightModel(GLEnum pname, float* params_)
    {
        if (pname == GLEnum.LightModelAmbient)
        {
            _ambientR = params_[0];
            _ambientG = params_[1];
            _ambientB = params_[2];
        }
    }

    public override void ColorMaterial(GLEnum face, GLEnum mode)
    {
        // Always treat vertex color as ambient+diffuse material (the only mode the game uses)
    }

    public override void Normal3(float nx, float ny, float nz)
    {
        // Set the current normal via vertex attrib 3 for immediate mode draws
        SilkGL.VertexAttrib3(3, nx, ny, nz);
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
