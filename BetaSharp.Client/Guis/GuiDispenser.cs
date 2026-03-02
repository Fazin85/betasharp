using BetaSharp.Blocks.Entities;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Inventorys;
using BetaSharp.Screens;

namespace BetaSharp.Client.Guis;

public class GuiDispenser : GuiContainer
{

    public GuiDispenser(InventoryPlayer inventory, BlockEntityDispenser dispenser) : base(new DispenserScreenHandler(inventory, dispenser))
    {
    }

    protected override void DrawGuiContainerForegroundLayer(int guiLeft, int guiTop)
    {
        FontRenderer.DrawString("Dispenser", guiLeft + 60, guiTop + 6, Color.Gray40);
        FontRenderer.DrawString("Inventory", guiLeft + 8, guiTop + _ySize - 96 + 2, Color.Gray40);
    }

    protected override void DrawGuiContainerBackgroundLayer(float partialTicks)
    {
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        var tex = mc.textureManager.GetTextureId("/gui/trap.png");
        mc.textureManager.BindTexture(tex);
        int guiLeft = (Width - _xSize) / 2;
        int guiTop = (Height - _ySize) / 2;
        DrawTexturedModalRect(mc.guiBatch, guiLeft, guiTop, 0, 0, _xSize, _ySize, tex);
    }
}
