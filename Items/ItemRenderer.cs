using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Items;
using betareborn.Materials;
using Silk.NET.OpenGL.Legacy;

namespace betareborn.Rendering
{
    public class ItemRenderer
    {
        private Minecraft mc;
        private ItemStack itemToRender = null;
        private float equippedProgress = 0.0F;
        private float prevEquippedProgress = 0.0F;
        private RenderBlocks renderBlocksInstance = new RenderBlocks();
        private MapItemRenderer field_28131_f;
        private int field_20099_f = -1;

        public ItemRenderer(Minecraft var1)
        {
            mc = var1;
            field_28131_f = new MapItemRenderer(var1.fontRenderer, var1.gameSettings, var1.renderEngine);
        }

        public void renderItem(EntityLiving var1, ItemStack var2)
        {
            GLManager.GL.PushMatrix();
            if (var2.itemID < 256 && RenderBlocks.renderItemIn3d(Block.BLOCKS[var2.itemID].getRenderType()))
            {
                GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.renderEngine.getTexture("/terrain.png"));
                renderBlocksInstance.renderBlockOnInventory(Block.BLOCKS[var2.itemID], var2.getItemDamage(), var1.getEntityBrightness(1.0F));
            }
            else
            {
                if (var2.itemID < 256)
                {
                    GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.renderEngine.getTexture("/terrain.png"));
                }
                else
                {
                    GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.renderEngine.getTexture("/gui/items.png"));
                }

                Tessellator var3 = Tessellator.instance;
                int var4 = var1.getItemIcon(var2);
                float var5 = ((float)(var4 % 16 * 16) + 0.0F) / 256.0F;
                float var6 = ((float)(var4 % 16 * 16) + 15.99F) / 256.0F;
                float var7 = ((float)(var4 / 16 * 16) + 0.0F) / 256.0F;
                float var8 = ((float)(var4 / 16 * 16) + 15.99F) / 256.0F;
                float var9 = 1.0F;
                float var10 = 0.0F;
                float var11 = 0.3F;
                GLManager.GL.Enable(GLEnum.RescaleNormal);
                GLManager.GL.Translate(-var10, -var11, 0.0F);
                float var12 = 1.5F;
                GLManager.GL.Scale(var12, var12, var12);
                GLManager.GL.Rotate(50.0F, 0.0F, 1.0F, 0.0F);
                GLManager.GL.Rotate(335.0F, 0.0F, 0.0F, 1.0F);
                GLManager.GL.Translate(-(15.0F / 16.0F), -(1.0F / 16.0F), 0.0F);
                float var13 = 1.0F / 16.0F;
                var3.startDrawingQuads();
                var3.setNormal(0.0F, 0.0F, 1.0F);
                var3.addVertexWithUV(0.0D, 0.0D, 0.0D, (double)var6, (double)var8);
                var3.addVertexWithUV((double)var9, 0.0D, 0.0D, (double)var5, (double)var8);
                var3.addVertexWithUV((double)var9, 1.0D, 0.0D, (double)var5, (double)var7);
                var3.addVertexWithUV(0.0D, 1.0D, 0.0D, (double)var6, (double)var7);
                var3.draw();
                var3.startDrawingQuads();
                var3.setNormal(0.0F, 0.0F, -1.0F);
                var3.addVertexWithUV(0.0D, 1.0D, (double)(0.0F - var13), (double)var6, (double)var7);
                var3.addVertexWithUV((double)var9, 1.0D, (double)(0.0F - var13), (double)var5, (double)var7);
                var3.addVertexWithUV((double)var9, 0.0D, (double)(0.0F - var13), (double)var5, (double)var8);
                var3.addVertexWithUV(0.0D, 0.0D, (double)(0.0F - var13), (double)var6, (double)var8);
                var3.draw();
                var3.startDrawingQuads();
                var3.setNormal(-1.0F, 0.0F, 0.0F);

                int var14;
                float var15;
                float var16;
                float var17;
                for (var14 = 0; var14 < 16; ++var14)
                {
                    var15 = (float)var14 / 16.0F;
                    var16 = var6 + (var5 - var6) * var15 - 0.001953125F;
                    var17 = var9 * var15;
                    var3.addVertexWithUV((double)var17, 0.0D, (double)(0.0F - var13), (double)var16, (double)var8);
                    var3.addVertexWithUV((double)var17, 0.0D, 0.0D, (double)var16, (double)var8);
                    var3.addVertexWithUV((double)var17, 1.0D, 0.0D, (double)var16, (double)var7);
                    var3.addVertexWithUV((double)var17, 1.0D, (double)(0.0F - var13), (double)var16, (double)var7);
                }

