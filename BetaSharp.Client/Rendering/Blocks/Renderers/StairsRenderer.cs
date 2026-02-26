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
        int direction = _blockAccess.getBlockMeta(x, y, z);

        if (direction == 0) // Ascending East (Stairs face West)
        {
            // Lower step (West half)
            SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 0.5F, 0.5F, 1.0F);
            RenderStandardBlock(block, x, y, z);

            // Upper step (East half)
            SetOverrideBoundingBox(0.5F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
            RenderStandardBlock(block, x, y, z);

            hasRendered = true;
        }
        else if (direction == 1) // Ascending West (Stairs face East)
        {
            // Upper step (West half)
            SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 0.5F, 1.0F, 1.0F);
            RenderStandardBlock(block, x, y, z);

            // Lower step (East half)
            SetOverrideBoundingBox(0.5F, 0.0F, 0.0F, 1.0F, 0.5F, 1.0F);
            RenderStandardBlock(block, x, y, z);

            hasRendered = true;
        }
        else if (direction == 2) // Ascending South (Stairs face North)
        {
            // Lower step (North half)
            SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 0.5F, 0.5F);
            RenderStandardBlock(block, x, y, z);

            // Upper step (South half)
            SetOverrideBoundingBox(0.0F, 0.0F, 0.5F, 1.0F, 1.0F, 1.0F);
            RenderStandardBlock(block, x, y, z);

            hasRendered = true;
        }
        else if (direction == 3) // Ascending North (Stairs face South)
        {
            // Upper step (North half)
            SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.5F);
            RenderStandardBlock(block, x, y, z);

            // Lower step (South half)
            SetOverrideBoundingBox(0.0F, 0.0F, 0.5F, 1.0F, 0.5F, 1.0F);
            RenderStandardBlock(block, x, y, z);

            hasRendered = true;
        }

        SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);

        return hasRendered;
    }
}
