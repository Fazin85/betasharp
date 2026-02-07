using betareborn.Blocks;
using betareborn.Entities;
using betareborn.TileEntities;
using betareborn.Worlds;

namespace betareborn.Items
{
    public class ItemSign : Item
    {

        public ItemSign(int var1) : base(var1)
        {
            maxStackSize = 1;
        }

        public override bool onItemUse(ItemStack var1, EntityPlayer var2, World var3, int var4, int var5, int var6, int var7)
        {
            if (var7 == 0)
            {
                return false;
            }
            else if (!var3.getMaterial(var4, var5, var6).isSolid())
            {
                return false;
            }
            else
            {
                if (var7 == 1)
                {
                    ++var5;
                }

                if (var7 == 2)
                {
                    --var6;
                }

                if (var7 == 3)
                {
                    ++var6;
                }

                if (var7 == 4)
                {
                    --var4;
                }

                if (var7 == 5)
                {
                    ++var4;
                }

                if (!Block.SIGN.canPlaceBlockAt(var3, var4, var5, var6))
                {
                    return false;
                }
                else
                {
                    if (var7 == 1)
                    {
                        var3.setBlockAndMetadataWithNotify(var4, var5, var6, Block.SIGN.id, MathHelper.floor_double((double)((var2.rotationYaw + 180.0F) * 16.0F / 360.0F) + 0.5D) & 15);
                    }
                    else
                    {
                        var3.setBlockAndMetadataWithNotify(var4, var5, var6, Block.WALL_SIGN.id, var7);
                    }

                    --var1.count;
                    TileEntitySign var8 = (TileEntitySign)var3.getBlockTileEntity(var4, var5, var6);
                    if (var8 != null)
                    {
                        var2.displayGUIEditSign(var8);
                    }

                    return true;
                }
            }
        }
    }

}