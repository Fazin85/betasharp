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
            sleep(5000L);
            if (Connection.getReader(this.netManager).isAlive())
            {
                try
                {
                    Connection.getReader(this.netManager).stop();
                }
                catch (Throwable) { }
            }

            if (Connection.getWriter(this.netManager).isAlive())
            {
                try
                {
                    Connection.getWriter(this.netManager).stop();
                }
                catch (Throwable) { }
            }
        }
        catch (InterruptedException ex)
        {
            ex.printStackTrace();
        }

    }
}