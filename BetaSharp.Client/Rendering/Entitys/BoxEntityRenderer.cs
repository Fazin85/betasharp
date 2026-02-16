using BetaSharp.Client.Rendering.Core;
using BetaSharp.Entities;

namespace BetaSharp.Client.Rendering.Entitys;

public class BoxEntityRenderer : EntityRenderer
{

    public override void render(Entity var1, double var2, double var4, double var6, float var8, float var9)
    {
        GLManager.GL.PushMatrix();
        renderShape(var1.boundingBox, var2 - var1.lastTickX, var4 - var1.lastTickY, var6 - var1.lastTickZ);
        GLManager.GL.PopMatrix();
    }
}