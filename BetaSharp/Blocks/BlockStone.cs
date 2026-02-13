using BetaSharp.Blocks.Materials;

namespace BetaSharp.Blocks;

public class BlockStone : Block
{
    public BlockStone(int id, int textureId) : base(id, textureId, Material.STONE)
    {
    }

    public override int getDroppedItemId(int blockMeta, java.util.Random random)
    {
        return Block.COBBLESTONE.id;
    }
}