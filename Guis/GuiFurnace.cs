using betareborn.Containers;
using betareborn.Blocks.BlockEntities;
using betareborn.Inventorys;

namespace betareborn.Guis
{
    public class GuiFurnace : GuiContainer
    {

        private BlockEntityFurnace furnaceInventory;

        public GuiFurnace(InventoryPlayer playerInventory, BlockEntityFurnace furnace) : base(new ContainerFurnace(playerInventory, furnace))
        {
            furnaceInventory = furnace;
        }

        protected override void drawGuiContainerForegroundLayer()
        {
            fontRenderer.drawString("Furnace", 60, 6, 4210752);
            fontRenderer.drawString("Inventory", 8, ySize - 96 + 2, 4210752);
        }

        protected override void drawGuiContainerBackgroundLayer(float var1)
        {
            int var2 = mc.renderEngine.getTexture("/gui/furnace.png");
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            mc.renderEngine.bindTexture(var2);
            int var3 = (width - xSize) / 2;
            int var4 = (height - ySize) / 2;
            drawTexturedModalRect(var3, var4, 0, 0, xSize, ySize);
            int var5;
            if (furnaceInventory.isBurning())
            {
                var5 = furnaceInventory.getFuelTimeDelta(12);
                drawTexturedModalRect(var3 + 56, var4 + 36 + 12 - var5, 176, 12 - var5, 14, var5 + 2);
            }

            var5 = furnaceInventory.getCookTimeDelta(24);
            drawTexturedModalRect(var3 + 79, var4 + 34, 176, 14, var5 + 1, 16);
        }
    }

}