using System.Diagnostics;
using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;
using BetaSharp.Client.Entities.FX;
using BetaSharp.Client.Input;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Items;
using BetaSharp.Entities;
using BetaSharp.Profiling;
using BetaSharp.Util.Hit;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using BetaSharp.Worlds.Biomes;
using BetaSharp.Worlds.Chunks;
using Silk.NET.Maths;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering;

public class GameRenderer
{
    public static int anaglyphField;
    private readonly Minecraft client;
    private float viewDistane = 0.0F;
    public HeldItemRenderer itemRenderer;
    private int ticks;
    private Entity targetedEntity = null;
    private readonly MouseFilter mouseFilterXAxis = new();
    private readonly MouseFilter mouseFilterYAxis = new();
    private readonly float thirdPersonDistance = 4.0F;
    private float prevThirdPersonDistance = 4.0F;
    private readonly float thirdPersonYaw = 0.0F;
    private float prevThirdPersonYaw = 0.0F;
    private readonly float thirdPersonPitch = 0.0F;
    private float prevThirdPersonPitch = 0.0F;
    private readonly float cameraRoll = 0.0F;
    private float prevCameraRoll = 0.0F;
    private readonly float cameraRollAmount = 0.0F;
    private float prevCameraRollAmount = 0.0F;
    private readonly bool cloudFog = false;
    private readonly double cameraZoom = 1.0D;
    private readonly double cameraYaw = 0.0D;
    private readonly double cameraPitch = 0.0D;
    private long prevFrameTime = java.lang.System.currentTimeMillis();
    private readonly java.util.Random random = new();
    private int rainSoundCounter = 0;
    private readonly float[] fogColorBuffer = new float[16];
    private float fogColorRed;
    private float fogColorGreen;
    private float fogColorBlue;
    private float lastViewBob;
    private float viewBob;
    private readonly Stopwatch fpsTimer = Stopwatch.StartNew();

    public GameRenderer(Minecraft mc)
    {
        client = mc;
        itemRenderer = new HeldItemRenderer(mc);
    }

    public void updateCamera()
    {
        lastViewBob = viewBob;
        prevThirdPersonDistance = thirdPersonDistance;
        prevThirdPersonYaw = thirdPersonYaw;
        prevThirdPersonPitch = thirdPersonPitch;
        prevCameraRoll = cameraRoll;
        prevCameraRollAmount = cameraRollAmount;
        client.camera ??= client.player;

        float var1 = client.world.getLuminance(MathHelper.floor_double(client.camera.x), MathHelper.floor_double(client.camera.y), MathHelper.floor_double(client.camera.z));
        float var2 = (3 - client.options.renderDistance) / 3.0F;
        float var3 = var1 * (1.0F - var2) + var2;
        viewBob += (var3 - viewBob) * 0.1F;
        ++ticks;
        itemRenderer.updateEquippedItem();
        renderRain();
    }

    public void tick(float var1)
    {
        if (client.terrainRenderer != null)
        {
            client.terrainRenderer.tick(client.camera, var1);
        }
    }

    public void updateTargetedEntity(float tickDelta)
    {
        if (client.camera != null)
        {
            if (client.world != null)
            {
                double var2 = (double)client.playerController.getBlockReachDistance();
                client.objectMouseOver = client.camera.rayTrace(var2, tickDelta);
                double var4 = var2;
                Vec3D var6 = client.camera.getPosition(tickDelta);
                if (client.objectMouseOver != null)
                {
                    var4 = client.objectMouseOver.pos.distanceTo(var6);
                }

                if (var4 > 3.0D)
                {
                    var4 = 3.0D;
                }

                var2 = var4;

                Vec3D var7 = client.camera.getLook(tickDelta);
                Vec3D var8 = var6 + var2 * var7;
                targetedEntity = null;
                float var9 = 1.0F;
                var var10 = client.world.getEntities(client.camera, client.camera.boundingBox.stretch(var7.x * var2, var7.y * var2, var7.z * var2).expand((double)var9, (double)var9, (double)var9));
                double var11 = 0.0D;

                for (int var13 = 0; var13 < var10.Count; ++var13)
                {
                    Entity var14 = var10[var13];
                    if (var14.isCollidable())
                    {
                        float var15 = var14.getTargetingMargin();
                        Box var16 = var14.boundingBox.expand((double)var15, (double)var15, (double)var15);
                        HitResult var17 = var16.raycast(var6, var8);
                        if (var16.contains(var6))
                        {
                            if (0.0D < var11 || var11 == 0.0D)
                            {
                                targetedEntity = var14;
                                var11 = 0.0D;
                            }
                        }
                        else if (var17 != null)
                        {
                            double var18 = var6.distanceTo(var17.pos);
                            if (var18 < var11 || var11 == 0.0D)
                            {
                                targetedEntity = var14;
                                var11 = var18;
                            }
                        }
                    }
                }

                if (targetedEntity != null)
                {
                    client.objectMouseOver = new HitResult(targetedEntity);
                }

            }
        }
    }

