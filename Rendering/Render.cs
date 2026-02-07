using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Models;
using betareborn.Worlds;
using Silk.NET.OpenGL.Legacy;

namespace betareborn.Rendering
{
    public abstract class Render
    {
        protected RenderManager renderManager;
        private ModelBase modelBase = new ModelBiped();
        private RenderBlocks renderBlocks = new RenderBlocks();
        protected float shadowSize = 0.0F;
        protected float field_194_c = 1.0F;

        public abstract void doRender(Entity var1, double var2, double var4, double var6, float var8, float var9);

        protected void loadTexture(string var1)
        {
            RenderEngine var2 = renderManager.renderEngine;
            var2.bindTexture(var2.getTexture(var1));
        }

        protected bool loadDownloadableImageTexture(string var1, string var2)
        {
            //RenderEngine var3 = renderManager.renderEngine;
            if (var2 == null)
            {
                return false;
            }

            loadTexture(var2);
            return true;
            //TODO: CUSTOM SKINS
            //int var4 = var3.getTextureForDownloadableImage(var1, var2);
            //int var4 = var3.getTexture(var1);
            //if (var4 >= 0)
            //{
            //    var3.bindTexture(var4);
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return false;
        }

        private void renderEntityOnFire(Entity var1, double var2, double var4, double var6, float var8)
        {
            GLManager.GL.Disable(GLEnum.Lighting);
            int var9 = Block.FIRE.textureId;
            int var10 = (var9 & 15) << 4;
            int var11 = var9 & 240;
            float var12 = (float)var10 / 256.0F;
            float var13 = ((float)var10 + 15.99F) / 256.0F;
            float var14 = (float)var11 / 256.0F;
            float var15 = ((float)var11 + 15.99F) / 256.0F;
            GLManager.GL.PushMatrix();
            GLManager.GL.Translate((float)var2, (float)var4, (float)var6);
            float var16 = var1.width * 1.4F;
            GLManager.GL.Scale(var16, var16, var16);
            loadTexture("/terrain.png");
            Tessellator var17 = Tessellator.instance;
            float var18 = 0.5F;
            float var19 = 0.0F;
            float var20 = var1.height / var16;
            float var21 = (float)(var1.posY - var1.boundingBox.minY);
            GLManager.GL.Rotate(-renderManager.playerViewY, 0.0F, 1.0F, 0.0F);
            GLManager.GL.Translate(0.0F, 0.0F, -0.3F + (float)((int)var20) * 0.02F);
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            float var22 = 0.0F;
            int var23 = 0;
            var17.startDrawingQuads();

            while (var20 > 0.0F)
            {
                if (var23 % 2 == 0)
                {
                    var12 = (float)var10 / 256.0F;
                    var13 = ((float)var10 + 15.99F) / 256.0F;
                    var14 = (float)var11 / 256.0F;
                    var15 = ((float)var11 + 15.99F) / 256.0F;
                }
                else
                {
                    var12 = (float)var10 / 256.0F;
                    var13 = ((float)var10 + 15.99F) / 256.0F;
                    var14 = (float)(var11 + 16) / 256.0F;
                    var15 = ((float)(var11 + 16) + 15.99F) / 256.0F;
                }

                if (var23 / 2 % 2 == 0)
                {
                    float var24 = var13;
                    var13 = var12;
                    var12 = var24;
                }

                var17.addVertexWithUV((double)(var18 - var19), (double)(0.0F - var21), (double)var22, (double)var13, (double)var15);
                var17.addVertexWithUV((double)(-var18 - var19), (double)(0.0F - var21), (double)var22, (double)var12, (double)var15);
                var17.addVertexWithUV((double)(-var18 - var19), (double)(1.4F - var21), (double)var22, (double)var12, (double)var14);
                var17.addVertexWithUV((double)(var18 - var19), (double)(1.4F - var21), (double)var22, (double)var13, (double)var14);
                var20 -= 0.45F;
                var21 -= 0.45F;
                var18 *= 0.9F;
                var22 += 0.03F;
                ++var23;
            }

            var17.draw();
            GLManager.GL.PopMatrix();
            GLManager.GL.Enable(GLEnum.Lighting);
        }

