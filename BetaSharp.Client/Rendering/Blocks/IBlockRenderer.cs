using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks;

public interface IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, BlockPos pos, Tessellator tess);
}