    private float getFov(float tickDelta, bool isHand = false)
    {
        EntityLiving var2 = client.camera;
        float var3 = isHand ? 70.0F : (30.0F + client.options.fov * 90.0F);
        if (var2.isInFluid(Material.Water))
        {
            var3 = 60.0F;
        }

        if (var2.health <= 0)
        {
            float var4 = var2.deathTime + tickDelta;
            var3 /= (1.0F - 500.0F / (var4 + 500.0F)) * 2.0F + 1.0F;
        }

        return var3 + prevCameraRoll + (cameraRoll - prevCameraRoll) * tickDelta;
    }

    private void applyDamageTiltEffect(float tickDelta)
    {
        EntityLiving var2 = client.camera;
        float var3 = var2.hurtTime - tickDelta;
        float var4;
        if (var2.health <= 0)
        {
            var4 = var2.deathTime + tickDelta;
            GLManager.GL.Rotate(40.0F - 8000.0F / (var4 + 200.0F), 0.0F, 0.0F, 1.0F);
        }

        if (var3 >= 0.0F)
        {
            var3 /= var2.maxHurtTime;
            var3 = MathHelper.sin(var3 * var3 * var3 * var3 * (float)java.lang.Math.PI);
            var4 = var2.attackedAtYaw;
            GLManager.GL.Rotate(-var4, 0.0F, 1.0F, 0.0F);
            GLManager.GL.Rotate(-var3 * 14.0F, 0.0F, 0.0F, 1.0F);
            GLManager.GL.Rotate(var4, 0.0F, 1.0F, 0.0F);
        }
    }

    private void applyViewBobbing(float tickDelta)
    {
        if (client.camera is EntityPlayer)
        {
            EntityPlayer var2 = (EntityPlayer)client.camera;
            float var3 = var2.horizontalSpeed - var2.prevHorizontalSpeed;
            float var4 = -(var2.horizontalSpeed + var3 * tickDelta);
            float var5 = var2.prevStepBobbingAmount + (var2.stepBobbingAmount - var2.prevStepBobbingAmount) * tickDelta;
            float var6 = var2.cameraPitch + (var2.tilt - var2.cameraPitch) * tickDelta;
            GLManager.GL.Translate(MathHelper.sin(var4 * (float)java.lang.Math.PI) * var5 * 0.5F, -java.lang.Math.abs(MathHelper.cos(var4 * (float)java.lang.Math.PI) * var5), 0.0F);
            GLManager.GL.Rotate(MathHelper.sin(var4 * (float)java.lang.Math.PI) * var5 * 3.0F, 0.0F, 0.0F, 1.0F);
            GLManager.GL.Rotate(java.lang.Math.abs(MathHelper.cos(var4 * (float)java.lang.Math.PI - 0.2F) * var5) * 5.0F, 1.0F, 0.0F, 0.0F);
            GLManager.GL.Rotate(var6, 1.0F, 0.0F, 0.0F);
        }
    }

