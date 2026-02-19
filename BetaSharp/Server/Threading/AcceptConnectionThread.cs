using BetaSharp.Server.Network;
using java.net;

namespace BetaSharp.Server.Threading;

public class AcceptConnectionThread : java.lang.Thread
{
    private readonly ConnectionListener _listener;

    public AcceptConnectionThread(ConnectionListener listener, string name) : base(name)
    {
        this._listener = listener;
    }

    public override void run()
    {
        Dictionary<InetAddress, long> map = [];

        while (_listener.open)
        {
            try
            {
                Socket socket = _listener.socket.accept();
                if (socket != null)
                {
                    socket.setTcpNoDelay(true);
                    InetAddress addr = socket.getInetAddress();
                    if (map.ContainsKey(addr) && !"127.0.0.1".Equals(addr.getHostAddress()) && java.lang.System.currentTimeMillis() - map[addr] < 5000L)
                    {
                        map[addr] = java.lang.System.currentTimeMillis();
                        socket.close();
                    }
                    else
                    {
                        map[addr] = java.lang.System.currentTimeMillis();
                        ServerLoginNetworkHandler handler = new(_listener.server, socket, "Connection # " + _listener.connectionCounter);
                        _listener.AddPendingConnection(handler);
                    }
                }
            }
            catch (java.io.IOException ex)
            {
                ex.printStackTrace();
            }
        }
    }
}
