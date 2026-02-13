using BetaSharp.Stats;
using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Guis;

public class GuiIngameMenu : GuiScreen
{

    private int updateCounter2 = 0;
    private int updateCounter = 0;

    public override void initGui()
    {
        updateCounter2 = 0;
        controlList.clear();
        int var1 = -16;
        controlList.add(new GuiButton(1, width / 2 - 100, height / 4 + 120 + var1, "Save and quit to title"));
        if (mc.isMultiplayerWorld() && mc.internalServer == null)
        {
            ((GuiButton)controlList.get(0)).displayString = "Disconnect";
        }

        controlList.add(new GuiButton(4, width / 2 - 100, height / 4 + 24 + var1, "Back to game"));
        controlList.add(new GuiButton(0, width / 2 - 100, height / 4 + 96 + var1, "Options..."));
        controlList.add(new GuiButton(5, width / 2 - 100, height / 4 + 48 + var1, 98, 20,
            StatCollector.translateToLocal("gui.achievements")));
        controlList.add(new GuiButton(6, width / 2 + 2, height / 4 + 48 + var1, 98, 20,
            StatCollector.translateToLocal("gui.stats")));
    }

    protected override void actionPerformed(GuiButton var1)
    {
        if (var1.id == 0)
        {
            mc.displayGuiScreen(new GuiOptions(this, mc.options));
        }

        if (var1.id == 1)
        {
            mc.statFileWriter.readStat(Stats.Stats.leaveGameStat, 1);
            if (mc.isMultiplayerWorld())
            {
                mc.world.disconnect();
            }

            mc.stopInternalServer();
            mc.changeWorld1(null);
            mc.displayGuiScreen(new GuiMainMenu());
        }

        if (var1.id == 4)
        {
            mc.displayGuiScreen(null);
            mc.setIngameFocus();
        }

        if (var1.id == 5)
        {
            mc.displayGuiScreen(new GuiAchievements(mc.statFileWriter));
        }

        if (var1.id == 6)
        {
            mc.displayGuiScreen(new GuiStats(this, mc.statFileWriter));
        }
    }

    public override void updateScreen()
    {
        base.updateScreen();
        ++updateCounter;
    }

    public override void render(int var1, int var2, float var3)
    {
        drawDefaultBackground();
        bool var4 = !mc.world.attemptSaving(updateCounter2++);
        if (var4 || updateCounter < 20)
        {
            float var5 = (updateCounter % 10 + var3) / 10.0F;
            var5 = MathHelper.sin(var5 * (float)Math.PI * 2.0F) * 0.2F + 0.8F;
            int var6 = (int)(255.0F * var5);
            drawString(fontRenderer, "Saving level..", 8, height - 16, var6 << 16 | var6 << 8 | var6);
        }

        drawCenteredString(fontRenderer, "Game menu", width / 2, 40, 16777215);
        base.render(var1, var2, var3);
    }
}