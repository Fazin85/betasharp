namespace BetaSharp.Client.Guis;

public class GuiErrorScreen : GuiScreen
{

    private int tickCounter = 0;

    public override void updateScreen()
    {
        ++tickCounter;
    }

    public override void initGui()
    {
    }

    protected override void actionPerformed(GuiButton var1)
    {
    }

    protected override void keyTyped(char eventChar, int eventKey)
    {
    }

    public override void render(int mouseX, int mouseY, float partialTicks)
    {
        drawDefaultBackground();
        drawCenteredString(fontRenderer, "Out of memory!", width / 2, height / 4 - 60 + 20, 16777215);
        drawString(fontRenderer, "Minecraft has run out of memory.", width / 2 - 140, height / 4 - 60 + 60 + 0, 10526880);
        drawString(fontRenderer, "This could be caused by a bug in the game or by the", width / 2 - 140, height / 4 - 60 + 60 + 18, 10526880);
        drawString(fontRenderer, "Java Virtual Machine not being allocated enough", width / 2 - 140, height / 4 - 60 + 60 + 27, 10526880);
        drawString(fontRenderer, "memory. If you are playing in a web browser, try", width / 2 - 140, height / 4 - 60 + 60 + 36, 10526880);
        drawString(fontRenderer, "downloading the game and playing it offline.", width / 2 - 140, height / 4 - 60 + 60 + 45, 10526880);
        drawString(fontRenderer, "To prevent level corruption, the current game has quit.", width / 2 - 140, height / 4 - 60 + 60 + 63, 10526880);
        drawString(fontRenderer, "Please restart the game.", width / 2 - 140, height / 4 - 60 + 60 + 81, 10526880);
        base.render(mouseX, mouseY, partialTicks);
    }
}