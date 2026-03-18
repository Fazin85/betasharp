using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockLockedChest : Block
{

    public BlockLockedChest(int id) : base(id, Material.Wood)
    {
        textureId = "locked_chest";
    }

    public override string getTextureId(BlockView blockView, int x, int y, int z, string side)
    {
        if (side == "top")
        {
            return $"{textureId}_top";
        }
        else if (side == "bottom")
        {
            return $"{textureId}_top";
        }
        else
        {
            int var6 = blockView.getBlockId(x, y, z - 1);
            int var7 = blockView.getBlockId(x, y, z + 1);
            int var8 = blockView.getBlockId(x - 1, y, z);
            int var9 = blockView.getBlockId(x + 1, y, z);
            sbyte var10 = 3;
            if (Block.BlocksOpaque[var6] && !Block.BlocksOpaque[var7])
            {
                var10 = 3;
            }

            if (Block.BlocksOpaque[var7] && !Block.BlocksOpaque[var6])
            {
                var10 = 2;
            }

            if (Block.BlocksOpaque[var8] && !Block.BlocksOpaque[var9])
            {
                var10 = 5;
            }

            if (Block.BlocksOpaque[var9] && !Block.BlocksOpaque[var8])
            {
                var10 = 4;
            }

            return faceString2Int(side) == var10 ? $"{textureId}_front" : textureId;
        }
    }

    public override string getTexture(string side)
    {
        return side == "up" ? textureId
            : side == "down" ? $"{textureId}_top"
            : $"{textureId}_side";
    }


    public override bool canPlaceAt(World world, int x, int y, int z)
    {
        return true;
    }

    public override void onTick(World world, int x, int y, int z, JavaRandom random)
    {
        world.setBlock(x, y, z, 0);
    }
}
