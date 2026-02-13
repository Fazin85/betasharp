using BetaSharp.Blocks.Materials;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockBreakable : Block
{
    private bool hideAdjacentFaces;

    protected BlockBreakable(int id, int textureId, Material material, bool hideAdjacentFaces) : base(id, textureId, material)
    {
        this.hideAdjacentFaces = hideAdjacentFaces;
    }

    public override bool isOpaque()
    {
        return false;
    }

    public override bool isSideVisible(BlockView blockView, int x, int y, int z, int side)
    {
        int neighborBlockId = blockView.getBlockId(x, y, z);
        return !hideAdjacentFaces && neighborBlockId == id ? false : base.isSideVisible(blockView, x, y, z, side);
    }
}