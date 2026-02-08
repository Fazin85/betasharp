using betareborn.Blocks;
using betareborn.Blocks.Materials;
using betareborn.Inventorys;
using betareborn.Items;
using betareborn.Rendering;
using java.awt;
using java.util;
using Silk.NET.OpenGL.Legacy;

namespace betareborn.Guis
{
    public class GuiIngame : Gui
    {

        private static RenderItem itemRenderer = new RenderItem();
        private java.util.List chatMessageList = new ArrayList();
        private java.util.Random rand = new();
        private Minecraft mc;
        public String field_933_a = null;
        private int updateCounter = 0;
        private String recordPlaying = "";
        private int recordPlayingUpFor = 0;
        private bool field_22065_l = false;
        public float damageGuiPartialTime;
        float prevVignetteBrightness = 1.0F;

        public GuiIngame(Minecraft var1)
        {
            mc = var1;
        }

        public void renderGameOverlay(float var1, bool var2, int var3, int var4)
        {
            ScaledResolution var5 = new ScaledResolution(mc.gameSettings, mc.displayWidth, mc.displayHeight);
            int var6 = var5.getScaledWidth();
            int var7 = var5.getScaledHeight();
            FontRenderer var8 = mc.fontRenderer;
            mc.entityRenderer.func_905_b();
            GLManager.GL.Enable(GLEnum.Blend);
            if (Minecraft.isFancyGraphicsEnabled())
            {
                renderVignette(mc.thePlayer.getEntityBrightness(var1), var6, var7);
            }

            ItemStack var9 = mc.thePlayer.inventory.armorItemInSlot(3);
            if (!mc.gameSettings.thirdPersonView && var9 != null && var9.itemID == Block.PUMPKIN.id)
            {
                renderPumpkinBlur(var6, var7);
            }

            float var10 = mc.thePlayer.prevTimeInPortal + (mc.thePlayer.timeInPortal - mc.thePlayer.prevTimeInPortal) * var1;
            if (var10 > 0.0F)
            {
                renderPortalOverlay(var10, var6, var7);
            }

            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.renderEngine.getTexture("/gui/gui.png"));
            InventoryPlayer var11 = mc.thePlayer.inventory;
            zLevel = -90.0F;
            drawTexturedModalRect(var6 / 2 - 91, var7 - 22, 0, 0, 182, 22);
            drawTexturedModalRect(var6 / 2 - 91 - 1 + var11.currentItem * 20, var7 - 22 - 1, 0, 22, 24, 22);
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.renderEngine.getTexture("/gui/icons.png"));
            GLManager.GL.Enable(GLEnum.Blend);
            GLManager.GL.BlendFunc(GLEnum.OneMinusDstColor, GLEnum.OneMinusSrcColor);
            drawTexturedModalRect(var6 / 2 - 7, var7 / 2 - 7, 0, 0, 16, 16);
            GLManager.GL.Disable(GLEnum.Blend);
            bool var12 = mc.thePlayer.heartsLife / 3 % 2 == 1;
            if (mc.thePlayer.heartsLife < 10)
            {
                var12 = false;
            }

            int var13 = mc.thePlayer.health;
            int var14 = mc.thePlayer.prevHealth;
            rand.setSeed((long)(updateCounter * 312871));
            int var15;
            int var16;
            int var17;
            if (mc.playerController.shouldDrawHUD())
            {
                var15 = mc.thePlayer.getPlayerArmorValue();

                int var18;
                for (var16 = 0; var16 < 10; ++var16)
                {
                    var17 = var7 - 32;
                    if (var15 > 0)
                    {
                        var18 = var6 / 2 + 91 - var16 * 8 - 9;
                        if (var16 * 2 + 1 < var15)
                        {
                            drawTexturedModalRect(var18, var17, 34, 9, 9, 9);
                        }

                        if (var16 * 2 + 1 == var15)
                        {
                            drawTexturedModalRect(var18, var17, 25, 9, 9, 9);
                        }

                        if (var16 * 2 + 1 > var15)
                        {
                            drawTexturedModalRect(var18, var17, 16, 9, 9, 9);
                        }
                    }

                    byte var28 = 0;
                    if (var12)
                    {
                        var28 = 1;
                    }

                    int var19 = var6 / 2 - 91 + var16 * 8;
                    if (var13 <= 4)
                    {
                        var17 += rand.nextInt(2);
                    }

                    drawTexturedModalRect(var19, var17, 16 + var28 * 9, 0, 9, 9);
                    if (var12)
                    {
                        if (var16 * 2 + 1 < var14)
                        {
                            drawTexturedModalRect(var19, var17, 70, 0, 9, 9);
                        }

                        if (var16 * 2 + 1 == var14)
                        {
                            drawTexturedModalRect(var19, var17, 79, 0, 9, 9);
                        }
                    }

                    if (var16 * 2 + 1 < var13)
                    {
                        drawTexturedModalRect(var19, var17, 52, 0, 9, 9);
                    }

                    if (var16 * 2 + 1 == var13)
                    {
                        drawTexturedModalRect(var19, var17, 61, 0, 9, 9);
                    }
                }

                if (mc.thePlayer.isInsideOfMaterial(Material.WATER))
                {
                    var16 = (int)java.lang.Math.ceil((double)(mc.thePlayer.air - 2) * 10.0D / 300.0D);
                    var17 = (int)java.lang.Math.ceil((double)mc.thePlayer.air * 10.0D / 300.0D) - var16;

                    for (var18 = 0; var18 < var16 + var17; ++var18)
                    {
                        if (var18 < var16)
                        {
                            drawTexturedModalRect(var6 / 2 - 91 + var18 * 8, var7 - 32 - 9, 16, 18, 9, 9);
                        }
                        else
                        {
                            drawTexturedModalRect(var6 / 2 - 91 + var18 * 8, var7 - 32 - 9, 25, 18, 9, 9);
                        }
                    }
                }
            }

