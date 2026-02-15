using BetaSharp.Client.Guis;
using BetaSharp.Client.Network;
using BetaSharp.Network;
using BetaSharp.Network.Packets;
using java.net;

namespace BetaSharp.Threading;

public class ThreadConnectToServer(GuiConnecting var1, Minecraft var2, string var3, int var4) : java.lang.Thread
{
    readonly Minecraft mc = var2;
    readonly string hostName = var3;
    readonly int port = var4;
    readonly GuiConnecting connectingGui = var1;

    public override void run()
    {
        try
        {
            if (mc.internalServer != null && (hostName.Equals("127.0.0.1") || hostName.Equals("localhost")))
            {
                InternalConnection clientConnection = new(null, "Internal-Client");
                InternalConnection serverConnection = new(null, "Internal-Server");
                clientConnection.AssignRemote(serverConnection);
                serverConnection.AssignRemote(clientConnection);

                mc.internalServer.connections.addInternalConnection(serverConnection);
                Console.WriteLine("[Internal-Client] Created internal connection");

                GuiConnecting.setNetClientHandler(connectingGui, new ClientNetworkHandler(mc, clientConnection));
            }
            else
            {
                GuiConnecting.setNetClientHandler(connectingGui, new ClientNetworkHandler(mc, hostName, port));
            }

            if (GuiConnecting.isCancelled(connectingGui))
            {
                return;
            }

            GuiConnecting.getNetClientHandler(connectingGui).addToSendQueue(new HandshakePacket(mc.session.username));
        }
        catch (UnknownHostException)
        {
            if (GuiConnecting.isCancelled(connectingGui))
            {
                return;
            }

            mc.displayGuiScreen(new GuiConnectFailed("connect.failed", "disconnect.genericReason", new Object[] { "Unknown host \'" + hostName + "\'" }));
        }
        catch (ConnectException ex)
        {
            if (GuiConnecting.isCancelled(connectingGui))
            {
                return;
            }

            mc.displayGuiScreen(new GuiConnectFailed("connect.failed", "disconnect.genericReason", new Object[] { ex.getMessage() }));
        }
        catch (java.lang.Exception ex)
        {
            if (GuiConnecting.isCancelled(connectingGui))
            {
                return;
            }

            ex.printStackTrace();
            mc.displayGuiScreen(new GuiConnectFailed("connect.failed", "disconnect.genericReason", new Object[] { ex.toString() }));
        }

    }
}