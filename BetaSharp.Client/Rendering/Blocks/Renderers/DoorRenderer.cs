using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class DoorRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess, in BlockRenderContext context)
    {
        Box bounds = context.OverrideBounds ?? block.BoundingBox;

        var flatCtx = context with { EnableAo = false };

        float lightBottom = 0.5F;
        float lightTop = 1.0F;
        float lightZ = 0.8F; // East/West
        float lightX = 0.6F; // North/South

        float blockLuminance = block.getLuminance(world, pos.x, pos.y, pos.z);
        bool isLightEmitter = Block.BlocksLightLuminance[block.id] > 0;

        // Dummy colors since Door uses flat shading (tess.setColorOpaque_F) instead of AO
        FaceColors dummyColors = new FaceColors();

        // If your Helper specifically requires Vec3D instead of BlockPos, use this:
        Vec3D vecPos = new Vec3D(pos.x, pos.y, pos.z);

        // --- Bottom Face (Y - 1) ---
        float faceLuminance = block.getLuminance(world, pos.x, pos.y - 1, pos.z);
        if (bounds.MinY > 0.0D) faceLuminance = blockLuminance;
        if (isLightEmitter) faceLuminance = 1.0F;

        tess.setColorOpaque_F(lightBottom * faceLuminance, lightBottom * faceLuminance, lightBottom * faceLuminance);
        Helper.RenderBottomFace(block, vecPos, tess, flatCtx, dummyColors, block.getTextureId(world, pos.x, pos.y, pos.z, 0), false);

        // --- Top Face (Y + 1) ---
        faceLuminance = block.getLuminance(world, pos.x, pos.y + 1, pos.z);
        if (bounds.MaxY < 1.0D) faceLuminance = blockLuminance;
        if (isLightEmitter) faceLuminance = 1.0F;

        tess.setColorOpaque_F(lightTop * faceLuminance, lightTop * faceLuminance, lightTop * faceLuminance);
        Helper.RenderTopFace(block, vecPos, tess, flatCtx, dummyColors, block.getTextureId(world, pos.x, pos.y, pos.z, 1), false);

        // --- East Face (Z - 1) ---
        faceLuminance = block.getLuminance(world, pos.x, pos.y, pos.z - 1);
        if (bounds.MinZ > 0.0D) faceLuminance = blockLuminance;
        if (isLightEmitter) faceLuminance = 1.0F;

        tess.setColorOpaque_F(lightZ * faceLuminance, lightZ * faceLuminance, lightZ * faceLuminance);
        int textureId = block.getTextureId(world, pos.x, pos.y, pos.z, 2);

        bool flip = false;
        if (textureId < 0)
        {
            flip = true;
            textureId = -textureId; // Make it positive for the UV math
        }
        Helper.RenderEastFace(block, vecPos, tess, flatCtx, dummyColors, textureId, flip);

        // --- West Face (Z + 1) ---
        faceLuminance = block.getLuminance(world, pos.x, pos.y, pos.z + 1);
        if (bounds.MaxZ < 1.0D) faceLuminance = blockLuminance;
        if (isLightEmitter) faceLuminance = 1.0F;

        tess.setColorOpaque_F(lightZ * faceLuminance, lightZ * faceLuminance, lightZ * faceLuminance);
        textureId = block.getTextureId(world, pos.x, pos.y, pos.z, 3);

        flip = false;
        if (textureId < 0)
        {
            flip = true;
            textureId = -textureId;
        }
        Helper.RenderWestFace(block, vecPos, tess, flatCtx, dummyColors, textureId, flip);

        // --- North Face (X - 1) ---
        faceLuminance = block.getLuminance(world, pos.x - 1, pos.y, pos.z);
        if (bounds.MinX > 0.0D) faceLuminance = blockLuminance;
        if (isLightEmitter) faceLuminance = 1.0F;

        tess.setColorOpaque_F(lightX * faceLuminance, lightX * faceLuminance, lightX * faceLuminance);
        textureId = block.getTextureId(world, pos.x, pos.y, pos.z, 4);

        flip = false;
        if (textureId < 0)
        {
            flip = true;
            textureId = -textureId;
        }
        Helper.RenderNorthFace(block, vecPos, tess, flatCtx, dummyColors, textureId, flip);

        // --- South Face (X + 1) ---
        faceLuminance = block.getLuminance(world, pos.x + 1, pos.y, pos.z);
        if (bounds.MaxX < 1.0D) faceLuminance = blockLuminance;
        if (isLightEmitter) faceLuminance = 1.0F;

        tess.setColorOpaque_F(lightX * faceLuminance, lightX * faceLuminance, lightX * faceLuminance);
        textureId = block.getTextureId(world, pos.x, pos.y, pos.z, 5);

        flip = false;
        if (textureId < 0)
        {
            flip = true;
            textureId = -textureId;
        }
        Helper.RenderSouthFace(block, vecPos, tess, flatCtx, dummyColors, textureId, flip);

        return true;
    }
}
