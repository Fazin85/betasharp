using BetaSharp.Client.Rendering.Core;
using BetaSharp.Entities;

namespace BetaSharp.Client.Rendering.Entitys;

public class BoxEntityRenderer : EntityRenderer
{

    public override void render(Entity target, double x, double y, double z, float yaw, float tickDelta)
    {
        GLManager.GL.PushMatrix();
        renderShape(target.boundingBox, x - target.lastTickX, y - target.lastTickY, z - target.lastTickZ);
        GLManager.GL.PopMatrix();
    }
}