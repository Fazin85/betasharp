using betareborn.Biomes;
using betareborn.Blocks;
using betareborn.Chunks;
using betareborn.Materials;
using betareborn.Profiling;
using betareborn.Rendering;
using betareborn.Worlds;
using Silk.NET.Maths;
using Silk.NET.OpenGL.Legacy;

namespace betareborn.Entities
{
    public class EntityRenderer
    {
        public static int anaglyphField;
        private Minecraft mc;
        private float farPlaneDistance = 0.0F;
        public ItemRenderer itemRenderer;
        private int rendererUpdateCount;
        private Entity pointedEntity = null;
        private MouseFilter mouseFilterXAxis = new MouseFilter();
        private MouseFilter mouseFilterYAxis = new MouseFilter();
        private float field_22228_r = 4.0F;
        private float field_22227_s = 4.0F;
        private float field_22226_t = 0.0F;
        private float field_22225_u = 0.0F;
        private float field_22224_v = 0.0F;
        private float field_22223_w = 0.0F;
        private float field_22222_x = 0.0F;
        private float field_22221_y = 0.0F;
        private float field_22220_z = 0.0F;
        private float field_22230_A = 0.0F;
        private bool cloudFog = false;
        private double cameraZoom = 1.0D;
        private double cameraYaw = 0.0D;
        private double cameraPitch = 0.0D;
        private long prevFrameTime = java.lang.System.currentTimeMillis();
        private long field_28133_I = 0L;
        private java.util.Random random = new();
        private int rainSoundCounter = 0;
        float[] fogColorBuffer = new float[16];
        float fogColorRed;
        float fogColorGreen;
        float fogColorBlue;
        private float fogColor2;
        private float fogColor1;

        public EntityRenderer(Minecraft var1)
        {
            mc = var1;
            itemRenderer = new ItemRenderer(var1);
        }

        public void updateRenderer()
        {
            fogColor2 = fogColor1;
            field_22227_s = field_22228_r;
            field_22225_u = field_22226_t;
            field_22223_w = field_22224_v;
            field_22221_y = field_22222_x;
            field_22230_A = field_22220_z;
            if (mc.renderViewEntity == null)
            {
                mc.renderViewEntity = mc.thePlayer;
            }

            float var1 = mc.theWorld.getLuminance(MathHelper.floor_double(mc.renderViewEntity.posX), MathHelper.floor_double(mc.renderViewEntity.posY), MathHelper.floor_double(mc.renderViewEntity.posZ));
            float var2 = (float)(3 - mc.gameSettings.renderDistance) / 3.0F;
            float var3 = var1 * (1.0F - var2) + var2;
            fogColor1 += (var3 - fogColor1) * 0.1F;
            ++rendererUpdateCount;
            itemRenderer.updateEquippedItem();
            addRainParticles();
        }

        public void tick(float var1)
        {
            if (mc.renderGlobal != null)
            {
                mc.renderGlobal.tick(mc.renderViewEntity, var1);
            }
        }

        public void getMouseOver(float var1)
        {
            if (mc.renderViewEntity != null)
            {
                if (mc.theWorld != null)
                {
                    double var2 = (double)mc.playerController.getBlockReachDistance();
                    mc.objectMouseOver = mc.renderViewEntity.rayTrace(var2, var1);
                    double var4 = var2;
                    Vec3D var6 = mc.renderViewEntity.getPosition(var1);
                    if (mc.objectMouseOver != null)
                    {
                        var4 = mc.objectMouseOver.hitVec.distanceTo(var6);
                    }

                    if (mc.playerController is PlayerControllerTest)
                    {
                        var2 = 32.0D;
                    }
                    else
                    {
                        if (var4 > 3.0D)
                        {
                            var4 = 3.0D;
                        }

                        var2 = var4;
                    }

                    Vec3D var7 = mc.renderViewEntity.getLook(var1);
                    Vec3D var8 = var6.addVector(var7.xCoord * var2, var7.yCoord * var2, var7.zCoord * var2);
                    pointedEntity = null;
                    float var9 = 1.0F;
                    var var10 = mc.theWorld.getEntitiesWithinAABBExcludingEntity(mc.renderViewEntity, mc.renderViewEntity.boundingBox.stretch(var7.xCoord * var2, var7.yCoord * var2, var7.zCoord * var2).expand((double)var9, (double)var9, (double)var9));
                    double var11 = 0.0D;

                    for (int var13 = 0; var13 < var10.Count; ++var13)
                    {
                        Entity var14 = var10[var13];
                        if (var14.canBeCollidedWith())
                        {
                            float var15 = var14.getCollisionBorderSize();
                            Box var16 = var14.boundingBox.expand((double)var15, (double)var15, (double)var15);
                            MovingObjectPosition var17 = var16.raycast(var6, var8);
                            if (var16.contains(var6))
                            {
                                if (0.0D < var11 || var11 == 0.0D)
                                {
                                    pointedEntity = var14;
                                    var11 = 0.0D;
                                }
                            }
                            else if (var17 != null)
                            {
                                double var18 = var6.distanceTo(var17.hitVec);
                                if (var18 < var11 || var11 == 0.0D)
                                {
                                    pointedEntity = var14;
                                    var11 = var18;
                                }
                            }
                        }
                    }

                    if (pointedEntity != null && !(mc.playerController is PlayerControllerTest))
                    {
                        mc.objectMouseOver = new MovingObjectPosition(pointedEntity);
                    }

                }
            }
        }

