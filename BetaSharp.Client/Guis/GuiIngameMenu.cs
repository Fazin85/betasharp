using BetaSharp.Stats;
using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Guis;

public class GuiIngameMenu : GuiScreen
{

    private int updateCounter2 = 0;
    private int updateCounter = 0;

    public override void InitGui()
    {
        updateCounter2 = 0;
        controlList.Clear();
        int var1 = -16;
        controlList.Add(new GuiButton(1, Width / 2 - 100, Height / 4 + 120 + var1, "Save and quit to title"));
        if (mc.isMultiplayerWorld() && mc.internalServer == null)
        {
            controlList[0].DisplayString = "Disconnect";
        }

        controlList.Add(new GuiButton(4, Width / 2 - 100, Height / 4 + 24 + var1, "Back to game"));
        controlList.Add(new GuiButton(0, Width / 2 - 100, Height / 4 + 96 + var1, "Options..."));
        controlList.Add(new GuiButton(5, Width / 2 - 100, Height / 4 + 48 + var1, 98, 20,
            StatCollector.translateToLocal("gui.achievements")));
        controlList.Add(new GuiButton(6, Width / 2 + 2, Height / 4 + 48 + var1, 98, 20,
            StatCollector.translateToLocal("gui.stats")));
    }

    protected override void ActionPerformed(GuiButton var1)
    {
        if (var1.Id == 0)
        {
            mc.displayGuiScreen(new GuiOptions(this, mc.options));
        }

        if (var1.Id == 1)
        {
            mc.statFileWriter.readStat(Stats.Stats.leaveGameStat, 1);
            if (mc.isMultiplayerWorld())
            {
                mc.world.Disconnect();
            }

            mc.stopInternalServer();
            mc.changeWorld1(null);
            mc.displayGuiScreen(new GuiMainMenu());
        }

        if (var1.Id == 4)
        {
            mc.displayGuiScreen(null);
            mc.setIngameFocus();
        }

        if (var1.Id == 5)
        {
            mc.displayGuiScreen(new GuiAchievements(mc.statFileWriter));
        }

        if (var1.Id == 6)
        {
            mc.displayGuiScreen(new GuiStats(this, mc.statFileWriter));
        }
    }

    public override void UpdateScreen()
    {
        base.UpdateScreen();
        ++updateCounter;
    }

    public override void Render(int var1, int var2, float var3)
    {
        DrawDefaultBackground();
        bool var4 = !mc.world.attemptSaving(updateCounter2++);
        if (var4 || updateCounter < 20)
        {
            float var5 = (updateCounter % 10 + var3) / 10.0F;
            var5 = MathHelper.sin(var5 * (float)Math.PI * 2.0F) * 0.2F + 0.8F;
            int var6 = (int)(255.0F * var5);
            DrawString(fontRenderer, "Saving level..", 8, Height - 16, (uint)(var6 << 16 | var6 << 8 | var6));
        }

        DrawCenteredString(fontRenderer, "Game menu", Width / 2, 40, 0x00FFFFFF);
        base.Render(var1, var2, var3);
    }
}