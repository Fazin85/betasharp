using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockFire : Block
    {
        private int[] chanceToEncourageFire = new int[256];
        private int[] abilityToCatchFire = new int[256];

        public BlockFire(int var1, int var2) : base(var1, var2, Material.FIRE)
        {
            setTickRandomly(true);
        }

        protected override void init()
        {
            setBurnRate(Block.PLANKS.id, 5, 20);
            setBurnRate(Block.FENCE.id, 5, 20);
            setBurnRate(Block.WOODEN_STAIRS.id, 5, 20);
            setBurnRate(Block.LOG.id, 5, 5);
            setBurnRate(Block.LEAVES.id, 30, 60);
            setBurnRate(Block.BOOKSHELF.id, 30, 20);
            setBurnRate(Block.TNT.id, 15, 100);
            setBurnRate(Block.GRASS.id, 60, 100);
            setBurnRate(Block.WOOL.id, 30, 60);
        }

        private void setBurnRate(int var1, int var2, int var3)
        {
            chanceToEncourageFire[var1] = var2;
            abilityToCatchFire[var1] = var3;
        }

        public override Box getCollisionShape(World var1, int var2, int var3, int var4)
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
            return 3;
        }

        public override int quantityDropped(java.util.Random var1)
        {
            return 0;
        }

        public override int tickRate()
        {
            return 40;
        }

        public override void onTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            bool var6 = var1.getBlockId(var2, var3 - 1, var4) == Block.NETHERRACK.id;
            if (!canPlaceBlockAt(var1, var2, var3, var4))
            {
                var1.setBlockWithNotify(var2, var3, var4, 0);
            }

            if (var6 || !var1.func_27161_C() || !var1.canBlockBeRainedOn(var2, var3, var4) && !var1.canBlockBeRainedOn(var2 - 1, var3, var4) && !var1.canBlockBeRainedOn(var2 + 1, var3, var4) && !var1.canBlockBeRainedOn(var2, var3, var4 - 1) && !var1.canBlockBeRainedOn(var2, var3, var4 + 1))
            {
                int var7 = var1.getBlockMeta(var2, var3, var4);
                if (var7 < 15)
                {
                    var1.setBlockMetadata(var2, var3, var4, var7 + var5.nextInt(3) / 2);
                }

                var1.scheduleBlockUpdate(var2, var3, var4, id, tickRate());
                if (!var6 && !func_263_h(var1, var2, var3, var4))
                {
                    if (!var1.shouldSuffocate(var2, var3 - 1, var4) || var7 > 3)
                    {
                        var1.setBlockWithNotify(var2, var3, var4, 0);
                    }

                }
                else if (!var6 && !canBlockCatchFire(var1, var2, var3 - 1, var4) && var7 == 15 && var5.nextInt(4) == 0)
                {
                    var1.setBlockWithNotify(var2, var3, var4, 0);
                }
                else
                {
                    tryToCatchBlockOnFire(var1, var2 + 1, var3, var4, 300, var5, var7);
                    tryToCatchBlockOnFire(var1, var2 - 1, var3, var4, 300, var5, var7);
                    tryToCatchBlockOnFire(var1, var2, var3 - 1, var4, 250, var5, var7);
                    tryToCatchBlockOnFire(var1, var2, var3 + 1, var4, 250, var5, var7);
                    tryToCatchBlockOnFire(var1, var2, var3, var4 - 1, 300, var5, var7);
                    tryToCatchBlockOnFire(var1, var2, var3, var4 + 1, 300, var5, var7);

                    for (int var8 = var2 - 1; var8 <= var2 + 1; ++var8)
                    {
                        for (int var9 = var4 - 1; var9 <= var4 + 1; ++var9)
                        {
                            for (int var10 = var3 - 1; var10 <= var3 + 4; ++var10)
                            {
                                if (var8 != var2 || var10 != var3 || var9 != var4)
                                {
                                    int var11 = 100;
                                    if (var10 > var3 + 1)
                                    {
                                        var11 += (var10 - (var3 + 1)) * 100;
                                    }

                                    int var12 = getChanceOfNeighborsEncouragingFire(var1, var8, var10, var9);
                                    if (var12 > 0)
                                    {
                                        int var13 = (var12 + 40) / (var7 + 30);
                                        if (var13 > 0 && var5.nextInt(var11) <= var13 && (!var1.func_27161_C() || !var1.canBlockBeRainedOn(var8, var10, var9)) && !var1.canBlockBeRainedOn(var8 - 1, var10, var4) && !var1.canBlockBeRainedOn(var8 + 1, var10, var9) && !var1.canBlockBeRainedOn(var8, var10, var9 - 1) && !var1.canBlockBeRainedOn(var8, var10, var9 + 1))
                                        {
                                            int var14 = var7 + var5.nextInt(5) / 4;
                                            if (var14 > 15)
                                            {
                                                var14 = 15;
                                            }

                                            var1.setBlockAndMetadataWithNotify(var8, var10, var9, id, var14);
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
            else
            {
                var1.setBlockWithNotify(var2, var3, var4, 0);
            }
        }

        private void tryToCatchBlockOnFire(World var1, int var2, int var3, int var4, int var5, java.util.Random var6, int var7)
        {
            int var8 = abilityToCatchFire[var1.getBlockId(var2, var3, var4)];
            if (var6.nextInt(var5) < var8)
            {
                bool var9 = var1.getBlockId(var2, var3, var4) == Block.TNT.id;
                if (var6.nextInt(var7 + 10) < 5 && !var1.canBlockBeRainedOn(var2, var3, var4))
                {
                    int var10 = var7 + var6.nextInt(5) / 4;
                    if (var10 > 15)
                    {
                        var10 = 15;
                    }

                    var1.setBlockAndMetadataWithNotify(var2, var3, var4, id, var10);
                }
                else
                {
                    var1.setBlockWithNotify(var2, var3, var4, 0);
                }

                if (var9)
                {
                    Block.TNT.onBlockDestroyedByPlayer(var1, var2, var3, var4, 1);
                }
            }

        }

        private bool func_263_h(World var1, int var2, int var3, int var4)
        {
            return canBlockCatchFire(var1, var2 + 1, var3, var4) ? true : (canBlockCatchFire(var1, var2 - 1, var3, var4) ? true : (canBlockCatchFire(var1, var2, var3 - 1, var4) ? true : (canBlockCatchFire(var1, var2, var3 + 1, var4) ? true : (canBlockCatchFire(var1, var2, var3, var4 - 1) ? true : canBlockCatchFire(var1, var2, var3, var4 + 1)))));
        }

        private int getChanceOfNeighborsEncouragingFire(World var1, int var2, int var3, int var4)
        {
            sbyte var5 = 0;
            if (!var1.isAir(var2, var3, var4))
            {
                return 0;
            }
            else
            {
                int var6 = getChanceToEncourageFire(var1, var2 + 1, var3, var4, var5);
                var6 = getChanceToEncourageFire(var1, var2 - 1, var3, var4, var6);
                var6 = getChanceToEncourageFire(var1, var2, var3 - 1, var4, var6);
                var6 = getChanceToEncourageFire(var1, var2, var3 + 1, var4, var6);
                var6 = getChanceToEncourageFire(var1, var2, var3, var4 - 1, var6);
                var6 = getChanceToEncourageFire(var1, var2, var3, var4 + 1, var6);
                return var6;
            }
        }

        public override bool hasCollision()
        {
            return false;
        }

        public bool canBlockCatchFire(BlockView var1, int var2, int var3, int var4)
        {
            return chanceToEncourageFire[var1.getBlockId(var2, var3, var4)] > 0;
        }

        public int getChanceToEncourageFire(World var1, int var2, int var3, int var4, int var5)
        {
            int var6 = chanceToEncourageFire[var1.getBlockId(var2, var3, var4)];
            return var6 > var5 ? var6 : var5;
        }

        public override bool canPlaceBlockAt(World var1, int var2, int var3, int var4)
        {
            return var1.shouldSuffocate(var2, var3 - 1, var4) || func_263_h(var1, var2, var3, var4);
        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            if (!var1.shouldSuffocate(var2, var3 - 1, var4) && !func_263_h(var1, var2, var3, var4))
            {
                var1.setBlockWithNotify(var2, var3, var4, 0);
            }
        }

        public override void onBlockAdded(World var1, int var2, int var3, int var4)
        {
            if (var1.getBlockId(var2, var3 - 1, var4) != Block.OBSIDIAN.id || !Block.NETHER_PORTAL.tryToCreatePortal(var1, var2, var3, var4))
            {
                if (!var1.shouldSuffocate(var2, var3 - 1, var4) && !func_263_h(var1, var2, var3, var4))
                {
                    var1.setBlockWithNotify(var2, var3, var4, 0);
                }
                else
                {
                    var1.scheduleBlockUpdate(var2, var3, var4, id, tickRate());
                }
            }
        }

        public override void randomDisplayTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            if (var5.nextInt(24) == 0)
            {
                var1.playSoundEffect((double)((float)var2 + 0.5F), (double)((float)var3 + 0.5F), (double)((float)var4 + 0.5F), "fire.fire", 1.0F + var5.nextFloat(), var5.nextFloat() * 0.7F + 0.3F);
            }

            int var6;
            float var7;
            float var8;
            float var9;
            if (!var1.shouldSuffocate(var2, var3 - 1, var4) && !Block.FIRE.canBlockCatchFire(var1, var2, var3 - 1, var4))
            {
                if (Block.FIRE.canBlockCatchFire(var1, var2 - 1, var3, var4))
                {
                    for (var6 = 0; var6 < 2; ++var6)
                    {
                        var7 = (float)var2 + var5.nextFloat() * 0.1F;
                        var8 = (float)var3 + var5.nextFloat();
                        var9 = (float)var4 + var5.nextFloat();
                        var1.addParticle("largesmoke", (double)var7, (double)var8, (double)var9, 0.0D, 0.0D, 0.0D);
                    }
                }

                if (Block.FIRE.canBlockCatchFire(var1, var2 + 1, var3, var4))
                {
                    for (var6 = 0; var6 < 2; ++var6)
                    {
                        var7 = (float)(var2 + 1) - var5.nextFloat() * 0.1F;
                        var8 = (float)var3 + var5.nextFloat();
                        var9 = (float)var4 + var5.nextFloat();
                        var1.addParticle("largesmoke", (double)var7, (double)var8, (double)var9, 0.0D, 0.0D, 0.0D);
                    }
                }

                if (Block.FIRE.canBlockCatchFire(var1, var2, var3, var4 - 1))
                {
                    for (var6 = 0; var6 < 2; ++var6)
                    {
                        var7 = (float)var2 + var5.nextFloat();
                        var8 = (float)var3 + var5.nextFloat();
                        var9 = (float)var4 + var5.nextFloat() * 0.1F;
                        var1.addParticle("largesmoke", (double)var7, (double)var8, (double)var9, 0.0D, 0.0D, 0.0D);
                    }
                }

                if (Block.FIRE.canBlockCatchFire(var1, var2, var3, var4 + 1))
                {
                    for (var6 = 0; var6 < 2; ++var6)
                    {
                        var7 = (float)var2 + var5.nextFloat();
                        var8 = (float)var3 + var5.nextFloat();
                        var9 = (float)(var4 + 1) - var5.nextFloat() * 0.1F;
                        var1.addParticle("largesmoke", (double)var7, (double)var8, (double)var9, 0.0D, 0.0D, 0.0D);
                    }
                }

                if (Block.FIRE.canBlockCatchFire(var1, var2, var3 + 1, var4))
                {
                    for (var6 = 0; var6 < 2; ++var6)
                    {
                        var7 = (float)var2 + var5.nextFloat();
                        var8 = (float)(var3 + 1) - var5.nextFloat() * 0.1F;
                        var9 = (float)var4 + var5.nextFloat();
                        var1.addParticle("largesmoke", (double)var7, (double)var8, (double)var9, 0.0D, 0.0D, 0.0D);
                    }
                }
            }
            else
            {
                for (var6 = 0; var6 < 3; ++var6)
                {
                    var7 = (float)var2 + var5.nextFloat();
                    var8 = (float)var3 + var5.nextFloat() * 0.5F + 0.5F;
                    var9 = (float)var4 + var5.nextFloat();
                    var1.addParticle("largesmoke", (double)var7, (double)var8, (double)var9, 0.0D, 0.0D, 0.0D);
                }
            }

        }
    }

}