using BetaSharp.Server.Internal;
using BetaSharp.Server.Threading;

namespace BetaSharp.Client.Guis;

public class GuiLevelLoading(string worldDir, long seed) : GuiScreen
{
    private readonly string _worldDir = worldDir;
    private readonly long _seed = seed;
    private bool _serverStarted = false;

    public override void initGui()
    {
        controlList.clear();
        if (!_serverStarted)
        {
            _serverStarted = true;
            mc.internalServer = new InternalServer(System.IO.Path.Combine(Minecraft.getMinecraftDir().getAbsolutePath(), "saves"), _worldDir, _seed.ToString(), 10);
            new RunServerThread(mc.internalServer, "InternalServer").start();
        }
    }

    public override void updateScreen()
    {
        if (mc.internalServer != null)
        {
            if (mc.internalServer.stopped)
            {
                mc.displayGuiScreen(new GuiConnectFailed("connect.failed", "disconnect.genericReason", new object[] { "Internal server stopped unexpectedly" }));
                return;
            }

            if (mc.internalServer.isReady)
            {
                java.lang.Thread.sleep(100);
                mc.displayGuiScreen(new GuiConnecting(mc, "localhost", mc.internalServer.Port));
            }
        }
    }

    public override bool doesGuiPauseGame()
    {
        return false;
    }

    public override void render(int var1, int var2, float var3)
    {
        drawDefaultBackground();
        TranslationStorage var4 = TranslationStorage.getInstance();

        string title = "Loading level";
        string progressMsg = "";
        int progress = 0;

        if (mc.internalServer != null)
        {
            progressMsg = mc.internalServer.progressMessage ?? "Starting server...";
            progress = mc.internalServer.progress;
        }

        drawCenteredString(fontRenderer, title, width / 2, height / 2 - 50, 0x00FFFFFF);
        drawCenteredString(fontRenderer, progressMsg + " (" + progress + "%)", width / 2, height / 2 - 10, 0x00FFFFFF);

        base.render(var1, var2, var3);
    }
}
