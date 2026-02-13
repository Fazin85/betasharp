using System.Text.Json;
using BetaSharp.Util;
using java.lang;
using java.util;

namespace BetaSharp.Stats;

public class StatFileWriter
{
    private Map field_25102_a = new HashMap();
    private Map field_25101_b = new HashMap();
    private bool field_27189_c = false;
    private StatsSyncer _statsSyncer;

    public StatFileWriter(Session var1, java.io.File var2)
    {
        java.io.File var3 = new(var2, "stats");
        if (!var3.exists())
        {
            var3.mkdir();
        }

        java.io.File[] var4 = var2.listFiles();
        int var5 = var4.Length;

        for (int var6 = 0; var6 < var5; ++var6)
        {
            java.io.File var7 = var4[var6];
            if (var7.getName().StartsWith("stats_") && var7.getName().EndsWith(".dat"))
            {
                java.io.File var8 = new(var3, var7.getName());
                if (!var8.exists())
                {
                    java.lang.System.@out.println("Relocating " + var7.getName());
                    var7.renameTo(var8);
                }
            }
        }

        _statsSyncer = new StatsSyncer(var1, this, var3);
    }

    public void readStat(StatBase var1, int var2)
    {
        writeStatToMap(field_25101_b, var1, var2);
        writeStatToMap(field_25102_a, var1, var2);
        field_27189_c = true;
    }

    private void writeStatToMap(Map var1, StatBase var2, int var3)
    {
        Integer var4 = (Integer)var1.get(var2);
        int var5 = var4 == null ? 0 : var4.intValue();
        var1.put(var2, Integer.valueOf(var5 + var3));
    }

    public Map func_27176_a()
    {
        return new HashMap(field_25101_b);
    }

    public void func_27179_a(Map var1)
    {
        if (var1 != null)
        {
            field_27189_c = true;
            Iterator var2 = var1.keySet().iterator();

            while (var2.hasNext())
            {
                StatBase var3 = (StatBase)var2.next();
                writeStatToMap(field_25101_b, var3, ((Integer)var1.get(var3)).intValue());
                writeStatToMap(field_25102_a, var3, ((Integer)var1.get(var3)).intValue());
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
            field_27189_c = true;
            Iterator var2 = var1.keySet().iterator();

            while (var2.hasNext())
            {
                StatBase var3 = (StatBase)var2.next();
                writeStatToMap(field_25101_b, var3, ((Integer)var1.get(var3)).intValue());
            }

        }
    }

    public static java.util.Map func_27177_a(string var0)
    {
        java.util.HashMap var1 = new java.util.HashMap();
        try
        {
            string var2 = "local";
            java.lang.StringBuilder var3 = new java.lang.StringBuilder();

            // Parse JSON using System.Text.Json
            using JsonDocument var4 = JsonDocument.Parse(var0);
            JsonElement root = var4.RootElement;

            // Get the "stats-change" array
            if (root.TryGetProperty("stats-change", out JsonElement statsChangeArray))
            {
                foreach (JsonElement var7 in statsChangeArray.EnumerateArray())
                {
                    // Each element should be an object with one key-value pair
                    JsonProperty var9 = var7.EnumerateObject().First();

                    int var10 = java.lang.Integer.parseInt(var9.Name);
                    int var11 = var9.Value.ValueKind == JsonValueKind.Number
                        ? var9.Value.GetInt32()
                        : java.lang.Integer.parseInt(var9.Value.GetString());

                    StatBase var12 = Stats.getStatById(var10);
                    if (var12 == null)
                    {
                        java.lang.System.@out.println(var10 + " is not a valid stat");
                    }
                    else
                    {
                        var3.append(Stats.getStatById(var10).statGuid).append(",");
                        var3.append(var11).append(",");
                        var1.put(var12, java.lang.Integer.valueOf(var11));
                    }
                }
            }

            MD5String var14 = new MD5String(var2);
            string var15 = var14.func_27369_a(var3.toString());

            if (root.TryGetProperty("checksum", out JsonElement checksumElement))
            {
                string checksum = checksumElement.GetString();
                if (!var15.Equals(checksum))
                {
                    java.lang.System.@out.println("CHECKSUM MISMATCH");
                    return null;
                }
            }
            else
            {
                java.lang.System.@out.println("CHECKSUM MISMATCH");
                return null;
            }
        }
        catch (JsonException var13)
        {
            java.lang.System.@out.println(var13.ToString());
        }

        return var1;
    }

    public static string func_27185_a(string var0, string var1, Map var2)
    {
        StringBuilder var3 = new StringBuilder();
        StringBuilder var4 = new StringBuilder();
        bool var5 = true;
        var3.append("{\r\n");
        if (var0 != null && var1 != null)
        {
            var3.append("  \"user\":{\r\n");
            var3.append("    \"name\":\"").append(var0).append("\",\r\n");
            var3.append("    \"sessionid\":\"").append(var1).append("\"\r\n");
            var3.append("  },\r\n");
        }

        var3.append("  \"stats-change\":[");
        Iterator var6 = var2.keySet().iterator();

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

            var3.append("\r\n    {\"").append(var7.id).append("\":").append(var2.get(var7));
            var4.append(var7.statGuid).append(",");
            var4.append(var2.get(var7)).append(",");
        }

        if (!var5)
        {
            var3.append("}");
        }

        MD5String var8 = new MD5String(var1);
        var3.append("\r\n  ],\r\n");
        var3.append("  \"checksum\":\"").append(var8.func_27369_a(var4.toString())).append("\"\r\n");
        var3.append("}");
        return var3.toString();
    }

    public bool hasAchievementUnlocked(Achievement var1)
    {
        return field_25102_a.containsKey(var1);
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
        if (field_27189_c && _statsSyncer.func_27420_b())
        {
            _statsSyncer.func_27424_a(func_27176_a());
        }

        _statsSyncer.func_27425_c();
    }
}