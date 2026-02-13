using BetaSharp.Blocks.Materials;

namespace BetaSharp.Blocks;

public class BlockSandStone : Block
{
    public BlockSandStone(int id) : base(id, 192, Material.STONE)
    {
    }

    public override int getTexture(int side)
    {
        return side == 1 ? textureId - 16 : (side == 0 ? textureId + 16 : textureId);
    }
}