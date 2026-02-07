using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Items;
using betareborn.TileEntities;
using betareborn.Worlds;
using Silk.NET.Maths;
using Silk.NET.OpenGL.Legacy;

namespace betareborn.Rendering
{
    public class RenderGlobal : IWorldAccess
    {
        public List<TileEntity> tileEntities = [];
        private World worldObj;
        private readonly RenderEngine renderEngine;
        private readonly Minecraft mc;
        private RenderBlocks globalRenderBlocks;
        private int cloudOffsetX = 0;
        private readonly int starGLCallList;
        private readonly int glSkyList;
        private readonly int glSkyList2;
        private int renderDistance = -1;
        private int renderEntitiesStartupCounter = 2;
        private int countEntitiesTotal;
        private int countEntitiesRendered;
        private int countEntitiesHidden;
        public WorldRenderer worldRenderer;
        public float damagePartialTime;

        public RenderGlobal(Minecraft var1, RenderEngine var2)
        {
            mc = var1;
            renderEngine = var2;
            byte var3 = 64;

            starGLCallList = GLAllocation.generateDisplayLists(3);
            GLManager.GL.PushMatrix();
            GLManager.GL.NewList((uint)starGLCallList, GLEnum.Compile);
            renderStars();
            GLManager.GL.EndList();
            GLManager.GL.PopMatrix();
            Tessellator var4 = Tessellator.instance;
            glSkyList = starGLCallList + 1;
            GLManager.GL.NewList((uint)glSkyList, GLEnum.Compile);
            byte var6 = 64;
            int var7 = 256 / var6 + 2;
            float var5 = 16.0F;

            worldRenderer = new(var1.theWorld, 2);

            int var8;
            int var9;
            for (var8 = -var6 * var7; var8 <= var6 * var7; var8 += var6)
            {
                for (var9 = -var6 * var7; var9 <= var6 * var7; var9 += var6)
                {
                    var4.startDrawingQuads();
                    var4.addVertex((double)(var8 + 0), (double)var5, (double)(var9 + 0));
                    var4.addVertex((double)(var8 + var6), (double)var5, (double)(var9 + 0));
                    var4.addVertex((double)(var8 + var6), (double)var5, (double)(var9 + var6));
                    var4.addVertex((double)(var8 + 0), (double)var5, (double)(var9 + var6));
                    var4.draw();
                }
            }

            GLManager.GL.EndList();
            glSkyList2 = starGLCallList + 2;
            GLManager.GL.NewList((uint)glSkyList2, GLEnum.Compile);
            var5 = -16.0F;
            var4.startDrawingQuads();

            for (var8 = -var6 * var7; var8 <= var6 * var7; var8 += var6)
            {
                for (var9 = -var6 * var7; var9 <= var6 * var7; var9 += var6)
                {
                    var4.addVertex((double)(var8 + var6), (double)var5, (double)(var9 + 0));
                    var4.addVertex((double)(var8 + 0), (double)var5, (double)(var9 + 0));
                    var4.addVertex((double)(var8 + 0), (double)var5, (double)(var9 + var6));
                    var4.addVertex((double)(var8 + var6), (double)var5, (double)(var9 + var6));
                }
            }

            var4.draw();
            GLManager.GL.EndList();
        }

        private void renderStars()
        {
            java.util.Random var1 = new(10842L);
            Tessellator var2 = Tessellator.instance;
            var2.startDrawingQuads();

            for (int var3 = 0; var3 < 1500; ++var3)
            {
                double var4 = (double)(var1.nextFloat() * 2.0F - 1.0F);
                double var6 = (double)(var1.nextFloat() * 2.0F - 1.0F);
                double var8 = (double)(var1.nextFloat() * 2.0F - 1.0F);
                double var10 = (double)(0.25F + var1.nextFloat() * 0.25F);
                double var12 = var4 * var4 + var6 * var6 + var8 * var8;
                if (var12 < 1.0D && var12 > 0.01D)
                {
                    var12 = 1.0D / java.lang.Math.sqrt(var12);
                    var4 *= var12;
                    var6 *= var12;
                    var8 *= var12;
                    double var14 = var4 * 100.0D;
                    double var16 = var6 * 100.0D;
                    double var18 = var8 * 100.0D;
                    double var20 = java.lang.Math.atan2(var4, var8);
                    double var22 = java.lang.Math.sin(var20);
                    double var24 = java.lang.Math.cos(var20);
                    double var26 = java.lang.Math.atan2(java.lang.Math.sqrt(var4 * var4 + var8 * var8), var6);
                    double var28 = java.lang.Math.sin(var26);
                    double var30 = java.lang.Math.cos(var26);
                    double var32 = var1.nextDouble() * java.lang.Math.PI * 2.0D;
                    double var34 = java.lang.Math.sin(var32);
                    double var36 = java.lang.Math.cos(var32);

                    for (int var38 = 0; var38 < 4; ++var38)
                    {
                        double var39 = 0.0D;
                        double var41 = (double)((var38 & 2) - 1) * var10;
                        double var43 = (double)((var38 + 1 & 2) - 1) * var10;
                        double var47 = var41 * var36 - var43 * var34;
                        double var49 = var43 * var36 + var41 * var34;
                        double var53 = var47 * var28 + var39 * var30;
                        double var55 = var39 * var28 - var47 * var30;
                        double var57 = var55 * var22 - var49 * var24;
                        double var61 = var49 * var22 + var55 * var24;
                        var2.addVertex(var14 + var57, var16 + var53, var18 + var61);
                    }
                }
            }

            var2.draw();
        }

