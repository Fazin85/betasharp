using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Blocks.Renderers;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks;

public class BlockRenderer
{
    private static readonly StandardBlockRenderer _standard = new();
    private static readonly ReedRenderer _reed = new();
    private static readonly TorchRenderer _torch = new();
    private static readonly FireRenderer _fire = new();
    private static readonly FluidsRenderer _fluids = new();
    private static readonly RedstoneWireRenderer _wire = new();
    private static readonly CropsRenderer _crops = new();
    private static readonly DoorRenderer _door = new();
    private static readonly LadderRenderer _ladder = new();
    private static readonly MinecartTrackRenderer _track = new();
    private static readonly StairsRenderer _stairs = new();
    private static readonly FenceRenderer _fence = new();
    private static readonly LeverRenderer _lever = new();
    private static readonly CactusRenderer _cactus = new();
    private static readonly BedRenderer _bed = new();
    private static readonly RepeaterRenderer _repeater = new();
    private static readonly PistonBaseRenderer _pistonBase = new();
    private static readonly PistonExtensionRenderer _pistonExt = new();


    public bool RenderBlockByRenderType(IBlockAccess world, Block block, BlockPos pos, Tessellator tess,
        int overrideTexture = -1, bool renderAllFaces = false)
    {
        RendererType type = (RendererType)block.getRenderType();

        block.updateBoundingBox(world, pos.x, pos.y, pos.z);

        var ctx = new BlockRenderContext(
            overrideTexture: overrideTexture,
            renderAllFaces: renderAllFaces,
            flipTexture: false,
            bounds: block.BoundingBox,
            uvTop: 0,
            uvBottom: 0,
            uvNorth: 0,
            uvSouth: 0,
            uvEast: 0,
            uvWest: 0,
            aoBlendMode: 1,
            customFlag: type == RendererType.PistonExtension
        );

        return type switch
        {
            RendererType.StandardBlock => _standard.Render(world, block, pos, tess, ctx),
            RendererType.Reed => _reed.Render(world, block, pos, tess, ctx),
            RendererType.Torch => _torch.Render(world, block, pos, tess, ctx),
            RendererType.Fire => _fire.Render(world, block, pos, tess, ctx),
            RendererType.Fluids => _fluids.Render(world, block, pos, tess, ctx),
            RendererType.RedstoneWire => _wire.Render(world, block, pos, tess, ctx),
            RendererType.Crops => _crops.Render(world, block, pos, tess, ctx),
            RendererType.Door => _door.Render(world, block, pos, tess, ctx),
            RendererType.Ladder => _ladder.Render(world, block, pos, tess, ctx),
            RendererType.MinecartTrack => _track.Render(world, block, pos, tess, ctx),
            RendererType.Stairs => _stairs.Render(world, block, pos, tess, ctx),
            RendererType.Fence => _fence.Render(world, block, pos, tess, ctx),
            RendererType.Lever => _lever.Render(world, block, pos, tess, ctx),
            RendererType.Cactus => _cactus.Render(world, block, pos, tess, ctx),
            RendererType.Bed => _bed.Render(world, block, pos, tess, ctx),
            RendererType.Repeater => _repeater.Render(world, block, pos, tess, ctx),
            RendererType.PistonBase => _pistonBase.Render(world, block, pos, tess, ctx),
            RendererType.PistonExtension => _pistonExt.Render(world, block, pos, tess, ctx),
            _ => false
        };
    }
}
//public void RenderBlockFallingSand(Block block, World world, int x, int y, int z)
//{
//    // Directional shading multipliers for fake 3D depth
//    float lightBottom = 0.5F;
//    float lightTop = 1.0F;
//    float lightZ = 0.8F; // East/West faces
//    float lightX = 0.6F; // North/South faces
//
//    Tessellator tess = _tess;
//    tess.startDrawingQuads();
//
//    // Base luminance at the entity's current position
//    float currentLuminance = block.getLuminance(world, x, y, z);
//
//    // --- Bottom Face (Y - 1) ---
//    float faceLuminance = block.getLuminance(world, x, y - 1, z);
//    // Ensure the face isn't darker than the air block it occupies
//    if (faceLuminance < currentLuminance) faceLuminance = currentLuminance;
//
//    tess.setColorOpaque_F(lightBottom * faceLuminance, lightBottom * faceLuminance, lightBottom * faceLuminance);
//    // Note: Rendered at local origin (-0.5) because the entity's global transform handles the actual world position
//    Helper.RenderBottomFace(block, -0.5D, -0.5D, -0.5D, block.getTexture(0));
//
//    // --- Top Face (Y + 1) ---
//    faceLuminance = block.getLuminance(world, x, y + 1, z);
//    if (faceLuminance < currentLuminance) faceLuminance = currentLuminance;
//
//    tess.setColorOpaque_F(lightTop * faceLuminance, lightTop * faceLuminance, lightTop * faceLuminance);
//    Helper.RenderTopFace(block, -0.5D, -0.5D, -0.5D, block.getTexture(1));
//
//    // --- East Face (Z - 1) ---
//    faceLuminance = block.getLuminance(world, x, y, z - 1);
//    if (faceLuminance < currentLuminance) faceLuminance = currentLuminance;
//
//    tess.setColorOpaque_F(lightZ * faceLuminance, lightZ * faceLuminance, lightZ * faceLuminance);
//    Helper.RenderEastFace(block, -0.5D, -0.5D, -0.5D, block.getTexture(2));
//
//    // --- West Face (Z + 1) ---
//    faceLuminance = block.getLuminance(world, x, y, z + 1);
//    if (faceLuminance < currentLuminance) faceLuminance = currentLuminance;
//
//    tess.setColorOpaque_F(lightZ * faceLuminance, lightZ * faceLuminance, lightZ * faceLuminance);
//    Helper.RenderWestFace(block, -0.5D, -0.5D, -0.5D, block.getTexture(3));
//
//    // --- North Face (X - 1) ---
//    faceLuminance = block.getLuminance(world, x - 1, y, z);
//    if (faceLuminance < currentLuminance) faceLuminance = currentLuminance;
//
//    tess.setColorOpaque_F(lightX * faceLuminance, lightX * faceLuminance, lightX * faceLuminance);
//    Helper.RenderNorthFace(block, -0.5D, -0.5D, -0.5D, block.getTexture(4));
//
//    // --- South Face (X + 1) ---
//    faceLuminance = block.getLuminance(world, x + 1, y, z);
//    if (faceLuminance < currentLuminance) faceLuminance = currentLuminance;
//
//    tess.setColorOpaque_F(lightX * faceLuminance, lightX * faceLuminance, lightX * faceLuminance);
//    RenderSouthFace(block, -0.5D, -0.5D, -0.5D, block.getTexture(5));
//
//    tess.draw();
//}

