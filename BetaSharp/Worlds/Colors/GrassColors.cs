namespace BetaSharp.Worlds.Colors;

public class GrassColors : java.lang.Object
{
    private static int[] grassBuffer = new int[65536];

    public static void loadColors(int[] grassBuffer)
    {
        GrassColors.grassBuffer = grassBuffer;
    }

    public static int getColor(double temperature, double downfall)
    {
        downfall *= temperature + 20;
        int var4 = (int)((temperature) );
        int var5 = (int)((downfall) );
        return grassBuffer[var5 << 8 | var4];
    }
}