        private float getFOVModifier(float var1)
        {
            EntityLiving var2 = mc.renderViewEntity;
            float var3 = 70.0F;
            if (var2.isInsideOfMaterial(Material.WATER))
            {
                var3 = 60.0F;
            }

            if (var2.health <= 0)
            {
                float var4 = (float)var2.deathTime + var1;
                var3 /= (1.0F - 500.0F / (var4 + 500.0F)) * 2.0F + 1.0F;
            }

            return var3 + field_22221_y + (field_22222_x - field_22221_y) * var1;
        }

        private void hurtCameraEffect(float var1)
        {
            EntityLiving var2 = mc.renderViewEntity;
            float var3 = (float)var2.hurtTime - var1;
            float var4;
            if (var2.health <= 0)
            {
                var4 = (float)var2.deathTime + var1;
                GLManager.GL.Rotate(40.0F - 8000.0F / (var4 + 200.0F), 0.0F, 0.0F, 1.0F);
            }

            if (var3 >= 0.0F)
            {
                var3 /= (float)var2.maxHurtTime;
                var3 = MathHelper.sin(var3 * var3 * var3 * var3 * (float)java.lang.Math.PI);
                var4 = var2.attackedAtYaw;
                GLManager.GL.Rotate(-var4, 0.0F, 1.0F, 0.0F);
                GLManager.GL.Rotate(-var3 * 14.0F, 0.0F, 0.0F, 1.0F);
                GLManager.GL.Rotate(var4, 0.0F, 1.0F, 0.0F);
            }
        }

        private void setupViewBobbing(float var1)
        {
            if (mc.renderViewEntity is EntityPlayer)
            {
                EntityPlayer var2 = (EntityPlayer)mc.renderViewEntity;
                float var3 = var2.distanceWalkedModified - var2.prevDistanceWalkedModified;
                float var4 = -(var2.distanceWalkedModified + var3 * var1);
                float var5 = var2.field_775_e + (var2.field_774_f - var2.field_775_e) * var1;
                float var6 = var2.cameraPitch + (var2.field_9328_R - var2.cameraPitch) * var1;
                GLManager.GL.Translate(MathHelper.sin(var4 * (float)java.lang.Math.PI) * var5 * 0.5F, -java.lang.Math.abs(MathHelper.cos(var4 * (float)java.lang.Math.PI) * var5), 0.0F);
                GLManager.GL.Rotate(MathHelper.sin(var4 * (float)java.lang.Math.PI) * var5 * 3.0F, 0.0F, 0.0F, 1.0F);
                GLManager.GL.Rotate(java.lang.Math.abs(MathHelper.cos(var4 * (float)java.lang.Math.PI - 0.2F) * var5) * 5.0F, 1.0F, 0.0F, 0.0F);
                GLManager.GL.Rotate(var6, 1.0F, 0.0F, 0.0F);
            }
        }

        private void orientCamera(float var1)
        {
            EntityLiving var2 = mc.renderViewEntity;
            float var3 = var2.yOffset - 1.62F;
            double var4 = var2.prevPosX + (var2.posX - var2.prevPosX) * (double)var1;
            double var6 = var2.prevPosY + (var2.posY - var2.prevPosY) * (double)var1 - (double)var3;
            double var8 = var2.prevPosZ + (var2.posZ - var2.prevPosZ) * (double)var1;
            GLManager.GL.Rotate(field_22230_A + (field_22220_z - field_22230_A) * var1, 0.0F, 0.0F, 1.0F);
            if (var2.isPlayerSleeping())
            {
                var3 = (float)((double)var3 + 1.0D);
                GLManager.GL.Translate(0.0F, 0.3F, 0.0F);
                if (!mc.gameSettings.field_22273_E)
                {
                    int var10 = mc.theWorld.getBlockId(MathHelper.floor_double(var2.posX), MathHelper.floor_double(var2.posY), MathHelper.floor_double(var2.posZ));
                    if (var10 == Block.BED.id)
                    {
                        int var11 = mc.theWorld.getBlockMeta(MathHelper.floor_double(var2.posX), MathHelper.floor_double(var2.posY), MathHelper.floor_double(var2.posZ));
                        int var12 = var11 & 3;
                        GLManager.GL.Rotate((float)(var12 * 90), 0.0F, 1.0F, 0.0F);
                    }

                    GLManager.GL.Rotate(var2.prevRotationYaw + (var2.rotationYaw - var2.prevRotationYaw) * var1 + 180.0F, 0.0F, -1.0F, 0.0F);
                    GLManager.GL.Rotate(var2.prevRotationPitch + (var2.rotationPitch - var2.prevRotationPitch) * var1, -1.0F, 0.0F, 0.0F);
                }
            }
            else if (mc.gameSettings.thirdPersonView)
            {
                double var27 = (double)(field_22227_s + (field_22228_r - field_22227_s) * var1);
                float var13;
                float var28;
                if (mc.gameSettings.field_22273_E)
                {
                    var28 = field_22225_u + (field_22226_t - field_22225_u) * var1;
                    var13 = field_22223_w + (field_22224_v - field_22223_w) * var1;
                    GLManager.GL.Translate(0.0F, 0.0F, (float)(-var27));
                    GLManager.GL.Rotate(var13, 1.0F, 0.0F, 0.0F);
                    GLManager.GL.Rotate(var28, 0.0F, 1.0F, 0.0F);
                }
                else
                {
                    var28 = var2.rotationYaw;
                    var13 = var2.rotationPitch;
                    double var14 = (double)(-MathHelper.sin(var28 / 180.0F * (float)java.lang.Math.PI) * MathHelper.cos(var13 / 180.0F * (float)java.lang.Math.PI)) * var27;
                    double var16 = (double)(MathHelper.cos(var28 / 180.0F * (float)java.lang.Math.PI) * MathHelper.cos(var13 / 180.0F * (float)java.lang.Math.PI)) * var27;
                    double var18 = (double)(-MathHelper.sin(var13 / 180.0F * (float)java.lang.Math.PI)) * var27;

                    for (int var20 = 0; var20 < 8; ++var20)
                    {
                        float var21 = (float)((var20 & 1) * 2 - 1);
                        float var22 = (float)((var20 >> 1 & 1) * 2 - 1);
                        float var23 = (float)((var20 >> 2 & 1) * 2 - 1);
                        var21 *= 0.1F;
                        var22 *= 0.1F;
                        var23 *= 0.1F;
                        MovingObjectPosition var24 = mc.theWorld.rayTraceBlocks(Vec3D.createVector(var4 + (double)var21, var6 + (double)var22, var8 + (double)var23), Vec3D.createVector(var4 - var14 + (double)var21 + (double)var23, var6 - var18 + (double)var22, var8 - var16 + (double)var23));
                        if (var24 != null)
                        {
                            double var25 = var24.hitVec.distanceTo(Vec3D.createVector(var4, var6, var8));
                            if (var25 < var27)
                            {
                                var27 = var25;
                            }
                        }
                    }

                    GLManager.GL.Rotate(var2.rotationPitch - var13, 1.0F, 0.0F, 0.0F);
                    GLManager.GL.Rotate(var2.rotationYaw - var28, 0.0F, 1.0F, 0.0F);
                    GLManager.GL.Translate(0.0F, 0.0F, (float)(-var27));
                    GLManager.GL.Rotate(var28 - var2.rotationYaw, 0.0F, 1.0F, 0.0F);
                    GLManager.GL.Rotate(var13 - var2.rotationPitch, 1.0F, 0.0F, 0.0F);
                }
            }
            else
            {
                GLManager.GL.Translate(0.0F, 0.0F, -0.1F);
            }

            if (!mc.gameSettings.field_22273_E)
            {
                GLManager.GL.Rotate(var2.prevRotationPitch + (var2.rotationPitch - var2.prevRotationPitch) * var1, 1.0F, 0.0F, 0.0F);
                GLManager.GL.Rotate(var2.prevRotationYaw + (var2.rotationYaw - var2.prevRotationYaw) * var1 + 180.0F, 0.0F, 1.0F, 0.0F);
            }

            GLManager.GL.Translate(0.0F, var3, 0.0F);
            var4 = var2.prevPosX + (var2.posX - var2.prevPosX) * (double)var1;
            var6 = var2.prevPosY + (var2.posY - var2.prevPosY) * (double)var1 - (double)var3;
            var8 = var2.prevPosZ + (var2.posZ - var2.prevPosZ) * (double)var1;
        }

