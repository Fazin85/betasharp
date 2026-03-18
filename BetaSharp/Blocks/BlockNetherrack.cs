using BetaSharp.Blocks.Materials;

namespace BetaSharp.Blocks;

public class BlockNetherrack : Block
{

    public BlockNetherrack(int id, string textureId) : base(id, textureId, Material.Stone)
    {
    }
}