using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class StairsRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess,
        in BlockRenderContext context)
    {
        bool hasRendered = false;
        int direction = world.getBlockMeta(pos.x, pos.y, pos.z);

        if (direction == 0) // Ascending East (Stairs face West)
        {
            // Lower step (West half)
            var lowerCtx = context with { OverrideBounds = new Box(0.0F, 0.0F, 0.0F, 0.5F, 0.5F, 1.0F) };
            hasRendered |= lowerCtx.RenderStandardBlock(block, pos, world, tess);

            // Upper step (East half)
            var upperCtx = context with { OverrideBounds = new Box(0.5F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F) };
            hasRendered |= upperCtx.RenderStandardBlock(block, pos, world, tess);
        }
        else if (direction == 1) // Ascending West (Stairs face East)
        {
            // Upper step (West half)
            var upperCtx = context with { OverrideBounds = new Box(0.0F, 0.0F, 0.0F, 0.5F, 1.0F, 1.0F) };
            hasRendered |= upperCtx.RenderStandardBlock(block, pos, world, tess);

            // Lower step (East half)
            var lowerCtx = context with { OverrideBounds = new Box(0.5F, 0.0F, 0.0F, 1.0F, 0.5F, 1.0F) };
            hasRendered |= lowerCtx.RenderStandardBlock(block, pos, world, tess);
        }
        else if (direction == 2) // Ascending South (Stairs face North)
        {
            // Lower step (North half)
            var lowerCtx = context with { OverrideBounds = new Box(0.0F, 0.0F, 0.0F, 1.0F, 0.5F, 0.5F) };
            hasRendered |= lowerCtx.RenderStandardBlock(block, pos, world, tess);

            // Upper step (South half)
            var upperCtx = context with { OverrideBounds = new Box(0.0F, 0.0F, 0.5F, 1.0F, 1.0F, 1.0F) };
            hasRendered |= upperCtx.RenderStandardBlock(block, pos, world, tess);
        }
        else if (direction == 3) // Ascending North (Stairs face South)
        {
            // Upper step (North half)
            var upperCtx = context with { OverrideBounds = new Box(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.5F) };
            hasRendered |= upperCtx.RenderStandardBlock(block, pos, world, tess);

            // Lower step (South half)
            var lowerCtx = context with { OverrideBounds = new Box(0.0F, 0.0F, 0.5F, 1.0F, 0.5F, 1.0F) };
            hasRendered |= lowerCtx.RenderStandardBlock(block, pos, world, tess);
        }

        // Notice: No cleanup required!
        // The original context remains untouched and the sub-contexts just fall out of scope.

        return hasRendered;
    }
}
