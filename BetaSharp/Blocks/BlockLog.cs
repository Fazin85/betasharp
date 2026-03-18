using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockLog : Block
{
    public BlockLog(int id) : base(id, Material.Wood)
    {
        textureId = "oak";
    }

    public override int getDroppedItemCount(JavaRandom random)
    {
        return 1;
    }

    public override int getDroppedItemId(int blockMeta, JavaRandom random)
    {
        return Block.Log.id;
    }

    public override void afterBreak(World world, EntityPlayer player, int x, int y, int z, int meta)
    {
        base.afterBreak(world, player, x, y, z, meta);
    }

    public override void onBreak(World world, int x, int y, int z)
    {
        sbyte searchRadius = 4;
        int regionExtent = searchRadius + 1;
        if (world.isRegionLoaded(x - regionExtent, y - regionExtent, z - regionExtent, x + regionExtent, y + regionExtent, z + regionExtent))
        {
            for (int offsetX = -searchRadius; offsetX <= searchRadius; ++offsetX)
            {
                for (int offsetY = -searchRadius; offsetY <= searchRadius; ++offsetY)
                {
                    for (int offsetZ = -searchRadius; offsetZ <= searchRadius; ++offsetZ)
                    {
                        int neighborBlockId = world.getBlockId(x + offsetX, y + offsetY, z + offsetZ);
                        if (neighborBlockId == Block.Leaves.id)
                        {
                            int leavesMeta = world.getBlockMeta(x + offsetX, y + offsetY, z + offsetZ);
                            if ((leavesMeta & 8) == 0)
                            {
                                world.SetBlockMetaWithoutNotifyingNeighbors(x + offsetX, y + offsetY, z + offsetZ, leavesMeta | 8);
                            }
                        }
                    }
                }
            }
        }

    }

    public override string getTexture(string side, int meta)
    {
        return side == "top" ? $"{textureId}_top" : (side == "bottom" ? $"{textureId}_top" : (meta == 2 ? textureId : (meta == 2 ? textureId : textureId)));
    }

    protected override int getDroppedItemMeta(int blockMeta)
    {
        return blockMeta;
    }
}
