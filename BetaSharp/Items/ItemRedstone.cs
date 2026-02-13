using BetaSharp.Blocks;
using BetaSharp.Entities;
using BetaSharp.Worlds;

namespace BetaSharp.Items;

public class ItemRedstone : Item
{

    public ItemRedstone(int id) : base(id)
    {
    }

    public override bool useOnBlock(ItemStack itemStack, EntityPlayer entityPlayer, World world, int x, int y, int z, int meta)
    {
        if (world.getBlockId(x, y, z) != Block.SNOW.id)
        {
            if (meta == 0)
            {
                --y;
            }

            if (meta == 1)
            {
                ++y;
            }

            if (meta == 2)
            {
                --z;
            }

            if (meta == 3)
            {
                ++z;
            }

            if (meta == 4)
            {
                --x;
            }

            if (meta == 5)
            {
                ++x;
            }

            if (!world.isAir(x, y, z))
            {
                return false;
            }
        }

        if (Block.REDSTONE_WIRE.canPlaceAt(world, x, y, z))
        {
            --itemStack.count;
            world.setBlock(x, y, z, Block.REDSTONE_WIRE.id);
        }

        return true;
    }
}