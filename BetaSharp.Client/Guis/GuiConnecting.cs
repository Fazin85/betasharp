using BetaSharp.Client.Network;
using BetaSharp.Client.Threading;

namespace BetaSharp.Client.Guis;

public class GuiConnecting : GuiScreen
{

    private ClientNetworkHandler clientHandler;
    private bool cancelled = false;
    private const int BUTTON_CANCEL = 0;

    public GuiConnecting(Minecraft mc, string host, int port)
    {
        java.lang.System.@out.println("Connecting to " + host + ", " + port);
        mc.changeWorld1(null);
        new ThreadConnectToServer(this, mc, host, port).start();
    }

    public override void UpdateScreen()
    {
        if (clientHandler != null)
        {
            clientHandler.tick();
        }

    }

    protected override void KeyTyped(char eventChar, int eventKey)
    {
    }

    public override void InitGui()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        controlList.Clear();
        controlList.Add(new GuiButton(BUTTON_CANCEL, Width / 2 - 100, Height / 4 + 120 + 12, translations.translateKey("gui.cancel")));
    }

    protected override void ActionPerformed(GuiButton button)
    {
        switch (button.Id)
        {
            case BUTTON_CANCEL:
                cancelled = true;
                if (clientHandler != null)
                {
                    clientHandler.disconnect();
                }

                mc.displayGuiScreen(new GuiMainMenu());
                break;
        }

    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        DrawDefaultBackground();
        TranslationStorage translations = TranslationStorage.getInstance();
        if (clientHandler == null)
        {
            DrawCenteredString(fontRenderer, translations.translateKey("connect.connecting"), Width / 2, Height / 2 - 50, 0x00FFFFFF);
            DrawCenteredString(fontRenderer, "", Width / 2, Height / 2 - 10, 0x00FFFFFF);
        }
        else
        {
            DrawCenteredString(fontRenderer, translations.translateKey("connect.authorizing"), Width / 2, Height / 2 - 50, 0x00FFFFFF);
            DrawCenteredString(fontRenderer, clientHandler.field_1209_a, Width / 2, Height / 2 - 10, 0x00FFFFFF);
        }

        base.Render(mouseX, mouseY, partialTicks);
    }

    public override bool DoesGuiPauseGame()
    {
        return false;
    }

    public static ClientNetworkHandler setNetClientHandler(GuiConnecting guiConnecting, ClientNetworkHandler handler)
    {
        return guiConnecting.clientHandler = handler;
    }

    public static bool isCancelled(GuiConnecting guiConnecting)
    {
        return guiConnecting.cancelled;
    }

    public static ClientNetworkHandler getNetClientHandler(GuiConnecting guiConnecting)
    {
        return guiConnecting.clientHandler;
    }
}
