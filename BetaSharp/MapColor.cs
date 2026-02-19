namespace BetaSharp;

public class MapColor : java.lang.Object
{
    public static readonly MapColor[] mapColorArray = new MapColor[16];
    public static readonly MapColor airColor =      new(0,  0x000000);
    public static readonly MapColor grassColor =    new(1,  0x7FB238);
    public static readonly MapColor sandColor =     new(2,  0xF7E9A3);
    public static readonly MapColor clothColor =    new(3,  0xA7A7A7);
    public static readonly MapColor tntColor =      new(4,  0xFF0000);
    public static readonly MapColor iceColor =      new(5,  0xA0A0FF);
    public static readonly MapColor ironColor =     new(6,  0xA7A7A7);
    public static readonly MapColor foliageColor =  new(7,  0x007C00);
    public static readonly MapColor snowColor =     new(8,  0xFFFFFF);
    public static readonly MapColor clayColor =     new(9,  0xA4A8B8);
    public static readonly MapColor dirtColor =     new(10, 0xB76A2F);
    public static readonly MapColor stoneColor =    new(11, 0x707070);
    public static readonly MapColor waterColor =    new(12, 0x4040FF);
    public static readonly MapColor woodColor =     new(13, 0x685332);
    public readonly uint colorValue;
    public readonly int colorIndex;

    private MapColor(int var1, uint var2)
    {
        colorIndex = var1;
        colorValue = var2;
        mapColorArray[var1] = this;
    }
}