        private void setupCameraTransform(float var1)
        {
            farPlaneDistance = (float)(256 >> mc.gameSettings.renderDistance);
            GLManager.GL.MatrixMode(GLEnum.Projection);
            GLManager.GL.LoadIdentity();

            if (cameraZoom != 1.0D)
            {
                GLManager.GL.Translate((float)cameraYaw, (float)(-cameraPitch), 0.0F);
                GLManager.GL.Scale(cameraZoom, cameraZoom, 1.0D);
                GLU.gluPerspective(getFOVModifier(var1), (float)mc.displayWidth / (float)mc.displayHeight, 0.05F, farPlaneDistance * 2.0F);
            }
            else
            {
                GLU.gluPerspective(getFOVModifier(var1), (float)mc.displayWidth / (float)mc.displayHeight, 0.05F, farPlaneDistance * 2.0F);
            }

            GLManager.GL.MatrixMode(GLEnum.Modelview);
            GLManager.GL.LoadIdentity();

            hurtCameraEffect(var1);
            if (mc.gameSettings.viewBobbing)
            {
                setupViewBobbing(var1);
            }

            float var4 = mc.thePlayer.prevTimeInPortal + (mc.thePlayer.timeInPortal - mc.thePlayer.prevTimeInPortal) * var1;
            if (var4 > 0.0F)
            {
                float var5 = 5.0F / (var4 * var4 + 5.0F) - var4 * 0.04F;
                var5 *= var5;
                GLManager.GL.Rotate(((float)rendererUpdateCount + var1) * 20.0F, 0.0F, 1.0F, 1.0F);
                GLManager.GL.Scale(1.0F / var5, 1.0F, 1.0F);
                GLManager.GL.Rotate(-((float)rendererUpdateCount + var1) * 20.0F, 0.0F, 1.0F, 1.0F);
            }

            orientCamera(var1);
        }

        private void func_4135_b(float var1)
        {
            GLManager.GL.LoadIdentity();

            GLManager.GL.PushMatrix();
            hurtCameraEffect(var1);
            if (mc.gameSettings.viewBobbing)
            {
                setupViewBobbing(var1);
            }

            if (!mc.gameSettings.thirdPersonView && !mc.renderViewEntity.isPlayerSleeping() && !mc.gameSettings.hideGUI)
            {
                itemRenderer.renderItemInFirstPerson(var1);
            }

            GLManager.GL.PopMatrix();
            if (!mc.gameSettings.thirdPersonView && !mc.renderViewEntity.isPlayerSleeping())
            {
                itemRenderer.renderOverlays(var1);
                hurtCameraEffect(var1);
            }

            if (mc.gameSettings.viewBobbing)
            {
                setupViewBobbing(var1);
            }

        }

