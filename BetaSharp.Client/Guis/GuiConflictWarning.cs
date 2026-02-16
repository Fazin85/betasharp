namespace BetaSharp.Client.Guis;

public class GuiConflictWarning : GuiScreen
{

    private int updateCounter = 0;

    public override void updateScreen()
    {
        ++updateCounter;
    }

    public override void initGui()
    {
        controlList.clear();
        controlList.add(new GuiButton(0, width / 2 - 100, height / 4 + 120 + 12, "Back to title screen"));
    }

    protected override void actionPerformed(GuiButton var1)
    {
        if (var1.enabled)
        {
            if (var1.id == 0)
            {
                mc.displayGuiScreen(new GuiMainMenu());
            }

        }
    }

    public override void render(int var1, int var2, float var3)
    {
        drawDefaultBackground();
        drawCenteredString(fontRenderer, "Level save conflict", width / 2, height / 4 - 60 + 20, 0x00FFFFFF);
        drawString(fontRenderer, "Minecraft detected a conflict in the level save data.", width / 2 - 140, height / 4 - 60 + 60 + 0, 10526880);
        drawString(fontRenderer, "This could be caused by two copies of the game", width / 2 - 140, height / 4 - 60 + 60 + 18, 10526880);
        drawString(fontRenderer, "accessing the same level.", width / 2 - 140, height / 4 - 60 + 60 + 27, 10526880);
        drawString(fontRenderer, "To prevent level corruption, the current game has quit.", width / 2 - 140, height / 4 - 60 + 60 + 45, 10526880);
        base.render(var1, var2, var3);
    }
}