using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockPumpkin : Block
{

    private bool lit;
    private bool cut;
    private int[] textures; //[top, side, front]
    public BlockPumpkin(int id, int textureId, bool lit, bool cut) : base(id, Material.Pumpkin)
    {
        this.textureId = textureId;
        textures = [textureId, textureId + 16, textureId + 17];//[top, side, front]
        setTickRandomly(true);
        this.lit = lit;
        this.cut = cut;
    }

    public override int getTexture(int side, int meta)
    {
        if (side == 0 || side == 1)
        {
            return textures[0];
        }
        if (cut)
        {
        bool isFace = (meta == 2 && side == 2) || 
              (meta == 3 && side == 5) || 
              (meta == 0 && side == 3) || 
              (meta == 1 && side == 4);

            if (isFace) 
            {
                if (lit)
                {
                    return textures[2] + 1;
                }
                return textures[2];
            } 
        }

        return textures[1];
    }

    public override int getTexture(int side)
    {
        if (side == 0 || side == 1)
        {
            return textures[0];
        }
        if (side == 3)
        {
            return textures[2];
        }
        return textures[1];
    }

    public override void onPlaced(World world, int x, int y, int z)
    {
        base.onPlaced(world, x, y, z);
    }

    public override bool canPlaceAt(World world, int x, int y, int z)
    {
        int blockId = world.getBlockId(x, y, z);
        return (blockId == 0 || Block.Blocks[blockId].material.IsReplaceable) && world.shouldSuffocate(x, y - 1, z);
    }

    public override void onPlaced(World world, int x, int y, int z, EntityLiving placer)
    {
        int direction = MathHelper.Floor((double)(placer.yaw * 4.0F / 360.0F) + 2.5D) & 3;
        world.setBlockMeta(x, y, z, direction);
    }
}