            GLManager.GL.Disable(GLEnum.Blend);
            GLManager.GL.Enable(GLEnum.RescaleNormal);
            GLManager.GL.PushMatrix();
            GLManager.GL.Rotate(120.0F, 1.0F, 0.0F, 0.0F);
            RenderHelper.enableStandardItemLighting();
            GLManager.GL.PopMatrix();

            for (var15 = 0; var15 < 9; ++var15)
            {
                var16 = var6 / 2 - 90 + var15 * 20 + 2;
                var17 = var7 - 16 - 3;
                renderInventorySlot(var15, var16, var17, var1);
            }

            RenderHelper.disableStandardItemLighting();
            GLManager.GL.Disable(GLEnum.RescaleNormal);
            if (mc.thePlayer.func_22060_M() > 0)
            {
                GLManager.GL.Disable(GLEnum.DepthTest);
                GLManager.GL.Disable(GLEnum.AlphaTest);
                var15 = mc.thePlayer.func_22060_M();
                float var27 = (float)var15 / 100.0F;
                if (var27 > 1.0F)
                {
                    var27 = 1.0F - (float)(var15 - 100) / 10.0F;
                }

                var17 = (int)(220.0F * var27) << 24 | 1052704;
                drawRect(0, 0, var6, var7, var17);
                GLManager.GL.Enable(GLEnum.AlphaTest);
                GLManager.GL.Enable(GLEnum.DepthTest);
            }

            String var23;
            if (mc.gameSettings.showDebugInfo)
            {
                GLManager.GL.PushMatrix();
                if (Minecraft.hasPaidCheckTime > 0L)
                {
                    GLManager.GL.Translate(0.0F, 32.0F, 0.0F);
                }

                var8.drawStringWithShadow("Minecraft Beta 1.7.3 (" + mc.debug + ")", 2, 2, 16777215);
                var8.drawStringWithShadow(mc.func_6262_n(), 2, 22, 16777215);
                var8.drawStringWithShadow(mc.func_6245_o(), 2, 32, 16777215);
                var8.drawStringWithShadow(mc.func_21002_o(), 2, 42, 16777215);
                long var24 = java.lang.Runtime.getRuntime().maxMemory();
                long var29 = java.lang.Runtime.getRuntime().totalMemory();
                long var30 = java.lang.Runtime.getRuntime().freeMemory();
                long var21 = var29 - var30;
                var23 = "Used memory: " + var21 * 100L / var24 + "% (" + var21 / 1024L / 1024L + "MB) of " + var24 / 1024L / 1024L + "MB";
                drawString(var8, var23, var6 - var8.getStringWidth(var23) - 2, 2, 14737632);
                var23 = "Allocated memory: " + var29 * 100L / var24 + "% (" + var29 / 1024L / 1024L + "MB)";
                drawString(var8, var23, var6 - var8.getStringWidth(var23) - 2, 12, 14737632);
                drawString(var8, "x: " + mc.thePlayer.posX, 2, 64, 14737632);
                drawString(var8, "y: " + mc.thePlayer.posY, 2, 72, 14737632);
                drawString(var8, "z: " + mc.thePlayer.posZ, 2, 80, 14737632);
                drawString(var8, "f: " + (MathHelper.floor_double((double)(mc.thePlayer.rotationYaw * 4.0F / 360.0F) + 0.5D) & 3), 2, 88, 14737632);
                GLManager.GL.PopMatrix();
            }

