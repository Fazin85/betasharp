using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Core.OpenGL;
using BetaSharp.Client.Rendering.Core.Textures;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Client.Entities.FX;

public class EntityFootStepFX : EntityFX
{
    private readonly int maxAge;
    private readonly TextureManager textureManager;

    private int localAge;

    public EntityFootStepFX(TextureManager textureManager, IBlockWorldContext world, double x, double y, double z) : base(world, x, y, z, 0.0D, 0.0D, 0.0D)
    {
        this.textureManager = textureManager;
        velocityX = velocityY = velocityZ = 0.0;
        maxAge = 200;
    }

    public override void renderParticle(Tessellator t, float partialTick, float rotX, float rotY, float rotZ, float upX, float upZ)
    {
        float lifeProgress = (localAge + partialTick) / maxAge;
        lifeProgress *= lifeProgress;
        float alpha = 2.0F - lifeProgress * 2.0F;
        if (alpha > 1.0F)
        {
            alpha = 1.0F;
        }

        alpha *= 0.2F;
        GLManager.GL.Disable(GLEnum.Lighting);
        float footprintSize = 2.0F / 16.0F;
        float renderX = (float)(x - interpPosX);
        float renderY = (float)(y - interpPosY);
        float renderZ = (float)(z - interpPosZ);
        float brightness = _level.Lighting.GetLuminance(MathHelper.Floor(x), MathHelper.Floor(y), MathHelper.Floor(z));
        textureManager.BindTexture(textureManager.GetTextureId("/misc/footprint.png"));
        GLManager.GL.Enable(GLEnum.Blend);
        GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        t.startDrawingQuads();
        t.setColorRGBA_F(brightness, brightness, brightness, alpha);
        t.addVertexWithUV(renderX - footprintSize, renderY, renderZ + footprintSize, 0.0, 1.0);
        t.addVertexWithUV(renderX + footprintSize, renderY, renderZ + footprintSize, 1.0, 1.0);
        t.addVertexWithUV(renderX + footprintSize, renderY, renderZ - footprintSize, 1.0, 0.0);
        t.addVertexWithUV(renderX - footprintSize, renderY, renderZ - footprintSize, 0.0, 0.0);
        t.draw();
        GLManager.GL.Disable(GLEnum.Blend);
        GLManager.GL.Enable(GLEnum.Lighting);
    }

    public override void tick()
    {
        ++localAge;
        if (localAge == maxAge)
        {
            markDead();
        }
    }

    public override int getFXLayer() => 3;
}
