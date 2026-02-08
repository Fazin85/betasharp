using betareborn.Containers;
using betareborn.Inventorys;

namespace betareborn.Guis
{
    public class GuiChest : GuiContainer
    {

        private IInventory upperChestInventory;
        private IInventory lowerChestInventory;
        private int inventoryRows = 0;

        public GuiChest(IInventory var1, IInventory var2) : base(new ContainerChest(var1, var2))
        {
            upperChestInventory = var1;
            lowerChestInventory = var2;
            field_948_f = false;
            short var3 = 222;
            int var4 = var3 - 108;
            inventoryRows = var2.size() / 9;
            ySize = var4 + inventoryRows * 18;
        }

        protected override void drawGuiContainerForegroundLayer()
        {
            fontRenderer.drawString(lowerChestInventory.getName(), 8, 6, 4210752);
            fontRenderer.drawString(upperChestInventory.getName(), 8, ySize - 96 + 2, 4210752);
        }

        protected override void drawGuiContainerBackgroundLayer(float var1)
        {
            int var2 = mc.renderEngine.getTexture("/gui/container.png");
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            mc.renderEngine.bindTexture(var2);
            int var3 = (width - xSize) / 2;
            int var4 = (height - ySize) / 2;
            drawTexturedModalRect(var3, var4, 0, 0, xSize, inventoryRows * 18 + 17);
            drawTexturedModalRect(var3, var4 + inventoryRows * 18 + 17, 0, 126, xSize, 96);
        }
    }

}