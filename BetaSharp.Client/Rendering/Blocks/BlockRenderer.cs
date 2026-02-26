using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;
using BetaSharp.Client.Rendering.Blocks.Renderers;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using Silk.NET.Maths;

namespace BetaSharp.Client.Rendering.Blocks;

public class BlockRenderer
{
    private readonly IBlockAccess _blockAccess;
    private readonly Tessellator _tess;
    private readonly RendererRegistry _rendererRegistry = new();


    public BlockRenderer(IBlockAccess iBlockAccess, Tessellator? tess)
    {
        _blockAccess = iBlockAccess;
        _tess = tess ?? Tessellator.instance;

        _rendererRegistry[RendererType.StandardBlock] = new StandardBlockRenderer();
        _rendererRegistry[RendererType.Reed] = new ReedRenderer();
        _rendererRegistry[RendererType.Torch] = new TorchRenderer();
        _rendererRegistry[RendererType.Fire] = new FireRenderer();
        _rendererRegistry[RendererType.Fluids] = new FluidsRenderer();
        _rendererRegistry[RendererType.RedstoneWire] = new RedstoneWireRenderer();
        _rendererRegistry[RendererType.Crops] = new CropsRenderer();
        _rendererRegistry[RendererType.Door] = new DoorRenderer();
        _rendererRegistry[RendererType.Ladder] = new LadderRenderer();
        _rendererRegistry[RendererType.MinecartTrack] = new MinecartTrackRenderer();
        _rendererRegistry[RendererType.Stairs] = new StairsRenderer();
        _rendererRegistry[RendererType.Fence] = new FenceRenderer();
        _rendererRegistry[RendererType.Lever] = new LeverRenderer();
        _rendererRegistry[RendererType.Cactus] = new CactusRenderer();
        _rendererRegistry[RendererType.Bed] = new BedRenderer();
        _rendererRegistry[RendererType.Repeater] = new RepeaterRenderer();
        _rendererRegistry[RendererType.PistonBase] = new PistonBaseRenderer();
        _rendererRegistry[RendererType.PistonExtension] = new PistonExtensionRenderer();
    }

    public bool RenderBlockByRenderType(IBlockAccess world, Block block, BlockPos pos, Tessellator tess, int overrideTexture = -1, bool renderAllFaces = false)
    {
        RendererType type = (RendererType)block.getRenderType();

        block.updateBoundingBox(_blockAccess, pos.x, pos.y, pos.z);

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
            customFlag: type == RendererType.PistonExtension
        );

