using System.Text.Json;
using BetaSharp.Util;
using java.lang;
using java.util;
using File = java.io.File;

namespace BetaSharp.Stats;

public class StatFileWriter
{
    private Map field_25102_a = new HashMap();
    private Map field_25101_b = new HashMap();
    private bool statsExist;
    private StatsSyncer _statsSyncer;

    public StatFileWriter(Session session, java.io.File mcDataDir)
    {
        java.io.File statsFolder = new(mcDataDir, "stats");
        if (!statsFolder.exists())
        {
            statsFolder.mkdir();
        }

        java.io.File[] mcFiles = mcDataDir.listFiles();

        foreach (File file in mcFiles)
        {
            if (file.getName().StartsWith("stats_") && file.getName().EndsWith(".dat"))
            {
                java.io.File statsFile = new(statsFolder, file.getName());
                if (!statsFile.exists())
                {
                    Log.Info($"Relocating {file.getName()}");
                    file.renameTo(statsFile);
                }
            }
        }

        _statsSyncer = new StatsSyncer(session, this, statsFolder);
    }

    public void readStat(StatBase stat, int increment)
    {
        writeStatToMap(field_25101_b, stat, increment);
        writeStatToMap(field_25102_a, stat, increment);
        statsExist = true;
    }

    private void writeStatToMap(Map map, StatBase stat, int increment)
    {
        int current = ((Integer)map.get(stat))?.intValue() ?? 0;
        map.put(stat, Integer.valueOf(current + increment));
    }

    public Map func_27176_a()
    {
        return new HashMap(field_25101_b);
    }

    public void loadStats(Map statsMap)
    {
        if (statsMap != null)
        {
            statsExist = true;
            Iterator keys = statsMap.keySet().iterator();

            while (keys.hasNext())
            {
                StatBase stat = (StatBase)keys.next();
                writeStatToMap(field_25101_b, stat, ((Integer)statsMap.get(stat)).intValue());
                writeStatToMap(field_25102_a, stat, ((Integer)statsMap.get(stat)).intValue());
            }
        }
    }

    public void func_27180_b(Map var1)
    {
        if (var1 != null)
        {
            Iterator var2 = var1.keySet().iterator();

            while (var2.hasNext())
            {
                StatBase var3 = (StatBase)var2.next();
                Integer var4 = (Integer)field_25101_b.get(var3);
                int var5 = var4 == null ? 0 : var4.intValue();
                field_25102_a.put(var3, Integer.valueOf(((Integer)var1.get(var3)).intValue() + var5));
            }

        }
    }

    public void func_27187_c(Map var1)
    {
        if (var1 != null)
        {
            statsExist = true;
            Iterator var2 = var1.keySet().iterator();

            while (var2.hasNext())
            {
                StatBase var3 = (StatBase)var2.next();
                writeStatToMap(field_25101_b, var3, ((Integer)var1.get(var3)).intValue());
            }

        }
    }

    public static java.util.Map createStatsMap(string statsFileContents)
    {
        java.util.HashMap statsMap = new java.util.HashMap();
        try
        {
            java.lang.StringBuilder sb = new java.lang.StringBuilder();

            // Parse JSON using System.Text.Json
            using JsonDocument statsJson = JsonDocument.Parse(statsFileContents);
            JsonElement root = statsJson.RootElement;

            // Get the "stats-change" array
            if (root.TryGetProperty("stats-change", out JsonElement statsChangeArray))
            {
                foreach (JsonElement statJson in statsChangeArray.EnumerateArray())
                {
                    // Each element should be an object with one key-value pair
                    JsonProperty var9 = statJson.EnumerateObject().First();

                    int var10 = java.lang.Integer.parseInt(var9.Name);
                    int var11 = var9.Value.ValueKind == JsonValueKind.Number
                        ? var9.Value.GetInt32()
                        : java.lang.Integer.parseInt(var9.Value.GetString());

                    StatBase var12 = Stats.getStatById(var10);
                    if (var12 == null)
                    {
                        Log.Info($"{var10} is not a valid stat");
                    }
                    else
                    {
                        sb.append(Stats.getStatById(var10).statGuid).append(",");
                        sb.append(var11).append(",");
                        statsMap.put(var12, java.lang.Integer.valueOf(var11));
                    }
                }
            }

            string statsChecksum = new MD5String("local").hash(sb.toString());

            if (root.TryGetProperty("checksum", out JsonElement checksumElement))
            {
                string checksum = checksumElement.GetString();
                if (!statsChecksum.Equals(checksum))
                {
                    Log.Info("CHECKSUM MISMATCH");
                    return null;
                }
            }
            else
            {
                Log.Info("CHECKSUM MISMATCH");
                return null;
            }
        }
        catch (JsonException ex)
        {
            Log.Error(ex);
        }

        return statsMap;
    }

    public static string func_27185_a(string username, string salt, Map statsMap)
    {
        StringBuilder var3 = new StringBuilder();
        bool var5 = true;
        var3.append("{\r\n");
        if (username != null && salt != null)
        {
            var3.append("  \"user\":{\r\n");
            var3.append("    \"name\":\"").append(username).append("\",\r\n");
            var3.append("    \"sessionid\":\"").append(salt).append("\"\r\n");
            var3.append("  },\r\n");
        }

        var3.append("  \"stats-change\":[");
        Iterator var6 = statsMap.keySet().iterator();

        StringBuilder var4 = new StringBuilder();
        while (var6.hasNext())
        {
            StatBase var7 = (StatBase)var6.next();
            if (!var5)
            {
                var3.append("},");
            }
            else
            {
                var5 = false;
            }

            var3.append("\r\n    {\"").append(var7.id).append("\":").append(statsMap.get(var7));
            var4.append(var7.statGuid).append(",");
            var4.append(statsMap.get(var7)).append(",");
        }

        if (!var5)
        {
            var3.append("}");
        }

        MD5String var8 = new MD5String(salt);
        var3.append("\r\n  ],\r\n");
        var3.append("  \"checksum\":\"").append(var8.hash(var4.toString())).append("\"\r\n");
        var3.append("}");
        return var3.toString();
    }

    public bool hasAchievementUnlocked(Achievement achievement)
    {
        return field_25102_a.containsKey(achievement);
    }

    public bool func_27181_b(Achievement var1)
    {
        return var1.parent == null || hasAchievementUnlocked(var1.parent);
    }

    public int writeStat(StatBase var1)
    {
        Integer var2 = (Integer)field_25102_a.get(var1);
        return var2 == null ? 0 : var2.intValue();
    }

    public void func_27175_b()
    {
    }

    public void syncStats()
    {
        _statsSyncer.syncStatsFileWithMap(func_27176_a());
    }

    public void func_27178_d()
    {
        if (statsExist && _statsSyncer.func_27420_b())
        {
            _statsSyncer.sendStats(func_27176_a());
        }

        _statsSyncer.func_27425_c();
    }
}
