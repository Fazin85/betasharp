using BetaSharp.Blocks;
using BetaSharp.Entities;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Items;

internal class ItemBlock : Item
{

    private int blockID;

    public ItemBlock(int id) : base(id)
    {
        blockID = id + 256;
        setTextureId(Block.Blocks[id + 256].getTexture(2));
    }

    public override bool useOnBlock(ItemStack itemStack, EntityPlayer entityPlayer, IBlockWorldContext world, int x, int y, int z, int meta)
    {
        if (world.BlocksReader.GetBlockId(x, y, z) == Block.Snow.id)
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
        else if (y == 127 && Block.Blocks[blockID].material.IsSolid)
        {
            return false;
        }
        else if (Block.Blocks[blockID].canPlaceAt(new CanPlaceAtCtx(world, 0, x, y, z)))
        {
            Block block = Block.Blocks[blockID];
            if (world.BlockWriter.SetBlock(x, y, z, blockID, getPlacementMetadata(itemStack.getDamage())))
            {
                Block.Blocks[blockID].onPlaced(new OnPlacedEvt(world, null, 0, 0, x, y, z));
                Block.Blocks[blockID].onPlaced(new OnPlacedEvt(world, entityPlayer, 0, 0, x, y, z));
                world.Broadcaster.PlaySoundAtPos(x + 0.5F, y + 0.5F, z + 0.5F, block.soundGroup.StepSound, (block.soundGroup.Volume + 1.0F) / 2.0F, block.soundGroup.Pitch * 0.8F);
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
        return Block.Blocks[blockID].getBlockName();
    }

    public override String getItemName()
    {
        return Block.Blocks[blockID].getBlockName();
    }
}
