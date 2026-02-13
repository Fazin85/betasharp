using BetaSharp.Client.Network;
using BetaSharp.Client.Resource.Language;
using BetaSharp.Network.Packets.Play;

namespace BetaSharp.Client.Guis;

public class GuiDownloadTerrain : GuiScreen
{

    private ClientNetworkHandler networkHandler;
    private int tickCounter = 0;

    public GuiDownloadTerrain(ClientNetworkHandler networkHandler)
    {
        this.networkHandler = networkHandler;
    }

    protected override void keyTyped(char eventChar, int eventKey)
    {
    }

    public override void initGui()
    {
        controlList.clear();
    }

    public override void updateScreen()
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

    protected override void actionPerformed(GuiButton button)
    {
    }

    public override void render(int mouseX, int mouseY, float partialTicks)
    {
        drawBackground(0);
        TranslationStorage translations = TranslationStorage.getInstance();
        drawCenteredString(fontRenderer, translations.translateKey("multiplayer.downloadingTerrain"), width / 2, height / 2 - 50, 16777215);
        base.render(mouseX, mouseY, partialTicks);
    }
}