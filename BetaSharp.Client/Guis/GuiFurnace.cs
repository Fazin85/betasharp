using BetaSharp.Blocks.Entities;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Inventorys;
using BetaSharp.Screens;

namespace BetaSharp.Client.Guis;

public class GuiFurnace : GuiContainer
{

    private readonly BlockEntityFurnace furnaceInventory;

    public GuiFurnace(InventoryPlayer playerInventory, BlockEntityFurnace furnace) : base(new FurnaceScreenHandler(playerInventory, furnace))
    {
        furnaceInventory = furnace;
    }

    protected override void DrawGuiContainerForegroundLayer()
    {
        fontRenderer.drawString("Furnace", 60, 6, 4210752);
        fontRenderer.drawString("Inventory", 8, _ySize - 96 + 2, 4210752);
    }

    protected override void DrawGuiContainerBackgroundLayer(float partialTicks)
    {
        int textureId = mc.textureManager.getTextureId("/gui/furnace.png");
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        mc.textureManager.bindTexture(textureId);
        int guiLeft = (Width - _xSize) / 2;
        int guiTop = (Height - _ySize) / 2;
        DrawTexturedModalRect(guiLeft, guiTop, 0, 0, _xSize, _ySize);
        int progress;
        if (furnaceInventory.isBurning())
        {
            progress = furnaceInventory.getFuelTimeDelta(12);
            DrawTexturedModalRect(guiLeft + 56, guiTop + 36 + 12 - progress, 176, 12 - progress, 14, progress + 2);
        }

        progress = furnaceInventory.getCookTimeDelta(24);
        DrawTexturedModalRect(guiLeft + 79, guiTop + 34, 176, 14, progress + 1, 16);
    }
}