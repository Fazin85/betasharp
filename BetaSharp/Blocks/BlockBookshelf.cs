using BetaSharp.Blocks.Materials;

namespace BetaSharp.Blocks;

public class BlockBookshelf : Block
{
    public BlockBookshelf(int Id, int TextureId) : base(Id, TextureId, Material.Wood)
    {
    }

    public override int getTexture(int Side)
    {
        return Side <= 1 ? 4 : textureId;
    }

    public override int getDroppedItemCount(java.util.Random Random)
    {
        return 0;
    }
}