            if (recordPlayingUpFor > 0)
            {
                float var25 = (float)recordPlayingUpFor - var1;
                var16 = (int)(var25 * 256.0F / 20.0F);
                if (var16 > 255)
                {
                    var16 = 255;
                }

                if (var16 > 0)
                {
                    GLManager.GL.PushMatrix();
                    GLManager.GL.Translate((float)(var6 / 2), (float)(var7 - 48), 0.0F);
                    GLManager.GL.Enable(GLEnum.Blend);
                    GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
                    var17 = 16777215;
                    if (field_22065_l)
                    {
                        var17 = Color.HSBtoRGB(var25 / 50.0F, 0.7F, 0.6F) & 16777215;
                    }

                    var8.drawString(recordPlaying, -var8.getStringWidth(recordPlaying) / 2, -4, var17 + (var16 << 24));
                    GLManager.GL.Disable(GLEnum.Blend);
                    GLManager.GL.PopMatrix();
                }
            }

            byte var26 = 10;
            bool var31 = false;
            if (mc.currentScreen is GuiChat)
            {
                var26 = 20;
                var31 = true;
            }

            GLManager.GL.Enable(GLEnum.Blend);
            GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
            GLManager.GL.Disable(GLEnum.AlphaTest);
            GLManager.GL.PushMatrix();
            GLManager.GL.Translate(0.0F, (float)(var7 - 48), 0.0F);

            for (var17 = 0; var17 < chatMessageList.size() && var17 < var26; ++var17)
            {
                if (((ChatLine)chatMessageList.get(var17)).updateCounter < 200 || var31)
                {
                    double var32 = (double)((ChatLine)chatMessageList.get(var17)).updateCounter / 200.0D;
                    var32 = 1.0D - var32;
                    var32 *= 10.0D;
                    if (var32 < 0.0D)
                    {
                        var32 = 0.0D;
                    }

                    if (var32 > 1.0D)
                    {
                        var32 = 1.0D;
                    }

                    var32 *= var32;
                    int var20 = (int)(255.0D * var32);
                    if (var31)
                    {
                        var20 = 255;
                    }

                    if (var20 > 0)
                    {
                        byte var33 = 2;
                        int var22 = -var17 * 9;
                        var23 = ((ChatLine)chatMessageList.get(var17)).message;
                        drawRect(var33, var22 - 1, var33 + 320, var22 + 8, var20 / 2 << 24);
                        GLManager.GL.Enable(GLEnum.Blend);
                        var8.drawStringWithShadow(var23, var33, var22, 16777215 + (var20 << 24));
                    }
                }
            }

