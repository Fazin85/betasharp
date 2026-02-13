using BetaSharp.Blocks;
using BetaSharp.Entities;
using BetaSharp.Worlds;

namespace BetaSharp.Items;

public class ItemSeeds : Item
{

    private int blockId;

    public ItemSeeds(int id, int blockId) : base(id)
    {
        this.blockId = blockId;
    }

    public override bool useOnBlock(ItemStack itemStack, EntityPlayer entityPlayer, World world, int x, int y, int z, int meta)
    {
        if (meta != 1)
        {
            return false;
        }
        else
        {
            int blockId = world.getBlockId(x, y, z);
            if (blockId == Block.FARMLAND.id && world.isAir(x, y + 1, z))
            {
                world.setBlock(x, y + 1, z, this.blockId);
                --itemStack.count;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}