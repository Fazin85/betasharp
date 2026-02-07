using betareborn.Rendering;
using betareborn.Stats;
using Silk.NET.OpenGL.Legacy;

namespace betareborn.Guis
{
    public class GuiAchievement : Gui
    {

        private Minecraft theGame;
        private int achievementWindowWidth;
        private int achievementWindowHeight;
        private String field_25085_d;
        private String field_25084_e;
        private Achievement theAchievement;
        private long field_25083_f;
        private RenderItem itemRender;
        private bool field_27103_i;

        public GuiAchievement(Minecraft var1)
        {
            theGame = var1;
            itemRender = new RenderItem();
        }

        public void queueTakenAchievement(Achievement var1)
        {
            field_25085_d = StatCollector.translateToLocal("achievement.get");
            field_25084_e = var1.statName;
            field_25083_f = java.lang.System.currentTimeMillis();
            theAchievement = var1;
            field_27103_i = false;
        }

        public void queueAchievementInformation(Achievement var1)
        {
            field_25085_d = var1.statName;
            field_25084_e = var1.getTranslatedDescription();
            field_25083_f = java.lang.System.currentTimeMillis() - 2500L;
            theAchievement = var1;
            field_27103_i = true;
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
            ScaledResolution var1 = new ScaledResolution(theGame.gameSettings, theGame.displayWidth, theGame.displayHeight);
            achievementWindowWidth = var1.getScaledWidth();
            achievementWindowHeight = var1.getScaledHeight();
            GLManager.GL.Clear(ClearBufferMask.DepthBufferBit);
            GLManager.GL.MatrixMode(GLEnum.Projection);
            GLManager.GL.LoadIdentity();
            GLManager.GL.Ortho(0.0D, (double)achievementWindowWidth, (double)achievementWindowHeight, 0.0D, 1000.0D, 3000.0D);
            GLManager.GL.MatrixMode(GLEnum.Modelview);
            GLManager.GL.LoadIdentity();
            GLManager.GL.Translate(0.0F, 0.0F, -2000.0F);
        }

        public void updateAchievementWindow()
        {
            if (Minecraft.hasPaidCheckTime > 0L)
            {
                GLManager.GL.Disable(GLEnum.DepthTest);
                GLManager.GL.DepthMask(false);
                RenderHelper.disableStandardItemLighting();
                updateAchievementWindowScale();
                String var1 = "Minecraft Beta 1.7.3   Unlicensed Copy :(";
                String var2 = "(Or logged in from another location)";
                String var3 = "Purchase at minecraft.net";
                theGame.fontRenderer.drawStringWithShadow(var1, 2, 2, 16777215);
                theGame.fontRenderer.drawStringWithShadow(var2, 2, 11, 16777215);
                theGame.fontRenderer.drawStringWithShadow(var3, 2, 20, 16777215);
                GLManager.GL.DepthMask(true);
                GLManager.GL.Enable(GLEnum.DepthTest);
            }

            if (theAchievement != null && field_25083_f != 0L)
            {
                double var8 = (double)(java.lang.System.currentTimeMillis() - field_25083_f) / 3000.0D;
                if (field_27103_i || field_27103_i || var8 >= 0.0D && var8 <= 1.0D)
                {
                    updateAchievementWindowScale();
                    GLManager.GL.Disable(GLEnum.DepthTest);
                    GLManager.GL.DepthMask(false);
                    double var9 = var8 * 2.0D;
                    if (var9 > 1.0D)
                    {
                        var9 = 2.0D - var9;
                    }

                    var9 *= 4.0D;
                    var9 = 1.0D - var9;
                    if (var9 < 0.0D)
                    {
                        var9 = 0.0D;
                    }

                    var9 *= var9;
                    var9 *= var9;
                    int var5 = achievementWindowWidth - 160;
                    int var6 = 0 - (int)(var9 * 36.0D);
                    int var7 = theGame.renderEngine.getTexture("/achievement/bg.png");
                    GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
                    GLManager.GL.Enable(GLEnum.Lighting);
                    GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)var7);
                    GLManager.GL.Disable(GLEnum.Lighting);
                    drawTexturedModalRect(var5, var6, 96, 202, 160, 32);
                    if (field_27103_i)
                    {
                        theGame.fontRenderer.func_27278_a(field_25084_e, var5 + 30, var6 + 7, 120, -1);
                    }
                    else
                    {
                        theGame.fontRenderer.drawString(field_25085_d, var5 + 30, var6 + 7, -256);
                        theGame.fontRenderer.drawString(field_25084_e, var5 + 30, var6 + 18, -1);
                    }

                    GLManager.GL.PushMatrix();
                    GLManager.GL.Rotate(180.0F, 1.0F, 0.0F, 0.0F);
                    RenderHelper.enableStandardItemLighting();
                    GLManager.GL.PopMatrix();
                    GLManager.GL.Disable(GLEnum.Lighting);
                    GLManager.GL.Enable(GLEnum.RescaleNormal);
                    GLManager.GL.Enable(GLEnum.ColorMaterial);
                    GLManager.GL.Enable(GLEnum.Lighting);
                    itemRender.renderItemIntoGUI(theGame.fontRenderer, theGame.renderEngine, theAchievement.icon, var5 + 8, var6 + 8);
                    GLManager.GL.Disable(GLEnum.Lighting);
                    GLManager.GL.DepthMask(true);
                    GLManager.GL.Enable(GLEnum.DepthTest);
                }
                else
                {
                    field_25083_f = 0L;
                }
            }
        }
    }

}