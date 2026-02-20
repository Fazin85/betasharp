using BetaSharp.Blocks;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Items;

public class ItemBed : Item
{

    public ItemBed(int id) : base(id)
    {
    }

    public override bool useOnBlock(ItemStack itemStack, EntityPlayer entityPlayer, World world, int x, int y, int z, int meta)
    {
        if (meta != 1)
        {
            return false;
        }
        else
        {
            ++y;
            BlockBed blockBed = (BlockBed)Block.Bed;
            int direction = MathHelper.Floor((double)(entityPlayer.yaw * 4.0F / 360.0F) + 0.5D) & 3;
            sbyte headOffsetX = 0;
            sbyte headOffsetZ = 0;
            if (direction == 0)
            {
                headOffsetZ = 1;
            }

            if (direction == 1)
            {
                headOffsetX = -1;
            }

            if (direction == 2)
            {
                headOffsetZ = -1;
            }

            if (direction == 3)
            {
                headOffsetX = 1;
            }

            if (world.isAir(x, y, z) && world.isAir(x + headOffsetX, y, z + headOffsetZ) && world.shouldSuffocate(x, y - 1, z) && world.shouldSuffocate(x + headOffsetX, y - 1, z + headOffsetZ))
            {
                world.setBlock(x, y, z, blockBed.id, direction);
                world.setBlock(x + headOffsetX, y, z + headOffsetZ, blockBed.id, direction + 8);
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