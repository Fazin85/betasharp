namespace BetaSharp.Blocks;

public class BlockObsidian : BlockStone
{
    public BlockObsidian(int id, int textureId) : base(id, textureId)
    {
    }

    public override int getDroppedItemCount(java.util.Random random)
    {
        return 1;
    }

    public override int getDroppedItemId(int blockMeta, java.util.Random random)
    {
        return Block.Obsidian.id;
    }
}