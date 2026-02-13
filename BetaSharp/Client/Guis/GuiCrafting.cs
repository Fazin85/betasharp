using BetaSharp.Client.Rendering.Core;
using BetaSharp.Inventorys;
using BetaSharp.Screens;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Guis;

public class GuiCrafting : GuiContainer
{

    public GuiCrafting(InventoryPlayer player, World world, int posX, int posY, int posZ) : base(new CraftingScreenHandler(player, world, posX, posY, posZ))
    {
    }

    public override void onGuiClosed()
    {
        base.onGuiClosed();
        inventorySlots.onClosed(mc.player);
    }

    protected override void drawGuiContainerForegroundLayer()
    {
        fontRenderer.drawString("Crafting", 28, 6, 4210752);
        fontRenderer.drawString("Inventory", 8, ySize - 96 + 2, 4210752);
    }

    protected override void drawGuiContainerBackgroundLayer(float partialTicks)
    {
        int textureId = mc.textureManager.getTextureId("/gui/crafting.png");
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        mc.textureManager.bindTexture(textureId);
        int guiLeft = (width - xSize) / 2;
        int guiTop = (height - ySize) / 2;
        drawTexturedModalRect(guiLeft, guiTop, 0, 0, xSize, ySize);
    }
}