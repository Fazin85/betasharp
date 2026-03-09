using BetaSharp.Blocks;
using BetaSharp.Entities;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Items;

public class ItemRecord : Item
{

    public readonly string recordName;

    public ItemRecord(int id, string recordName) : base(id)
    {
        this.recordName = recordName;
        maxCount = 1;
    }

    public override bool useOnBlock(ItemStack itemStack, EntityPlayer entityPlayer, IBlockWorldContext world, int x, int y, int z, int meta)
    {
        if (world.BlocksReader.GetBlockId(x, y, z) == Block.Jukebox.id && world.BlocksReader.GetMeta(x, y, z) == 0)
        {
            if (world.IsRemote)
            {
                return true;
            }
            else
            {
                ((BlockJukeBox)Block.Jukebox).insertRecord(world, x, y, z, id);
                world.Broadcaster.WorldEvent(1005, x, y, z, id);
                --itemStack.count;
                return true;
            }
        }
        else
        {
            return false;
        }
    }
}
