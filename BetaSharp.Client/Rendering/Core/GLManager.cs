using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Core;

public class GLManager
{
    public static IGL GL { get; private set; }

    public static void Init(GL silkGl)
    {
        GL = new EmulatedGL(silkGl);
    }
}
