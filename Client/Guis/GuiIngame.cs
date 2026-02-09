using betareborn.Blocks;
using betareborn.Blocks.Materials;
using betareborn.Client.Rendering.Core;
using betareborn.Client.Rendering.Items;
using betareborn.Client.Resource.Language;
using betareborn.Inventorys;
using betareborn.Items;
using betareborn.Util.Maths;
using java.awt;
using java.util;
using Silk.NET.OpenGL.Legacy;
using System.Diagnostics;

namespace betareborn.Client.Guis
{
    public class GuiIngame : Gui
    {

        private static ItemRenderer itemRenderer = new ItemRenderer();
        private java.util.List chatMessageList = new ArrayList();
        private java.util.Random rand = new();
        private Minecraft mc;
        public string field_933_a = null;
        private int updateCounter = 0;
        private string recordPlaying = "";
        private int recordPlayingUpFor = 0;
        private bool field_22065_l = false;
        public float damageGuiPartialTime;
        float prevVignetteBrightness = 1.0F;

        public GuiIngame(Minecraft gameInstance)
        {
            mc = gameInstance;
        }

        public void renderGameOverlay(float var1, bool var2, int var3, int var4)
        {
            ScaledResolution var5 = new ScaledResolution(mc.options, mc.displayWidth, mc.displayHeight);
            int var6 = var5.getScaledWidth();
            int var7 = var5.getScaledHeight();
            TextRenderer var8 = mc.fontRenderer;
            mc.gameRenderer.setupHudRender();
            GLManager.GL.Enable(GLEnum.Blend);
            if (Minecraft.isFancyGraphicsEnabled())
            {
                renderVignette(mc.player.getBrightnessAtEyes(var1), var6, var7);
            }

            ItemStack var9 = mc.player.inventory.armorItemInSlot(3);
            if (!mc.options.thirdPersonView && var9 != null && var9.itemId == Block.PUMPKIN.id)
            {
                renderPumpkinBlur(var6, var7);
            }

            float var10 = mc.player.lastScreenDistortion + (mc.player.changeDimensionCooldown - mc.player.lastScreenDistortion) * var1;
            if (var10 > 0.0F)
            {
                renderPortalOverlay(var10, var6, var7);
            }

            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.getTextureId("/gui/gui.png"));
            InventoryPlayer var11 = mc.player.inventory;
            zLevel = -90.0F;
            drawTexturedModalRect(var6 / 2 - 91, var7 - 22, 0, 0, 182, 22);
            drawTexturedModalRect(var6 / 2 - 91 - 1 + var11.selectedSlot * 20, var7 - 22 - 1, 0, 22, 24, 22);
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.getTextureId("/gui/icons.png"));
            GLManager.GL.Enable(GLEnum.Blend);
            GLManager.GL.BlendFunc(GLEnum.OneMinusDstColor, GLEnum.OneMinusSrcColor);
            drawTexturedModalRect(var6 / 2 - 7, var7 / 2 - 7, 0, 0, 16, 16);
            GLManager.GL.Disable(GLEnum.Blend);
            bool var12 = mc.player.hearts / 3 % 2 == 1;
            if (mc.player.hearts < 10)
            {
                var12 = false;
            }

