using BetaSharp.Blocks.Materials;

namespace BetaSharp.Blocks;

public class BlockBookshelf : Block
{
    public BlockBookshelf(int id, int textureId) : base(id, textureId, Material.Wood)
    {
    }

    public override int getTexture(int side)
    {
        return side <= 1 ? 4 : textureId;
    }

    public override int getDroppedItemCount(java.util.Random random)
    {
        return 0;
    }
}