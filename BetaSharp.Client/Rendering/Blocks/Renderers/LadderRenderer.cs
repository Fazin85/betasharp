using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class LadderRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess,
        in BlockRenderContext context)
    {
        Tessellator tess = _tess;

        int textureId = block.getTexture(0);
        if (_overrideBlockTexture >= 0)
        {
            textureId = _overrideBlockTexture;
        }

        float luminance = block.getLuminance(_blockAccess, x, y, z);
        tess.setColorOpaque_F(luminance, luminance, luminance);

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = texU / 256.0D;
        double maxU = (texU + 15.99D) / 256.0D;
        double minV = texV / 256.0D;
        double maxV = (texV + 15.99D) / 256.0D;

        int metadata = _blockAccess.getBlockMeta(x, y, z);
        double offset = 0.05D;

        if (metadata == 5)
        {
            tess.addVertexWithUV(x + offset, y + 1.0D, z + 1.0D, minU, minV);
            tess.addVertexWithUV(x + offset, y + 0.0D, z + 1.0D, minU, maxV);
            tess.addVertexWithUV(x + offset, y + 0.0D, z + 0.0D, maxU, maxV);
            tess.addVertexWithUV(x + offset, y + 1.0D, z + 0.0D, maxU, minV);
        }
        else if (metadata == 4)
        {
            tess.addVertexWithUV(x + 1.0D - offset, y + 0.0D, z + 1.0D, maxU, maxV);
            tess.addVertexWithUV(x + 1.0D - offset, y + 1.0D, z + 1.0D, maxU, minV);
            tess.addVertexWithUV(x + 1.0D - offset, y + 1.0D, z + 0.0D, minU, minV);
            tess.addVertexWithUV(x + 1.0D - offset, y + 0.0D, z + 0.0D, minU, maxV);
        }
        else if (metadata == 3)
        {
            tess.addVertexWithUV(x + 1.0D, y + 0.0D, z + offset, maxU, maxV);
            tess.addVertexWithUV(x + 1.0D, y + 1.0D, z + offset, maxU, minV);
            tess.addVertexWithUV(x + 0.0D, y + 1.0D, z + offset, minU, minV);
            tess.addVertexWithUV(x + 0.0D, y + 0.0D, z + offset, minU, maxV);
        }
        else if (metadata == 2)
        {
            tess.addVertexWithUV(x + 1.0D, y + 1.0D, z + 1.0D - offset, minU, minV);
            tess.addVertexWithUV(x + 1.0D, y + 0.0D, z + 1.0D - offset, minU, maxV);
            tess.addVertexWithUV(x + 0.0D, y + 0.0D, z + 1.0D - offset, maxU, maxV);
            tess.addVertexWithUV(x + 0.0D, y + 1.0D, z + 1.0D - offset, maxU, minV);
        }

        return true;
    }
}
