using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Worlds;

namespace betareborn.Items
{
    public class ItemDye : Item
    {

        public static readonly String[] dyeColors = new String[] { "black", "red", "green", "brown", "blue", "purple", "cyan", "silver", "gray", "pink", "lime", "yellow", "lightBlue", "magenta", "orange", "white" };
        public static readonly int[] field_31002_bk = new int[] { 1973019, 11743532, 3887386, 5320730, 2437522, 8073150, 2651799, 2651799, 4408131, 14188952, 4312372, 14602026, 6719955, 12801229, 15435844, 15790320 };

        public ItemDye(int var1) : base(var1)
        {
            setHasSubtypes(true);
            setMaxDamage(0);
        }

        public override int getIconFromDamage(int var1)
        {
            return iconIndex + var1 % 8 * 16 + var1 / 8;
        }

        public override String getItemNameIS(ItemStack var1)
        {
            return base.getItemName() + "." + dyeColors[var1.getItemDamage()];
        }

        public override bool onItemUse(ItemStack var1, EntityPlayer var2, World var3, int var4, int var5, int var6, int var7)
        {
            if (var1.getItemDamage() == 15)
            {
                int var8 = var3.getBlockId(var4, var5, var6);
                if (var8 == Block.SAPLING.id)
                {
                    if (!var3.multiplayerWorld)
                    {
                        ((BlockSapling)Block.SAPLING).growTree(var3, var4, var5, var6, var3.random);
                        --var1.count;
                    }
                    return true;
                }
                if (var8 == Block.WHEAT.id)
                {
                    if (!var3.multiplayerWorld)
                    {
                        ((BlockCrops)Block.WHEAT).fertilize(var3, var4, var5, var6);
                        --var1.count;
                    }
                    return true;
                }
                if (var8 == Block.GRASS_BLOCK.id)
                {
                    if (!var3.multiplayerWorld)
                    {
                        --var1.count;

                        for (int var9 = 0; var9 < 128; ++var9)
                        {
                            int var10 = var4;
                            int var11 = var5 + 1;
                            int var12 = var6;

                            bool validPosition = true;
                            for (int var13 = 0; var13 < var9 / 16 && validPosition; ++var13)
                            {
                                var10 += itemRand.nextInt(3) - 1;
                                var11 += (itemRand.nextInt(3) - 1) * itemRand.nextInt(3) / 2;
                                var12 += itemRand.nextInt(3) - 1;
                                if (var3.getBlockId(var10, var11 - 1, var12) != Block.GRASS_BLOCK.id || var3.shouldSuffocate(var10, var11, var12))
                                {
                                    validPosition = false;
                                }
                            }

                            if (validPosition && var3.getBlockId(var10, var11, var12) == 0)
                            {
                                if (itemRand.nextInt(10) != 0)
                                {
                                    var3.setBlockAndMetadataWithNotify(var10, var11, var12, Block.GRASS.id, 1);
                                }
                                else if (itemRand.nextInt(3) != 0)
                                {
                                    var3.setBlockWithNotify(var10, var11, var12, Block.DANDELION.id);
                                }
                                else
                                {
                                    var3.setBlockWithNotify(var10, var11, var12, Block.ROSE.id);
                                }
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public override void saddleEntity(ItemStack var1, EntityLiving var2)
        {
            if (var2 is EntitySheep)
            {
                EntitySheep var3 = (EntitySheep)var2;
                int var4 = BlockCloth.func_21034_c(var1.getItemDamage());
                if (!var3.getSheared() && var3.getFleeceColor() != var4)
                {
                    var3.setFleeceColor(var4);
                    --var1.count;
                }
            }

        }
    }

}