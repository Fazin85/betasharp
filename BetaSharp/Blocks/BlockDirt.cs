using BetaSharp.Blocks.Materials;

namespace BetaSharp.Blocks;

internal class BlockDirt : Block
{
    public BlockDirt(int id, int textureId) : base(id, textureId, Material.Soil)
    {
    }
}
