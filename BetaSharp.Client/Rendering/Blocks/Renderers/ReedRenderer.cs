using BetaSharp.Blocks;
using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class ReedRenderer : IBlockRenderer
{
    public bool Render(Block block, in BlockPos pos, in BlockRenderContext ctx)
    {
        float luminance = block.getLuminance(ctx.World, pos.x, pos.y, pos.z);
        int colorMultiplier = block.getColorMultiplier(ctx.World, pos.x, pos.y, pos.z);
        float r = (colorMultiplier >> 16 & 255) / 255.0F;
        float g = (colorMultiplier >> 8 & 255) / 255.0F;
        float b = (colorMultiplier & 255) / 255.0F;

        ctx.Tess.setColorOpaque_F(luminance * r, luminance * g, luminance * b);

        double renderX = pos.x;
        double renderY = pos.y;
        double renderZ = pos.z;

        // Apply random organic offset for grass so it doesn't look grid-aligned
        if (block == Block.Grass) // Assuming Block.TallGrass or equivalent
        {
            long hash = pos.x * 3129871L ^ pos.z * 116129781L ^ pos.y;
            hash = hash * hash * 42317861L + hash * 11L;

            renderX += (((hash >> 16 & 15L) / 15.0F) - 0.5D) * 0.5D;
            renderY += (((hash >> 20 & 15L) / 15.0F) - 1.0D) * 0.2D;
            renderZ += (((hash >> 24 & 15L) / 15.0F) - 0.5D) * 0.5D;
        }

        RenderCrossedSquares(block, ctx.World.getBlockMeta(pos.x, pos.y, pos.z), renderX, renderY, renderZ,
            ctx);
        return true;
    }

    private void RenderCrossedSquares(Block block, int metadata, double x, double y, double z,
        in BlockRenderContext ctx)
    {
        int textureId = block.getTexture(0, metadata);
        if (ctx.OverrideTexture >= 0)
        {
            textureId = ctx.OverrideTexture;
        }

        // Convert texture ID to UV coordinates (0.0 to 1.0 range)
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = texU / 256.0F;
        double maxU = (texU + 15.99F) / 256.0F;
        double minV = texV / 256.0F;
        double maxV = (texV + 15.99F) / 256.0F;

        // Magic number 0.45 means the planes stretch from 0.05 to 0.95 within the block.
        // This slight inset prevents Z-fighting (flickering) if the plant touches an adjacent solid block.
        double minOffset = 0.5D - 0.45D; // 0.05
        double maxOffset = 0.5D + 0.45D; // 0.95

        double minX = x + minOffset;
        double maxX = x + maxOffset;
        double minZ = z + minOffset;
        double maxZ = z + maxOffset;

        // --- First Diagonal Plane (Bottom-Left to Top-Right across the X/Z grid) ---

        // Front side
        ctx.Tess.addVertexWithUV(minX, y + 1.0D, minZ, minU, minV);
        ctx.Tess.addVertexWithUV(minX, y + 0.0D, minZ, minU, maxV);
        ctx.Tess.addVertexWithUV(maxX, y + 0.0D, maxZ, maxU, maxV);
        ctx.Tess.addVertexWithUV(maxX, y + 1.0D, maxZ, maxU, minV);

        // Back side (reversed winding order and UVs)
        ctx.Tess.addVertexWithUV(maxX, y + 1.0D, maxZ, minU, minV);
        ctx.Tess.addVertexWithUV(maxX, y + 0.0D, maxZ, minU, maxV);
        ctx.Tess.addVertexWithUV(minX, y + 0.0D, minZ, maxU, maxV);
        ctx.Tess.addVertexWithUV(minX, y + 1.0D, minZ, maxU, minV);

        // --- Second Diagonal Plane (Top-Left to Bottom-Right across the X/Z grid) ---

        // Front side
        ctx.Tess.addVertexWithUV(minX, y + 1.0D, maxZ, minU, minV);
        ctx.Tess.addVertexWithUV(minX, y + 0.0D, maxZ, minU, maxV);
        ctx.Tess.addVertexWithUV(maxX, y + 0.0D, minZ, maxU, maxV);
        ctx.Tess.addVertexWithUV(maxX, y + 1.0D, minZ, maxU, minV);

        // Back side (reversed winding order and UVs)
        ctx.Tess.addVertexWithUV(maxX, y + 1.0D, minZ, minU, minV);
        ctx.Tess.addVertexWithUV(maxX, y + 0.0D, minZ, minU, maxV);
        ctx.Tess.addVertexWithUV(minX, y + 0.0D, maxZ, maxU, maxV);
        ctx.Tess.addVertexWithUV(minX, y + 1.0D, maxZ, maxU, minV);
    }
}
