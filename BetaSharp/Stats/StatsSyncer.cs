using BetaSharp.Threading;
using java.io;
using java.lang;
using java.util;

namespace BetaSharp.Stats;

public class StatsSyncer
{
    private volatile bool busy;
    private volatile Map field_27437_b;
    private volatile Map field_27436_c;
    private StatFileWriter statFileWriter;
    private java.io.File unsentStatsFile;
    private java.io.File statsFile;
    private java.io.File tempUnsentStatsFile;
    private java.io.File tempStatsFile;
    private java.io.File oldUnsentStatsFile;
    private java.io.File oldStatsFile;
    private Session session;
    private int field_27427_l;
    private int field_27426_m;

    public StatsSyncer(Session session, StatFileWriter statFileWriter, java.io.File statsFolder)
    {
        unsentStatsFile = new java.io.File(statsFolder, "stats_" + session.username.ToLower() + "_unsent.dat");
        statsFile = new java.io.File(statsFolder, "stats_" + session.username.ToLower() + ".dat");
        oldUnsentStatsFile = new java.io.File(statsFolder, "stats_" + session.username.ToLower() + "_unsent.old");
        oldStatsFile = new java.io.File(statsFolder, "stats_" + session.username.ToLower() + ".old");
        tempUnsentStatsFile = new java.io.File(statsFolder, "stats_" + session.username.ToLower() + "_unsent.tmp");
        tempStatsFile = new java.io.File(statsFolder, "stats_" + session.username.ToLower() + ".tmp");
        if (!session.username.ToLower().Equals(session.username))
        {
            ensureStatFileIsLowercase(statsFolder, "stats_" + session.username + "_unsent.dat", unsentStatsFile);
            ensureStatFileIsLowercase(statsFolder, "stats_" + session.username + ".dat", statsFile);
            ensureStatFileIsLowercase(statsFolder, "stats_" + session.username + "_unsent.old", oldUnsentStatsFile);
            ensureStatFileIsLowercase(statsFolder, "stats_" + session.username + ".old", oldStatsFile);
            ensureStatFileIsLowercase(statsFolder, "stats_" + session.username + "_unsent.tmp", tempUnsentStatsFile);
            ensureStatFileIsLowercase(statsFolder, "stats_" + session.username + ".tmp", tempStatsFile);
        }

        this.statFileWriter = statFileWriter;
        this.session = session;
        if (unsentStatsFile.exists())
        {
            statFileWriter.loadStats(getNewestAvailableStats(unsentStatsFile, tempUnsentStatsFile, oldUnsentStatsFile));
        }

        receiveStats();
    }

    private void ensureStatFileIsLowercase(java.io.File statsFolder, string fileNameNotLowercase, java.io.File file)
    {
        java.io.File otherFile = new java.io.File(statsFolder, fileNameNotLowercase);
        if (otherFile.exists() && !otherFile.isDirectory() && !file.exists())
        {
            otherFile.renameTo(file);
        }
    }

    private Map getNewestAvailableStats(java.io.File unsentStatsFile, java.io.File tempUnsentStatsFile, java.io.File oldUnsentStatsFile)
    {
        if (unsentStatsFile.exists())
            return createStatsMapFromFile(unsentStatsFile);
        if (oldUnsentStatsFile.exists())
            return createStatsMapFromFile(oldUnsentStatsFile);
        if (tempUnsentStatsFile.exists())
            return createStatsMapFromFile(tempUnsentStatsFile);
        return null;
    }

    private Map createStatsMapFromFile(java.io.File statsFile)
    {
        BufferedReader statsFileReader = null;

        try
        {
            statsFileReader = new BufferedReader(new java.io.FileReader(statsFile));
            StringBuilder sb = new();

            while (true)
            {
                string var3 = statsFileReader.readLine();
                if (var3 == null)
                {
                    Map var5 = StatFileWriter.createStatsMap(sb.toString());
                    return var5;
                }

                sb.append(var3);
            }
        }
        catch (java.lang.Exception ex)
        {
            ex.printStackTrace();
        }
        finally
        {
            if (statsFileReader != null)
            {
                try
                {
                    statsFileReader.close();
                }
                catch (java.lang.Exception ex)
                {
                    ex.printStackTrace();
                }
            }

        }

        return null;
    }

    private void func_27410_a(Map statsMap, java.io.File unsentStatsFile, java.io.File tempUnsentStatsFile, java.io.File oldUnsentStatsFile)
    {
        PrintWriter fileWriter = new PrintWriter(new java.io.FileWriter(tempUnsentStatsFile, false));

        try
        {
            fileWriter.print(StatFileWriter.func_27185_a(session.username, "local", statsMap));
        }
        finally
        {
            fileWriter.close();
        }

        if (oldUnsentStatsFile.exists())
        {
            oldUnsentStatsFile.delete();
        }

        if (unsentStatsFile.exists())
        {
            unsentStatsFile.renameTo(oldUnsentStatsFile);
        }

        tempUnsentStatsFile.renameTo(unsentStatsFile);
    }

    public void receiveStats()
    {
        if (busy)
        {
            throw new IllegalStateException("Can\'t get stats from server while StatsSyncher is busy!");
        }
        else
        {
            field_27427_l = 100;
            busy = true;
            (new ThreadStatSyncerReceive(this)).start();
        }
    }

    public void sendStats(Map var1)
    {
        if (busy)
        {
            throw new IllegalStateException("Can\'t save stats while StatsSyncher is busy!");
        }
        else
        {
            field_27427_l = 100;
            busy = true;
            (new ThreadStatSyncerSend(this, var1)).start();
        }
    }

    public void syncStatsFileWithMap(Map statsMap)
    {
        int waitCycles = 30;

        while (busy)
        {
            --waitCycles;
            if (waitCycles <= 0)
            {
                break;
            }

            try
            {
                java.lang.Thread.sleep(100L);
            }
            catch (InterruptedException ex)
            {
                ex.printStackTrace();
            }
        }

        busy = true;

        try
        {
            func_27410_a(statsMap, unsentStatsFile, tempUnsentStatsFile, oldUnsentStatsFile);
        }
        catch (java.lang.Exception ex)
        {
            ex.printStackTrace();
        }
        finally
        {
            busy = false;
        }

    }

    public bool func_27420_b()
    {
        return field_27427_l <= 0 && !busy && field_27436_c == null;
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
            statFileWriter.func_27187_c(field_27436_c);
            field_27436_c = null;
        }

        if (field_27437_b != null)
        {
            statFileWriter.func_27180_b(field_27437_b);
            field_27437_b = null;
        }

    }

    public static Map func_27422_a(StatsSyncer var0)
    {
        return var0.field_27437_b;
    }

    public static java.io.File func_27423_b(StatsSyncer var0)
    {
        return var0.statsFile;
    }

    public static java.io.File func_27411_c(StatsSyncer var0)
    {
        return var0.tempStatsFile;
    }

    public static java.io.File func_27413_d(StatsSyncer var0)
    {
        return var0.oldStatsFile;
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
        return var0.getNewestAvailableStats(var1, var2, var3);
    }

    public static bool func_27416_a(StatsSyncer var0, bool var1)
    {
        return var0.busy = var1;
    }

    public static java.io.File func_27414_e(StatsSyncer var0)
    {
        return var0.unsentStatsFile;
    }

    public static java.io.File func_27417_f(StatsSyncer var0)
    {
        return var0.tempUnsentStatsFile;
    }

    public static java.io.File func_27419_g(StatsSyncer var0)
    {
        return var0.oldUnsentStatsFile;
    }
}