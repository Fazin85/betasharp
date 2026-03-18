using BetaSharp.Blocks.Materials;

namespace BetaSharp.Blocks;

public class BlockSandStone : Block
{
    public BlockSandStone(int id) : base(id, "sandstone", Material.Stone)
    {
    }

    public override string getTexture(string side)
    {
        return side == "up" ? textureId
            : side == "down" ? $"{textureId}_top"
            : $"{textureId}Side";
    }

}
