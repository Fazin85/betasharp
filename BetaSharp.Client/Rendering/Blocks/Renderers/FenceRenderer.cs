using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class FenceRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess,
        in BlockRenderContext context)
    {
        bool hasRendered = true;

        // 1. Render the central vertical post
        float postMin = 6.0F / 16.0F;
        float postMax = 10.0F / 16.0F;
        SetOverrideBoundingBox(postMin, 0.0F, postMin, postMax, 1.0F, postMax);
        RenderStandardBlock(block, x, y, z);

        // Check for adjacent fences
        bool connectsWest = _blockAccess.getBlockId(x - 1, y, z) == block.id;
        bool connectsEast = _blockAccess.getBlockId(x + 1, y, z) == block.id;
        bool connectsNorth = _blockAccess.getBlockId(x, y, z - 1) == block.id;
        bool connectsSouth = _blockAccess.getBlockId(x, y, z + 1) == block.id;

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
        // If connecting, stretch to the edge (0.0 or 1.0). Otherwise, stay near the post.
        float barMinX = connectsWest ? 0.0F : barDepthMin;
        float barMaxX = connectsEast ? 1.0F : barDepthMax;
        float barMinZ = connectsNorth ? 0.0F : barDepthMin;
        float barMaxZ = connectsSouth ? 1.0F : barDepthMax;

        // 2. Render Top Connecting Bars
        float topBarMinY = 12.0F / 16.0F;
        float topBarMaxY = 15.0F / 16.0F;

        if (connectsX)
        {
            SetOverrideBoundingBox(barMinX, topBarMinY, barDepthMin, barMaxX, topBarMaxY, barDepthMax);
            RenderStandardBlock(block, x, y, z);
        }

        if (connectsZ)
        {
            SetOverrideBoundingBox(barDepthMin, topBarMinY, barMinZ, barDepthMax, topBarMaxY, barMaxZ);
            RenderStandardBlock(block, x, y, z);
        }

        // 3. Render Bottom Connecting Bars
        float bottomBarMinY = 6.0F / 16.0F;
        float bottomBarMaxY = 9.0F / 16.0F;

        if (connectsX)
        {
            SetOverrideBoundingBox(barMinX, bottomBarMinY, barDepthMin, barMaxX, bottomBarMaxY, barDepthMax);
            RenderStandardBlock(block, x, y, z);
        }

        if (connectsZ)
        {
            SetOverrideBoundingBox(barDepthMin, bottomBarMinY, barMinZ, barDepthMax, bottomBarMaxY, barMaxZ);
            RenderStandardBlock(block, x, y, z);
        }

        // Reset bounding box state to prevent breaking the next block in the chunk
        SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);

        return hasRendered;
    }
}
