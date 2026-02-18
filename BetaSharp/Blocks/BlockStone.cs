using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;

namespace BetaSharp.Blocks;

public class BlockStone : Block
{
    public BlockStone(int id, int textureId) : base(id, textureId, Material.Stone)
    {
    }

    public override int getDroppedItemId(int blockMeta, JavaRandom random)
    {
        return Block.Cobblestone.id;
    }
}