using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class LadderRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess, in BlockRenderContext context)
    {
        int textureId = block.getTexture(0);
        if (context.OverrideTexture >= 0)
        {
            textureId = context.OverrideTexture;
        }

        float luminance = block.getLuminance(world, pos.x, pos.y, pos.z);
        tess.setColorOpaque_F(luminance, luminance, luminance);

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = texU / 256.0D;
        double maxU = (texU + 15.99D) / 256.0D;
        double minV = texV / 256.0D;
        double maxV = (texV + 15.99D) / 256.0D;

        int metadata = world.getBlockMeta(pos.x, pos.y, pos.z);

        // Push the ladder slightly off the wall to prevent Z-fighting
        double offset = 0.05D;

        if (metadata == 5)
        {
            tess.addVertexWithUV(pos.x + offset, pos.y + 1.0D, pos.z + 1.0D, minU, minV);
            tess.addVertexWithUV(pos.x + offset, pos.y + 0.0D, pos.z + 1.0D, minU, maxV);
            tess.addVertexWithUV(pos.x + offset, pos.y + 0.0D, pos.z + 0.0D, maxU, maxV);
            tess.addVertexWithUV(pos.x + offset, pos.y + 1.0D, pos.z + 0.0D, maxU, minV);
        }
        else if (metadata == 4)
        {
            tess.addVertexWithUV(pos.x + 1.0D - offset, pos.y + 0.0D, pos.z + 1.0D, maxU, maxV);
            tess.addVertexWithUV(pos.x + 1.0D - offset, pos.y + 1.0D, pos.z + 1.0D, maxU, minV);
            tess.addVertexWithUV(pos.x + 1.0D - offset, pos.y + 1.0D, pos.z + 0.0D, minU, minV);
            tess.addVertexWithUV(pos.x + 1.0D - offset, pos.y + 0.0D, pos.z + 0.0D, minU, maxV);
        }
        else if (metadata == 3)
        {
            tess.addVertexWithUV(pos.x + 1.0D, pos.y + 0.0D, pos.z + offset, maxU, maxV);
            tess.addVertexWithUV(pos.x + 1.0D, pos.y + 1.0D, pos.z + offset, maxU, minV);
            tess.addVertexWithUV(pos.x + 0.0D, pos.y + 1.0D, pos.z + offset, minU, minV);
            tess.addVertexWithUV(pos.x + 0.0D, pos.y + 0.0D, pos.z + offset, minU, maxV);
        }
        else if (metadata == 2)
        {
            tess.addVertexWithUV(pos.x + 1.0D, pos.y + 1.0D, pos.z + 1.0D - offset, minU, minV);
            tess.addVertexWithUV(pos.x + 1.0D, pos.y + 0.0D, pos.z + 1.0D - offset, minU, maxV);
            tess.addVertexWithUV(pos.x + 0.0D, pos.y + 0.0D, pos.z + 1.0D - offset, maxU, maxV);
            tess.addVertexWithUV(pos.x + 0.0D, pos.y + 1.0D, pos.z + 1.0D - offset, maxU, minV);
        }

        return true;
    }
}