        public void changeWorld(World var1)
        {
            worldObj?.removeWorldAccess(this);

            RenderManager.instance.func_852_a(var1);
            worldObj = var1;
            globalRenderBlocks = new RenderBlocks(var1);
            if (var1 != null)
            {
                var1.addWorldAccess(this);
                loadRenderers();
            }

        }

        public void tick(Entity view, float var3)
        {
            if (view == null)
            {
                return;
            }

            double var33 = view.lastTickPosX + (view.posX - view.lastTickPosX) * var3;
            double var7 = view.lastTickPosY + (view.posY - view.lastTickPosY) * var3;
            double var9 = view.lastTickPosZ + (view.posZ - view.lastTickPosZ) * var3;
            worldRenderer.Tick(new(var33, var7, var9));
        }

        public void loadRenderers()
        {
            Block.LEAVES.setGraphicsLevel(true);
            renderDistance = mc.gameSettings.renderDistance;

            worldRenderer?.Dispose();
            worldRenderer = new(worldObj, 2);

            tileEntities.Clear();

            if (renderDistance == 0)
            {
                SubChunkRenderer.SIZE = 32;
                SubChunkRenderer.BITSHIFT_AMOUNT = 5;
            }
            else
            {
                SubChunkRenderer.SIZE = 16;
                SubChunkRenderer.BITSHIFT_AMOUNT = 4;
            }

            renderEntitiesStartupCounter = 2;
        }

        public void renderEntities(Vec3D var1, ICamera var2, float var3)
        {
            if (renderEntitiesStartupCounter > 0)
            {
                --renderEntitiesStartupCounter;
            }
            else
            {
                TileEntityRenderer.instance.cacheActiveRenderInfo(worldObj, renderEngine, mc.fontRenderer, mc.renderViewEntity, var3);
                RenderManager.instance.cacheActiveRenderInfo(worldObj, renderEngine, mc.fontRenderer, mc.renderViewEntity, mc.gameSettings, var3);
                countEntitiesTotal = 0;
                countEntitiesRendered = 0;
                countEntitiesHidden = 0;
                EntityLiving var4 = mc.renderViewEntity;
                RenderManager.renderPosX = var4.lastTickPosX + (var4.posX - var4.lastTickPosX) * (double)var3;
                RenderManager.renderPosY = var4.lastTickPosY + (var4.posY - var4.lastTickPosY) * (double)var3;
                RenderManager.renderPosZ = var4.lastTickPosZ + (var4.posZ - var4.lastTickPosZ) * (double)var3;
                TileEntityRenderer.staticPlayerX = var4.lastTickPosX + (var4.posX - var4.lastTickPosX) * (double)var3;
                TileEntityRenderer.staticPlayerY = var4.lastTickPosY + (var4.posY - var4.lastTickPosY) * (double)var3;
                TileEntityRenderer.staticPlayerZ = var4.lastTickPosZ + (var4.posZ - var4.lastTickPosZ) * (double)var3;
                List<Entity> var5 = worldObj.getLoadedEntityList();
                countEntitiesTotal = var5.Count;

                int var6;
                Entity var7;
                for (var6 = 0; var6 < worldObj.weatherEffects.size(); ++var6)
                {
                    var7 = (Entity)worldObj.weatherEffects.get(var6);
                    ++countEntitiesRendered;
                    if (var7.isInRangeToRenderVec3D(var1))
                    {
                        RenderManager.instance.renderEntity(var7, var3);
                    }
                }

                for (var6 = 0; var6 < var5.Count; ++var6)
                {
                    var7 = var5[var6];
                    if (var7.isInRangeToRenderVec3D(var1) && (var7.ignoreFrustumCheck || var2.isBoundingBoxInFrustum(var7.boundingBox)) && (var7 != mc.renderViewEntity || mc.gameSettings.thirdPersonView || mc.renderViewEntity.isPlayerSleeping()))
                    {
                        int var8 = MathHelper.floor_double(var7.posY);
                        if (var8 < 0)
                        {
                            var8 = 0;
                        }

                        if (var8 >= 128)
                        {
                            var8 = 127;
                        }

                        if (worldObj.blockExists(MathHelper.floor_double(var7.posX), var8, MathHelper.floor_double(var7.posZ)))
                        {
                            ++countEntitiesRendered;
                            RenderManager.instance.renderEntity(var7, var3);
                        }
                    }
                }

                for (var6 = 0; var6 < tileEntities.Count; ++var6)
                {
                    TileEntityRenderer.instance.renderTileEntity(tileEntities[var6], var3);
                }

            }
        }

        public string getDebugInfoEntities()
        {
            return "E: " + countEntitiesRendered + "/" + countEntitiesTotal + ". B: " + countEntitiesHidden + ", I: " + (countEntitiesTotal - countEntitiesHidden - countEntitiesRendered);
        }

        public int sortAndRender(EntityLiving var1, int pass, double var3, ICamera cam)
        {
            if (mc.gameSettings.renderDistance != renderDistance)
            {
                loadRenderers();
            }

            double var33 = var1.lastTickPosX + (var1.posX - var1.lastTickPosX) * var3;
            double var7 = var1.lastTickPosY + (var1.posY - var1.lastTickPosY) * var3;
            double var9 = var1.lastTickPosZ + (var1.posZ - var1.lastTickPosZ) * var3;

            RenderHelper.disableStandardItemLighting();

            if (pass == 0)
            {
                worldRenderer.Render(cam, new(var33, var7, var9), renderDistance, worldObj.getWorldTime(), (float)var3, mc.gameSettings.environmentAnimation);
            }
            else
            {
                worldRenderer.RenderTransparent(new(var33, var7, var9));
            }

            return 0;
        }

        public void updateClouds()
        {
            ++cloudOffsetX;
        }