    private void applyCameraTransform(float tickDelta)
    {
        EntityLiving var2 = client.camera;
        float var3 = var2.standingEyeHeight - 1.62F;
        double var4 = var2.prevX + (var2.x - var2.prevX) * (double)tickDelta;
        double var6 = var2.prevY + (var2.y - var2.prevY) * (double)tickDelta - (double)var3;
        double var8 = var2.prevZ + (var2.z - var2.prevZ) * (double)tickDelta;
        GLManager.GL.Rotate(prevCameraRollAmount + (cameraRollAmount - prevCameraRollAmount) * tickDelta, 0.0F, 0.0F, 1.0F);
        if (var2.isSleeping())
        {
            var3 = (float)((double)var3 + 1.0D);
            GLManager.GL.Translate(0.0F, 0.3F, 0.0F);
            if (!client.options.debugCamera)
            {
                int var10 = client.world.getBlockId(MathHelper.floor_double(var2.x), MathHelper.floor_double(var2.y), MathHelper.floor_double(var2.z));
                if (var10 == Block.Bed.id)
                {
                    int var11 = client.world.getBlockMeta(MathHelper.floor_double(var2.x), MathHelper.floor_double(var2.y), MathHelper.floor_double(var2.z));
                    int var12 = var11 & 3;
                    GLManager.GL.Rotate(var12 * 90, 0.0F, 1.0F, 0.0F);
                }

                GLManager.GL.Rotate(var2.prevYaw + (var2.yaw - var2.prevYaw) * tickDelta + 180.0F, 0.0F, -1.0F, 0.0F);
                GLManager.GL.Rotate(var2.prevPitch + (var2.pitch - var2.prevPitch) * tickDelta, -1.0F, 0.0F, 0.0F);
            }
        }
        else if (client.options.thirdPersonView)
        {
            double var27 = (double)(prevThirdPersonDistance + (thirdPersonDistance - prevThirdPersonDistance) * tickDelta);
            float var13;
            float var28;
            if (client.options.debugCamera)
            {
                var28 = prevThirdPersonYaw + (thirdPersonYaw - prevThirdPersonYaw) * tickDelta;
                var13 = prevThirdPersonPitch + (thirdPersonPitch - prevThirdPersonPitch) * tickDelta;
                GLManager.GL.Translate(0.0F, 0.0F, (float)-var27);
                GLManager.GL.Rotate(var13, 1.0F, 0.0F, 0.0F);
                GLManager.GL.Rotate(var28, 0.0F, 1.0F, 0.0F);
            }
            else
            {
                var28 = var2.yaw;
                var13 = var2.pitch;
                double var14 = (double)(-MathHelper.sin(var28 / 180.0F * (float)java.lang.Math.PI) * MathHelper.cos(var13 / 180.0F * (float)java.lang.Math.PI)) * var27;
                double var16 = (double)(MathHelper.cos(var28 / 180.0F * (float)java.lang.Math.PI) * MathHelper.cos(var13 / 180.0F * (float)java.lang.Math.PI)) * var27;
                double var18 = (double)-MathHelper.sin(var13 / 180.0F * (float)java.lang.Math.PI) * var27;

                for (int var20 = 0; var20 < 8; ++var20)
                {
                    float var21 = (var20 & 1) * 2 - 1;
                    float var22 = (var20 >> 1 & 1) * 2 - 1;
                    float var23 = (var20 >> 2 & 1) * 2 - 1;
                    var21 *= 0.1F;
                    var22 *= 0.1F;
                    var23 *= 0.1F;
                    HitResult var24 = client.world.raycast(new Vec3D(var4 + (double)var21, var6 + (double)var22, var8 + (double)var23), new Vec3D(var4 - var14 + (double)var21 + (double)var23, var6 - var18 + (double)var22, var8 - var16 + (double)var23));
                    if (var24 != null)
                    {
                        double var25 = var24.pos.distanceTo(new Vec3D(var4, var6, var8));
                        if (var25 < var27)
                        {
                            var27 = var25;
                        }
                    }
                }

                GLManager.GL.Rotate(var2.pitch - var13, 1.0F, 0.0F, 0.0F);
                GLManager.GL.Rotate(var2.yaw - var28, 0.0F, 1.0F, 0.0F);
                GLManager.GL.Translate(0.0F, 0.0F, (float)-var27);
                GLManager.GL.Rotate(var28 - var2.yaw, 0.0F, 1.0F, 0.0F);
                GLManager.GL.Rotate(var13 - var2.pitch, 1.0F, 0.0F, 0.0F);
            }
        }
        else
        {
            GLManager.GL.Translate(0.0F, 0.0F, -0.1F);
        }

        if (!client.options.debugCamera)
        {
            GLManager.GL.Rotate(var2.prevPitch + (var2.pitch - var2.prevPitch) * tickDelta, 1.0F, 0.0F, 0.0F);
            GLManager.GL.Rotate(var2.prevYaw + (var2.yaw - var2.prevYaw) * tickDelta + 180.0F, 0.0F, 1.0F, 0.0F);
        }

        GLManager.GL.Translate(0.0F, var3, 0.0F);
        var4 = var2.prevX + (var2.x - var2.prevX) * (double)tickDelta;
        var6 = var2.prevY + (var2.y - var2.prevY) * (double)tickDelta - (double)var3;
        var8 = var2.prevZ + (var2.z - var2.prevZ) * (double)tickDelta;
    }

    private void renderWorld(float tickDelta)
    {
        viewDistane = 256 >> client.options.renderDistance;
        GLManager.GL.MatrixMode(GLEnum.Projection);
        GLManager.GL.LoadIdentity();

        if (cameraZoom != 1.0D)
        {
            GLManager.GL.Translate((float)cameraYaw, (float)-cameraPitch, 0.0F);
            GLManager.GL.Scale(cameraZoom, cameraZoom, 1.0D);
            GLU.gluPerspective(getFov(tickDelta), client.displayWidth / (float)client.displayHeight, 0.05F, viewDistane * 2.0F);
        }
        else
        {
            GLU.gluPerspective(getFov(tickDelta), client.displayWidth / (float)client.displayHeight, 0.05F, viewDistane * 2.0F);
        }

        GLManager.GL.MatrixMode(GLEnum.Modelview);
        GLManager.GL.LoadIdentity();

        applyDamageTiltEffect(tickDelta);
        if (client.options.viewBobbing)
        {
            applyViewBobbing(tickDelta);
        }

        float var4 = client.player.lastScreenDistortion + (client.player.changeDimensionCooldown - client.player.lastScreenDistortion) * tickDelta;
        if (var4 > 0.0F)
        {
            float var5 = 5.0F / (var4 * var4 + 5.0F) - var4 * 0.04F;
            var5 *= var5;
            GLManager.GL.Rotate((ticks + tickDelta) * 20.0F, 0.0F, 1.0F, 1.0F);
            GLManager.GL.Scale(1.0F / var5, 1.0F, 1.0F);
            GLManager.GL.Rotate(-(ticks + tickDelta) * 20.0F, 0.0F, 1.0F, 1.0F);
        }

        applyCameraTransform(tickDelta);
    }

