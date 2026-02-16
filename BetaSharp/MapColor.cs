namespace BetaSharp;

public class MapColor : java.lang.Object
{
    public static readonly MapColor[] mapColorArray = new MapColor[16];
    public static readonly MapColor airColor =      new(0,  0x00000000u);
    public static readonly MapColor grassColor =    new(1,  0x007FB238u);
    public static readonly MapColor sandColor =     new(2,  0x00F7E9A3u);
    public static readonly MapColor clothColor =    new(3,  0x00A7A7A7u);
    public static readonly MapColor tntColor =      new(4,  0x00FF0000u);
    public static readonly MapColor iceColor =      new(5,  0x00A0A0FFu);
    public static readonly MapColor ironColor =     new(6,  0x00A7A7A7u);
    public static readonly MapColor foliageColor =  new(7,  0x00007C00u);
    public static readonly MapColor snowColor =     new(8,  0x00FFFFFFu);
    public static readonly MapColor clayColor =     new(9,  0x00A4A8B8u);
    public static readonly MapColor dirtColor =     new(10, 0x00B76A2Fu);
    public static readonly MapColor stoneColor =    new(11, 0x00707070u);
    public static readonly MapColor waterColor =    new(12, 0x004040FFu);
    public static readonly MapColor woodColor =     new(13, 0x00685332u);
    public readonly uint colorValue;
    public readonly int colorIndex;

    private MapColor(int var1, uint var2)
    {
        colorIndex = var1;
        colorValue = var2;
        mapColorArray[var1] = this;
    }
}