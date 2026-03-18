using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockSlab : Block
{
    public static readonly string[] names = ["stone", "sand", "wood", "cobble"];
    private bool doubleSlab;

    public BlockSlab(int id, bool doubleSlab) : base(id, "stone_slab", Material.Stone)
    {
        this.doubleSlab = doubleSlab;
        if (!doubleSlab)
        {
            setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 0.5F, 1.0F);
        }

        setOpacity(255);
    }

    public override string getTexture(string side, int meta)
    {
        return meta == 0 ? (side == "bottom" || side == "top" ? $"{textureId}_top" : $"{textureId}_side") : (meta == 1 ? (side == "bottom" || side == "top" ? $"{textureId}_side" : textureId) : (meta == 2 ? "oak_planks" : (meta == 3 ? "oak_planks" : "oak_planks")));
    }

    public override string getTexture(string side)
    {
        return getTexture(side, 0);
    }

    public override bool isOpaque()
    {
        return doubleSlab;
    }

    public override void onPlaced(World world, int x, int y, int z)
    {
        if (this != Block.Slab)
        {
            base.onPlaced(world, x, y, z);
        }

        int blockBelowId = world.getBlockId(x, y - 1, z);
        int slabMeta = world.getBlockMeta(x, y, z);
        int blockBelowMeta = world.getBlockMeta(x, y - 1, z);
        if (slabMeta == blockBelowMeta)
        {
            if (blockBelowId == Slab.id)
            {
                world.setBlock(x, y, z, 0);
                world.setBlock(x, y - 1, z, Block.DoubleSlab.id, slabMeta);
            }

        }
    }

    public override int getDroppedItemId(int blockMeta, JavaRandom random)
    {
        return Block.Slab.id;
    }

    public override int getDroppedItemCount(JavaRandom random)
    {
        return doubleSlab ? 2 : 1;
    }

    protected override int getDroppedItemMeta(int blockMeta)
    {
        return blockMeta;
    }

    public override bool isFullCube()
    {
        return doubleSlab;
    }

    public override bool isSideVisible(BlockView blockView, int x, int y, int z, int side)
    {
        if (this != Block.Slab)
        {
            base.isSideVisible(blockView, x, y, z, side);
        }

        return side == 1 ? true : (!base.isSideVisible(blockView, x, y, z, side) ? false : (side == 0 ? true : blockView.getBlockId(x, y, z) != id));
    }
}
