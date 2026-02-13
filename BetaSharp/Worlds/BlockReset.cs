namespace BetaSharp.Worlds;

public class BlockReset
{
    public int x;
    public int y;
    public int z;
    public int delay;
    public int block;
    public int meta;
    public readonly ClientWorld world;

    public BlockReset(ClientWorld var1, int var2, int var3, int var4, int var5, int var6)
    {
        world = var1;
        x = var2;
        y = var3;
        z = var4;
        delay = 80;
        block = var5;
        meta = var6;
    }
}