using BetaSharp.Threading;

namespace BetaSharp.Network;

internal class NetworkMasterThread : JavaThread
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
                catch (Exception) { }
            }

            if (Connection.getWriter(this.netManager).isAlive())
            {
                try
                {
                    Connection.getWriter(this.netManager).stop();
                }
                catch (Exception) { }
            }
        }
        catch (ThreadInterruptedException) { }
    }
}