//public void RenderBlockOnInventory(Block block, int metadata, float brightness)
//{
//    Tessellator tess = _tess;
//    int renderType = block.getRenderType();
//
//    if (RenderFromInside)
//    {
//        int color = block.getColor(metadata);
//        float red = (color >> 16 & 255) / 255.0F;
//        float green = (color >> 8 & 255) / 255.0F;
//        float blue = (color & 255) / 255.0F;
//
//        // Apply color and brightness to the global GL state
//        GLManager.GL.Color4(red * brightness, green * brightness, blue * brightness, 1.0F);
//    }
//
//    // Standard blocks (0) and Piston Bases (16) use standard 6-face cube rendering
//    if (renderType == 0 || renderType == 16)
//    {
//        if (renderType == 16) metadata = 1; // Force standard texture for piston items
//
//        block.setupRenderBoundingBox();
//        GLManager.GL.Translate(-0.5F, -0.5F, -0.5F);
//
//        tess.startDrawingQuads();
//        tess.setNormal(0.0F, -1.0F, 0.0F);
//        Helper.RenderBottomFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(0, metadata));
//        tess.draw();
//
//        tess.startDrawingQuads();
//        tess.setNormal(0.0F, 1.0F, 0.0F);
//        Helper.RenderTopFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(1, metadata));
//        tess.draw();
//
//        tess.startDrawingQuads();
//        tess.setNormal(0.0F, 0.0F, -1.0F);
//        Helper.RenderEastFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(2, metadata));
//        tess.draw();
//
//        tess.startDrawingQuads();
//        tess.setNormal(0.0F, 0.0F, 1.0F);
//        Helper.RenderWestFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(3, metadata));
//        tess.draw();
//
//        tess.startDrawingQuads();
//        tess.setNormal(-1.0F, 0.0F, 0.0F);
//        Helper.RenderNorthFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(4, metadata));
//        tess.draw();
//
//        tess.startDrawingQuads();
//        tess.setNormal(1.0F, 0.0F, 0.0F);
//        RenderSouthFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(5, metadata));
//        tess.draw();
//
//        GLManager.GL.Translate(0.5F, 0.5F, 0.5F);
//    }
//    else if (renderType == 1) // Crossed squares (Flowers, Saplings)
//    {
//        tess.startDrawingQuads();
//        tess.setNormal(0.0F, -1.0F, 0.0F);
//        RenderCrossedSquares(block, metadata, -0.5D, -0.5D, -0.5D);
//        tess.draw();
//    }
//    else if (renderType == 13) // Cactus (slightly inset faces)
//    {
//        block.setupRenderBoundingBox();
//        GLManager.GL.Translate(-0.5F, -0.5F, -0.5F);
//        float inset = 1.0F / 16.0F;
//
//        tess.startDrawingQuads();
//        tess.setNormal(0.0F, -1.0F, 0.0F);
//        Helper.RenderBottomFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(0));
//        tess.draw();
//
//        tess.startDrawingQuads();
//        tess.setNormal(0.0F, 1.0F, 0.0F);
//        Helper.RenderTopFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(1));
//        tess.draw();
//
//        tess.startDrawingQuads();
//        tess.setNormal(0.0F, 0.0F, -1.0F);
//        tess.setTranslationF(0.0F, 0.0F, inset);
//        Helper.RenderEastFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(2));
//        tess.setTranslationF(0.0F, 0.0F, -inset);
//        tess.draw();
//
//        tess.startDrawingQuads();
//        tess.setNormal(0.0F, 0.0F, 1.0F);
//        tess.setTranslationF(0.0F, 0.0F, -inset);
//        Helper.RenderWestFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(3));
//        tess.setTranslationF(0.0F, 0.0F, inset);
//        tess.draw();
//
//        tess.startDrawingQuads();
//        tess.setNormal(-1.0F, 0.0F, 0.0F);
//        tess.setTranslationF(inset, 0.0F, 0.0F);
//        Helper.RenderNorthFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(4));
//        tess.setTranslationF(-inset, 0.0F, 0.0F);
//        tess.draw();
//
//        tess.startDrawingQuads();
//        tess.setNormal(1.0F, 0.0F, 0.0F);
//        tess.setTranslationF(-inset, 0.0F, 0.0F);
//        RenderSouthFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(5));
//        tess.setTranslationF(inset, 0.0F, 0.0F);
//        tess.draw();
//
//        GLManager.GL.Translate(0.5F, 0.5F, 0.5F);
//    }
//    else if (renderType == 6) // Crops (Wheat/Seeds)
//    {
//        tess.startDrawingQuads();
//        tess.setNormal(0.0F, -1.0F, 0.0F);
//        RenderCropQuads(block, metadata, -0.5D, -0.5D, -0.5D);
//        tess.draw();
//    }
//    else if (renderType == 2) // Torch
//    {
//        tess.startDrawingQuads();
//        tess.setNormal(0.0F, -1.0F, 0.0F);
//        RenderTorchAtAngle(block, -0.5D, -0.5D, -0.5D, 0.0D, 0.0D);
//        tess.draw();
//    }
//    else if (renderType == 10) // Stairs
//    {
//        for (int i = 0; i < 2; ++i)
//        {
//            if (i == 0) block.setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.5F);
//            if (i == 1) block.setBoundingBox(0.0F, 0.0F, 0.5F, 1.0F, 0.5F, 1.0F);
//
//            GLManager.GL.Translate(-0.5F, -0.5F, -0.5F);
//            RenderCubeItem(block, tess);
//            GLManager.GL.Translate(0.5F, 0.5F, 0.5F);
//        }
//    }
//    else if (renderType == 11) // Fence
//    {
//        for (int i = 0; i < 4; ++i)
//        {
//            float size = 2.0F / 16.0F;
//            if (i == 0) block.setBoundingBox(0.5F - size, 0.0F, 0.0F, 0.5F + size, 1.0F, size * 2.0F);
//            if (i == 1) block.setBoundingBox(0.5F - size, 0.0F, 1.0F - size * 2.0F, 0.5F + size, 1.0F, 1.0F);
//
//            size = 1.0F / 16.0F;
//            if (i == 2)
//                block.setBoundingBox(0.5F - size, 1.0F - size * 3.0F, -size * 2.0F, 0.5F + size, 1.0F - size,
//                    1.0F + size * 2.0F);
//            if (i == 3)
//                block.setBoundingBox(0.5F - size, 0.5F - size * 3.0F, -size * 2.0F, 0.5F + size, 0.5F - size,
//                    1.0F + size * 2.0F);
//
//            GLManager.GL.Translate(-0.5F, -0.5F, -0.5F);
//            RenderCubeItem(block, tess);
//            GLManager.GL.Translate(0.5F, 0.5F, 0.5F);
//        }
//
//        block.setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
//    }
//}

