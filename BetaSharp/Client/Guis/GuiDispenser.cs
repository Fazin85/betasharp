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

    protected override void drawGuiContainerForegroundLayer()
    {
        fontRenderer.drawString("Dispenser", 60, 6, 4210752);
        fontRenderer.drawString("Inventory", 8, ySize - 96 + 2, 4210752);
    }

    protected override void drawGuiContainerBackgroundLayer(float partialTicks)
    {
        int textureId = mc.textureManager.getTextureId("/gui/trap.png");
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        mc.textureManager.bindTexture(textureId);
        int guiLeft = (width - xSize) / 2;
        int guiTop = (height - ySize) / 2;
        drawTexturedModalRect(guiLeft, guiTop, 0, 0, xSize, ySize);
    }
}