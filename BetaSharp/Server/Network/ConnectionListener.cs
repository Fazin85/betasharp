using BetaSharp.Server.Threading;
using java.lang;
using java.net;
using java.util.logging;

namespace BetaSharp.Server.Network;

public class ConnectionListener
{
    public static Logger LOGGER = Logger.getLogger("Minecraft");
    public ServerSocket socket;
    private java.lang.Thread thread;
    public volatile bool open = false;
    public int connectionCounter = 0;
    private List<ServerLoginNetworkHandler> pendingConnections = [];
    private List<ServerPlayNetworkHandler> connections = [];
    public MinecraftServer server;
    public int port;

    public ConnectionListener(MinecraftServer server, InetAddress address, int port)
    {
        this.server = server;
        socket = new ServerSocket(port, 0, address);
        socket.setPerformancePreferences(0, 2, 1);
        this.port = socket.getLocalPort();
        open = true;
        thread = new AcceptConnectionThread(this, "Listen Thread");
        thread.start();
    }

    public void addConnection(ServerPlayNetworkHandler connection)
    {
        connections.Add(connection);
    }

    public void addPendingConnection(ServerLoginNetworkHandler connection)
    {
        if (connection == null)
        {
            throw new IllegalArgumentException("Got null pendingconnection!");
        }
        else
        {
            pendingConnections.Add(connection);
        }
    }

    public void tick()
    {
        for (int i = 0; i < pendingConnections.Count; i++)
        {
            ServerLoginNetworkHandler connection = pendingConnections[i];

            try
            {
                connection.tick();
            }
            catch (java.lang.Exception ex)
            {
                connection.disconnect("Internal server error");
                LOGGER.log(Level.WARNING, "Failed to handle packet: " + ex, ex);
            }

            if (connection.closed)
            {
                pendingConnections.RemoveAt(i--);
            }

            connection.connection.interrupt();
        }

        for (int i = 0; i < connections.Count; i++)
        {
            ServerPlayNetworkHandler connection = connections[i];

            try
            {
                connection.tick();
            }
            catch (java.lang.Exception ex)
            {
                LOGGER.log(Level.WARNING, "Failed to handle packet: " + ex, (Throwable)ex);
                connection.disconnect("Internal server error");
            }

            if (connection.disconnected)
            {
                connections.RemoveAt(i--);
            }

            connection.connection.interrupt();
        }
    }
}