            int var13 = mc.player.health;
            int var14 = mc.player.lastHealth;
            rand.setSeed(updateCounter * 312871);
            int var15;
            int var16;
            int var17;
            if (mc.playerController.shouldDrawHUD())
            {
                var15 = mc.player.getPlayerArmorValue();

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

                if (mc.player.isInFluid(Material.WATER))
                {
                    var16 = (int)java.lang.Math.ceil((mc.player.air - 2) * 10.0D / 300.0D);
                    var17 = (int)java.lang.Math.ceil(mc.player.air * 10.0D / 300.0D) - var16;

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
            Lighting.turnOn();
            GLManager.GL.PopMatrix();

            for (var15 = 0; var15 < 9; ++var15)
            {
                var16 = var6 / 2 - 90 + var15 * 20 + 2;
                var17 = var7 - 16 - 3;
                renderInventorySlot(var15, var16, var17, var1);
            }

            Lighting.turnOff();
            GLManager.GL.Disable(GLEnum.RescaleNormal);
            if (mc.player.getSleepTimer() > 0)
            {
                GLManager.GL.Disable(GLEnum.DepthTest);
                GLManager.GL.Disable(GLEnum.AlphaTest);
                var15 = mc.player.getSleepTimer();
                float var27 = var15 / 100.0F;
                if (var27 > 1.0F)
                {
                    var27 = 1.0F - (var15 - 100) / 10.0F;
                }

                var17 = (int)(220.0F * var27) << 24 | 1052704;
                drawRect(0, 0, var6, var7, var17);
                GLManager.GL.Enable(GLEnum.AlphaTest);
                GLManager.GL.Enable(GLEnum.DepthTest);
            }

            string line;
            if (mc.options.showDebugInfo)
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
                long maxMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
                long usedMemory = Process.GetCurrentProcess().WorkingSet64;
                long usedHeapMemory = GC.GetTotalMemory(false);
                line = "Used memory: " + usedMemory * 100L / maxMemory + "% (" + usedMemory / 1024L / 1024L + "MB) of " + maxMemory / 1024L / 1024L + "MB";
                drawString(var8, line, var6 - var8.getStringWidth(line) - 2, 2, 14737632);
                line = "Used heap memory: " + usedHeapMemory / 1024L / 1024L + "MB";
                drawString(var8, line, var6 - var8.getStringWidth(line) - 2, 12, 14737632);
                drawString(var8, "x: " + mc.player.x, 2, 64, 14737632);
                drawString(var8, "y: " + mc.player.y, 2, 72, 14737632);
                drawString(var8, "z: " + mc.player.z, 2, 80, 14737632);
                drawString(var8, "f: " + (MathHelper.floor_double((double)(mc.player.yaw * 4.0F / 360.0F) + 0.5D) & 3), 2, 88, 14737632);
                GLManager.GL.PopMatrix();
            }

            if (recordPlayingUpFor > 0)
            {
                float var25 = recordPlayingUpFor - var1;
                var16 = (int)(var25 * 256.0F / 20.0F);
                if (var16 > 255)
                {
                    var16 = 255;
                }

                if (var16 > 0)
                {
                    GLManager.GL.PushMatrix();
                    GLManager.GL.Translate(var6 / 2, var7 - 48, 0.0F);
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
            GLManager.GL.Translate(0.0F, var7 - 48, 0.0F);

            for (var17 = 0; var17 < chatMessageList.size() && var17 < var26; ++var17)
            {
                if (((ChatLine)chatMessageList.get(var17)).updateCounter < 200 || var31)
                {
                    double var32 = ((ChatLine)chatMessageList.get(var17)).updateCounter / 200.0D;
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
                        line = ((ChatLine)chatMessageList.get(var17)).message;
                        drawRect(var33, var22 - 1, var33 + 320, var22 + 8, var20 / 2 << 24);
                        GLManager.GL.Enable(GLEnum.Blend);
                        var8.drawStringWithShadow(line, var33, var22, 16777215 + (var20 << 24));
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
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.getTextureId("%blur%/misc/pumpkinblur.png"));
            Tessellator var3 = Tessellator.instance;
            var3.startDrawingQuads();
            var3.addVertexWithUV(0.0D, var2, -90.0D, 0.0D, 1.0D);
            var3.addVertexWithUV(var1, var2, -90.0D, 1.0D, 1.0D);
            var3.addVertexWithUV(var1, 0.0D, -90.0D, 1.0D, 0.0D);
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

            prevVignetteBrightness = (float)(prevVignetteBrightness + (double)(var1 - prevVignetteBrightness) * 0.01D);
            GLManager.GL.Disable(GLEnum.DepthTest);
            GLManager.GL.DepthMask(false);
            GLManager.GL.BlendFunc(GLEnum.Zero, GLEnum.OneMinusSrcColor);
            GLManager.GL.Color4(prevVignetteBrightness, prevVignetteBrightness, prevVignetteBrightness, 1.0F);
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.getTextureId("%blur%/misc/vignette.png"));
            Tessellator var4 = Tessellator.instance;
            var4.startDrawingQuads();
            var4.addVertexWithUV(0.0D, var3, -90.0D, 0.0D, 1.0D);
            var4.addVertexWithUV(var2, var3, -90.0D, 1.0D, 1.0D);
            var4.addVertexWithUV(var2, 0.0D, -90.0D, 1.0D, 0.0D);
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
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.getTextureId("/terrain.png"));
            float var4 = Block.NETHER_PORTAL.textureId % 16 / 16.0F;
            float var5 = Block.NETHER_PORTAL.textureId / 16 / 16.0F;
            float var6 = (Block.NETHER_PORTAL.textureId % 16 + 1) / 16.0F;
            float var7 = (Block.NETHER_PORTAL.textureId / 16 + 1) / 16.0F;
            Tessellator var8 = Tessellator.instance;
            var8.startDrawingQuads();
            var8.addVertexWithUV(0.0D, var3, -90.0D, (double)var4, (double)var7);
            var8.addVertexWithUV(var2, var3, -90.0D, (double)var6, (double)var7);
            var8.addVertexWithUV(var2, 0.0D, -90.0D, (double)var6, (double)var5);
            var8.addVertexWithUV(0.0D, 0.0D, -90.0D, (double)var4, (double)var5);
            var8.draw();
            GLManager.GL.DepthMask(true);
            GLManager.GL.Enable(GLEnum.DepthTest);
            GLManager.GL.Enable(GLEnum.AlphaTest);
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        }

        private void renderInventorySlot(int var1, int var2, int var3, float var4)
        {
            ItemStack var5 = mc.player.inventory.main[var1];
            if (var5 != null)
            {
                float var6 = var5.bobbingAnimationTime - var4;
                if (var6 > 0.0F)
                {
                    GLManager.GL.PushMatrix();
                    float var7 = 1.0F + var6 / 5.0F;
                    GLManager.GL.Translate(var2 + 8, var3 + 12, 0.0F);
                    GLManager.GL.Scale(1.0F / var7, (var7 + 1.0F) / 2.0F, 1.0F);
                    GLManager.GL.Translate(-(var2 + 8), -(var3 + 12), 0.0F);
                }

                itemRenderer.renderItemIntoGUI(mc.fontRenderer, mc.textureManager, var5, var2, var3);
                if (var6 > 0.0F)
                {
                    GLManager.GL.PopMatrix();
                }

                itemRenderer.renderItemOverlayIntoGUI(mc.fontRenderer, mc.textureManager, var5, var2, var3);
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

        public void addChatMessage(string message) {
            foreach (string line in message.Split("\n")) {
                addWrappedChatMessage(line);
            }
        }

        private void addWrappedChatMessage(string message)
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

        public void setRecordPlayingMessage(string var1)
        {
            recordPlaying = "Now playing: " + var1;
            recordPlayingUpFor = 60;
            field_22065_l = true;
        }

        public void addChatMessageTranslate(string var1)
        {
            TranslationStorage var2 = TranslationStorage.getInstance();
            string var3 = var2.translateKey(var1);
            addChatMessage(var3);
        }
    }

}