        public void updateCameraAndRender(float var1)
        {
            if (!Display.isActive())
            {
                if (java.lang.System.currentTimeMillis() - prevFrameTime > 500L)
                {
                    mc.displayInGameMenu();
                }
            }
            else
            {
                prevFrameTime = java.lang.System.currentTimeMillis();
            }

            if (mc.inGameHasFocus)
            {
                mc.mouseHelper.mouseXYChange();
                float var2 = mc.gameSettings.mouseSensitivity * 0.6F + 0.2F;
                float var3 = var2 * var2 * var2 * 8.0F;
                float var4 = (float)mc.mouseHelper.deltaX * var3;
                float var5 = (float)mc.mouseHelper.deltaY * var3;
                //we flip var6 because something is funky with the controls/mouse
                int var6 = -1;
                if (mc.gameSettings.invertMouse)
                {
                    var6 = 1;
                }

                if (mc.gameSettings.smoothCamera)
                {
                    var4 = mouseFilterXAxis.func_22386_a(var4, 0.05F * var3);
                    var5 = mouseFilterYAxis.func_22386_a(var5, 0.05F * var3);
                }

                mc.thePlayer.func_346_d(var4, var5 * (float)var6);
            }

            if (!mc.skipRenderWorld)
            {
                ScaledResolution var13 = new ScaledResolution(mc.gameSettings, mc.displayWidth, mc.displayHeight);
                int var14 = var13.getScaledWidth();
                int var15 = var13.getScaledHeight();
                int var16 = Mouse.getX() * var14 / mc.displayWidth;
                int var17 = var15 - Mouse.getY() * var15 / mc.displayHeight - 1;
                short var7 = 200;
                if (mc.gameSettings.limitFramerate == 1)
                {
                    var7 = 120;
                }

                if (mc.gameSettings.limitFramerate == 2)
                {
                    var7 = 40;
                }

                long var8;
                if (mc.theWorld != null)
                {
                    Profiler.PushGroup("renderWorld");
                    if (mc.gameSettings.limitFramerate == 0)
                    {
                        renderWorld(var1, 0L);
                    }
                    else
                    {
                        renderWorld(var1, field_28133_I + (long)(1000000000 / var7));
                    }
                    Profiler.PopGroup();

                    if (mc.gameSettings.limitFramerate == 2)
                    {
                        var8 = (field_28133_I + (long)(1000000000 / var7) - java.lang.System.nanoTime()) / 1000000L;
                        if (var8 > 0L && var8 < 500L)
                        {
                            try
                            {
                                java.lang.Thread.sleep(var8);
                            }
                            catch (java.lang.InterruptedException var12)
                            {
                                var12.printStackTrace();
                            }
                        }
                    }

                    Profiler.Start("renderGameOverlay");
                    field_28133_I = java.lang.System.nanoTime();
                    if (!mc.gameSettings.hideGUI || mc.currentScreen != null)
                    {
                        mc.ingameGUI.renderGameOverlay(var1, mc.currentScreen != null, var16, var17);
                    }
                    Profiler.Stop("renderGameOverlay");
                }
                else
                {
                    GLManager.GL.Viewport(0, 0, (uint)mc.displayWidth, (uint)mc.displayHeight);
                    GLManager.GL.MatrixMode(GLEnum.Projection);
                    GLManager.GL.LoadIdentity();
                    GLManager.GL.MatrixMode(GLEnum.Modelview);
                    GLManager.GL.LoadIdentity();
                    func_905_b();
                    if (mc.gameSettings.limitFramerate == 2)
                    {
                        var8 = (field_28133_I + (long)(1000000000 / var7) - java.lang.System.nanoTime()) / 1000000L;
                        if (var8 < 0L)
                        {
                            var8 += 10L;
                        }

                        if (var8 > 0L && var8 < 500L)
                        {
                            try
                            {
                                java.lang.Thread.sleep(var8);
                            }
                            catch (java.lang.InterruptedException var11)
                            {
                                var11.printStackTrace();
                            }
                        }
                    }

                    field_28133_I = java.lang.System.nanoTime();
                }

                if (mc.currentScreen != null)
                {
                    GLManager.GL.Clear(ClearBufferMask.DepthBufferBit);
                    mc.currentScreen.drawScreen(var16, var17, var1);
                    if (mc.currentScreen != null && mc.currentScreen.field_25091_h != null)
                    {
                        mc.currentScreen.field_25091_h.func_25087_a(var1);
                    }
                }

            }
        }

        public void renderWorld(float var1, long var2)
        {
            GLManager.GL.Enable(GLEnum.CullFace);
            GLManager.GL.Enable(GLEnum.DepthTest);
            if (mc.renderViewEntity == null)
            {
                mc.renderViewEntity = mc.thePlayer;
            }

            Profiler.Start("getMouseOver");
            getMouseOver(var1);
            Profiler.Stop("getMouseOver");

            EntityLiving var4 = mc.renderViewEntity;
            RenderGlobal var5 = mc.renderGlobal;
            EffectRenderer var6 = mc.effectRenderer;
            double var7 = var4.lastTickPosX + (var4.posX - var4.lastTickPosX) * (double)var1;
            double var9 = var4.lastTickPosY + (var4.posY - var4.lastTickPosY) * (double)var1;
            double var11 = var4.lastTickPosZ + (var4.posZ - var4.lastTickPosZ) * (double)var1;
            IChunkProvider var13 = mc.theWorld.getIChunkProvider();
            int var16;
            if (var13 is ChunkProviderLoadOrGenerate)
            {
                ChunkProviderLoadOrGenerate var14 = (ChunkProviderLoadOrGenerate)var13;
                int var15 = MathHelper.floor_float((float)((int)var7)) >> 4;
                var16 = MathHelper.floor_float((float)((int)var11)) >> 4;
                var14.setCurrentChunkOver(var15, var16);
            }

            Profiler.Start("updateFog");
            GLManager.GL.Viewport(0, 0, (uint)mc.displayWidth, (uint)mc.displayHeight);
            updateFogColor(var1);
            Profiler.Stop("updateFog");
            GLManager.GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            GLManager.GL.Enable(GLEnum.CullFace);
            setupCameraTransform(var1);
            ClippingHelperImpl.getInstance();
            if (mc.gameSettings.renderDistance < 2)
            {
                setupFog(-1, var1);
                var5.renderSky(var1);
            }

            GLManager.GL.Enable(GLEnum.Fog);
            setupFog(1, var1);

            Frustrum var19 = new();
            var19.setPosition(var7, var9, var11);

            setupFog(0, var1);
            GLManager.GL.Enable(GLEnum.Fog);
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.renderEngine.getTexture("/terrain.png"));
            RenderHelper.disableStandardItemLighting();