        try
        {
            IBlockRenderer renderer = _rendererRegistry[type];
            return renderer?.Render(world, block, pos, tess, ctx) ?? false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public void RenderPistonBaseAllFaces(Block block, int x, int y, int z)
    {
        _renderAllFaces = true;
        RenderPistonBase(block, x, y, z, true);
        _renderAllFaces = false;
    }

    private bool RenderPistonBase(Block block, int x, int y, int z, bool expanded)
    {
        int metadata = _blockAccess.getBlockMeta(x, y, z);
        bool isExpanded = expanded || (metadata & 8) != 0;
        int facing = BlockPistonBase.getFacing(metadata);

        if (isExpanded)
        {
            // If the piston is expanded, we shrink the base block's bounding box
            // to make room for the piston head/arm.
            switch (facing)
            {
                case 0: // Down
                    _uvRotateEast = 3;
                    _uvRotateWest = 3;
                    _uvRotateSouth = 3;
                    _uvRotateNorth = 3;
                    SetOverrideBoundingBox(0.0F, 0.25F, 0.0F, 1.0F, 1.0F, 1.0F);
                    break;
                case 1: // Up
                    SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 0.75F, 1.0F);
                    break;
                case 2: // North
                    _uvRotateSouth = 1;
                    _uvRotateNorth = 2;
                    SetOverrideBoundingBox(0.0F, 0.0F, 0.25F, 1.0F, 1.0F, 1.0F);
                    break;
                case 3: // South
                    _uvRotateSouth = 2;
                    _uvRotateNorth = 1;
                    _uvRotateTop = 3;
                    _uvRotateBottom = 3;
                    SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.75F);
                    break;
                case 4: // West
                    _uvRotateEast = 1;
                    _uvRotateWest = 2;
                    _uvRotateTop = 2;
                    _uvRotateBottom = 1;
                    SetOverrideBoundingBox(0.25F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                    break;
                case 5: // East
                    _uvRotateEast = 2;
                    _uvRotateWest = 1;
                    _uvRotateTop = 1;
                    _uvRotateBottom = 2;
                    SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 0.75F, 1.0F, 1.0F);
                    break;
            }

            RenderStandardBlock(block, x, y, z);

            // Reset rotations and box
            ResetUvRotation();
            SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
        }
        else
        {
            // Piston is retracted (full block), but we still apply UV rotation
            // so the "face" of the piston points in the correct direction.
            switch (facing)
            {
                case 0: // Down
                    _uvRotateEast = 3;
                    _uvRotateWest = 3;
                    _uvRotateSouth = 3;
                    _uvRotateNorth = 3;
                    break;
                case 2: // North
                    _uvRotateSouth = 1;
                    _uvRotateNorth = 2;
                    break;
                case 3: // South
                    _uvRotateSouth = 2;
                    _uvRotateNorth = 1;
                    _uvRotateTop = 3;
                    _uvRotateBottom = 3;
                    break;
                case 4: // West
                    _uvRotateEast = 1;
                    _uvRotateWest = 2;
                    _uvRotateTop = 2;
                    _uvRotateBottom = 1;
                    break;
                case 5: // East
                    _uvRotateEast = 2;
                    _uvRotateWest = 1;
                    _uvRotateTop = 1;
                    _uvRotateBottom = 2;
                    break;
            }

            RenderStandardBlock(block, x, y, z);
            ResetUvRotation();
        }