        private void renderShadow(Entity var1, double var2, double var4, double var6, float var8, float var9)
        {
            GLManager.GL.Enable(GLEnum.Blend);
            GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
            RenderEngine var10 = renderManager.renderEngine;
            var10.bindTexture(var10.getTexture("%clamp%/misc/shadow.png"));
            World var11 = getWorldFromRenderManager();
            GLManager.GL.DepthMask(false);
            float var12 = shadowSize;
            double var13 = var1.lastTickPosX + (var1.posX - var1.lastTickPosX) * (double)var9;
            double var15 = var1.lastTickPosY + (var1.posY - var1.lastTickPosY) * (double)var9 + (double)var1.getShadowSize();
            double var17 = var1.lastTickPosZ + (var1.posZ - var1.lastTickPosZ) * (double)var9;
            int var19 = MathHelper.floor_double(var13 - (double)var12);
            int var20 = MathHelper.floor_double(var13 + (double)var12);
            int var21 = MathHelper.floor_double(var15 - (double)var12);
            int var22 = MathHelper.floor_double(var15);
            int var23 = MathHelper.floor_double(var17 - (double)var12);
            int var24 = MathHelper.floor_double(var17 + (double)var12);
            double var25 = var2 - var13;
            double var27 = var4 - var15;
            double var29 = var6 - var17;
            Tessellator var31 = Tessellator.instance;
            var31.startDrawingQuads();

            for (int var32 = var19; var32 <= var20; ++var32)
            {
                for (int var33 = var21; var33 <= var22; ++var33)
                {
                    for (int var34 = var23; var34 <= var24; ++var34)
                    {
                        int var35 = var11.getBlockId(var32, var33 - 1, var34);
                        if (var35 > 0 && var11.getBlockLightValue(var32, var33, var34) > 3)
                        {
                            renderShadowOnBlock(Block.BLOCKS[var35], var2, var4 + (double)var1.getShadowSize(), var6, var32, var33, var34, var8, var12, var25, var27 + (double)var1.getShadowSize(), var29);
                        }
                    }
                }
            }

            var31.draw();
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            GLManager.GL.Disable(GLEnum.Blend);
            GLManager.GL.DepthMask(true);
        }

        private World getWorldFromRenderManager()
        {
            return renderManager.worldObj;
        }

        private void renderShadowOnBlock(Block var1, double var2, double var4, double var6, int var8, int var9, int var10, float var11, float var12, double var13, double var15, double var17)
        {
            Tessellator var19 = Tessellator.instance;
            if (var1.isFullCube())
            {
                double var20 = ((double)var11 - (var4 - ((double)var9 + var15)) / 2.0D) * 0.5D * (double)getWorldFromRenderManager().getLuminance(var8, var9, var10);
                if (var20 >= 0.0D)
                {
                    if (var20 > 1.0D)
                    {
                        var20 = 1.0D;
                    }

                    var19.setColorRGBA_F(1.0F, 1.0F, 1.0F, (float)var20);
                    double var22 = (double)var8 + var1.minX + var13;
                    double var24 = (double)var8 + var1.maxX + var13;
                    double var26 = (double)var9 + var1.minY + var15 + 1.0D / 64.0D;
                    double var28 = (double)var10 + var1.minZ + var17;
                    double var30 = (double)var10 + var1.maxZ + var17;
                    float var32 = (float)((var2 - var22) / 2.0D / (double)var12 + 0.5D);
                    float var33 = (float)((var2 - var24) / 2.0D / (double)var12 + 0.5D);
                    float var34 = (float)((var6 - var28) / 2.0D / (double)var12 + 0.5D);
                    float var35 = (float)((var6 - var30) / 2.0D / (double)var12 + 0.5D);
                    var19.addVertexWithUV(var22, var26, var28, (double)var32, (double)var34);
                    var19.addVertexWithUV(var22, var26, var30, (double)var32, (double)var35);
                    var19.addVertexWithUV(var24, var26, var30, (double)var33, (double)var35);
                    var19.addVertexWithUV(var24, var26, var28, (double)var33, (double)var34);
                }
            }
        }

