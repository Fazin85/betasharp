using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Items;
using BetaSharp.Stats;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Guis;

public class GuiAchievement : Gui
{
    private const long ACHIEVEMENT_DISPLAY_DURATION = 3000L;
    private const string LICENSE_WARNING_TEXT = "Minecraft Beta 1.7.3   Unlicensed Copy :(";
    private const string ALT_LOCATION_WARNING_TEXT = "(Or logged in from another location)";
    private const string PURCHASE_PROMPT_TEXT = "Purchase at minecraft.net";

    private readonly Minecraft theGame;
    private int achievementWindowWidth;
    private int achievementWindowHeight;
    private string achievementTitle;
    private string? achievementDescription;
    private Achievement theAchievement;
    private long achievementDisplayStartTime;
    private readonly ItemRenderer itemRender;
    private bool isAchievementInformation;

    public GuiAchievement(Minecraft mc)
    {
        theGame = mc;
        itemRender = new ItemRenderer();
    }

    public void queueTakenAchievement(Achievement achievement)
    {
        achievementTitle = StatCollector.translateToLocal("achievement.get");
        achievementDescription = achievement.statName;
        achievementDisplayStartTime = java.lang.System.currentTimeMillis();
        theAchievement = achievement;
        isAchievementInformation = false;
    }

    public void queueAchievementInformation(Achievement achievement)
    {
        achievementTitle = achievement.statName;
        achievementDescription = achievement.getTranslatedDescription();
        achievementDisplayStartTime = java.lang.System.currentTimeMillis() - 2500L;
        theAchievement = achievement;
        isAchievementInformation = true;
    }

    private void updateAchievementWindowScale()
    {
        GLManager.GL.Viewport(0, 0, (uint)theGame.displayWidth, (uint)theGame.displayHeight);
        GLManager.GL.MatrixMode(GLEnum.Projection);
        GLManager.GL.LoadIdentity();
        GLManager.GL.MatrixMode(GLEnum.Modelview);
        GLManager.GL.LoadIdentity();
        achievementWindowWidth = theGame.displayWidth;
        achievementWindowHeight = theGame.displayHeight;
        ScaledResolution scaledResolution = new(theGame.options, theGame.displayWidth, theGame.displayHeight);
        achievementWindowWidth = scaledResolution.ScaledWidth;
        achievementWindowHeight = scaledResolution.ScaledHeight;
        GLManager.GL.Clear(ClearBufferMask.DepthBufferBit);
        GLManager.GL.MatrixMode(GLEnum.Projection);
        GLManager.GL.LoadIdentity();
        GLManager.GL.Ortho(0.0D, achievementWindowWidth, achievementWindowHeight, 0.0D, 1000.0D, 3000.0D);
        GLManager.GL.MatrixMode(GLEnum.Modelview);
        GLManager.GL.LoadIdentity();
        GLManager.GL.Translate(0.0F, 0.0F, -2000.0F);
    }

    public void updateAchievementWindow()
    {
        if (Minecraft.hasPaidCheckTime > 0L)
        {
            displayLicenseWarning();
        }

        if (theAchievement != null && achievementDisplayStartTime != 0L)
        {
            displayAchievementNotification();
        }
    }

    private void displayLicenseWarning()
    {
        GLManager.GL.Disable(GLEnum.DepthTest);
        GLManager.GL.DepthMask(false);
        Lighting.turnOff();
        updateAchievementWindowScale();
        theGame.fontRenderer.drawStringWithShadow(LICENSE_WARNING_TEXT, 2, 2, 0x00FFFFFF);
        theGame.fontRenderer.drawStringWithShadow(ALT_LOCATION_WARNING_TEXT, 2, 11, 0x00FFFFFF);
        theGame.fontRenderer.drawStringWithShadow(PURCHASE_PROMPT_TEXT, 2, 20, 0x00FFFFFF);
        GLManager.GL.DepthMask(true);
        GLManager.GL.Enable(GLEnum.DepthTest);
    }

    private void displayAchievementNotification()
    {
        double elapsedTime = (java.lang.System.currentTimeMillis() - achievementDisplayStartTime) / ACHIEVEMENT_DISPLAY_DURATION;
        if (isAchievementInformation || isAchievementInformation || elapsedTime >= 0.0D && elapsedTime <= 1.0D)
        {
            renderAchievementNotification(elapsedTime);
        }
        else
        {
            achievementDisplayStartTime = 0L;
        }
    }

    private void renderAchievementNotification(double elapsedTime)
    {
        updateAchievementWindowScale();
        GLManager.GL.Disable(GLEnum.DepthTest);
        GLManager.GL.DepthMask(false);
        double animationProgress = calculateAnimationProgress(elapsedTime);
        int achievementX = achievementWindowWidth - 160;
        int achievementY = 0 - (int)(animationProgress * 36.0D);
        int achievementTextureId = theGame.textureManager.getTextureId("/achievement/bg.png");

        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        GLManager.GL.Enable(GLEnum.Lighting);
        GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)achievementTextureId);
        GLManager.GL.Disable(GLEnum.Lighting);
        drawTexturedModalRect(achievementX, achievementY, 96, 202, 160, 32);
        drawAchievementText(achievementX, achievementY);

        GLManager.GL.PushMatrix();
        GLManager.GL.Rotate(180.0F, 1.0F, 0.0F, 0.0F);
        Lighting.turnOn();
        GLManager.GL.PopMatrix();
        GLManager.GL.Disable(GLEnum.Lighting);
        GLManager.GL.Enable(GLEnum.RescaleNormal);
        GLManager.GL.Enable(GLEnum.ColorMaterial);
        GLManager.GL.Enable(GLEnum.Lighting);
        itemRender.renderItemIntoGUI(theGame.fontRenderer, theGame.textureManager, theAchievement.icon, achievementX + 8, achievementY + 8);
        GLManager.GL.Disable(GLEnum.Lighting);
        GLManager.GL.DepthMask(true);
        GLManager.GL.Enable(GLEnum.DepthTest);
    }

    private double calculateAnimationProgress(double elapsedTime)
    {
        double animationProgress = elapsedTime * 2.0D;
        if (animationProgress > 1.0D)
        {
            animationProgress = 2.0D - animationProgress;
        }

        animationProgress *= 4.0D;
        animationProgress = 1.0D - animationProgress;
        if (animationProgress < 0.0D)
        {
            animationProgress = 0.0D;
        }

        animationProgress *= animationProgress;
        animationProgress *= animationProgress;
        return animationProgress;
    }

    private void drawAchievementText(int achievementX, int achievementY)
    {
        if (isAchievementInformation)
        {
            theGame.fontRenderer.func_27278_a(achievementDescription ?? "", achievementX + 30, achievementY + 7, 120, 0xFFFFFFFF);
        }
        else
        {
            theGame.fontRenderer.drawString(achievementTitle, achievementX + 30, achievementY + 7, 0xFFFFFF00);
            theGame.fontRenderer.drawString(achievementDescription ?? "", achievementX + 30, achievementY + 18, 0xFFFFFFFF);
        }
    }
}
