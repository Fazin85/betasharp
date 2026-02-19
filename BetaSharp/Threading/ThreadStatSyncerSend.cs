using BetaSharp.Stats;
using java.util;

namespace BetaSharp.Threading;

public class ThreadStatSyncerSend : java.lang.Thread
{
    readonly Map field_27233_a;
    readonly StatsSyncer field_27232_b;

    public ThreadStatSyncerSend(StatsSyncer var1, Map var2)
    {
        field_27232_b = var1;
        field_27233_a = var2;
    }


    public override void run()
    {
        try
        {
            StatsSyncer.func_27412_a(field_27232_b, field_27233_a, StatsSyncer.func_27414_e(field_27232_b), StatsSyncer.func_27417_f(field_27232_b), StatsSyncer.func_27419_g(field_27232_b));
        }
        catch (java.lang.Exception ex)
        {
            ex.printStackTrace();
        }
        finally
        {
            StatsSyncer.func_27416_a(field_27232_b, false);
        }

    }
}