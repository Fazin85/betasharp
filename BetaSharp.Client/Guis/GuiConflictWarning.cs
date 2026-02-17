namespace BetaSharp.Client.Guis;

public class GuiConflictWarning : GuiScreen
{

    private int updateCounter = 0;

    public override void UpdateScreen()
    {
        ++updateCounter;
    }

    public override void InitGui()
    {
        _controlList.Clear();
        _controlList.Add(new GuiButton(0, Width / 2 - 100, Height / 4 + 120 + 12, "Back to title screen"));
    }

    protected override void ActionPerformed(GuiButton btt)
    {
        if (btt.Enabled)
        {
            if (btt.Id == 0)
            {
                mc.displayGuiScreen(new GuiMainMenu());
            }
        }
    }

    public override void Render(int mouseX, int mouseY, float parcialTick)
    {
        DrawDefaultBackground();
        DrawCenteredString(fontRenderer, "Level save conflict", Width / 2, Height / 4 - 60 + 20, 0x00FFFFFF);
        DrawString(fontRenderer, "Minecraft detected a conflict in the level save data.", Width / 2 - 140, Height / 4 - 60 + 60 + 0, 10526880);
        DrawString(fontRenderer, "This could be caused by two copies of the game", Width / 2 - 140, Height / 4 - 60 + 60 + 18, 10526880);
        DrawString(fontRenderer, "accessing the same level.", Width / 2 - 140, Height / 4 - 60 + 60 + 27, 10526880);
        DrawString(fontRenderer, "To prevent level corruption, the current game has quit.", Width / 2 - 140, Height / 4 - 60 + 60 + 45, 10526880);
        base.Render(mouseX, mouseY, parcialTick);
    }
}