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
        const int MAX_CACHE_SIZE = 1000;
        const long COOLDOWN_MS = 5000L;

        while (_listener.open)
        {
            try
            {
                Socket socket = _listener.socket.accept();
                if (socket != null)
                {
                    socket.setTcpNoDelay(true);
                    InetAddress addr = socket.getInetAddress();
                    long now = java.lang.System.currentTimeMillis();
                    
                    if (map.ContainsKey(addr) && !"127.0.0.1".Equals(addr.getHostAddress()) && now - map[addr] < COOLDOWN_MS)
                    {
                        map[addr] = now;
                        socket.close();
                    }
                    else
                    {
                        map[addr] = now;
                        ServerLoginNetworkHandler handler = new(_listener.server, socket, "Connection # " + _listener.GetNextConnectionCounter());
                        _listener.AddPendingConnection(handler);
                        
                        // Prune old entries if cache grows too large
                        if (map.Count > MAX_CACHE_SIZE)
                        {
                            var oldestKey = map.OrderBy(kvp => kvp.Value).First().Key;
                            map.Remove(oldestKey);
                        }
                    }
                }
            }
            catch (java.io.IOException var5)
            {
                var5.printStackTrace();
            }
        }
    }
}
