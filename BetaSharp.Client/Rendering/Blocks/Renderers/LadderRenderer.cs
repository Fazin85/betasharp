using BetaSharp.Blocks;
using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class LadderRenderer : IBlockRenderer
{
    public bool Draw(Block block, in BlockPos pos, ref BlockRenderContext ctx)
    {
        int textureId = block.getTexture(0);
        if (ctx.OverrideTexture >= 0)
        {
            textureId = ctx.OverrideTexture;
        }

        float luminance = block.getLuminance(ctx.World, pos.x, pos.y, pos.z);
        ctx.Tess.setColorOpaque_F(luminance, luminance, luminance);

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = texU / 256.0D;
        double maxU = (texU + 15.99D) / 256.0D;
        double minV = texV / 256.0D;
        double maxV = (texV + 15.99D) / 256.0D;

        int metadata = ctx.World.getBlockMeta(pos.x, pos.y, pos.z);

        // Push the ladder slightly off the wall to prevent Z-fighting
        double offset = 0.05D;

        if (metadata == 5)
        {
            ctx.Tess.addVertexWithUV(pos.x + offset, pos.y + 1.0D, pos.z + 1.0D, minU, minV);
            ctx.Tess.addVertexWithUV(pos.x + offset, pos.y + 0.0D, pos.z + 1.0D, minU, maxV);
            ctx.Tess.addVertexWithUV(pos.x + offset, pos.y + 0.0D, pos.z + 0.0D, maxU, maxV);
            ctx.Tess.addVertexWithUV(pos.x + offset, pos.y + 1.0D, pos.z + 0.0D, maxU, minV);
        }
        else if (metadata == 4)
        {
            ctx.Tess.addVertexWithUV(pos.x + 1.0D - offset, pos.y + 0.0D, pos.z + 1.0D, maxU, maxV);
            ctx.Tess.addVertexWithUV(pos.x + 1.0D - offset, pos.y + 1.0D, pos.z + 1.0D, maxU, minV);
            ctx.Tess.addVertexWithUV(pos.x + 1.0D - offset, pos.y + 1.0D, pos.z + 0.0D, minU, minV);
            ctx.Tess.addVertexWithUV(pos.x + 1.0D - offset, pos.y + 0.0D, pos.z + 0.0D, minU, maxV);
        }
        else if (metadata == 3)
        {
            ctx.Tess.addVertexWithUV(pos.x + 1.0D, pos.y + 0.0D, pos.z + offset, maxU, maxV);
            ctx.Tess.addVertexWithUV(pos.x + 1.0D, pos.y + 1.0D, pos.z + offset, maxU, minV);
            ctx.Tess.addVertexWithUV(pos.x + 0.0D, pos.y + 1.0D, pos.z + offset, minU, minV);
            ctx.Tess.addVertexWithUV(pos.x + 0.0D, pos.y + 0.0D, pos.z + offset, minU, maxV);
        }
        else if (metadata == 2)
        {
            ctx.Tess.addVertexWithUV(pos.x + 1.0D, pos.y + 1.0D, pos.z + 1.0D - offset, minU, minV);
            ctx.Tess.addVertexWithUV(pos.x + 1.0D, pos.y + 0.0D, pos.z + 1.0D - offset, minU, maxV);
            ctx.Tess.addVertexWithUV(pos.x + 0.0D, pos.y + 0.0D, pos.z + 1.0D - offset, maxU, maxV);
            ctx.Tess.addVertexWithUV(pos.x + 0.0D, pos.y + 1.0D, pos.z + 1.0D - offset, maxU, minV);
        }

        return true;
    }
}
