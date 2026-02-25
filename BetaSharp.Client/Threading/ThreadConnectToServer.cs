using BetaSharp.Client.Guis;
using BetaSharp.Client.Network;
using BetaSharp.Network.Packets;
using java.net;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Client.Threading;

public class ThreadConnectToServer(GuiConnecting var1, Minecraft var2, string var3, int var4) : java.lang.Thread
{
    private readonly Minecraft _mc = var2;
    private readonly string _hostName = var3;
    private readonly int _port = var4;
    private readonly GuiConnecting _connectingGui = var1;
    private readonly ILogger<ThreadConnectToServer> _logger = Log.Instance.For<ThreadConnectToServer>();

    public override void run()
    {
        try
        {
            GuiConnecting.setNetClientHandler(_connectingGui, new ClientNetworkHandler(_mc, _hostName, _port));

            if (GuiConnecting.isCancelled(_connectingGui))
            {
                return;
            }

            GuiConnecting.getNetClientHandler(_connectingGui).addToSendQueue(new HandshakePacket(_mc.session.username));
        }
        catch (UnknownHostException)
        {
            if (GuiConnecting.isCancelled(_connectingGui))
            {
                return;
            }

            _mc.displayGuiScreen(new GuiConnectFailed("connect.failed", "disconnect.genericReason", "Unknown host \'" + _hostName + "\'"));
        }
        catch (ConnectException ex)
        {
            if (GuiConnecting.isCancelled(_connectingGui))
            {
                return;
            }

            _mc.displayGuiScreen(new GuiConnectFailed("connect.failed", "disconnect.genericReason", ex.getMessage()));
        }
        catch (Exception e)
        {
            if (GuiConnecting.isCancelled(_connectingGui))
            {
                return;
            }

            _logger.LogError(e, e.Message);
            _mc.displayGuiScreen(new GuiConnectFailed("connect.failed", "disconnect.genericReason", e.ToString()));
        }

    }
}
