using BetaSharp.Client.Rendering.Core;
using BetaSharp.Inventorys;
using BetaSharp.Screens;

namespace BetaSharp.Client.Guis;

public class GuiChest : GuiContainer
{

    private readonly IInventory _upperChestInventory;
    private readonly IInventory _lowerChestInventory;
    private readonly int _inventoryRows = 0;

    public GuiChest(IInventory upper, IInventory lower) : base(new GenericContainerScreenHandler(upper, lower))
    {
        _upperChestInventory = upper;
        _lowerChestInventory = lower;
        AllowUserInput = false;
        short baseHeight = 222;
        int guiHeightMinus = baseHeight - 108;
        _inventoryRows = lower.size() / 9;
        _ySize = guiHeightMinus + _inventoryRows * 18;
    }

    protected override void DrawGuiContainerForegroundLayer(int guiLeft, int guiTop)
    {
        FontRenderer.DrawString(_lowerChestInventory.getName(), guiLeft + 8, guiTop + 6, Color.Gray40);
        FontRenderer.DrawString(_upperChestInventory.getName(), guiLeft + 8, guiTop + _ySize - 96 + 2, Color.Gray40);
    }

    protected override void DrawGuiContainerBackgroundLayer(float partialTicks)
    {
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        var tex = mc.textureManager.GetTextureId("/gui/container.png");
        mc.textureManager.BindTexture(tex);

        int guiLeft = (Width - _xSize) / 2;
        int guiTop = (Height - _ySize) / 2;

        DrawTexturedModalRect(mc.guiBatch, guiLeft, guiTop, 0, 0, _xSize, _inventoryRows * 18 + 17, tex);
        DrawTexturedModalRect(mc.guiBatch, guiLeft, guiTop + _inventoryRows * 18 + 17, 0, 126, _xSize, 96, tex);
    }
}
