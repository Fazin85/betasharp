using java.io;
using java.lang;
using java.util;

namespace BetaSharp.Stats.Achievements;

public class AchievementMap : java.lang.Object
{
    public static AchievementMap instance = new();
    private readonly Map guidMap = new HashMap();

    private AchievementMap()
    {
        try
        {
            BufferedReader var1 = new(new java.io.StringReader(AssetManager.Instance.getAsset("achievement/map.txt").getTextContent()));

            while (true)
            {
                string var2 = var1.readLine();
                if (var2 == null)
                {
                    var1.close();
                    break;
                }

                string[] var3 = var2.Split(',');
                int var4 = Integer.parseInt(var3[0]);
                guidMap.put(Integer.valueOf(var4), var3[1]);
            }
        }
        catch (java.lang.Exception ex)
        {
            ex.printStackTrace();
        }

    }

    public static string getGuid(int var0)
    {
        return (string)instance.guidMap.get(Integer.valueOf(var0));
    }
}