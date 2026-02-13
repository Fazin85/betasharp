using BetaSharp.Blocks;

namespace BetaSharp.Worlds.Gen.Features;

public class NetherLavaSpringFeature : Feature
{

    private int lavaBlockId;

    public NetherLavaSpringFeature(int var1)
    {
        lavaBlockId = var1;
    }

    public override bool generate(World var1, java.util.Random var2, int var3, int var4, int var5)
    {
        if (var1.getBlockId(var3, var4 + 1, var5) != Block.NETHERRACK.id)
        {
            return false;
        }
        else if (var1.getBlockId(var3, var4, var5) != 0 && var1.getBlockId(var3, var4, var5) != Block.NETHERRACK.id)
        {
            return false;
        }
        else
        {
            int var6 = 0;
            if (var1.getBlockId(var3 - 1, var4, var5) == Block.NETHERRACK.id)
            {
                ++var6;
            }

            if (var1.getBlockId(var3 + 1, var4, var5) == Block.NETHERRACK.id)
            {
                ++var6;
            }

            if (var1.getBlockId(var3, var4, var5 - 1) == Block.NETHERRACK.id)
            {
                ++var6;
            }

            if (var1.getBlockId(var3, var4, var5 + 1) == Block.NETHERRACK.id)
            {
                ++var6;
            }

            if (var1.getBlockId(var3, var4 - 1, var5) == Block.NETHERRACK.id)
            {
                ++var6;
            }

            int var7 = 0;
            if (var1.isAir(var3 - 1, var4, var5))
            {
                ++var7;
            }

            if (var1.isAir(var3 + 1, var4, var5))
            {
                ++var7;
            }

            if (var1.isAir(var3, var4, var5 - 1))
            {
                ++var7;
            }

            if (var1.isAir(var3, var4, var5 + 1))
            {
                ++var7;
            }

            if (var1.isAir(var3, var4 - 1, var5))
            {
                ++var7;
            }

            if (var6 == 4 && var7 == 1)
            {
                var1.setBlock(var3, var4, var5, lavaBlockId);
                var1.instantBlockUpdateEnabled = true;
                Block.BLOCKS[lavaBlockId].onTick(var1, var3, var4, var5, var2);
                var1.instantBlockUpdateEnabled = false;
            }

            return true;
        }
    }
}