using BetaSharp.Network;

namespace BetaSharp.Threading;

public class ThreadCloseConnection : java.lang.Thread
{
    public readonly Connection field_28109_a;

    public ThreadCloseConnection(Connection var1)
    {
        this.field_28109_a = var1;
    }


    public override void run()
    {
        try
        {
            java.lang.Thread.sleep(2000L);
            if (Connection.isOpen(this.field_28109_a))
            {
                Connection.getWriter(this.field_28109_a).interrupt();
                this.field_28109_a.disconnect("disconnect.closed");
            }
        }
        catch (java.lang.Exception ex)
        {
            ex.printStackTrace();
        }

    }
}