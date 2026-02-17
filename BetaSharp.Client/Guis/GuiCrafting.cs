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

    public override void OnGuiClosed()
    {
        base.OnGuiClosed();
        InventorySlots.onClosed(mc.player);
    }

    protected override void DrawGuiContainerForegroundLayer()
    {
        FontRenderer.drawString("Crafting", 28, 6, 0x404040);
        FontRenderer.drawString("Inventory", 8, _ySize - 96 + 2, 0x404040);
    }

    protected override void DrawGuiContainerBackgroundLayer(float partialTicks)
    {
        int textureId = mc.textureManager.getTextureId("/gui/crafting.png");
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        mc.textureManager.bindTexture(textureId);
        int guiLeft = (Width - _xSize) / 2;
        int guiTop = (Height - _ySize) / 2;
        DrawTexturedModalRect(guiLeft, guiTop, 0, 0, _xSize, _ySize);
    }
}