        return true;
    }

    private void ResetUvRotation()
    {
        _uvRotateEast = 0;
        _uvRotateWest = 0;
        _uvRotateSouth = 0;
        _uvRotateNorth = 0;
        _uvRotateTop = 0;
        _uvRotateBottom = 0;
    }

    private void RenderPistonArmY(double x1, double x2, double y1, double y2, double z1, double z2, float luminance,
        double textureWidth)
    {
        Tessellator tess = _tess;
        int textureId = 108; // Piston arm texture
        if (_overrideBlockTexture >= 0) textureId = _overrideBlockTexture;

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        double minU = texU / 256.0D;
        double minV = texV / 256.0D;
        double maxU = (texU + textureWidth - 0.01D) / 256.0D;
        double maxV = (texV + 4.0D - 0.01D) / 256.0D;

        tess.setColorOpaque_F(luminance, luminance, luminance);
        tess.addVertexWithUV(x1, y2, z1, maxU, minV);
        tess.addVertexWithUV(x1, y1, z1, minU, minV);
        tess.addVertexWithUV(x2, y1, z2, minU, maxV);
        tess.addVertexWithUV(x2, y2, z2, maxU, maxV);
    }

    private void RenderPistonArmZ(double x1, double x2, double y1, double y2, double z1, double z2, float luminance,
        double textureWidth)
    {
        Tessellator tess = _tess;
        int textureId = 108;
        if (_overrideBlockTexture >= 0) textureId = _overrideBlockTexture;

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        double minU = texU / 256.0D;
        double minV = texV / 256.0D;
        double maxU = (texU + textureWidth - 0.01D) / 256.0D;
        double maxV = (texV + 4.0D - 0.01D) / 256.0D;

        tess.setColorOpaque_F(luminance, luminance, luminance);
        tess.addVertexWithUV(x1, y1, z2, maxU, minV);
        tess.addVertexWithUV(x1, y1, z1, minU, minV);
        tess.addVertexWithUV(x2, y2, z1, minU, maxV);
        tess.addVertexWithUV(x2, y2, z2, maxU, maxV);
    }

    private void RenderPistonArmX(double x1, double x2, double y1, double y2, double z1, double z2, float luminance,
        double textureWidth)
    {
        Tessellator tess = _tess;
        int textureId = 108;
        if (_overrideBlockTexture >= 0) textureId = _overrideBlockTexture;

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        double minU = texU / 256.0D;
        double minV = texV / 256.0D;
        double maxU = (texU + textureWidth - 0.01D) / 256.0D;
        double maxV = (texV + 4.0D - 0.01D) / 256.0D;

        tess.setColorOpaque_F(luminance, luminance, luminance);
        tess.addVertexWithUV(x2, y1, z1, maxU, minV);
        tess.addVertexWithUV(x1, y1, z1, minU, minV);
        tess.addVertexWithUV(x1, y2, z2, minU, maxV);
        tess.addVertexWithUV(x2, y2, z2, maxU, maxV);
    }

    public void RenderPistonExtensionAllFaces(Block block, int x, int y, int z, bool isShortArm)
    {
        _renderAllFaces = true;
        RenderPistonExtension(block, x, y, z, isShortArm);
        _renderAllFaces = false;
    }

    private bool RenderPistonExtension(Block block, int x, int y, int z, bool isShortArm)
    {
        int metadata = _blockAccess.getBlockMeta(x, y, z);
        int facing = BlockPistonExtension.getFacing(metadata);
        float luminance = block.getLuminance(_blockAccess, x, y, z);

        // Arm length logic: 1.0 for full extension, 0.5 for partial
        float armLength = isShortArm ? 1.0F : 0.5F;
        double texWidth = isShortArm ? 16.0D : 8.0D;

        switch (facing)
        {
            case 0: // Down
                ApplyRotation(3, 3, 3, 3, 0, 0);
                SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 0.25F, 1.0F);
                RenderStandardBlock(block, x, y, z);
                RenderPistonArmY(x + 0.375, x + 0.625, y + 0.25, y + 0.25 + armLength, z + 0.625, z + 0.625,
                    luminance * 0.8F, texWidth);
                RenderPistonArmY(x + 0.625, x + 0.375, y + 0.25, y + 0.25 + armLength, z + 0.375, z + 0.375,
                    luminance * 0.8F, texWidth);
                RenderPistonArmY(x + 0.375, x + 0.375, y + 0.25, y + 0.25 + armLength, z + 0.375, z + 0.625,
                    luminance * 0.6F, texWidth);
                RenderPistonArmY(x + 0.625, x + 0.625, y + 0.25, y + 0.25 + armLength, z + 0.625, z + 0.375,
                    luminance * 0.6F, texWidth);
                break;

            case 1: // Up
                SetOverrideBoundingBox(0.0F, 0.75F, 0.0F, 1.0F, 1.0F, 1.0F);
                RenderStandardBlock(block, x, y, z);
                RenderPistonArmY(x + 0.375, x + 0.625, y + 0.75 - armLength, y + 0.75, z + 0.625, z + 0.625,
                    luminance * 0.8F, texWidth);
                RenderPistonArmY(x + 0.625, x + 0.375, y + 0.75 - armLength, y + 0.75, z + 0.375, z + 0.375,
                    luminance * 0.8F, texWidth);
                RenderPistonArmY(x + 0.375, x + 0.375, y + 0.75 - armLength, y + 0.75, z + 0.375, z + 0.625,
                    luminance * 0.6F, texWidth);
                RenderPistonArmY(x + 0.625, x + 0.625, y + 0.75 - armLength, y + 0.75, z + 0.625, z + 0.375,
                    luminance * 0.6F, texWidth);
                break;

            case 2: // North
                ApplyRotation(0, 0, 2, 1, 0, 0);
                SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.25F);
                RenderStandardBlock(block, x, y, z);
                RenderPistonArmZ(x + 0.375, x + 0.375, y + 0.625, y + 0.375, z + 0.25, z + 0.25 + armLength,
                    luminance * 0.6F, texWidth);
                RenderPistonArmZ(x + 0.625, x + 0.625, y + 0.375, y + 0.625, z + 0.25, z + 0.25 + armLength,
                    luminance * 0.6F, texWidth);
                RenderPistonArmZ(x + 0.375, x + 0.625, y + 0.375, y + 0.375, z + 0.25, z + 0.25 + armLength,
                    luminance * 0.5F, texWidth);
                RenderPistonArmZ(x + 0.625, x + 0.375, y + 0.625, y + 0.625, z + 0.25, z + 0.25 + armLength, luminance,
                    texWidth);
                break;

            case 3: // South
                ApplyRotation(3, 3, 1, 2, 0, 0);
                SetOverrideBoundingBox(0.0F, 0.0F, 0.75F, 1.0F, 1.0F, 1.0F);
                RenderStandardBlock(block, x, y, z);
                RenderPistonArmZ(x + 0.375, x + 0.375, y + 0.625, y + 0.375, z + 0.75 - armLength, z + 0.75,
                    luminance * 0.6F, texWidth);
                RenderPistonArmZ(x + 0.625, x + 0.625, y + 0.375, y + 0.625, z + 0.75 - armLength, z + 0.75,
                    luminance * 0.6F, texWidth);
                RenderPistonArmZ(x + 0.375, x + 0.625, y + 0.375, y + 0.375, z + 0.75 - armLength, z + 0.75,
                    luminance * 0.5F, texWidth);
                RenderPistonArmZ(x + 0.625, x + 0.375, y + 0.625, y + 0.625, z + 0.75 - armLength, z + 0.75, luminance,
                    texWidth);
                break;

            case 4: // West
                ApplyRotation(2, 1, 0, 0, 2, 1);
                SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 0.25F, 1.0F, 1.0F);
                RenderStandardBlock(block, x, y, z);
                RenderPistonArmX(x + 0.25, x + 0.25 + armLength, y + 0.375, y + 0.375, z + 0.625, z + 0.375,
                    luminance * 0.5F, texWidth);
                RenderPistonArmX(x + 0.25, x + 0.25 + armLength, y + 0.625, y + 0.625, z + 0.375, z + 0.625, luminance,
                    texWidth);
                RenderPistonArmX(x + 0.25, x + 0.25 + armLength, y + 0.375, y + 0.625, z + 0.375, z + 0.375,
                    luminance * 0.6F, texWidth);
                RenderPistonArmX(x + 0.25, x + 0.25 + armLength, y + 0.625, y + 0.375, z + 0.625, z + 0.625,
                    luminance * 0.6F, texWidth);
                break;

            case 5: // East
                ApplyRotation(1, 2, 0, 0, 1, 2);
                SetOverrideBoundingBox(0.75F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                RenderStandardBlock(block, x, y, z);
                RenderPistonArmX(x + 0.75 - armLength, x + 0.75, y + 0.375, y + 0.375, z + 0.625, z + 0.375,
                    luminance * 0.5F, texWidth);
                RenderPistonArmX(x + 0.75 - armLength, x + 0.75, y + 0.625, y + 0.625, z + 0.375, z + 0.625, luminance,
                    texWidth);
                RenderPistonArmX(x + 0.75 - armLength, x + 0.75, y + 0.375, y + 0.625, z + 0.375, z + 0.375,
                    luminance * 0.6F, texWidth);
                RenderPistonArmX(x + 0.75 - armLength, x + 0.75, y + 0.625, y + 0.375, z + 0.625, z + 0.625,
                    luminance * 0.6F, texWidth);
                break;
        }

        ResetUvRotation();
        SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
        return true;
    }

    private void ApplyRotation(int top, int bottom, int north, int south, int west, int east)
    {
        _uvRotateTop = top;
        _uvRotateBottom = bottom;
        _uvRotateNorth = north;
        _uvRotateSouth = south;
        _uvRotateWest = west;
        _uvRotateEast = east;
    }

    private void RenderCrossedSquares(Block block, int metadata, double x, double y, double z)
    {
        Tessellator tess = _tess;

        int textureId = block.getTexture(0, metadata);
        if (_overrideBlockTexture >= 0)
        {
            textureId = _overrideBlockTexture;
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

        // First Diagonal Plane (Bottom-Left to Top-Right across the X/Z grid)

        // Front side
        tess.addVertexWithUV(minX, y + 1.0D, minZ, minU, minV);
        tess.addVertexWithUV(minX, y + 0.0D, minZ, minU, maxV);
        tess.addVertexWithUV(maxX, y + 0.0D, maxZ, maxU, maxV);
        tess.addVertexWithUV(maxX, y + 1.0D, maxZ, maxU, minV);

        // Back side (reversed winding order and UVs)
        tess.addVertexWithUV(maxX, y + 1.0D, maxZ, minU, minV);
        tess.addVertexWithUV(maxX, y + 0.0D, maxZ, minU, maxV);
        tess.addVertexWithUV(minX, y + 0.0D, minZ, maxU, maxV);
        tess.addVertexWithUV(minX, y + 1.0D, minZ, maxU, minV);

        // Second Diagonal Plane (Top-Left to Bottom-Right across the X/Z grid)

        // Front side
        tess.addVertexWithUV(minX, y + 1.0D, maxZ, minU, minV);
        tess.addVertexWithUV(minX, y + 0.0D, maxZ, minU, maxV);
        tess.addVertexWithUV(maxX, y + 0.0D, minZ, maxU, maxV);
        tess.addVertexWithUV(maxX, y + 1.0D, minZ, maxU, minV);

        // Back side (reversed winding order and UVs)
        tess.addVertexWithUV(maxX, y + 1.0D, minZ, minU, minV);
        tess.addVertexWithUV(maxX, y + 0.0D, minZ, minU, maxV);
        tess.addVertexWithUV(minX, y + 0.0D, maxZ, maxU, maxV);
        tess.addVertexWithUV(minX, y + 1.0D, maxZ, maxU, minV);
    }


    public void RenderBlockFallingSand(Block block, World world, int x, int y, int z)
    {
        // Directional shading multipliers for fake 3D depth
        float lightBottom = 0.5F;
        float lightTop = 1.0F;
        float lightZ = 0.8F; // East/West faces
        float lightX = 0.6F; // North/South faces

        Tessellator tess = _tess;
        tess.startDrawingQuads();

        // Base luminance at the entity's current position
        float currentLuminance = block.getLuminance(world, x, y, z);

        // --- Bottom Face (Y - 1) ---
        float faceLuminance = block.getLuminance(world, x, y - 1, z);
        // Ensure the face isn't darker than the air block it occupies
        if (faceLuminance < currentLuminance) faceLuminance = currentLuminance;

        tess.setColorOpaque_F(lightBottom * faceLuminance, lightBottom * faceLuminance, lightBottom * faceLuminance);
        // Note: Rendered at local origin (-0.5) because the entity's global transform handles the actual world position
        Helper.RenderBottomFace(block, -0.5D, -0.5D, -0.5D, block.getTexture(0));

        // --- Top Face (Y + 1) ---
        faceLuminance = block.getLuminance(world, x, y + 1, z);
        if (faceLuminance < currentLuminance) faceLuminance = currentLuminance;

        tess.setColorOpaque_F(lightTop * faceLuminance, lightTop * faceLuminance, lightTop * faceLuminance);
        Helper.RenderTopFace(block, -0.5D, -0.5D, -0.5D, block.getTexture(1));

        // --- East Face (Z - 1) ---
        faceLuminance = block.getLuminance(world, x, y, z - 1);
        if (faceLuminance < currentLuminance) faceLuminance = currentLuminance;

        tess.setColorOpaque_F(lightZ * faceLuminance, lightZ * faceLuminance, lightZ * faceLuminance);
        Helper.RenderEastFace(block, -0.5D, -0.5D, -0.5D, block.getTexture(2));

        // --- West Face (Z + 1) ---
        faceLuminance = block.getLuminance(world, x, y, z + 1);
        if (faceLuminance < currentLuminance) faceLuminance = currentLuminance;

        tess.setColorOpaque_F(lightZ * faceLuminance, lightZ * faceLuminance, lightZ * faceLuminance);
        Helper.RenderWestFace(block, -0.5D, -0.5D, -0.5D, block.getTexture(3));

        // --- North Face (X - 1) ---
        faceLuminance = block.getLuminance(world, x - 1, y, z);
        if (faceLuminance < currentLuminance) faceLuminance = currentLuminance;

        tess.setColorOpaque_F(lightX * faceLuminance, lightX * faceLuminance, lightX * faceLuminance);
        Helper.RenderNorthFace(block, -0.5D, -0.5D, -0.5D, block.getTexture(4));

        // --- South Face (X + 1) ---
        faceLuminance = block.getLuminance(world, x + 1, y, z);
        if (faceLuminance < currentLuminance) faceLuminance = currentLuminance;

        tess.setColorOpaque_F(lightX * faceLuminance, lightX * faceLuminance, lightX * faceLuminance);
        RenderSouthFace(block, -0.5D, -0.5D, -0.5D, block.getTexture(5));

        tess.draw();
    }

    public void RenderBlockOnInventory(Block block, int metadata, float brightness)
    {
        Tessellator tess = _tess;
        int renderType = block.getRenderType();

        if (RenderFromInside)
        {
            int color = block.getColor(metadata);
            float red = (color >> 16 & 255) / 255.0F;
            float green = (color >> 8 & 255) / 255.0F;
            float blue = (color & 255) / 255.0F;

            // Apply color and brightness to the global GL state
            GLManager.GL.Color4(red * brightness, green * brightness, blue * brightness, 1.0F);
        }

        // Standard blocks (0) and Piston Bases (16) use standard 6-face cube rendering
        if (renderType == 0 || renderType == 16)
        {
            if (renderType == 16) metadata = 1; // Force standard texture for piston items

            block.setupRenderBoundingBox();
            GLManager.GL.Translate(-0.5F, -0.5F, -0.5F);

            tess.startDrawingQuads();
            tess.setNormal(0.0F, -1.0F, 0.0F);
            Helper.RenderBottomFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(0, metadata));
            tess.draw();

            tess.startDrawingQuads();
            tess.setNormal(0.0F, 1.0F, 0.0F);
            Helper.RenderTopFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(1, metadata));
            tess.draw();

            tess.startDrawingQuads();
            tess.setNormal(0.0F, 0.0F, -1.0F);
            Helper.RenderEastFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(2, metadata));
            tess.draw();

            tess.startDrawingQuads();
            tess.setNormal(0.0F, 0.0F, 1.0F);
            Helper.RenderWestFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(3, metadata));
            tess.draw();

            tess.startDrawingQuads();
            tess.setNormal(-1.0F, 0.0F, 0.0F);
            Helper.RenderNorthFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(4, metadata));
            tess.draw();

            tess.startDrawingQuads();
            tess.setNormal(1.0F, 0.0F, 0.0F);
            RenderSouthFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(5, metadata));
            tess.draw();

            GLManager.GL.Translate(0.5F, 0.5F, 0.5F);
        }
        else if (renderType == 1) // Crossed squares (Flowers, Saplings)
        {
            tess.startDrawingQuads();
            tess.setNormal(0.0F, -1.0F, 0.0F);
            RenderCrossedSquares(block, metadata, -0.5D, -0.5D, -0.5D);
            tess.draw();
        }
        else if (renderType == 13) // Cactus (slightly inset faces)
        {
            block.setupRenderBoundingBox();
            GLManager.GL.Translate(-0.5F, -0.5F, -0.5F);
            float inset = 1.0F / 16.0F;

            tess.startDrawingQuads();
            tess.setNormal(0.0F, -1.0F, 0.0F);
            Helper.RenderBottomFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(0));
            tess.draw();

            tess.startDrawingQuads();
            tess.setNormal(0.0F, 1.0F, 0.0F);
            Helper.RenderTopFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(1));
            tess.draw();

            tess.startDrawingQuads();
            tess.setNormal(0.0F, 0.0F, -1.0F);
            tess.setTranslationF(0.0F, 0.0F, inset);
            Helper.RenderEastFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(2));
            tess.setTranslationF(0.0F, 0.0F, -inset);
            tess.draw();

            tess.startDrawingQuads();
            tess.setNormal(0.0F, 0.0F, 1.0F);
            tess.setTranslationF(0.0F, 0.0F, -inset);
            Helper.RenderWestFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(3));
            tess.setTranslationF(0.0F, 0.0F, inset);
            tess.draw();

            tess.startDrawingQuads();
            tess.setNormal(-1.0F, 0.0F, 0.0F);
            tess.setTranslationF(inset, 0.0F, 0.0F);
            Helper.RenderNorthFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(4));
            tess.setTranslationF(-inset, 0.0F, 0.0F);
            tess.draw();

            tess.startDrawingQuads();
            tess.setNormal(1.0F, 0.0F, 0.0F);
            tess.setTranslationF(-inset, 0.0F, 0.0F);
            RenderSouthFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(5));
            tess.setTranslationF(inset, 0.0F, 0.0F);
            tess.draw();

            GLManager.GL.Translate(0.5F, 0.5F, 0.5F);
        }
        else if (renderType == 6) // Crops (Wheat/Seeds)
        {
            tess.startDrawingQuads();
            tess.setNormal(0.0F, -1.0F, 0.0F);
            RenderCropQuads(block, metadata, -0.5D, -0.5D, -0.5D);
            tess.draw();
        }
        else if (renderType == 2) // Torch
        {
            tess.startDrawingQuads();
            tess.setNormal(0.0F, -1.0F, 0.0F);
            RenderTorchAtAngle(block, -0.5D, -0.5D, -0.5D, 0.0D, 0.0D);
            tess.draw();
        }
        else if (renderType == 10) // Stairs
        {
            for (int i = 0; i < 2; ++i)
            {
                if (i == 0) block.setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.5F);
                if (i == 1) block.setBoundingBox(0.0F, 0.0F, 0.5F, 1.0F, 0.5F, 1.0F);

                GLManager.GL.Translate(-0.5F, -0.5F, -0.5F);
                RenderCubeItem(block, tess);
                GLManager.GL.Translate(0.5F, 0.5F, 0.5F);
            }
        }
        else if (renderType == 11) // Fence
        {
            for (int i = 0; i < 4; ++i)
            {
                float size = 2.0F / 16.0F;
                if (i == 0) block.setBoundingBox(0.5F - size, 0.0F, 0.0F, 0.5F + size, 1.0F, size * 2.0F);
                if (i == 1) block.setBoundingBox(0.5F - size, 0.0F, 1.0F - size * 2.0F, 0.5F + size, 1.0F, 1.0F);

                size = 1.0F / 16.0F;
                if (i == 2)
                    block.setBoundingBox(0.5F - size, 1.0F - size * 3.0F, -size * 2.0F, 0.5F + size, 1.0F - size,
                        1.0F + size * 2.0F);
                if (i == 3)
                    block.setBoundingBox(0.5F - size, 0.5F - size * 3.0F, -size * 2.0F, 0.5F + size, 0.5F - size,
                        1.0F + size * 2.0F);

                GLManager.GL.Translate(-0.5F, -0.5F, -0.5F);
                RenderCubeItem(block, tess);
                GLManager.GL.Translate(0.5F, 0.5F, 0.5F);
            }

            block.setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
        }
    }

    private void RenderCubeItem(Block block, Tessellator tess)
    {
        tess.startDrawingQuads();
        tess.setNormal(0.0F, -1.0F, 0.0F);
        Helper.RenderBottomFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(0));
        tess.draw();
        tess.startDrawingQuads();
        tess.setNormal(0.0F, 1.0F, 0.0F);
        Helper.RenderTopFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(1));
        tess.draw();
        tess.startDrawingQuads();
        tess.setNormal(0.0F, 0.0F, -1.0F);
        Helper.RenderEastFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(2));
        tess.draw();
        tess.startDrawingQuads();
        tess.setNormal(0.0F, 0.0F, 1.0F);
        Helper.RenderWestFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(3));
        tess.draw();
        tess.startDrawingQuads();
        tess.setNormal(-1.0F, 0.0F, 0.0F);
        Helper.RenderNorthFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(4));
        tess.draw();
        tess.startDrawingQuads();
        tess.setNormal(1.0F, 0.0F, 0.0F);
        RenderSouthFace(block, 0.0D, 0.0D, 0.0D, block.getTexture(5));
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
