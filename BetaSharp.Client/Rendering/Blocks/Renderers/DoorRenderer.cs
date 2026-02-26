using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class DoorRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess,
        in BlockRenderContext context)
    {
        Tessellator tess = _tess;
        Box bounds = _useOverrideBoundingBox ? _overrideBoundingBox : block.BoundingBox;

        float lightBottom = 0.5F;
        float lightTop = 1.0F;
        float lightZ = 0.8F; // East/West
        float lightX = 0.6F; // North/South

        float blockLuminance = block.getLuminance(_blockAccess, x, y, z);

        bool isLightEmitter = Block.BlocksLightLuminance[block.id] > 0;

        // --- Bottom Face (Y - 1) ---
        float faceLuminance = block.getLuminance(_blockAccess, x, y - 1, z);
        if (bounds.MinY > 0.0D) faceLuminance = blockLuminance;
        if (isLightEmitter) faceLuminance = 1.0F;

        tess.setColorOpaque_F(lightBottom * faceLuminance, lightBottom * faceLuminance, lightBottom * faceLuminance);
        Helper.RenderBottomFace(block, x, y, z, block.getTextureId(_blockAccess, x, y, z, 0));

        // --- Top Face (Y + 1) ---
        faceLuminance = block.getLuminance(_blockAccess, x, y + 1, z);
        if (bounds.MaxY < 1.0D) faceLuminance = blockLuminance;
        if (isLightEmitter) faceLuminance = 1.0F;

        tess.setColorOpaque_F(lightTop * faceLuminance, lightTop * faceLuminance, lightTop * faceLuminance);
        Helper.RenderTopFace(block, x, y, z, block.getTextureId(_blockAccess, x, y, z, 1));

        // --- East Face (Z - 1) ---
        faceLuminance = block.getLuminance(_blockAccess, x, y, z - 1);
        if (bounds.MinZ > 0.0D) faceLuminance = blockLuminance;
        if (isLightEmitter) faceLuminance = 1.0F;

        tess.setColorOpaque_F(lightZ * faceLuminance, lightZ * faceLuminance, lightZ * faceLuminance);
        int textureId = block.getTextureId(_blockAccess, x, y, z, 2);

        // Negative texture ID is used as a flag to flip the texture horizontally (for door hinges)
        if (textureId < 0)
        {
            flipTexture = true;
            textureId = -textureId;
        }

        Helper.RenderEastFace(block, x, y, z, textureId);
        flipTexture = false;

        // --- West Face (Z + 1) ---
        faceLuminance = block.getLuminance(_blockAccess, x, y, z + 1);
        if (bounds.MaxZ < 1.0D) faceLuminance = blockLuminance;
        if (isLightEmitter) faceLuminance = 1.0F;

        tess.setColorOpaque_F(lightZ * faceLuminance, lightZ * faceLuminance, lightZ * faceLuminance);
        textureId = block.getTextureId(_blockAccess, x, y, z, 3);
        if (textureId < 0)
        {
            flipTexture = true;
            textureId = -textureId;
        }

        Helper.RenderWestFace(block, x, y, z, textureId);
        flipTexture = false;

        // --- North Face (X - 1) ---
        faceLuminance = block.getLuminance(_blockAccess, x - 1, y, z);
        if (bounds.MinX > 0.0D) faceLuminance = blockLuminance;
        if (isLightEmitter) faceLuminance = 1.0F;

        tess.setColorOpaque_F(lightX * faceLuminance, lightX * faceLuminance, lightX * faceLuminance);
        textureId = block.getTextureId(_blockAccess, x, y, z, 4);
        if (textureId < 0)
        {
            flipTexture = true;
            textureId = -textureId;
        }

        Helper.RenderNorthFace(block, x, y, z, textureId);
        flipTexture = false;

        // --- South Face (X + 1) ---
        faceLuminance = block.getLuminance(_blockAccess, x + 1, y, z);
        if (bounds.MaxX < 1.0D) faceLuminance = blockLuminance;
        if (isLightEmitter) faceLuminance = 1.0F;

        tess.setColorOpaque_F(lightX * faceLuminance, lightX * faceLuminance, lightX * faceLuminance);
        textureId = block.getTextureId(_blockAccess, x, y, z, 5);
        if (textureId < 0)
        {
            flipTexture = true;
            textureId = -textureId;
        }

        RenderSouthFace(block, x, y, z, textureId);
        flipTexture = false;

        return true;
    }
}