            Profiler.Start("sortAndRender");
            var5.sortAndRender(var4, 0, (double)var1, var19);
            Profiler.Stop("sortAndRender");

            GLManager.GL.ShadeModel(GLEnum.Flat);
            RenderHelper.enableStandardItemLighting();

            Profiler.Start("renderEntities");
            var5.renderEntities(var4.getPosition(var1), var19, var1);
            Profiler.Stop("renderEntities");

            var6.func_1187_b(var4, var1);

            RenderHelper.disableStandardItemLighting();
            setupFog(0, var1);

            Profiler.Start("renderParticles");
            var6.renderParticles(var4, var1);
            Profiler.Stop("renderParticles");

            EntityPlayer var21;
            if (mc.objectMouseOver != null && var4.isInsideOfMaterial(Material.WATER) && var4 is EntityPlayer)
            {
                var21 = (EntityPlayer)var4;
                GLManager.GL.Disable(GLEnum.AlphaTest);
                var5.drawBlockBreaking(var21, mc.objectMouseOver, 0, var21.inventory.getCurrentItem(), var1);
                var5.drawSelectionBox(var21, mc.objectMouseOver, 0, var21.inventory.getCurrentItem(), var1);
                GLManager.GL.Enable(GLEnum.AlphaTest);
            }

            GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
            setupFog(0, var1);
            GLManager.GL.Enable(GLEnum.Blend);
            GLManager.GL.Disable(GLEnum.CullFace);
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.renderEngine.getTexture("/terrain.png"));

            Profiler.Start("sortAndRender2");

            var5.sortAndRender(var4, 1, var1, var19);

            GLManager.GL.ShadeModel(GLEnum.Flat);

            Profiler.Stop("sortAndRender2");

            //TODO: SELCTION BOX/BLOCK BREAKING VISUALIZATON DON'T APPEAR PROPERLY MOST OF THE TIME, SAME WITH ENTITY SHADOWS. VIEW BOBBING MAKES ENTITES BOB UP AND DOWN

            GLManager.GL.DepthMask(true);
            GLManager.GL.Enable(GLEnum.CullFace);
            GLManager.GL.Disable(GLEnum.Blend);
            if (cameraZoom == 1.0D && var4 is EntityPlayer && mc.objectMouseOver != null && !var4.isInsideOfMaterial(Material.WATER))
            {
                var21 = (EntityPlayer)var4;
                GLManager.GL.Disable(GLEnum.AlphaTest);
                var5.drawBlockBreaking(var21, mc.objectMouseOver, 0, var21.inventory.getCurrentItem(), var1);
                var5.drawSelectionBox(var21, mc.objectMouseOver, 0, var21.inventory.getCurrentItem(), var1);
                GLManager.GL.Enable(GLEnum.AlphaTest);
            }

            renderRainSnow(var1);
            GLManager.GL.Disable(GLEnum.Fog);
            if (pointedEntity != null)
            {
            }

