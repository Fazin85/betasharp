using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks;

public interface IBlockRenderer
{
    bool Render(Block block, in BlockPos pos, in BlockRenderContext ctx);
}
