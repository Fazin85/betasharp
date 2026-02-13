using BetaSharp.Server.Network;
using java.net;
using java.util;

namespace BetaSharp.Server.Threading;

public class AcceptConnectionThread : java.lang.Thread
{
    private readonly ConnectionListener listener;

    public AcceptConnectionThread(ConnectionListener listener, string name) : base(name)
    {
        this.listener = listener;
    }

    public override void run()
    {
        HashMap map = [];

        while (listener.open)
        {
            try
            {
                Socket socket = listener.socket.accept();
                if (socket != null)
                {
                    socket.setTcpNoDelay(true);
                    InetAddress addr = socket.getInetAddress();
                    if (map.containsKey(addr) && !"127.0.0.1".Equals(addr.getHostAddress()) && java.lang.System.currentTimeMillis() - (long)map.get(addr) < 5000L)
                    {
                        map.put(addr, java.lang.System.currentTimeMillis());
                        socket.close();
                    }
                    else
                    {
                        map.put(addr, java.lang.System.currentTimeMillis());
                        ServerLoginNetworkHandler handler = new(listener.server, socket, "Connection # " + listener.connectionCounter);
                        listener.addPendingConnection(handler);
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