//private void RenderCubeItem(Block block, Tessellator tess)
//{
//    tess.startDrawingQuads();
//    tess.setNormal(0.0F, -1.0F, 0.0F);
//    Helper.RenderBottomFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(0));
//    tess.draw();
//    tess.startDrawingQuads();
//    tess.setNormal(0.0F, 1.0F, 0.0F);
//    Helper.RenderTopFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(1));
//    tess.draw();
//    tess.startDrawingQuads();
//    tess.setNormal(0.0F, 0.0F, -1.0F);
//    Helper.RenderEastFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(2));
//    tess.draw();
//    tess.startDrawingQuads();
//    tess.setNormal(0.0F, 0.0F, 1.0F);
//    Helper.RenderWestFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(3));
//    tess.draw();
//    tess.startDrawingQuads();
//    tess.setNormal(-1.0F, 0.0F, 0.0F);
//    Helper.RenderNorthFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(4));
//    tess.draw();
//    tess.startDrawingQuads();
//    tess.setNormal(1.0F, 0.0F, 0.0F);
//    RenderSouthFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(5));
//    tess.draw();
//}

//public static bool IsSideLit(int renderType)
//{
//    return renderType == 0 || // Standard
//           renderType == 10 || // Stairs
//           renderType == 11 || // Fence
//           renderType == 13 || // Cactus
//           renderType == 16; // Piston Base
//}
