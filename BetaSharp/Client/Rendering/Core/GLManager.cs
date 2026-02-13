using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Core;

public class GLManager
{
    public static GL GL { get => gl; }

    private static GL gl;

    public static void Init(GL gl)
    {
        GLManager.gl = gl;
    }
}