using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Resource.Pack;
using java.util;

namespace BetaSharp.Client.Guis;

public class GuiTexturePackSlot : GuiSlot
{
    public readonly GuiTexturePacks _parentTexturePackGui;


    public GuiTexturePackSlot(GuiTexturePacks parent) 
        : base(parent.mc, parent.Width, parent.Height, 32, parent.Height - 55 + 4, 36)
    {
        _parentTexturePackGui = parent;
    }

    public override int GetSize()
{
        return _parentTexturePackGui.mc.texturePackList.availableTexturePacks().size();
    }
    protected override void ElementClicked(int index, bool doubleClick)
    {
        var packs = _parentTexturePackGui.mc.texturePackList.availableTexturePacks();
        var selectedPack = (TexturePack)packs.get(index);
        
        _parentTexturePackGui.mc.texturePackList.setTexturePack(selectedPack);
        _parentTexturePackGui.mc.textureManager.reload();
    }

    protected override bool isSelected(int index)
    {
        var packs = _parentTexturePackGui.mc.texturePackList.availableTexturePacks();
        return _parentTexturePackGui.mc.texturePackList.selectedTexturePack == packs.get(index);
    }

    protected override int getContentHeight()
    {
        return GetSize() * 36;
    }

    protected override void drawBackground()
    {
        _parentTexturePackGui.DrawDefaultBackground();
    }

    protected override void drawSlot(int index, int x, int y, int slotHeight, Tessellator tess)
    {
        var pack = (TexturePack)_parentTexturePackGui.mc.texturePackList.availableTexturePacks().get(index);
        pack.bindThumbnailTexture(_parentTexturePackGui.mc);

        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);

        tess.startDrawingQuads();
        tess.setColorOpaque_I(0x00FFFFFF);
        tess.addVertexWithUV(x, y + slotHeight, 0.0D, 0.0D, 1.0D);
        tess.addVertexWithUV(x + 32, y + slotHeight, 0.0D, 1.0D, 1.0D);
        tess.addVertexWithUV(x + 32, y, 0.0D, 1.0D, 0.0D);
        tess.addVertexWithUV(x, y, 0.0D, 0.0D, 0.0D);
        tess.draw();

        _parentTexturePackGui.DrawString(_parentTexturePackGui.FontRenderer, pack.texturePackFileName, x + 32 + 2, y + 1, 0x00FFFFFF);
        _parentTexturePackGui.DrawString(_parentTexturePackGui.FontRenderer, pack.firstDescriptionLine, x + 32 + 2, y + 12, 8421504);
        _parentTexturePackGui.DrawString(_parentTexturePackGui.FontRenderer, pack.secondDescriptionLine, x + 32 + 2, y + 12 + 10, 8421504);
    }
}