using BetaSharp.Worlds.Chunks;

namespace BetaSharp.Worlds.Gen.Carvers;

public class Carver
{
    protected int field_1306_a = 8;
    protected java.util.Random rand = new();

    public virtual void carve(ChunkSource source, World world, int var3, int var4, byte[] var5)
    {
        int var6 = field_1306_a;
        rand.setSeed(world.getSeed());
        long var7 = rand.nextLong() / 2L * 2L + 1L;
        long var9 = rand.nextLong() / 2L * 2L + 1L;

        for (int var11 = var3 - var6; var11 <= var3 + var6; ++var11)
        {
            for (int var12 = var4 - var6; var12 <= var4 + var6; ++var12)
            {
                rand.setSeed(var11 * var7 + var12 * var9 ^ world.getSeed());
                func_868_a(world, var11, var12, var3, var4, var5);
            }
        }

    }

    protected virtual void func_868_a(World var1, int var2, int var3, int var4, int var5, byte[] var6)
    {
    }
}