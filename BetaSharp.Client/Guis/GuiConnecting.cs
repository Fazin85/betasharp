using BetaSharp.Client.Network;
using BetaSharp.Client.Threading;

namespace BetaSharp.Client.Guis;

public class GuiConnecting : GuiScreen
{

    private ClientNetworkHandler _clientHandler;
    private bool _cancelled = false;
    private const int BUTTON_CANCEL = 0;

    public GuiConnecting(Minecraft mc, string host, int port)
    {
        java.lang.System.@out.println("Connecting to " + host + ", " + port);
        mc.changeWorld1(null);
        new ThreadConnectToServer(this, mc, host, port).start();
    }

    public GuiConnecting(Minecraft mc, ClientNetworkHandler clientHandler)
    {
        _clientHandler = clientHandler;
        mc.changeWorld1(null);
    }

    public override void updateScreen()
    {
        _clientHandler?.tick();

    }

    protected override void keyTyped(char eventChar, int eventKey)
    {
    }

    public override void initGui()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        controlList.clear();
        controlList.add(new GuiButton(BUTTON_CANCEL, width / 2 - 100, height / 4 + 120 + 12, translations.translateKey("gui.cancel")));
    }

    protected override void actionPerformed(GuiButton button)
    {
        switch (button.id)
        {
            case BUTTON_CANCEL:
                _cancelled = true;
                _clientHandler?.disconnect();

                mc.displayGuiScreen(new GuiMainMenu());
                break;
        }

    }

    public override void render(int mouseX, int mouseY, float partialTicks)
    {
        drawDefaultBackground();
        TranslationStorage translations = TranslationStorage.getInstance();
        if (_clientHandler == null)
        {
            drawCenteredString(fontRenderer, translations.translateKey("connect.connecting"), width / 2, height / 2 - 50, 0x00FFFFFF);
            drawCenteredString(fontRenderer, "", width / 2, height / 2 - 10, 0x00FFFFFF);
        }
        else
        {
            drawCenteredString(fontRenderer, translations.translateKey("connect.authorizing"), width / 2, height / 2 - 50, 0x00FFFFFF);
            drawCenteredString(fontRenderer, _clientHandler.field_1209_a, width / 2, height / 2 - 10, 0x00FFFFFF);
        }

        base.render(mouseX, mouseY, partialTicks);
    }

    public override bool doesGuiPauseGame()
    {
        return false;
    }

    public static ClientNetworkHandler setNetClientHandler(GuiConnecting guiConnecting, ClientNetworkHandler handler)
    {
        return guiConnecting._clientHandler = handler;
    }

    public static bool isCancelled(GuiConnecting guiConnecting)
    {
        return guiConnecting._cancelled;
    }

    public static ClientNetworkHandler getNetClientHandler(GuiConnecting guiConnecting)
    {
        return guiConnecting._clientHandler;
    }
}
