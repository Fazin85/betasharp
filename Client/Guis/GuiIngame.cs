using betareborn.Blocks;
using betareborn.Blocks.Materials;
using betareborn.Client.Rendering.Core;
using betareborn.Client.Rendering.Items;
using betareborn.Client.Resource.Language;
using betareborn.Inventorys;
using betareborn.Items;
using betareborn.Util;
using betareborn.Util.Maths;
using java.awt;
using java.util;
using Silk.NET.OpenGL.Legacy;

namespace betareborn.Client.Guis
{
    public class GuiIngame : Gui
    {
        private readonly GCMonitor GCMonitor;
        private static readonly ItemRenderer itemRenderer = new ItemRenderer();
        private readonly java.util.List chatMessageList = new ArrayList();
        private readonly java.util.Random rand = new();
        private readonly Minecraft mc;
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
            GCMonitor = new GCMonitor();
        }

        public void renderGameOverlay(float partialTicks, bool unusedFlag, int unusedA, int unusedB)
        {
            ScaledResolution scaled = new ScaledResolution(mc.options, mc.displayWidth, mc.displayHeight);
            int scaledWidth = scaled.getScaledWidth();
            int scaledHeight = scaled.getScaledHeight();
            TextRenderer font = mc.fontRenderer;
            mc.gameRenderer.setupHudRender();
            GLManager.GL.Enable(GLEnum.Blend);
            if (Minecraft.isFancyGraphicsEnabled())
            {
                renderVignette(mc.player.getBrightnessAtEyes(partialTicks), scaledWidth, scaledHeight);
            }

            ItemStack helmet = mc.player.inventory.armorItemInSlot(3);
            if (!mc.options.thirdPersonView && helmet != null && helmet.itemId == Block.PUMPKIN.id)
            {
                renderPumpkinBlur(scaledWidth, scaledHeight);
            }

            float screenDistortion = mc.player.lastScreenDistortion + (mc.player.changeDimensionCooldown - mc.player.lastScreenDistortion) * partialTicks;
            if (screenDistortion > 0.0F)
            {
                renderPortalOverlay(screenDistortion, scaledWidth, scaledHeight);
            }

            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.getTextureId("/gui/gui.png"));
            InventoryPlayer inventory = mc.player.inventory;
            zLevel = -90.0F;
            drawTexturedModalRect(scaledWidth / 2 - 91, scaledHeight - 22, 0, 0, 182, 22);
            drawTexturedModalRect(scaledWidth / 2 - 91 - 1 + inventory.selectedSlot * 20, scaledHeight - 22 - 1, 0, 22, 24, 22);
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.getTextureId("/gui/icons.png"));
            GLManager.GL.Enable(GLEnum.Blend);
            GLManager.GL.BlendFunc(GLEnum.OneMinusDstColor, GLEnum.OneMinusSrcColor);
            drawTexturedModalRect(scaledWidth / 2 - 7, scaledHeight / 2 - 7, 0, 0, 16, 16);
            GLManager.GL.Disable(GLEnum.Blend);
            bool heartBlink = mc.player.hearts / 3 % 2 == 1;
            if (mc.player.hearts < 10)
            {
                heartBlink = false;
            }

            int health = mc.player.health;
            int lastHealth = mc.player.lastHealth;
            rand.setSeed(updateCounter * 312871);
            int armorValue;
            int i;
            int j;
            if (mc.playerController.shouldDrawHUD())
            {
                armorValue = mc.player.getPlayerArmorValue();

                int k;
                for (i = 0; i < 10; ++i)
                {
                    j = scaledHeight - 32;
                    if (armorValue > 0)
                    {
                        k = scaledWidth / 2 + 91 - i * 8 - 9;
                        if (i * 2 + 1 < armorValue)
                        {
                            drawTexturedModalRect(k, j, 34, 9, 9, 9);
                        }

                        if (i * 2 + 1 == armorValue)
                        {
                            drawTexturedModalRect(k, j, 25, 9, 9, 9);
                        }

                        if (i * 2 + 1 > armorValue)
                        {
                            drawTexturedModalRect(k, j, 16, 9, 9, 9);
                        }
                    }

                    byte blinkIndex = 0;
                    if (heartBlink)
                    {
                        blinkIndex = 1;
                    }

                    int x = scaledWidth / 2 - 91 + i * 8;
                    if (health <= 4)
                    {
                        j += rand.nextInt(2);
                    }

                    drawTexturedModalRect(x, j, 16 + blinkIndex * 9, 0, 9, 9);
                    if (heartBlink)
                    {
                        if (i * 2 + 1 < lastHealth)
                        {
                            drawTexturedModalRect(x, j, 70, 0, 9, 9);
                        }

                        if (i * 2 + 1 == lastHealth)
                        {
                            drawTexturedModalRect(x, j, 79, 0, 9, 9);
                        }
                    }

                    if (i * 2 + 1 < health)
                    {
                        drawTexturedModalRect(x, j, 52, 0, 9, 9);
                    }

                    if (i * 2 + 1 == health)
                    {
                        drawTexturedModalRect(x, j, 61, 0, 9, 9);
                    }
                }

                if (mc.player.isInFluid(Material.WATER))
                {
                    i = (int)java.lang.Math.ceil((mc.player.air - 2) * 10.0D / 300.0D);
                    j = (int)java.lang.Math.ceil(mc.player.air * 10.0D / 300.0D) - i;

                    for (k = 0; k < i + j; ++k)
                    {
                        if (k < i)
                        {
                            drawTexturedModalRect(scaledWidth / 2 - 91 + k * 8, scaledHeight - 32 - 9, 16, 18, 9, 9);
                        }
                        else
                        {
                            drawTexturedModalRect(scaledWidth / 2 - 91 + k * 8, scaledHeight - 32 - 9, 25, 18, 9, 9);
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

            for (armorValue = 0; armorValue < 9; ++armorValue)
            {
                i = scaledWidth / 2 - 90 + armorValue * 20 + 2;
                j = scaledHeight - 16 - 3;
                renderInventorySlot(armorValue, i, j, partialTicks);
            }

            Lighting.turnOff();
            GLManager.GL.Disable(GLEnum.RescaleNormal);
            if (mc.player.getSleepTimer() > 0)
            {
                GLManager.GL.Disable(GLEnum.DepthTest);
                GLManager.GL.Disable(GLEnum.AlphaTest);
                armorValue = mc.player.getSleepTimer();
                float sleepAlpha = armorValue / 100.0F;
                if (sleepAlpha > 1.0F)
                {
                    sleepAlpha = 1.0F - (armorValue - 100) / 10.0F;
                }

                j = (int)(220.0F * sleepAlpha) << 24 | 1052704;
                drawRect(0, 0, scaledWidth, scaledHeight, j);
                GLManager.GL.Enable(GLEnum.AlphaTest);
                GLManager.GL.Enable(GLEnum.DepthTest);
            }

            string debugStr;
            if (mc.options.showDebugInfo)
            {
                GCMonitor.AllowUpdating = true;
                GLManager.GL.PushMatrix();
                if (Minecraft.hasPaidCheckTime > 0L)
                    GLManager.GL.Translate(0.0F, 32.0F, 0.0F);

                font.drawStringWithShadow("Minecraft Beta 1.7.3 (" + mc.debug + ")", 2, 2, 16777215);
                font.drawStringWithShadow(mc.func_6262_n(), 2, 22, 16777215);
                font.drawStringWithShadow(mc.func_6245_o(), 2, 32, 16777215);
                font.drawStringWithShadow(mc.func_21002_o(), 2, 42, 16777215);
                long maxMem = GCMonitor.MaxMemoryBytes;
                long usedMem = GCMonitor.UsedMemoryBytes;
                long heapMem = GCMonitor.UsedHeapBytes;
                debugStr = "Used memory: " + usedMem * 100L / maxMem + "% (" + usedMem / 1024L / 1024L + "MB) of " + maxMem / 1024L / 1024L + "MB";
                drawString(font, debugStr, scaledWidth - font.getStringWidth(debugStr) - 2, 2, 14737632);
                debugStr = "GC heap: " + heapMem * 100L / maxMem + "% (" + heapMem / 1024L / 1024L + "MB)";
                drawString(font, debugStr, scaledWidth - font.getStringWidth(debugStr) - 2, 12, 14737632);
                drawString(font, "x: " + mc.player.x, 2, 64, 14737632);
                drawString(font, "y: " + mc.player.y, 2, 72, 14737632);
                drawString(font, "z: " + mc.player.z, 2, 80, 14737632);
                drawString(font, "f: " + (MathHelper.floor_double((double)(mc.player.yaw * 4.0F / 360.0F) + 0.5D) & 3), 2, 88, 14737632);

                if (mc.internalServer != null)
                {
                    drawString(font, $"Server TPS: {mc.internalServer.Tps:F1}", 2, 104, 14737632);
                }

                int meshY = mc.internalServer != null ? 120 : 104;
                var cr = mc.terrainRenderer.chunkRenderer;
                drawString(font, $"Meshes: S: {cr.LoadedMeshes} T: {cr.TranslucentMeshes}", 2, meshY, 14737632);
                GLManager.GL.PopMatrix();
            }
            else
            {
                GCMonitor.AllowUpdating = false;
            }

            if (recordPlayingUpFor > 0)
            {
                float t = recordPlayingUpFor - partialTicks;
                i = (int)(t * 256.0F / 20.0F);
                if (i > 255)
                {
                    i = 255;
                }

                if (i > 0)
                {
                    GLManager.GL.PushMatrix();
                    GLManager.GL.Translate(scaledWidth / 2, scaledHeight - 48, 0.0F);
                    GLManager.GL.Enable(GLEnum.Blend);
                    GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
                    j = 16777215;
                    if (field_22065_l)
                    {
                        j = Color.HSBtoRGB(t / 50.0F, 0.7F, 0.6F) & 16777215;
                    }

                    font.drawString(recordPlaying, -font.getStringWidth(recordPlaying) / 2, -4, j + (i << 24));
                    GLManager.GL.Disable(GLEnum.Blend);
                    GLManager.GL.PopMatrix();
                }
            }

            byte linesToShow = 10;
            bool chatOpen = false;
            if (mc.currentScreen is GuiChat)
            {
                linesToShow = 20;
                chatOpen = true;
            }

            GLManager.GL.Enable(GLEnum.Blend);
            GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
            GLManager.GL.Disable(GLEnum.AlphaTest);
            GLManager.GL.PushMatrix();
            GLManager.GL.Translate(0.0F, scaledHeight - 48, 0.0F);

            for (j = 0; j < chatMessageList.size() && j < linesToShow; ++j)
            {
                if (((ChatLine)chatMessageList.get(j)).updateCounter < 200 || chatOpen)
                {
                    double d = ((ChatLine)chatMessageList.get(j)).updateCounter / 200.0D;
                    d = 1.0D - d;
                    d *= 10.0D;
                    if (d < 0.0D)
                    {
                        d = 0.0D;
                    }

                    if (d > 1.0D)
                    {
                        d = 1.0D;
                    }

                    d *= d;
                    int alpha = (int)(255.0D * d);
                    if (chatOpen)
                    {
                        alpha = 255;
                    }

                    if (alpha > 0)
                    {
                        byte left = 2;
                        int y = -j * 9;
                        debugStr = ((ChatLine)chatMessageList.get(j)).message;
                        drawRect(left, y - 1, left + 320, y + 8, alpha / 2 << 24);
                        GLManager.GL.Enable(GLEnum.Blend);
                        font.drawStringWithShadow(debugStr, left, y, 16777215 + (alpha << 24));
                    }
                }
            }

            GLManager.GL.PopMatrix();
            GLManager.GL.Enable(GLEnum.AlphaTest);
            GLManager.GL.Disable(GLEnum.Blend);
        }

        private void renderPumpkinBlur(int screenWidth, int screenHeight)
        {
            GLManager.GL.Disable(GLEnum.DepthTest);
            GLManager.GL.DepthMask(false);
            GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            GLManager.GL.Disable(GLEnum.AlphaTest);
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.getTextureId("%blur%/misc/pumpkinblur.png"));
            Tessellator tess = Tessellator.instance;
            tess.startDrawingQuads();
            tess.addVertexWithUV(0.0D, screenHeight, -90.0D, 0.0D, 1.0D);
            tess.addVertexWithUV(screenWidth, screenHeight, -90.0D, 1.0D, 1.0D);
            tess.addVertexWithUV(screenWidth, 0.0D, -90.0D, 1.0D, 0.0D);
            tess.addVertexWithUV(0.0D, 0.0D, -90.0D, 0.0D, 0.0D);
            tess.draw();
            GLManager.GL.DepthMask(true);
            GLManager.GL.Enable(GLEnum.DepthTest);
            GLManager.GL.Enable(GLEnum.AlphaTest);
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        }

        private void renderVignette(float darkness, int screenWidth, int screenHeight)
        {
            darkness = 1.0F - darkness;
            if (darkness < 0.0F)
            {
                darkness = 0.0F;
            }

            if (darkness > 1.0F)
            {
                darkness = 1.0F;
            }

            prevVignetteBrightness = (float)(prevVignetteBrightness + (double)(darkness - prevVignetteBrightness) * 0.01D);
            GLManager.GL.Disable(GLEnum.DepthTest);
            GLManager.GL.DepthMask(false);
            GLManager.GL.BlendFunc(GLEnum.Zero, GLEnum.OneMinusSrcColor);
            GLManager.GL.Color4(prevVignetteBrightness, prevVignetteBrightness, prevVignetteBrightness, 1.0F);
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.getTextureId("%blur%/misc/vignette.png"));
            Tessellator tess = Tessellator.instance;
            tess.startDrawingQuads();
            tess.addVertexWithUV(0.0D, screenHeight, -90.0D, 0.0D, 1.0D);
            tess.addVertexWithUV(screenWidth, screenHeight, -90.0D, 1.0D, 1.0D);
            tess.addVertexWithUV(screenWidth, 0.0D, -90.0D, 1.0D, 0.0D);
            tess.addVertexWithUV(0.0D, 0.0D, -90.0D, 0.0D, 0.0D);
            tess.draw();
            GLManager.GL.DepthMask(true);
            GLManager.GL.Enable(GLEnum.DepthTest);
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        }

        private void renderPortalOverlay(float portalStrength, int screenWidth, int screenHeight)
        {
            if (portalStrength < 1.0F)
            {
                portalStrength *= portalStrength;
                portalStrength *= portalStrength;
                portalStrength = portalStrength * 0.8F + 0.2F;
            }

            GLManager.GL.Disable(GLEnum.AlphaTest);
            GLManager.GL.Disable(GLEnum.DepthTest);
            GLManager.GL.DepthMask(false);
            GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, portalStrength);
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.getTextureId("/terrain.png"));
            float u1 = Block.NETHER_PORTAL.textureId % 16 / 16.0F;
            float v1 = Block.NETHER_PORTAL.textureId / 16 / 16.0F;
            float u2 = (Block.NETHER_PORTAL.textureId % 16 + 1) / 16.0F;
            float v2 = (Block.NETHER_PORTAL.textureId / 16 + 1) / 16.0F;
            Tessellator tess = Tessellator.instance;
            tess.startDrawingQuads();
            tess.addVertexWithUV(0.0D, screenHeight, -90.0D, (double)u1, (double)v2);
            tess.addVertexWithUV(screenWidth, screenHeight, -90.0D, (double)u2, (double)v2);
            tess.addVertexWithUV(screenWidth, 0.0D, -90.0D, (double)u2, (double)v1);
            tess.addVertexWithUV(0.0D, 0.0D, -90.0D, (double)u1, (double)v1);
            tess.draw();
            GLManager.GL.DepthMask(true);
            GLManager.GL.Enable(GLEnum.DepthTest);
            GLManager.GL.Enable(GLEnum.AlphaTest);
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        }

        private void renderInventorySlot(int slotIndex, int x, int y, float partialTicks)
        {
            ItemStack stack = mc.player.inventory.main[slotIndex];
            if (stack != null)
            {
                float bob = stack.bobbingAnimationTime - partialTicks;
                if (bob > 0.0F)
                {
                    GLManager.GL.PushMatrix();
                    float scale = 1.0F + bob / 5.0F;
                    GLManager.GL.Translate(x + 8, y + 12, 0.0F);
                    GLManager.GL.Scale(1.0F / scale, (scale + 1.0F) / 2.0F, 1.0F);
                    GLManager.GL.Translate(-(x + 8), -(y + 12), 0.0F);
                }

                itemRenderer.renderItemIntoGUI(mc.fontRenderer, mc.textureManager, stack, x, y);
                if (bob > 0.0F)
                {
                    GLManager.GL.PopMatrix();
                }

                itemRenderer.renderItemOverlayIntoGUI(mc.fontRenderer, mc.textureManager, stack, x, y);
            }
        }

        public void updateTick()
        {
            if (recordPlayingUpFor > 0)
            {
                --recordPlayingUpFor;
            }

            ++updateCounter;

            for (int i = 0; i < chatMessageList.size(); ++i)
            {
                ++((ChatLine)chatMessageList.get(i)).updateCounter;
            }

        }

        public void clearChatMessages()
        {
            chatMessageList.clear();
        }

        public void addChatMessage(string message)
        {
            foreach (string line in message.Split("\n"))
            {
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

        public void setRecordPlayingMessage(string recordName)
        {
            recordPlaying = "Now playing: " + recordName;
            recordPlayingUpFor = 60;
            field_22065_l = true;
        }

        public void addChatMessageTranslate(string key)
        {
            TranslationStorage translations = TranslationStorage.getInstance();
            string translated = translations.translateKey(key);
            addChatMessage(translated);
        }
    }

}