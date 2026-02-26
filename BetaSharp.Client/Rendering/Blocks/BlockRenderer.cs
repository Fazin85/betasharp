using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Blocks.Renderers;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using java.lang;
using Math = System.Math;

namespace BetaSharp.Client.Rendering.Blocks;

public class BlockRenderer
{
    private static readonly ReedRenderer s_reed = new();
    private static readonly TorchRenderer s_torch = new();
    private static readonly FireRenderer s_fire = new();
    private static readonly FluidsRenderer s_fluids = new();
    private static readonly RedstoneWireRenderer s_wire = new();
    private static readonly CropsRenderer s_crops = new();
    private static readonly DoorRenderer s_door = new();
    private static readonly LadderRenderer s_ladder = new();
    private static readonly MinecartTrackRenderer s_track = new();
    private static readonly StairsRenderer s_stairs = new();
    private static readonly FenceRenderer s_fence = new();
    private static readonly LeverRenderer s_lever = new();
    private static readonly CactusRenderer s_cactus = new();
    private static readonly BedRenderer s_bed = new();
    private static readonly RepeaterRenderer s_repeater = new();
    private static readonly PistonBaseRenderer s_pistonBase = new();
    private static readonly PistonExtensionRenderer s_pistonExt = new();


    public bool RenderBlockByRenderType(IBlockAccess world, Block block, BlockPos pos, Tessellator tess,
        int overrideTexture = -1, bool renderAllFaces = false)
    {
        RendererType type = (RendererType)block.getRenderType();

        block.updateBoundingBox(world, pos.x, pos.y, pos.z);

        var ctx = new BlockRenderContext(
            tess: tess,
            world: world,
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
            RendererType.StandardBlock => ctx.DrawBlock(block, pos),
            RendererType.Reed => s_reed.Render(block, pos, ctx),
            RendererType.Torch => s_torch.Render(block, pos, ctx),
            RendererType.Fire => s_fire.Render(block, pos, ctx),
            RendererType.Fluids => s_fluids.Render(block, pos, ctx),
            RendererType.RedstoneWire => s_wire.Render(block, pos, ctx),
            RendererType.Crops => s_crops.Render(block, pos, ctx),
            RendererType.Door => s_door.Render(block, pos, ctx),
            RendererType.Ladder => s_ladder.Render(block, pos, ctx),
            RendererType.MinecartTrack => s_track.Render(block, pos, ctx),
            RendererType.Stairs => s_stairs.Render(block, pos, ctx),
            RendererType.Fence => s_fence.Render(block, pos, ctx),
            RendererType.Lever => s_lever.Render(block, pos, ctx),
            RendererType.Cactus => s_cactus.Render(block, pos, ctx),
            RendererType.Bed => s_bed.Render(block, pos, ctx),
            RendererType.Repeater => s_repeater.Render(block, pos, ctx),
            RendererType.PistonBase => s_pistonBase.Render(block, pos, ctx),
            RendererType.PistonExtension => s_pistonExt.Render(block, pos, ctx),
            _ => false
        };
    }


    // TODO: THIS SHOULD BE PART OF THE UI
    [Deprecated(["THIS SHOULD BE PART OF THE UI"])]
    public void RenderBlockOnInventory(Block block, int metadata, float brightness, Tessellator tess)
    {
        int renderType = block.getRenderType();

        int color = block.getColor(metadata);
        float red = (color >> 16 & 255) / 255.0F * brightness;
        float green = (color >> 8 & 255) / 255.0F * brightness;
        float blue = (color & 255) / 255.0F * brightness;
        GLManager.GL.Color4(red, green, blue, 1.0F);

        var uiCtx = new BlockRenderContext(
            world: null,
            tess: tess,
            renderAllFaces: true,
            enableAo: false,
            overrideTexture: (renderType == 16) ? 1 : -1
        );

        Vec3D origin = new Vec3D(0, 0, 0);
        FaceColors dummyColors = new FaceColors();

        if (IsSideLit(renderType))
        {
            block.setupRenderBoundingBox();
            GLManager.GL.Translate(-0.5F, -0.5F, -0.5F);

            // We manually draw the cube faces here to set GL Normals for the UI lighting
            tess.startDrawingQuads();
            tess.setNormal(0.0F, -1.0F, 0.0F);
            uiCtx.DrawBottomFace(block, origin, dummyColors, block.getTexture(0, metadata));
            tess.draw();

            tess.startDrawingQuads();
            tess.setNormal(0.0F, 1.0F, 0.0F);
            uiCtx.DrawTopFace(block, origin, dummyColors, block.getTexture(1, metadata));
            tess.draw();

            tess.startDrawingQuads();
            tess.setNormal(0.0F, 0.0F, -1.0F);
            uiCtx.DrawEastFace(block, origin, dummyColors, block.getTexture(2, metadata));
            tess.draw();

            tess.startDrawingQuads();
            tess.setNormal(0.0F, 0.0F, 1.0F);
            uiCtx.DrawWestFace(block, origin, dummyColors, block.getTexture(3, metadata));
            tess.draw();

            tess.startDrawingQuads();
            tess.setNormal(-1.0F, 0.0F, 0.0F);
            uiCtx.DrawNorthFace(block, origin, dummyColors, block.getTexture(4, metadata));
            tess.draw();

            tess.startDrawingQuads();
            tess.setNormal(1.0F, 0.0F, 0.0F);
            uiCtx.DrawSouthFace(block, origin, dummyColors, block.getTexture(5, metadata));
            tess.draw();

            GLManager.GL.Translate(0.5F, 0.5F, 0.5F);
        }
        else
        {
            // For non-cube blocks (Torches, Flowers), we use their specialized renderers
            BlockPos itemPos = new BlockPos(0, 0, 0);
            RenderBlockByRenderType(null, block, itemPos, tess, uiCtx.OverrideTexture, true);
        }
    }