            setupFog(0, var1);
            GLManager.GL.Enable(GLEnum.Fog);
            var5.renderClouds(var1);
            GLManager.GL.Disable(GLEnum.Fog);
            setupFog(1, var1);
            if (cameraZoom == 1.0D)
            {
                GLManager.GL.Clear(ClearBufferMask.DepthBufferBit);
                func_4135_b(var1);
            }
        }

        private void addRainParticles()
        {
            float var1 = mc.theWorld.func_27162_g(1.0F);

            if (var1 != 0.0F)
            {
                random.setSeed((long)rendererUpdateCount * 312987231L);
                EntityLiving var2 = mc.renderViewEntity;
                World var3 = mc.theWorld;
                int var4 = MathHelper.floor_double(var2.posX);
                int var5 = MathHelper.floor_double(var2.posY);
                int var6 = MathHelper.floor_double(var2.posZ);
                byte var7 = 10;
                double var8 = 0.0D;
                double var10 = 0.0D;
                double var12 = 0.0D;
                int var14 = 0;

                for (int var15 = 0; var15 < (int)(100.0F * var1 * var1); ++var15)
                {
                    int var16 = var4 + random.nextInt(var7) - random.nextInt(var7);
                    int var17 = var6 + random.nextInt(var7) - random.nextInt(var7);
                    int var18 = var3.findTopSolidBlock(var16, var17);
                    int var19 = var3.getBlockId(var16, var18 - 1, var17);
                    if (var18 <= var5 + var7 && var18 >= var5 - var7 && var3.getBiomeSource().getBiome(var16, var17).canSpawnLightningBolt())
                    {
                        float var20 = random.nextFloat();
                        float var21 = random.nextFloat();
                        if (var19 > 0)
                        {
                            if (Block.BLOCKS[var19].material == Material.LAVA)
                            {
                                mc.effectRenderer.addEffect(new EntitySmokeFX(var3, (double)((float)var16 + var20), (double)((float)var18 + 0.1F) - Block.BLOCKS[var19].minY, (double)((float)var17 + var21), 0.0D, 0.0D, 0.0D));
                            }
                            else
                            {
                                ++var14;
                                if (random.nextInt(var14) == 0)
                                {
                                    var8 = (double)((float)var16 + var20);
                                    var10 = (double)((float)var18 + 0.1F) - Block.BLOCKS[var19].minY;
                                    var12 = (double)((float)var17 + var21);
                                }

                                mc.effectRenderer.addEffect(new EntityRainFX(var3, (double)((float)var16 + var20), (double)((float)var18 + 0.1F) - Block.BLOCKS[var19].minY, (double)((float)var17 + var21)));
                            }
                        }
                    }
                }

                if (var14 > 0 && random.nextInt(3) < rainSoundCounter++)
                {
                    rainSoundCounter = 0;
                    if (var10 > var2.posY + 1.0D && var3.findTopSolidBlock(MathHelper.floor_double(var2.posX), MathHelper.floor_double(var2.posZ)) > MathHelper.floor_double(var2.posY))
                    {
                        mc.theWorld.playSoundEffect(var8, var10, var12, "ambient.weather.rain", 0.1F, 0.5F);
                    }
                    else
                    {
                        mc.theWorld.playSoundEffect(var8, var10, var12, "ambient.weather.rain", 0.2F, 1.0F);
                    }
                }

            }
        }

        protected void renderRainSnow(float var1)
        {
            float var2 = mc.theWorld.func_27162_g(var1);
            if (var2 > 0.0F)
            {
                EntityLiving var3 = mc.renderViewEntity;
                World var4 = mc.theWorld;
                int var5 = MathHelper.floor_double(var3.posX);
                int var6 = MathHelper.floor_double(var3.posY);
                int var7 = MathHelper.floor_double(var3.posZ);
                Tessellator var8 = Tessellator.instance;
                GLManager.GL.Disable(GLEnum.CullFace);
                GLManager.GL.Normal3(0.0F, 1.0F, 0.0F);
                GLManager.GL.Enable(GLEnum.Blend);
                GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
                GLManager.GL.AlphaFunc(GLEnum.Greater, 0.01F);
                GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.renderEngine.getTexture("/environment/snow.png"));
                double var9 = var3.lastTickPosX + (var3.posX - var3.lastTickPosX) * (double)var1;
                double var11 = var3.lastTickPosY + (var3.posY - var3.lastTickPosY) * (double)var1;
                double var13 = var3.lastTickPosZ + (var3.posZ - var3.lastTickPosZ) * (double)var1;
                int var15 = MathHelper.floor_double(var11);
                byte var16 = 10;

                Biome[] var17 = var4.getBiomeSource().getBiomesInArea(var5 - var16, var7 - var16, var16 * 2 + 1, var16 * 2 + 1);
                int var18 = 0;

                int var19;
                int var20;
                Biome var21;
                int var22;
                int var23;
                int var24;
                float var26;
                for (var19 = var5 - var16; var19 <= var5 + var16; ++var19)
                {
                    for (var20 = var7 - var16; var20 <= var7 + var16; ++var20)
                    {
                        var21 = var17[var18++];
                        if (var21.getEnableSnow())
                        {
                            var22 = var4.findTopSolidBlock(var19, var20);
                            if (var22 < 0)
                            {
                                var22 = 0;
                            }

                            var23 = var22;
                            if (var22 < var15)
                            {
                                var23 = var15;
                            }

                            var24 = var6 - var16;
                            int var25 = var6 + var16;
                            if (var24 < var22)
                            {
                                var24 = var22;
                            }

                            if (var25 < var22)
                            {
                                var25 = var22;
                            }

                            var26 = 1.0F;
                            if (var24 != var25)
                            {
                                random.setSeed((long)(var19 * var19 * 3121 + var19 * 45238971 + var20 * var20 * 418711 + var20 * 13761));
                                float var27 = (float)rendererUpdateCount + var1;
                                float var28 = ((float)(rendererUpdateCount & 511) + var1) / 512.0F;
                                float var29 = random.nextFloat() + var27 * 0.01F * (float)random.nextGaussian();
                                float var30 = random.nextFloat() + var27 * (float)random.nextGaussian() * 0.001F;
                                double var31 = (double)((float)var19 + 0.5F) - var3.posX;
                                double var33 = (double)((float)var20 + 0.5F) - var3.posZ;
                                float var35 = MathHelper.sqrt_double(var31 * var31 + var33 * var33) / (float)var16;
                                var8.startDrawingQuads();
                                float var36 = var4.getLuminance(var19, var23, var20);
                                GLManager.GL.Color4(var36, var36, var36, ((1.0F - var35 * var35) * 0.3F + 0.5F) * var2);
                                var8.setTranslationD(-var9 * 1.0D, -var11 * 1.0D, -var13 * 1.0D);
                                var8.addVertexWithUV((double)(var19 + 0), (double)var24, (double)var20 + 0.5D, (double)(0.0F * var26 + var29), (double)((float)var24 * var26 / 4.0F + var28 * var26 + var30));
                                var8.addVertexWithUV((double)(var19 + 1), (double)var24, (double)var20 + 0.5D, (double)(1.0F * var26 + var29), (double)((float)var24 * var26 / 4.0F + var28 * var26 + var30));
                                var8.addVertexWithUV((double)(var19 + 1), (double)var25, (double)var20 + 0.5D, (double)(1.0F * var26 + var29), (double)((float)var25 * var26 / 4.0F + var28 * var26 + var30));
                                var8.addVertexWithUV((double)(var19 + 0), (double)var25, (double)var20 + 0.5D, (double)(0.0F * var26 + var29), (double)((float)var25 * var26 / 4.0F + var28 * var26 + var30));
                                var8.addVertexWithUV((double)var19 + 0.5D, (double)var24, (double)(var20 + 0), (double)(0.0F * var26 + var29), (double)((float)var24 * var26 / 4.0F + var28 * var26 + var30));
                                var8.addVertexWithUV((double)var19 + 0.5D, (double)var24, (double)(var20 + 1), (double)(1.0F * var26 + var29), (double)((float)var24 * var26 / 4.0F + var28 * var26 + var30));
                                var8.addVertexWithUV((double)var19 + 0.5D, (double)var25, (double)(var20 + 1), (double)(1.0F * var26 + var29), (double)((float)var25 * var26 / 4.0F + var28 * var26 + var30));
                                var8.addVertexWithUV((double)var19 + 0.5D, (double)var25, (double)(var20 + 0), (double)(0.0F * var26 + var29), (double)((float)var25 * var26 / 4.0F + var28 * var26 + var30));
                                var8.setTranslationD(0.0D, 0.0D, 0.0D);
                                var8.draw();
                            }
                        }
                    }
                }

                GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.renderEngine.getTexture("/environment/rain.png"));
                var16 = 10;

                var18 = 0;

                for (var19 = var5 - var16; var19 <= var5 + var16; ++var19)
                {
                    for (var20 = var7 - var16; var20 <= var7 + var16; ++var20)
                    {
                        var21 = var17[var18++];
                        if (var21.canSpawnLightningBolt())
                        {
                            var22 = var4.findTopSolidBlock(var19, var20);
                            var23 = var6 - var16;
                            var24 = var6 + var16;
                            if (var23 < var22)
                            {
                                var23 = var22;
                            }

                            if (var24 < var22)
                            {
                                var24 = var22;
                            }

                            float var37 = 1.0F;
                            if (var23 != var24)
                            {
                                random.setSeed((long)(var19 * var19 * 3121 + var19 * 45238971 + var20 * var20 * 418711 + var20 * 13761));
                                var26 = ((float)(rendererUpdateCount + var19 * var19 * 3121 + var19 * 45238971 + var20 * var20 * 418711 + var20 * 13761 & 31) + var1) / 32.0F * (3.0F + random.nextFloat());
                                double var38 = (double)((float)var19 + 0.5F) - var3.posX;
                                double var39 = (double)((float)var20 + 0.5F) - var3.posZ;
                                float var40 = MathHelper.sqrt_double(var38 * var38 + var39 * var39) / (float)var16;
                                var8.startDrawingQuads();
                                float var32 = var4.getLuminance(var19, 128, var20) * 0.85F + 0.15F;
                                GLManager.GL.Color4(var32, var32, var32, ((1.0F - var40 * var40) * 0.5F + 0.5F) * var2);
                                var8.setTranslationD(-var9 * 1.0D, -var11 * 1.0D, -var13 * 1.0D);
                                var8.addVertexWithUV((double)(var19 + 0), (double)var23, (double)var20 + 0.5D, (double)(0.0F * var37), (double)((float)var23 * var37 / 4.0F + var26 * var37));
                                var8.addVertexWithUV((double)(var19 + 1), (double)var23, (double)var20 + 0.5D, (double)(1.0F * var37), (double)((float)var23 * var37 / 4.0F + var26 * var37));
                                var8.addVertexWithUV((double)(var19 + 1), (double)var24, (double)var20 + 0.5D, (double)(1.0F * var37), (double)((float)var24 * var37 / 4.0F + var26 * var37));
                                var8.addVertexWithUV((double)(var19 + 0), (double)var24, (double)var20 + 0.5D, (double)(0.0F * var37), (double)((float)var24 * var37 / 4.0F + var26 * var37));
                                var8.addVertexWithUV((double)var19 + 0.5D, (double)var23, (double)(var20 + 0), (double)(0.0F * var37), (double)((float)var23 * var37 / 4.0F + var26 * var37));
                                var8.addVertexWithUV((double)var19 + 0.5D, (double)var23, (double)(var20 + 1), (double)(1.0F * var37), (double)((float)var23 * var37 / 4.0F + var26 * var37));
                                var8.addVertexWithUV((double)var19 + 0.5D, (double)var24, (double)(var20 + 1), (double)(1.0F * var37), (double)((float)var24 * var37 / 4.0F + var26 * var37));
                                var8.addVertexWithUV((double)var19 + 0.5D, (double)var24, (double)(var20 + 0), (double)(0.0F * var37), (double)((float)var24 * var37 / 4.0F + var26 * var37));
                                var8.setTranslationD(0.0D, 0.0D, 0.0D);
                                var8.draw();
                            }
                        }
                    }
                }

                GLManager.GL.Enable(GLEnum.CullFace);
                GLManager.GL.Disable(GLEnum.Blend);
                GLManager.GL.AlphaFunc(GLEnum.Greater, 0.1F);
            }
        }

        public void func_905_b()
        {
            ScaledResolution var1 = new ScaledResolution(mc.gameSettings, mc.displayWidth, mc.displayHeight);
            GLManager.GL.Clear(ClearBufferMask.DepthBufferBit);
            GLManager.GL.MatrixMode(GLEnum.Projection);
            GLManager.GL.LoadIdentity();
            GLManager.GL.Ortho(0.0D, var1.field_25121_a, var1.field_25120_b, 0.0D, 1000.0D, 3000.0D);
            GLManager.GL.MatrixMode(GLEnum.Modelview);
            GLManager.GL.LoadIdentity();
            GLManager.GL.Translate(0.0F, 0.0F, -2000.0F);
        }

        private void updateFogColor(float var1)
        {
            World var2 = mc.theWorld;
            EntityLiving var3 = mc.renderViewEntity;
            float var4 = 1.0F / (float)(4 - mc.gameSettings.renderDistance);
            var4 = 1.0F - (float)java.lang.Math.pow((double)var4, 0.25D);
            Vector3D<double> var5 = var2.func_4079_a(mc.renderViewEntity, var1);
            float var6 = (float)var5.X;
            float var7 = (float)var5.Y;
            float var8 = (float)var5.Z;
            Vector3D<double> var9 = var2.getFogColor(var1);
            fogColorRed = (float)var9.X;
            fogColorGreen = (float)var9.Y;
            fogColorBlue = (float)var9.Z;
            fogColorRed += (var6 - fogColorRed) * var4;
            fogColorGreen += (var7 - fogColorGreen) * var4;
            fogColorBlue += (var8 - fogColorBlue) * var4;
            float var10 = var2.func_27162_g(var1);
            float var11;
            float var12;
            if (var10 > 0.0F)
            {
                var11 = 1.0F - var10 * 0.5F;
                var12 = 1.0F - var10 * 0.4F;
                fogColorRed *= var11;
                fogColorGreen *= var11;
                fogColorBlue *= var12;
            }

            var11 = var2.func_27166_f(var1);
            if (var11 > 0.0F)
            {
                var12 = 1.0F - var11 * 0.5F;
                fogColorRed *= var12;
                fogColorGreen *= var12;
                fogColorBlue *= var12;
            }

            if (cloudFog)
            {
                Vector3D<double> var16 = var2.func_628_d(var1);
                fogColorRed = (float)var16.X;
                fogColorGreen = (float)var16.Y;
                fogColorBlue = (float)var16.Z;
            }
            else if (var3.isInsideOfMaterial(Material.WATER))
            {
                fogColorRed = 0.02F;
                fogColorGreen = 0.02F;
                fogColorBlue = 0.2F;
            }
            else if (var3.isInsideOfMaterial(Material.LAVA))
            {
                fogColorRed = 0.6F;
                fogColorGreen = 0.1F;
                fogColorBlue = 0.0F;
            }

            var12 = fogColor2 + (fogColor1 - fogColor2) * var1;
            fogColorRed *= var12;
            fogColorGreen *= var12;
            fogColorBlue *= var12;

            GLManager.GL.ClearColor(fogColorRed, fogColorGreen, fogColorBlue, 0.0F);
        }

        private void setupFog(int var1, float var2)
        {
            EntityLiving var3 = mc.renderViewEntity;
            GLManager.GL.Fog(GLEnum.FogColor, func_908_a(fogColorRed, fogColorGreen, fogColorBlue, 1.0F));
            mc.renderGlobal.worldRenderer.SetFogColor(fogColorRed, fogColorGreen, fogColorBlue, 1.0f);
            GLManager.GL.Normal3(0.0F, -1.0F, 0.0F);
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            if (cloudFog)
            {
                GLManager.GL.Fog(GLEnum.FogMode, (int)GLEnum.Exp);
                GLManager.GL.Fog(GLEnum.FogDensity, 0.1F);
                mc.renderGlobal.worldRenderer.SetFogMode(1);
                mc.renderGlobal.worldRenderer.SetFogDensity(0.1f);
            }
            else if (var3.isInsideOfMaterial(Material.WATER))
            {
                GLManager.GL.Fog(GLEnum.FogMode, (int)GLEnum.Exp);
                GLManager.GL.Fog(GLEnum.FogDensity, 0.1F);
                mc.renderGlobal.worldRenderer.SetFogMode(1);
                mc.renderGlobal.worldRenderer.SetFogDensity(0.1f);
            }
            else if (var3.isInsideOfMaterial(Material.LAVA))
            {
                GLManager.GL.Fog(GLEnum.FogMode, (int)GLEnum.Exp);
                GLManager.GL.Fog(GLEnum.FogDensity, 2.0F);
                mc.renderGlobal.worldRenderer.SetFogMode(1);
                mc.renderGlobal.worldRenderer.SetFogDensity(2.0f);
            }
            else
            {
                GLManager.GL.Fog(GLEnum.FogMode, (int)GLEnum.Linear);
                GLManager.GL.Fog(GLEnum.FogStart, farPlaneDistance * 0.25F);
                GLManager.GL.Fog(GLEnum.FogEnd, farPlaneDistance);
                mc.renderGlobal.worldRenderer.SetFogMode(0);
                mc.renderGlobal.worldRenderer.SetFogStart(farPlaneDistance * 0.25f);
                mc.renderGlobal.worldRenderer.SetFogEnd(farPlaneDistance);
                if (var1 < 0)
                {
                    GLManager.GL.Fog(GLEnum.FogStart, 0.0F);
                    GLManager.GL.Fog(GLEnum.FogEnd, farPlaneDistance * 0.8F);
                    mc.renderGlobal.worldRenderer.SetFogStart(0.0f);
                    mc.renderGlobal.worldRenderer.SetFogEnd(farPlaneDistance * 0.8f);
                }

                if (mc.theWorld.dimension.isNether)
                {
                    GLManager.GL.Fog(GLEnum.FogStart, 0.0F);
                    mc.renderGlobal.worldRenderer.SetFogStart(0.0f);
                }
            }

            GLManager.GL.Enable(GLEnum.ColorMaterial);
            GLManager.GL.ColorMaterial(GLEnum.Front, GLEnum.Ambient);
        }

        private float[] func_908_a(float var1, float var2, float var3, float var4)
        {
            fogColorBuffer[0] = var1;
            fogColorBuffer[1] = var2;
            fogColorBuffer[2] = var3;
            fogColorBuffer[3] = var4;
            return fogColorBuffer;
        }
    }

}