                var3.draw();
                var3.startDrawingQuads();
                var3.setNormal(1.0F, 0.0F, 0.0F);

                for (var14 = 0; var14 < 16; ++var14)
                {
                    var15 = (float)var14 / 16.0F;
                    var16 = var6 + (var5 - var6) * var15 - 0.001953125F;
                    var17 = var9 * var15 + 1.0F / 16.0F;
                    var3.addVertexWithUV((double)var17, 1.0D, (double)(0.0F - var13), (double)var16, (double)var7);
                    var3.addVertexWithUV((double)var17, 1.0D, 0.0D, (double)var16, (double)var7);
                    var3.addVertexWithUV((double)var17, 0.0D, 0.0D, (double)var16, (double)var8);
                    var3.addVertexWithUV((double)var17, 0.0D, (double)(0.0F - var13), (double)var16, (double)var8);
                }

                var3.draw();
                var3.startDrawingQuads();
                var3.setNormal(0.0F, 1.0F, 0.0F);

                for (var14 = 0; var14 < 16; ++var14)
                {
                    var15 = (float)var14 / 16.0F;
                    var16 = var8 + (var7 - var8) * var15 - 0.001953125F;
                    var17 = var9 * var15 + 1.0F / 16.0F;
                    var3.addVertexWithUV(0.0D, (double)var17, 0.0D, (double)var6, (double)var16);
                    var3.addVertexWithUV((double)var9, (double)var17, 0.0D, (double)var5, (double)var16);
                    var3.addVertexWithUV((double)var9, (double)var17, (double)(0.0F - var13), (double)var5, (double)var16);
                    var3.addVertexWithUV(0.0D, (double)var17, (double)(0.0F - var13), (double)var6, (double)var16);
                }

                var3.draw();
                var3.startDrawingQuads();
                var3.setNormal(0.0F, -1.0F, 0.0F);

                for (var14 = 0; var14 < 16; ++var14)
                {
                    var15 = (float)var14 / 16.0F;
                    var16 = var8 + (var7 - var8) * var15 - 0.001953125F;
                    var17 = var9 * var15;
                    var3.addVertexWithUV((double)var9, (double)var17, 0.0D, (double)var5, (double)var16);
                    var3.addVertexWithUV(0.0D, (double)var17, 0.0D, (double)var6, (double)var16);
                    var3.addVertexWithUV(0.0D, (double)var17, (double)(0.0F - var13), (double)var6, (double)var16);
                    var3.addVertexWithUV((double)var9, (double)var17, (double)(0.0F - var13), (double)var5, (double)var16);
                }

