using BetaSharp.Blocks.Materials;
using BetaSharp.Items;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockRedstoneWire : Block
{

    private bool wiresProvidePower = true;
    private HashSet<BlockPos> neighbors;

    public BlockRedstoneWire(int var1, int var2) : base(var1, var2, Material.PistonBreakable)
    {
        setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F / 16.0F, 1.0F);
    }

    public override int getTexture(int var1, int var2)
    {
        return textureId;
    }

    public override Box? getCollisionShape(World var1, int var2, int var3, int var4)
    {
        return null;
    }

    public override bool isOpaque()
    {
        return false;
    }

    public override bool isFullCube()
    {
        return false;
    }

    public override int getRenderType()
    {
        return 5;
    }

    public override int getColorMultiplier(BlockView var1, int var2, int var3, int var4)
    {
        return 8388608;
    }

    public override bool canPlaceAt(World var1, int var2, int var3, int var4)
    {
        return var1.shouldSuffocate(var2, var3 - 1, var4);
    }

    private void updateAndPropagateCurrentStrength(World var1, int var2, int var3, int var4)
    {
        func_21030_a(var1, var2, var3, var4, var2, var3, var4);
        List<BlockPos> neighborsCopy = new (neighbors);
        neighbors.Clear();

        foreach (var n in neighborsCopy)
        {
            var1.notifyNeighbors(n.x, n.y, n.z, id);
        }

    }

    private void func_21030_a(World var1, int var2, int var3, int var4, int var5, int var6, int var7)
    {
        int var8 = var1.getBlockMeta(var2, var3, var4);
        int var9 = 0;
        wiresProvidePower = false;
        bool var10 = var1.isPowered(var2, var3, var4);
        wiresProvidePower = true;
        int var11;
        int var12;
        int var13;
        if (var10)
        {
            var9 = 15;
        }
        else
        {
            for (var11 = 0; var11 < 4; ++var11)
            {
                var12 = var2;
                var13 = var4;
                if (var11 == 0)
                {
                    var12 = var2 - 1;
                }

                if (var11 == 1)
                {
                    ++var12;
                }

                if (var11 == 2)
                {
                    var13 = var4 - 1;
                }

                if (var11 == 3)
                {
                    ++var13;
                }

                if (var12 != var5 || var3 != var6 || var13 != var7)
                {
                    var9 = getMaxCurrentStrength(var1, var12, var3, var13, var9);
                }

                if (var1.shouldSuffocate(var12, var3, var13) && !var1.shouldSuffocate(var2, var3 + 1, var4))
                {
                    if (var12 != var5 || var3 + 1 != var6 || var13 != var7)
                    {
                        var9 = getMaxCurrentStrength(var1, var12, var3 + 1, var13, var9);
                    }
                }
                else if (!var1.shouldSuffocate(var12, var3, var13) && (var12 != var5 || var3 - 1 != var6 || var13 != var7))
                {
                    var9 = getMaxCurrentStrength(var1, var12, var3 - 1, var13, var9);
                }
            }

            if (var9 > 0)
            {
                --var9;
            }
            else
            {
                var9 = 0;
            }
        }

        if (var8 != var9)
        {
            var1.pauseTicking = true;
            var1.setBlockMeta(var2, var3, var4, var9);
            var1.setBlocksDirty(var2, var3, var4, var2, var3, var4);
            var1.pauseTicking = false;

            for (var11 = 0; var11 < 4; ++var11)
            {
                var12 = var2;
                var13 = var4;
                int var14 = var3 - 1;
                if (var11 == 0)
                {
                    var12 = var2 - 1;
                }

                if (var11 == 1)
                {
                    ++var12;
                }

                if (var11 == 2)
                {
                    var13 = var4 - 1;
                }

                if (var11 == 3)
                {
                    ++var13;
                }

                if (var1.shouldSuffocate(var12, var3, var13))
                {
                    var14 += 2;
                }

                bool var15 = false;
                int var16 = getMaxCurrentStrength(var1, var12, var3, var13, -1);
                var9 = var1.getBlockMeta(var2, var3, var4);
                if (var9 > 0)
                {
                    --var9;
                }

                if (var16 >= 0 && var16 != var9)
                {
                    func_21030_a(var1, var12, var3, var13, var2, var3, var4);
                }

                var16 = getMaxCurrentStrength(var1, var12, var14, var13, -1);
                var9 = var1.getBlockMeta(var2, var3, var4);
                if (var9 > 0)
                {
                    --var9;
                }

                if (var16 >= 0 && var16 != var9)
                {
                    func_21030_a(var1, var12, var14, var13, var2, var3, var4);
                }
            }

            if (var8 == 0 || var9 == 0)
            {
                neighbors.Add(new BlockPos(var2, var3, var4));
                neighbors.Add(new BlockPos(var2 - 1, var3, var4));
                neighbors.Add(new BlockPos(var2 + 1, var3, var4));
                neighbors.Add(new BlockPos(var2, var3 - 1, var4));
                neighbors.Add(new BlockPos(var2, var3 + 1, var4));
                neighbors.Add(new BlockPos(var2, var3, var4 - 1));
                neighbors.Add(new BlockPos(var2, var3, var4 + 1));
            }
        }

    }

    private void notifyWireNeighborsOfNeighborChange(World var1, int var2, int var3, int var4)
    {
        if (var1.getBlockId(var2, var3, var4) == id)
        {
            var1.notifyNeighbors(var2, var3, var4, id);
            var1.notifyNeighbors(var2 - 1, var3, var4, id);
            var1.notifyNeighbors(var2 + 1, var3, var4, id);
            var1.notifyNeighbors(var2, var3, var4 - 1, id);
            var1.notifyNeighbors(var2, var3, var4 + 1, id);
            var1.notifyNeighbors(var2, var3 - 1, var4, id);
            var1.notifyNeighbors(var2, var3 + 1, var4, id);
        }
    }

    public override void onPlaced(World var1, int var2, int var3, int var4)
    {
        base.onPlaced(var1, var2, var3, var4);
        if (!var1.isRemote)
        {
            updateAndPropagateCurrentStrength(var1, var2, var3, var4);
            var1.notifyNeighbors(var2, var3 + 1, var4, id);
            var1.notifyNeighbors(var2, var3 - 1, var4, id);
            notifyWireNeighborsOfNeighborChange(var1, var2 - 1, var3, var4);
            notifyWireNeighborsOfNeighborChange(var1, var2 + 1, var3, var4);
            notifyWireNeighborsOfNeighborChange(var1, var2, var3, var4 - 1);
            notifyWireNeighborsOfNeighborChange(var1, var2, var3, var4 + 1);
            if (var1.shouldSuffocate(var2 - 1, var3, var4))
            {
                notifyWireNeighborsOfNeighborChange(var1, var2 - 1, var3 + 1, var4);
            }
            else
            {
                notifyWireNeighborsOfNeighborChange(var1, var2 - 1, var3 - 1, var4);
            }

            if (var1.shouldSuffocate(var2 + 1, var3, var4))
            {
                notifyWireNeighborsOfNeighborChange(var1, var2 + 1, var3 + 1, var4);
            }
            else
            {
                notifyWireNeighborsOfNeighborChange(var1, var2 + 1, var3 - 1, var4);
            }

            if (var1.shouldSuffocate(var2, var3, var4 - 1))
            {
                notifyWireNeighborsOfNeighborChange(var1, var2, var3 + 1, var4 - 1);
            }
            else
            {
                notifyWireNeighborsOfNeighborChange(var1, var2, var3 - 1, var4 - 1);
            }

            if (var1.shouldSuffocate(var2, var3, var4 + 1))
            {
                notifyWireNeighborsOfNeighborChange(var1, var2, var3 + 1, var4 + 1);
            }
            else
            {
                notifyWireNeighborsOfNeighborChange(var1, var2, var3 - 1, var4 + 1);
            }

        }
    }

    public override void onBreak(World var1, int var2, int var3, int var4)
    {
        base.onBreak(var1, var2, var3, var4);
        if (!var1.isRemote)
        {
            var1.notifyNeighbors(var2, var3 + 1, var4, id);
            var1.notifyNeighbors(var2, var3 - 1, var4, id);
            updateAndPropagateCurrentStrength(var1, var2, var3, var4);
            notifyWireNeighborsOfNeighborChange(var1, var2 - 1, var3, var4);
            notifyWireNeighborsOfNeighborChange(var1, var2 + 1, var3, var4);
            notifyWireNeighborsOfNeighborChange(var1, var2, var3, var4 - 1);
            notifyWireNeighborsOfNeighborChange(var1, var2, var3, var4 + 1);
            if (var1.shouldSuffocate(var2 - 1, var3, var4))
            {
                notifyWireNeighborsOfNeighborChange(var1, var2 - 1, var3 + 1, var4);
            }
            else
            {
                notifyWireNeighborsOfNeighborChange(var1, var2 - 1, var3 - 1, var4);
            }

            if (var1.shouldSuffocate(var2 + 1, var3, var4))
            {
                notifyWireNeighborsOfNeighborChange(var1, var2 + 1, var3 + 1, var4);
            }
            else
            {
                notifyWireNeighborsOfNeighborChange(var1, var2 + 1, var3 - 1, var4);
            }

            if (var1.shouldSuffocate(var2, var3, var4 - 1))
            {
                notifyWireNeighborsOfNeighborChange(var1, var2, var3 + 1, var4 - 1);
            }
            else
            {
                notifyWireNeighborsOfNeighborChange(var1, var2, var3 - 1, var4 - 1);
            }

            if (var1.shouldSuffocate(var2, var3, var4 + 1))
            {
                notifyWireNeighborsOfNeighborChange(var1, var2, var3 + 1, var4 + 1);
            }
            else
            {
                notifyWireNeighborsOfNeighborChange(var1, var2, var3 - 1, var4 + 1);
            }

        }
    }

    private int getMaxCurrentStrength(World var1, int var2, int var3, int var4, int var5)
    {
        if (var1.getBlockId(var2, var3, var4) != id)
        {
            return var5;
        }
        else
        {
            int var6 = var1.getBlockMeta(var2, var3, var4);
            return var6 > var5 ? var6 : var5;
        }
    }

    public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
    {
        if (!var1.isRemote)
        {
            int var6 = var1.getBlockMeta(var2, var3, var4);
            bool var7 = canPlaceAt(var1, var2, var3, var4);
            if (!var7)
            {
                dropStacks(var1, var2, var3, var4, var6);
                var1.setBlock(var2, var3, var4, 0);
            }
            else
            {
                updateAndPropagateCurrentStrength(var1, var2, var3, var4);
            }

            base.neighborUpdate(var1, var2, var3, var4, var5);
        }
    }

    public override int getDroppedItemId(int var1, java.util.Random var2)
    {
        return Item.REDSTONE.id;
    }

    public override bool isStrongPoweringSide(World var1, int var2, int var3, int var4, int var5)
    {
        return !wiresProvidePower ? false : isPoweringSide(var1, var2, var3, var4, var5);
    }

    public override bool isPoweringSide(BlockView var1, int var2, int var3, int var4, int var5)
    {
        if (!wiresProvidePower)
        {
            return false;
        }
        else if (var1.getBlockMeta(var2, var3, var4) == 0)
        {
            return false;
        }
        else if (var5 == 1)
        {
            return true;
        }
        else
        {
            bool var6 = isPowerProviderOrWire(var1, var2 - 1, var3, var4, 1) || !var1.shouldSuffocate(var2 - 1, var3, var4) && isPowerProviderOrWire(var1, var2 - 1, var3 - 1, var4, -1);
            bool var7 = isPowerProviderOrWire(var1, var2 + 1, var3, var4, 3) || !var1.shouldSuffocate(var2 + 1, var3, var4) && isPowerProviderOrWire(var1, var2 + 1, var3 - 1, var4, -1);
            bool var8 = isPowerProviderOrWire(var1, var2, var3, var4 - 1, 2) || !var1.shouldSuffocate(var2, var3, var4 - 1) && isPowerProviderOrWire(var1, var2, var3 - 1, var4 - 1, -1);
            bool var9 = isPowerProviderOrWire(var1, var2, var3, var4 + 1, 0) || !var1.shouldSuffocate(var2, var3, var4 + 1) && isPowerProviderOrWire(var1, var2, var3 - 1, var4 + 1, -1);
            if (!var1.shouldSuffocate(var2, var3 + 1, var4))
            {
                if (var1.shouldSuffocate(var2 - 1, var3, var4) && isPowerProviderOrWire(var1, var2 - 1, var3 + 1, var4, -1))
                {
                    var6 = true;
                }

                if (var1.shouldSuffocate(var2 + 1, var3, var4) && isPowerProviderOrWire(var1, var2 + 1, var3 + 1, var4, -1))
                {
                    var7 = true;
                }

                if (var1.shouldSuffocate(var2, var3, var4 - 1) && isPowerProviderOrWire(var1, var2, var3 + 1, var4 - 1, -1))
                {
                    var8 = true;
                }

                if (var1.shouldSuffocate(var2, var3, var4 + 1) && isPowerProviderOrWire(var1, var2, var3 + 1, var4 + 1, -1))
                {
                    var9 = true;
                }
            }

            return !var8 && !var7 && !var6 && !var9 && var5 >= 2 && var5 <= 5 ? true : (var5 == 2 && var8 && !var6 && !var7 ? true : (var5 == 3 && var9 && !var6 && !var7 ? true : (var5 == 4 && var6 && !var8 && !var9 ? true : var5 == 5 && var7 && !var8 && !var9)));
        }
    }

    public override bool canEmitRedstonePower()
    {
        return wiresProvidePower;
    }

    public override void randomDisplayTick(World var1, int var2, int var3, int var4, java.util.Random var5)
    {
        int var6 = var1.getBlockMeta(var2, var3, var4);
        if (var6 > 0)
        {
            double var7 = (double)var2 + 0.5D + ((double)var5.nextFloat() - 0.5D) * 0.2D;
            double var9 = (double)((float)var3 + 1.0F / 16.0F);
            double var11 = (double)var4 + 0.5D + ((double)var5.nextFloat() - 0.5D) * 0.2D;
            float var13 = (float)var6 / 15.0F;
            float var14 = var13 * 0.6F + 0.4F;
            if (var6 == 0)
            {
                var14 = 0.0F;
            }

            float var15 = var13 * var13 * 0.7F - 0.5F;
            float var16 = var13 * var13 * 0.6F - 0.7F;
            if (var15 < 0.0F)
            {
                var15 = 0.0F;
            }

            if (var16 < 0.0F)
            {
                var16 = 0.0F;
            }

            var1.addParticle("reddust", var7, var9, var11, (double)var14, (double)var15, (double)var16);
        }

    }

    public static bool isPowerProviderOrWire(BlockView var0, int var1, int var2, int var3, int var4)
    {
        int var5 = var0.getBlockId(var1, var2, var3);
        if (var5 == Block.REDSTONE_WIRE.id)
        {
            return true;
        }
        else if (var5 == 0)
        {
            return false;
        }
        else if (Block.BLOCKS[var5].canEmitRedstonePower())
        {
            return true;
        }
        else if (var5 != Block.REPEATER.id && var5 != Block.POWERED_REPEATER.id)
        {
            return false;
        }
        else
        {
            int var6 = var0.getBlockMeta(var1, var2, var3);
            return var4 == Facings.OPPOSITE[var6 & 3];
        }
    }
}