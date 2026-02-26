using BetaSharp.Blocks;
using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class CropsRenderer : IBlockRenderer
{
    public bool Render(Block block, in BlockPos pos, in BlockRenderContext ctx)
    {
        float luminance = block.getLuminance(ctx.World, pos.x, pos.y, pos.z);
        ctx.Tess.setColorOpaque_F(luminance, luminance, luminance);

        int metadata = ctx.World.getBlockMeta(pos.x, pos.y, pos.z);

        // Crops are pushed down slightly into the soil block
        double yOffset = pos.y - (1.0D / 16.0D);

        RenderCropQuads(block, metadata, pos.x, yOffset, pos.z, ctx);

        return true;
    }

    private void RenderCropQuads(Block block, int metadata, double x, double y, double z, in BlockRenderContext ctx)
    {
        int textureId = block.getTexture(0, metadata);

        if (ctx.OverrideTexture >= 0)
        {
            textureId = ctx.OverrideTexture;
        }

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = (texU / 256.0F);
        double maxU = ((texU + 15.99F) / 256.0F);
        double minV = (texV / 256.0F);
        double maxV = ((texV + 15.99F) / 256.0F);

        double minX = x + 0.5D - 0.25D; // Left plane X
        double maxX = x + 0.5D + 0.25D; // Right plane X
        double minZ = z + 0.5D - 0.5D; // Front plane Z
        double maxZ = z + 0.5D + 0.5D; // Back plane Z

        // --- Vertical Planes (North-South aligned) ---
        ctx.Tess.addVertexWithUV(minX, y + 1.0D, minZ, minU, minV);
        ctx.Tess.addVertexWithUV(minX, y + 0.0D, minZ, minU, maxV);
        ctx.Tess.addVertexWithUV(minX, y + 0.0D, maxZ, maxU, maxV);
        ctx.Tess.addVertexWithUV(minX, y + 1.0D, maxZ, maxU, minV);

        ctx.Tess.addVertexWithUV(minX, y + 1.0D, maxZ, minU, minV);
        ctx.Tess.addVertexWithUV(minX, y + 0.0D, maxZ, minU, maxV);
        ctx.Tess.addVertexWithUV(minX, y + 0.0D, minZ, maxU, maxV);
        ctx.Tess.addVertexWithUV(minX, y + 1.0D, minZ, maxU, minV);

        ctx.Tess.addVertexWithUV(maxX, y + 1.0D, maxZ, minU, minV);
        ctx.Tess.addVertexWithUV(maxX, y + 0.0D, maxZ, minU, maxV);
        ctx.Tess.addVertexWithUV(maxX, y + 0.0D, minZ, maxU, maxV);
        ctx.Tess.addVertexWithUV(maxX, y + 1.0D, minZ, maxU, minV);

        ctx.Tess.addVertexWithUV(maxX, y + 1.0D, minZ, minU, minV);
        ctx.Tess.addVertexWithUV(maxX, y + 0.0D, minZ, minU, maxV);
        ctx.Tess.addVertexWithUV(maxX, y + 0.0D, maxZ, maxU, maxV);
        ctx.Tess.addVertexWithUV(maxX, y + 1.0D, maxZ, maxU, minV);

        // --- Horizontal Planes (East-West aligned) ---
        // Reposition coordinates for the crossing planes
        minX = x + 0.5D - 0.5D;
        maxX = x + 0.5D + 0.5D;
        minZ = z + 0.5D - 0.25D;
        maxZ = z + 0.5D + 0.25D;

        ctx.Tess.addVertexWithUV(minX, y + 1.0D, minZ, minU, minV);
        ctx.Tess.addVertexWithUV(minX, y + 0.0D, minZ, minU, maxV);
        ctx.Tess.addVertexWithUV(maxX, y + 0.0D, minZ, maxU, maxV);
        ctx.Tess.addVertexWithUV(maxX, y + 1.0D, minZ, maxU, minV);

        ctx.Tess.addVertexWithUV(maxX, y + 1.0D, minZ, minU, minV);
        ctx.Tess.addVertexWithUV(maxX, y + 0.0D, minZ, minU, maxV);
        ctx.Tess.addVertexWithUV(minX, y + 0.0D, minZ, maxU, maxV);
        ctx.Tess.addVertexWithUV(minX, y + 1.0D, minZ, maxU, minV);

        ctx.Tess.addVertexWithUV(maxX, y + 1.0D, maxZ, minU, minV);
        ctx.Tess.addVertexWithUV(maxX, y + 0.0D, maxZ, minU, maxV);
        ctx.Tess.addVertexWithUV(minX, y + 0.0D, maxZ, maxU, maxV);
        ctx.Tess.addVertexWithUV(minX, y + 1.0D, maxZ, maxU, minV);

        ctx.Tess.addVertexWithUV(minX, y + 1.0D, maxZ, minU, minV);
        ctx.Tess.addVertexWithUV(minX, y + 0.0D, maxZ, minU, maxV);
        ctx.Tess.addVertexWithUV(maxX, y + 0.0D, maxZ, maxU, maxV);
        ctx.Tess.addVertexWithUV(maxX, y + 1.0D, maxZ, maxU, minV);
    }
}
