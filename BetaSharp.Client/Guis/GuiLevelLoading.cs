using BetaSharp.Client.Network;
using BetaSharp.Network;
using BetaSharp.Server.Internal;
using BetaSharp.Server.Threading;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Client.Guis;

public class GuiLevelLoading : GuiScreen
{
    private readonly ILogger<GuiLevelLoading> _logger = Log.Instance.For<GuiLevelLoading>();
    private bool _serverStarted;
    public override bool PausesGame => false;

    public GuiLevelLoading(string worldDir, long seed)
    {
        if (!_serverStarted)
        {
            _serverStarted = true;
            mc.internalServer = new(System.IO.Path.Combine(Minecraft.getMinecraftDir().getAbsolutePath(), "saves"),
            worldDir, seed.ToString(), mc.options.renderDistance, mc.options.Difficulty);
            new RunServerThread(mc.internalServer, "InternalServer").start();
        }
    }

    public override void UpdateScreen()
    {
        if (mc.internalServer != null)
        {
            if (mc.internalServer.stopped)
            {
                mc.OpenScreen(new GuiConnectFailed("connect.failed", "disconnect.genericReason",
                    "Internal server stopped unexpectedly"));
                return;
            }

            if (mc.internalServer.isReady)
            {
                InternalConnection clientConnection = new(null, "Internal-Client");
                InternalConnection serverConnection = new(null, "Internal-Server");

                clientConnection.AssignRemote(serverConnection);
                serverConnection.AssignRemote(clientConnection);

                mc.internalServer.connections.AddInternalConnection(serverConnection);
                _logger.LogInformation("[Internal-Client] Created internal connection");

                ClientNetworkHandler clientHandler = new(mc, clientConnection);
                clientConnection.setNetworkHandler(clientHandler);
                clientHandler.addToSendQueue(new BetaSharp.Network.Packets.HandshakePacket(mc.session.username));

                mc.OpenScreen(new GuiConnecting(mc, clientHandler));
            }
        }
    }

    protected override void OnRendered(RenderEventArgs e)
    {
        DrawDefaultBackground();

        string title = "Loading level";
        string progressMsg = "";
        int progress = 0;

        if (mc.internalServer != null)
        {
            progressMsg = mc.internalServer.progressMessage ?? "Starting server...";
            progress = mc.internalServer.progress;
        }

        Gui.DrawCenteredString(FontRenderer, title, Width / 2, Height / 2 - 50, 0xFFFFFF);
        Gui.DrawCenteredString(FontRenderer, progressMsg + " (" + progress + "%)", Width / 2, Height / 2 - 10, 0xFFFFFF);
    }
}
