namespace BetaSharp.Client.Guis;

public class GuiStoppingServer : GuiScreen
{
    private int tickCounter = 0;

    public override void initGui()
    {
        controlList.clear();
    }

    public override void updateScreen()
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

    public override void render(int mouseX, int mouseY, float partialTicks)
    {
        drawDefaultBackground();
        TranslationStorage translations = TranslationStorage.getInstance();
        drawCenteredString(fontRenderer, "Saving level..", width / 2, height / 2 - 50, 0x00FFFFFF);
        drawCenteredString(fontRenderer, "Stopping internal server", width / 2, height / 2 - 10, 0x00FFFFFF);
        base.render(mouseX, mouseY, partialTicks);
    }
}