    private void renderFirstPersonHand(float tickDelta)
    {
        GLManager.GL.MatrixMode(GLEnum.Projection);
        GLManager.GL.LoadIdentity();
        if (cameraZoom != 1.0D)
        {
            GLManager.GL.Translate((float)cameraYaw, (float)-cameraPitch, 0.0F);
            GLManager.GL.Scale(cameraZoom, cameraZoom, 1.0D);
        }
        GLU.gluPerspective(getFov(tickDelta, true), client.displayWidth / (float)client.displayHeight, 0.05F, viewDistane * 2.0F);
        GLManager.GL.MatrixMode(GLEnum.Modelview);
        GLManager.GL.LoadIdentity();

        GLManager.GL.PushMatrix();
        applyDamageTiltEffect(tickDelta);
        if (client.options.viewBobbing)
        {
            applyViewBobbing(tickDelta);
        }

        if (!client.options.thirdPersonView && !client.camera.isSleeping() && !client.options.hideGUI)
        {
            itemRenderer.renderItemInFirstPerson(tickDelta);
        }

        GLManager.GL.PopMatrix();
        if (!client.options.thirdPersonView && !client.camera.isSleeping())
        {
            itemRenderer.renderOverlays(tickDelta);
            applyDamageTiltEffect(tickDelta);
        }

        if (client.options.viewBobbing)
        {
            applyViewBobbing(tickDelta);
        }

    }

    public void onFrameUpdate(float tickDelta)
    {
        if (!Display.isActive())
        {
            if (java.lang.System.currentTimeMillis() - prevFrameTime > 500L)
            {
                client.displayInGameMenu();
            }
        }
        else
        {
            prevFrameTime = java.lang.System.currentTimeMillis();
        }

        if (client.inGameHasFocus)
        {
            client.mouseHelper.mouseXYChange();
            float var2 = client.options.mouseSensitivity * 0.6F + 0.2F;
            float var3 = var2 * var2 * var2 * 8.0F;
            float var4 = client.mouseHelper.deltaX * var3;
            float var5 = client.mouseHelper.deltaY * var3;
            int var6 = -1;
            if (client.options.invertMouse)
            {
                var6 = 1;
            }
            if (client.options.smoothCamera)
            {
                var4 = mouseFilterXAxis.Smooth(var4, 0.05F * var3);
                var5 = mouseFilterYAxis.Smooth(var5, 0.05F * var3);
            }
            client.player.changeLookDirection(var4, var5 * var6);
        }

        if (!client.skipRenderWorld)
        {
            ScaledResolution var13 = new(client.options, client.displayWidth, client.displayHeight);
            int var14 = var13.ScaledWidth;
            int var15 = var13.ScaledHeight;
            int var16 = Mouse.getX() * var14 / client.displayWidth;
            int var17 = var15 - Mouse.getY() * var15 / client.displayHeight - 1;
            int var7 = 30 + (int)(client.options.limitFramerate * 210.0f);

            if (var7 < 240)
            {
                Display.setVSyncEnabled(false);
            }

            if (client.world != null)
            {
                Profiler.PushGroup("renderWorld");
                renderFrame(tickDelta, 0L);
                Profiler.PopGroup();
                Profiler.Start("renderGameOverlay");
                if (!client.options.hideGUI || client.currentScreen != null)
                {
                    client.ingameGUI.renderGameOverlay(tickDelta, client.currentScreen != null, var16, var17);
                }
                Profiler.Stop("renderGameOverlay");
            }
            else
            {
                GLManager.GL.Viewport(0, 0, (uint)client.displayWidth, (uint)client.displayHeight);
                GLManager.GL.MatrixMode(GLEnum.Projection);
                GLManager.GL.LoadIdentity();
                GLManager.GL.MatrixMode(GLEnum.Modelview);
                GLManager.GL.LoadIdentity();
                setupHudRender();
            }

            if (client.currentScreen != null)
            {
                GLManager.GL.Clear(ClearBufferMask.DepthBufferBit);
                client.currentScreen.Render(var16, var17, tickDelta);
                if (client.currentScreen != null && client.currentScreen.ParticlesGui != null)
                {
                    client.currentScreen.ParticlesGui.render(tickDelta);
                }
            }

            if (var7 < 240)
            {
                //frametime in milliseconds
                double targetMs = 1000.0 / var7;

                double elapsedMs = fpsTimer.Elapsed.TotalMilliseconds;
                double waitTime = targetMs - elapsedMs;

                if (waitTime > 0)
                {
                    if (waitTime > 2.0)
                    {
                        Thread.Sleep((int)(waitTime - 1.0));
                    }

                    while (fpsTimer.Elapsed.TotalMilliseconds < targetMs)
                    {
                        Thread.SpinWait(10);
                    }
                }

                fpsTimer.Restart();
            }
        }
    }