            GLManager.GL.PopMatrix();
            GLManager.GL.Enable(GLEnum.AlphaTest);
            GLManager.GL.Disable(GLEnum.Blend);
        }

        private void renderPumpkinBlur(int var1, int var2)
        {
            GLManager.GL.Disable(GLEnum.DepthTest);
            GLManager.GL.DepthMask(false);
            GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            GLManager.GL.Disable(GLEnum.AlphaTest);
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.renderEngine.getTexture("%blur%/misc/pumpkinblur.png"));
            Tessellator var3 = Tessellator.instance;
            var3.startDrawingQuads();
            var3.addVertexWithUV(0.0D, (double)var2, -90.0D, 0.0D, 1.0D);
            var3.addVertexWithUV((double)var1, (double)var2, -90.0D, 1.0D, 1.0D);
            var3.addVertexWithUV((double)var1, 0.0D, -90.0D, 1.0D, 0.0D);
            var3.addVertexWithUV(0.0D, 0.0D, -90.0D, 0.0D, 0.0D);
            var3.draw();
            GLManager.GL.DepthMask(true);
            GLManager.GL.Enable(GLEnum.DepthTest);
            GLManager.GL.Enable(GLEnum.AlphaTest);
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        }

        private void renderVignette(float var1, int var2, int var3)
        {
            var1 = 1.0F - var1;
            if (var1 < 0.0F)
            {
                var1 = 0.0F;
            }

            if (var1 > 1.0F)
            {
                var1 = 1.0F;
            }

            prevVignetteBrightness = (float)((double)prevVignetteBrightness + (double)(var1 - prevVignetteBrightness) * 0.01D);
            GLManager.GL.Disable(GLEnum.DepthTest);
            GLManager.GL.DepthMask(false);
            GLManager.GL.BlendFunc(GLEnum.Zero, GLEnum.OneMinusSrcColor);
            GLManager.GL.Color4(prevVignetteBrightness, prevVignetteBrightness, prevVignetteBrightness, 1.0F);
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.renderEngine.getTexture("%blur%/misc/vignette.png"));
            Tessellator var4 = Tessellator.instance;
            var4.startDrawingQuads();
            var4.addVertexWithUV(0.0D, (double)var3, -90.0D, 0.0D, 1.0D);
            var4.addVertexWithUV((double)var2, (double)var3, -90.0D, 1.0D, 1.0D);
            var4.addVertexWithUV((double)var2, 0.0D, -90.0D, 1.0D, 0.0D);
            var4.addVertexWithUV(0.0D, 0.0D, -90.0D, 0.0D, 0.0D);
            var4.draw();
            GLManager.GL.DepthMask(true);
            GLManager.GL.Enable(GLEnum.DepthTest);
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        }

        private void renderPortalOverlay(float var1, int var2, int var3)
        {
            if (var1 < 1.0F)
            {
                var1 *= var1;
                var1 *= var1;
                var1 = var1 * 0.8F + 0.2F;
            }

            GLManager.GL.Disable(GLEnum.AlphaTest);
            GLManager.GL.Disable(GLEnum.DepthTest);
            GLManager.GL.DepthMask(false);
            GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, var1);
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.renderEngine.getTexture("/terrain.png"));
            float var4 = (float)(Block.NETHER_PORTAL.textureId % 16) / 16.0F;
            float var5 = (float)(Block.NETHER_PORTAL.textureId / 16) / 16.0F;
            float var6 = (float)(Block.NETHER_PORTAL.textureId % 16 + 1) / 16.0F;
            float var7 = (float)(Block.NETHER_PORTAL.textureId / 16 + 1) / 16.0F;
            Tessellator var8 = Tessellator.instance;
            var8.startDrawingQuads();
            var8.addVertexWithUV(0.0D, (double)var3, -90.0D, (double)var4, (double)var7);
            var8.addVertexWithUV((double)var2, (double)var3, -90.0D, (double)var6, (double)var7);
            var8.addVertexWithUV((double)var2, 0.0D, -90.0D, (double)var6, (double)var5);
            var8.addVertexWithUV(0.0D, 0.0D, -90.0D, (double)var4, (double)var5);
            var8.draw();
            GLManager.GL.DepthMask(true);
            GLManager.GL.Enable(GLEnum.DepthTest);
            GLManager.GL.Enable(GLEnum.AlphaTest);
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        }

        private void renderInventorySlot(int var1, int var2, int var3, float var4)
        {
            ItemStack var5 = mc.thePlayer.inventory.mainInventory[var1];
            if (var5 != null)
            {
                float var6 = (float)var5.animationsToGo - var4;
                if (var6 > 0.0F)
                {
                    GLManager.GL.PushMatrix();
                    float var7 = 1.0F + var6 / 5.0F;
                    GLManager.GL.Translate((float)(var2 + 8), (float)(var3 + 12), 0.0F);
                    GLManager.GL.Scale(1.0F / var7, (var7 + 1.0F) / 2.0F, 1.0F);
                    GLManager.GL.Translate((float)(-(var2 + 8)), (float)(-(var3 + 12)), 0.0F);
                }

                itemRenderer.renderItemIntoGUI(mc.fontRenderer, mc.renderEngine, var5, var2, var3);
                if (var6 > 0.0F)
                {
                    GLManager.GL.PopMatrix();
                }

                itemRenderer.renderItemOverlayIntoGUI(mc.fontRenderer, mc.renderEngine, var5, var2, var3);
            }
        }

        public void updateTick()
        {
            if (recordPlayingUpFor > 0)
            {
                --recordPlayingUpFor;
            }

            ++updateCounter;

            for (int var1 = 0; var1 < chatMessageList.size(); ++var1)
            {
                ++((ChatLine)chatMessageList.get(var1)).updateCounter;
            }

        }

        public void clearChatMessages()
        {
            chatMessageList.clear();
        }

        public void addChatMessage(String message) {
            foreach (String line in message.Split("\n")) {
                addWrappedChatMessage(line);
            }
        }

        private void addWrappedChatMessage(String message)
        {
            while (mc.fontRenderer.getStringWidth(message) > 320)
            {
                int i;
                for (i = 1; i < message.Length && mc.fontRenderer.getStringWidth(message.Substring(0, i + 1)) <= 320; ++i)
                {
                }

                chatMessageList.add(0, new ChatLine(message.Substring(0, i)));
                message = message.Substring(i);
            }

            chatMessageList.add(0, new ChatLine(message));

            while (chatMessageList.size() > 50)
            {
                chatMessageList.remove(chatMessageList.size() - 1);
            }
        }

        public void setRecordPlayingMessage(String var1)
        {
            recordPlaying = "Now playing: " + var1;
            recordPlayingUpFor = 60;
            field_22065_l = true;
        }

        public void addChatMessageTranslate(String var1)
        {
            StringTranslate var2 = StringTranslate.getInstance();
            String var3 = var2.translateKey(var1);
            addChatMessage(var3);
        }
    }

}