namespace BetaSharp;

public class MapColor : java.lang.Object
{
    public static readonly MapColor[] mapColorArray = new MapColor[16];
    public static readonly MapColor airColor = new(0, 0);
    public static readonly MapColor grassColor = new(1, 8368696);
    public static readonly MapColor sandColor = new(2, 16247203);
    public static readonly MapColor clothColor = new(3, 10987431);
    public static readonly MapColor tntColor = new(4, 16711680);
    public static readonly MapColor iceColor = new(5, 10526975);
    public static readonly MapColor ironColor = new(6, 10987431);
    public static readonly MapColor foliageColor = new(7, 31744);
    public static readonly MapColor snowColor = new(8, 0x00FFFFFFu);
    public static readonly MapColor clayColor = new(9, 10791096);
    public static readonly MapColor dirtColor = new(10, 12020271);
    public static readonly MapColor stoneColor = new(11, 7368816);
    public static readonly MapColor waterColor = new(12, 4210943);
    public static readonly MapColor woodColor = new(13, 6837042);
    public readonly uint colorValue;
    public readonly int colorIndex;

    private MapColor(int var1, uint var2)
    {
        colorIndex = var1;
        colorValue = var2;
        mapColorArray[var1] = this;
    }
}