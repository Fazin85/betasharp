using BetaSharp.Blocks.Materials;

namespace BetaSharp.Blocks;

public class BlockOreStorage : Block
{

    public BlockOreStorage(int id, int textureId) : base(id, Material.Metal)
    {
        base.textureId = textureId;
    }

    public override int getTexture(int side)
    {
        return textureId;
    }
}