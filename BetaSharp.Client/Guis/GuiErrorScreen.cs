namespace BetaSharp.Client.Guis;

public class GuiErrorScreen : GuiScreen
{

    private int tickCounter = 0;

    public override void UpdateScreen()
    {
        ++tickCounter;
    }

    public override void InitGui()
    {
    }

    protected override void ActionPerformed(GuiButton var1)
    {
    }

    protected override void KeyTyped(char eventChar, int eventKey)
    {
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        DrawDefaultBackground();
        DrawCenteredString(fontRenderer, "Out of memory!", Width / 2, Height / 4 - 60 + 20, 0x00FFFFFF);
        DrawString(fontRenderer, "Minecraft has run out of memory.", Width / 2 - 140, Height / 4 - 60 + 60 + 0, 10526880);
        DrawString(fontRenderer, "This could be caused by a bug in the game or by the", Width / 2 - 140, Height / 4 - 60 + 60 + 18, 10526880);
        DrawString(fontRenderer, "Java Virtual Machine not being allocated enough", Width / 2 - 140, Height / 4 - 60 + 60 + 27, 10526880);
        DrawString(fontRenderer, "memory. If you are playing in a web browser, try", Width / 2 - 140, Height / 4 - 60 + 60 + 36, 10526880);
        DrawString(fontRenderer, "downloading the game and playing it offline.", Width / 2 - 140, Height / 4 - 60 + 60 + 45, 10526880);
        DrawString(fontRenderer, "To prevent level corruption, the current game has quit.", Width / 2 - 140, Height / 4 - 60 + 60 + 63, 10526880);
        DrawString(fontRenderer, "Please restart the game.", Width / 2 - 140, Height / 4 - 60 + 60 + 81, 10526880);
        base.Render(mouseX, mouseY, partialTicks);
    }
}