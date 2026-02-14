using BetaSharp.Blocks.Materials;
using BetaSharp.Items;

namespace BetaSharp.Blocks;

public class BlockClay : Block
{

    public BlockClay(int id, int textureId) : base(id, textureId, Material.Clay)
    {
    }

    public override int getDroppedItemId(int blockMeta, java.util.Random random)
    {
        return Item.CLAY.id;
    }

    public override int getDroppedItemCount(java.util.Random random)
    {
        return 4;
    }
}