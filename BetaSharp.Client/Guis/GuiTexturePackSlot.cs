using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Guis;
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
        return _parentTexturePackGui.mc.texturePackList.AvailableTexturePacks.Count;
    }
    protected override void ElementClicked(int index, bool doubleClick)
    {
        var packs = _parentTexturePackGui.mc.texturePackList.AvailableTexturePacks;
        var selectedPack = packs[index];

        _parentTexturePackGui.mc.texturePackList.setTexturePack(selectedPack);
        _parentTexturePackGui.mc.textureManager.Reload();
    }

    protected override bool IsSelected(int index)
    {
        var packs = _parentTexturePackGui.mc.texturePackList.AvailableTexturePacks;
        return _parentTexturePackGui.mc.texturePackList.SelectedTexturePack == packs[index];
    }

    protected override int GetContentHeight()
    {
        return GetSize() * 36;
    }

    protected override void DrawBackground()
    {
        _parentTexturePackGui.DrawDefaultBackground();
    }

    protected override void DrawSlot(int index, int x, int y, int slotHeight, GuiBatch batch)
    {
        var pack = _parentTexturePackGui.mc.texturePackList.AvailableTexturePacks[index];
        var tex = pack.BindThumbnailTexture(_parentTexturePackGui.mc);

        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);

        batch.DrawTexturedQuad(x, y, 32, slotHeight, 0f, 1f, 1f, 0f, Color.White, 0f, (uint)(tex?.Id ?? 0));

        Gui.DrawString(_parentTexturePackGui.FontRenderer, pack.TexturePackFileName, x + 32 + 2, y + 1, Color.White);
        Gui.DrawString(_parentTexturePackGui.FontRenderer, pack.FirstDescriptionLine, x + 32 + 2, y + 12,  Color.Gray80);
        Gui.DrawString(_parentTexturePackGui.FontRenderer, pack.SecondDescriptionLine, x + 32 + 2, y + 12 + 10, Color.Gray80);
    }
}
