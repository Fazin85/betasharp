using BetaSharp.Blocks;
using BetaSharp.Entities;
using BetaSharp.Worlds;

namespace BetaSharp.Items;

public class ItemBlock : Item
{

    private int blockID;

    public ItemBlock(int id) : base(id)
    {
        blockID = id + 256;
        setTextureId(Block.BLOCKS[id + 256].getTexture(2));
    }

    public override bool useOnBlock(ItemStack itemStack, EntityPlayer entityPlayer, World world, int x, int y, int z, int meta)
    {
        if (world.getBlockId(x, y, z) == Block.SNOW.id)
        {
            meta = 0;
        }
        else
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
        }

        if (itemStack.count == 0)
        {
            return false;
        }
        else if (y == 127 && Block.BLOCKS[blockID].material.IsSolid)
        {
            return false;
        }
        else if (world.canPlace(blockID, x, y, z, false, meta))
        {
            Block block = Block.BLOCKS[blockID];
            if (world.setBlock(x, y, z, blockID, getPlacementMetadata(itemStack.getDamage())))
            {
                Block.BLOCKS[blockID].onPlaced(world, x, y, z, meta);
                Block.BLOCKS[blockID].onPlaced(world, x, y, z, entityPlayer);
                world.playSound((double)((float)x + 0.5F), (double)((float)y + 0.5F), (double)((float)z + 0.5F), block.soundGroup.func_1145_d(), (block.soundGroup.getVolume() + 1.0F) / 2.0F, block.soundGroup.getPitch() * 0.8F);
                --itemStack.count;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public override String getItemNameIS(ItemStack itemStack)
    {
        return Block.BLOCKS[blockID].getBlockName();
    }

    public override String getItemName()
    {
        return Block.BLOCKS[blockID].getBlockName();
    }
}