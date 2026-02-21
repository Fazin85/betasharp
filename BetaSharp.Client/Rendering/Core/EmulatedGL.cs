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

    public EmulatedGL(GL gl) : base(gl)
    {
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

    private void SyncNativeMatrix()
    {
        Matrix4X4<float> top = ActiveStack.Top;
        SilkGL.LoadMatrix((float*)&top);
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
        SyncNativeMatrix();
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
        SyncNativeMatrix();
    }

    public override void Translate(double x, double y, double z)
    {
        if (_isCompilingList)
        {
            base.Translate(x, y, z);
            return;
        }
        ActiveStack.Translate((float)x, (float)y, (float)z);
        SyncNativeMatrix();
    }

    public override void Rotate(float angle, float x, float y, float z)
    {
        if (_isCompilingList)
        {
            base.Rotate(angle, x, y, z);
            return;
        }
        ActiveStack.Rotate(angle, x, y, z);
        SyncNativeMatrix();
    }

    public override void Scale(float x, float y, float z)
    {
        if (_isCompilingList)
        {
            base.Scale(x, y, z);
            return;
        }
        ActiveStack.Scale(x, y, z);
        SyncNativeMatrix();
    }

    public override void Scale(double x, double y, double z)
    {
        if (_isCompilingList)
        {
            base.Scale(x, y, z);
            return;
        }
        ActiveStack.Scale((float)x, (float)y, (float)z);
        SyncNativeMatrix();
    }

    public override void Ortho(double left, double right, double bottom, double top, double zNear, double zFar)
    {
        if (_isCompilingList)
        {
            base.Ortho(left, right, bottom, top, zNear, zFar);
            return;
        }
        ActiveStack.Ortho(left, right, bottom, top, zNear, zFar);
        SyncNativeMatrix();
    }

    public override void Frustum(double left, double right, double bottom, double top, double zNear, double zFar)
    {
        if (_isCompilingList)
        {
            base.Frustum(left, right, bottom, top, zNear, zFar);
            return;
        }
        ActiveStack.Frustum(left, right, bottom, top, zNear, zFar);
        SyncNativeMatrix();
    }
}