                var3.draw();
                GLManager.GL.Disable(GLEnum.RescaleNormal);
            }

            GLManager.GL.PopMatrix();
        }

        public void renderItemInFirstPerson(float var1)
        {
            float var2 = prevEquippedProgress + (equippedProgress - prevEquippedProgress) * var1;
            EntityPlayerSP var3 = mc.thePlayer;
            float var4 = var3.prevRotationPitch + (var3.rotationPitch - var3.prevRotationPitch) * var1;
            GLManager.GL.PushMatrix();
            GLManager.GL.Rotate(var4, 1.0F, 0.0F, 0.0F);
            GLManager.GL.Rotate(var3.prevRotationYaw + (var3.rotationYaw - var3.prevRotationYaw) * var1, 0.0F, 1.0F, 0.0F);
            RenderHelper.enableStandardItemLighting();
            GLManager.GL.PopMatrix();
            ItemStack var5 = itemToRender;
            float var6 = mc.theWorld.getLuminance(MathHelper.floor_double(var3.posX), MathHelper.floor_double(var3.posY), MathHelper.floor_double(var3.posZ));
            float var8;
            float var9;
            float var10;
            if (var5 != null)
            {
                int var7 = Item.itemsList[var5.itemID].getColorFromDamage(var5.getItemDamage());
                var8 = (float)(var7 >> 16 & 255) / 255.0F;
                var9 = (float)(var7 >> 8 & 255) / 255.0F;
                var10 = (float)(var7 & 255) / 255.0F;
                GLManager.GL.Color4(var6 * var8, var6 * var9, var6 * var10, 1.0F);
            }
            else
            {
                GLManager.GL.Color4(var6, var6, var6, 1.0F);
            }

            float var14;
            if (var5 != null && var5.itemID == Item.mapItem.id)
            {
                GLManager.GL.PushMatrix();
                var14 = 0.8F;
                var8 = var3.getSwingProgress(var1);
                var9 = MathHelper.sin(var8 * (float)Math.PI);
                var10 = MathHelper.sin(MathHelper.sqrt_float(var8) * (float)Math.PI);
                GLManager.GL.Translate(-var10 * 0.4F, MathHelper.sin(MathHelper.sqrt_float(var8) * (float)Math.PI * 2.0F) * 0.2F, -var9 * 0.2F);
                var8 = 1.0F - var4 / 45.0F + 0.1F;
                if (var8 < 0.0F)
                {
                    var8 = 0.0F;
                }

                if (var8 > 1.0F)
                {
                    var8 = 1.0F;
                }

                var8 = -MathHelper.cos(var8 * (float)Math.PI) * 0.5F + 0.5F;
                GLManager.GL.Translate(0.0F, 0.0F * var14 - (1.0F - var2) * 1.2F - var8 * 0.5F + 0.04F, -0.9F * var14);
                GLManager.GL.Rotate(90.0F, 0.0F, 1.0F, 0.0F);
                GLManager.GL.Rotate(var8 * -85.0F, 0.0F, 0.0F, 1.0F);
                GLManager.GL.Enable(GLEnum.RescaleNormal);
                //TODO: ADD SKINS
                GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.renderEngine.getTexture(mc.thePlayer.getEntityTexture()));

                for (int var17 = 0; var17 < 2; ++var17)
                {
                    int var21 = var17 * 2 - 1;
                    GLManager.GL.PushMatrix();
                    GLManager.GL.Translate(-0.0F, -0.6F, 1.1F * (float)var21);
                    GLManager.GL.Rotate((float)(-45 * var21), 1.0F, 0.0F, 0.0F);
                    GLManager.GL.Rotate(-90.0F, 0.0F, 0.0F, 1.0F);
                    GLManager.GL.Rotate(59.0F, 0.0F, 0.0F, 1.0F);
                    GLManager.GL.Rotate((float)(-65 * var21), 0.0F, 1.0F, 0.0F);
                    Render var11 = RenderManager.instance.getEntityRenderObject(mc.thePlayer);
                    RenderPlayer var12 = (RenderPlayer)var11;
                    float var13 = 1.0F;
                    GLManager.GL.Scale(var13, var13, var13);
                    var12.drawFirstPersonHand();
                    GLManager.GL.PopMatrix();
                }

                var9 = var3.getSwingProgress(var1);
                var10 = MathHelper.sin(var9 * var9 * (float)Math.PI);
                float var18 = MathHelper.sin(MathHelper.sqrt_float(var9) * (float)Math.PI);
                GLManager.GL.Rotate(-var10 * 20.0F, 0.0F, 1.0F, 0.0F);
                GLManager.GL.Rotate(-var18 * 20.0F, 0.0F, 0.0F, 1.0F);
                GLManager.GL.Rotate(-var18 * 80.0F, 1.0F, 0.0F, 0.0F);
                var9 = 0.38F;
                GLManager.GL.Scale(var9, var9, var9);
                GLManager.GL.Rotate(90.0F, 0.0F, 1.0F, 0.0F);
                GLManager.GL.Rotate(180.0F, 0.0F, 0.0F, 1.0F);
                GLManager.GL.Translate(-1.0F, -1.0F, 0.0F);
                var10 = 0.015625F;
                GLManager.GL.Scale(var10, var10, var10);
                mc.renderEngine.bindTexture(mc.renderEngine.getTexture("/misc/mapbg.png"));
                Tessellator var19 = Tessellator.instance;
                GLManager.GL.Normal3(0.0F, 0.0F, -1.0F);
                var19.startDrawingQuads();
                byte var20 = 7;
                var19.addVertexWithUV((double)(0 - var20), (double)(128 + var20), 0.0D, 0.0D, 1.0D);
                var19.addVertexWithUV((double)(128 + var20), (double)(128 + var20), 0.0D, 1.0D, 1.0D);
                var19.addVertexWithUV((double)(128 + var20), (double)(0 - var20), 0.0D, 1.0D, 0.0D);
                var19.addVertexWithUV((double)(0 - var20), (double)(0 - var20), 0.0D, 0.0D, 0.0D);
                var19.draw();
                MapData var22 = Item.mapItem.func_28012_a(var5, mc.theWorld);
                field_28131_f.func_28157_a(mc.thePlayer, mc.renderEngine, var22);
                GLManager.GL.PopMatrix();
            }
            else if (var5 != null)
            {
                GLManager.GL.PushMatrix();
                var14 = 0.8F;
                var8 = var3.getSwingProgress(var1);
                var9 = MathHelper.sin(var8 * (float)Math.PI);
                var10 = MathHelper.sin(MathHelper.sqrt_float(var8) * (float)Math.PI);
                GLManager.GL.Translate(-var10 * 0.4F, MathHelper.sin(MathHelper.sqrt_float(var8) * (float)Math.PI * 2.0F) * 0.2F, -var9 * 0.2F);
                GLManager.GL.Translate(0.7F * var14, -0.65F * var14 - (1.0F - var2) * 0.6F, -0.9F * var14);
                GLManager.GL.Rotate(45.0F, 0.0F, 1.0F, 0.0F);
                GLManager.GL.Enable(GLEnum.RescaleNormal);
                var8 = var3.getSwingProgress(var1);
                var9 = MathHelper.sin(var8 * var8 * (float)Math.PI);
                var10 = MathHelper.sin(MathHelper.sqrt_float(var8) * (float)Math.PI);
                GLManager.GL.Rotate(-var9 * 20.0F, 0.0F, 1.0F, 0.0F);
                GLManager.GL.Rotate(-var10 * 20.0F, 0.0F, 0.0F, 1.0F);
                GLManager.GL.Rotate(-var10 * 80.0F, 1.0F, 0.0F, 0.0F);
                var8 = 0.4F;
                GLManager.GL.Scale(var8, var8, var8);
                if (var5.getItem().shouldRotateAroundWhenRendering())
                {
                    GLManager.GL.Rotate(180.0F, 0.0F, 1.0F, 0.0F);
                }

                renderItem(var3, var5);
                GLManager.GL.PopMatrix();
            }
            else
            {
                GLManager.GL.PushMatrix();
                var14 = 0.8F;
                var8 = var3.getSwingProgress(var1);
                var9 = MathHelper.sin(var8 * (float)Math.PI);
                var10 = MathHelper.sin(MathHelper.sqrt_float(var8) * (float)Math.PI);
                GLManager.GL.Translate(-var10 * 0.3F, MathHelper.sin(MathHelper.sqrt_float(var8) * (float)Math.PI * 2.0F) * 0.4F, -var9 * 0.4F);
                GLManager.GL.Translate(0.8F * var14, -(12.0F / 16.0F) * var14 - (1.0F - var2) * 0.6F, -0.9F * var14);
                GLManager.GL.Rotate(45.0F, 0.0F, 1.0F, 0.0F);
                GLManager.GL.Enable(GLEnum.RescaleNormal);
                var8 = var3.getSwingProgress(var1);
                var9 = MathHelper.sin(var8 * var8 * (float)Math.PI);
                var10 = MathHelper.sin(MathHelper.sqrt_float(var8) * (float)Math.PI);
                GLManager.GL.Rotate(var10 * 70.0F, 0.0F, 1.0F, 0.0F);
                GLManager.GL.Rotate(-var9 * 20.0F, 0.0F, 0.0F, 1.0F);
                //TODO: ADD SKIN
                GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.renderEngine.getTexture(mc.thePlayer.getEntityTexture()));
                GLManager.GL.Translate(-1.0F, 3.6F, 3.5F);
                GLManager.GL.Rotate(120.0F, 0.0F, 0.0F, 1.0F);
                GLManager.GL.Rotate(200.0F, 1.0F, 0.0F, 0.0F);
                GLManager.GL.Rotate(-135.0F, 0.0F, 1.0F, 0.0F);
                GLManager.GL.Scale(1.0F, 1.0F, 1.0F);
                GLManager.GL.Translate(5.6F, 0.0F, 0.0F);
                Render var15 = RenderManager.instance.getEntityRenderObject(mc.thePlayer);
                RenderPlayer var16 = (RenderPlayer)var15;
                var10 = 1.0F;
                GLManager.GL.Scale(var10, var10, var10);
                var16.drawFirstPersonHand();
                GLManager.GL.PopMatrix();
            }

            GLManager.GL.Disable(GLEnum.RescaleNormal);
            RenderHelper.disableStandardItemLighting();
        }

        public void renderOverlays(float var1)
        {
            GLManager.GL.Disable(GLEnum.AlphaTest);
            int var2;
            if (mc.thePlayer.isBurning())
            {
                var2 = mc.renderEngine.getTexture("/terrain.png");
                GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)var2);
                renderFireInFirstPerson(var1);
            }

            if (mc.thePlayer.isEntityInsideOpaqueBlock())
            {
                var2 = MathHelper.floor_double(mc.thePlayer.posX);
                int var3 = MathHelper.floor_double(mc.thePlayer.posY);
                int var4 = MathHelper.floor_double(mc.thePlayer.posZ);
                int var5 = mc.renderEngine.getTexture("/terrain.png");
                GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)var5);
                int var6 = mc.theWorld.getBlockId(var2, var3, var4);
                if (mc.theWorld.shouldSuffocate(var2, var3, var4))
                {
                    renderInsideOfBlock(var1, Block.BLOCKS[var6].getTexture(2));
                }
                else
                {
                    for (int var7 = 0; var7 < 8; ++var7)
                    {
                        float var8 = ((float)((var7 >> 0) % 2) - 0.5F) * mc.thePlayer.width * 0.9F;
                        float var9 = ((float)((var7 >> 1) % 2) - 0.5F) * mc.thePlayer.height * 0.2F;
                        float var10 = ((float)((var7 >> 2) % 2) - 0.5F) * mc.thePlayer.width * 0.9F;
                        int var11 = MathHelper.floor_float((float)var2 + var8);
                        int var12 = MathHelper.floor_float((float)var3 + var9);
                        int var13 = MathHelper.floor_float((float)var4 + var10);
                        if (mc.theWorld.shouldSuffocate(var11, var12, var13))
                        {
                            var6 = mc.theWorld.getBlockId(var11, var12, var13);
                        }
                    }
                }

                if (Block.BLOCKS[var6] != null)
                {
                    renderInsideOfBlock(var1, Block.BLOCKS[var6].getTexture(2));
                }
            }

            if (mc.thePlayer.isInsideOfMaterial(Material.WATER))
            {
                var2 = mc.renderEngine.getTexture("/misc/water.png");
                GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)var2);
                renderWarpedTextureOverlay(var1);
            }

            GLManager.GL.Enable(GLEnum.AlphaTest);
        }

        private void renderInsideOfBlock(float var1, int var2)
        {
            Tessellator var3 = Tessellator.instance;
            mc.thePlayer.getEntityBrightness(var1);
            float var4 = 0.1F;
            GLManager.GL.Color4(var4, var4, var4, 0.5F);
            GLManager.GL.PushMatrix();
            float var5 = -1.0F;
            float var6 = 1.0F;
            float var7 = -1.0F;
            float var8 = 1.0F;
            float var9 = -0.5F;
            float var10 = 0.0078125F;
            float var11 = (float)(var2 % 16) / 256.0F - var10;
            float var12 = ((float)(var2 % 16) + 15.99F) / 256.0F + var10;
            float var13 = (float)(var2 / 16) / 256.0F - var10;
            float var14 = ((float)(var2 / 16) + 15.99F) / 256.0F + var10;
            var3.startDrawingQuads();
            var3.addVertexWithUV((double)var5, (double)var7, (double)var9, (double)var12, (double)var14);
            var3.addVertexWithUV((double)var6, (double)var7, (double)var9, (double)var11, (double)var14);
            var3.addVertexWithUV((double)var6, (double)var8, (double)var9, (double)var11, (double)var13);
            var3.addVertexWithUV((double)var5, (double)var8, (double)var9, (double)var12, (double)var13);
            var3.draw();
            GLManager.GL.PopMatrix();
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        }

        private void renderWarpedTextureOverlay(float var1)
        {
            Tessellator var2 = Tessellator.instance;
            float var3 = mc.thePlayer.getEntityBrightness(var1);
            GLManager.GL.Color4(var3, var3, var3, 0.5F);
            GLManager.GL.Enable(GLEnum.Blend);
            GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
            GLManager.GL.PushMatrix();
            float var4 = 4.0F;
            float var5 = -1.0F;
            float var6 = 1.0F;
            float var7 = -1.0F;
            float var8 = 1.0F;
            float var9 = -0.5F;
            float var10 = -mc.thePlayer.rotationYaw / 64.0F;
            float var11 = mc.thePlayer.rotationPitch / 64.0F;
            var2.startDrawingQuads();
            var2.addVertexWithUV((double)var5, (double)var7, (double)var9, (double)(var4 + var10), (double)(var4 + var11));
            var2.addVertexWithUV((double)var6, (double)var7, (double)var9, (double)(0.0F + var10), (double)(var4 + var11));
            var2.addVertexWithUV((double)var6, (double)var8, (double)var9, (double)(0.0F + var10), (double)(0.0F + var11));
            var2.addVertexWithUV((double)var5, (double)var8, (double)var9, (double)(var4 + var10), (double)(0.0F + var11));
            var2.draw();
            GLManager.GL.PopMatrix();
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            GLManager.GL.Disable(GLEnum.Blend);
        }

        private void renderFireInFirstPerson(float var1)
        {
            Tessellator var2 = Tessellator.instance;
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 0.9F);
            GLManager.GL.Enable(GLEnum.Blend);
            GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
            float var3 = 1.0F;

            for (int var4 = 0; var4 < 2; ++var4)
            {
                GLManager.GL.PushMatrix();
                int var5 = Block.FIRE.textureId + var4 * 16;
                int var6 = (var5 & 15) << 4;
                int var7 = var5 & 240;
                float var8 = (float)var6 / 256.0F;
                float var9 = ((float)var6 + 15.99F) / 256.0F;
                float var10 = (float)var7 / 256.0F;
                float var11 = ((float)var7 + 15.99F) / 256.0F;
                float var12 = (0.0F - var3) / 2.0F;
                float var13 = var12 + var3;
                float var14 = 0.0F - var3 / 2.0F;
                float var15 = var14 + var3;
                float var16 = -0.5F;
                GLManager.GL.Translate((float)(-(var4 * 2 - 1)) * 0.24F, -0.3F, 0.0F);
                GLManager.GL.Rotate((float)(var4 * 2 - 1) * 10.0F, 0.0F, 1.0F, 0.0F);
                var2.startDrawingQuads();
                var2.addVertexWithUV((double)var12, (double)var14, (double)var16, (double)var9, (double)var11);
                var2.addVertexWithUV((double)var13, (double)var14, (double)var16, (double)var8, (double)var11);
                var2.addVertexWithUV((double)var13, (double)var15, (double)var16, (double)var8, (double)var10);
                var2.addVertexWithUV((double)var12, (double)var15, (double)var16, (double)var9, (double)var10);
                var2.draw();
                GLManager.GL.PopMatrix();
            }

            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            GLManager.GL.Disable(GLEnum.Blend);
        }

        public void updateEquippedItem()
        {
            prevEquippedProgress = equippedProgress;
            EntityPlayerSP var1 = mc.thePlayer;
            ItemStack var2 = var1.inventory.getCurrentItem();
            bool var4 = field_20099_f == var1.inventory.currentItem && var2 == itemToRender;
            if (itemToRender == null && var2 == null)
            {
                var4 = true;
            }

            if (var2 != null && itemToRender != null && var2 != itemToRender && var2.itemID == itemToRender.itemID && var2.getItemDamage() == itemToRender.getItemDamage())
            {
                itemToRender = var2;
                var4 = true;
            }

            float var5 = 0.4F;
            float var6 = var4 ? 1.0F : 0.0F;
            float var7 = var6 - equippedProgress;
            if (var7 < -var5)
            {
                var7 = -var5;
            }

            if (var7 > var5)
            {
                var7 = var5;
            }

            equippedProgress += var7;
            if (equippedProgress < 0.1F)
            {
                itemToRender = var2;
                field_20099_f = var1.inventory.currentItem;
            }

        }

        public void func_9449_b()
        {
            equippedProgress = 0.0F;
        }

        public void func_9450_c()
        {
            equippedProgress = 0.0F;
        }
    }

}