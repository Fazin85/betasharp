using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class CactusRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess,
        in BlockRenderContext context)
    {
        Tessellator tess = _tess;
        Box bounds = _useOverrideBoundingBox ? _overrideBoundingBox : block.BoundingBox;
        bool hasRendered = false;

        // 1. Calculate the specific biome/tint color for this cactus
        int colorMultiplier = block.getColorMultiplier(_blockAccess, x, y, z);
        float red = (colorMultiplier >> 16 & 255) / 255.0F;
        float green = (colorMultiplier >> 8 & 255) / 255.0F;
        float blue = (colorMultiplier & 255) / 255.0F;

        // 2. Base directional lighting multipliers
        float lightBottom = 0.5F;
        float lightTop = 1.0F;
        float lightZ = 0.8F; // East/West faces
        float lightX = 0.6F; // North/South faces

        // Pre-calculate tinted colors for each face
        float rBottom = lightBottom * red, gBottom = lightBottom * green, bBottom = lightBottom * blue;
        float rTop = lightTop * red, gTop = lightTop * green, bTop = lightTop * blue;
        float rZ = lightZ * red, gZ = lightZ * green, bZ = lightZ * blue;
        float rX = lightX * red, gX = lightX * green, bX = lightX * blue;

        // 1/16th of a block = exactly 1 pixel width in a standard 16x16 texture
        float inset = 1.0F / 16.0F;

        float centerLuminance = block.getLuminance(_blockAccess, x, y, z);
        float faceLuminance;

        // --- Bottom Face (Y - 1) ---
        if (_renderAllFaces || bounds.MinY > 0.0D || block.isSideVisible(_blockAccess, x, y - 1, z, 0))
        {
            faceLuminance = block.getLuminance(_blockAccess, x, y - 1, z);
            tess.setColorOpaque_F(rBottom * faceLuminance, gBottom * faceLuminance, bBottom * faceLuminance);
            Helper.RenderBottomFace(block, x, y, z, block.getTextureId(_blockAccess, x, y, z, 0));
            hasRendered = true;
        }

        // --- Top Face (Y + 1) ---
        if (_renderAllFaces || bounds.MaxY < 1.0D || block.isSideVisible(_blockAccess, x, y + 1, z, 1))
        {
            faceLuminance = block.getLuminance(_blockAccess, x, y + 1, z);
            if (Math.Abs(bounds.MaxY - 1.0D) > 0.1 && !block.material.IsFluid)
            {
                faceLuminance = centerLuminance;
            }

            tess.setColorOpaque_F(rTop * faceLuminance, gTop * faceLuminance, bTop * faceLuminance);
            Helper.RenderTopFace(block, x, y, z, block.getTextureId(_blockAccess, x, y, z, 1));
            hasRendered = true;
        }

        // --- East Face (Z - 1) ---
        if (_renderAllFaces || bounds.MinZ > 0.0D || block.isSideVisible(_blockAccess, x, y, z - 1, 2))
        {
            faceLuminance = block.getLuminance(_blockAccess, x, y, z - 1);
            if (bounds.MinZ > 0.0D) faceLuminance = centerLuminance;

            tess.setColorOpaque_F(rZ * faceLuminance, gZ * faceLuminance, bZ * faceLuminance);

            // Translate inward by 1 pixel, render face, then reset
            tess.setTranslationF(0.0F, 0.0F, inset);
            Helper.RenderEastFace(block, x, y, z, block.getTextureId(_blockAccess, x, y, z, 2));
            tess.setTranslationF(0.0F, 0.0F, -inset);
            hasRendered = true;
        }

        // --- West Face (Z + 1) ---
        if (_renderAllFaces || bounds.MaxZ < 1.0D || block.isSideVisible(_blockAccess, x, y, z + 1, 3))
        {
            faceLuminance = block.getLuminance(_blockAccess, x, y, z + 1);
            if (bounds.MaxZ < 1.0D) faceLuminance = centerLuminance;

            tess.setColorOpaque_F(rZ * faceLuminance, gZ * faceLuminance, bZ * faceLuminance);

            tess.setTranslationF(0.0F, 0.0F, -inset);
            Helper.RenderWestFace(block, x, y, z, block.getTextureId(_blockAccess, x, y, z, 3));
            tess.setTranslationF(0.0F, 0.0F, inset);
            hasRendered = true;
        }

        // --- North Face (X - 1) ---
        if (_renderAllFaces || bounds.MinX > 0.0D || block.isSideVisible(_blockAccess, x - 1, y, z, 4))
        {
            faceLuminance = block.getLuminance(_blockAccess, x - 1, y, z);
            if (bounds.MinX > 0.0D) faceLuminance = centerLuminance;

            tess.setColorOpaque_F(rX * faceLuminance, gX * faceLuminance, bX * faceLuminance);

            tess.setTranslationF(inset, 0.0F, 0.0F);
            Helper.RenderNorthFace(block, x, y, z, block.getTextureId(_blockAccess, x, y, z, 4));
            tess.setTranslationF(-inset, 0.0F, 0.0F);
            hasRendered = true;
        }

        // --- South Face (X + 1) ---
        if (_renderAllFaces || bounds.MaxX < 1.0D || block.isSideVisible(_blockAccess, x + 1, y, z, 5))
        {
            faceLuminance = block.getLuminance(_blockAccess, x + 1, y, z);
            if (bounds.MaxX < 1.0D) faceLuminance = centerLuminance;

            tess.setColorOpaque_F(rX * faceLuminance, gX * faceLuminance, bX * faceLuminance);

            tess.setTranslationF(-inset, 0.0F, 0.0F);
            RenderSouthFace(block, x, y, z, block.getTextureId(_blockAccess, x, y, z, 5));
            tess.setTranslationF(inset, 0.0F, 0.0F);
            hasRendered = true;
        }

        return hasRendered;
    }
}
