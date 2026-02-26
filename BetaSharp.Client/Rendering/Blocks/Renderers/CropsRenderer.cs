using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class CropsRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess,
        in BlockRenderContext context)
    {
        Tessellator tess = _tess;
        float luminance = block.getLuminance(world, x, y, z);
        tess.setColorOpaque_F(luminance, luminance, luminance);

        int metadata = world.getBlockMeta(x, y, z);

        double yOffset = y - (1.0D / 16.0D);
        RenderCropQuads(block, metadata, x, yOffset, z);
        return true;
    }

     private void RenderCropQuads(Block block, int metadata, double x, double y, double z)
    {
        Tessellator tess = _tess;
        int textureId = block.getTexture(0, metadata);

        if (_overrideBlockTexture >= 0)
        {
            textureId = _overrideBlockTexture;
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
