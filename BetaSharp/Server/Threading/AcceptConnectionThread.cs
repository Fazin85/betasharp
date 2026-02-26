using System.Net;
using System.Net.Sockets;
using BetaSharp.Server.Network;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Server.Threading;

public class AcceptConnectionThread : java.lang.Thread
{
    private readonly ILogger<AcceptConnectionThread> _logger = Log.Instance.For<AcceptConnectionThread>();
    private readonly ConnectionListener _listener;

    public AcceptConnectionThread(ConnectionListener listener, string name) : base(name)
    {
        _listener = listener;
    }

    public override void run()
    {
        Dictionary<IPAddress, long> map = [];

        while (_listener.open)
        {
            try
            {
                Socket socket = _listener.Socket.Accept();

                socket.NoDelay = true;

                var address = ((IPEndPoint?) socket.RemoteEndPoint)?.Address;

                ArgumentNullException.ThrowIfNull(address);

                if (map.TryGetValue(address, out long id) && ! IPAddress.Loopback.Equals(address) && java.lang.System.currentTimeMillis() - id < 5000L)
                {
                    map[address] = java.lang.System.currentTimeMillis();
                    socket.Close();
                }
                else
                {
                    map[address] = java.lang.System.currentTimeMillis();
                    ServerLoginNetworkHandler handler = new(_listener.server, socket, "Connection # " + _listener.connectionCounter);
                    _listener.AddPendingConnection(handler);
                }
            }
            catch (SocketException ex)
            {
                _logger.LogError(ex, "Failed to accept connection");
            }
        }
    }
}
