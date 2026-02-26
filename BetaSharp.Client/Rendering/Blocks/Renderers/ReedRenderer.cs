using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class ReedRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess,
        in BlockRenderContext context)
    {
        Tessellator tess = _tess;

        float luminance = block.getLuminance(_blockAccess, x, y, z);
        int colorMultiplier = block.getColorMultiplier(_blockAccess, x, y, z);
        float r = (colorMultiplier >> 16 & 255) / 255.0F;
        float g = (colorMultiplier >> 8 & 255) / 255.0F;
        float b = (colorMultiplier & 255) / 255.0F;

        tess.setColorOpaque_F(luminance * r, luminance * g, luminance * b);

        double renderX = x;
        double renderY = y;
        double renderZ = z;

        if (block == Block.Grass)
        {
            long hash = x * 3129871L ^ z * 116129781L ^ y;
            hash = hash * hash * 42317861L + hash * 11L;

            renderX += (((hash >> 16 & 15L) / 15.0F) - 0.5D) * 0.5D;
            renderY += (((hash >> 20 & 15L) / 15.0F) - 1.0D) * 0.2D;
            renderZ += (((hash >> 24 & 15L) / 15.0F) - 0.5D) * 0.5D;
        }

        RenderCrossedSquares(block, _blockAccess.getBlockMeta(x, y, z), renderX, renderY, renderZ);
        return true;
    }
}
