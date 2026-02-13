using BetaSharp.Blocks.Entities;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Entities;

public abstract class BlockEntitySpecialRenderer
{
    protected BlockEntityRenderer tileEntityRenderer;

    public abstract void renderTileEntityAt(BlockEntity var1, double var2, double var4, double var6, float var8);

    protected void bindTextureByName(string var1)
    {
        TextureManager var2 = tileEntityRenderer.renderEngine;
        var2.bindTexture(var2.getTextureId(var1));
    }

    public void setTileEntityRenderer(BlockEntityRenderer var1)
    {
        tileEntityRenderer = var1;
    }

    public virtual void func_31069_a(World var1)
    {
    }

    public TextRenderer getFontRenderer()
    {
        return tileEntityRenderer.getFontRenderer();
    }
}