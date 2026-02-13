using BetaSharp.Threading;
using java.io;
using java.lang;
using java.util;

namespace BetaSharp.Stats;

public class StatsSyncer
{
    private volatile bool field_27438_a = false;
    private volatile Map field_27437_b = null;
    private volatile Map field_27436_c = null;
    private StatFileWriter field_27435_d;
    private java.io.File field_27434_e;
    private java.io.File field_27433_f;
    private java.io.File field_27432_g;
    private java.io.File field_27431_h;
    private java.io.File field_27430_i;
    private java.io.File field_27429_j;
    private Session field_27428_k;
    private int field_27427_l = 0;
    private int field_27426_m = 0;

    public StatsSyncer(Session var1, StatFileWriter var2, java.io.File var3)
    {
        field_27434_e = new java.io.File(var3, "stats_" + var1.username.ToLower() + "_unsent.dat");
        field_27433_f = new java.io.File(var3, "stats_" + var1.username.ToLower() + ".dat");
        field_27430_i = new java.io.File(var3, "stats_" + var1.username.ToLower() + "_unsent.old");
        field_27429_j = new java.io.File(var3, "stats_" + var1.username.ToLower() + ".old");
        field_27432_g = new java.io.File(var3, "stats_" + var1.username.ToLower() + "_unsent.tmp");
        field_27431_h = new java.io.File(var3, "stats_" + var1.username.ToLower() + ".tmp");
        if (!var1.username.ToLower().Equals(var1.username))
        {
            func_28214_a(var3, "stats_" + var1.username + "_unsent.dat", field_27434_e);
            func_28214_a(var3, "stats_" + var1.username + ".dat", field_27433_f);
            func_28214_a(var3, "stats_" + var1.username + "_unsent.old", field_27430_i);
            func_28214_a(var3, "stats_" + var1.username + ".old", field_27429_j);
            func_28214_a(var3, "stats_" + var1.username + "_unsent.tmp", field_27432_g);
            func_28214_a(var3, "stats_" + var1.username + ".tmp", field_27431_h);
        }

        field_27435_d = var2;
        field_27428_k = var1;
        if (field_27434_e.exists())
        {
            var2.func_27179_a(func_27415_a(field_27434_e, field_27432_g, field_27430_i));
        }

        func_27418_a();
    }

    private void func_28214_a(java.io.File var1, string var2, java.io.File var3)
    {
        java.io.File var4 = new java.io.File(var1, var2);
        if (var4.exists() && !var4.isDirectory() && !var3.exists())
        {
            var4.renameTo(var3);
        }

    }

    private Map func_27415_a(java.io.File var1, java.io.File var2, java.io.File var3)
    {
        return var1.exists() ? func_27408_a(var1) : (var3.exists() ? func_27408_a(var3) : (var2.exists() ? func_27408_a(var2) : null));
    }

    private Map func_27408_a(java.io.File var1)
    {
        BufferedReader var2 = null;

        try
        {
            var2 = new BufferedReader(new java.io.FileReader(var1));
            string var3 = "";
            StringBuilder var4 = new StringBuilder();

            while (true)
            {
                var3 = var2.readLine();
                if (var3 == null)
                {
                    Map var5 = StatFileWriter.func_27177_a(var4.toString());
                    return var5;
                }

                var4.append(var3);
            }
        }
        catch (java.lang.Exception var15)
        {
            var15.printStackTrace();
        }
        finally
        {
            if (var2 != null)
            {
                try
                {
                    var2.close();
                }
                catch (java.lang.Exception var14)
                {
                    var14.printStackTrace();
                }
            }

        }

        return null;
    }

    private void func_27410_a(Map var1, java.io.File var2, java.io.File var3, java.io.File var4)
    {
        PrintWriter var5 = new PrintWriter(new java.io.FileWriter(var3, false));

        try
        {
            var5.print(StatFileWriter.func_27185_a(field_27428_k.username, "local", var1));
        }
        finally
        {
            var5.close();
        }

        if (var4.exists())
        {
            var4.delete();
        }

        if (var2.exists())
        {
            var2.renameTo(var4);
        }

        var3.renameTo(var2);
    }

    public void func_27418_a()
    {
        if (field_27438_a)
        {
            throw new IllegalStateException("Can\'t get stats from server while StatsSyncher is busy!");
        }
        else
        {
            field_27427_l = 100;
            field_27438_a = true;
            (new ThreadStatSyncerReceive(this)).start();
        }
    }

    public void func_27424_a(Map var1)
    {
        if (field_27438_a)
        {
            throw new IllegalStateException("Can\'t save stats while StatsSyncher is busy!");
        }
        else
        {
            field_27427_l = 100;
            field_27438_a = true;
            (new ThreadStatSyncerSend(this, var1)).start();
        }
    }

    public void syncStatsFileWithMap(Map var1)
    {
        int var2 = 30;

        while (field_27438_a)
        {
            --var2;
            if (var2 <= 0)
            {
                break;
            }

            try
            {
                java.lang.Thread.sleep(100L);
            }
            catch (InterruptedException var10)
            {
                var10.printStackTrace();
            }
        }

        field_27438_a = true;

        try
        {
            func_27410_a(var1, field_27434_e, field_27432_g, field_27430_i);
        }
        catch (java.lang.Exception var8)
        {
            var8.printStackTrace();
        }
        finally
        {
            field_27438_a = false;
        }

    }

    public bool func_27420_b()
    {
        return field_27427_l <= 0 && !field_27438_a && field_27436_c == null;
    }

    public void func_27425_c()
    {
        if (field_27427_l > 0)
        {
            --field_27427_l;
        }

        if (field_27426_m > 0)
        {
            --field_27426_m;
        }

        if (field_27436_c != null)
        {
            field_27435_d.func_27187_c(field_27436_c);
            field_27436_c = null;
        }

        if (field_27437_b != null)
        {
            field_27435_d.func_27180_b(field_27437_b);
            field_27437_b = null;
        }

    }

    public static Map func_27422_a(StatsSyncer var0)
    {
        return var0.field_27437_b;
    }

    public static java.io.File func_27423_b(StatsSyncer var0)
    {
        return var0.field_27433_f;
    }

    public static java.io.File func_27411_c(StatsSyncer var0)
    {
        return var0.field_27431_h;
    }

    public static java.io.File func_27413_d(StatsSyncer var0)
    {
        return var0.field_27429_j;
    }

    public static void func_27412_a(StatsSyncer var0, Map var1, java.io.File var2, java.io.File var3, java.io.File var4)
    {
        var0.func_27410_a(var1, var2, var3, var4);
    }

    public static Map func_27421_a(StatsSyncer var0, Map var1)
    {
        return var0.field_27437_b = var1;
    }

    public static Map func_27409_a(StatsSyncer var0, java.io.File var1, java.io.File var2, java.io.File var3)
    {
        return var0.func_27415_a(var1, var2, var3);
    }

    public static bool func_27416_a(StatsSyncer var0, bool var1)
    {
        return var0.field_27438_a = var1;
    }

    public static java.io.File func_27414_e(StatsSyncer var0)
    {
        return var0.field_27434_e;
    }

    public static java.io.File func_27417_f(StatsSyncer var0)
    {
        return var0.field_27432_g;
    }

    public static java.io.File func_27419_g(StatsSyncer var0)
    {
        return var0.field_27430_i;
    }
}