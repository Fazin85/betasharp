using BetaSharp.Client.Guis;
using BetaSharp.Client.Network;
using BetaSharp.Network.Packets;
using java.net;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Client.Threading;

public class ThreadConnectToServer(GuiConnecting connectingGui, Minecraft mc, string hostName, int port) : java.lang.Thread
{
    private readonly ILogger<ThreadConnectToServer> _logger = Log.Instance.For<ThreadConnectToServer>();

    public override void run()
    {
        try
        {
            _connectingGui.ClientHandler = new(_mc, _hostName, _port);

            if (_connectingGui.Cancelled)
            {
                return;
            }

            _connectingGui.ClientHandler.addToSendQueue(new HandshakePacket(_mc.session.username));
        }
        catch (UnknownHostException)
        {
            if (_connectingGui.Cancelled)
            {
                return;
            }

            _mc.OpenScreen(new GuiConnectFailed("connect.failed", "disconnect.genericReason", "Unknown host \'" + _hostName + "\'"));
        }
        catch (ConnectException ex)
        {
            if (_connectingGui.Cancelled)
            {
                return;
            }

            _mc.OpenScreen(new GuiConnectFailed("connect.failed", "disconnect.genericReason", ex.getMessage()));
        }
        catch (Exception e)
        {
            if (_connectingGui.Cancelled)
            {
                return;
            }

            _logger.LogError(e, e.Message);
            _mc.OpenScreen(new GuiConnectFailed("connect.failed", "disconnect.genericReason", ex.toString()));
        }
    }
}
