using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockFlowing : BlockFluid
    {
        int numAdjacentSources = 0;
        bool[] isOptimalFlowDirection = new bool[4];
        int[] flowCost = new int[4];

        public BlockFlowing(int var1, Material var2) : base(var1, var2)
        {
        }

        private void func_30003_j(World var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockMeta(var2, var3, var4);
            var1.setBlockAndMetadata(var2, var3, var4, id + 1, var5);
            var1.markBlocksDirty(var2, var3, var4, var2, var3, var4);
            var1.markBlockNeedsUpdate(var2, var3, var4);
        }

        public override void onTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            int var6 = getFlowDecay(var1, var2, var3, var4);
            sbyte var7 = 1;
            if (material == Material.LAVA && !var1.dimension.isHellWorld)
            {
                var7 = 2;
            }

            bool var8 = true;
            int var10;
            if (var6 > 0)
            {
                int var9 = -100;
                numAdjacentSources = 0;
                int var12 = getSmallestFlowDecay(var1, var2 - 1, var3, var4, var9);
                var12 = getSmallestFlowDecay(var1, var2 + 1, var3, var4, var12);
                var12 = getSmallestFlowDecay(var1, var2, var3, var4 - 1, var12);
                var12 = getSmallestFlowDecay(var1, var2, var3, var4 + 1, var12);
                var10 = var12 + var7;
                if (var10 >= 8 || var12 < 0)
                {
                    var10 = -1;
                }

                if (getFlowDecay(var1, var2, var3 + 1, var4) >= 0)
                {
                    int var11 = getFlowDecay(var1, var2, var3 + 1, var4);
                    if (var11 >= 8)
                    {
                        var10 = var11;
                    }
                    else
                    {
                        var10 = var11 + 8;
                    }
                }

                if (numAdjacentSources >= 2 && material == Material.WATER)
                {
                    if (var1.getMaterial(var2, var3 - 1, var4).isSolid())
                    {
                        var10 = 0;
                    }
                    else if (var1.getMaterial(var2, var3 - 1, var4) == material && var1.getBlockMeta(var2, var3, var4) == 0)
                    {
                        var10 = 0;
                    }
                }

                if (material == Material.LAVA && var6 < 8 && var10 < 8 && var10 > var6 && var5.nextInt(4) != 0)
                {
                    var10 = var6;
                    var8 = false;
                }

                if (var10 != var6)
                {
                    var6 = var10;
                    if (var10 < 0)
                    {
                        var1.setBlockWithNotify(var2, var3, var4, 0);
                    }
                    else
                    {
                        var1.setBlockMeta(var2, var3, var4, var10);
                        var1.scheduleBlockUpdate(var2, var3, var4, id, tickRate());
                        var1.notifyBlocksOfNeighborChange(var2, var3, var4, id);
                    }
                }
                else if (var8)
                {
                    func_30003_j(var1, var2, var3, var4);
                }
            }
            else
            {
                func_30003_j(var1, var2, var3, var4);
            }

            if (liquidCanDisplaceBlock(var1, var2, var3 - 1, var4))
            {
                if (var6 >= 8)
                {
                    var1.setBlockAndMetadataWithNotify(var2, var3 - 1, var4, id, var6);
                }
                else
                {
                    var1.setBlockAndMetadataWithNotify(var2, var3 - 1, var4, id, var6 + 8);
                }
            }
            else if (var6 >= 0 && (var6 == 0 || blockBlocksFlow(var1, var2, var3 - 1, var4)))
            {
                bool[] var13 = getOptimalFlowDirections(var1, var2, var3, var4);
                var10 = var6 + var7;
                if (var6 >= 8)
                {
                    var10 = 1;
                }

                if (var10 >= 8)
                {
                    return;
                }

                if (var13[0])
                {
                    flowIntoBlock(var1, var2 - 1, var3, var4, var10);
                }

                if (var13[1])
                {
                    flowIntoBlock(var1, var2 + 1, var3, var4, var10);
                }

                if (var13[2])
                {
                    flowIntoBlock(var1, var2, var3, var4 - 1, var10);
                }

                if (var13[3])
                {
                    flowIntoBlock(var1, var2, var3, var4 + 1, var10);
                }
            }

        }

        private void flowIntoBlock(World var1, int var2, int var3, int var4, int var5)
        {
            if (liquidCanDisplaceBlock(var1, var2, var3, var4))
            {
                int var6 = var1.getBlockId(var2, var3, var4);
                if (var6 > 0)
                {
                    if (material == Material.LAVA)
                    {
                        triggerLavaMixEffects(var1, var2, var3, var4);
                    }
                    else
                    {
                        Block.BLOCKS[var6].dropBlockAsItem(var1, var2, var3, var4, var1.getBlockMeta(var2, var3, var4));
                    }
                }

                var1.setBlockAndMetadataWithNotify(var2, var3, var4, id, var5);
            }

        }

        private int calculateFlowCost(World var1, int var2, int var3, int var4, int var5, int var6)
        {
            int var7 = 1000;

            for (int var8 = 0; var8 < 4; ++var8)
            {
                if ((var8 != 0 || var6 != 1) && (var8 != 1 || var6 != 0) && (var8 != 2 || var6 != 3) && (var8 != 3 || var6 != 2))
                {
                    int var9 = var2;
                    int var11 = var4;
                    if (var8 == 0)
                    {
                        var9 = var2 - 1;
                    }

                    if (var8 == 1)
                    {
                        ++var9;
                    }

                    if (var8 == 2)
                    {
                        var11 = var4 - 1;
                    }

                    if (var8 == 3)
                    {
                        ++var11;
                    }

                    if (!blockBlocksFlow(var1, var9, var3, var11) && (var1.getMaterial(var9, var3, var11) != material || var1.getBlockMeta(var9, var3, var11) != 0))
                    {
                        if (!blockBlocksFlow(var1, var9, var3 - 1, var11))
                        {
                            return var5;
                        }

                        if (var5 < 4)
                        {
                            int var12 = calculateFlowCost(var1, var9, var3, var11, var5 + 1, var8);
                            if (var12 < var7)
                            {
                                var7 = var12;
                            }
                        }
                    }
                }
            }

            return var7;
        }

        private bool[] getOptimalFlowDirections(World var1, int var2, int var3, int var4)
        {
            int var5;
            int var6;
            for (var5 = 0; var5 < 4; ++var5)
            {
                flowCost[var5] = 1000;
                var6 = var2;
                int var8 = var4;
                if (var5 == 0)
                {
                    var6 = var2 - 1;
                }

                if (var5 == 1)
                {
                    ++var6;
                }

                if (var5 == 2)
                {
                    var8 = var4 - 1;
                }

                if (var5 == 3)
                {
                    ++var8;
                }

                if (!blockBlocksFlow(var1, var6, var3, var8) && (var1.getMaterial(var6, var3, var8) != material || var1.getBlockMeta(var6, var3, var8) != 0))
                {
                    if (!blockBlocksFlow(var1, var6, var3 - 1, var8))
                    {
                        flowCost[var5] = 0;
                    }
                    else
                    {
                        flowCost[var5] = calculateFlowCost(var1, var6, var3, var8, 1, var5);
                    }
                }
            }

            var5 = flowCost[0];

            for (var6 = 1; var6 < 4; ++var6)
            {
                if (flowCost[var6] < var5)
                {
                    var5 = flowCost[var6];
                }
            }

            for (var6 = 0; var6 < 4; ++var6)
            {
                isOptimalFlowDirection[var6] = flowCost[var6] == var5;
            }

            return isOptimalFlowDirection;
        }

        private bool blockBlocksFlow(World var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockId(var2, var3, var4);
            if (var5 != Block.DOOR.id && var5 != Block.IRON_DOOR.id && var5 != Block.SIGN.id && var5 != Block.LADDER.id && var5 != Block.SUGAR_CANE.id)
            {
                if (var5 == 0)
                {
                    return false;
                }
                else
                {
                    Material var6 = Block.BLOCKS[var5].material;
                    return var6.blocksMovement();
                }
            }
            else
            {
                return true;
            }
        }

        protected int getSmallestFlowDecay(World var1, int var2, int var3, int var4, int var5)
        {
            int var6 = getFlowDecay(var1, var2, var3, var4);
            if (var6 < 0)
            {
                return var5;
            }
            else
            {
                if (var6 == 0)
                {
                    ++numAdjacentSources;
                }

                if (var6 >= 8)
                {
                    var6 = 0;
                }

                return var5 >= 0 && var6 >= var5 ? var5 : var6;
            }
        }

        private bool liquidCanDisplaceBlock(World var1, int var2, int var3, int var4)
        {
            Material var5 = var1.getMaterial(var2, var3, var4);
            return var5 == material ? false : (var5 == Material.LAVA ? false : !blockBlocksFlow(var1, var2, var3, var4));
        }

        public override void onBlockAdded(World var1, int var2, int var3, int var4)
        {
            base.onBlockAdded(var1, var2, var3, var4);
            if (var1.getBlockId(var2, var3, var4) == id)
            {
                var1.scheduleBlockUpdate(var2, var3, var4, id, tickRate());
            }

        }
    }

}