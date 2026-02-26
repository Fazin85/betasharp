using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class RepeaterRenderer:IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess, in BlockRenderContext context) =>
        throw new NotImplementedException();
}
