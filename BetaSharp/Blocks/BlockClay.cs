using BetaSharp.Blocks.Materials;
using BetaSharp.Items;
using BetaSharp.Util.Maths;

namespace BetaSharp.Blocks;

public class BlockClay : Block
{

    public BlockClay(int id, int textureId) : base(id, textureId, Material.Clay)
    {
    }

    public override int getDroppedItemId(int blockMeta, JavaRandom random)
    {
        return Item.Clay.id;
    }

    public override int getDroppedItemCount(JavaRandom random)
    {
        return 4;
    }
}