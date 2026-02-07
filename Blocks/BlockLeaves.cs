using betareborn.Entities;
using betareborn.Items;
using betareborn.Materials;
using betareborn.Stats;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockLeaves : BlockLeavesBase
    {
        private int baseIndexInPNG;
        int[] adjacentTreeBlocks;

        public BlockLeaves(int var1, int var2) : base(var1, var2, Material.LEAVES, false)
        {
            baseIndexInPNG = var2;
            setTickRandomly(true);
        }

        public override int getRenderColor(int var1)
        {
            return (var1 & 1) == 1 ? ColorizerFoliage.getFoliageColorPine() : ((var1 & 2) == 2 ? ColorizerFoliage.getFoliageColorBirch() : ColorizerFoliage.func_31073_c());
        }

        public override int colorMultiplier(BlockView var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockMeta(var2, var3, var4);
            if ((var5 & 1) == 1)
            {
                return ColorizerFoliage.getFoliageColorPine();
            }
            else if ((var5 & 2) == 2)
            {
                return ColorizerFoliage.getFoliageColorBirch();
            }
            else
            {
                var1.getBiomeSource().getBiomesInArea(var2, var4, 1, 1);
                double var6 = var1.getBiomeSource().temperatureMap[0];
                double var8 = var1.getBiomeSource().downfallMap[0];
                return ColorizerFoliage.getFoliageColor(var6, var8);
            }
        }

        public override void onBlockRemoval(World var1, int var2, int var3, int var4)
        {
            sbyte var5 = 1;
            int var6 = var5 + 1;
            if (var1.checkChunksExist(var2 - var6, var3 - var6, var4 - var6, var2 + var6, var3 + var6, var4 + var6))
            {
                for (int var7 = -var5; var7 <= var5; ++var7)
                {
                    for (int var8 = -var5; var8 <= var5; ++var8)
                    {
                        for (int var9 = -var5; var9 <= var5; ++var9)
                        {
                            int var10 = var1.getBlockId(var2 + var7, var3 + var8, var4 + var9);
                            if (var10 == Block.LEAVES.id)
                            {
                                int var11 = var1.getBlockMeta(var2 + var7, var3 + var8, var4 + var9);
                                var1.setBlockMetadata(var2 + var7, var3 + var8, var4 + var9, var11 | 8);
                            }
                        }
                    }
                }
            }

        }

        public override void onTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            if (!var1.multiplayerWorld)
            {
                int var6 = var1.getBlockMeta(var2, var3, var4);
                if ((var6 & 8) != 0)
                {
                    sbyte var7 = 4;
                    int var8 = var7 + 1;
                    sbyte var9 = 32;
                    int var10 = var9 * var9;
                    int var11 = var9 / 2;
                    if (adjacentTreeBlocks == null)
                    {
                        adjacentTreeBlocks = new int[var9 * var9 * var9];
                    }

                    int var12;
                    if (var1.checkChunksExist(var2 - var8, var3 - var8, var4 - var8, var2 + var8, var3 + var8, var4 + var8))
                    {
                        var12 = -var7;

                        while (var12 <= var7)
                        {
                            int var13;
                            int var14;
                            int var15;

                            for (var13 = -var7; var13 <= var7; ++var13)
                            {
                                for (var14 = -var7; var14 <= var7; ++var14)
                                {
                                    var15 = var1.getBlockId(var2 + var12, var3 + var13, var4 + var14);
                                    if (var15 == Block.LOG.id)
                                    {
                                        adjacentTreeBlocks[(var12 + var11) * var10 + (var13 + var11) * var9 + var14 + var11] = 0;
                                    }
                                    else if (var15 == Block.LEAVES.id)
                                    {
                                        adjacentTreeBlocks[(var12 + var11) * var10 + (var13 + var11) * var9 + var14 + var11] = -2;
                                    }
                                    else
                                    {
                                        adjacentTreeBlocks[(var12 + var11) * var10 + (var13 + var11) * var9 + var14 + var11] = -1;
                                    }
                                }
                            }

                            ++var12;
                        }

                        for (var12 = 1; var12 <= 4; ++var12)
                        {
                            int var13;
                            int var14;
                            int var15;

                            for (var13 = -var7; var13 <= var7; ++var13)
                            {
                                for (var14 = -var7; var14 <= var7; ++var14)
                                {
                                    for (var15 = -var7; var15 <= var7; ++var15)
                                    {
                                        if (adjacentTreeBlocks[(var13 + var11) * var10 + (var14 + var11) * var9 + var15 + var11] == var12 - 1)
                                        {
                                            if (adjacentTreeBlocks[(var13 + var11 - 1) * var10 + (var14 + var11) * var9 + var15 + var11] == -2)
                                            {
                                                adjacentTreeBlocks[(var13 + var11 - 1) * var10 + (var14 + var11) * var9 + var15 + var11] = var12;
                                            }

                                            if (adjacentTreeBlocks[(var13 + var11 + 1) * var10 + (var14 + var11) * var9 + var15 + var11] == -2)
                                            {
                                                adjacentTreeBlocks[(var13 + var11 + 1) * var10 + (var14 + var11) * var9 + var15 + var11] = var12;
                                            }

                                            if (adjacentTreeBlocks[(var13 + var11) * var10 + (var14 + var11 - 1) * var9 + var15 + var11] == -2)
                                            {
                                                adjacentTreeBlocks[(var13 + var11) * var10 + (var14 + var11 - 1) * var9 + var15 + var11] = var12;
                                            }

                                            if (adjacentTreeBlocks[(var13 + var11) * var10 + (var14 + var11 + 1) * var9 + var15 + var11] == -2)
                                            {
                                                adjacentTreeBlocks[(var13 + var11) * var10 + (var14 + var11 + 1) * var9 + var15 + var11] = var12;
                                            }

                                            if (adjacentTreeBlocks[(var13 + var11) * var10 + (var14 + var11) * var9 + (var15 + var11 - 1)] == -2)
                                            {
                                                adjacentTreeBlocks[(var13 + var11) * var10 + (var14 + var11) * var9 + (var15 + var11 - 1)] = var12;
                                            }

                                            if (adjacentTreeBlocks[(var13 + var11) * var10 + (var14 + var11) * var9 + var15 + var11 + 1] == -2)
                                            {
                                                adjacentTreeBlocks[(var13 + var11) * var10 + (var14 + var11) * var9 + var15 + var11 + 1] = var12;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var12 = adjacentTreeBlocks[var11 * var10 + var11 * var9 + var11];
                    if (var12 >= 0)
                    {
                        var1.setBlockMetadata(var2, var3, var4, var6 & -9);
                    }
                    else
                    {
                        removeLeaves(var1, var2, var3, var4);
                    }
                }

            }
        }

        private void removeLeaves(World var1, int var2, int var3, int var4)
        {
            dropBlockAsItem(var1, var2, var3, var4, var1.getBlockMeta(var2, var3, var4));
            var1.setBlockWithNotify(var2, var3, var4, 0);
        }

        public override int quantityDropped(java.util.Random var1)
        {
            return var1.nextInt(20) == 0 ? 1 : 0;
        }

        public override int getDroppedItemId(int var1, java.util.Random var2)
        {
            return Block.SAPLING.id;
        }

        public override void harvestBlock(World var1, EntityPlayer var2, int var3, int var4, int var5, int var6)
        {
            if (!var1.multiplayerWorld && var2.getCurrentEquippedItem() != null && var2.getCurrentEquippedItem().itemID == Item.shears.id)
            {
                var2.addStat(StatList.mineBlockStatArray[id], 1);
                dropBlockAsItem_do(var1, var3, var4, var5, new ItemStack(Block.LEAVES.id, 1, var6 & 3));
            }
            else
            {
                base.harvestBlock(var1, var2, var3, var4, var5, var6);
            }

        }

        protected override int damageDropped(int var1)
        {
            return var1 & 3;
        }

        public override bool isOpaque()
        {
            return !graphicsLevel;
        }

        public override int getTexture(int var1, int var2)
        {
            return (var2 & 3) == 1 ? textureId + 80 : textureId;
        }

        public void setGraphicsLevel(bool var1)
        {
            graphicsLevel = var1;
            textureId = baseIndexInPNG + (var1 ? 0 : 1);
        }

        public override void onEntityWalking(World var1, int var2, int var3, int var4, Entity var5)
        {
            base.onEntityWalking(var1, var2, var3, var4, var5);
        }
    }

}