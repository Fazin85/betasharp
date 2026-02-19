using BetaSharp.Network;
using BetaSharp.Server.Threading;
using java.lang;
using java.net;
using java.util.logging;

namespace BetaSharp.Server.Network;

public class ConnectionListener
{
    public ServerSocket socket;
    private readonly java.lang.Thread _thread;
    public volatile bool open;
    public int connectionCounter = 0;
    private readonly List<ServerLoginNetworkHandler> _pendingConnections = [];
    private readonly List<ServerPlayNetworkHandler> _connections = [];
    public MinecraftServer server;
    public int port;

    public ConnectionListener(MinecraftServer server, InetAddress address, int port)
    {
        this.server = server;
        socket = new ServerSocket(port, 0, address);
        socket.setPerformancePreferences(0, 2, 1);
        this.port = socket.getLocalPort();
        open = true;
        _thread = new AcceptConnectionThread(this, "Listen Thread");
        _thread.start();
    }

    public ConnectionListener(MinecraftServer server)
    {
        this.server = server;
        socket = null;
        port = 0;
        open = true;
        _thread = null;
    }

    public void AddConnection(ServerPlayNetworkHandler connection)
    {
        _connections.Add(connection);
    }

    public void AddPendingConnection(ServerLoginNetworkHandler connection)
    {
        if (connection == null)
        {
            throw new IllegalArgumentException("Got null pendingconnection!");
        }
        else
        {
            _pendingConnections.Add(connection);
        }
    }

    public void AddInternalConnection(InternalConnection connection)
    {
        ServerLoginNetworkHandler loginHandler = new(server, connection);
        _pendingConnections.Add(loginHandler);
    }

    public void Tick()
    {
        for (int i = 0; i < _pendingConnections.Count; i++)
        {
            ServerLoginNetworkHandler connection = _pendingConnections[i];

            try
            {
                connection.tick();
            }
            catch (java.lang.Exception ex)
            {
                connection.disconnect("Internal server error");
                Log.Error($"Failed to handle packet: {ex}");
            }

            if (connection.closed)
            {
                _pendingConnections.RemoveAt(i--);
            }

            connection.connection.interrupt();
        }

        for (int i = 0; i < _connections.Count; i++)
        {
            ServerPlayNetworkHandler connection = _connections[i];

            try
            {
                connection.tick();
            }
            catch (java.lang.Exception ex)
            {
                Log.Error($"Failed to handle packet: {ex}");
                connection.disconnect("Internal server error");
            }

            if (connection.disconnected)
            {
                _connections.RemoveAt(i--);
            }

            connection.connection.interrupt();
        }
    }
}
