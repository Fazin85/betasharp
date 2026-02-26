using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class CropsRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess, in BlockRenderContext context)
    {
        float luminance = block.getLuminance(world, pos.x, pos.y, pos.z);
        tess.setColorOpaque_F(luminance, luminance, luminance);

        int metadata = world.getBlockMeta(pos.x, pos.y, pos.z);

        // Crops are pushed down slightly into the soil block
        double yOffset = pos.y - (1.0D / 16.0D);

        RenderCropQuads(block, metadata, pos.x, yOffset, pos.z, tess, context);

        return true;
    }

    private void RenderCropQuads(Block block, int metadata, double x, double y, double z, Tessellator tess, in BlockRenderContext context)
    {
        int textureId = block.getTexture(0, metadata);

        if (context.OverrideTexture >= 0)
        {
            textureId = context.OverrideTexture;
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
        tess.addVertexWithUV(minX, y + 1.0D, minZ, minU, minV);
        tess.addVertexWithUV(minX, y + 0.0D, minZ, minU, maxV);
        tess.addVertexWithUV(minX, y + 0.0D, maxZ, maxU, maxV);
        tess.addVertexWithUV(minX, y + 1.0D, maxZ, maxU, minV);

        tess.addVertexWithUV(minX, y + 1.0D, maxZ, minU, minV);
        tess.addVertexWithUV(minX, y + 0.0D, maxZ, minU, maxV);
        tess.addVertexWithUV(minX, y + 0.0D, minZ, maxU, maxV);
        tess.addVertexWithUV(minX, y + 1.0D, minZ, maxU, minV);

        tess.addVertexWithUV(maxX, y + 1.0D, maxZ, minU, minV);
        tess.addVertexWithUV(maxX, y + 0.0D, maxZ, minU, maxV);
        tess.addVertexWithUV(maxX, y + 0.0D, minZ, maxU, maxV);
        tess.addVertexWithUV(maxX, y + 1.0D, minZ, maxU, minV);

        tess.addVertexWithUV(maxX, y + 1.0D, minZ, minU, minV);
        tess.addVertexWithUV(maxX, y + 0.0D, minZ, minU, maxV);
        tess.addVertexWithUV(maxX, y + 0.0D, maxZ, maxU, maxV);
        tess.addVertexWithUV(maxX, y + 1.0D, maxZ, maxU, minV);

        // --- Horizontal Planes (East-West aligned) ---
        // Reposition coordinates for the crossing planes
        minX = x + 0.5D - 0.5D;
        maxX = x + 0.5D + 0.5D;
        minZ = z + 0.5D - 0.25D;
        maxZ = z + 0.5D + 0.25D;

        tess.addVertexWithUV(minX, y + 1.0D, minZ, minU, minV);
        tess.addVertexWithUV(minX, y + 0.0D, minZ, minU, maxV);
        tess.addVertexWithUV(maxX, y + 0.0D, minZ, maxU, maxV);
        tess.addVertexWithUV(maxX, y + 1.0D, minZ, maxU, minV);

        tess.addVertexWithUV(maxX, y + 1.0D, minZ, minU, minV);
        tess.addVertexWithUV(maxX, y + 0.0D, minZ, minU, maxV);
        tess.addVertexWithUV(minX, y + 0.0D, minZ, maxU, maxV);
        tess.addVertexWithUV(minX, y + 1.0D, minZ, maxU, minV);

        tess.addVertexWithUV(maxX, y + 1.0D, maxZ, minU, minV);
        tess.addVertexWithUV(maxX, y + 0.0D, maxZ, minU, maxV);
        tess.addVertexWithUV(minX, y + 0.0D, maxZ, maxU, maxV);
        tess.addVertexWithUV(minX, y + 1.0D, maxZ, maxU, minV);

        tess.addVertexWithUV(minX, y + 1.0D, maxZ, minU, minV);
        tess.addVertexWithUV(minX, y + 0.0D, maxZ, minU, maxV);
        tess.addVertexWithUV(maxX, y + 0.0D, maxZ, maxU, maxV);
        tess.addVertexWithUV(maxX, y + 1.0D, maxZ, maxU, minV);
    }
}