    public void RenderBlockFallingSand(Block block, World world, int x, int y, int z, Tessellator tess)
    {
        // Directional shading multipliers for fake 3D depth
        float lightBottom = 0.5F;
        float lightTop = 1.0F;
        float lightZ = 0.8F; // East/West faces
        float lightX = 0.6F; // North/South faces

        // Create a specialized Entity Context: No AO, Forced All Faces
        var entityCtx = new BlockRenderContext(
            world: world,
            tess: tess,
            renderAllFaces: true,
            enableAo: false
        );

        tess.startDrawingQuads();

        // Base luminance at the entity's current position
        float currentLuminance = block.getLuminance(world, x, y, z);
        Vec3D localOrigin = new Vec3D(-0.5, -0.5, -0.5);
        FaceColors dummyColors = new FaceColors();

        // --- Bottom Face ---
        float faceLum = Math.Max(currentLuminance, block.getLuminance(world, x, y - 1, z));
        tess.setColorOpaque_F(lightBottom * faceLum, lightBottom * faceLum, lightBottom * faceLum);
        entityCtx.DrawBottomFace(block, localOrigin, dummyColors, block.getTexture(0));

        // --- Top Face ---
        faceLum = Math.Max(currentLuminance, block.getLuminance(world, x, y + 1, z));
        tess.setColorOpaque_F(lightTop * faceLum, lightTop * faceLum, lightTop * faceLum);
        entityCtx.DrawTopFace(block, localOrigin, dummyColors, block.getTexture(1));

        // --- East/West Faces ---
        faceLum = Math.Max(currentLuminance, block.getLuminance(world, x, y, z - 1));
        tess.setColorOpaque_F(lightZ * faceLum, lightZ * faceLum, lightZ * faceLum);
        entityCtx.DrawEastFace(block, localOrigin, dummyColors, block.getTexture(2));

        faceLum = Math.Max(currentLuminance, block.getLuminance(world, x, y, z + 1));
        tess.setColorOpaque_F(lightZ * faceLum, lightZ * faceLum, lightZ * faceLum);
        entityCtx.DrawWestFace(block, localOrigin, dummyColors, block.getTexture(3));

        // --- North/South Faces ---
        faceLum = Math.Max(currentLuminance, block.getLuminance(world, x - 1, y, z));
        tess.setColorOpaque_F(lightX * faceLum, lightX * faceLum, lightX * faceLum);
        entityCtx.DrawNorthFace(block, localOrigin, dummyColors, block.getTexture(4));

        faceLum = Math.Max(currentLuminance, block.getLuminance(world, x + 1, y, z));
        tess.setColorOpaque_F(lightX * faceLum, lightX * faceLum, lightX * faceLum);
        entityCtx.DrawSouthFace(block, localOrigin, dummyColors, block.getTexture(5));

        tess.draw();
    }

    public static bool IsSideLit(int renderType)
    {
        return renderType == 0 || // Standard
               renderType == 10 || // Stairs
               renderType == 11 || // Fence
               renderType == 13 || // Cactus
               renderType == 16; // Piston Base
    }
}
