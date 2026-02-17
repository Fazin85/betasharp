using BetaSharp.Client.Network;
using BetaSharp.Network.Packets.Play;

namespace BetaSharp.Client.Guis;

public class GuiDownloadTerrain : GuiScreen
{

    private readonly ClientNetworkHandler networkHandler;
    private int tickCounter = 0;

    public GuiDownloadTerrain(ClientNetworkHandler networkHandler)
    {
        this.networkHandler = networkHandler;
    }

    protected override void KeyTyped(char eventChar, int eventKey)
    {
    }

    public override void InitGui()
    {
        _controlList.Clear();
    }

    public override void UpdateScreen()
    {
        ++tickCounter;
        if (tickCounter % 20 == 0)
        {
            networkHandler.addToSendQueue(new KeepAlivePacket());
        }

        if (networkHandler != null)
        {
            networkHandler.tick();
        }

    }

    public override bool DoesGuiPauseGame()
    {
        return false;
    }

    protected override void ActionPerformed(GuiButton button)
    {
    }

    public override void Render(int mouseX, int mouseY, float partialTicks)
    {
        DrawBackground(0);
        TranslationStorage translations = TranslationStorage.getInstance();
        DrawCenteredString(fontRenderer, translations.translateKey("multiplayer.downloadingTerrain"), Width / 2, Height / 2 - 50, 0x00FFFFFF);
        base.Render(mouseX, mouseY, partialTicks);
    }
}