        public static void renderOffsetAABB(Box var0, double var1, double var3, double var5)
        {
            GLManager.GL.Disable(GLEnum.Texture2D);
            Tessellator var7 = Tessellator.instance;
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            var7.startDrawingQuads();
            var7.setTranslationD(var1, var3, var5);
            var7.setNormal(0.0F, 0.0F, -1.0F);
            var7.addVertex(var0.minX, var0.maxY, var0.minZ);
            var7.addVertex(var0.maxX, var0.maxY, var0.minZ);
            var7.addVertex(var0.maxX, var0.minY, var0.minZ);
            var7.addVertex(var0.minX, var0.minY, var0.minZ);
            var7.setNormal(0.0F, 0.0F, 1.0F);
            var7.addVertex(var0.minX, var0.minY, var0.maxZ);
            var7.addVertex(var0.maxX, var0.minY, var0.maxZ);
            var7.addVertex(var0.maxX, var0.maxY, var0.maxZ);
            var7.addVertex(var0.minX, var0.maxY, var0.maxZ);
            var7.setNormal(0.0F, -1.0F, 0.0F);
            var7.addVertex(var0.minX, var0.minY, var0.minZ);
            var7.addVertex(var0.maxX, var0.minY, var0.minZ);
            var7.addVertex(var0.maxX, var0.minY, var0.maxZ);
            var7.addVertex(var0.minX, var0.minY, var0.maxZ);
            var7.setNormal(0.0F, 1.0F, 0.0F);
            var7.addVertex(var0.minX, var0.maxY, var0.maxZ);
            var7.addVertex(var0.maxX, var0.maxY, var0.maxZ);
            var7.addVertex(var0.maxX, var0.maxY, var0.minZ);
            var7.addVertex(var0.minX, var0.maxY, var0.minZ);
            var7.setNormal(-1.0F, 0.0F, 0.0F);
            var7.addVertex(var0.minX, var0.minY, var0.maxZ);
            var7.addVertex(var0.minX, var0.maxY, var0.maxZ);
            var7.addVertex(var0.minX, var0.maxY, var0.minZ);
            var7.addVertex(var0.minX, var0.minY, var0.minZ);
            var7.setNormal(1.0F, 0.0F, 0.0F);
            var7.addVertex(var0.maxX, var0.minY, var0.minZ);
            var7.addVertex(var0.maxX, var0.maxY, var0.minZ);
            var7.addVertex(var0.maxX, var0.maxY, var0.maxZ);
            var7.addVertex(var0.maxX, var0.minY, var0.maxZ);
            var7.setTranslationD(0.0D, 0.0D, 0.0D);
            var7.draw();
            GLManager.GL.Enable(GLEnum.Texture2D);
        }

        public static void renderAABB(Box var0)
        {
            Tessellator var1 = Tessellator.instance;
            var1.startDrawingQuads();
            var1.addVertex(var0.minX, var0.maxY, var0.minZ);
            var1.addVertex(var0.maxX, var0.maxY, var0.minZ);
            var1.addVertex(var0.maxX, var0.minY, var0.minZ);
            var1.addVertex(var0.minX, var0.minY, var0.minZ);
            var1.addVertex(var0.minX, var0.minY, var0.maxZ);
            var1.addVertex(var0.maxX, var0.minY, var0.maxZ);
            var1.addVertex(var0.maxX, var0.maxY, var0.maxZ);
            var1.addVertex(var0.minX, var0.maxY, var0.maxZ);
            var1.addVertex(var0.minX, var0.minY, var0.minZ);
            var1.addVertex(var0.maxX, var0.minY, var0.minZ);
            var1.addVertex(var0.maxX, var0.minY, var0.maxZ);
            var1.addVertex(var0.minX, var0.minY, var0.maxZ);
            var1.addVertex(var0.minX, var0.maxY, var0.maxZ);
            var1.addVertex(var0.maxX, var0.maxY, var0.maxZ);
            var1.addVertex(var0.maxX, var0.maxY, var0.minZ);
            var1.addVertex(var0.minX, var0.maxY, var0.minZ);
            var1.addVertex(var0.minX, var0.minY, var0.maxZ);
            var1.addVertex(var0.minX, var0.maxY, var0.maxZ);
            var1.addVertex(var0.minX, var0.maxY, var0.minZ);
            var1.addVertex(var0.minX, var0.minY, var0.minZ);
            var1.addVertex(var0.maxX, var0.minY, var0.minZ);
            var1.addVertex(var0.maxX, var0.maxY, var0.minZ);
            var1.addVertex(var0.maxX, var0.maxY, var0.maxZ);
            var1.addVertex(var0.maxX, var0.minY, var0.maxZ);
            var1.draw();
        }

        public void setRenderManager(RenderManager var1)
        {
            renderManager = var1;
        }

        public void doRenderShadowAndFire(Entity var1, double var2, double var4, double var6, float var8, float var9)
        {
            if (shadowSize > 0.0F)
            {
                double var10 = renderManager.func_851_a(var1.posX, var1.posY, var1.posZ);
                float var12 = (float)((1.0D - var10 / 256.0D) * (double)field_194_c);
                if (var12 > 0.0F)
                {
                    renderShadow(var1, var2, var4, var6, var12, var9);
                }
            }

            if (var1.isBurning())
            {
                renderEntityOnFire(var1, var2, var4, var6, var9);
            }

        }

        public FontRenderer getFontRendererFromRenderManager()
        {
            return renderManager.getFontRenderer();
        }
    }

}