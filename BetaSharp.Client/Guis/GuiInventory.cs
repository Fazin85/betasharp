using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Entities;
using BetaSharp.Entities;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Guis;

public class GuiInventory : GuiContainer
{

    private float _mouseX;
    private float _mouseY;

    public GuiInventory(EntityPlayer player) : base(player.playerScreenHandler)
    {
        AllowUserInput = true;
        player.increaseStat(BetaSharp.Achievements.OpenInventory, 1);
    }

    public override void InitGui()
    {
        _controlList.Clear();
    }

    protected override void DrawGuiContainerForegroundLayer()
    {
        FontRenderer.DrawString("Crafting", 86, 16, 0x404040);
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        base.Render(mouseX, mouseY, partialTicks);
        _mouseX = mouseX;
        _mouseY = mouseY;
    }

    protected override void DrawGuiContainerBackgroundLayer(float partialTicks)
    {
        int textureId = mc.textureManager.GetTextureId("/gui/inventory.png");
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        mc.textureManager.BindTexture(textureId);

        int guiLeft = (Width - _xSize) / 2;
        int guiTop = (Height - _ySize) / 2;

        DrawTexturedModalRect(guiLeft, guiTop, 0, 0, _xSize, _ySize);
        GLManager.GL.Enable(GLEnum.RescaleNormal);
        GLManager.GL.Enable(GLEnum.ColorMaterial);
        GLManager.GL.PushMatrix();
        GLManager.GL.Translate(guiLeft + 51, guiTop + 75, 50.0F);

        float scale = 30.0F;
        GLManager.GL.Scale(-scale, scale, scale);
        GLManager.GL.Rotate(180.0F, 0.0F, 0.0F, 1.0F);

        float bodyYaw = mc.player.bodyYaw;
        float headYaw = mc.player.yaw;
        float headPitch = mc.player.pitch;
        float lookX = guiLeft + 51 - _mouseX;
        float lookY = guiTop + 75 - 50 - _mouseY;

        GLManager.GL.Rotate(135.0F, 0.0F, 1.0F, 0.0F);
        Lighting.turnOn();
        GLManager.GL.Rotate(-135.0F, 0.0F, 1.0F, 0.0F);
        GLManager.GL.Rotate(-(float)Math.Atan(lookY / 40.0F) * 20.0F, 1.0F, 0.0F, 0.0F);

        mc.player.bodyYaw = (float)Math.Atan(lookX / 40.0F) * 20.0F;
        mc.player.yaw = (float)Math.Atan(lookX / 40.0F) * 40.0F;
        mc.player.pitch = -(float)Math.Atan(lookY / 40.0F) * 20.0F;
        mc.player.minBrightness = 1.0F;

        GLManager.GL.Translate(0.0F, mc.player.standingEyeHeight, 0.0F);
        EntityRenderDispatcher.instance.playerViewY = 180.0F;
        EntityRenderDispatcher.instance.renderEntityWithPosYaw(mc.player, 0.0D, 0.0D, 0.0D, 0.0F, 1.0F);

        mc.player.minBrightness = 0.0F;
        mc.player.bodyYaw = bodyYaw;
        mc.player.yaw = headYaw;
        mc.player.pitch = headPitch;

        GLManager.GL.PopMatrix();
        Lighting.turnOff();
        GLManager.GL.Disable(GLEnum.RescaleNormal);
    }

    protected override void ActionPerformed(GuiButton btt)
    {
        if (btt.Id == 0)
        {
            mc.displayGuiScreen(new GuiAchievements(mc.statFileWriter));
        }

        if (btt.Id == 1)
        {
            mc.displayGuiScreen(new GuiStats(this, mc.statFileWriter));
        }

    }
}
