using BetaSharp.Blocks.Materials;
using BetaSharp.Items;

namespace BetaSharp.Blocks;

public class BlockGlowstone : Block
{

    public BlockGlowstone(int i, int j, Material material) : base(i, j, material)
    {
    }

    public override int getDroppedItemCount(java.util.Random random)
    {
        return 2 + random.nextInt(3);
    }

    public override int getDroppedItemId(int blockMeta, java.util.Random random)
    {
        return Item.GlowstoneDust.id;
    }
}