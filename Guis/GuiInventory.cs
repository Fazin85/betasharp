using betareborn.Entities;
using betareborn.Rendering;
using Silk.NET.OpenGL.Legacy;

namespace betareborn.Guis
{
    public class GuiInventory : GuiContainer
    {

        private float xSize_lo;
        private float ySize_lo;

        public GuiInventory(EntityPlayer var1) : base(var1.inventorySlots)
        {
            field_948_f = true;
            var1.addStat(Achievements.OPEN_INVENTORY, 1);
        }

        public override void initGui()
        {
            controlList.clear();
        }

        protected override void drawGuiContainerForegroundLayer()
        {
            fontRenderer.drawString("Crafting", 86, 16, 4210752);
        }

        public override void drawScreen(int var1, int var2, float var3)
        {
            base.drawScreen(var1, var2, var3);
            xSize_lo = (float)var1;
            ySize_lo = (float)var2;
        }

        protected override void drawGuiContainerBackgroundLayer(float var1)
        {
            int var2 = mc.renderEngine.getTexture("/gui/inventory.png");
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            mc.renderEngine.bindTexture(var2);
            int var3 = (width - xSize) / 2;
            int var4 = (height - ySize) / 2;
            drawTexturedModalRect(var3, var4, 0, 0, xSize, ySize);
            GLManager.GL.Enable(GLEnum.RescaleNormal);
            GLManager.GL.Enable(GLEnum.ColorMaterial);
            GLManager.GL.PushMatrix();
            GLManager.GL.Translate((float)(var3 + 51), (float)(var4 + 75), 50.0F);
            float var5 = 30.0F;
            GLManager.GL.Scale(-var5, var5, var5);
            GLManager.GL.Rotate(180.0F, 0.0F, 0.0F, 1.0F);
            float var6 = mc.thePlayer.renderYawOffset;
            float var7 = mc.thePlayer.rotationYaw;
            float var8 = mc.thePlayer.rotationPitch;
            float var9 = (float)(var3 + 51) - xSize_lo;
            float var10 = (float)(var4 + 75 - 50) - ySize_lo;
            GLManager.GL.Rotate(135.0F, 0.0F, 1.0F, 0.0F);
            RenderHelper.enableStandardItemLighting();
            GLManager.GL.Rotate(-135.0F, 0.0F, 1.0F, 0.0F);
            GLManager.GL.Rotate(-((float)java.lang.Math.atan((double)(var10 / 40.0F))) * 20.0F, 1.0F, 0.0F, 0.0F);
            mc.thePlayer.renderYawOffset = (float)java.lang.Math.atan((double)(var9 / 40.0F)) * 20.0F;
            mc.thePlayer.rotationYaw = (float)java.lang.Math.atan((double)(var9 / 40.0F)) * 40.0F;
            mc.thePlayer.rotationPitch = -((float)java.lang.Math.atan((double)(var10 / 40.0F))) * 20.0F;
            mc.thePlayer.entityBrightness = 1.0F;
            GLManager.GL.Translate(0.0F, mc.thePlayer.yOffset, 0.0F);
            RenderManager.instance.playerViewY = 180.0F;
            RenderManager.instance.renderEntityWithPosYaw(mc.thePlayer, 0.0D, 0.0D, 0.0D, 0.0F, 1.0F);
            mc.thePlayer.entityBrightness = 0.0F;
            mc.thePlayer.renderYawOffset = var6;
            mc.thePlayer.rotationYaw = var7;
            mc.thePlayer.rotationPitch = var8;
            GLManager.GL.PopMatrix();
            RenderHelper.disableStandardItemLighting();
            GLManager.GL.Disable(GLEnum.RescaleNormal);
        }

        protected override void actionPerformed(GuiButton var1)
        {
            if (var1.id == 0)
            {
                mc.displayGuiScreen(new GuiAchievements(mc.statFileWriter));
            }

            if (var1.id == 1)
            {
                mc.displayGuiScreen(new GuiStats(this, mc.statFileWriter));
            }

        }
    }

}