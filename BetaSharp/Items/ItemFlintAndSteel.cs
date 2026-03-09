using BetaSharp.Blocks;
using BetaSharp.Entities;
using BetaSharp.Worlds.Core;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Items;

internal class ItemFlintAndSteel : Item
{

    public ItemFlintAndSteel(int id) : base(id)
    {
        maxCount = 1;
        setMaxDamage(64);
    }

    public override bool useOnBlock(ItemStack itemStack, EntityPlayer entityPlayer, IBlockWorldContext world, int x, int y, int z, int meta)
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

        int blockId = world.BlocksReader.GetBlockId(x, y, z);
        if (blockId == 0)
        {
            world.Broadcaster.PlaySoundAtPos(x + 0.5D, y + 0.5D, z + 0.5D, "fire.ignite", 1.0F, itemRand.NextFloat() * 0.4F + 0.8F);
            world.BlockWriter.SetBlock(x, y, z, Block.Fire.id);
        }

        itemStack.damageItem(1, entityPlayer);
        return true;
    }
}
