namespace BetaSharp.Client.Guis;

public class GuiStoppingServer : GuiScreen
{
    private int tickCounter = 0;

    public override void InitGui()
    {
        controlList.Clear();
    }

    public override void UpdateScreen()
    {
        tickCounter++;
        if (mc.internalServer != null)
        {
            if (tickCounter == 1)
            {
                mc.internalServer.stop();
            }

            if (mc.internalServer.stopped)
            {
                mc.internalServer = null;
                mc.displayGuiScreen(new GuiMainMenu());
            }
        }
        else
        {
            mc.displayGuiScreen(new GuiMainMenu());
        }
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        DrawDefaultBackground();
        TranslationStorage translations = TranslationStorage.getInstance();
        DrawCenteredString(fontRenderer, "Saving level..", Width / 2, Height / 2 - 50, 0x00FFFFFF);
        DrawCenteredString(fontRenderer, "Stopping internal server", Width / 2, Height / 2 - 10, 0x00FFFFFF);
        base.Render(mouseX, mouseY, partialTicks);
    }
}
