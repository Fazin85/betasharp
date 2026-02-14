using BetaSharp.Blocks.Materials;

namespace BetaSharp.Blocks;

public class BlockDirt : Block
{
    public BlockDirt(int id, int textureId) : base(id, textureId, Material.Soil)
    {
    }
}