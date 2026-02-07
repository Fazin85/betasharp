using betareborn.Entities;
using betareborn.Items;
using betareborn.Materials;
using betareborn.TileEntities;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockDispenser : BlockContainer
    {
        private java.util.Random random = new();

        public BlockDispenser(int var1) : base(var1, Material.STONE)
        {
            textureId = 45;
        }

        public override int tickRate()
        {
            return 4;
        }

        public override int getDroppedItemId(int var1, java.util.Random var2)
        {
            return Block.DISPENSER.id;
        }

        public override void onBlockAdded(World var1, int var2, int var3, int var4)
        {
            base.onBlockAdded(var1, var2, var3, var4);
            setDispenserDefaultDirection(var1, var2, var3, var4);
        }

        private void setDispenserDefaultDirection(World var1, int var2, int var3, int var4)
        {
            if (!var1.multiplayerWorld)
            {
                int var5 = var1.getBlockId(var2, var3, var4 - 1);
                int var6 = var1.getBlockId(var2, var3, var4 + 1);
                int var7 = var1.getBlockId(var2 - 1, var3, var4);
                int var8 = var1.getBlockId(var2 + 1, var3, var4);
                sbyte var9 = 3;
                if (Block.BLOCKS_OPAQUE[var5] && !Block.BLOCKS_OPAQUE[var6])
                {
                    var9 = 3;
                }

                if (Block.BLOCKS_OPAQUE[var6] && !Block.BLOCKS_OPAQUE[var5])
                {
                    var9 = 2;
                }

                if (Block.BLOCKS_OPAQUE[var7] && !Block.BLOCKS_OPAQUE[var8])
                {
                    var9 = 5;
                }

                if (Block.BLOCKS_OPAQUE[var8] && !Block.BLOCKS_OPAQUE[var7])
                {
                    var9 = 4;
                }

                var1.setBlockMeta(var2, var3, var4, var9);
            }
        }

        public override int getTexture(BlockView var1, int var2, int var3, int var4, int var5)
        {
            if (var5 == 1)
            {
                return textureId + 17;
            }
            else if (var5 == 0)
            {
                return textureId + 17;
            }
            else
            {
                int var6 = var1.getBlockMeta(var2, var3, var4);
                return var5 != var6 ? textureId : textureId + 1;
            }
        }

        public override int getTexture(int var1)
        {
            return var1 == 1 ? textureId + 17 : (var1 == 0 ? textureId + 17 : (var1 == 3 ? textureId + 1 : textureId));
        }

        public override bool onUse(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            if (var1.multiplayerWorld)
            {
                return true;
            }
            else
            {
                TileEntityDispenser var6 = (TileEntityDispenser)var1.getBlockTileEntity(var2, var3, var4);
                var5.displayGUIDispenser(var6);
                return true;
            }
        }

        private void dispenseItem(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            int var6 = var1.getBlockMeta(var2, var3, var4);
            int var9 = 0;
            int var10 = 0;
            if (var6 == 3)
            {
                var10 = 1;
            }
            else if (var6 == 2)
            {
                var10 = -1;
            }
            else if (var6 == 5)
            {
                var9 = 1;
            }
            else
            {
                var9 = -1;
            }

            TileEntityDispenser var11 = (TileEntityDispenser)var1.getBlockTileEntity(var2, var3, var4);
            ItemStack var12 = var11.getItemToDispose();
            double var13 = (double)var2 + (double)var9 * 0.6D + 0.5D;
            double var15 = (double)var3 + 0.5D;
            double var17 = (double)var4 + (double)var10 * 0.6D + 0.5D;
            if (var12 == null)
            {
                var1.func_28106_e(1001, var2, var3, var4, 0);
            }
            else
            {
                if (var12.itemID == Item.arrow.id)
                {
                    EntityArrow var19 = new EntityArrow(var1, var13, var15, var17);
                    var19.setArrowHeading((double)var9, (double)0.1F, (double)var10, 1.1F, 6.0F);
                    var19.doesArrowBelongToPlayer = true;
                    var1.spawnEntity(var19);
                    var1.func_28106_e(1002, var2, var3, var4, 0);
                }
                else if (var12.itemID == Item.egg.id)
                {
                    EntityEgg var22 = new EntityEgg(var1, var13, var15, var17);
                    var22.setEggHeading((double)var9, (double)0.1F, (double)var10, 1.1F, 6.0F);
                    var1.spawnEntity(var22);
                    var1.func_28106_e(1002, var2, var3, var4, 0);
                }
                else if (var12.itemID == Item.snowball.id)
                {
                    EntitySnowball var23 = new EntitySnowball(var1, var13, var15, var17);
                    var23.setSnowballHeading((double)var9, (double)0.1F, (double)var10, 1.1F, 6.0F);
                    var1.spawnEntity(var23);
                    var1.func_28106_e(1002, var2, var3, var4, 0);
                }
                else
                {
                    EntityItem var24 = new EntityItem(var1, var13, var15 - 0.3D, var17, var12);
                    double var20 = var5.nextDouble() * 0.1D + 0.2D;
                    var24.motionX = (double)var9 * var20;
                    var24.motionY = (double)0.2F;
                    var24.motionZ = (double)var10 * var20;
                    var24.motionX += var5.nextGaussian() * (double)0.0075F * 6.0D;
                    var24.motionY += var5.nextGaussian() * (double)0.0075F * 6.0D;
                    var24.motionZ += var5.nextGaussian() * (double)0.0075F * 6.0D;
                    var1.spawnEntity(var24);
                    var1.func_28106_e(1000, var2, var3, var4, 0);
                }

                var1.func_28106_e(2000, var2, var3, var4, var9 + 1 + (var10 + 1) * 3);
            }

        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            if (var5 > 0 && Block.BLOCKS[var5].canProvidePower())
            {
                bool var6 = var1.isBlockIndirectlyGettingPowered(var2, var3, var4) || var1.isBlockIndirectlyGettingPowered(var2, var3 + 1, var4);
                if (var6)
                {
                    var1.scheduleBlockUpdate(var2, var3, var4, id, tickRate());
                }
            }

        }

        public override void onTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            if (var1.isBlockIndirectlyGettingPowered(var2, var3, var4) || var1.isBlockIndirectlyGettingPowered(var2, var3 + 1, var4))
            {
                dispenseItem(var1, var2, var3, var4, var5);
            }

        }

        protected override TileEntity getBlockEntity()
        {
            return new TileEntityDispenser();
        }

        public override void onBlockPlacedBy(World var1, int var2, int var3, int var4, EntityLiving var5)
        {
            int var6 = MathHelper.floor_double((double)(var5.rotationYaw * 4.0F / 360.0F) + 0.5D) & 3;
            if (var6 == 0)
            {
                var1.setBlockMeta(var2, var3, var4, 2);
            }

            if (var6 == 1)
            {
                var1.setBlockMeta(var2, var3, var4, 5);
            }

            if (var6 == 2)
            {
                var1.setBlockMeta(var2, var3, var4, 3);
            }

            if (var6 == 3)
            {
                var1.setBlockMeta(var2, var3, var4, 4);
            }

        }

        public override void onBlockRemoval(World var1, int var2, int var3, int var4)
        {
            TileEntityDispenser var5 = (TileEntityDispenser)var1.getBlockTileEntity(var2, var3, var4);

            for (int var6 = 0; var6 < var5.size(); ++var6)
            {
                ItemStack var7 = var5.getStack(var6);
                if (var7 != null)
                {
                    float var8 = random.nextFloat() * 0.8F + 0.1F;
                    float var9 = random.nextFloat() * 0.8F + 0.1F;
                    float var10 = random.nextFloat() * 0.8F + 0.1F;

                    while (var7.count > 0)
                    {
                        int var11 = random.nextInt(21) + 10;
                        if (var11 > var7.count)
                        {
                            var11 = var7.count;
                        }

                        var7.count -= var11;
                        EntityItem var12 = new EntityItem(var1, (double)((float)var2 + var8), (double)((float)var3 + var9), (double)((float)var4 + var10), new ItemStack(var7.itemID, var11, var7.getItemDamage()));
                        float var13 = 0.05F;
                        var12.motionX = (double)((float)random.nextGaussian() * var13);
                        var12.motionY = (double)((float)random.nextGaussian() * var13 + 0.2F);
                        var12.motionZ = (double)((float)random.nextGaussian() * var13);
                        var1.spawnEntity(var12);
                    }
                }
            }

            base.onBlockRemoval(var1, var2, var3, var4);
        }
    }

}