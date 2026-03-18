using BetaSharp.Blocks.Materials;

namespace BetaSharp.Blocks;

public class BlockOreStorage : Block
{

    public BlockOreStorage(int id, string textureId) : base(id, Material.Metal)
    {
        base.textureId = textureId;
    }

    public override string getTexture(string side)
    {
        return side == "up" ? "top" : side == "down" ? "bottom" : "side";
    }
}
