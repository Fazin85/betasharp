using BetaSharp.Client.Rendering.Core;
using BetaSharp.Inventorys;
using BetaSharp.Screens;

namespace BetaSharp.Client.Guis;

public class GuiChest : GuiContainer
{

    private IInventory upperChestInventory;
    private IInventory lowerChestInventory;
    private int inventoryRows = 0;

    public GuiChest(IInventory upper, IInventory lower) : base(new GenericContainerScreenHandler(upper, lower))
    {
        upperChestInventory = upper;
        lowerChestInventory = lower;
        field_948_f = false;
        short baseHeight = 222;
        int guiHeightMinus = baseHeight - 108;
        inventoryRows = lower.size() / 9;
        ySize = guiHeightMinus + inventoryRows * 18;
    }

    protected override void drawGuiContainerForegroundLayer()
    {
        fontRenderer.drawString(lowerChestInventory.getName(), 8, 6, 4210752);
        fontRenderer.drawString(upperChestInventory.getName(), 8, ySize - 96 + 2, 4210752);
    }

    protected override void drawGuiContainerBackgroundLayer(float partialTicks)
    {
        int textureId = mc.textureManager.getTextureId("/gui/container.png");
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        mc.textureManager.bindTexture(textureId);
        int guiLeft = (width - xSize) / 2;
        int guiTop = (height - ySize) / 2;
        drawTexturedModalRect(guiLeft, guiTop, 0, 0, xSize, inventoryRows * 18 + 17);
        drawTexturedModalRect(guiLeft, guiTop + inventoryRows * 18 + 17, 0, 126, xSize, 96);
    }
}