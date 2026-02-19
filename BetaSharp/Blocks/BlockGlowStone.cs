using BetaSharp.Blocks.Materials;
using BetaSharp.Items;
using BetaSharp.Util.Maths;

namespace BetaSharp.Blocks;

public class BlockGlowstone : Block
{

    public BlockGlowstone(int i, int j, Material material) : base(i, j, material)
    {
    }

    public override int getDroppedItemCount(JavaRandom random)
    {
        return 2 + random.NextInt(3);
    }

    public override int getDroppedItemId(int blockMeta, JavaRandom random)
    {
        return Item.GlowstoneDust.id;
    }
}