        public void renderSky(float var1)
        {
            if (!mc.theWorld.dimension.isNether)
            {
                GLManager.GL.Disable(GLEnum.Texture2D);
                Vector3D<double> var2 = worldObj.func_4079_a(mc.renderViewEntity, var1);
                float var3 = (float)var2.X;
                float var4 = (float)var2.Y;
                float var5 = (float)var2.Z;
                float var7;
                float var8;

                GLManager.GL.Color3(var3, var4, var5);
                Tessellator var17 = Tessellator.instance;
                GLManager.GL.DepthMask(false);
                GLManager.GL.Enable(GLEnum.Fog);
                GLManager.GL.Color3(var3, var4, var5);
                GLManager.GL.CallList((uint)glSkyList);
                GLManager.GL.Disable(GLEnum.Fog);
                GLManager.GL.Disable(GLEnum.AlphaTest);
                GLManager.GL.Enable(GLEnum.Blend);
                GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
                RenderHelper.disableStandardItemLighting();
                float[] var18 = worldObj.dimension.calcSunriseSunsetColors(worldObj.getCelestialAngle(var1), var1);
                float var9;
                float var10;
                float var11;
                float var12;
                if (var18 != null)
                {
                    GLManager.GL.Disable(GLEnum.Texture2D);
                    GLManager.GL.ShadeModel(GLEnum.Smooth);
                    GLManager.GL.PushMatrix();
                    GLManager.GL.Rotate(90.0F, 1.0F, 0.0F, 0.0F);
                    var8 = worldObj.getCelestialAngle(var1);
                    GLManager.GL.Rotate(var8 > 0.5F ? 180.0F : 0.0F, 0.0F, 0.0F, 1.0F);
                    var9 = var18[0];
                    var10 = var18[1];
                    var11 = var18[2];
                    float var14;

                    var17.startDrawing(6);
                    var17.setColorRGBA_F(var9, var10, var11, var18[3]);
                    var17.addVertex(0.0D, 100.0D, 0.0D);
                    byte var19 = 16;
                    var17.setColorRGBA_F(var18[0], var18[1], var18[2], 0.0F);

                    for (int var20 = 0; var20 <= var19; ++var20)
                    {
                        var14 = (float)var20 * (float)java.lang.Math.PI * 2.0F / (float)var19;
                        float var15 = MathHelper.sin(var14);
                        float var16 = MathHelper.cos(var14);
                        var17.addVertex((double)(var15 * 120.0F), (double)(var16 * 120.0F), (double)(-var16 * 40.0F * var18[3]));
                    }

                    var17.draw();
                    GLManager.GL.PopMatrix();
                    GLManager.GL.ShadeModel(GLEnum.Flat);
                }

                GLManager.GL.Enable(GLEnum.Texture2D);
                GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.One);
                GLManager.GL.PushMatrix();
                var7 = 1.0F - worldObj.func_27162_g(var1);
                var8 = 0.0F;
                var9 = 0.0F;
                var10 = 0.0F;
                GLManager.GL.Color4(1.0F, 1.0F, 1.0F, var7);
                GLManager.GL.Translate(var8, var9, var10);
                GLManager.GL.Rotate(0.0F, 0.0F, 0.0F, 1.0F);
                GLManager.GL.Rotate(worldObj.getCelestialAngle(var1) * 360.0F, 1.0F, 0.0F, 0.0F);
                var11 = 30.0F;
                GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)renderEngine.getTexture("/terrain/sun.png"));
                var17.startDrawingQuads();
                var17.addVertexWithUV((double)(-var11), 100.0D, (double)(-var11), 0.0D, 0.0D);
                var17.addVertexWithUV((double)var11, 100.0D, (double)(-var11), 1.0D, 0.0D);
                var17.addVertexWithUV((double)var11, 100.0D, (double)var11, 1.0D, 1.0D);
                var17.addVertexWithUV((double)(-var11), 100.0D, (double)var11, 0.0D, 1.0D);
                var17.draw();
                var11 = 20.0F;
                GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)renderEngine.getTexture("/terrain/moon.png"));
                var17.startDrawingQuads();
                var17.addVertexWithUV((double)(-var11), -100.0D, (double)var11, 1.0D, 1.0D);
                var17.addVertexWithUV((double)var11, -100.0D, (double)var11, 0.0D, 1.0D);
                var17.addVertexWithUV((double)var11, -100.0D, (double)(-var11), 0.0D, 0.0D);
                var17.addVertexWithUV((double)(-var11), -100.0D, (double)(-var11), 1.0D, 0.0D);
                var17.draw();
                GLManager.GL.Disable(GLEnum.Texture2D);
                var12 = worldObj.getStarBrightness(var1) * var7;
                if (var12 > 0.0F)
                {
                    GLManager.GL.Color4(var12, var12, var12, var12);
                    GLManager.GL.CallList((uint)starGLCallList);
                }

                GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
                GLManager.GL.Disable(GLEnum.Blend);
                GLManager.GL.Enable(GLEnum.AlphaTest);
                GLManager.GL.Enable(GLEnum.Fog);
                GLManager.GL.PopMatrix();
                if (worldObj.dimension.func_28112_c())
                {
                    GLManager.GL.Color3(var3 * 0.2F + 0.04F, var4 * 0.2F + 0.04F, var5 * 0.6F + 0.1F);
                }
                else
                {
                    GLManager.GL.Color3(var3, var4, var5);
                }

                GLManager.GL.Disable(GLEnum.Texture2D);
                GLManager.GL.CallList((uint)glSkyList2);
                GLManager.GL.Enable(GLEnum.Texture2D);
                GLManager.GL.DepthMask(true);
            }
        }

        public void renderClouds(float var1)
        {
            if (!mc.theWorld.dimension.isNether)
            {
                renderCloudsFancy(var1);
            }
        }

        private void renderCloudsFancy(float var1)
        {
            GLManager.GL.Disable(GLEnum.CullFace);
            float var2 = (float)(mc.renderViewEntity.lastTickPosY + (mc.renderViewEntity.posY - mc.renderViewEntity.lastTickPosY) * (double)var1);
            Tessellator var3 = Tessellator.instance;
            float var4 = 12.0F;
            float var5 = 4.0F;
            double var6 = (mc.renderViewEntity.prevPosX + (mc.renderViewEntity.posX - mc.renderViewEntity.prevPosX) * (double)var1 + (double)(((float)cloudOffsetX + var1) * 0.03F)) / (double)var4;
            double var8 = (mc.renderViewEntity.prevPosZ + (mc.renderViewEntity.posZ - mc.renderViewEntity.prevPosZ) * (double)var1) / (double)var4 + (double)0.33F;
            float var10 = worldObj.dimension.getCloudHeight() - var2 + 0.33F;
            int var11 = MathHelper.floor_double(var6 / 2048.0D);
            int var12 = MathHelper.floor_double(var8 / 2048.0D);
            var6 -= (double)(var11 * 2048);
            var8 -= (double)(var12 * 2048);
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)renderEngine.getTexture("/environment/clouds.png"));
            GLManager.GL.Enable(GLEnum.Blend);
            GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
            Vector3D<double> var13 = worldObj.func_628_d(var1);
            float var14 = (float)var13.X;
            float var15 = (float)var13.Y;
            float var16 = (float)var13.Z;
            float var17;
            float var18;
            float var19;

            var17 = (float)(var6 * 0.0D);
            var18 = (float)(var8 * 0.0D);
            var19 = 0.00390625F;
            var17 = (float)MathHelper.floor_double(var6) * var19;
            var18 = (float)MathHelper.floor_double(var8) * var19;
            float var20 = (float)(var6 - (double)MathHelper.floor_double(var6));
            float var21 = (float)(var8 - (double)MathHelper.floor_double(var8));
            byte var22 = 8;
            byte var23 = 3;
            float var24 = 1.0F / 1024.0F;
            GLManager.GL.Scale(var4, 1.0F, var4);

            for (int var25 = 0; var25 < 2; ++var25)
            {
                if (var25 == 0)
                {
                    GLManager.GL.ColorMask(false, false, false, false);
                }
                else
                {
                    GLManager.GL.ColorMask(true, true, true, true);
                }

                for (int var26 = -var23 + 1; var26 <= var23; ++var26)
                {
                    for (int var27 = -var23 + 1; var27 <= var23; ++var27)
                    {
                        var3.startDrawingQuads();
                        float var28 = (float)(var26 * var22);
                        float var29 = (float)(var27 * var22);
                        float var30 = var28 - var20;
                        float var31 = var29 - var21;
                        if (var10 > -var5 - 1.0F)
                        {
                            var3.setColorRGBA_F(var14 * 0.7F, var15 * 0.7F, var16 * 0.7F, 0.8F);
                            var3.setNormal(0.0F, -1.0F, 0.0F);
                            var3.addVertexWithUV((double)(var30 + 0.0F), (double)(var10 + 0.0F), (double)(var31 + (float)var22), (double)((var28 + 0.0F) * var19 + var17), (double)((var29 + (float)var22) * var19 + var18));
                            var3.addVertexWithUV((double)(var30 + (float)var22), (double)(var10 + 0.0F), (double)(var31 + (float)var22), (double)((var28 + (float)var22) * var19 + var17), (double)((var29 + (float)var22) * var19 + var18));
                            var3.addVertexWithUV((double)(var30 + (float)var22), (double)(var10 + 0.0F), (double)(var31 + 0.0F), (double)((var28 + (float)var22) * var19 + var17), (double)((var29 + 0.0F) * var19 + var18));
                            var3.addVertexWithUV((double)(var30 + 0.0F), (double)(var10 + 0.0F), (double)(var31 + 0.0F), (double)((var28 + 0.0F) * var19 + var17), (double)((var29 + 0.0F) * var19 + var18));
                        }

                        if (var10 <= var5 + 1.0F)
                        {
                            var3.setColorRGBA_F(var14, var15, var16, 0.8F);
                            var3.setNormal(0.0F, 1.0F, 0.0F);
                            var3.addVertexWithUV((double)(var30 + 0.0F), (double)(var10 + var5 - var24), (double)(var31 + (float)var22), (double)((var28 + 0.0F) * var19 + var17), (double)((var29 + (float)var22) * var19 + var18));
                            var3.addVertexWithUV((double)(var30 + (float)var22), (double)(var10 + var5 - var24), (double)(var31 + (float)var22), (double)((var28 + (float)var22) * var19 + var17), (double)((var29 + (float)var22) * var19 + var18));
                            var3.addVertexWithUV((double)(var30 + (float)var22), (double)(var10 + var5 - var24), (double)(var31 + 0.0F), (double)((var28 + (float)var22) * var19 + var17), (double)((var29 + 0.0F) * var19 + var18));
                            var3.addVertexWithUV((double)(var30 + 0.0F), (double)(var10 + var5 - var24), (double)(var31 + 0.0F), (double)((var28 + 0.0F) * var19 + var17), (double)((var29 + 0.0F) * var19 + var18));
                        }

                        var3.setColorRGBA_F(var14 * 0.9F, var15 * 0.9F, var16 * 0.9F, 0.8F);
                        int var32;
                        if (var26 > -1)
                        {
                            var3.setNormal(-1.0F, 0.0F, 0.0F);

                            for (var32 = 0; var32 < var22; ++var32)
                            {
                                var3.addVertexWithUV((double)(var30 + (float)var32 + 0.0F), (double)(var10 + 0.0F), (double)(var31 + (float)var22), (double)((var28 + (float)var32 + 0.5F) * var19 + var17), (double)((var29 + (float)var22) * var19 + var18));
                                var3.addVertexWithUV((double)(var30 + (float)var32 + 0.0F), (double)(var10 + var5), (double)(var31 + (float)var22), (double)((var28 + (float)var32 + 0.5F) * var19 + var17), (double)((var29 + (float)var22) * var19 + var18));
                                var3.addVertexWithUV((double)(var30 + (float)var32 + 0.0F), (double)(var10 + var5), (double)(var31 + 0.0F), (double)((var28 + (float)var32 + 0.5F) * var19 + var17), (double)((var29 + 0.0F) * var19 + var18));
                                var3.addVertexWithUV((double)(var30 + (float)var32 + 0.0F), (double)(var10 + 0.0F), (double)(var31 + 0.0F), (double)((var28 + (float)var32 + 0.5F) * var19 + var17), (double)((var29 + 0.0F) * var19 + var18));
                            }
                        }

                        if (var26 <= 1)
                        {
                            var3.setNormal(1.0F, 0.0F, 0.0F);

                            for (var32 = 0; var32 < var22; ++var32)
                            {
                                var3.addVertexWithUV((double)(var30 + (float)var32 + 1.0F - var24), (double)(var10 + 0.0F), (double)(var31 + (float)var22), (double)((var28 + (float)var32 + 0.5F) * var19 + var17), (double)((var29 + (float)var22) * var19 + var18));
                                var3.addVertexWithUV((double)(var30 + (float)var32 + 1.0F - var24), (double)(var10 + var5), (double)(var31 + (float)var22), (double)((var28 + (float)var32 + 0.5F) * var19 + var17), (double)((var29 + (float)var22) * var19 + var18));
                                var3.addVertexWithUV((double)(var30 + (float)var32 + 1.0F - var24), (double)(var10 + var5), (double)(var31 + 0.0F), (double)((var28 + (float)var32 + 0.5F) * var19 + var17), (double)((var29 + 0.0F) * var19 + var18));
                                var3.addVertexWithUV((double)(var30 + (float)var32 + 1.0F - var24), (double)(var10 + 0.0F), (double)(var31 + 0.0F), (double)((var28 + (float)var32 + 0.5F) * var19 + var17), (double)((var29 + 0.0F) * var19 + var18));
                            }
                        }

                        var3.setColorRGBA_F(var14 * 0.8F, var15 * 0.8F, var16 * 0.8F, 0.8F);
                        if (var27 > -1)
                        {
                            var3.setNormal(0.0F, 0.0F, -1.0F);

                            for (var32 = 0; var32 < var22; ++var32)
                            {
                                var3.addVertexWithUV((double)(var30 + 0.0F), (double)(var10 + var5), (double)(var31 + (float)var32 + 0.0F), (double)((var28 + 0.0F) * var19 + var17), (double)((var29 + (float)var32 + 0.5F) * var19 + var18));
                                var3.addVertexWithUV((double)(var30 + (float)var22), (double)(var10 + var5), (double)(var31 + (float)var32 + 0.0F), (double)((var28 + (float)var22) * var19 + var17), (double)((var29 + (float)var32 + 0.5F) * var19 + var18));
                                var3.addVertexWithUV((double)(var30 + (float)var22), (double)(var10 + 0.0F), (double)(var31 + (float)var32 + 0.0F), (double)((var28 + (float)var22) * var19 + var17), (double)((var29 + (float)var32 + 0.5F) * var19 + var18));
                                var3.addVertexWithUV((double)(var30 + 0.0F), (double)(var10 + 0.0F), (double)(var31 + (float)var32 + 0.0F), (double)((var28 + 0.0F) * var19 + var17), (double)((var29 + (float)var32 + 0.5F) * var19 + var18));
                            }
                        }

                        if (var27 <= 1)
                        {
                            var3.setNormal(0.0F, 0.0F, 1.0F);

                            for (var32 = 0; var32 < var22; ++var32)
                            {
                                var3.addVertexWithUV((double)(var30 + 0.0F), (double)(var10 + var5), (double)(var31 + (float)var32 + 1.0F - var24), (double)((var28 + 0.0F) * var19 + var17), (double)((var29 + (float)var32 + 0.5F) * var19 + var18));
                                var3.addVertexWithUV((double)(var30 + (float)var22), (double)(var10 + var5), (double)(var31 + (float)var32 + 1.0F - var24), (double)((var28 + (float)var22) * var19 + var17), (double)((var29 + (float)var32 + 0.5F) * var19 + var18));
                                var3.addVertexWithUV((double)(var30 + (float)var22), (double)(var10 + 0.0F), (double)(var31 + (float)var32 + 1.0F - var24), (double)((var28 + (float)var22) * var19 + var17), (double)((var29 + (float)var32 + 0.5F) * var19 + var18));
                                var3.addVertexWithUV((double)(var30 + 0.0F), (double)(var10 + 0.0F), (double)(var31 + (float)var32 + 1.0F - var24), (double)((var28 + 0.0F) * var19 + var17), (double)((var29 + (float)var32 + 0.5F) * var19 + var18));
                            }
                        }

                        var3.draw();
                    }
                }
            }

            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            GLManager.GL.Disable(GLEnum.Blend);
            GLManager.GL.Enable(GLEnum.CullFace);
        }

        public void drawBlockBreaking(EntityPlayer var1, MovingObjectPosition var2, int var3, ItemStack var4, float var5)
        {
            Tessellator var6 = Tessellator.instance;
            GLManager.GL.Enable(GLEnum.Blend);
            GLManager.GL.Enable(GLEnum.AlphaTest);
            GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.One);
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, (MathHelper.sin((float)java.lang.System.currentTimeMillis() / 100.0F) * 0.2F + 0.4F) * 0.5F);
            int var8;
            if (var3 == 0)
            {
                if (damagePartialTime > 0.0F)
                {
                    GLManager.GL.BlendFunc(GLEnum.DstColor, GLEnum.SrcColor);
                    int var7 = renderEngine.getTexture("/terrain.png");
                    GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)var7);
                    GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 0.5F);
                    GLManager.GL.PushMatrix();
                    var8 = worldObj.getBlockId(var2.blockX, var2.blockY, var2.blockZ);
                    Block var9 = var8 > 0 ? Block.BLOCKS[var8] : null;
                    GLManager.GL.Disable(GLEnum.AlphaTest);
                    GLManager.GL.PolygonOffset(-3.0F, -3.0F);
                    GLManager.GL.Enable(GLEnum.PolygonOffsetFill);
                    double var10 = var1.lastTickPosX + (var1.posX - var1.lastTickPosX) * (double)var5;
                    double var12 = var1.lastTickPosY + (var1.posY - var1.lastTickPosY) * (double)var5;
                    double var14 = var1.lastTickPosZ + (var1.posZ - var1.lastTickPosZ) * (double)var5;
                    if (var9 == null)
                    {
                        var9 = Block.STONE;
                    }

                    GLManager.GL.Enable(GLEnum.AlphaTest);
                    var6.startDrawingQuads();
                    var6.setTranslationD(-var10, -var12, -var14);
                    var6.disableColor();
                    globalRenderBlocks.renderBlockUsingTexture(var9, var2.blockX, var2.blockY, var2.blockZ, 240 + (int)(damagePartialTime * 10.0F));
                    var6.draw();
                    var6.setTranslationD(0.0D, 0.0D, 0.0D);
                    GLManager.GL.Disable(GLEnum.AlphaTest);
                    GLManager.GL.PolygonOffset(0.0F, 0.0F);
                    GLManager.GL.Disable(GLEnum.PolygonOffsetFill);
                    GLManager.GL.Enable(GLEnum.AlphaTest);
                    GLManager.GL.DepthMask(true);
                    GLManager.GL.PopMatrix();
                }
            }
            else if (var4 != null)
            {
                GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
                float var16 = MathHelper.sin((float)java.lang.System.currentTimeMillis() / 100.0F) * 0.2F + 0.8F;
                GLManager.GL.Color4(var16, var16, var16, MathHelper.sin((float)java.lang.System.currentTimeMillis() / 200.0F) * 0.2F + 0.5F);
                var8 = renderEngine.getTexture("/terrain.png");
                GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)var8);
                int var17 = var2.blockX;
                int var18 = var2.blockY;
                int var11 = var2.blockZ;
                if (var2.sideHit == 0)
                {
                    --var18;
                }

                if (var2.sideHit == 1)
                {
                    ++var18;
                }

                if (var2.sideHit == 2)
                {
                    --var11;
                }

                if (var2.sideHit == 3)
                {
                    ++var11;
                }

                if (var2.sideHit == 4)
                {
                    --var17;
                }

                if (var2.sideHit == 5)
                {
                    ++var17;
                }
            }

            GLManager.GL.Disable(GLEnum.Blend);
            GLManager.GL.Disable(GLEnum.AlphaTest);
        }

        public void drawSelectionBox(EntityPlayer var1, MovingObjectPosition var2, int var3, ItemStack var4, float var5)
        {
            if (var3 == 0 && var2.typeOfHit == EnumMovingObjectType.TILE)
            {
                GLManager.GL.Enable(GLEnum.Blend);
                GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
                GLManager.GL.Color4(0.0F, 0.0F, 0.0F, 0.4F);
                GLManager.GL.LineWidth(2.0F);
                GLManager.GL.Disable(GLEnum.Texture2D);
                GLManager.GL.DepthMask(false);
                float var6 = 0.002F;
                int var7 = worldObj.getBlockId(var2.blockX, var2.blockY, var2.blockZ);
                if (var7 > 0)
                {
                    Block.BLOCKS[var7].updateBoundingBox(worldObj, var2.blockX, var2.blockY, var2.blockZ);
                    double var8 = var1.lastTickPosX + (var1.posX - var1.lastTickPosX) * (double)var5;
                    double var10 = var1.lastTickPosY + (var1.posY - var1.lastTickPosY) * (double)var5;
                    double var12 = var1.lastTickPosZ + (var1.posZ - var1.lastTickPosZ) * (double)var5;
                    drawOutlinedBoundingBox(Block.BLOCKS[var7].getBoundingBox(worldObj, var2.blockX, var2.blockY, var2.blockZ).expand((double)var6, (double)var6, (double)var6).translate(-var8, -var10, -var12));
                }

                GLManager.GL.DepthMask(true);
                GLManager.GL.Enable(GLEnum.Texture2D);
                GLManager.GL.Disable(GLEnum.Blend);
            }

        }

        private void drawOutlinedBoundingBox(Box var1)
        {
            Tessellator var2 = Tessellator.instance;
            var2.startDrawing(3);
            var2.addVertex(var1.minX, var1.minY, var1.minZ);
            var2.addVertex(var1.maxX, var1.minY, var1.minZ);
            var2.addVertex(var1.maxX, var1.minY, var1.maxZ);
            var2.addVertex(var1.minX, var1.minY, var1.maxZ);
            var2.addVertex(var1.minX, var1.minY, var1.minZ);
            var2.draw();
            var2.startDrawing(3);
            var2.addVertex(var1.minX, var1.maxY, var1.minZ);
            var2.addVertex(var1.maxX, var1.maxY, var1.minZ);
            var2.addVertex(var1.maxX, var1.maxY, var1.maxZ);
            var2.addVertex(var1.minX, var1.maxY, var1.maxZ);
            var2.addVertex(var1.minX, var1.maxY, var1.minZ);
            var2.draw();
            var2.startDrawing(1);
            var2.addVertex(var1.minX, var1.minY, var1.minZ);
            var2.addVertex(var1.minX, var1.maxY, var1.minZ);
            var2.addVertex(var1.maxX, var1.minY, var1.minZ);
            var2.addVertex(var1.maxX, var1.maxY, var1.minZ);
            var2.addVertex(var1.maxX, var1.minY, var1.maxZ);
            var2.addVertex(var1.maxX, var1.maxY, var1.maxZ);
            var2.addVertex(var1.minX, var1.minY, var1.maxZ);
            var2.addVertex(var1.minX, var1.maxY, var1.maxZ);
            var2.draw();
        }

        public void func_949_a(int var1, int var2, int var3, int var4, int var5, int var6)
        {
            int var7 = MathHelper.bucketInt(var1, SubChunkRenderer.SIZE);
            int var8 = MathHelper.bucketInt(var2, SubChunkRenderer.SIZE);
            int var9 = MathHelper.bucketInt(var3, SubChunkRenderer.SIZE);
            int var10 = MathHelper.bucketInt(var4, SubChunkRenderer.SIZE);
            int var11 = MathHelper.bucketInt(var5, SubChunkRenderer.SIZE);
            int var12 = MathHelper.bucketInt(var6, SubChunkRenderer.SIZE);

            for (int var13 = var7; var13 <= var10; ++var13)
            {
                for (int var15 = var8; var15 <= var11; ++var15)
                {
                    for (int var17 = var9; var17 <= var12; ++var17)
                    {
                        worldRenderer.MarkDirty(new Vector3D<int>(var13, var15, var17) * SubChunkRenderer.SIZE, true);
                    }
                }
            }
        }

        public void markBlockAndNeighborsNeedsUpdate(int var1, int var2, int var3)
        {
            func_949_a(var1 - 1, var2 - 1, var3 - 1, var1 + 1, var2 + 1, var3 + 1);
        }

        public void markBlockRangeNeedsUpdate(int var1, int var2, int var3, int var4, int var5, int var6)
        {
            func_949_a(var1 - 1, var2 - 1, var3 - 1, var4 + 1, var5 + 1, var6 + 1);
        }

        public void playRecord(string var1, int var2, int var3, int var4)
        {
            if (var1 != null)
            {
                mc.ingameGUI.setRecordPlayingMessage("C418 - " + var1);
            }

            mc.sndManager.playStreaming(var1, (float)var2, (float)var3, (float)var4, 1.0F, 1.0F);
        }

        public void playSound(string var1, double var2, double var4, double var6, float var8, float var9)
        {
            float var10 = 16.0F;
            if (var8 > 1.0F)
            {
                var10 *= var8;
            }

            if (mc.renderViewEntity.getSquaredDistance(var2, var4, var6) < (double)(var10 * var10))
            {
                mc.sndManager.playSound(var1, (float)var2, (float)var4, (float)var6, var8, var9);
            }

        }

        public void spawnParticle(string var1, double var2, double var4, double var6, double var8, double var10, double var12)
        {
            if (mc != null && mc.renderViewEntity != null && mc.effectRenderer != null)
            {
                double var14 = mc.renderViewEntity.posX - var2;
                double var16 = mc.renderViewEntity.posY - var4;
                double var18 = mc.renderViewEntity.posZ - var6;
                double var20 = 16.0D;
                if (var14 * var14 + var16 * var16 + var18 * var18 <= var20 * var20)
                {
                    if (var1.Equals("bubble"))
                    {
                        mc.effectRenderer.addEffect(new EntityBubbleFX(worldObj, var2, var4, var6, var8, var10, var12));
                    }
                    else if (var1.Equals("smoke"))
                    {
                        mc.effectRenderer.addEffect(new EntitySmokeFX(worldObj, var2, var4, var6, var8, var10, var12));
                    }
                    else if (var1.Equals("note"))
                    {
                        mc.effectRenderer.addEffect(new EntityNoteFX(worldObj, var2, var4, var6, var8, var10, var12));
                    }
                    else if (var1.Equals("portal"))
                    {
                        mc.effectRenderer.addEffect(new EntityPortalFX(worldObj, var2, var4, var6, var8, var10, var12));
                    }
                    else if (var1.Equals("explode"))
                    {
                        mc.effectRenderer.addEffect(new EntityExplodeFX(worldObj, var2, var4, var6, var8, var10, var12));
                    }
                    else if (var1.Equals("flame"))
                    {
                        mc.effectRenderer.addEffect(new EntityFlameFX(worldObj, var2, var4, var6, var8, var10, var12));
                    }
                    else if (var1.Equals("lava"))
                    {
                        mc.effectRenderer.addEffect(new EntityLavaFX(worldObj, var2, var4, var6));
                    }
                    else if (var1.Equals("footstep"))
                    {
                        mc.effectRenderer.addEffect(new EntityFootStepFX(renderEngine, worldObj, var2, var4, var6));
                    }
                    else if (var1.Equals("splash"))
                    {
                        mc.effectRenderer.addEffect(new EntitySplashFX(worldObj, var2, var4, var6, var8, var10, var12));
                    }
                    else if (var1.Equals("largesmoke"))
                    {
                        mc.effectRenderer.addEffect(new EntitySmokeFX(worldObj, var2, var4, var6, var8, var10, var12, 2.5F));
                    }
                    else if (var1.Equals("reddust"))
                    {
                        mc.effectRenderer.addEffect(new EntityReddustFX(worldObj, var2, var4, var6, (float)var8, (float)var10, (float)var12));
                    }
                    else if (var1.Equals("snowballpoof"))
                    {
                        mc.effectRenderer.addEffect(new EntitySlimeFX(worldObj, var2, var4, var6, Item.snowball));
                    }
                    else if (var1.Equals("snowshovel"))
                    {
                        mc.effectRenderer.addEffect(new EntitySnowShovelFX(worldObj, var2, var4, var6, var8, var10, var12));
                    }
                    else if (var1.Equals("slime"))
                    {
                        mc.effectRenderer.addEffect(new EntitySlimeFX(worldObj, var2, var4, var6, Item.slimeBall));
                    }
                    else if (var1.Equals("heart"))
                    {
                        mc.effectRenderer.addEffect(new EntityHeartFX(worldObj, var2, var4, var6, var8, var10, var12));
                    }

                }
            }
        }

        public void obtainEntitySkin(Entity var1)
        {
            var1.updateCloak();
            //TODO: SKINS
            //if (var1.skinUrl != null)
            //{
            //    renderEngine.obtainImageData(var1.skinUrl, new ImageBufferDownload());
            //}

            //if (var1.cloakUrl != null)
            //{
            //    renderEngine.obtainImageData(var1.cloakUrl, new ImageBufferDownload());
            //}

        }

        public void releaseEntitySkin(Entity var1)
        {
            //TODO: SKINS
            //if (var1.skinUrl != null)
            //{
            //    renderEngine.releaseImageData(var1.skinUrl);
            //}

            //if (var1.cloakUrl != null)
            //{
            //    renderEngine.releaseImageData(var1.cloakUrl);
            //}

        }

        public void updateAllRenderers()
        {
            worldRenderer.UpdateAllRenderers();
        }

        public void doNothingWithTileEntity(int var1, int var2, int var3, TileEntity var4)
        {
        }

        public void func_28136_a(EntityPlayer var1, int var2, int var3, int var4, int var5, int var6)
        {
            java.util.Random var7 = worldObj.random;
            int var16;
            switch (var2)
            {
                case 1000:
                    worldObj.playSoundEffect((double)var3, (double)var4, (double)var5, "random.click", 1.0F, 1.0F);
                    break;
                case 1001:
                    worldObj.playSoundEffect((double)var3, (double)var4, (double)var5, "random.click", 1.0F, 1.2F);
                    break;
                case 1002:
                    worldObj.playSoundEffect((double)var3, (double)var4, (double)var5, "random.bow", 1.0F, 1.2F);
                    break;
                case 1003:
                    if (java.lang.Math.random() < 0.5D)
                    {
                        worldObj.playSoundEffect((double)var3 + 0.5D, (double)var4 + 0.5D, (double)var5 + 0.5D, "random.door_open", 1.0F, worldObj.random.nextFloat() * 0.1F + 0.9F);
                    }
                    else
                    {
                        worldObj.playSoundEffect((double)var3 + 0.5D, (double)var4 + 0.5D, (double)var5 + 0.5D, "random.door_close", 1.0F, worldObj.random.nextFloat() * 0.1F + 0.9F);
                    }
                    break;
                case 1004:
                    worldObj.playSoundEffect((double)((float)var3 + 0.5F), (double)((float)var4 + 0.5F), (double)((float)var5 + 0.5F), "random.fizz", 0.5F, 2.6F + (var7.nextFloat() - var7.nextFloat()) * 0.8F);
                    break;
                case 1005:
                    if (Item.itemsList[var6] is ItemRecord)
                    {
                        worldObj.playRecord(((ItemRecord)Item.itemsList[var6]).recordName, var3, var4, var5);
                    }
                    else
                    {
                        worldObj.playRecord((String)null, var3, var4, var5);
                    }
                    break;
                case 2000:
                    int var8 = var6 % 3 - 1;
                    int var9 = var6 / 3 % 3 - 1;
                    double var10 = (double)var3 + (double)var8 * 0.6D + 0.5D;
                    double var12 = (double)var4 + 0.5D;
                    double var14 = (double)var5 + (double)var9 * 0.6D + 0.5D;

                    for (var16 = 0; var16 < 10; ++var16)
                    {
                        double var31 = var7.nextDouble() * 0.2D + 0.01D;
                        double var19 = var10 + (double)var8 * 0.01D + (var7.nextDouble() - 0.5D) * (double)var9 * 0.5D;
                        double var21 = var12 + (var7.nextDouble() - 0.5D) * 0.5D;
                        double var23 = var14 + (double)var9 * 0.01D + (var7.nextDouble() - 0.5D) * (double)var8 * 0.5D;
                        double var25 = (double)var8 * var31 + var7.nextGaussian() * 0.01D;
                        double var27 = -0.03D + var7.nextGaussian() * 0.01D;
                        double var29 = (double)var9 * var31 + var7.nextGaussian() * 0.01D;
                        spawnParticle("smoke", var19, var21, var23, var25, var27, var29);
                    }

                    return;
                case 2001:
                    var16 = var6 & 255;
                    if (var16 > 0)
                    {
                        Block var17 = Block.BLOCKS[var16];
                        mc.sndManager.playSound(var17.soundGroup.stepSoundDir(), (float)var3 + 0.5F, (float)var4 + 0.5F, (float)var5 + 0.5F, (var17.soundGroup.getVolume() + 1.0F) / 2.0F, var17.soundGroup.getPitch() * 0.8F);
                    }

                    mc.effectRenderer.addBlockDestroyEffects(var3, var4, var5, var6 & 255, var6 >> 8 & 255);
                    break;
            }

        }
    }

}