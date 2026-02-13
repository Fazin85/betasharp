namespace BetaSharp.Client.Colors;

public class WaterColors : java.lang.Object
{
    private static int[] waterBuffer = new int[65536];

    public static void loadColors(int[] waterBuffer)
    {
        WaterColors.waterBuffer = waterBuffer;
    }
}