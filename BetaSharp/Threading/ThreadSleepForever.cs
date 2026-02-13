using java.lang;

namespace BetaSharp.Threading;

public class ThreadSleepForever : java.lang.Thread
{
    private readonly Minecraft mc;

    public ThreadSleepForever(Minecraft var1, string var2) : base(var2)
    {
        mc = var1;
        setDaemon(true);
        start();
    }

    public override void run()
    {
        while (mc.running)
        {
            try
            {
                sleep(2147483647L);
            }
            catch (InterruptedException var2)
            {
            }
        }
    }
}