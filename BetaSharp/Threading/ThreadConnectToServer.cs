using BetaSharp.Client.Guis;
using BetaSharp.Client.Network;
using BetaSharp.Network.Packets;
using java.net;

namespace BetaSharp.Threading;

public class ThreadConnectToServer : java.lang.Thread
{
    readonly Minecraft mc;
    readonly String hostName;
    readonly int port;
    readonly GuiConnecting connectingGui;

    public ThreadConnectToServer(GuiConnecting var1, Minecraft var2, String var3, int var4)
    {
        connectingGui = var1;
        mc = var2;
        hostName = var3;
        port = var4;
    }


    public override void run()
    {
        try
        {
            GuiConnecting.setNetClientHandler(connectingGui, new ClientNetworkHandler(mc, hostName, port));
            if (GuiConnecting.isCancelled(connectingGui))
            {
                return;
            }

            GuiConnecting.getNetClientHandler(connectingGui).addToSendQueue(new HandshakePacket(mc.session.username));
        }
        catch (UnknownHostException var2)
        {
            if (GuiConnecting.isCancelled(connectingGui))
            {
                return;
            }

            mc.displayGuiScreen(new GuiConnectFailed("connect.failed", "disconnect.genericReason", new Object[] { "Unknown host \'" + hostName + "\'" }));
        }
        catch (ConnectException var3)
        {
            if (GuiConnecting.isCancelled(connectingGui))
            {
                return;
            }

            mc.displayGuiScreen(new GuiConnectFailed("connect.failed", "disconnect.genericReason", new Object[] { var3.getMessage() }));
        }
        catch (java.lang.Exception var4)
        {
            if (GuiConnecting.isCancelled(connectingGui))
            {
                return;
            }

            var4.printStackTrace();
            mc.displayGuiScreen(new GuiConnectFailed("connect.failed", "disconnect.genericReason", new Object[] { var4.toString() }));
        }

    }
}