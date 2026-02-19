using BetaSharp.Util.Maths;

namespace BetaSharp.Blocks;

public class BlockObsidian : BlockStone
{
    public BlockObsidian(int id, int textureId) : base(id, textureId)
    {
    }

    public override int getDroppedItemCount(JavaRandom random)
    {
        return 1;
    }

    public override int getDroppedItemId(int blockMeta, JavaRandom random)
    {
        return Block.Obsidian.id;
    }
}