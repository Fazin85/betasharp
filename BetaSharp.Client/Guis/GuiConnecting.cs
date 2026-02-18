using BetaSharp.Client.Network;
using BetaSharp.Client.Threading;

namespace BetaSharp.Client.Guis;

public class GuiConnecting : GuiScreen
{

    private ClientNetworkHandler _clientHandler;
    private bool _cancelled = false;
    private const int _buttonCancel = 0;

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

    public override void UpdateScreen()
    {
        if (_clientHandler != null)
        {
            _clientHandler.tick();
        }

    }

    protected override void KeyTyped(char eventChar, int eventKey)
    {
    }

    public override void InitGui()
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        _controlList.Clear();
        _controlList.Add(new GuiButton(_buttonCancel, Width / 2 - 100, Height / 4 + 120 + 12, translations.translateKey("gui.cancel")));
    }

    protected override void ActionPerformed(GuiButton button)
    {
        switch (button.Id)
        {
            case _buttonCancel:
                _cancelled = true;
                _clientHandler?.disconnect();

                mc.displayGuiScreen(new GuiMainMenu());
                break;
        }

    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        DrawDefaultBackground();
        TranslationStorage translations = TranslationStorage.getInstance();
        if (_clientHandler == null)
        {
            DrawCenteredString(FontRenderer, translations.translateKey("connect.connecting"), Width / 2, Height / 2 - 50, 0x00FFFFFF);
            DrawCenteredString(FontRenderer, "", Width / 2, Height / 2 - 10, 0x00FFFFFF);
        }
        else
        {
            DrawCenteredString(FontRenderer, translations.translateKey("connect.authorizing"), Width / 2, Height / 2 - 50, 0x00FFFFFF);
            DrawCenteredString(FontRenderer, _clientHandler.field_1209_a, Width / 2, Height / 2 - 10, 0x00FFFFFF);
        }

        base.Render(mouseX, mouseY, partialTicks);
    }

    public override bool DoesGuiPauseGame()
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
