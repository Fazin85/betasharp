namespace BetaSharp;

public class MapCoord
{
    public byte type;
    public byte x;
    public byte z;
    public byte rotation;
    readonly MapState mapState;

    public MapCoord(MapState var1, byte var2, byte var3, byte var4, byte var5)
    {
        mapState = var1;
        type = var2;
        x = var3;
        z = var4;
        rotation = var5;
    }
}