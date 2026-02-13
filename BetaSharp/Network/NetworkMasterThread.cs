using java.lang;

namespace BetaSharp.Network;

public class NetworkMasterThread : java.lang.Thread
{
    public readonly Connection netManager;

    public NetworkMasterThread(Connection var1)
    {
        netManager = var1;
    }


    public override void run()
    {
        try
        {
            java.lang.Thread.sleep(5000L);
            if (Connection.getReader(this.netManager).isAlive())
            {
                try
                {
                    Connection.getReader(this.netManager).stop();
                }
                catch (Throwable var3)
                {
                }
            }

            if (Connection.getWriter(this.netManager).isAlive())
            {
                try
                {
                    Connection.getWriter(this.netManager).stop();
                }
                catch (Throwable var2)
                {
                }
            }
        }
        catch (InterruptedException var4)
        {
            var4.printStackTrace();
        }

    }
}