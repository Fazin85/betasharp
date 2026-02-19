using BetaSharp.Stats;

namespace BetaSharp.Threading;

public class ThreadStatSyncerReceive : java.lang.Thread
{
    readonly StatsSyncer field_27231_a;

    public ThreadStatSyncerReceive(StatsSyncer var1)
    {
        field_27231_a = var1;
    }


    public override void run()
    {
        try
        {
            if (StatsSyncer.func_27422_a(field_27231_a) != null)
            {
                StatsSyncer.func_27412_a(field_27231_a, StatsSyncer.func_27422_a(field_27231_a), StatsSyncer.func_27423_b(field_27231_a), StatsSyncer.func_27411_c(field_27231_a), StatsSyncer.func_27413_d(field_27231_a));
            }
            else if (StatsSyncer.func_27423_b(field_27231_a).exists())
            {
                StatsSyncer.func_27421_a(field_27231_a, StatsSyncer.func_27409_a(field_27231_a, StatsSyncer.func_27423_b(field_27231_a), StatsSyncer.func_27411_c(field_27231_a), StatsSyncer.func_27413_d(field_27231_a)));
            }
        }
        catch (java.lang.Exception ex)
        {
            ex.printStackTrace();
        }
        finally
        {
            StatsSyncer.func_27416_a(field_27231_a, false);
        }

    }
}