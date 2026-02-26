using System;
using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class CactusRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess, in BlockRenderContext context)
    {
        Box bounds = context.OverrideBounds ?? block.BoundingBox;
        bool hasRendered = false;

        // Force the helper to use flat shading so it doesn't override our colors with the dummy struct
        var flatCtx = context with { EnableAo = false };

        // 1. Calculate the specific biome/tint color for this cactus
        int colorMultiplier = block.getColorMultiplier(world, pos.x, pos.y, pos.z);
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

        float centerLuminance = block.getLuminance(world, pos.x, pos.y, pos.z);
        float faceLuminance;

        FaceColors dummyColors = new FaceColors();

        // --- Bottom Face (Y - 1) ---
        if (flatCtx.RenderAllFaces || bounds.MinY > 0.0D || block.isSideVisible(world, pos.x, pos.y - 1, pos.z, 0))
        {
            faceLuminance = block.getLuminance(world, pos.x, pos.y - 1, pos.z);
            tess.setColorOpaque_F(rBottom * faceLuminance, gBottom * faceLuminance, bBottom * faceLuminance);

            int tex = block.getTextureId(world, pos.x, pos.y, pos.z, 0);
            Helper.RenderBottomFace(block, new Vec3D(pos.x, pos.y, pos.z), tess, flatCtx, dummyColors, tex, flatCtx.FlipTexture);
            hasRendered = true;
        }

        // --- Top Face (Y + 1) ---
        if (flatCtx.RenderAllFaces || bounds.MaxY < 1.0D || block.isSideVisible(world, pos.x, pos.y + 1, pos.z, 1))
        {
            faceLuminance = block.getLuminance(world, pos.x, pos.y + 1, pos.z);
            if (Math.Abs(bounds.MaxY - 1.0D) > 0.1 && !block.material.IsFluid)
            {
                faceLuminance = centerLuminance;
            }

            tess.setColorOpaque_F(rTop * faceLuminance, gTop * faceLuminance, bTop * faceLuminance);

            int tex = block.getTextureId(world, pos.x, pos.y, pos.z, 1);
            Helper.RenderTopFace(block, new Vec3D(pos.x, pos.y, pos.z), tess, flatCtx, dummyColors, tex, flatCtx.FlipTexture);
            hasRendered = true;
        }

        // --- East Face (Z - 1) ---
        if (flatCtx.RenderAllFaces || bounds.MinZ > 0.0D || block.isSideVisible(world, pos.x, pos.y, pos.z - 1, 2))
        {
            faceLuminance = block.getLuminance(world, pos.x, pos.y, pos.z - 1);
            if (bounds.MinZ > 0.0D) faceLuminance = centerLuminance;

            tess.setColorOpaque_F(rZ * faceLuminance, gZ * faceLuminance, bZ * faceLuminance);

            tess.setTranslationF(0.0F, 0.0F, inset);

            int tex = block.getTextureId(world, pos.x, pos.y, pos.z, 2);
            Helper.RenderEastFace(block, new Vec3D(pos.x, pos.y, pos.z), tess, flatCtx, dummyColors, tex, flatCtx.FlipTexture);

            tess.setTranslationF(0.0F, 0.0F, -inset);
            hasRendered = true;
        }

        // --- West Face (Z + 1) ---
        if (flatCtx.RenderAllFaces || bounds.MaxZ < 1.0D || block.isSideVisible(world, pos.x, pos.y, pos.z + 1, 3))
        {
            faceLuminance = block.getLuminance(world, pos.x, pos.y, pos.z + 1);
            if (bounds.MaxZ < 1.0D) faceLuminance = centerLuminance;

            tess.setColorOpaque_F(rZ * faceLuminance, gZ * faceLuminance, bZ * faceLuminance);

            tess.setTranslationF(0.0F, 0.0F, -inset);

            int tex = block.getTextureId(world, pos.x, pos.y, pos.z, 3);
            Helper.RenderWestFace(block, new Vec3D(pos.x, pos.y, pos.z), tess, flatCtx, dummyColors, tex, flatCtx.FlipTexture);

            tess.setTranslationF(0.0F, 0.0F, inset);
            hasRendered = true;
        }

        // --- North Face (X - 1) ---
        if (flatCtx.RenderAllFaces || bounds.MinX > 0.0D || block.isSideVisible(world, pos.x - 1, pos.y, pos.z, 4))
        {
            faceLuminance = block.getLuminance(world, pos.x - 1, pos.y, pos.z);
            if (bounds.MinX > 0.0D) faceLuminance = centerLuminance;

            tess.setColorOpaque_F(rX * faceLuminance, gX * faceLuminance, bX * faceLuminance);

            tess.setTranslationF(inset, 0.0F, 0.0F);

            int tex = block.getTextureId(world, pos.x, pos.y, pos.z, 4);
            Helper.RenderNorthFace(block, new Vec3D(pos.x, pos.y, pos.z), tess, flatCtx, dummyColors, tex, flatCtx.FlipTexture);

            tess.setTranslationF(-inset, 0.0F, 0.0F);
            hasRendered = true;
        }

        // --- South Face (X + 1) ---
        if (flatCtx.RenderAllFaces || bounds.MaxX < 1.0D || block.isSideVisible(world, pos.x + 1, pos.y, pos.z, 5))
        {
            faceLuminance = block.getLuminance(world, pos.x + 1, pos.y, pos.z);
            if (bounds.MaxX < 1.0D) faceLuminance = centerLuminance;

            tess.setColorOpaque_F(rX * faceLuminance, gX * faceLuminance, bX * faceLuminance);

            tess.setTranslationF(-inset, 0.0F, 0.0F);

            int tex = block.getTextureId(world, pos.x, pos.y, pos.z, 5);
            Helper.RenderSouthFace(block, new Vec3D(pos.x, pos.y, pos.z), tess, flatCtx, dummyColors, tex, flatCtx.FlipTexture);

            tess.setTranslationF(inset, 0.0F, 0.0F);
            hasRendered = true;
        }

        return hasRendered;
    }
}
