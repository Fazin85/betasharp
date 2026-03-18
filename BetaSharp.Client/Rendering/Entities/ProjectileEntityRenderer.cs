using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Textures;
using BetaSharp.Entities;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Entities;

public class ProjectileEntityRenderer : EntityRenderer
{
    private readonly string _itemTextureId;

    public ProjectileEntityRenderer(string textureId)
    {
        _itemTextureId = textureId;
    }

    // Surcharge legacy pour les appelants qui passent encore un int
    public ProjectileEntityRenderer(int legacyIndex)
        : this(TextureIdMap.GetName(legacyIndex)) { }

    public override void render(Entity target, double x, double y, double z, float yaw, float tickDelta)
    {
        GLManager.GL.PushMatrix();
        GLManager.GL.Translate((float)x, (float)y, (float)z);
        GLManager.GL.Enable(GLEnum.RescaleNormal);
        GLManager.GL.Scale(0.5F, 0.5F, 0.5F);

        TextureAtlas atlas = TextureAtlasManager.Instance.Items;
        atlas.Bind();
        UVRegion uv = atlas.GetUV(_itemTextureId);

        float var15 = 1.0F;
        float var16 = 0.5F;
        float var17 = 0.25F;

        Tessellator var10 = Tessellator.instance;
        GLManager.GL.Rotate(180.0F - Dispatcher.playerViewY, 0.0F, 1.0F, 0.0F);
        GLManager.GL.Rotate(-Dispatcher.playerViewX, 1.0F, 0.0F, 0.0F);
        var10.startDrawingQuads();
        var10.setNormal(0.0F, 1.0F, 0.0F);
        var10.addVertexWithUV(0.0F - var16, 0.0F - var17, 0.0D, uv.U0, uv.V1);
        var10.addVertexWithUV(var15 - var16, 0.0F - var17, 0.0D, uv.U1, uv.V1);
        var10.addVertexWithUV(var15 - var16, 1.0F - var17, 0.0D, uv.U1, uv.V0);
        var10.addVertexWithUV(0.0F - var16, 1.0F - var17, 0.0D, uv.U0, uv.V0);
        var10.draw();

        GLManager.GL.Disable(GLEnum.RescaleNormal);
        GLManager.GL.PopMatrix();
    }
}
