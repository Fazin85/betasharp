using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class FenceRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess, in BlockRenderContext context)
    {
        bool hasRendered = true;

        // 1. Render the central vertical post
        float postMin = 6.0F / 16.0F;
        float postMax = 10.0F / 16.0F;

        // Clone the context and apply the new bounding box for the post
        var postCtx = context with { OverrideBounds = new Box(postMin, 0.0F, postMin, postMax, 1.0F, postMax) };
        Helper.RenderStandardBlock(block, pos, world, tess, postCtx);

        // Check for adjacent fences using 'world' and 'pos'
        bool connectsWest = world.getBlockId(pos.x - 1, pos.y, pos.z) == block.id;
        bool connectsEast = world.getBlockId(pos.x + 1, pos.y, pos.z) == block.id;
        bool connectsNorth = world.getBlockId(pos.x, pos.y, pos.z - 1) == block.id;
        bool connectsSouth = world.getBlockId(pos.x, pos.y, pos.z + 1) == block.id;

        bool connectsX = connectsWest || connectsEast;
        bool connectsZ = connectsNorth || connectsSouth;

        // If the fence is completely isolated, default to drawing small stubs along the X-axis
        if (!connectsX && !connectsZ)
        {
            connectsX = true;
        }

        // Base depth/thickness for the horizontal connecting bars
        float barDepthMin = 7.0F / 16.0F;
        float barDepthMax = 9.0F / 16.0F;

        // Determine how far the bars extend based on neighbor connections
        float barMinX = connectsWest ? 0.0F : barDepthMin;
        float barMaxX = connectsEast ? 1.0F : barDepthMax;
        float barMinZ = connectsNorth ? 0.0F : barDepthMin;
        float barMaxZ = connectsSouth ? 1.0F : barDepthMax;

        // 2. Render Top Connecting Bars
        float topBarMinY = 12.0F / 16.0F;
        float topBarMaxY = 15.0F / 16.0F;

        if (connectsX)
        {
            var topXCtx = context with { OverrideBounds = new Box(barMinX, topBarMinY, barDepthMin, barMaxX, topBarMaxY, barDepthMax) };
            Helper.RenderStandardBlock(block, pos, world, tess, topXCtx);
        }

        if (connectsZ)
        {
            var topZCtx = context with { OverrideBounds = new Box(barDepthMin, topBarMinY, barMinZ, barDepthMax, topBarMaxY, barMaxZ) };
            Helper.RenderStandardBlock(block, pos, world, tess, topZCtx);
        }

        // 3. Render Bottom Connecting Bars
        float bottomBarMinY = 6.0F / 16.0F;
        float bottomBarMaxY = 9.0F / 16.0F;

        if (connectsX)
        {
            var bottomXCtx = context with { OverrideBounds = new Box(barMinX, bottomBarMinY, barDepthMin, barMaxX, bottomBarMaxY, barDepthMax) };
            Helper.RenderStandardBlock(block, pos, world, tess, bottomXCtx);
        }

        if (connectsZ)
        {
            var bottomZCtx = context with { OverrideBounds = new Box(barDepthMin, bottomBarMinY, barMinZ, barDepthMax, bottomBarMaxY, barMaxZ) };
            Helper.RenderStandardBlock(block, pos, world, tess, bottomZCtx);
        }

        // Notice we COMPLETELY REMOVED the bounding box reset!
        // The original 'context' was never mutated, so we don't need to clean up after ourselves.

        return hasRendered;
    }
}
