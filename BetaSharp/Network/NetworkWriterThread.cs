namespace BetaSharp.Network;

public class NetworkWriterThread : java.lang.Thread
{
    public readonly Connection netManager;

    public NetworkWriterThread(Connection var1, string var2) : base(var2)
    {
        this.netManager = var1;
    }


    public override void run()
    {
        object var1 = Connection.LOCK;
        lock (var1)
        {
            ++Connection.WRITE_THREAD_COUNTER;
        }

        while (true)
        {
            bool var13 = false;

            try
            {
                var13 = true;
                if (!Connection.isOpen(this.netManager))
                {
                    var13 = false;
                    break;
                }

                while (Connection.writePacket(this.netManager))
                {
                }

                netManager.waitForSignal(10);

                try
                {
                    if (Connection.getOutputStream(this.netManager) != null)
                    {
                        Connection.getOutputStream(this.netManager).flush();
                    }
                }
                catch (java.io.IOException var18)
                {
                    if (!Connection.isDisconnected(this.netManager))
                    {
                        Connection.disconnect(this.netManager, var18);
                        var18.printStackTrace();
                    }
                }
            }
            finally
            {
                if (var13)
                {
                    object var5 = Connection.LOCK;
                    lock (var5)
                    {
                        --Connection.WRITE_THREAD_COUNTER;
                    }
                }
            }
        }

        var1 = Connection.LOCK;
        lock (var1)
        {
            --Connection.WRITE_THREAD_COUNTER;
        }
    }
}