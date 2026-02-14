using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Resource.Pack;
using java.util;

namespace BetaSharp.Client.Guis;

public class GuiTexturePackSlot : GuiSlot
{
    public readonly GuiTexturePacks parentTexturePackGui;


    public GuiTexturePackSlot(GuiTexturePacks var1) : base(GuiTexturePacks.func_22124_a(var1), var1.width, var1.height, 32, var1.height - 55 + 4, 36)
    {
        parentTexturePackGui = var1;
    }

    public override int getSize()
    {
        List var1 = GuiTexturePacks.func_22126_b(parentTexturePackGui).texturePackList.availableTexturePacks();
        return var1.size();
    }

    protected override void elementClicked(int var1, bool var2)
    {
        List var3 = GuiTexturePacks.func_22119_c(parentTexturePackGui).texturePackList.availableTexturePacks();
        GuiTexturePacks.func_22122_d(parentTexturePackGui).texturePackList.setTexturePack((TexturePack)var3.get(var1));
        GuiTexturePacks.func_22117_e(parentTexturePackGui).textureManager.reload();
    }

    protected override bool isSelected(int var1)
    {
        List var2 = GuiTexturePacks.func_22118_f(parentTexturePackGui).texturePackList.availableTexturePacks();
        return GuiTexturePacks.func_22116_g(parentTexturePackGui).texturePackList.selectedTexturePack == var2.get(var1);
    }

    protected override int getContentHeight()
    {
        return getSize() * 36;
    }

    protected override void drawBackground()
    {
        parentTexturePackGui.drawDefaultBackground();
    }

    protected override void drawSlot(int var1, int var2, int var3, int var4, Tessellator var5)
    {
        TexturePack var6 = (TexturePack)GuiTexturePacks.func_22121_h(parentTexturePackGui).texturePackList.availableTexturePacks().get(var1);
        var6.bindThumbnailTexture(GuiTexturePacks.func_22123_i(parentTexturePackGui));
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        var5.startDrawingQuads();
        var5.setColorOpaque_I(0x00FFFFFF);
        var5.addVertexWithUV(var2, var3 + var4, 0.0D, 0.0D, 1.0D);
        var5.addVertexWithUV(var2 + 32, var3 + var4, 0.0D, 1.0D, 1.0D);
        var5.addVertexWithUV(var2 + 32, var3, 0.0D, 1.0D, 0.0D);
        var5.addVertexWithUV(var2, var3, 0.0D, 0.0D, 0.0D);
        var5.draw();
        parentTexturePackGui.drawString(GuiTexturePacks.func_22127_j(parentTexturePackGui), var6.texturePackFileName, var2 + 32 + 2, var3 + 1, 0x00FFFFFF);
        parentTexturePackGui.drawString(GuiTexturePacks.func_22120_k(parentTexturePackGui), var6.firstDescriptionLine, var2 + 32 + 2, var3 + 12, 8421504);
        parentTexturePackGui.drawString(GuiTexturePacks.func_22125_l(parentTexturePackGui), var6.secondDescriptionLine, var2 + 32 + 2, var3 + 12 + 10, 8421504);
    }
}