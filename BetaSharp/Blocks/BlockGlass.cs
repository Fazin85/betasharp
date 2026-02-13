using BetaSharp.Blocks.Materials;

namespace BetaSharp.Blocks;

public class BlockGlass : BlockBreakable
{
    public BlockGlass(int id, int texture, Material material, bool bl) : base(id, texture, material, bl)
    {
    }

    public override int getDroppedItemCount(java.util.Random random)
    {
        return 0;
    }

    public override int getRenderLayer()
    {
        return 0;
    }
}