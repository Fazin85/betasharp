using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;
using BetaSharp.Client.Entities;
using BetaSharp.Client.Rendering.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Entities;
using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Util.Maths;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Items;

public class HeldItemRenderer
{
    private readonly Minecraft mc;
    private ItemStack itemToRender;
    private float equippedProgress;
    private float prevEquippedProgress;
    private readonly BlockRenderer renderBlocksInstance = new();
    private readonly MapItemRenderer field_28131_f;
    private int field_20099_f = -1;

    public HeldItemRenderer(Minecraft var1)
    {
        mc = var1;
        field_28131_f = new MapItemRenderer(var1.fontRenderer, var1.options, var1.textureManager);
    }

    public void renderItem(EntityLiving var1, ItemStack var2)
    {
        GLManager.GL.PushMatrix();
        if (var2.itemId < 256 && BlockRenderer.isSideLit(Block.Blocks[var2.itemId].getRenderType()))
        {
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.GetTextureId("/terrain.png"));
            renderBlocksInstance.renderBlockOnInventory(Block.Blocks[var2.itemId], var2.getDamage(), var1.getBrightnessAtEyes(1.0F));
        }
        else
        {
            if (var2.itemId < 256)
            {
                GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.GetTextureId("/terrain.png"));
            }
            else
            {
                GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.GetTextureId("/gui/items.png"));
            }

            Tessellator var3 = Tessellator.instance;
            int var4 = var1.getItemStackTextureId(var2);
            float var5 = (var4 % 16 * 16 + 0.0F) / 256.0F;
            float var6 = (var4 % 16 * 16 + 15.99F) / 256.0F;
            float var7 = (var4 / 16 * 16 + 0.0F) / 256.0F;
            float var8 = (var4 / 16 * 16 + 15.99F) / 256.0F;
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
                var15 = var14 / 16.0F;
                var16 = var6 + (var5 - var6) * var15 - (1 / 512f);
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
                var15 = var14 / 16.0F;
                var16 = var6 + (var5 - var6) * var15 - (1 / 512f);
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
                var15 = var14 / 16.0F;
                var16 = var8 + (var7 - var8) * var15 - (1 / 512f);
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
                var15 = var14 / 16.0F;
                var16 = var8 + (var7 - var8) * var15 - (1 / 512f);
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
        ClientPlayerEntity var3 = mc.player;
        float var4 = var3.prevPitch + (var3.pitch - var3.prevPitch) * var1;
        GLManager.GL.PushMatrix();
        GLManager.GL.Rotate(var4, 1.0F, 0.0F, 0.0F);
        GLManager.GL.Rotate(var3.prevYaw + (var3.yaw - var3.prevYaw) * var1, 0.0F, 1.0F, 0.0F);
        Lighting.turnOn();
        GLManager.GL.PopMatrix();
        ItemStack var5 = itemToRender;
        float var6 = mc.world.getLuminance(MathHelper.Floor(var3.x), MathHelper.Floor(var3.y), MathHelper.Floor(var3.z));
        float var8;
        float var9;
        float var10;
        if (var5 != null)
        {
            int var7 = Item.ITEMS[var5.itemId].getColorMultiplier(var5.getDamage());
            var8 = (var7 >> 16 & 255) / 255.0F;
            var9 = (var7 >> 8 & 255) / 255.0F;
            var10 = (var7 & 255) / 255.0F;
            GLManager.GL.Color4(var6 * var8, var6 * var9, var6 * var10, 1.0F);
        }
        else
        {
            GLManager.GL.Color4(var6, var6, var6, 1.0F);
        }

        float var14;
        if (var5 != null && var5.itemId == Item.Map.id)
        {
            GLManager.GL.PushMatrix();
            var14 = 0.8F;
            var8 = var3.getSwingProgress(var1);
            var9 = MathHelper.Sin(var8 * (float)Math.PI);
            var10 = MathHelper.Sin(MathHelper.Sqrt(var8) * (float)Math.PI);
            GLManager.GL.Translate(-var10 * 0.4F, MathHelper.Sin(MathHelper.Sqrt(var8) * (float)Math.PI * 2.0F) * 0.2F, -var9 * 0.2F);
            var8 = 1.0F - var4 / 45.0F + 0.1F;
            if (var8 < 0.0F)
            {
                var8 = 0.0F;
            }

            if (var8 > 1.0F)
            {
                var8 = 1.0F;
            }

            var8 = -MathHelper.Cos(var8 * (float)Math.PI) * 0.5F + 0.5F;
            GLManager.GL.Translate(0.0F, 0.0F * var14 - (1.0F - var2) * 1.2F - var8 * 0.5F + 0.04F, -0.9F * var14);
            GLManager.GL.Rotate(90.0F, 0.0F, 1.0F, 0.0F);
            GLManager.GL.Rotate(var8 * -85.0F, 0.0F, 0.0F, 1.0F);
            GLManager.GL.Enable(GLEnum.RescaleNormal);
            //TODO: ADD SKINS
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.GetTextureId(mc.player.getTexture()));

            for (int var17 = 0; var17 < 2; ++var17)
            {
                int var21 = var17 * 2 - 1;
                GLManager.GL.PushMatrix();
                GLManager.GL.Translate(-0.0F, -0.6F, 1.1F * var21);
                GLManager.GL.Rotate(-45 * var21, 1.0F, 0.0F, 0.0F);
                GLManager.GL.Rotate(-90.0F, 0.0F, 0.0F, 1.0F);
                GLManager.GL.Rotate(59.0F, 0.0F, 0.0F, 1.0F);
                GLManager.GL.Rotate(-65 * var21, 0.0F, 1.0F, 0.0F);
                EntityRenderer var11 = EntityRenderDispatcher.instance.getEntityRenderObject(mc.player);
                PlayerEntityRenderer var12 = (PlayerEntityRenderer)var11;
                float var13 = 1.0F;
                GLManager.GL.Scale(var13, var13, var13);
                var12.drawFirstPersonHand();
                GLManager.GL.PopMatrix();
            }

            var9 = var3.getSwingProgress(var1);
            var10 = MathHelper.Sin(var9 * var9 * (float)Math.PI);
            float var18 = MathHelper.Sin(MathHelper.Sqrt(var9) * (float)Math.PI);
            GLManager.GL.Rotate(-var10 * 20.0F, 0.0F, 1.0F, 0.0F);
            GLManager.GL.Rotate(-var18 * 20.0F, 0.0F, 0.0F, 1.0F);
            GLManager.GL.Rotate(-var18 * 80.0F, 1.0F, 0.0F, 0.0F);
            var9 = 0.38F;
            GLManager.GL.Scale(var9, var9, var9);
            GLManager.GL.Rotate(90.0F, 0.0F, 1.0F, 0.0F);
            GLManager.GL.Rotate(180.0F, 0.0F, 0.0F, 1.0F);
            GLManager.GL.Translate(-1.0F, -1.0F, 0.0F);
            var10 = (1 / 64f);
            GLManager.GL.Scale(var10, var10, var10);
            mc.textureManager.BindTexture(mc.textureManager.GetTextureId("/misc/mapbg.png"));
            Tessellator var19 = Tessellator.instance;
            GLManager.GL.Normal3(0.0F, 0.0F, -1.0F);
            var19.startDrawingQuads();
            byte var20 = 7;
            var19.addVertexWithUV(0 - var20, 128 + var20, 0.0D, 0.0D, 1.0D);
            var19.addVertexWithUV(128 + var20, 128 + var20, 0.0D, 1.0D, 1.0D);
            var19.addVertexWithUV(128 + var20, 0 - var20, 0.0D, 1.0D, 0.0D);
            var19.addVertexWithUV(0 - var20, 0 - var20, 0.0D, 0.0D, 0.0D);
            var19.draw();
            MapState var22 = Item.Map.getSavedMapState(var5, mc.world);
            field_28131_f.render(mc.player, mc.textureManager, var22);
            GLManager.GL.PopMatrix();
        }
        else if (var5 != null)
        {
            GLManager.GL.PushMatrix();
            var14 = 0.8F;
            var8 = var3.getSwingProgress(var1);
            var9 = MathHelper.Sin(var8 * (float)Math.PI);
            var10 = MathHelper.Sin(MathHelper.Sqrt(var8) * (float)Math.PI);
            GLManager.GL.Translate(-var10 * 0.4F, MathHelper.Sin(MathHelper.Sqrt(var8) * (float)Math.PI * 2.0F) * 0.2F, -var9 * 0.2F);
            GLManager.GL.Translate(0.7F * var14, -0.65F * var14 - (1.0F - var2) * 0.6F, -0.9F * var14);
            GLManager.GL.Rotate(45.0F, 0.0F, 1.0F, 0.0F);
            GLManager.GL.Enable(GLEnum.RescaleNormal);
            var8 = var3.getSwingProgress(var1);
            var9 = MathHelper.Sin(var8 * var8 * (float)Math.PI);
            var10 = MathHelper.Sin(MathHelper.Sqrt(var8) * (float)Math.PI);
            GLManager.GL.Rotate(-var9 * 20.0F, 0.0F, 1.0F, 0.0F);
            GLManager.GL.Rotate(-var10 * 20.0F, 0.0F, 0.0F, 1.0F);
            GLManager.GL.Rotate(-var10 * 80.0F, 1.0F, 0.0F, 0.0F);
            var8 = 0.4F;
            GLManager.GL.Scale(var8, var8, var8);
            if (var5.getItem().isHandheldRod())
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
            var9 = MathHelper.Sin(var8 * (float)Math.PI);
            var10 = MathHelper.Sin(MathHelper.Sqrt(var8) * (float)Math.PI);
            GLManager.GL.Translate(-var10 * 0.3F, MathHelper.Sin(MathHelper.Sqrt(var8) * (float)Math.PI * 2.0F) * 0.4F, -var9 * 0.4F);
            GLManager.GL.Translate(0.8F * var14, -(12.0F / 16.0F) * var14 - (1.0F - var2) * 0.6F, -0.9F * var14);
            GLManager.GL.Rotate(45.0F, 0.0F, 1.0F, 0.0F);
            GLManager.GL.Enable(GLEnum.RescaleNormal);
            var8 = var3.getSwingProgress(var1);
            var9 = MathHelper.Sin(var8 * var8 * (float)Math.PI);
            var10 = MathHelper.Sin(MathHelper.Sqrt(var8) * (float)Math.PI);
            GLManager.GL.Rotate(var10 * 70.0F, 0.0F, 1.0F, 0.0F);
            GLManager.GL.Rotate(-var9 * 20.0F, 0.0F, 0.0F, 1.0F);
            //TODO: ADD SKIN
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.GetTextureId(mc.player.getTexture()));
            GLManager.GL.Translate(-1.0F, 3.6F, 3.5F);
            GLManager.GL.Rotate(120.0F, 0.0F, 0.0F, 1.0F);
            GLManager.GL.Rotate(200.0F, 1.0F, 0.0F, 0.0F);
            GLManager.GL.Rotate(-135.0F, 0.0F, 1.0F, 0.0F);
            GLManager.GL.Scale(1.0F, 1.0F, 1.0F);
            GLManager.GL.Translate(5.6F, 0.0F, 0.0F);
            EntityRenderer var15 = EntityRenderDispatcher.instance.getEntityRenderObject(mc.player);
            PlayerEntityRenderer var16 = (PlayerEntityRenderer)var15;
            var10 = 1.0F;
            GLManager.GL.Scale(var10, var10, var10);
            var16.drawFirstPersonHand();
            GLManager.GL.PopMatrix();
        }

        GLManager.GL.Disable(GLEnum.RescaleNormal);
        Lighting.turnOff();
    }

    public void renderOverlays(float var1)
    {
        GLManager.GL.Disable(GLEnum.AlphaTest);
        int var2;
        if (mc.player.isOnFire())
        {
            var2 = mc.textureManager.GetTextureId("/terrain.png");
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)var2);
            renderFireInFirstPerson(var1);
        }

        if (mc.player.isInsideWall())
        {
            var2 = MathHelper.Floor(mc.player.x);
            int var3 = MathHelper.Floor(mc.player.y);
            int var4 = MathHelper.Floor(mc.player.z);
            int var5 = mc.textureManager.GetTextureId("/terrain.png");
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)var5);
            int var6 = mc.world.getBlockId(var2, var3, var4);
            if (mc.world.shouldSuffocate(var2, var3, var4))
            {
                renderInsideOfBlock(var1, Block.Blocks[var6].getTexture(2));
            }
            else
            {
                for (int var7 = 0; var7 < 8; ++var7)
                {
                    float var8 = ((var7 >> 0) % 2 - 0.5F) * mc.player.width * 0.9F;
                    float var9 = ((var7 >> 1) % 2 - 0.5F) * mc.player.height * 0.2F;
                    float var10 = ((var7 >> 2) % 2 - 0.5F) * mc.player.width * 0.9F;
                    int var11 = MathHelper.Floor(var2 + var8);
                    int var12 = MathHelper.Floor(var3 + var9);
                    int var13 = MathHelper.Floor(var4 + var10);
                    if (mc.world.shouldSuffocate(var11, var12, var13))
                    {
                        var6 = mc.world.getBlockId(var11, var12, var13);
                    }
                }
            }

            if (Block.Blocks[var6] != null)
            {
                renderInsideOfBlock(var1, Block.Blocks[var6].getTexture(2));
            }
        }

        if (mc.player.isInFluid(Material.Water))
        {
            var2 = mc.textureManager.GetTextureId("/misc/water.png");
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)var2);
            renderWarpedTextureOverlay(var1);
        }

        GLManager.GL.Enable(GLEnum.AlphaTest);
    }

    private void renderInsideOfBlock(float var1, int var2)
    {
        Tessellator var3 = Tessellator.instance;
        mc.player.getBrightnessAtEyes(var1);
        float var4 = 0.1F;
        GLManager.GL.Color4(var4, var4, var4, 0.5F);
        GLManager.GL.PushMatrix();
        float var5 = -1.0F;
        float var6 = 1.0F;
        float var7 = -1.0F;
        float var8 = 1.0F;
        float var9 = -0.5F;
        float var10 = (1 / 128f);
        float var11 = var2 % 16 / 256.0F - var10;
        float var12 = (var2 % 16 + 15.99F) / 256.0F + var10;
        float var13 = var2 / 16 / 256.0F - var10;
        float var14 = (var2 / 16 + 15.99F) / 256.0F + var10;
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
        float var3 = mc.player.getBrightnessAtEyes(var1);
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
        float var10 = -mc.player.yaw / 64.0F;
        float var11 = mc.player.pitch / 64.0F;
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
            int var5 = Block.Fire.textureId + var4 * 16;
            int var6 = (var5 & 15) << 4;
            int var7 = var5 & 240;
            float var8 = var6 / 256.0F;
            float var9 = (var6 + 15.99F) / 256.0F;
            float var10 = var7 / 256.0F;
            float var11 = (var7 + 15.99F) / 256.0F;
            float var12 = (0.0F - var3) / 2.0F;
            float var13 = var12 + var3;
            float var14 = 0.0F - var3 / 2.0F;
            float var15 = var14 + var3;
            float var16 = -0.5F;
            GLManager.GL.Translate(-(var4 * 2 - 1) * 0.24F, -0.3F, 0.0F);
            GLManager.GL.Rotate((var4 * 2 - 1) * 10.0F, 0.0F, 1.0F, 0.0F);
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
        ClientPlayerEntity var1 = mc.player;
        ItemStack var2 = var1.inventory.getSelectedItem();
        bool var4 = field_20099_f == var1.inventory.selectedSlot && var2 == itemToRender;
        if (itemToRender == null && var2 == null)
        {
            var4 = true;
        }

        if (var2 != null && itemToRender != null && var2 != itemToRender && var2.itemId == itemToRender.itemId && var2.getDamage() == itemToRender.getDamage())
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
            field_20099_f = var1.inventory.selectedSlot;
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