    public void renderFrame(float tickDelta, long time)
    {
        GLManager.GL.Enable(GLEnum.CullFace);
        GLManager.GL.Enable(GLEnum.DepthTest);
        client.camera ??= client.player;

        Profiler.Start("getMouseOver");
        updateTargetedEntity(tickDelta);
        Profiler.Stop("getMouseOver");

        EntityLiving var4 = client.camera;
        WorldRenderer var5 = client.terrainRenderer;
        ParticleManager var6 = client.particleManager;
        double var7 = var4.lastTickX + (var4.x - var4.lastTickX) * (double)tickDelta;
        double var9 = var4.lastTickY + (var4.y - var4.lastTickY) * (double)tickDelta;
        double var11 = var4.lastTickZ + (var4.z - var4.lastTickZ) * (double)tickDelta;
        ChunkSource var13 = client.world.getChunkSource();

        Profiler.Start("updateFog");
        GLManager.GL.Viewport(0, 0, (uint)client.displayWidth, (uint)client.displayHeight);
        updateSkyAndFogColors(tickDelta);
        Profiler.Stop("updateFog");
        GLManager.GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
        GLManager.GL.Enable(GLEnum.CullFace);
        renderWorld(tickDelta);
        Frustum.getInstance();
        if (client.options.renderDistance < 2)
        {
            applyFog(-1);
            var5.renderSky(tickDelta);
        }

        GLManager.GL.Enable(GLEnum.Fog);
        applyFog(1);

        FrustrumCuller var19 = new();
        var19.setPosition(var7, var9, var11);

        applyFog(0);
        GLManager.GL.Enable(GLEnum.Fog);
        GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)client.textureManager.getTextureId("/terrain.png"));
        Lighting.turnOff();

        Profiler.Start("sortAndRender");
        var5.sortAndRender(var4, 0, (double)tickDelta, var19);
        Profiler.Stop("sortAndRender");

        GLManager.GL.ShadeModel(GLEnum.Flat);
        Lighting.turnOn();

        Profiler.Start("renderEntities");
        var5.renderEntities(var4.getPosition(tickDelta), var19, tickDelta);
        Profiler.Stop("renderEntities");

        var6.func_1187_b(var4, tickDelta);

        Lighting.turnOff();
        applyFog(0);

        Profiler.Start("renderParticles");
        var6.renderParticles(var4, tickDelta);
        Profiler.Stop("renderParticles");

        EntityPlayer var21;
        if (client.objectMouseOver != null && var4.isInFluid(Material.Water) && var4 is EntityPlayer)
        {
            var21 = (EntityPlayer)var4;
            GLManager.GL.Disable(GLEnum.AlphaTest);
            var5.drawBlockBreaking(var21, client.objectMouseOver, 0, var21.inventory.getSelectedItem(), tickDelta);
            var5.drawSelectionBox(var21, client.objectMouseOver, 0, var21.inventory.getSelectedItem(), tickDelta);
            GLManager.GL.Enable(GLEnum.AlphaTest);
        }

        GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        applyFog(0);
        GLManager.GL.Enable(GLEnum.Blend);
        GLManager.GL.Disable(GLEnum.CullFace);
        GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)client.textureManager.getTextureId("/terrain.png"));

        Profiler.Start("sortAndRender2");

        var5.sortAndRender(var4, 1, tickDelta, var19);

        GLManager.GL.ShadeModel(GLEnum.Flat);

        Profiler.Stop("sortAndRender2");

        //TODO: SELCTION BOX/BLOCK BREAKING VISUALIZATON DON'T APPEAR PROPERLY MOST OF THE TIME, SAME WITH ENTITY SHADOWS. VIEW BOBBING MAKES ENTITES BOB UP AND DOWN

        GLManager.GL.DepthMask(true);
        GLManager.GL.Enable(GLEnum.CullFace);
        GLManager.GL.Disable(GLEnum.Blend);
        if (cameraZoom == 1.0D && var4 is EntityPlayer && client.objectMouseOver != null && !var4.isInFluid(Material.Water))
        {
            var21 = (EntityPlayer)var4;
            GLManager.GL.Disable(GLEnum.AlphaTest);
            var5.drawBlockBreaking(var21, client.objectMouseOver, 0, var21.inventory.getSelectedItem(), tickDelta);
            var5.drawSelectionBox(var21, client.objectMouseOver, 0, var21.inventory.getSelectedItem(), tickDelta);
            GLManager.GL.Enable(GLEnum.AlphaTest);
        }

        renderSnow(tickDelta);
        GLManager.GL.Disable(GLEnum.Fog);
        if (targetedEntity != null)
        {
        }

        applyFog(0);
        GLManager.GL.Enable(GLEnum.Fog);
        var5.renderClouds(tickDelta);
        GLManager.GL.Disable(GLEnum.Fog);
        applyFog(1);
        if (cameraZoom == 1.0D)
        {
            GLManager.GL.Clear(ClearBufferMask.DepthBufferBit);
            renderFirstPersonHand(tickDelta);
        }
    }

    private void renderRain()
    {
        float var1 = client.world.getRainGradient(1.0F);

        if (var1 != 0.0F)
        {
            random.setSeed(ticks * 312987231L);
            EntityLiving var2 = client.camera;
            World var3 = client.world;
            int var4 = MathHelper.floor_double(var2.x);
            int var5 = MathHelper.floor_double(var2.y);
            int var6 = MathHelper.floor_double(var2.z);
            byte var7 = 10;
            double var8 = 0.0D;
            double var10 = 0.0D;
            double var12 = 0.0D;
            int var14 = 0;

            for (int var15 = 0; var15 < (int)(100.0F * var1 * var1); ++var15)
            {
                int var16 = var4 + random.nextInt(var7) - random.nextInt(var7);
                int var17 = var6 + random.nextInt(var7) - random.nextInt(var7);
                int var18 = var3.getTopSolidBlockY(var16, var17);
                int var19 = var3.getBlockId(var16, var18 - 1, var17);
                if (var18 <= var5 + var7 && var18 >= var5 - var7 && var3.getBiomeSource().GetBiome(var16, var17).CanSpawnLightningBolt())
                {
                    float var20 = random.nextFloat();
                    float var21 = random.nextFloat();
                    if (var19 > 0)
                    {
                        if (Block.Blocks[var19].material == Material.Lava)
                        {
                            client.particleManager.addEffect(new EntitySmokeFX(var3, (double)(var16 + var20), (double)(var18 + 0.1F) - Block.Blocks[var19].minY, (double)(var17 + var21), 0.0D, 0.0D, 0.0D));
                        }
                        else
                        {
                            ++var14;
                            if (random.nextInt(var14) == 0)
                            {
                                var8 = (double)(var16 + var20);
                                var10 = (double)(var18 + 0.1F) - Block.Blocks[var19].minY;
                                var12 = (double)(var17 + var21);
                            }

                            client.particleManager.addEffect(new EntityRainFX(var3, (double)(var16 + var20), (double)(var18 + 0.1F) - Block.Blocks[var19].minY, (double)(var17 + var21)));
                        }
                    }
                }
            }

            if (var14 > 0 && random.nextInt(3) < rainSoundCounter++)
            {
                rainSoundCounter = 0;
                if (var10 > var2.y + 1.0D && var3.getTopSolidBlockY(MathHelper.floor_double(var2.x), MathHelper.floor_double(var2.z)) > MathHelper.floor_double(var2.y))
                {
                    client.world.playSound(var8, var10, var12, "ambient.weather.rain", 0.1F, 0.5F);
                }
                else
                {
                    client.world.playSound(var8, var10, var12, "ambient.weather.rain", 0.2F, 1.0F);
                }
            }

        }
    }

    protected void renderSnow(float tickDelta)
    {
        float var2 = client.world.getRainGradient(tickDelta);
        if (var2 > 0.0F)
        {
            EntityLiving var3 = client.camera;
            World var4 = client.world;
            int var5 = MathHelper.floor_double(var3.x);
            int var6 = MathHelper.floor_double(var3.y);
            int var7 = MathHelper.floor_double(var3.z);
            Tessellator var8 = Tessellator.instance;
            GLManager.GL.Disable(GLEnum.CullFace);
            GLManager.GL.Normal3(0.0F, 1.0F, 0.0F);
            GLManager.GL.Enable(GLEnum.Blend);
            GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
            GLManager.GL.AlphaFunc(GLEnum.Greater, 0.01F);
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)client.textureManager.getTextureId("/environment/snow.png"));
            double var9 = var3.lastTickX + (var3.x - var3.lastTickX) * (double)tickDelta;
            double var11 = var3.lastTickY + (var3.y - var3.lastTickY) * (double)tickDelta;
            double var13 = var3.lastTickZ + (var3.z - var3.lastTickZ) * (double)tickDelta;
            int var15 = MathHelper.floor_double(var11);
            byte var16 = 10;

            Biome[] var17 = var4.getBiomeSource().GetBiomesInArea(var5 - var16, var7 - var16, var16 * 2 + 1, var16 * 2 + 1);
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
                    if (var21.GetEnableSnow())
                    {
                        var22 = var4.getTopSolidBlockY(var19, var20);
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
                            random.setSeed(var19 * var19 * 3121 + var19 * 45238971 + var20 * var20 * 418711 + var20 * 13761);
                            float var27 = ticks + tickDelta;
                            float var28 = ((ticks & 511) + tickDelta) / 512.0F;
                            float var29 = random.nextFloat() + var27 * 0.01F * (float)random.nextGaussian();
                            float var30 = random.nextFloat() + var27 * (float)random.nextGaussian() * 0.001F;
                            double var31 = (double)(var19 + 0.5F) - var3.x;
                            double var33 = (double)(var20 + 0.5F) - var3.z;
                            float var35 = MathHelper.sqrt_double(var31 * var31 + var33 * var33) / var16;
                            var8.startDrawingQuads();
                            float var36 = var4.getLuminance(var19, var23, var20);
                            GLManager.GL.Color4(var36, var36, var36, ((1.0F - var35 * var35) * 0.3F + 0.5F) * var2);
                            var8.setTranslationD(-var9 * 1.0D, -var11 * 1.0D, -var13 * 1.0D);
                            var8.addVertexWithUV(var19 + 0, var24, var20 + 0.5D, (double)(0.0F * var26 + var29), (double)(var24 * var26 / 4.0F + var28 * var26 + var30));
                            var8.addVertexWithUV(var19 + 1, var24, var20 + 0.5D, (double)(1.0F * var26 + var29), (double)(var24 * var26 / 4.0F + var28 * var26 + var30));
                            var8.addVertexWithUV(var19 + 1, var25, var20 + 0.5D, (double)(1.0F * var26 + var29), (double)(var25 * var26 / 4.0F + var28 * var26 + var30));
                            var8.addVertexWithUV(var19 + 0, var25, var20 + 0.5D, (double)(0.0F * var26 + var29), (double)(var25 * var26 / 4.0F + var28 * var26 + var30));
                            var8.addVertexWithUV(var19 + 0.5D, var24, var20 + 0, (double)(0.0F * var26 + var29), (double)(var24 * var26 / 4.0F + var28 * var26 + var30));
                            var8.addVertexWithUV(var19 + 0.5D, var24, var20 + 1, (double)(1.0F * var26 + var29), (double)(var24 * var26 / 4.0F + var28 * var26 + var30));
                            var8.addVertexWithUV(var19 + 0.5D, var25, var20 + 1, (double)(1.0F * var26 + var29), (double)(var25 * var26 / 4.0F + var28 * var26 + var30));
                            var8.addVertexWithUV(var19 + 0.5D, var25, var20 + 0, (double)(0.0F * var26 + var29), (double)(var25 * var26 / 4.0F + var28 * var26 + var30));
                            var8.setTranslationD(0.0D, 0.0D, 0.0D);
                            var8.draw();
                        }
                    }
                }
            }

            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)client.textureManager.getTextureId("/environment/rain.png"));
            var16 = 10;

            var18 = 0;

            for (var19 = var5 - var16; var19 <= var5 + var16; ++var19)
            {
                for (var20 = var7 - var16; var20 <= var7 + var16; ++var20)
                {
                    var21 = var17[var18++];
                    if (var21.CanSpawnLightningBolt())
                    {
                        var22 = var4.getTopSolidBlockY(var19, var20);
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
                            random.setSeed(var19 * var19 * 3121 + var19 * 45238971 + var20 * var20 * 418711 + var20 * 13761);
                            var26 = ((ticks + var19 * var19 * 3121 + var19 * 45238971 + var20 * var20 * 418711 + var20 * 13761 & 31) + tickDelta) / 32.0F * (3.0F + random.nextFloat());
                            double var38 = (double)(var19 + 0.5F) - var3.x;
                            double var39 = (double)(var20 + 0.5F) - var3.z;
                            float var40 = MathHelper.sqrt_double(var38 * var38 + var39 * var39) / var16;
                            var8.startDrawingQuads();
                            float var32 = var4.getLuminance(var19, 128, var20) * 0.85F + 0.15F;
                            GLManager.GL.Color4(var32, var32, var32, ((1.0F - var40 * var40) * 0.5F + 0.5F) * var2);
                            var8.setTranslationD(-var9 * 1.0D, -var11 * 1.0D, -var13 * 1.0D);
                            var8.addVertexWithUV(var19 + 0, var23, var20 + 0.5D, (double)(0.0F * var37), (double)(var23 * var37 / 4.0F + var26 * var37));
                            var8.addVertexWithUV(var19 + 1, var23, var20 + 0.5D, (double)(1.0F * var37), (double)(var23 * var37 / 4.0F + var26 * var37));
                            var8.addVertexWithUV(var19 + 1, var24, var20 + 0.5D, (double)(1.0F * var37), (double)(var24 * var37 / 4.0F + var26 * var37));
                            var8.addVertexWithUV(var19 + 0, var24, var20 + 0.5D, (double)(0.0F * var37), (double)(var24 * var37 / 4.0F + var26 * var37));
                            var8.addVertexWithUV(var19 + 0.5D, var23, var20 + 0, (double)(0.0F * var37), (double)(var23 * var37 / 4.0F + var26 * var37));
                            var8.addVertexWithUV(var19 + 0.5D, var23, var20 + 1, (double)(1.0F * var37), (double)(var23 * var37 / 4.0F + var26 * var37));
                            var8.addVertexWithUV(var19 + 0.5D, var24, var20 + 1, (double)(1.0F * var37), (double)(var24 * var37 / 4.0F + var26 * var37));
                            var8.addVertexWithUV(var19 + 0.5D, var24, var20 + 0, (double)(0.0F * var37), (double)(var24 * var37 / 4.0F + var26 * var37));
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

    public void setupHudRender()
    {
        ScaledResolution var1 = new(client.options, client.displayWidth, client.displayHeight);
        GLManager.GL.Clear(ClearBufferMask.DepthBufferBit);
        GLManager.GL.MatrixMode(GLEnum.Projection);
        GLManager.GL.LoadIdentity();
        GLManager.GL.Ortho(0.0D, var1.ScaledWidthDouble, var1.ScaledHeightDouble, 0.0D, 1000.0D, 3000.0D);
        GLManager.GL.MatrixMode(GLEnum.Modelview);
        GLManager.GL.LoadIdentity();
        GLManager.GL.Translate(0.0F, 0.0F, -2000.0F);
    }

    private void updateSkyAndFogColors(float tickDelta)
    {
        World var2 = client.world;
        EntityLiving var3 = client.camera;
        float var4 = 1.0F / (4 - client.options.renderDistance);
        var4 = 1.0F - (float)java.lang.Math.pow((double)var4, 0.25D);
        Vector3D<double> var5 = var2.getSkyColor(client.camera, tickDelta);
        float var6 = (float)var5.X;
        float var7 = (float)var5.Y;
        float var8 = (float)var5.Z;
        Vector3D<double> var9 = var2.getFogColor(tickDelta);
        fogColorRed = (float)var9.X;
        fogColorGreen = (float)var9.Y;
        fogColorBlue = (float)var9.Z;
        fogColorRed += (var6 - fogColorRed) * var4;
        fogColorGreen += (var7 - fogColorGreen) * var4;
        fogColorBlue += (var8 - fogColorBlue) * var4;
        float var10 = var2.getRainGradient(tickDelta);
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

        var11 = var2.getThunderGradient(tickDelta);
        if (var11 > 0.0F)
        {
            var12 = 1.0F - var11 * 0.5F;
            fogColorRed *= var12;
            fogColorGreen *= var12;
            fogColorBlue *= var12;
        }

        if (cloudFog)
        {
            Vector3D<double> var16 = var2.getCloudColor(tickDelta);
            fogColorRed = (float)var16.X;
            fogColorGreen = (float)var16.Y;
            fogColorBlue = (float)var16.Z;
        }
        else if (var3.isInFluid(Material.Water))
        {
            fogColorRed = 0.02F;
            fogColorGreen = 0.02F;
            fogColorBlue = 0.2F;
        }
        else if (var3.isInFluid(Material.Lava))
        {
            fogColorRed = 0.6F;
            fogColorGreen = 0.1F;
            fogColorBlue = 0.0F;
        }

        var12 = lastViewBob + (viewBob - lastViewBob) * tickDelta;
        fogColorRed *= var12;
        fogColorGreen *= var12;
        fogColorBlue *= var12;

        GLManager.GL.ClearColor(fogColorRed, fogColorGreen, fogColorBlue, 0.0F);
    }

    private void applyFog(int mode)
    {
        EntityLiving var3 = client.camera;
        GLManager.GL.Fog(GLEnum.FogColor, updateFogColorBuffer(fogColorRed, fogColorGreen, fogColorBlue, 1.0F));
        client.terrainRenderer.chunkRenderer.SetFogColor(fogColorRed, fogColorGreen, fogColorBlue, 1.0f);
        GLManager.GL.Normal3(0.0F, -1.0F, 0.0F);
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        if (cloudFog)
        {
            GLManager.GL.Fog(GLEnum.FogMode, (int)GLEnum.Exp);
            GLManager.GL.Fog(GLEnum.FogDensity, 0.1F);
            client.terrainRenderer.chunkRenderer.SetFogMode(1);
            client.terrainRenderer.chunkRenderer.SetFogDensity(0.1f);
        }
        else if (var3.isInFluid(Material.Water))
        {
            GLManager.GL.Fog(GLEnum.FogMode, (int)GLEnum.Exp);
            GLManager.GL.Fog(GLEnum.FogDensity, 0.1F);
            client.terrainRenderer.chunkRenderer.SetFogMode(1);
            client.terrainRenderer.chunkRenderer.SetFogDensity(0.1f);
        }
        else if (var3.isInFluid(Material.Lava))
        {
            GLManager.GL.Fog(GLEnum.FogMode, (int)GLEnum.Exp);
            GLManager.GL.Fog(GLEnum.FogDensity, 2.0F);
            client.terrainRenderer.chunkRenderer.SetFogMode(1);
            client.terrainRenderer.chunkRenderer.SetFogDensity(2.0f);
        }
        else
        {
            GLManager.GL.Fog(GLEnum.FogMode, (int)GLEnum.Linear);
            GLManager.GL.Fog(GLEnum.FogStart, viewDistane * 0.25F);
            GLManager.GL.Fog(GLEnum.FogEnd, viewDistane);
            client.terrainRenderer.chunkRenderer.SetFogMode(0);
            client.terrainRenderer.chunkRenderer.SetFogStart(viewDistane * 0.25f);
            client.terrainRenderer.chunkRenderer.SetFogEnd(viewDistane);
            if (mode < 0)
            {
                GLManager.GL.Fog(GLEnum.FogStart, 0.0F);
                GLManager.GL.Fog(GLEnum.FogEnd, viewDistane * 0.8F);
                client.terrainRenderer.chunkRenderer.SetFogStart(0.0f);
                client.terrainRenderer.chunkRenderer.SetFogEnd(viewDistane * 0.8f);
            }

            if (client.world.dimension.isNether)
            {
                GLManager.GL.Fog(GLEnum.FogStart, 0.0F);
                client.terrainRenderer.chunkRenderer.SetFogStart(0.0f);
            }
        }

        GLManager.GL.Enable(GLEnum.ColorMaterial);
        GLManager.GL.ColorMaterial(GLEnum.Front, GLEnum.Ambient);
    }

    private float[] updateFogColorBuffer(float var1, float var2, float var3, float var4)
    {
        fogColorBuffer[0] = var1;
        fogColorBuffer[1] = var2;
        fogColorBuffer[2] = var3;
        fogColorBuffer[3] = var4;
        return fogColorBuffer;
    }
}
