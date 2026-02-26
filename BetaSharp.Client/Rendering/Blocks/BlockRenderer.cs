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
    private readonly IBlockAccess _blockAccess = null!;
    private readonly Tessellator _tess = Tessellator.instance;
    private readonly RendererRegistry _rendererRegistry = new();


    public BlockRenderer(IBlockAccess iBlockAccess, Tessellator tess)
    {
        _blockAccess = iBlockAccess;
        _tess = tess;

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

    private void SetOverrideBoundingBox(double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
    {
        _overrideBoundingBox = new Box(minX, minY, minZ, maxX, maxY, maxZ);
        _useOverrideBoundingBox = true;
    }

    public void RenderBlockWithTextureOverride(IBlockAccess world, Block block, BlockPos pos, Tessellator tess,
        int textureId)
    {
        _overrideBlockTexture = textureId;
        RenderBlockByRenderType(world, block, pos, tess);
        _overrideBlockTexture = -1; // Reset to default
    }

    public void RenderBlockForcedAllFaces(IBlockAccess world, Block block, BlockPos pos, Tessellator tess)
    {
        _renderAllFaces = true;
        RenderBlockByRenderType(world, block, pos, tess);
        _renderAllFaces = false; // Reset to default
    }

    public bool RenderBlockByRenderType(IBlockAccess world, Block block, BlockPos pos, Tessellator tess)
    {
        RendererType type = (RendererType)block.getRenderType();

        block.updateBoundingBox(_blockAccess, pos.x, pos.y, pos.z);

        Box? activeBounds = _useOverrideBoundingBox ? _overrideBoundingBox : block.BoundingBox;

        bool isPistonExtension = (type == RendererType.PistonExtension);

        var ctx = new BlockRenderContext(
            overrideTexture: _overrideBlockTexture,
            renderAllFaces: _renderAllFaces,
            flipTexture: flipTexture,
            bounds: activeBounds,
            uvTop: _uvRotateTop,
            uvBottom: _uvRotateBottom,
            uvNorth: _uvRotateNorth,
            uvSouth: _uvRotateSouth,
            uvEast: _uvRotateEast,
            uvWest: _uvRotateWest,
            customFlag: isPistonExtension
        );

        try
        {
            IBlockRenderer renderer = _rendererRegistry[type];

            return renderer.Render(world, block, pos, tess, ctx);
        }
        catch
        {
            return false;
        }
    }

    private bool RenderBlockBed(Block block, int x, int y, int z)
    {
        Tessellator tess = _tess;
        Box bounds = _useOverrideBoundingBox ? _overrideBoundingBox : block.BoundingBox;

        int metadata = _blockAccess.getBlockMeta(x, y, z);
        int direction = BlockBed.getDirection(metadata);
        bool isHead = BlockBed.isHeadOfBed(metadata);

        float lightBottom = 0.5F;
        float lightTop = 1.0F;
        float lightZ = 0.8F;
        float lightX = 0.6F;

        float centerLuminance = block.getLuminance(_blockAccess, x, y, z);

        // BOTTOM FACE
        tess.setColorOpaque_F(lightBottom * centerLuminance, lightBottom * centerLuminance,
            lightBottom * centerLuminance);

        int texBottom = block.getTextureId(_blockAccess, x, y, z, 0);
        int texU = (texBottom & 15) << 4;
        int texV = texBottom & 240;

        double minU = texU / 256.0F;
        double maxU = (texU + 15.99D) / 256.0D;
        double minV = texV / 256.0F;
        double maxV = (texV + 15.99D) / 256.0D;

        double minX = x + bounds.MinX;
        double maxX = x + bounds.MaxX;
        double bedBottomY = y + bounds.MinY + 0.1875D; // Bed legs are 3 pixels tall (3/16 = 0.1875)
        double minZ = z + bounds.MinZ;
        double maxZ = z + bounds.MaxZ;

        tess.addVertexWithUV(minX, bedBottomY, maxZ, minU, maxV);
        tess.addVertexWithUV(minX, bedBottomY, minZ, minU, minV);
        tess.addVertexWithUV(maxX, bedBottomY, minZ, maxU, minV);
        tess.addVertexWithUV(maxX, bedBottomY, maxZ, maxU, maxV);

        // TOP FACE
        float topLuminance = block.getLuminance(_blockAccess, x, y + 1, z);
        tess.setColorOpaque_F(lightTop * topLuminance, lightTop * topLuminance, lightTop * topLuminance);

        int texTop = block.getTextureId(_blockAccess, x, y, z, 1);
        texU = (texTop & 15) << 4;
        texV = texTop & 240;

        minU = texU / 256.0F;
        maxU = (texU + 15.99D) / 256.0D;
        minV = texV / 256.0F;
        maxV = (texV + 15.99D) / 256.0D;

        double u1 = minU, u2 = maxU, u3 = minU, u4 = maxU;
        double v1 = minV, v2 = minV, v3 = maxV, v4 = maxV;

        // Rotate top texture based on bed orientation
        if (direction == 0) // South
        {
            u2 = minU;
            v2 = maxV;
            u3 = maxU;
            v3 = minV;
        }
        else if (direction == 2) // North
        {
            u1 = maxU;
            v1 = maxV;
            u4 = minU;
            v4 = minV;
        }
        else if (direction == 3) // East
        {
            u1 = maxU;
            v1 = maxV;
            u4 = minU;
            v4 = minV;
            u2 = minU;
            v2 = maxV;
            u3 = maxU;
            v3 = minV;
        }

        double bedTopY = y + bounds.MaxY;

        tess.addVertexWithUV(maxX, bedTopY, maxZ, u3, v3);
        tess.addVertexWithUV(maxX, bedTopY, minZ, u1, v1);
        tess.addVertexWithUV(minX, bedTopY, minZ, u2, v2);
        tess.addVertexWithUV(minX, bedTopY, maxZ, u4, v4);

        // SIDE FACES
        int forwardDir = Facings.TO_DIR[direction];
        if (isHead)
        {
            forwardDir = Facings.TO_DIR[Facings.OPPOSITE[direction]];
        }

        byte textureFlipDir = 4;
        switch (direction)
        {
            case 0: textureFlipDir = 5; break;
            case 1:
                textureFlipDir = 3;
                goto case 2;
            case 2:
            default: break;
            case 3: textureFlipDir = 2; break;
        }

        float faceLuminance;

        // East Face (Z - 1)
        if (forwardDir != 2 && (_renderAllFaces || block.isSideVisible(_blockAccess, x, y, z - 1, 2)))
        {
            faceLuminance = bounds.MinZ > 0.0D ? centerLuminance : block.getLuminance(_blockAccess, x, y, z - 1);
            tess.setColorOpaque_F(lightZ * faceLuminance, lightZ * faceLuminance, lightZ * faceLuminance);

            flipTexture = textureFlipDir == 2;
            Helper.RenderEastFace(block, x, y, z, block.getTextureId(_blockAccess, x, y, z, 2));
        }

        // West Face (Z + 1)
        if (forwardDir != 3 && (_renderAllFaces || block.isSideVisible(_blockAccess, x, y, z + 1, 3)))
        {
            faceLuminance = bounds.MaxZ < 1.0D ? centerLuminance : block.getLuminance(_blockAccess, x, y, z + 1);
            tess.setColorOpaque_F(lightZ * faceLuminance, lightZ * faceLuminance, lightZ * faceLuminance);

            flipTexture = textureFlipDir == 3;
            Helper.RenderWestFace(block, x, y, z, block.getTextureId(_blockAccess, x, y, z, 3));
        }

        // North Face (X - 1)
        if (forwardDir != 4 && (_renderAllFaces || block.isSideVisible(_blockAccess, x - 1, y, z, 4)))
        {
            faceLuminance = bounds.MinX > 0.0D ? centerLuminance : block.getLuminance(_blockAccess, x - 1, y, z);
            tess.setColorOpaque_F(lightX * faceLuminance, lightX * faceLuminance, lightX * faceLuminance);

            flipTexture = textureFlipDir == 4;
            Helper.RenderNorthFace(block, x, y, z, block.getTextureId(_blockAccess, x, y, z, 4));
        }

        // South Face (X + 1)
        if (forwardDir != 5 && (_renderAllFaces || block.isSideVisible(_blockAccess, x + 1, y, z, 5)))
        {
            faceLuminance = bounds.MaxX < 1.0D ? centerLuminance : block.getLuminance(_blockAccess, x + 1, y, z);
            tess.setColorOpaque_F(lightX * faceLuminance, lightX * faceLuminance, lightX * faceLuminance);

            flipTexture = textureFlipDir == 5;
            RenderSouthFace(block, x, y, z, block.getTextureId(_blockAccess, x, y, z, 5));
        }

        flipTexture = false;
        return true;
    }

    private bool RenderBlockTorch(Block block, int x, int y, int z)
    {
        int metadata = _blockAccess.getBlockMeta(x, y, z);
        Tessellator tess = _tess;

        float luminance = block.getLuminance(_blockAccess, x, y, z);
        if (Block.BlocksLightLuminance[block.id] > 0)
        {
            luminance = 1.0F;
        }

        tess.setColorOpaque_F(luminance, luminance, luminance);

        double tiltAmount = 0.4F;
        double horizontalOffset = 0.5D - tiltAmount;
        double verticalOffset = 0.2F;

        if (metadata == 1) // Attached to West wall (pointing East)
        {
            RenderTorchAtAngle(block, x - horizontalOffset, y + verticalOffset, z, -tiltAmount, 0.0D);
        }
        else if (metadata == 2) // Attached to East wall (pointing West)
        {
            RenderTorchAtAngle(block, x + horizontalOffset, y + verticalOffset, z, tiltAmount, 0.0D);
        }
        else if (metadata == 3) // Attached to North wall (pointing South)
        {
            RenderTorchAtAngle(block, x, y + verticalOffset, z - horizontalOffset, 0.0D, -tiltAmount);
        }
        else if (metadata == 4) // Attached to South wall (pointing North)
        {
            RenderTorchAtAngle(block, x, y + verticalOffset, z + horizontalOffset, 0.0D, tiltAmount);
        }
        else // Standing on floor
        {
            RenderTorchAtAngle(block, x, y, z, 0.0D, 0.0D);
        }

        return true;
    }

    private bool RenderBlockRepeater(Block block, int x, int y, int z)
    {
        int metadata = _blockAccess.getBlockMeta(x, y, z);
        int direction = metadata & 3;
        int delay = (metadata & 12) >> 2;

        // Render the base slab
        RenderStandardBlock(block, x, y, z);

        Tessellator tess = _tess;
        float luminance = block.getLuminance(_blockAccess, x, y, z);
        if (Block.BlocksLightLuminance[block.id] > 0)
        {
            luminance = (luminance + 1.0F) * 0.5F;
        }

        tess.setColorOpaque_F(luminance, luminance, luminance);

        double torchVerticalOffset = -0.1875D;
        double staticTorchX = 0.0D;
        double staticTorchZ = 0.0D;
        double delayTorchX = 0.0D;
        double delayTorchZ = 0.0D;

        // Calculate positions for the two torch pins based on direction and delay
        switch (direction)
        {
            case 0: // South
                delayTorchZ = -0.3125D;
                staticTorchZ = BlockRedstoneRepeater.RENDER_OFFSET[delay];
                break;
            case 1: // West
                delayTorchX = 0.3125D;
                staticTorchX = -BlockRedstoneRepeater.RENDER_OFFSET[delay];
                break;
            case 2: // North
                delayTorchZ = 0.3125D;
                staticTorchZ = -BlockRedstoneRepeater.RENDER_OFFSET[delay];
                break;
            case 3: // East
                delayTorchX = -0.3125D;
                staticTorchX = BlockRedstoneRepeater.RENDER_OFFSET[delay];
                break;
        }

        // Render the two torch pins on top of the slab
        RenderTorchAtAngle(block, x + staticTorchX, y + torchVerticalOffset, z + staticTorchZ, 0.0D, 0.0D);
        RenderTorchAtAngle(block, x + delayTorchX, y + torchVerticalOffset, z + delayTorchZ, 0.0D, 0.0D);

        // Render the top surface texture of the repeater slab
        int textureId = block.getTexture(1);
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = texU / 256.0F;
        double maxU = (texU + 15.99F) / 256.0F;
        double minV = texV / 256.0F;
        double maxV = (texV + 15.99F) / 256.0F;

        float surfaceHeight = y + (2.0F / 16.0F);
        float x1 = x + 1;
        float x2 = x + 1;
        float x3 = x + 0;
        float x4 = x + 0;
        float z1 = z + 0;
        float z2 = z + 1;
        float z3 = z + 1;
        float z4 = z + 0;

        if (direction == 2) // North
        {
            x2 = x + 0;
            x1 = x2;
            x4 = x + 1;
            x3 = x4;
            z4 = z + 1;
            z1 = z4;
            z3 = z + 0;
            z2 = z3;
        }
        else if (direction == 3) // East
        {
            x4 = x + 0;
            x1 = x4;
            x3 = x + 1;
            x2 = x3;
            z2 = z + 0;
            z1 = z2;
            z4 = z + 1;
            z3 = z4;
        }
        else if (direction == 1) // West
        {
            x4 = x + 1;
            x1 = x4;
            x3 = x + 0;
            x2 = x3;
            z2 = z + 1;
            z1 = z2;
            z4 = z + 0;
            z3 = z4;
        }

        tess.addVertexWithUV(x4, surfaceHeight, z4, minU, minV);
        tess.addVertexWithUV(x3, surfaceHeight, z3, minU, maxV);
        tess.addVertexWithUV(x2, surfaceHeight, z2, maxU, maxV);
        tess.addVertexWithUV(x1, surfaceHeight, z1, maxU, minV);

        return true;
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

    private bool RenderBlockLever(Block block, int x, int y, int z)
    {
        int metadata = _blockAccess.getBlockMeta(x, y, z);
        int orientation = metadata & 7;
        bool isActivated = (metadata & 8) > 0;
        Tessellator tess = _tess;

        // Levers use a cobblestone texture for the baseplate by default
        bool hasTextureOverride = _overrideBlockTexture >= 0;
        if (!hasTextureOverride)
        {
            _overrideBlockTexture = Block.Cobblestone.textureId;
        }

        float baseWidth = 0.25F;
        float baseThickness = 3.0F / 16.0F;
        float baseHeight = 3.0F / 16.0F;

        // --- 1. Render the Base Plate ---
        if (orientation == 5) // Floor (North/South)
        {
            SetOverrideBoundingBox(0.5F - baseHeight, 0.0F, 0.5F - baseWidth, 0.5F + baseHeight, baseThickness,
                0.5F + baseWidth);
        }
        else if (orientation == 6) // Floor (East/West)
        {
            SetOverrideBoundingBox(0.5F - baseWidth, 0.0F, 0.5F - baseHeight, 0.5F + baseWidth, baseThickness,
                0.5F + baseHeight);
        }
        else if (orientation == 4) // Wall South
        {
            SetOverrideBoundingBox(0.5F - baseHeight, 0.5F - baseWidth, 1.0F - baseThickness, 0.5F + baseHeight,
                0.5F + baseWidth, 1.0F);
        }
        else if (orientation == 3) // Wall North
        {
            SetOverrideBoundingBox(0.5F - baseHeight, 0.5F - baseWidth, 0.0F, 0.5F + baseHeight, 0.5F + baseWidth,
                baseThickness);
        }
        else if (orientation == 2) // Wall East
        {
            SetOverrideBoundingBox(1.0F - baseThickness, 0.5F - baseWidth, 0.5F - baseHeight, 1.0F, 0.5F + baseWidth,
                0.5F + baseHeight);
        }
        else if (orientation == 1) // Wall West
        {
            SetOverrideBoundingBox(0.0F, 0.5F - baseWidth, 0.5F - baseHeight, baseThickness, 0.5F + baseWidth,
                0.5F + baseHeight);
        }

        RenderStandardBlock(block, x, y, z);

        if (!hasTextureOverride)
        {
            _overrideBlockTexture = -1;
        }

        // --- 2. Calculate Handle Lighting & Texture ---
        float luminance = block.getLuminance(_blockAccess, x, y, z);
        if (Block.BlocksLightLuminance[block.id] > 0) luminance = 1.0F;
        tess.setColorOpaque_F(luminance, luminance, luminance);

        int textureId = block.getTexture(0);
        if (_overrideBlockTexture >= 0) textureId = _overrideBlockTexture;

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        float minU = texU / 256.0F;
        float maxU = (texU + 15.99F) / 256.0F;
        float minV = texV / 256.0F;
        float maxV = (texV + 15.99F) / 256.0F;

        // --- 3. Handle Vertex Math ---
        Vector3D<double>[] vertices = new Vector3D<double>[8];
        float hRadius = 1.0F / 16.0F;
        float hLength = 10.0F / 16.0F;

        // Initial handle box (standing straight up)
        vertices[0] = new(-hRadius, 0.0D, -hRadius);
        vertices[1] = new(hRadius, 0.0D, -hRadius);
        vertices[2] = new(hRadius, 0.0D, hRadius);
        vertices[3] = new(-hRadius, 0.0D, hRadius);
        vertices[4] = new(-hRadius, hLength, -hRadius);
        vertices[5] = new(hRadius, hLength, -hRadius);
        vertices[6] = new(hRadius, hLength, hRadius);
        vertices[7] = new(-hRadius, hLength, hRadius);

        for (int i = 0; i < 8; ++i)
        {
            // Toggle angle based on state
            if (isActivated)
            {
                vertices[i].Z -= 1.0D / 16.0D;
                RotateAroundX(ref vertices[i], (float)Math.PI * 2.0F / 9.0F);
            }
            else
            {
                vertices[i].Z += 1.0D / 16.0D;
                RotateAroundX(ref vertices[i], -(float)Math.PI * 2.0F / 9.0F);
            }

            // Apply orientation rotations
            if (orientation == 6) RotateAroundY(ref vertices[i], (float)Math.PI * 0.5F);

            if (orientation < 5) // Wall mount requires extra rotation
            {
                vertices[i].Y -= 0.375D;
                RotateAroundX(ref vertices[i], (float)Math.PI * 0.5F);

                if (orientation == 3) RotateAroundY(ref vertices[i], (float)Math.PI);
                if (orientation == 2) RotateAroundY(ref vertices[i], (float)Math.PI * 0.5F);
                if (orientation == 1) RotateAroundY(ref vertices[i], (float)Math.PI * -0.5F);

                vertices[i].X += x + 0.5D;
                vertices[i].Y += y + 0.5D;
                vertices[i].Z += z + 0.5D;
            }
            else
            {
                vertices[i].X += x + 0.5D;
                vertices[i].Y += y + 2.0F / 16.0F;
                vertices[i].Z += z + 0.5D;
            }
        }

        // --- 4. Draw the Handle Faces ---
        for (int face = 0; face < 6; ++face)
        {
            // The handle uses specific tiny snippets of the texture atlas for its detail
            if (face == 0) // Bottom cap
            {
                minU = (texU + 7) / 256.0F;
                maxU = (texU + 9 - 0.01F) / 256.0F;
                minV = (texV + 6) / 256.0F;
                maxV = (texV + 8 - 0.01F) / 256.0F;
            }
            else if (face == 2) // Side detail
            {
                minU = (texU + 7) / 256.0F;
                maxU = (texU + 9 - 0.01F) / 256.0F;
                minV = (texV + 6) / 256.0F;
                maxV = (texV + 16 - 0.01F) / 256.0F;
            }

            Vector3D<double> v1 = default, v2 = default, v3 = default, v4 = default;

            switch (face)
            {
                case 0:
                    v1 = vertices[0];
                    v2 = vertices[1];
                    v3 = vertices[2];
                    v4 = vertices[3];
                    break;
                case 1:
                    v1 = vertices[7];
                    v2 = vertices[6];
                    v3 = vertices[5];
                    v4 = vertices[4];
                    break;
                case 2:
                    v1 = vertices[1];
                    v2 = vertices[0];
                    v3 = vertices[4];
                    v4 = vertices[5];
                    break;
                case 3:
                    v1 = vertices[2];
                    v2 = vertices[1];
                    v3 = vertices[5];
                    v4 = vertices[6];
                    break;
                case 4:
                    v1 = vertices[3];
                    v2 = vertices[2];
                    v3 = vertices[6];
                    v4 = vertices[7];
                    break;
                case 5:
                    v1 = vertices[0];
                    v2 = vertices[3];
                    v3 = vertices[7];
                    v4 = vertices[4];
                    break;
            }

            tess.addVertexWithUV(v1.X, v1.Y, v1.Z, minU, maxV);
            tess.addVertexWithUV(v2.X, v2.Y, v2.Z, maxU, maxV);
            tess.addVertexWithUV(v3.X, v3.Y, v3.Z, maxU, minV);
            tess.addVertexWithUV(v4.X, v4.Y, v4.Z, minU, minV);
        }

        return true;
    }

    private bool RenderBlockFire(Block block, int x, int y, int z)
    {
        Tessellator tess = _tess;
        int textureId = block.getTexture(0);
        if (_overrideBlockTexture >= 0) textureId = _overrideBlockTexture;

        float luminance = block.getLuminance(_blockAccess, x, y, z);
        tess.setColorOpaque_F(luminance, luminance, luminance);

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = texU / 256.0F;
        double maxU = (texU + 15.99F) / 256.0F;
        double minV = texV / 256.0F;
        double maxV = (texV + 15.99F) / 256.0F;

        float fireHeight = 1.4F;

        // If not on a solid/flammable floor, render climbing flames on walls
        if (!_blockAccess.shouldSuffocate(x, y - 1, z) && !Block.Fire.isFlammable(_blockAccess, x, y - 1, z))
        {
            float sideInset = 0.2F;
            float yOffset = 1.0F / 16.0F;

            // Variation: Flip texture or use second fire frame based on position
            if ((x + y + z & 1) == 1)
            {
                minV = (texV + 16) / 256.0F;
                maxV = (texV + 15.99F + 16.0F) / 256.0F;
            }

            if ((x / 2 + y / 2 + z / 2 & 1) == 1)
            {
                (minU, maxU) = (maxU, minU);
            }

            // Climbing West Wall
            if (Block.Fire.isFlammable(_blockAccess, x - 1, y, z))
            {
                tess.addVertexWithUV(x + sideInset, y + fireHeight + yOffset, z + 1, maxU, minV);
                tess.addVertexWithUV(x, y + yOffset, z + 1, maxU, maxV);
                tess.addVertexWithUV(x, y + yOffset, z, minU, maxV);
                tess.addVertexWithUV(x + sideInset, y + fireHeight + yOffset, z, minU, minV);
                // Backface
                tess.addVertexWithUV(x + sideInset, y + fireHeight + yOffset, z, minU, minV);
                tess.addVertexWithUV(x, y + yOffset, z, minU, maxV);
                tess.addVertexWithUV(x, y + yOffset, z + 1, maxU, maxV);
                tess.addVertexWithUV(x + sideInset, y + fireHeight + yOffset, z + 1, maxU, minV);
            }

            // Climbing East Wall
            if (Block.Fire.isFlammable(_blockAccess, x + 1, y, z))
            {
                tess.addVertexWithUV(x + 1 - sideInset, y + fireHeight + yOffset, z, minU, minV);
                tess.addVertexWithUV(x + 1, y + yOffset, z, minU, maxV);
                tess.addVertexWithUV(x + 1, y + yOffset, z + 1, maxU, maxV);
                tess.addVertexWithUV(x + 1 - sideInset, y + fireHeight + yOffset, z + 1, maxU, minV);
                // Backface
                tess.addVertexWithUV(x + 1 - sideInset, y + fireHeight + yOffset, z + 1, maxU, minV);
                tess.addVertexWithUV(x + 1, y + yOffset, z + 1, maxU, maxV);
                tess.addVertexWithUV(x + 1, y + yOffset, z, minU, maxV);
                tess.addVertexWithUV(x + 1 - sideInset, y + fireHeight + yOffset, z, minU, minV);
            }

            // Climbing North Wall
            if (Block.Fire.isFlammable(_blockAccess, x, y, z - 1))
            {
                tess.addVertexWithUV(x, y + fireHeight + yOffset, z + sideInset, maxU, minV);
                tess.addVertexWithUV(x, y + yOffset, z, maxU, maxV);
                tess.addVertexWithUV(x + 1, y + yOffset, z, minU, maxV);
                tess.addVertexWithUV(x + 1, y + fireHeight + yOffset, z + sideInset, minU, minV);
                // Backface
                tess.addVertexWithUV(x + 1, y + fireHeight + yOffset, z + sideInset, minU, minV);
                tess.addVertexWithUV(x + 1, y + yOffset, z, minU, maxV);
                tess.addVertexWithUV(x, y + yOffset, z, maxU, maxV);
                tess.addVertexWithUV(x, y + fireHeight + yOffset, z + sideInset, maxU, minV);
            }

            // Climbing South Wall
            if (Block.Fire.isFlammable(_blockAccess, x, y, z + 1))
            {
                tess.addVertexWithUV(x + 1, y + fireHeight + yOffset, z + 1 - sideInset, minU, minV);
                tess.addVertexWithUV(x + 1, y + yOffset, z + 1, minU, maxV);
                tess.addVertexWithUV(x, y + yOffset, z + 1, maxU, maxV);
                tess.addVertexWithUV(x, y + fireHeight + yOffset, z + 1 - sideInset, maxU, minV);
                // Backface
                tess.addVertexWithUV(x, y + fireHeight + yOffset, z + 1 - sideInset, maxU, minV);
                tess.addVertexWithUV(x, y + yOffset, z + 1, maxU, maxV);
                tess.addVertexWithUV(x + 1, y + yOffset, z + 1, minU, maxV);
                tess.addVertexWithUV(x + 1, y + fireHeight + yOffset, z + 1 - sideInset, minU, minV);
            }

            // Climbing Ceilings
            if (Block.Fire.isFlammable(_blockAccess, x, y + 1, z))
            {
                double xMax = x + 1, xMin = x;
                double zMax = z + 1, zMin = z;

                minU = texU / 256.0F;
                maxU = (texU + 15.99F) / 256.0F;
                minV = texV / 256.0F;
                maxV = (texV + 15.99F) / 256.0F;

                int ceilY = y + 1;
                float ceilOffset = -0.2F;

                if ((x + ceilY + z & 1) == 0)
                {
                    tess.addVertexWithUV(xMin, ceilY + ceilOffset, z, maxU, minV);
                    tess.addVertexWithUV(xMax, ceilY, z, maxU, maxV);
                    tess.addVertexWithUV(xMax, ceilY, z + 1, minU, maxV);
                    tess.addVertexWithUV(xMin, ceilY + ceilOffset, z + 1, minU, minV);

                    minV = (texV + 16) / 256.0F;
                    maxV = (texV + 15.99F + 16.0F) / 256.0F;

                    tess.addVertexWithUV(xMax, ceilY + ceilOffset, z + 1, maxU, minV);
                    tess.addVertexWithUV(xMin, ceilY, z + 1, maxU, maxV);
                    tess.addVertexWithUV(xMin, ceilY, z, minU, maxV);
                    tess.addVertexWithUV(xMax, ceilY + ceilOffset, z, minU, minV);
                }
                else
                {
                    tess.addVertexWithUV(x, ceilY + ceilOffset, zMax, maxU, minV);
                    tess.addVertexWithUV(x, ceilY, zMin, maxU, maxV);
                    tess.addVertexWithUV(x + 1, ceilY, zMin, minU, maxV);
                    tess.addVertexWithUV(x + 1, ceilY + ceilOffset, zMax, minU, minV);

                    minV = (texV + 16) / 256.0F;
                    maxV = (texV + 15.99F + 16.0F) / 256.0F;

                    tess.addVertexWithUV(x + 1, ceilY + ceilOffset, zMin, maxU, minV);
                    tess.addVertexWithUV(x + 1, ceilY, zMax, maxU, maxV);
                    tess.addVertexWithUV(x, ceilY, zMax, minU, maxV);
                    tess.addVertexWithUV(x, ceilY + ceilOffset, zMin, minU, minV);
                }
            }
        }
        else // Render central "X" flames for fire on solid floors
        {
            double insetSmall = 0.2D, insetLarge = 0.3D;
            double xC = x + 0.5D, zC = z + 0.5D;

            // First diagonal set
            tess.addVertexWithUV(xC - insetLarge, y + fireHeight, z + 1, maxU, minV);
            tess.addVertexWithUV(xC + insetSmall, y, z + 1, maxU, maxV);
            tess.addVertexWithUV(xC + insetSmall, y, z, minU, maxV);
            tess.addVertexWithUV(xC - insetLarge, y + fireHeight, z, minU, minV);

            tess.addVertexWithUV(xC + insetLarge, y + fireHeight, z, maxU, minV);
            tess.addVertexWithUV(xC - insetSmall, y, z, maxU, maxV);
            tess.addVertexWithUV(xC - insetSmall, y, z + 1, minU, maxV);
            tess.addVertexWithUV(xC + insetLarge, y + fireHeight, z + 1, minU, minV);

            // Switch texture frame
            minV = (texV + 16) / 256.0F;
            maxV = (texV + 15.99F + 16.0F) / 256.0F;

            // Second diagonal set (X-axis dominant)
            tess.addVertexWithUV(x + 1, y + fireHeight, zC + insetLarge, maxU, minV);
            tess.addVertexWithUV(x + 1, y, zC - insetSmall, maxU, maxV);
            tess.addVertexWithUV(x, y, zC - insetSmall, minU, maxV);
            tess.addVertexWithUV(x, y + fireHeight, zC + insetLarge, minU, minV);

            tess.addVertexWithUV(x, y + fireHeight, zC - insetLarge, maxU, minV);
            tess.addVertexWithUV(x, y, zC + insetSmall, maxU, maxV);
            tess.addVertexWithUV(x + 1, y, zC + insetSmall, minU, maxV);
            tess.addVertexWithUV(x + 1, y + fireHeight, zC - insetLarge, minU, minV);

            // Third set (outer crossing)
            double i4 = 0.4D, i5 = 0.5D;
            tess.addVertexWithUV(xC - i4, y + fireHeight, z, minU, minV);
            tess.addVertexWithUV(xC - i5, y, z, minU, maxV);
            tess.addVertexWithUV(xC - i5, y, z + 1, maxU, maxV);
            tess.addVertexWithUV(xC - i4, y + fireHeight, z + 1, maxU, minV);

            tess.addVertexWithUV(xC + i4, y + fireHeight, z + 1, minU, minV);
            tess.addVertexWithUV(xC + i5, y, z + 1, minU, maxV);
            tess.addVertexWithUV(xC + i5, y, z, maxU, maxV);
            tess.addVertexWithUV(xC + i4, y + fireHeight, z, maxU, minV);

            // Final set
            minV = texV / 256.0F;
            maxV = (texV + 15.99F) / 256.0F;
            tess.addVertexWithUV(x, y + fireHeight, zC + i4, minU, minV);
            tess.addVertexWithUV(x, y, zC + i5, minU, maxV);
            tess.addVertexWithUV(x + 1, y, zC + i5, maxU, maxV);
            tess.addVertexWithUV(x + 1, y + fireHeight, zC + i4, maxU, minV);

            tess.addVertexWithUV(x + 1, y + fireHeight, zC - i4, minU, minV);
            tess.addVertexWithUV(x + 1, y, zC - i5, minU, maxV);
            tess.addVertexWithUV(x, y, zC - i5, maxU, maxV);
            tess.addVertexWithUV(x, y + fireHeight, zC - i4, maxU, minV);
        }

        return true;
    }

    private bool RenderBlockRedstoneWire(Block block, int x, int y, int z)
    {
        Tessellator tess = _tess;
        int powerLevel = _blockAccess.getBlockMeta(x, y, z);

        int textureId = block.getTexture(1, powerLevel);
        if (_overrideBlockTexture >= 0) textureId = _overrideBlockTexture;

        // --- 1. Calculate the Glow Color ---
        float luminance = block.getLuminance(_blockAccess, x, y, z);
        float powerPercent = powerLevel / 15.0F;

        // Red component increases with power
        float r = powerPercent * 0.6F + 0.4F;
        if (powerLevel == 0) r = 0.3F;

        // Green and Blue are much lower to keep it red, but they curve up slightly at high power
        float g = powerPercent * powerPercent * 0.7F - 0.5F;
        float b = powerPercent * powerPercent * 0.6F - 0.7F;
        if (g < 0.0F) g = 0.0F;
        if (b < 0.0F) b = 0.0F;

        tess.setColorOpaque_F(luminance * r, luminance * g, luminance * b);

        // --- 2. UV Mapping ---
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = texU / 256.0F;
        double maxU = (texU + 15.99F) / 256.0F;
        double minV = texV / 256.0F;
        double maxV = (texV + 15.99F) / 256.0F;

        // --- 3. Connection Logic ---
        // Checks neighbors on same level OR one level down (if the neighbor isn't solid)
        bool connectsWest = BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x - 1, y, z, 1) ||
                            (!_blockAccess.shouldSuffocate(x - 1, y, z) &&
                             BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x - 1, y - 1, z, -1));
        bool connectsEast = BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x + 1, y, z, 3) ||
                            (!_blockAccess.shouldSuffocate(x + 1, y, z) &&
                             BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x + 1, y - 1, z, -1));
        bool connectsNorth = BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x, y, z - 1, 2) ||
                             (!_blockAccess.shouldSuffocate(x, y, z - 1) &&
                              BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x, y - 1, z - 1, -1));
        bool connectsSouth = BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x, y, z + 1, 0) ||
                             (!_blockAccess.shouldSuffocate(x, y, z + 1) &&
                              BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x, y - 1, z + 1, -1));

        // Check for connections climbing UP a block
        if (!_blockAccess.shouldSuffocate(x, y + 1, z))
        {
            if (_blockAccess.shouldSuffocate(x - 1, y, z) &&
                BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x - 1, y + 1, z, -1)) connectsWest = true;
            if (_blockAccess.shouldSuffocate(x + 1, y, z) &&
                BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x + 1, y + 1, z, -1)) connectsEast = true;
            if (_blockAccess.shouldSuffocate(x, y, z - 1) &&
                BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x, y + 1, z - 1, -1)) connectsNorth = true;
            if (_blockAccess.shouldSuffocate(x, y, z + 1) &&
                BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x, y + 1, z + 1, -1)) connectsSouth = true;
        }

        // --- 4. Determine Shape (Straight vs Cross) ---
        float renderMinX = x, renderMaxX = x + 1;
        float renderMinZ = z, renderMaxZ = z + 1;
        int shapeType = 0; // 0 = Cross, 1 = East/West, 2 = North/South

        if ((connectsWest || connectsEast) && !connectsNorth && !connectsSouth) shapeType = 1;
        if ((connectsNorth || connectsSouth) && !connectsEast && !connectsWest) shapeType = 2;

        if (shapeType != 0) // Use the "Straight Line" texture variant
        {
            minU = (texU + 16) / 256.0F;
            maxU = (texU + 16 + 15.99F) / 256.0F;
        }

        // Shrink the footprint if no connection exists on a specific side
        if (shapeType == 0)
        {
            if (connectsWest || connectsEast || connectsNorth || connectsSouth)
            {
                if (!connectsWest)
                {
                    renderMinX += 0.3125F;
                    minU += 0.01953125D;
                }

                if (!connectsEast)
                {
                    renderMaxX -= 0.3125F;
                    maxU -= 0.01953125D;
                }

                if (!connectsNorth)
                {
                    renderMinZ += 0.3125F;
                    minV += 0.01953125D;
                }

                if (!connectsSouth)
                {
                    renderMaxZ -= 0.3125F;
                    maxV -= 0.01953125D;
                }
            }
        }

        // --- 5. Render Horizontal Ground Quad ---
        double groundY = y + 0.015625D; // 1/64 height offset to prevent Z-fighting

        // Render the colored redstone
        tess.addVertexWithUV(renderMaxX, groundY, renderMaxZ, maxU, maxV);
        tess.addVertexWithUV(renderMaxX, groundY, renderMinZ, maxU, minV);
        tess.addVertexWithUV(renderMinX, groundY, renderMinZ, minU, minV);
        tess.addVertexWithUV(renderMinX, groundY, renderMaxZ, minU, maxV);

        // Render the dark shroud (shadow) underneath
        tess.setColorOpaque_F(luminance, luminance, luminance);
        double shroudVOffset = 1.0D / 16.0D; // Texture atlas row for shadow
        tess.addVertexWithUV(renderMaxX, groundY, renderMaxZ, maxU, maxV + shroudVOffset);
        tess.addVertexWithUV(renderMaxX, groundY, renderMinZ, maxU, minV + shroudVOffset);
        tess.addVertexWithUV(renderMinX, groundY, renderMinZ, minU, minV + shroudVOffset);
        tess.addVertexWithUV(renderMinX, groundY, renderMaxZ, minU, maxV + shroudVOffset);

        // --- 6. Render Slopes (Rising up walls) ---
        if (!_blockAccess.shouldSuffocate(x, y + 1, z))
        {
            minU = (texU + 16) / 256.0F;
            maxU = (texU + 16 + 15.99F) / 256.0F;
            double slopeHeight = y + 1.021875D; // Slight offset above the block

            // West Slope
            if (_blockAccess.shouldSuffocate(x - 1, y, z) && _blockAccess.getBlockId(x - 1, y + 1, z) == block.id)
            {
                tess.setColorOpaque_F(luminance * r, luminance * g, luminance * b);
                tess.addVertexWithUV(x + 0.015625D, slopeHeight, z + 1, maxU, minV);
                tess.addVertexWithUV(x + 0.015625D, y, z + 1, minU, minV);
                tess.addVertexWithUV(x + 0.015625D, y, z + 0, minU, maxV);
                tess.addVertexWithUV(x + 0.015625D, slopeHeight, z + 0, maxU, maxV);

                tess.setColorOpaque_F(luminance, luminance, luminance);
                tess.addVertexWithUV(x + 0.015625D, slopeHeight, z + 1, maxU, minV + shroudVOffset);
                tess.addVertexWithUV(x + 0.015625D, y, z + 1, minU, minV + shroudVOffset);
                tess.addVertexWithUV(x + 0.015625D, y, z + 0, minU, maxV + shroudVOffset);
                tess.addVertexWithUV(x + 0.015625D, slopeHeight, z + 0, maxU, maxV + shroudVOffset);
            }

            // East Slope
            if (_blockAccess.shouldSuffocate(x + 1, y, z) && _blockAccess.getBlockId(x + 1, y + 1, z) == block.id)
            {
                tess.setColorOpaque_F(luminance * r, luminance * g, luminance * b);
                tess.addVertexWithUV(x + 1 - 0.015625D, y, z + 1, minU, maxV);
                tess.addVertexWithUV(x + 1 - 0.015625D, slopeHeight, z + 1, maxU, maxV);
                tess.addVertexWithUV(x + 1 - 0.015625D, slopeHeight, z + 0, maxU, minV);
                tess.addVertexWithUV(x + 1 - 0.015625D, y, z + 0, minU, minV);

                tess.setColorOpaque_F(luminance, luminance, luminance);
                tess.addVertexWithUV(x + 1 - 0.015625D, y, z + 1, minU, maxV + shroudVOffset);
                tess.addVertexWithUV(x + 1 - 0.015625D, slopeHeight, z + 1, maxU, maxV + shroudVOffset);
                tess.addVertexWithUV(x + 1 - 0.015625D, slopeHeight, z + 0, maxU, minV + shroudVOffset);
                tess.addVertexWithUV(x + 1 - 0.015625D, y, z + 0, minU, minV + shroudVOffset);
            }

            // North Slope
            if (_blockAccess.shouldSuffocate(x, y, z - 1) && _blockAccess.getBlockId(x, y + 1, z - 1) == block.id)
            {
                tess.setColorOpaque_F(luminance * r, luminance * g, luminance * b);
                tess.addVertexWithUV(x + 1, y, z + 0.015625D, minU, maxV);
                tess.addVertexWithUV(x + 1, slopeHeight, z + 0.015625D, maxU, maxV);
                tess.addVertexWithUV(x + 0, slopeHeight, z + 0.015625D, maxU, minV);
                tess.addVertexWithUV(x + 0, y, z + 0.015625D, minU, minV);

                tess.setColorOpaque_F(luminance, luminance, luminance);
                tess.addVertexWithUV(x + 1, y, z + 0.015625D, minU, maxV + shroudVOffset);
                tess.addVertexWithUV(x + 1, slopeHeight, z + 0.015625D, maxU, maxV + shroudVOffset);
                tess.addVertexWithUV(x + 0, slopeHeight, z + 0.015625D, maxU, minV + shroudVOffset);
                tess.addVertexWithUV(x + 0, y, z + 0.015625D, minU, minV + shroudVOffset);
            }

            // South Slope
            if (_blockAccess.shouldSuffocate(x, y, z + 1) && _blockAccess.getBlockId(x, y + 1, z + 1) == block.id)
            {
                tess.setColorOpaque_F(luminance * r, luminance * g, luminance * b);
                tess.addVertexWithUV(x + 1, slopeHeight, z + 1 - 0.015625D, maxU, minV);
                tess.addVertexWithUV(x + 1, y, z + 1 - 0.015625D, minU, minV);
                tess.addVertexWithUV(x + 0, y, z + 1 - 0.015625D, minU, maxV);
                tess.addVertexWithUV(x + 0, slopeHeight, z + 1 - 0.015625D, maxU, maxV);

                tess.setColorOpaque_F(luminance, luminance, luminance);
                tess.addVertexWithUV(x + 1, slopeHeight, z + 1 - 0.015625D, maxU, minV + shroudVOffset);
                tess.addVertexWithUV(x + 1, y, z + 1 - 0.015625D, minU, minV + shroudVOffset);
                tess.addVertexWithUV(x + 0, y, z + 1 - 0.015625D, minU, maxV + shroudVOffset);
                tess.addVertexWithUV(x + 0, slopeHeight, z + 1 - 0.015625D, maxU, maxV + shroudVOffset);
            }
        }

        return true;
    }

    private bool RenderBlockMinecartTrack(BlockRail rail, int x, int y, int z)
    {
        Tessellator tess = _tess;
        int metadata = _blockAccess.getBlockMeta(x, y, z);
        int textureId = rail.getTexture(0, metadata);

        if (_overrideBlockTexture >= 0)
        {
            textureId = _overrideBlockTexture;
        }

        // Powered/Detector rails use bit 3 for state, but the first 8 shapes are identical
        if (rail.isAlwaysStraight())
        {
            metadata &= 7;
        }

        float luminance = rail.getLuminance(_blockAccess, x, y, z);
        tess.setColorOpaque_F(luminance, luminance, luminance);

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = texU / 256.0D;
        double maxU = (texU + 15.99D) / 256.0D;
        double minV = texV / 256.0D;
        double maxV = (texV + 15.99D) / 256.0D;

        float verticalOffset = 1.0F / 16.0F; // 1 pixel above the ground

        // Default vertex positions (flat square)
        float x1 = x + 1, x2 = x + 1, x3 = x + 0, x4 = x + 0;
        float z1 = z + 0, z2 = z + 1, z3 = z + 1, z4 = z + 0;

        float h1 = y + verticalOffset;
        float h2 = y + verticalOffset;
        float h3 = y + verticalOffset;
        float h4 = y + verticalOffset;

        // Handle coordinate swapping for curves and orientation
        if (metadata != 1 && metadata != 2 && metadata != 3 && metadata != 7)
        {
            if (metadata == 8)
            {
                x2 = x + 0;
                x1 = x2;
                x4 = x + 1;
                x3 = x4;
                z4 = z + 1;
                z1 = z4;
                z3 = z + 0;
                z2 = z3;
            }
            else if (metadata == 9)
            {
                x4 = x + 0;
                x1 = x4;
                x3 = x + 1;
                x2 = x3;
                z2 = z + 0;
                z1 = z2;
                z4 = z + 1;
                z3 = z4;
            }
        }
        else
        {
            x4 = x + 1;
            x1 = x4;
            x3 = x + 0;
            x2 = x3;
            z2 = z + 1;
            z1 = z2;
            z4 = z + 0;
            z3 = z4;
        }

        // Handle Slopes (ascending heights)
        if (metadata != 2 && metadata != 4)
        {
            if (metadata == 3 || metadata == 5)
            {
                h2++;
                h3++; // Sloping up North/South
            }
        }
        else
        {
            h1++;
            h4++; // Sloping up West/East
        }

        // Render both sides of the quad so it's visible from below (for glass/transparent floors)
        tess.addVertexWithUV(x1, h1, z1, maxU, minV);
        tess.addVertexWithUV(x2, h2, z2, maxU, maxV);
        tess.addVertexWithUV(x3, h3, z3, minU, maxV);
        tess.addVertexWithUV(x4, h4, z4, minU, minV);

        tess.addVertexWithUV(x4, h4, z4, minU, minV);
        tess.addVertexWithUV(x3, h3, z3, minU, maxV);
        tess.addVertexWithUV(x2, h2, z2, maxU, maxV);
        tess.addVertexWithUV(x1, h1, z1, maxU, minV);

        return true;
    }

    private bool RenderBlockLadder(Block block, int x, int y, int z)
    {
        Tessellator tess = _tess;

        int textureId = block.getTexture(0);
        if (_overrideBlockTexture >= 0)
        {
            textureId = _overrideBlockTexture;
        }

        float luminance = block.getLuminance(_blockAccess, x, y, z);
        tess.setColorOpaque_F(luminance, luminance, luminance);

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = texU / 256.0D;
        double maxU = (texU + 15.99D) / 256.0D;
        double minV = texV / 256.0D;
        double maxV = (texV + 15.99D) / 256.0D;

        int metadata = _blockAccess.getBlockMeta(x, y, z);
        double offset = 0.05D;

        if (metadata == 5)
        {
            tess.addVertexWithUV(x + offset, y + 1.0D, z + 1.0D, minU, minV);
            tess.addVertexWithUV(x + offset, y + 0.0D, z + 1.0D, minU, maxV);
            tess.addVertexWithUV(x + offset, y + 0.0D, z + 0.0D, maxU, maxV);
            tess.addVertexWithUV(x + offset, y + 1.0D, z + 0.0D, maxU, minV);
        }
        else if (metadata == 4)
        {
            tess.addVertexWithUV(x + 1.0D - offset, y + 0.0D, z + 1.0D, maxU, maxV);
            tess.addVertexWithUV(x + 1.0D - offset, y + 1.0D, z + 1.0D, maxU, minV);
            tess.addVertexWithUV(x + 1.0D - offset, y + 1.0D, z + 0.0D, minU, minV);
            tess.addVertexWithUV(x + 1.0D - offset, y + 0.0D, z + 0.0D, minU, maxV);
        }
        else if (metadata == 3)
        {
            tess.addVertexWithUV(x + 1.0D, y + 0.0D, z + offset, maxU, maxV);
            tess.addVertexWithUV(x + 1.0D, y + 1.0D, z + offset, maxU, minV);
            tess.addVertexWithUV(x + 0.0D, y + 1.0D, z + offset, minU, minV);
            tess.addVertexWithUV(x + 0.0D, y + 0.0D, z + offset, minU, maxV);
        }
        else if (metadata == 2)
        {
            tess.addVertexWithUV(x + 1.0D, y + 1.0D, z + 1.0D - offset, minU, minV);
            tess.addVertexWithUV(x + 1.0D, y + 0.0D, z + 1.0D - offset, minU, maxV);
            tess.addVertexWithUV(x + 0.0D, y + 0.0D, z + 1.0D - offset, maxU, maxV);
            tess.addVertexWithUV(x + 0.0D, y + 1.0D, z + 1.0D - offset, maxU, minV);
        }

        return true;
    }

    private bool RenderBlockReed(Block block, int x, int y, int z)
    {
        Tessellator tess = _tess;

        float luminance = block.getLuminance(_blockAccess, x, y, z);
        int colorMultiplier = block.getColorMultiplier(_blockAccess, x, y, z);
        float r = (colorMultiplier >> 16 & 255) / 255.0F;
        float g = (colorMultiplier >> 8 & 255) / 255.0F;
        float b = (colorMultiplier & 255) / 255.0F;

        tess.setColorOpaque_F(luminance * r, luminance * g, luminance * b);

        double renderX = x;
        double renderY = y;
        double renderZ = z;

        if (block == Block.Grass)
        {
            long hash = x * 3129871L ^ z * 116129781L ^ y;
            hash = hash * hash * 42317861L + hash * 11L;

            renderX += (((hash >> 16 & 15L) / 15.0F) - 0.5D) * 0.5D;
            renderY += (((hash >> 20 & 15L) / 15.0F) - 1.0D) * 0.2D;
            renderZ += (((hash >> 24 & 15L) / 15.0F) - 0.5D) * 0.5D;
        }

        RenderCrossedSquares(block, _blockAccess.getBlockMeta(x, y, z), renderX, renderY, renderZ);
        return true;
    }

    private bool RenderBlockCrops(Block block, int x, int y, int z)
    {
        Tessellator tess = _tess;
        float luminance = block.getLuminance(_blockAccess, x, y, z);
        tess.setColorOpaque_F(luminance, luminance, luminance);

        int metadata = _blockAccess.getBlockMeta(x, y, z);

        double yOffset = y - (1.0D / 16.0D);
        RenderCropQuads(block, metadata, x, yOffset, z);
        return true;
    }

    private void RenderTorchAtAngle(Block block, double x, double y, double z, double tiltX, double tiltZ)
    {
        Tessellator tess = _tess;

        int textureId = block.getTexture(0);
        if (_overrideBlockTexture >= 0)
        {
            textureId = _overrideBlockTexture;
        }

        // Standard UV boundaries for the sides of the torch
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        float minU = texU / 256.0F;
        float maxU = (texU + 15.99F) / 256.0F;
        float minV = texV / 256.0F;
        float maxV = (texV + 15.99F) / 256.0F;

        // Custom UV boundaries specifically for the TOP face of the torch (the burning coal part)
        // 1.75 / 64 = 7 / 256. 9 / 256. This targets a specific 2x2 pixel square on the texture.
        double topMinU = minU + 7.0D / 256.0D;
        double topMinV = minV + 6.0D / 256.0D;
        double topMaxU = minU + 9.0D / 256.0D;
        double topMaxV = minV + 8.0D / 256.0D; // 1.0D / 32.0D = 8.0D / 256.0D

        // Shift origin to the center of the block for easier rotation/tilting math
        double centerX = x + 0.5D;
        double centerZ = z + 0.5D;

        double leftX = centerX - 0.5D;
        double rightX = centerX + 0.5D;
        double frontZ = centerZ - 0.5D;
        double backZ = centerZ + 0.5D;

        // Torch dimensions
        double radius = 1.0D / 16.0D; // 1 pixel thick from the center
        double height = 0.625D; // 10 pixels tall (10 / 16)

        // TOP FACE (The burning tip)
        double tipOffsetBase = 1.0D - height; // How far down from the top of the block space the tip sits
        double tipX = centerX + tiltX * tipOffsetBase;
        double tipZ = centerZ + tiltZ * tipOffsetBase;

        tess.addVertexWithUV(tipX - radius, y + height, tipZ - radius, topMinU, topMinV);
        tess.addVertexWithUV(tipX - radius, y + height, tipZ + radius, topMinU, topMaxV);
        tess.addVertexWithUV(tipX + radius, y + height, tipZ + radius, topMaxU, topMaxV);
        tess.addVertexWithUV(tipX + radius, y + height, tipZ - radius, topMaxU, topMinV);

        // SIDE FACES
        // The top vertices stay near the center, while the bottom vertices are shifted by tiltX and tiltZ

        // West Face
        tess.addVertexWithUV(centerX - radius, y + 1.0D, frontZ, minU, minV);
        tess.addVertexWithUV(centerX - radius + tiltX, y + 0.0D, frontZ + tiltZ, minU, maxV);
        tess.addVertexWithUV(centerX - radius + tiltX, y + 0.0D, backZ + tiltZ, maxU, maxV);
        tess.addVertexWithUV(centerX - radius, y + 1.0D, backZ, maxU, minV);

        // East Face
        tess.addVertexWithUV(centerX + radius, y + 1.0D, backZ, minU, minV);
        tess.addVertexWithUV(centerX + radius + tiltX, y + 0.0D, backZ + tiltZ, minU, maxV);
        tess.addVertexWithUV(centerX + radius + tiltX, y + 0.0D, frontZ + tiltZ, maxU, maxV);
        tess.addVertexWithUV(centerX + radius, y + 1.0D, frontZ, maxU, minV);

        // North Face
        tess.addVertexWithUV(leftX, y + 1.0D, centerZ + radius, minU, minV);
        tess.addVertexWithUV(leftX + tiltX, y + 0.0D, centerZ + radius + tiltZ, minU, maxV);
        tess.addVertexWithUV(rightX + tiltX, y + 0.0D, centerZ + radius + tiltZ, maxU, maxV);
        tess.addVertexWithUV(rightX, y + 1.0D, centerZ + radius, maxU, minV);

        // South Face
        tess.addVertexWithUV(rightX, y + 1.0D, centerZ - radius, minU, minV);
        tess.addVertexWithUV(rightX + tiltX, y + 0.0D, centerZ - radius + tiltZ, minU, maxV);
        tess.addVertexWithUV(leftX + tiltX, y + 0.0D, centerZ - radius + tiltZ, maxU, maxV);
        tess.addVertexWithUV(leftX, y + 1.0D, centerZ - radius, maxU, minV);
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

    /// <summary>
    /// Renders dynamic fluid blocks (Water/Lava), calculating smooth slopes and flowing texture UVs.
    /// </summary>
    private bool RenderBlockFluids(Block block, int x, int y, int z)
    {
        Tessellator tess = _tess;
        Box bounds = block.BoundingBox;

        // Base fluid color tint (e.g., biome water color)
        int colorMultiplier = block.getColorMultiplier(_blockAccess, x, y, z);
        float tintR = (colorMultiplier >> 16 & 255) / 255.0F;
        float tintG = (colorMultiplier >> 8 & 255) / 255.0F;
        float tintB = (colorMultiplier & 255) / 255.0F;

        // Determine which faces are actually visible to the player
        bool isTopVisible = block.isSideVisible(_blockAccess, x, y + 1, z, 1);
        bool isBottomVisible = block.isSideVisible(_blockAccess, x, y - 1, z, 0);
        bool[] sideVisible =
        [
            block.isSideVisible(_blockAccess, x, y, z - 1, 2), // North
            block.isSideVisible(_blockAccess, x, y, z + 1, 3), // South
            block.isSideVisible(_blockAccess, x - 1, y, z, 4), // West
            block.isSideVisible(_blockAccess, x + 1, y, z, 5) // East
        ];

        // Fast exit if completely surrounded
        if (!isTopVisible && !isBottomVisible && !sideVisible[0] && !sideVisible[1] && !sideVisible[2] &&
            !sideVisible[3])
        {
            return false;
        }

        bool hasRendered = false;

        // Directional shading
        float lightBottom = 0.5F;
        float lightTop = 1.0F;
        float lightZ = 0.8F; // North/South
        float lightX = 0.6F; // East/West

        Material material = block.material;
        int meta = _blockAccess.getBlockMeta(x, y, z);

        // Calculate the height of the fluid at each of the 4 corners of this block
        float heightNw = GetFluidVertexHeight(x, y, z, material);
        float heightSw = GetFluidVertexHeight(x, y, z + 1, material);
        float heightSe = GetFluidVertexHeight(x + 1, y, z + 1, material);
        float heightNe = GetFluidVertexHeight(x + 1, y, z, material);

        // TOP FACE (Flowing Surface)
        if (_renderAllFaces || isTopVisible)
        {
            hasRendered = true;
            int textureId = block.getTexture(1, meta);
            float flowAngle = (float)BlockFluid.getFlowingAngle(_blockAccess, x, y, z, material);

            // If flowing, switch to the flowing texture variant
            if (flowAngle > -999.0F)
            {
                textureId = block.getTexture(2, meta);
            }

            int texU = (textureId & 15) << 4;
            int texV = textureId & 240;
            double centerU = (texU + 8.0D) / 256.0D;
            double centerV = (texV + 8.0D) / 256.0D;

            // If completely still, use standard flat UVs
            if (flowAngle < -999.0F)
            {
                flowAngle = 0.0F;
            }
            else
            {
                // Shift UV center for flowing animation
                centerU = (texU + 16) / 256.0F;
                centerV = (texV + 16) / 256.0F;
            }

            // Calculate rotational offsets for the UVs to make the texture flow in the correct direction
            float sinAngle = MathHelper.Sin(flowAngle) * 8.0F / 256.0F;
            float cosAngle = MathHelper.Cos(flowAngle) * 8.0F / 256.0F;

            float luminance = block.getLuminance(_blockAccess, x, y, z);
            tess.setColorOpaque_F(lightTop * luminance * tintR, lightTop * luminance * tintG,
                lightTop * luminance * tintB);

            // Draw top face with dynamic heights and rotated UVs
            tess.addVertexWithUV(x + 0, y + heightNw, z + 0, centerU - cosAngle - sinAngle,
                centerV - cosAngle + sinAngle);
            tess.addVertexWithUV(x + 0, y + heightSw, z + 1, centerU - cosAngle + sinAngle,
                centerV + cosAngle + sinAngle);
            tess.addVertexWithUV(x + 1, y + heightSe, z + 1, centerU + cosAngle + sinAngle,
                centerV + cosAngle - sinAngle);
            tess.addVertexWithUV(x + 1, y + heightNe, z + 0, centerU + cosAngle - sinAngle,
                centerV - cosAngle - sinAngle);
        }

        // BOTTOM FACE
        if (_renderAllFaces || isBottomVisible)
        {
            float luminance = block.getLuminance(_blockAccess, x, y - 1, z);
            tess.setColorOpaque_F(lightBottom * luminance, lightBottom * luminance, lightBottom * luminance);
            Helper.RenderBottomFace(block, x, y, z, block.getTexture(0));
            hasRendered = true;
        }

        // SIDE FACES (North, South, West, East)
        for (int side = 0; side < 4; ++side)
        {
            int adjX = x;
            int adjZ = z;

            if (side == 0) adjZ = z - 1; // North
            if (side == 1) adjZ = z + 1; // South
            if (side == 2) adjX = x - 1; // West
            if (side == 3) adjX = x + 1; // East

            int textureId = block.getTexture(side + 2, meta);
            int texU = (textureId & 15) << 4;
            int texV = textureId & 240;

            if (_renderAllFaces || sideVisible[side])
            {
                float h1, h2; // Top corner heights for this face
                float x1, x2; // X coordinates
                float z1, z2; // Z coordinates

                if (side == 0) // North
                {
                    h1 = heightNw;
                    h2 = heightNe;
                    x1 = x;
                    x2 = x + 1;
                    z1 = z;
                    z2 = z;
                }
                else if (side == 1) // South
                {
                    h1 = heightSe;
                    h2 = heightSw;
                    x1 = x + 1;
                    x2 = x;
                    z1 = z + 1;
                    z2 = z + 1;
                }
                else if (side == 2) // West
                {
                    h1 = heightSw;
                    h2 = heightNw;
                    x1 = x;
                    x2 = x;
                    z1 = z + 1;
                    z2 = z;
                }
                else // East
                {
                    h1 = heightNe;
                    h2 = heightSe;
                    x1 = x + 1;
                    x2 = x + 1;
                    z1 = z;
                    z2 = z + 1;
                }

                hasRendered = true;

                // Crop the UVs vertically so the texture doesn't stretch on short flowing water blocks
                double minU = (texU + 0) / 256.0F;
                double maxU = (texU + 16 - 0.01D) / 256.0D;
                double minV1 = (texV + (1.0F - h1) * 16.0F) / 256.0F; // UV height match for corner 1
                double minV2 = (texV + (1.0F - h2) * 16.0F) / 256.0F; // UV height match for corner 2
                double maxV = (texV + 16 - 0.01D) / 256.0D;

                float luminance = block.getLuminance(_blockAccess, adjX, y, adjZ);
                float shadow = (side < 2) ? lightZ : lightX;
                luminance *= shadow;

                tess.setColorOpaque_F(lightTop * luminance * tintR, lightTop * luminance * tintG,
                    lightTop * luminance * tintB);

                // Draw the side face matching the sloped top corners
                tess.addVertexWithUV(x1, y + h1, z1, minU, minV1);
                tess.addVertexWithUV(x2, y + h2, z2, maxU, minV2);
                tess.addVertexWithUV(x2, y + 0, z2, maxU, maxV);
                tess.addVertexWithUV(x1, y + 0, z1, minU, maxV);
            }
        }

        // Reset bounding box state
        bounds.MinY = 0.0D;
        bounds.MaxY = 1.0D;
        return hasRendered;
    }

    private float GetFluidVertexHeight(int x, int y, int z, Material material)
    {
        int totalWeight = 0;
        float totalDepth = 0.0F;

        // Iterate through the 2x2 grid sharing this vertex: (x, z), (x-1, z), (x, z-1), (x-1, z-1)
        for (int i = 0; i < 4; ++i)
        {
            int checkX = x - (i & 1);
            int checkZ = z - (i >> 1 & 1);

            // If there is fluid directly above any of the 4 blocks, the corner must be completely full (height 1.0)
            if (_blockAccess.getMaterial(checkX, y + 1, checkZ) == material)
            {
                return 1.0F;
            }

            Material neighborMaterial = _blockAccess.getMaterial(checkX, y, checkZ);

            if (neighborMaterial != material)
            {
                // If the neighbor is air or a non-solid block, it contributes "full depth" (pulls the water level down to 0)
                if (!neighborMaterial.IsSolid)
                {
                    ++totalDepth;
                    ++totalWeight;
                }
            }
            else
            {
                int neighborMeta = _blockAccess.getBlockMeta(checkX, y, checkZ);
                float fluidDepth = BlockFluid.getFluidHeightFromMeta(neighborMeta);

                // Meta >= 8 (falling fluid) or Meta == 0 (source block)
                if (neighborMeta >= 8 || neighborMeta == 0)
                {
                    // Source blocks and falling columns get 10x the "weight" in the average,
                    // heavily anchoring the fluid corner to their height.
                    totalDepth += fluidDepth * 10.0F;
                    totalWeight += 10;
                }

                totalDepth += fluidDepth;
                ++totalWeight;
            }
        }

        // Depth is measured from the top down. Subtract from 1.0 to get height from bottom up.
        return 1.0F - totalDepth / totalWeight;
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

    private bool RenderStandardBlock(Block block, int x, int y, int z)
    {
        _enableAo = true;
        bool hasRendered = false;
        Box bounds = _useOverrideBoundingBox ? _overrideBoundingBox : block.BoundingBox;

        // 1. Base Colors
        int colorMultiplier = block.getColorMultiplier(_blockAccess, x, y, z);
        float r = (colorMultiplier >> 16 & 255) / 255.0F;
        float g = (colorMultiplier >> 8 & 255) / 255.0F;
        float b = (colorMultiplier & 255) / 255.0F;

        bool tintBottom = true, tintTop = true, tintEast = true, tintWest = true, tintNorth = true, tintSouth = true;
        if (block.textureId == 3 || _overrideBlockTexture >= 0)
        {
            tintBottom = tintEast = tintWest = tintNorth = tintSouth = false;
        }

        // Cache luminances for the 6 direct neighbors
        float lXn = block.getLuminance(_blockAccess, x - 1, y, z);
        float lXp = block.getLuminance(_blockAccess, x + 1, y, z);
        float lYn = block.getLuminance(_blockAccess, x, y - 1, z);
        float lYp = block.getLuminance(_blockAccess, x, y + 1, z);
        float lZn = block.getLuminance(_blockAccess, x, y, z - 1);
        float lZp = block.getLuminance(_blockAccess, x, y, z + 1);

        // Cache opacity for the 12 edges (Used for AO shadowing)
        // Format: isOpaque[Axis][Direction][Side]
        bool opXnYn = !Block.BlocksAllowVision[_blockAccess.getBlockId(x - 1, y - 1, z)];
        bool opXnYp = !Block.BlocksAllowVision[_blockAccess.getBlockId(x - 1, y + 1, z)];
        bool opXpYn = !Block.BlocksAllowVision[_blockAccess.getBlockId(x + 1, y - 1, z)];
        bool opXpYp = !Block.BlocksAllowVision[_blockAccess.getBlockId(x + 1, y + 1, z)];
        bool opXnZn = !Block.BlocksAllowVision[_blockAccess.getBlockId(x - 1, y, z - 1)];
        bool opXnZp = !Block.BlocksAllowVision[_blockAccess.getBlockId(x - 1, y, z + 1)];
        bool opXpZn = !Block.BlocksAllowVision[_blockAccess.getBlockId(x + 1, y, z - 1)];
        bool opXpZp = !Block.BlocksAllowVision[_blockAccess.getBlockId(x + 1, y, z + 1)];
        bool opYnZn = !Block.BlocksAllowVision[_blockAccess.getBlockId(x, y - 1, z - 1)];
        bool opYnZp = !Block.BlocksAllowVision[_blockAccess.getBlockId(x, y - 1, z + 1)];
        bool opYpZn = !Block.BlocksAllowVision[_blockAccess.getBlockId(x, y + 1, z - 1)];
        bool opYpZp = !Block.BlocksAllowVision[_blockAccess.getBlockId(x, y + 1, z + 1)];

        float v0, v1, v2, v3;

        // ==========================================
        // BOTTOM FACE (Y - 1)
        // ==========================================
        if (_renderAllFaces || bounds.MinY > 0.0D || block.isSideVisible(_blockAccess, x, y - 1, z, 0))
        {
            if (_aoBlendMode <= 0) v0 = v1 = v2 = v3 = lYn;
            else
            {
                float n = block.getLuminance(_blockAccess, x, y - 1, z - 1);
                float s = block.getLuminance(_blockAccess, x, y - 1, z + 1);
                float w = block.getLuminance(_blockAccess, x - 1, y - 1, z);
                float e = block.getLuminance(_blockAccess, x + 1, y - 1, z);
                float nw = (opXnZn || opYnZn) ? w : block.getLuminance(_blockAccess, x - 1, y - 1, z - 1);
                float sw = (opXnZp || opYnZp) ? w : block.getLuminance(_blockAccess, x - 1, y - 1, z + 1);
                float ne = (opXpZn || opYnZn) ? e : block.getLuminance(_blockAccess, x + 1, y - 1, z - 1);
                float se = (opXpZp || opYnZp) ? e : block.getLuminance(_blockAccess, x + 1, y - 1, z + 1);
                v0 = (sw + w + s + lYn) / 4.0F; // minX, maxZ
                v1 = (w + nw + lYn + n) / 4.0F; // minX, minZ
                v2 = (lYn + n + e + ne) / 4.0F; // maxX, minZ
                v3 = (s + lYn + se + e) / 4.0F; // maxX, maxZ
            }

            AssignVertexColors(v0, v1, v2, v3, r, g, b, 0.5F, tintBottom);
            Helper.RenderBottomFace(block, x, y, z, block.getTextureId(_blockAccess, x, y, z, 0));
            hasRendered = true;
        }

        // ==========================================
        // TOP FACE (Y + 1)
        // ==========================================
        if (_renderAllFaces || bounds.MaxY < 1.0D || block.isSideVisible(_blockAccess, x, y + 1, z, 1))
        {
            if (_aoBlendMode <= 0) v0 = v1 = v2 = v3 = lYp;
            else
            {
                float n = block.getLuminance(_blockAccess, x, y + 1, z - 1);
                float s = block.getLuminance(_blockAccess, x, y + 1, z + 1);
                float w = block.getLuminance(_blockAccess, x - 1, y + 1, z);
                float e = block.getLuminance(_blockAccess, x + 1, y + 1, z);
                float nw = (opXnYp || opYpZn) ? w : block.getLuminance(_blockAccess, x - 1, y + 1, z - 1);
                float sw = (opXnYp || opYpZp) ? w : block.getLuminance(_blockAccess, x - 1, y + 1, z + 1);
                float ne = (opXpYp || opYpZn) ? e : block.getLuminance(_blockAccess, x + 1, y + 1, z - 1);
                float se = (opXpYp || opYpZp) ? e : block.getLuminance(_blockAccess, x + 1, y + 1, z + 1);
                v0 = (s + lYp + se + e) / 4.0F; // maxX, maxZ
                v1 = (lYp + n + e + ne) / 4.0F; // maxX, minZ
                v2 = (w + nw + lYp + n) / 4.0F; // minX, minZ
                v3 = (sw + w + s + lYp) / 4.0F; // minX, maxZ
            }

            AssignVertexColors(v0, v1, v2, v3, r, g, b, 1.0F, tintTop);
            Helper.RenderTopFace(block, x, y, z, block.getTextureId(_blockAccess, x, y, z, 1));
            hasRendered = true;
        }

        // ==========================================
        // EAST FACE (Z - 1)
        // ==========================================
        if (_renderAllFaces || bounds.MinZ > 0.0D || block.isSideVisible(_blockAccess, x, y, z - 1, 2))
        {
            if (_aoBlendMode <= 0) v0 = v1 = v2 = v3 = lZn;
            else
            {
                float u = block.getLuminance(_blockAccess, x, y + 1, z - 1);
                float d = block.getLuminance(_blockAccess, x, y - 1, z - 1);
                float w = block.getLuminance(_blockAccess, x - 1, y, z - 1);
                float e = block.getLuminance(_blockAccess, x + 1, y, z - 1);
                float uw = (opXnZn || opYpZn) ? w : block.getLuminance(_blockAccess, x - 1, y + 1, z - 1);
                float dw = (opXnZn || opYnZn) ? w : block.getLuminance(_blockAccess, x - 1, y - 1, z - 1);
                float ue = (opXpZn || opYpZn) ? e : block.getLuminance(_blockAccess, x + 1, y + 1, z - 1);
                float de = (opXpZn || opYnZn) ? e : block.getLuminance(_blockAccess, x + 1, y - 1, z - 1);
                v0 = (w + uw + lZn + u) / 4.0F;
                v1 = (lZn + u + e + ue) / 4.0F;
                v2 = (d + lZn + de + e) / 4.0F;
                v3 = (dw + w + d + lZn) / 4.0F;
            }

            AssignVertexColors(v0, v1, v2, v3, r, g, b, 0.8F, tintEast);
            Helper.RenderEastFace(block, x, y, z, block.getTextureId(_blockAccess, x, y, z, 2));
            hasRendered = true;
        }

        // ==========================================
        // WEST FACE (Z + 1)
        // ==========================================
        if (_renderAllFaces || bounds.MaxZ < 1.0D || block.isSideVisible(_blockAccess, x, y, z + 1, 3))
        {
            if (_aoBlendMode <= 0) v0 = v1 = v2 = v3 = lZp;
            else
            {
                float u = block.getLuminance(_blockAccess, x, y + 1, z + 1);
                float d = block.getLuminance(_blockAccess, x, y - 1, z + 1);
                float w = block.getLuminance(_blockAccess, x - 1, y, z + 1);
                float e = block.getLuminance(_blockAccess, x + 1, y, z + 1);
                float uw = (opXnZp || opYpZp) ? w : block.getLuminance(_blockAccess, x - 1, y + 1, z + 1);
                float dw = (opXnZp || opYnZp) ? w : block.getLuminance(_blockAccess, x - 1, y - 1, z + 1);
                float ue = (opXpZp || opYpZp) ? e : block.getLuminance(_blockAccess, x + 1, y + 1, z + 1);
                float de = (opXpZp || opYnZp) ? e : block.getLuminance(_blockAccess, x + 1, y - 1, z + 1);
                v0 = (w + uw + lZp + u) / 4.0F;
                v1 = (dw + w + d + lZp) / 4.0F;
                v2 = (d + lZp + de + e) / 4.0F;
                v3 = (lZp + u + e + ue) / 4.0F;
            }

            AssignVertexColors(v0, v1, v2, v3, r, g, b, 0.8F, tintWest);
            Helper.RenderWestFace(block, x, y, z, block.getTextureId(_blockAccess, x, y, z, 3));
            hasRendered = true;
        }

        // ==========================================
        // NORTH FACE (X - 1)
        // ==========================================
        if (_renderAllFaces || bounds.MinX > 0.0D || block.isSideVisible(_blockAccess, x - 1, y, z, 4))
        {
            if (_aoBlendMode <= 0) v0 = v1 = v2 = v3 = lXn;
            else
            {
                float u = block.getLuminance(_blockAccess, x - 1, y + 1, z);
                float d = block.getLuminance(_blockAccess, x - 1, y - 1, z);
                float n = block.getLuminance(_blockAccess, x - 1, y, z - 1);
                float s = block.getLuminance(_blockAccess, x - 1, y, z + 1);
                float un = (opXnZn || opXnYp) ? n : block.getLuminance(_blockAccess, x - 1, y + 1, z - 1);
                float dn = (opXnZn || opXnYn) ? n : block.getLuminance(_blockAccess, x - 1, y - 1, z - 1);
                float us = (opXnZp || opXnYp) ? s : block.getLuminance(_blockAccess, x - 1, y + 1, z + 1);
                float ds = (opXnZp || opXnYn) ? s : block.getLuminance(_blockAccess, x - 1, y - 1, z + 1);
                v0 = (u + us + lXn + s) / 4.0F;
                v1 = (u + un + n + lXn) / 4.0F;
                v2 = (n + lXn + dn + d) / 4.0F;
                v3 = (d + ds + lXn + s) / 4.0F;
            }

            AssignVertexColors(v0, v1, v2, v3, r, g, b, 0.6F, tintNorth);
            Helper.RenderNorthFace(block, x, y, z, block.getTextureId(_blockAccess, x, y, z, 4));
            hasRendered = true;
        }

        // ==========================================
        // SOUTH FACE (X + 1)
        // ==========================================
        if (_renderAllFaces || bounds.MaxX < 1.0D || block.isSideVisible(_blockAccess, x + 1, y, z, 5))
        {
            if (_aoBlendMode <= 0) v0 = v1 = v2 = v3 = lXp;
            else
            {
                float u = block.getLuminance(_blockAccess, x + 1, y + 1, z);
                float d = block.getLuminance(_blockAccess, x + 1, y - 1, z);
                float n = block.getLuminance(_blockAccess, x + 1, y, z - 1);
                float s = block.getLuminance(_blockAccess, x + 1, y, z + 1);
                float un = (opXpZn || opXpYp) ? n : block.getLuminance(_blockAccess, x + 1, y + 1, z - 1);
                float dn = (opXpZn || opXpYn) ? n : block.getLuminance(_blockAccess, x + 1, y - 1, z - 1);
                float us = (opXpZp || opXpYp) ? s : block.getLuminance(_blockAccess, x + 1, y + 1, z + 1);
                float ds = (opXpZp || opXpYn) ? s : block.getLuminance(_blockAccess, x + 1, y - 1, z + 1);
                v0 = (d + ds + lXp + s) / 4.0F;
                v1 = (n + lXp + dn + d) / 4.0F;
                v2 = (u + un + n + lXp) / 4.0F;
                v3 = (u + us + lXp + s) / 4.0F;
            }

            AssignVertexColors(v0, v1, v2, v3, r, g, b, 0.6F, tintSouth);
            RenderSouthFace(block, x, y, z, block.getTextureId(_blockAccess, x, y, z, 5));
            hasRendered = true;
        }

        _enableAo = false;
        return hasRendered;
    }

    private void AssignVertexColors(float v0, float v1, float v2, float v3, float r, float g, float b, float faceShadow,
        bool tint)
    {
        float tr = (tint ? r : 1.0F) * faceShadow;
        float tg = (tint ? g : 1.0F) * faceShadow;
        float tb = (tint ? b : 1.0F) * faceShadow;

        _colorRedTopLeft = tr * v0;
        _colorGreenTopLeft = tg * v0;
        _colorBlueTopLeft = tb * v0;
        _colorRedBottomLeft = tr * v1;
        _colorGreenBottomLeft = tg * v1;
        _colorBlueBottomLeft = tb * v1;
        _colorRedBottomRight = tr * v2;
        _colorGreenBottomRight = tg * v2;
        _colorBlueBottomRight = tb * v2;
        _colorRedTopRight = tr * v3;
        _colorGreenTopRight = tg * v3;
        _colorBlueTopRight = tb * v3;
    }


    private bool RenderBlockCactus(Block block, int x, int y, int z)
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

    private bool RenderBlockFence(Block block, int x, int y, int z)
    {
        bool hasRendered = true;

        // 1. Render the central vertical post
        float postMin = 6.0F / 16.0F;
        float postMax = 10.0F / 16.0F;
        SetOverrideBoundingBox(postMin, 0.0F, postMin, postMax, 1.0F, postMax);
        RenderStandardBlock(block, x, y, z);

        // Check for adjacent fences
        bool connectsWest = _blockAccess.getBlockId(x - 1, y, z) == block.id;
        bool connectsEast = _blockAccess.getBlockId(x + 1, y, z) == block.id;
        bool connectsNorth = _blockAccess.getBlockId(x, y, z - 1) == block.id;
        bool connectsSouth = _blockAccess.getBlockId(x, y, z + 1) == block.id;

        bool connectsX = connectsWest || connectsEast;
        bool connectsZ = connectsNorth || connectsSouth;

        // If the fence is completely isolated, default to drawing small stubs along the X-axis
        if (!connectsX && !connectsZ)
        {
            connectsX = true;
        }

        // Base depth/thickness for the horizontal connecting bars
        float barDepthMin = 7.0F / 16.0F;
        float barDepthMax = 9.0F / 16.0F;

        // Determine how far the bars extend based on neighbor connections
        // If connecting, stretch to the edge (0.0 or 1.0). Otherwise, stay near the post.
        float barMinX = connectsWest ? 0.0F : barDepthMin;
        float barMaxX = connectsEast ? 1.0F : barDepthMax;
        float barMinZ = connectsNorth ? 0.0F : barDepthMin;
        float barMaxZ = connectsSouth ? 1.0F : barDepthMax;

        // 2. Render Top Connecting Bars
        float topBarMinY = 12.0F / 16.0F;
        float topBarMaxY = 15.0F / 16.0F;

        if (connectsX)
        {
            SetOverrideBoundingBox(barMinX, topBarMinY, barDepthMin, barMaxX, topBarMaxY, barDepthMax);
            RenderStandardBlock(block, x, y, z);
        }

        if (connectsZ)
        {
            SetOverrideBoundingBox(barDepthMin, topBarMinY, barMinZ, barDepthMax, topBarMaxY, barMaxZ);
            RenderStandardBlock(block, x, y, z);
        }

        // 3. Render Bottom Connecting Bars
        float bottomBarMinY = 6.0F / 16.0F;
        float bottomBarMaxY = 9.0F / 16.0F;

        if (connectsX)
        {
            SetOverrideBoundingBox(barMinX, bottomBarMinY, barDepthMin, barMaxX, bottomBarMaxY, barDepthMax);
            RenderStandardBlock(block, x, y, z);
        }

        if (connectsZ)
        {
            SetOverrideBoundingBox(barDepthMin, bottomBarMinY, barMinZ, barDepthMax, bottomBarMaxY, barMaxZ);
            RenderStandardBlock(block, x, y, z);
        }

        // Reset bounding box state to prevent breaking the next block in the chunk
        SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);

        return hasRendered;
    }


    private bool RenderBlockStairs(Block block, int x, int y, int z)
    {
        bool hasRendered = false;
        int direction = _blockAccess.getBlockMeta(x, y, z);

        if (direction == 0) // Ascending East (Stairs face West)
        {
            // Lower step (West half)
            SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 0.5F, 0.5F, 1.0F);
            RenderStandardBlock(block, x, y, z);

            // Upper step (East half)
            SetOverrideBoundingBox(0.5F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
            RenderStandardBlock(block, x, y, z);

            hasRendered = true;
        }
        else if (direction == 1) // Ascending West (Stairs face East)
        {
            // Upper step (West half)
            SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 0.5F, 1.0F, 1.0F);
            RenderStandardBlock(block, x, y, z);

            // Lower step (East half)
            SetOverrideBoundingBox(0.5F, 0.0F, 0.0F, 1.0F, 0.5F, 1.0F);
            RenderStandardBlock(block, x, y, z);

            hasRendered = true;
        }
        else if (direction == 2) // Ascending South (Stairs face North)
        {
            // Lower step (North half)
            SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 0.5F, 0.5F);
            RenderStandardBlock(block, x, y, z);

            // Upper step (South half)
            SetOverrideBoundingBox(0.0F, 0.0F, 0.5F, 1.0F, 1.0F, 1.0F);
            RenderStandardBlock(block, x, y, z);

            hasRendered = true;
        }
        else if (direction == 3) // Ascending North (Stairs face South)
        {
            // Upper step (North half)
            SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.5F);
            RenderStandardBlock(block, x, y, z);

            // Lower step (South half)
            SetOverrideBoundingBox(0.0F, 0.0F, 0.5F, 1.0F, 0.5F, 1.0F);
            RenderStandardBlock(block, x, y, z);

            hasRendered = true;
        }

        SetOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);

        return hasRendered;
    }

    private bool RenderBlockDoor(Block block, int x, int y, int z)
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

    private void RenderSouthFace(Block block, double x, double y, double z, int textureId)
    {
        Tessellator tess = _tess;
        Box blockBb = _useOverrideBoundingBox ? _overrideBoundingBox : block.BoundingBox;

        if (_overrideBlockTexture >= 0)
        {
            textureId = _overrideBlockTexture;
        }

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        double minU = (texU + blockBb.MinZ * 16.0D) / 256.0D;
        double maxU = (texU + blockBb.MaxZ * 16.0D - 0.01D) / 256.0D;
        double minV = (texV + 16 - blockBb.MaxY * 16.0D) / 256.0D;
        double maxV = (texV + 16 - blockBb.MinY * 16.0D - 0.01D) / 256.0D;

        if (flipTexture)
        {
            (minU, maxU) = (maxU, minU);
        }

        if (blockBb.MinZ < 0.0D || blockBb.MaxZ > 1.0D)
        {
            minU = texU / 256.0D;
            maxU = (texU + 15.99D) / 256.0D;
        }

        if (blockBb.MinY < 0.0D || blockBb.MaxY > 1.0D)
        {
            minV = texV / 256.0D;
            maxV = (texV + 15.99D) / 256.0D;
        }

        double u1 = maxU, u2 = minU, v1 = minV, v2 = maxV;

        if (_uvRotateSouth == 2)
        {
            minU = (texU + blockBb.MinY * 16.0D) / 256.0D;
            minV = (texV + 16 - blockBb.MinZ * 16.0D) / 256.0D;
            maxU = (texU + blockBb.MaxY * 16.0D) / 256.0D;
            maxV = (texV + 16 - blockBb.MaxZ * 16.0D) / 256.0D;
            v1 = minV;
            v2 = maxV;
            u1 = minU;
            u2 = maxU;
            minV = maxV;
        }
        else if (_uvRotateSouth == 1)
        {
            minU = (texU + 16 - blockBb.MaxY * 16.0D) / 256.0D;
            minV = (texV + blockBb.MaxZ * 16.0D) / 256.0D;
            maxU = (texU + 16 - blockBb.MinY * 16.0D) / 256.0D;
            maxV = (texV + blockBb.MinZ * 16.0D) / 256.0D;
            u1 = maxU;
            u2 = minU;
            minU = maxU;
            v1 = maxV;
            v2 = minV;
        }
        else if (_uvRotateSouth == 3)
        {
            minU = (texU + 16 - blockBb.MinZ * 16.0D) / 256.0D;
            maxU = (texU + 16 - blockBb.MaxZ * 16.0D - 0.01D) / 256.0D;
            minV = (texV + blockBb.MaxY * 16.0D) / 256.0D;
            maxV = (texV + blockBb.MinY * 16.0D - 0.01D) / 256.0D;
            u1 = maxU;
            u2 = minU;
            v1 = minV;
            v2 = maxV;
        }

        double posX = x + blockBb.MaxX;
        double minY = y + blockBb.MinY;
        double maxY = y + blockBb.MaxY;
        double minZ = z + blockBb.MinZ;
        double maxZ = z + blockBb.MaxZ;

        if (_enableAo)
        {
            tess.setColorOpaque_F(_colorRedTopLeft, _colorGreenTopLeft, _colorBlueTopLeft);
            tess.addVertexWithUV(posX, minY, maxZ, u2, v2);
            tess.setColorOpaque_F(_colorRedBottomLeft, _colorGreenBottomLeft, _colorBlueBottomLeft);
            tess.addVertexWithUV(posX, minY, minZ, u1, v2);
            tess.setColorOpaque_F(_colorRedBottomRight, _colorGreenBottomRight, _colorBlueBottomRight);
            tess.addVertexWithUV(posX, maxY, minZ, u1, v1);
            tess.setColorOpaque_F(_colorRedTopRight, _colorGreenTopRight, _colorBlueTopRight);
            tess.addVertexWithUV(posX, maxY, maxZ, minU, minV);
        }
        else
        {
            tess.addVertexWithUV(posX, minY, maxZ, u2, v2);
            tess.addVertexWithUV(posX, minY, minZ, u1, v2);
            tess.addVertexWithUV(posX, maxY, minZ, u1, v1);
            tess.addVertexWithUV(posX, maxY, maxZ, minU, minV);
        }
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

    private static void RotateAroundX(ref Vector3D<double> vector, float angleRadians)
    {
        float cosAngle = MathHelper.Cos(angleRadians);
        float sinAngle = MathHelper.Sin(angleRadians);

        double rotatedY = vector.Y * cosAngle + vector.Z * sinAngle;
        double rotatedZ = vector.Z * cosAngle - vector.Y * sinAngle;

        vector.Y = rotatedY;
        vector.Z = rotatedZ;
    }

    private static void RotateAroundY(ref Vector3D<double> vector, float angleRadians)
    {
        float cosAngle = MathHelper.Cos(angleRadians);
        float sinAngle = MathHelper.Sin(angleRadians);

        double rotatedX = vector.X * cosAngle + vector.Z * sinAngle;
        double rotatedZ = vector.Z * cosAngle - vector.X * sinAngle;

        vector.X = rotatedX;
        vector.Z = rotatedZ;
    }
}
