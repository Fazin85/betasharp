using betareborn.Entities;
using betareborn.Items;
using betareborn.Materials;
using betareborn.TileEntities;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockFurnace : BlockContainer
    {

        private java.util.Random furnaceRand = new();
        private readonly bool isActive;
        private static bool keepFurnaceInventory = false;

        public BlockFurnace(int var1, bool var2) : base(var1, Material.STONE)
        {
            isActive = var2;
            textureId = 45;
        }

        public override int getDroppedItemId(int var1, java.util.Random var2)
        {
            return Block.FURNACE.id;
        }

        public override void onBlockAdded(World var1, int var2, int var3, int var4)
        {
            base.onBlockAdded(var1, var2, var3, var4);
            setDefaultDirection(var1, var2, var3, var4);
        }

        private void setDefaultDirection(World var1, int var2, int var3, int var4)
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
                return var5 != var6 ? textureId : (isActive ? textureId + 16 : textureId - 1);
            }
        }

        public override void randomDisplayTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            if (isActive)
            {
                int var6 = var1.getBlockMeta(var2, var3, var4);
                float var7 = (float)var2 + 0.5F;
                float var8 = (float)var3 + 0.0F + var5.nextFloat() * 6.0F / 16.0F;
                float var9 = (float)var4 + 0.5F;
                float var10 = 0.52F;
                float var11 = var5.nextFloat() * 0.6F - 0.3F;
                if (var6 == 4)
                {
                    var1.addParticle("smoke", (double)(var7 - var10), (double)var8, (double)(var9 + var11), 0.0D, 0.0D, 0.0D);
                    var1.addParticle("flame", (double)(var7 - var10), (double)var8, (double)(var9 + var11), 0.0D, 0.0D, 0.0D);
                }
                else if (var6 == 5)
                {
                    var1.addParticle("smoke", (double)(var7 + var10), (double)var8, (double)(var9 + var11), 0.0D, 0.0D, 0.0D);
                    var1.addParticle("flame", (double)(var7 + var10), (double)var8, (double)(var9 + var11), 0.0D, 0.0D, 0.0D);
                }
                else if (var6 == 2)
                {
                    var1.addParticle("smoke", (double)(var7 + var11), (double)var8, (double)(var9 - var10), 0.0D, 0.0D, 0.0D);
                    var1.addParticle("flame", (double)(var7 + var11), (double)var8, (double)(var9 - var10), 0.0D, 0.0D, 0.0D);
                }
                else if (var6 == 3)
                {
                    var1.addParticle("smoke", (double)(var7 + var11), (double)var8, (double)(var9 + var10), 0.0D, 0.0D, 0.0D);
                    var1.addParticle("flame", (double)(var7 + var11), (double)var8, (double)(var9 + var10), 0.0D, 0.0D, 0.0D);
                }

            }
        }

        public override int getTexture(int var1)
        {
            return var1 == 1 ? textureId + 17 : (var1 == 0 ? textureId + 17 : (var1 == 3 ? textureId - 1 : textureId));
        }

        public override bool onUse(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            if (var1.multiplayerWorld)
            {
                return true;
            }
            else
            {
                TileEntityFurnace var6 = (TileEntityFurnace)var1.getBlockTileEntity(var2, var3, var4);
                var5.displayGUIFurnace(var6);
                return true;
            }
        }

        public static void updateFurnaceBlockState(bool var0, World var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockMeta(var2, var3, var4);
            TileEntity var6 = var1.getBlockTileEntity(var2, var3, var4);
            keepFurnaceInventory = true;
            if (var0)
            {
                var1.setBlockWithNotify(var2, var3, var4, Block.LIT_FURNACE.id);
            }
            else
            {
                var1.setBlockWithNotify(var2, var3, var4, Block.FURNACE.id);
            }

            keepFurnaceInventory = false;
            var1.setBlockMeta(var2, var3, var4, var5);
            var6.cancelRemoval();
            var1.setBlockTileEntity(var2, var3, var4, var6);
        }

        protected override TileEntity getBlockEntity()
        {
            return new TileEntityFurnace();
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
            if (!keepFurnaceInventory)
            {
                TileEntityFurnace var5 = (TileEntityFurnace)var1.getBlockTileEntity(var2, var3, var4);

                for (int var6 = 0; var6 < var5.size(); ++var6)
                {
                    ItemStack var7 = var5.getStack(var6);
                    if (var7 != null)
                    {
                        float var8 = furnaceRand.nextFloat() * 0.8F + 0.1F;
                        float var9 = furnaceRand.nextFloat() * 0.8F + 0.1F;
                        float var10 = furnaceRand.nextFloat() * 0.8F + 0.1F;

                        while (var7.count > 0)
                        {
                            int var11 = furnaceRand.nextInt(21) + 10;
                            if (var11 > var7.count)
                            {
                                var11 = var7.count;
                            }

                            var7.count -= var11;
                            EntityItem var12 = new EntityItem(var1, (double)((float)var2 + var8), (double)((float)var3 + var9), (double)((float)var4 + var10), new ItemStack(var7.itemID, var11, var7.getItemDamage()));
                            float var13 = 0.05F;
                            var12.motionX = (double)((float)furnaceRand.nextGaussian() * var13);
                            var12.motionY = (double)((float)furnaceRand.nextGaussian() * var13 + 0.2F);
                            var12.motionZ = (double)((float)furnaceRand.nextGaussian() * var13);
                            var1.spawnEntity(var12);
                        }
                    }
                }
            }

            base.onBlockRemoval(var1, var2, var3, var4);
        }
    }
}