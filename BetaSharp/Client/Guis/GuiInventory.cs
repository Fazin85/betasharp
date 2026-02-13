using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Entitys;
using BetaSharp.Entities;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Guis;

public class GuiInventory : GuiContainer
{

    private float xSize_lo;
    private float ySize_lo;

    public GuiInventory(EntityPlayer var1) : base(var1.playerScreenHandler)
    {
        field_948_f = true;
        var1.increaseStat(Achievements.OpenInventory, 1);
    }

    public override void initGui()
    {
        controlList.clear();
    }

    protected override void drawGuiContainerForegroundLayer()
    {
        fontRenderer.drawString("Crafting", 86, 16, 4210752);
    }

    public override void render(int var1, int var2, float var3)
    {
        base.render(var1, var2, var3);
        xSize_lo = var1;
        ySize_lo = var2;
    }

    protected override void drawGuiContainerBackgroundLayer(float var1)
    {
        int var2 = mc.textureManager.getTextureId("/gui/inventory.png");
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        mc.textureManager.bindTexture(var2);
        int var3 = (width - xSize) / 2;
        int var4 = (height - ySize) / 2;
        drawTexturedModalRect(var3, var4, 0, 0, xSize, ySize);
        GLManager.GL.Enable(GLEnum.RescaleNormal);
        GLManager.GL.Enable(GLEnum.ColorMaterial);
        GLManager.GL.PushMatrix();
        GLManager.GL.Translate(var3 + 51, var4 + 75, 50.0F);
        float var5 = 30.0F;
        GLManager.GL.Scale(-var5, var5, var5);
        GLManager.GL.Rotate(180.0F, 0.0F, 0.0F, 1.0F);
        float var6 = mc.player.bodyYaw;
        float var7 = mc.player.yaw;
        float var8 = mc.player.pitch;
        float var9 = var3 + 51 - xSize_lo;
        float var10 = var4 + 75 - 50 - ySize_lo;
        GLManager.GL.Rotate(135.0F, 0.0F, 1.0F, 0.0F);
        Lighting.turnOn();
        GLManager.GL.Rotate(-135.0F, 0.0F, 1.0F, 0.0F);
        GLManager.GL.Rotate(-(float)java.lang.Math.atan((double)(var10 / 40.0F)) * 20.0F, 1.0F, 0.0F, 0.0F);
        mc.player.bodyYaw = (float)java.lang.Math.atan((double)(var9 / 40.0F)) * 20.0F;
        mc.player.yaw = (float)java.lang.Math.atan((double)(var9 / 40.0F)) * 40.0F;
        mc.player.pitch = -(float)java.lang.Math.atan((double)(var10 / 40.0F)) * 20.0F;
        mc.player.minBrightness = 1.0F;
        GLManager.GL.Translate(0.0F, mc.player.standingEyeHeight, 0.0F);
        EntityRenderDispatcher.instance.playerViewY = 180.0F;
        EntityRenderDispatcher.instance.renderEntityWithPosYaw(mc.player, 0.0D, 0.0D, 0.0D, 0.0F, 1.0F);
        mc.player.minBrightness = 0.0F;
        mc.player.bodyYaw = var6;
        mc.player.yaw = var7;
        mc.player.pitch = var8;
        GLManager.GL.PopMatrix();
        Lighting.turnOff();
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