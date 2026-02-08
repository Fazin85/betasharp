using betareborn.Containers;
using betareborn.Blocks.BlockEntities;
using betareborn.Inventorys;

namespace betareborn.Guis
{
    public class GuiDispenser : GuiContainer
    {

        public GuiDispenser(InventoryPlayer var1, BlockEntityDispenser var2) : base(new ContainerDispenser(var1, var2))
        {
        }

        protected override void drawGuiContainerForegroundLayer()
        {
            fontRenderer.drawString("Dispenser", 60, 6, 4210752);
            fontRenderer.drawString("Inventory", 8, ySize - 96 + 2, 4210752);
        }

        protected override void drawGuiContainerBackgroundLayer(float var1)
        {
            int var2 = mc.renderEngine.getTexture("/gui/trap.png");
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            mc.renderEngine.bindTexture(var2);
            int var3 = (width - xSize) / 2;
            int var4 = (height - ySize) / 2;
            drawTexturedModalRect(var3, var4, 0, 0, xSize, ySize);
        }
    }

}