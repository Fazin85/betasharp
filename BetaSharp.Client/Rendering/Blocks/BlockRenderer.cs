using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using Silk.NET.Maths;

namespace BetaSharp.Client.Rendering.Blocks;

public class BlockRenderer
{
    private readonly BlockView _blockAccess;
    private readonly Tessellator? _tessellator;
    private int _overrideBlockTexture = -1;

    
    private bool _flipTexture;
    private bool _renderAllFaces;
    private static readonly bool s_fancyGrass = true;
    public bool renderFromInside = true;
    private int _uvRotateEast;
    private int _uvRotateWest;
    private int _uvRotateSouth;
    private int _uvRotateNorth;
    private int _uvRotateTop;
    private int _uvRotateBottom;
    private bool _enableAO;
    private float _lightValueOwn;
    private float _aoLightValueXNeg;
    private float _aoLightValueYNeg;
    private float _aoLightValueZNeg;
    private float _aoLightValueXPos;
    private float _aoLightValueYPos;
    private float _aoLightValueZPos;
    private float _colorRedTopLeft_V;
    private float _colorRedBottomLeft_V;
    private float _colorRedBottomRight_V;
    private float _colorRedTopRight_V;
    private float _colorGreenTopLeft_V;
    private float _colorGreenBottomLeft_V;
    private float _colorGreenBottomRight_V;
    private float _colorGreenTopRight_V;
    private float _colorBlueTopLeft_V;
    private float _colorBlueBottomLeft_V;
    private float _colorBlueBottomRight_V;
    private float _colorBlueTopRight_V;
    private float _aoLightValueScratch1;
    private float _aoLightValueScratch2;
    private float _aoLightValueScratch3;
    private float _aoLightValueScratch4;
    private float _aoLightValueScratch5;
    private float _aoLightValueScratch6;
    private float _aoLightValueScratch7;
    private float _aoLightValueScratch8;
    private readonly int _aoBlendMode = 1;
    private float _colorRedTopLeft;
    private float _colorRedBottomLeft;
    private float _colorRedBottomRight;
    private float _colorRedTopRight;
    private float _colorGreenTopLeft;
    private float _colorGreenBottomLeft;
    private float _colorGreenBottomRight;
    private float _colorGreenTopRight;
    private float _colorBlueTopLeft;
    private float _colorBlueBottomLeft;
    private float _colorBlueBottomRight;
    private float _colorBlueTopRight;
    private bool _aoBlockOpXNegYPos;
    private bool _aoBlockOpXPosYPos;
    private bool _aoBlockOpXNegYNeg;
    private bool _aoBlockOpXPosYNeg;
    private bool _aoBlockOpXNegZNeg;
    private bool _aoBlockOpXPosZNeg;
    private bool _aoBlockOpXNegZPos;
    private bool _aoBlockOpXPosZPos;
    private bool _aoBlockOpYNegZNeg;
    private bool _aoBlockOpYPosZNeg;
    private bool _aoBlockOpYNegZPos;
    private bool _aoBlockOpYPosZPos;
    private bool _useOverrideBoundingBox;
    private Box _overrideBoundingBox;

    public BlockRenderer(BlockView var1)
    {
        _blockAccess = var1;
    }

    public BlockRenderer(BlockView var1, Tessellator t)
    {
        _blockAccess = var1;
        _tessellator = t;
    }

    public BlockRenderer()
    {
    }

    public void setOverrideBoundingBox(double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
    {
        _overrideBoundingBox = new Box(minX, minY, minZ, maxX, maxY, maxZ);
        _useOverrideBoundingBox = true;
    }

    public void clearOverrideBoundingBox()
    {
        _useOverrideBoundingBox = false;
    }

    private Tessellator getTessellator()
    {
        if (_tessellator == null)
        {
            return Tessellator.instance;
        }

        return _tessellator;
    }

    public void renderBlockUsingTexture(Block var1, int var2, int var3, int var4, int var5)
    {
        _overrideBlockTexture = var5;
        renderBlockByRenderType(var1, var2, var3, var4);
        _overrideBlockTexture = -1;
    }

    public void func_31075_a(Block var1, int var2, int var3, int var4)
    {
        _renderAllFaces = true;
        renderBlockByRenderType(var1, var2, var3, var4);
        _renderAllFaces = false;
    }

    public bool renderBlockByRenderType(Block block, int x, int y, int z)
    {
        int type = block.getRenderType();
        block.updateBoundingBox(_blockAccess, x, y, z);
        _useOverrideBoundingBox = false;

        return type switch
        {
            0 => renderStandardBlock(block, x, y, z),
            1 => renderBlockReed(block, x, y, z),
            2 => renderBlockTorch(block, x, y, z),
            3 => renderBlockFire(block, x, y, z),
            4 => renderBlockFluids(block, x, y, z),
            5 => renderBlockRedstoneWire(block, x, y, z),
            6 => renderBlockCrops(block, x, y, z),
            7 => renderBlockDoor(block, x, y, z),
            8 => renderBlockLadder(block, x, y, z),
            9 => renderBlockMinecartTrack((BlockRail)block, x, y, z),
            10 => renderBlockStairs(block, x, y, z),
            11 => renderBlockFence(block, x, y, z),
            12 => renderBlockLever(block, x, y, z),
            13 => renderBlockCactus(block, x, y, z),
            14 => renderBlockBed(block, x, y, z),
            15 => renderBlockRepeater(block, x, y, z),
            16 => func_31074_b(block, x, y, z, false),
            17 => func_31080_c(block, x, y, z, true),
            _ => false
        };
    }

    private bool renderBlockBed(Block var1, int var2, int var3, int var4)
    {
        Tessellator var5 = getTessellator();
        Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
        int var6 = _blockAccess.getBlockMeta(var2, var3, var4);
        int var7 = BlockBed.getDirection(var6);
        bool var8 = BlockBed.isHeadOfBed(var6);
        float var9 = 0.5F;
        float var10 = 1.0F;
        float var11 = 0.8F;
        float var12 = 0.6F;
        float var25 = var1.getLuminance(_blockAccess, var2, var3, var4);
        var5.setColorOpaque_F(var9 * var25, var9 * var25, var9 * var25);
        int var26 = var1.getTextureId(_blockAccess, var2, var3, var4, 0);
        int var27 = (var26 & 15) << 4;
        int var28 = var26 & 240;
        double var29 = (double)(var27 / 256.0F);
        double var31 = (var27 + 16 - 0.01D) / 256.0D;
        double var33 = (double)(var28 / 256.0F);
        double var35 = (var28 + 16 - 0.01D) / 256.0D;
        double var37 = var2 + blockBB.MinX;
        double var39 = var2 + blockBB.MaxX;
        double var41 = var3 + blockBB.MinY + 0.1875D;
        double var43 = var4 + blockBB.MinZ;
        double var45 = var4 + blockBB.MaxZ;
        var5.addVertexWithUV(var37, var41, var45, var29, var35);
        var5.addVertexWithUV(var37, var41, var43, var29, var33);
        var5.addVertexWithUV(var39, var41, var43, var31, var33);
        var5.addVertexWithUV(var39, var41, var45, var31, var35);
        float var64 = var1.getLuminance(_blockAccess, var2, var3 + 1, var4);
        var5.setColorOpaque_F(var10 * var64, var10 * var64, var10 * var64);
        var27 = var1.getTextureId(_blockAccess, var2, var3, var4, 1);
        var28 = (var27 & 15) << 4;
        int var67 = var27 & 240;
        double var30 = (double)(var28 / 256.0F);
        double var32 = (var28 + 16 - 0.01D) / 256.0D;
        double var34 = (double)(var67 / 256.0F);
        double var36 = (var67 + 16 - 0.01D) / 256.0D;
        double var38 = var30;
        double var40 = var32;
        double var42 = var34;
        double var44 = var34;
        double var46 = var30;
        double var48 = var32;
        double var50 = var36;
        double var52 = var36;
        if (var7 == 0)
        {
            var40 = var30;
            var42 = var36;
            var46 = var32;
            var52 = var34;
        }
        else if (var7 == 2)
        {
            var38 = var32;
            var44 = var36;
            var48 = var30;
            var50 = var34;
        }
        else if (var7 == 3)
        {
            var38 = var32;
            var44 = var36;
            var48 = var30;
            var50 = var34;
            var40 = var30;
            var42 = var36;
            var46 = var32;
            var52 = var34;
        }

        double var54 = var2 + blockBB.MinX;
        double var56 = var2 + blockBB.MaxX;
        double var58 = var3 + blockBB.MaxY;
        double var60 = var4 + blockBB.MinZ;
        double var62 = var4 + blockBB.MaxZ;
        var5.addVertexWithUV(var56, var58, var62, var46, var50);
        var5.addVertexWithUV(var56, var58, var60, var38, var42);
        var5.addVertexWithUV(var54, var58, var60, var40, var44);
        var5.addVertexWithUV(var54, var58, var62, var48, var52);
        var26 = Facings.TO_DIR[var7];
        if (var8)
        {
            var26 = Facings.TO_DIR[Facings.OPPOSITE[var7]];
        }

        byte var65 = 4;
        switch (var7)
        {
            case 0:
                var65 = 5;
                break;
            case 1:
                var65 = 3;
                goto case 2;
            case 2:
            default:
                break;
            case 3:
                var65 = 2;
                break;
        }

        float var66;
        if (var26 != 2 && (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3, var4 - 1, 2)))
        {
            var66 = var1.getLuminance(_blockAccess, var2, var3, var4 - 1);
            if (blockBB.MinZ > 0.0D)
            {
                var66 = var25;
            }

            var5.setColorOpaque_F(var11 * var66, var11 * var66, var11 * var66);
            _flipTexture = var65 == 2;
            renderEastFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, 2));
        }

        if (var26 != 3 && (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3, var4 + 1, 3)))
        {
            var66 = var1.getLuminance(_blockAccess, var2, var3, var4 + 1);
            if (blockBB.MaxZ < 1.0D)
            {
                var66 = var25;
            }

            var5.setColorOpaque_F(var11 * var66, var11 * var66, var11 * var66);
            _flipTexture = var65 == 3;
            renderWestFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, 3));
        }

        if (var26 != 4 && (_renderAllFaces || var1.isSideVisible(_blockAccess, var2 - 1, var3, var4, 4)))
        {
            var66 = var1.getLuminance(_blockAccess, var2 - 1, var3, var4);
            if (blockBB.MinX > 0.0D)
            {
                var66 = var25;
            }

            var5.setColorOpaque_F(var12 * var66, var12 * var66, var12 * var66);
            _flipTexture = var65 == 4;
            renderNorthFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, 4));
        }

        if (var26 != 5 && (_renderAllFaces || var1.isSideVisible(_blockAccess, var2 + 1, var3, var4, 5)))
        {
            var66 = var1.getLuminance(_blockAccess, var2 + 1, var3, var4);
            if (blockBB.MaxX < 1.0D)
            {
                var66 = var25;
            }

            var5.setColorOpaque_F(var12 * var66, var12 * var66, var12 * var66);
            _flipTexture = var65 == 5;
            renderSouthFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, 5));
        }

        _flipTexture = false;
        return true;
    }

    public bool renderBlockTorch(Block var1, int var2, int var3, int var4)
    {
        int var5 = _blockAccess.getBlockMeta(var2, var3, var4);
        Tessellator var6 = getTessellator();
        float var7 = var1.getLuminance(_blockAccess, var2, var3, var4);
        if (Block.BlocksLightLuminance[var1.id] > 0)
        {
            var7 = 1.0F;
        }

        var6.setColorOpaque_F(var7, var7, var7);
        double var8 = (double)0.4F;
        double var10 = 0.5D - var8;
        double var12 = (double)0.2F;
        if (var5 == 1)
        {
            renderTorchAtAngle(var1, var2 - var10, var3 + var12, var4, -var8, 0.0D);
        }
        else if (var5 == 2)
        {
            renderTorchAtAngle(var1, var2 + var10, var3 + var12, var4, var8, 0.0D);
        }
        else if (var5 == 3)
        {
            renderTorchAtAngle(var1, var2, var3 + var12, var4 - var10, 0.0D, -var8);
        }
        else if (var5 == 4)
        {
            renderTorchAtAngle(var1, var2, var3 + var12, var4 + var10, 0.0D, var8);
        }
        else
        {
            renderTorchAtAngle(var1, var2, var3, var4, 0.0D, 0.0D);
        }

        return true;
    }

    private bool renderBlockRepeater(Block var1, int var2, int var3, int var4)
    {
        int var5 = _blockAccess.getBlockMeta(var2, var3, var4);
        int var6 = var5 & 3;
        int var7 = (var5 & 12) >> 2;
        renderStandardBlock(var1, var2, var3, var4);
        Tessellator var8 = getTessellator();
        float var9 = var1.getLuminance(_blockAccess, var2, var3, var4);
        if (Block.BlocksLightLuminance[var1.id] > 0)
        {
            var9 = (var9 + 1.0F) * 0.5F;
        }

        var8.setColorOpaque_F(var9, var9, var9);
        double var10 = -0.1875D;
        double var12 = 0.0D;
        double var14 = 0.0D;
        double var16 = 0.0D;
        double var18 = 0.0D;
        switch (var6)
        {
            case 0:
                var18 = -0.3125D;
                var14 = BlockRedstoneRepeater.RENDER_OFFSET[var7];
                break;
            case 1:
                var16 = 0.3125D;
                var12 = -BlockRedstoneRepeater.RENDER_OFFSET[var7];
                break;
            case 2:
                var18 = 0.3125D;
                var14 = -BlockRedstoneRepeater.RENDER_OFFSET[var7];
                break;
            case 3:
                var16 = -0.3125D;
                var12 = BlockRedstoneRepeater.RENDER_OFFSET[var7];
                break;
        }

        renderTorchAtAngle(var1, var2 + var12, var3 + var10, var4 + var14, 0.0D, 0.0D);
        renderTorchAtAngle(var1, var2 + var16, var3 + var10, var4 + var18, 0.0D, 0.0D);
        int var20 = var1.getTexture(1);
        int var21 = (var20 & 15) << 4;
        int var22 = var20 & 240;
        double var23 = (double)(var21 / 256.0F);
        double var25 = (double)((var21 + 15.99F) / 256.0F);
        double var27 = (double)(var22 / 256.0F);
        double var29 = (double)((var22 + 15.99F) / 256.0F);
        float var31 = 2.0F / 16.0F;
        float var32 = var2 + 1;
        float var33 = var2 + 1;
        float var34 = var2 + 0;
        float var35 = var2 + 0;
        float var36 = var4 + 0;
        float var37 = var4 + 1;
        float var38 = var4 + 1;
        float var39 = var4 + 0;
        float var40 = var3 + var31;
        if (var6 == 2)
        {
            var33 = var2 + 0;
            var32 = var33;
            var35 = var2 + 1;
            var34 = var35;
            var39 = var4 + 1;
            var36 = var39;
            var38 = var4 + 0;
            var37 = var38;
        }
        else if (var6 == 3)
        {
            var35 = var2 + 0;
            var32 = var35;
            var34 = var2 + 1;
            var33 = var34;
            var37 = var4 + 0;
            var36 = var37;
            var39 = var4 + 1;
            var38 = var39;
        }
        else if (var6 == 1)
        {
            var35 = var2 + 1;
            var32 = var35;
            var34 = var2 + 0;
            var33 = var34;
            var37 = var4 + 1;
            var36 = var37;
            var39 = var4 + 0;
            var38 = var39;
        }

        var8.addVertexWithUV((double)var35, (double)var40, (double)var39, var23, var27);
        var8.addVertexWithUV((double)var34, (double)var40, (double)var38, var23, var29);
        var8.addVertexWithUV((double)var33, (double)var40, (double)var37, var25, var29);
        var8.addVertexWithUV((double)var32, (double)var40, (double)var36, var25, var27);
        return true;
    }

    public void func_31078_d(Block var1, int var2, int var3, int var4)
    {
        _renderAllFaces = true;
        func_31074_b(var1, var2, var3, var4, true);
        _renderAllFaces = false;
    }

    private bool func_31074_b(Block var1, int var2, int var3, int var4, bool var5)
    {
        int var6 = _blockAccess.getBlockMeta(var2, var3, var4);
        bool var7 = var5 || (var6 & 8) != 0;
        int var8 = BlockPistonBase.getFacing(var6);
        if (var7)
        {
            switch (var8)
            {
                case 0:
                    _uvRotateEast = 3;
                    _uvRotateWest = 3;
                    _uvRotateSouth = 3;
                    _uvRotateNorth = 3;
                    setOverrideBoundingBox(0.0F, 0.25F, 0.0F, 1.0F, 1.0F, 1.0F);
                    break;
                case 1:
                    setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 12.0F / 16.0F, 1.0F);
                    break;
                case 2:
                    _uvRotateSouth = 1;
                    _uvRotateNorth = 2;
                    setOverrideBoundingBox(0.0F, 0.0F, 0.25F, 1.0F, 1.0F, 1.0F);
                    break;
                case 3:
                    _uvRotateSouth = 2;
                    _uvRotateNorth = 1;
                    _uvRotateTop = 3;
                    _uvRotateBottom = 3;
                    setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 12.0F / 16.0F);
                    break;
                case 4:
                    _uvRotateEast = 1;
                    _uvRotateWest = 2;
                    _uvRotateTop = 2;
                    _uvRotateBottom = 1;
                    setOverrideBoundingBox(0.25F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                    break;
                case 5:
                    _uvRotateEast = 2;
                    _uvRotateWest = 1;
                    _uvRotateTop = 1;
                    _uvRotateBottom = 2;
                    setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 12.0F / 16.0F, 1.0F, 1.0F);
                    break;
            }

            renderStandardBlock(var1, var2, var3, var4);
            _uvRotateEast = 0;
            _uvRotateWest = 0;
            _uvRotateSouth = 0;
            _uvRotateNorth = 0;
            _uvRotateTop = 0;
            _uvRotateBottom = 0;
            setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
        }
        else
        {
            switch (var8)
            {
                case 0:
                    _uvRotateEast = 3;
                    _uvRotateWest = 3;
                    _uvRotateSouth = 3;
                    _uvRotateNorth = 3;
                    goto case 1;
                case 1:
                default:
                    break;
                case 2:
                    _uvRotateSouth = 1;
                    _uvRotateNorth = 2;
                    break;
                case 3:
                    _uvRotateSouth = 2;
                    _uvRotateNorth = 1;
                    _uvRotateTop = 3;
                    _uvRotateBottom = 3;
                    break;
                case 4:
                    _uvRotateEast = 1;
                    _uvRotateWest = 2;
                    _uvRotateTop = 2;
                    _uvRotateBottom = 1;
                    break;
                case 5:
                    _uvRotateEast = 2;
                    _uvRotateWest = 1;
                    _uvRotateTop = 1;
                    _uvRotateBottom = 2;
                    break;
            }

            renderStandardBlock(var1, var2, var3, var4);
            _uvRotateEast = 0;
            _uvRotateWest = 0;
            _uvRotateSouth = 0;
            _uvRotateNorth = 0;
            _uvRotateTop = 0;
            _uvRotateBottom = 0;
        }

        return true;
    }

    private void func_31076_a(double var1, double var3, double var5, double var7, double var9, double var11, float var13, double var14)
    {
        int var16 = 108;
        if (_overrideBlockTexture >= 0)
        {
            var16 = _overrideBlockTexture;
        }

        int var17 = (var16 & 15) << 4;
        int var18 = var16 & 240;
        Tessellator var19 = getTessellator();
        double var20 = (double)((var17 + 0) / 256.0F);
        double var22 = (double)((var18 + 0) / 256.0F);
        double var24 = (var17 + var14 - 0.01D) / 256.0D;
        double var26 = ((double)(var18 + 4.0F) - 0.01D) / 256.0D;
        var19.setColorOpaque_F(var13, var13, var13);
        var19.addVertexWithUV(var1, var7, var9, var24, var22);
        var19.addVertexWithUV(var1, var5, var9, var20, var22);
        var19.addVertexWithUV(var3, var5, var11, var20, var26);
        var19.addVertexWithUV(var3, var7, var11, var24, var26);
    }

    private void func_31081_b(double var1, double var3, double var5, double var7, double var9, double var11, float var13, double var14)
    {
        int var16 = 108;
        if (_overrideBlockTexture >= 0)
        {
            var16 = _overrideBlockTexture;
        }

        int var17 = (var16 & 15) << 4;
        int var18 = var16 & 240;
        Tessellator var19 = getTessellator();
        double var20 = (double)((var17 + 0) / 256.0F);
        double var22 = (double)((var18 + 0) / 256.0F);
        double var24 = (var17 + var14 - 0.01D) / 256.0D;
        double var26 = ((double)(var18 + 4.0F) - 0.01D) / 256.0D;
        var19.setColorOpaque_F(var13, var13, var13);
        var19.addVertexWithUV(var1, var5, var11, var24, var22);
        var19.addVertexWithUV(var1, var5, var9, var20, var22);
        var19.addVertexWithUV(var3, var7, var9, var20, var26);
        var19.addVertexWithUV(var3, var7, var11, var24, var26);
    }

    private void func_31077_c(double var1, double var3, double var5, double var7, double var9, double var11, float var13, double var14)
    {
        int var16 = 108;
        if (_overrideBlockTexture >= 0)
        {
            var16 = _overrideBlockTexture;
        }

        int var17 = (var16 & 15) << 4;
        int var18 = var16 & 240;
        Tessellator var19 = getTessellator();
        double var20 = (double)((var17 + 0) / 256.0F);
        double var22 = (double)((var18 + 0) / 256.0F);
        double var24 = (var17 + var14 - 0.01D) / 256.0D;
        double var26 = ((double)(var18 + 4.0F) - 0.01D) / 256.0D;
        var19.setColorOpaque_F(var13, var13, var13);
        var19.addVertexWithUV(var3, var5, var9, var24, var22);
        var19.addVertexWithUV(var1, var5, var9, var20, var22);
        var19.addVertexWithUV(var1, var7, var11, var20, var26);
        var19.addVertexWithUV(var3, var7, var11, var24, var26);
    }

    public void func_31079_a(Block var1, int var2, int var3, int var4, bool var5)
    {
        _renderAllFaces = true;
        func_31080_c(var1, var2, var3, var4, var5);
        _renderAllFaces = false;
    }

    private bool func_31080_c(Block var1, int var2, int var3, int var4, bool var5)
    {
        int var6 = _blockAccess.getBlockMeta(var2, var3, var4);
        int var7 = BlockPistonExtension.getFacing(var6);
        float var11 = var1.getLuminance(_blockAccess, var2, var3, var4);
        float var12 = var5 ? 1.0F : 0.5F;
        double var13 = var5 ? 16.0D : 8.0D;
        switch (var7)
        {
            case 0:
                _uvRotateEast = 3;
                _uvRotateWest = 3;
                _uvRotateSouth = 3;
                _uvRotateNorth = 3;
                setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 0.25F, 1.0F);
                renderStandardBlock(var1, var2, var3, var4);
                func_31076_a((double)(var2 + 6.0F / 16.0F), (double)(var2 + 10.0F / 16.0F), (double)(var3 + 0.25F), (double)(var3 + 0.25F + var12), (double)(var4 + 10.0F / 16.0F), (double)(var4 + 10.0F / 16.0F), var11 * 0.8F, var13);
                func_31076_a((double)(var2 + 10.0F / 16.0F), (double)(var2 + 6.0F / 16.0F), (double)(var3 + 0.25F), (double)(var3 + 0.25F + var12), (double)(var4 + 6.0F / 16.0F), (double)(var4 + 6.0F / 16.0F), var11 * 0.8F, var13);
                func_31076_a((double)(var2 + 6.0F / 16.0F), (double)(var2 + 6.0F / 16.0F), (double)(var3 + 0.25F), (double)(var3 + 0.25F + var12), (double)(var4 + 6.0F / 16.0F), (double)(var4 + 10.0F / 16.0F), var11 * 0.6F, var13);
                func_31076_a((double)(var2 + 10.0F / 16.0F), (double)(var2 + 10.0F / 16.0F), (double)(var3 + 0.25F), (double)(var3 + 0.25F + var12), (double)(var4 + 10.0F / 16.0F), (double)(var4 + 6.0F / 16.0F), var11 * 0.6F, var13);
                break;
            case 1:
                setOverrideBoundingBox(0.0F, 12.0F / 16.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                renderStandardBlock(var1, var2, var3, var4);
                func_31076_a((double)(var2 + 6.0F / 16.0F), (double)(var2 + 10.0F / 16.0F), (double)(var3 - 0.25F + 1.0F - var12), (double)(var3 - 0.25F + 1.0F), (double)(var4 + 10.0F / 16.0F), (double)(var4 + 10.0F / 16.0F), var11 * 0.8F, var13);
                func_31076_a((double)(var2 + 10.0F / 16.0F), (double)(var2 + 6.0F / 16.0F), (double)(var3 - 0.25F + 1.0F - var12), (double)(var3 - 0.25F + 1.0F), (double)(var4 + 6.0F / 16.0F), (double)(var4 + 6.0F / 16.0F), var11 * 0.8F, var13);
                func_31076_a((double)(var2 + 6.0F / 16.0F), (double)(var2 + 6.0F / 16.0F), (double)(var3 - 0.25F + 1.0F - var12), (double)(var3 - 0.25F + 1.0F), (double)(var4 + 6.0F / 16.0F), (double)(var4 + 10.0F / 16.0F), var11 * 0.6F, var13);
                func_31076_a((double)(var2 + 10.0F / 16.0F), (double)(var2 + 10.0F / 16.0F), (double)(var3 - 0.25F + 1.0F - var12), (double)(var3 - 0.25F + 1.0F), (double)(var4 + 10.0F / 16.0F), (double)(var4 + 6.0F / 16.0F), var11 * 0.6F, var13);
                break;
            case 2:
                _uvRotateSouth = 1;
                _uvRotateNorth = 2;
                setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.25F);
                renderStandardBlock(var1, var2, var3, var4);
                func_31081_b((double)(var2 + 6.0F / 16.0F), (double)(var2 + 6.0F / 16.0F), (double)(var3 + 10.0F / 16.0F), (double)(var3 + 6.0F / 16.0F), (double)(var4 + 0.25F), (double)(var4 + 0.25F + var12), var11 * 0.6F, var13);
                func_31081_b((double)(var2 + 10.0F / 16.0F), (double)(var2 + 10.0F / 16.0F), (double)(var3 + 6.0F / 16.0F), (double)(var3 + 10.0F / 16.0F), (double)(var4 + 0.25F), (double)(var4 + 0.25F + var12), var11 * 0.6F, var13);
                func_31081_b((double)(var2 + 6.0F / 16.0F), (double)(var2 + 10.0F / 16.0F), (double)(var3 + 6.0F / 16.0F), (double)(var3 + 6.0F / 16.0F), (double)(var4 + 0.25F), (double)(var4 + 0.25F + var12), var11 * 0.5F, var13);
                func_31081_b((double)(var2 + 10.0F / 16.0F), (double)(var2 + 6.0F / 16.0F), (double)(var3 + 10.0F / 16.0F), (double)(var3 + 10.0F / 16.0F), (double)(var4 + 0.25F), (double)(var4 + 0.25F + var12), var11, var13);
                break;
            case 3:
                _uvRotateSouth = 2;
                _uvRotateNorth = 1;
                _uvRotateTop = 3;
                _uvRotateBottom = 3;
                setOverrideBoundingBox(0.0F, 0.0F, 12.0F / 16.0F, 1.0F, 1.0F, 1.0F);
                renderStandardBlock(var1, var2, var3, var4);
                func_31081_b((double)(var2 + 6.0F / 16.0F), (double)(var2 + 6.0F / 16.0F), (double)(var3 + 10.0F / 16.0F), (double)(var3 + 6.0F / 16.0F), (double)(var4 - 0.25F + 1.0F - var12), (double)(var4 - 0.25F + 1.0F), var11 * 0.6F, var13);
                func_31081_b((double)(var2 + 10.0F / 16.0F), (double)(var2 + 10.0F / 16.0F), (double)(var3 + 6.0F / 16.0F), (double)(var3 + 10.0F / 16.0F), (double)(var4 - 0.25F + 1.0F - var12), (double)(var4 - 0.25F + 1.0F), var11 * 0.6F, var13);
                func_31081_b((double)(var2 + 6.0F / 16.0F), (double)(var2 + 10.0F / 16.0F), (double)(var3 + 6.0F / 16.0F), (double)(var3 + 6.0F / 16.0F), (double)(var4 - 0.25F + 1.0F - var12), (double)(var4 - 0.25F + 1.0F), var11 * 0.5F, var13);
                func_31081_b((double)(var2 + 10.0F / 16.0F), (double)(var2 + 6.0F / 16.0F), (double)(var3 + 10.0F / 16.0F), (double)(var3 + 10.0F / 16.0F), (double)(var4 - 0.25F + 1.0F - var12), (double)(var4 - 0.25F + 1.0F), var11, var13);
                break;
            case 4:
                _uvRotateEast = 1;
                _uvRotateWest = 2;
                _uvRotateTop = 2;
                _uvRotateBottom = 1;
                setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 0.25F, 1.0F, 1.0F);
                renderStandardBlock(var1, var2, var3, var4);
                func_31077_c((double)(var2 + 0.25F), (double)(var2 + 0.25F + var12), (double)(var3 + 6.0F / 16.0F), (double)(var3 + 6.0F / 16.0F), (double)(var4 + 10.0F / 16.0F), (double)(var4 + 6.0F / 16.0F), var11 * 0.5F, var13);
                func_31077_c((double)(var2 + 0.25F), (double)(var2 + 0.25F + var12), (double)(var3 + 10.0F / 16.0F), (double)(var3 + 10.0F / 16.0F), (double)(var4 + 6.0F / 16.0F), (double)(var4 + 10.0F / 16.0F), var11, var13);
                func_31077_c((double)(var2 + 0.25F), (double)(var2 + 0.25F + var12), (double)(var3 + 6.0F / 16.0F), (double)(var3 + 10.0F / 16.0F), (double)(var4 + 6.0F / 16.0F), (double)(var4 + 6.0F / 16.0F), var11 * 0.6F, var13);
                func_31077_c((double)(var2 + 0.25F), (double)(var2 + 0.25F + var12), (double)(var3 + 10.0F / 16.0F), (double)(var3 + 6.0F / 16.0F), (double)(var4 + 10.0F / 16.0F), (double)(var4 + 10.0F / 16.0F), var11 * 0.6F, var13);
                break;
            case 5:
                _uvRotateEast = 2;
                _uvRotateWest = 1;
                _uvRotateTop = 1;
                _uvRotateBottom = 2;
                setOverrideBoundingBox(12.0F / 16.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                renderStandardBlock(var1, var2, var3, var4);
                func_31077_c((double)(var2 - 0.25F + 1.0F - var12), (double)(var2 - 0.25F + 1.0F), (double)(var3 + 6.0F / 16.0F), (double)(var3 + 6.0F / 16.0F), (double)(var4 + 10.0F / 16.0F), (double)(var4 + 6.0F / 16.0F), var11 * 0.5F, var13);
                func_31077_c((double)(var2 - 0.25F + 1.0F - var12), (double)(var2 - 0.25F + 1.0F), (double)(var3 + 10.0F / 16.0F), (double)(var3 + 10.0F / 16.0F), (double)(var4 + 6.0F / 16.0F), (double)(var4 + 10.0F / 16.0F), var11, var13);
                func_31077_c((double)(var2 - 0.25F + 1.0F - var12), (double)(var2 - 0.25F + 1.0F), (double)(var3 + 6.0F / 16.0F), (double)(var3 + 10.0F / 16.0F), (double)(var4 + 6.0F / 16.0F), (double)(var4 + 6.0F / 16.0F), var11 * 0.6F, var13);
                func_31077_c((double)(var2 - 0.25F + 1.0F - var12), (double)(var2 - 0.25F + 1.0F), (double)(var3 + 10.0F / 16.0F), (double)(var3 + 6.0F / 16.0F), (double)(var4 + 10.0F / 16.0F), (double)(var4 + 10.0F / 16.0F), var11 * 0.6F, var13);
                break;
        }

        _uvRotateEast = 0;
        _uvRotateWest = 0;
        _uvRotateSouth = 0;
        _uvRotateNorth = 0;
        _uvRotateTop = 0;
        _uvRotateBottom = 0;
        setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
        return true;
    }

    public bool renderBlockLever(Block var1, int var2, int var3, int var4)
    {
        int var5 = _blockAccess.getBlockMeta(var2, var3, var4);
        int var6 = var5 & 7;
        bool var7 = (var5 & 8) > 0;
        Tessellator var8 = getTessellator();
        bool var9 = _overrideBlockTexture >= 0;
        if (!var9)
        {
            _overrideBlockTexture = Block.Cobblestone.textureId;
        }

        float var10 = 0.25F;
        float var11 = 3.0F / 16.0F;
        float var12 = 3.0F / 16.0F;
        if (var6 == 5)
        {
            setOverrideBoundingBox(0.5F - var11, 0.0F, 0.5F - var10, 0.5F + var11, var12, 0.5F + var10);
        }
        else if (var6 == 6)
        {
            setOverrideBoundingBox(0.5F - var10, 0.0F, 0.5F - var11, 0.5F + var10, var12, 0.5F + var11);
        }
        else if (var6 == 4)
        {
            setOverrideBoundingBox(0.5F - var11, 0.5F - var10, 1.0F - var12, 0.5F + var11, 0.5F + var10, 1.0F);
        }
        else if (var6 == 3)
        {
            setOverrideBoundingBox(0.5F - var11, 0.5F - var10, 0.0F, 0.5F + var11, 0.5F + var10, var12);
        }
        else if (var6 == 2)
        {
            setOverrideBoundingBox(1.0F - var12, 0.5F - var10, 0.5F - var11, 1.0F, 0.5F + var10, 0.5F + var11);
        }
        else if (var6 == 1)
        {
            setOverrideBoundingBox(0.0F, 0.5F - var10, 0.5F - var11, var12, 0.5F + var10, 0.5F + var11);
        }

        renderStandardBlock(var1, var2, var3, var4);
        if (!var9)
        {
            _overrideBlockTexture = -1;
        }

        float var13 = var1.getLuminance(_blockAccess, var2, var3, var4);
        if (Block.BlocksLightLuminance[var1.id] > 0)
        {
            var13 = 1.0F;
        }

        var8.setColorOpaque_F(var13, var13, var13);
        int var14 = var1.getTexture(0);
        if (_overrideBlockTexture >= 0)
        {
            var14 = _overrideBlockTexture;
        }

        int var15 = (var14 & 15) << 4;
        int var16 = var14 & 240;
        float var17 = var15 / 256.0F;
        float var18 = (var15 + 15.99F) / 256.0F;
        float var19 = var16 / 256.0F;
        float var20 = (var16 + 15.99F) / 256.0F;
        Vector3D<double>[] var21 = new Vector3D<double>[8];
        float var22 = 1.0F / 16.0F;
        float var23 = 1.0F / 16.0F;
        float var24 = 10.0F / 16.0F;
        var21[0] = new((double)-var22, 0.0D, (double)-var23);
        var21[1] = new((double)var22, 0.0D, (double)-var23);
        var21[2] = new((double)var22, 0.0D, (double)var23);
        var21[3] = new((double)-var22, 0.0D, (double)var23);
        var21[4] = new((double)-var22, (double)var24, (double)-var23);
        var21[5] = new((double)var22, (double)var24, (double)-var23);
        var21[6] = new((double)var22, (double)var24, (double)var23);
        var21[7] = new((double)-var22, (double)var24, (double)var23);

        for (int var25 = 0; var25 < 8; ++var25)
        {
            if (var7)
            {
                var21[var25].Z -= 1.0D / 16.0D;
                rotateAroundX(ref var21[var25], (float)Math.PI * 2.0F / 9.0F);
            }
            else
            {
                var21[var25].Z += 1.0D / 16.0D;
                rotateAroundX(ref var21[var25], -((float)Math.PI * 2.0F / 9.0F));
            }

            if (var6 == 6)
            {
                rotateAroundY(ref var21[var25], (float)Math.PI * 0.5F);
            }

            if (var6 < 5)
            {
                var21[var25].Y -= 0.375D;
                rotateAroundX(ref var21[var25], (float)Math.PI * 0.5F);
                if (var6 == 4)
                {
                    rotateAroundY(ref var21[var25], 0.0f);
                }

                if (var6 == 3)
                {
                    rotateAroundY(ref var21[var25], (float)Math.PI);
                }

                if (var6 == 2)
                {
                    rotateAroundY(ref var21[var25], (float)Math.PI * 0.5F);
                }

                if (var6 == 1)
                {
                    rotateAroundY(ref var21[var25], (float)Math.PI * -0.5F);
                }

                var21[var25].X += var2 + 0.5D;
                var21[var25].Y += var3 + 0.5F;
                var21[var25].Z += var4 + 0.5D;
            }
            else
            {
                var21[var25].X += var2 + 0.5D;
                var21[var25].Y += var3 + 2.0F / 16.0F;
                var21[var25].Z += var4 + 0.5D;
            }
        }

        Vector3D<double> var30 = new();
        Vector3D<double> var26 = new();
        Vector3D<double> var27 = new();
        Vector3D<double> var28 = new();

        for (int var29 = 0; var29 < 6; ++var29)
        {
            if (var29 == 0)
            {
                var17 = (var15 + 7) / 256.0F;
                var18 = (var15 + 9 - 0.01F) / 256.0F;
                var19 = (var16 + 6) / 256.0F;
                var20 = (var16 + 8 - 0.01F) / 256.0F;
            }
            else if (var29 == 2)
            {
                var17 = (var15 + 7) / 256.0F;
                var18 = (var15 + 9 - 0.01F) / 256.0F;
                var19 = (var16 + 6) / 256.0F;
                var20 = (var16 + 16 - 0.01F) / 256.0F;
            }

            if (var29 == 0)
            {
                var30 = var21[0];
                var26 = var21[1];
                var27 = var21[2];
                var28 = var21[3];
            }
            else if (var29 == 1)
            {
                var30 = var21[7];
                var26 = var21[6];
                var27 = var21[5];
                var28 = var21[4];
            }
            else if (var29 == 2)
            {
                var30 = var21[1];
                var26 = var21[0];
                var27 = var21[4];
                var28 = var21[5];
            }
            else if (var29 == 3)
            {
                var30 = var21[2];
                var26 = var21[1];
                var27 = var21[5];
                var28 = var21[6];
            }
            else if (var29 == 4)
            {
                var30 = var21[3];
                var26 = var21[2];
                var27 = var21[6];
                var28 = var21[7];
            }
            else if (var29 == 5)
            {
                var30 = var21[0];
                var26 = var21[3];
                var27 = var21[7];
                var28 = var21[4];
            }

            var8.addVertexWithUV(var30.X, var30.Y, var30.Z, (double)var17, (double)var20);
            var8.addVertexWithUV(var26.X, var26.Y, var26.Z, (double)var18, (double)var20);
            var8.addVertexWithUV(var27.X, var27.Y, var27.Z, (double)var18, (double)var19);
            var8.addVertexWithUV(var28.X, var28.Y, var28.Z, (double)var17, (double)var19);
        }

        return true;
    }

    public bool renderBlockFire(Block var1, int var2, int var3, int var4)
    {
        Tessellator var5 = getTessellator();
        int var6 = var1.getTexture(0);
        if (_overrideBlockTexture >= 0)
        {
            var6 = _overrideBlockTexture;
        }

        float var7 = var1.getLuminance(_blockAccess, var2, var3, var4);
        var5.setColorOpaque_F(var7, var7, var7);
        int var8 = (var6 & 15) << 4;
        int var9 = var6 & 240;
        double var10 = (double)(var8 / 256.0F);
        double var12 = (double)((var8 + 15.99F) / 256.0F);
        double var14 = (double)(var9 / 256.0F);
        double var16 = (double)((var9 + 15.99F) / 256.0F);
        float var18 = 1.4F;
        double var21;
        double var23;
        double var25;
        double var27;
        double var29;
        double var31;
        double var33;
        if (!_blockAccess.shouldSuffocate(var2, var3 - 1, var4) && !Block.Fire.isFlammable(_blockAccess, var2, var3 - 1, var4))
        {
            float var37 = 0.2F;
            float var20 = 1.0F / 16.0F;
            if ((var2 + var3 + var4 & 1) == 1)
            {
                var10 = (double)(var8 / 256.0F);
                var12 = (double)((var8 + 15.99F) / 256.0F);
                var14 = (double)((var9 + 16) / 256.0F);
                var16 = (double)((var9 + 15.99F + 16.0F) / 256.0F);
            }

            if ((var2 / 2 + var3 / 2 + var4 / 2 & 1) == 1)
            {
                var21 = var12;
                var12 = var10;
                var10 = var21;
            }

            if (Block.Fire.isFlammable(_blockAccess, var2 - 1, var3, var4))
            {
                var5.addVertexWithUV((double)(var2 + var37), (double)(var3 + var18 + var20), var4 + 1, var12, var14);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 0 + var20), var4 + 1, var12, var16);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 0 + var20), var4 + 0, var10, var16);
                var5.addVertexWithUV((double)(var2 + var37), (double)(var3 + var18 + var20), var4 + 0, var10, var14);
                var5.addVertexWithUV((double)(var2 + var37), (double)(var3 + var18 + var20), var4 + 0, var10, var14);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 0 + var20), var4 + 0, var10, var16);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 0 + var20), var4 + 1, var12, var16);
                var5.addVertexWithUV((double)(var2 + var37), (double)(var3 + var18 + var20), var4 + 1, var12, var14);
            }

            if (Block.Fire.isFlammable(_blockAccess, var2 + 1, var3, var4))
            {
                var5.addVertexWithUV((double)(var2 + 1 - var37), (double)(var3 + var18 + var20), var4 + 0, var10, var14);
                var5.addVertexWithUV(var2 + 1 - 0, (double)(var3 + 0 + var20), var4 + 0, var10, var16);
                var5.addVertexWithUV(var2 + 1 - 0, (double)(var3 + 0 + var20), var4 + 1, var12, var16);
                var5.addVertexWithUV((double)(var2 + 1 - var37), (double)(var3 + var18 + var20), var4 + 1, var12, var14);
                var5.addVertexWithUV((double)(var2 + 1 - var37), (double)(var3 + var18 + var20), var4 + 1, var12, var14);
                var5.addVertexWithUV(var2 + 1 - 0, (double)(var3 + 0 + var20), var4 + 1, var12, var16);
                var5.addVertexWithUV(var2 + 1 - 0, (double)(var3 + 0 + var20), var4 + 0, var10, var16);
                var5.addVertexWithUV((double)(var2 + 1 - var37), (double)(var3 + var18 + var20), var4 + 0, var10, var14);
            }

            if (Block.Fire.isFlammable(_blockAccess, var2, var3, var4 - 1))
            {
                var5.addVertexWithUV(var2 + 0, (double)(var3 + var18 + var20), (double)(var4 + var37), var12, var14);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 0 + var20), var4 + 0, var12, var16);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + 0 + var20), var4 + 0, var10, var16);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + var18 + var20), (double)(var4 + var37), var10, var14);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + var18 + var20), (double)(var4 + var37), var10, var14);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + 0 + var20), var4 + 0, var10, var16);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 0 + var20), var4 + 0, var12, var16);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + var18 + var20), (double)(var4 + var37), var12, var14);
            }

            if (Block.Fire.isFlammable(_blockAccess, var2, var3, var4 + 1))
            {
                var5.addVertexWithUV(var2 + 1, (double)(var3 + var18 + var20), (double)(var4 + 1 - var37), var10, var14);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + 0 + var20), var4 + 1 - 0, var10, var16);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 0 + var20), var4 + 1 - 0, var12, var16);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + var18 + var20), (double)(var4 + 1 - var37), var12, var14);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + var18 + var20), (double)(var4 + 1 - var37), var12, var14);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 0 + var20), var4 + 1 - 0, var12, var16);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + 0 + var20), var4 + 1 - 0, var10, var16);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + var18 + var20), (double)(var4 + 1 - var37), var10, var14);
            }

            if (Block.Fire.isFlammable(_blockAccess, var2, var3 + 1, var4))
            {
                var21 = var2 + 0.5D + 0.5D;
                var23 = var2 + 0.5D - 0.5D;
                var25 = var4 + 0.5D + 0.5D;
                var27 = var4 + 0.5D - 0.5D;
                var29 = var2 + 0.5D - 0.5D;
                var31 = var2 + 0.5D + 0.5D;
                var33 = var4 + 0.5D - 0.5D;
                double var35 = var4 + 0.5D + 0.5D;
                var10 = (double)(var8 / 256.0F);
                var12 = (double)((var8 + 15.99F) / 256.0F);
                var14 = (double)(var9 / 256.0F);
                var16 = (double)((var9 + 15.99F) / 256.0F);
                ++var3;
                var18 = -0.2F;
                if ((var2 + var3 + var4 & 1) == 0)
                {
                    var5.addVertexWithUV(var29, (double)(var3 + var18), var4 + 0, var12, var14);
                    var5.addVertexWithUV(var21, var3 + 0, var4 + 0, var12, var16);
                    var5.addVertexWithUV(var21, var3 + 0, var4 + 1, var10, var16);
                    var5.addVertexWithUV(var29, (double)(var3 + var18), var4 + 1, var10, var14);
                    var10 = (double)(var8 / 256.0F);
                    var12 = (double)((var8 + 15.99F) / 256.0F);
                    var14 = (double)((var9 + 16) / 256.0F);
                    var16 = (double)((var9 + 15.99F + 16.0F) / 256.0F);
                    var5.addVertexWithUV(var31, (double)(var3 + var18), var4 + 1, var12, var14);
                    var5.addVertexWithUV(var23, var3 + 0, var4 + 1, var12, var16);
                    var5.addVertexWithUV(var23, var3 + 0, var4 + 0, var10, var16);
                    var5.addVertexWithUV(var31, (double)(var3 + var18), var4 + 0, var10, var14);
                }
                else
                {
                    var5.addVertexWithUV(var2 + 0, (double)(var3 + var18), var35, var12, var14);
                    var5.addVertexWithUV(var2 + 0, var3 + 0, var27, var12, var16);
                    var5.addVertexWithUV(var2 + 1, var3 + 0, var27, var10, var16);
                    var5.addVertexWithUV(var2 + 1, (double)(var3 + var18), var35, var10, var14);
                    var10 = (double)(var8 / 256.0F);
                    var12 = (double)((var8 + 15.99F) / 256.0F);
                    var14 = (double)((var9 + 16) / 256.0F);
                    var16 = (double)((var9 + 15.99F + 16.0F) / 256.0F);
                    var5.addVertexWithUV(var2 + 1, (double)(var3 + var18), var33, var12, var14);
                    var5.addVertexWithUV(var2 + 1, var3 + 0, var25, var12, var16);
                    var5.addVertexWithUV(var2 + 0, var3 + 0, var25, var10, var16);
                    var5.addVertexWithUV(var2 + 0, (double)(var3 + var18), var33, var10, var14);
                }
            }
        }
        else
        {
            double var19 = var2 + 0.5D + 0.2D;
            var21 = var2 + 0.5D - 0.2D;
            var23 = var4 + 0.5D + 0.2D;
            var25 = var4 + 0.5D - 0.2D;
            var27 = var2 + 0.5D - 0.3D;
            var29 = var2 + 0.5D + 0.3D;
            var31 = var4 + 0.5D - 0.3D;
            var33 = var4 + 0.5D + 0.3D;
            var5.addVertexWithUV(var27, (double)(var3 + var18), var4 + 1, var12, var14);
            var5.addVertexWithUV(var19, var3 + 0, var4 + 1, var12, var16);
            var5.addVertexWithUV(var19, var3 + 0, var4 + 0, var10, var16);
            var5.addVertexWithUV(var27, (double)(var3 + var18), var4 + 0, var10, var14);
            var5.addVertexWithUV(var29, (double)(var3 + var18), var4 + 0, var12, var14);
            var5.addVertexWithUV(var21, var3 + 0, var4 + 0, var12, var16);
            var5.addVertexWithUV(var21, var3 + 0, var4 + 1, var10, var16);
            var5.addVertexWithUV(var29, (double)(var3 + var18), var4 + 1, var10, var14);
            var10 = (double)(var8 / 256.0F);
            var12 = (double)((var8 + 15.99F) / 256.0F);
            var14 = (double)((var9 + 16) / 256.0F);
            var16 = (double)((var9 + 15.99F + 16.0F) / 256.0F);
            var5.addVertexWithUV(var2 + 1, (double)(var3 + var18), var33, var12, var14);
            var5.addVertexWithUV(var2 + 1, var3 + 0, var25, var12, var16);
            var5.addVertexWithUV(var2 + 0, var3 + 0, var25, var10, var16);
            var5.addVertexWithUV(var2 + 0, (double)(var3 + var18), var33, var10, var14);
            var5.addVertexWithUV(var2 + 0, (double)(var3 + var18), var31, var12, var14);
            var5.addVertexWithUV(var2 + 0, var3 + 0, var23, var12, var16);
            var5.addVertexWithUV(var2 + 1, var3 + 0, var23, var10, var16);
            var5.addVertexWithUV(var2 + 1, (double)(var3 + var18), var31, var10, var14);
            var19 = var2 + 0.5D - 0.5D;
            var21 = var2 + 0.5D + 0.5D;
            var23 = var4 + 0.5D - 0.5D;
            var25 = var4 + 0.5D + 0.5D;
            var27 = var2 + 0.5D - 0.4D;
            var29 = var2 + 0.5D + 0.4D;
            var31 = var4 + 0.5D - 0.4D;
            var33 = var4 + 0.5D + 0.4D;
            var5.addVertexWithUV(var27, (double)(var3 + var18), var4 + 0, var10, var14);
            var5.addVertexWithUV(var19, var3 + 0, var4 + 0, var10, var16);
            var5.addVertexWithUV(var19, var3 + 0, var4 + 1, var12, var16);
            var5.addVertexWithUV(var27, (double)(var3 + var18), var4 + 1, var12, var14);
            var5.addVertexWithUV(var29, (double)(var3 + var18), var4 + 1, var10, var14);
            var5.addVertexWithUV(var21, var3 + 0, var4 + 1, var10, var16);
            var5.addVertexWithUV(var21, var3 + 0, var4 + 0, var12, var16);
            var5.addVertexWithUV(var29, (double)(var3 + var18), var4 + 0, var12, var14);
            var10 = (double)(var8 / 256.0F);
            var12 = (double)((var8 + 15.99F) / 256.0F);
            var14 = (double)(var9 / 256.0F);
            var16 = (double)((var9 + 15.99F) / 256.0F);
            var5.addVertexWithUV(var2 + 0, (double)(var3 + var18), var33, var10, var14);
            var5.addVertexWithUV(var2 + 0, var3 + 0, var25, var10, var16);
            var5.addVertexWithUV(var2 + 1, var3 + 0, var25, var12, var16);
            var5.addVertexWithUV(var2 + 1, (double)(var3 + var18), var33, var12, var14);
            var5.addVertexWithUV(var2 + 1, (double)(var3 + var18), var31, var10, var14);
            var5.addVertexWithUV(var2 + 1, var3 + 0, var23, var10, var16);
            var5.addVertexWithUV(var2 + 0, var3 + 0, var23, var12, var16);
            var5.addVertexWithUV(var2 + 0, (double)(var3 + var18), var31, var12, var14);
        }

        return true;
    }

    public bool renderBlockRedstoneWire(Block var1, int var2, int var3, int var4)
    {
        Tessellator var5 = getTessellator();
        int var6 = _blockAccess.getBlockMeta(var2, var3, var4);
        int var7 = var1.getTexture(1, var6);
        if (_overrideBlockTexture >= 0)
        {
            var7 = _overrideBlockTexture;
        }

        float var8 = var1.getLuminance(_blockAccess, var2, var3, var4);
        float var9 = var6 / 15.0F;
        float var10 = var9 * 0.6F + 0.4F;
        if (var6 == 0)
        {
            var10 = 0.3F;
        }

        float var11 = var9 * var9 * 0.7F - 0.5F;
        float var12 = var9 * var9 * 0.6F - 0.7F;
        if (var11 < 0.0F)
        {
            var11 = 0.0F;
        }

        if (var12 < 0.0F)
        {
            var12 = 0.0F;
        }

        var5.setColorOpaque_F(var8 * var10, var8 * var11, var8 * var12);
        int var13 = (var7 & 15) << 4;
        int var14 = var7 & 240;
        double var15 = (double)(var13 / 256.0F);
        double var17 = (double)((var13 + 15.99F) / 256.0F);
        double var19 = (double)(var14 / 256.0F);
        double var21 = (double)((var14 + 15.99F) / 256.0F);
        bool var26 = BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2 - 1, var3, var4, 1) || !_blockAccess.shouldSuffocate(var2 - 1, var3, var4) && BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2 - 1, var3 - 1, var4, -1);
        bool var27 = BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2 + 1, var3, var4, 3) || !_blockAccess.shouldSuffocate(var2 + 1, var3, var4) && BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2 + 1, var3 - 1, var4, -1);
        bool var28 = BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2, var3, var4 - 1, 2) || !_blockAccess.shouldSuffocate(var2, var3, var4 - 1) && BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2, var3 - 1, var4 - 1, -1);
        bool var29 = BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2, var3, var4 + 1, 0) || !_blockAccess.shouldSuffocate(var2, var3, var4 + 1) && BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2, var3 - 1, var4 + 1, -1);
        if (!_blockAccess.shouldSuffocate(var2, var3 + 1, var4))
        {
            if (_blockAccess.shouldSuffocate(var2 - 1, var3, var4) && BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2 - 1, var3 + 1, var4, -1))
            {
                var26 = true;
            }

            if (_blockAccess.shouldSuffocate(var2 + 1, var3, var4) && BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2 + 1, var3 + 1, var4, -1))
            {
                var27 = true;
            }

            if (_blockAccess.shouldSuffocate(var2, var3, var4 - 1) && BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2, var3 + 1, var4 - 1, -1))
            {
                var28 = true;
            }

            if (_blockAccess.shouldSuffocate(var2, var3, var4 + 1) && BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2, var3 + 1, var4 + 1, -1))
            {
                var29 = true;
            }
        }

        float var31 = var2 + 0;
        float var32 = var2 + 1;
        float var33 = var4 + 0;
        float var34 = var4 + 1;
        byte var35 = 0;
        if ((var26 || var27) && !var28 && !var29)
        {
            var35 = 1;
        }

        if ((var28 || var29) && !var27 && !var26)
        {
            var35 = 2;
        }

        if (var35 != 0)
        {
            var15 = (double)((var13 + 16) / 256.0F);
            var17 = (double)((var13 + 16 + 15.99F) / 256.0F);
            var19 = (double)(var14 / 256.0F);
            var21 = (double)((var14 + 15.99F) / 256.0F);
        }

        if (var35 == 0)
        {
            if (var27 || var28 || var29 || var26)
            {
                if (!var26)
                {
                    var31 += 5.0F / 16.0F;
                }

                if (!var26)
                {
                    var15 += 1.25D / 64.0D;
                }

                if (!var27)
                {
                    var32 -= 5.0F / 16.0F;
                }

                if (!var27)
                {
                    var17 -= 1.25D / 64.0D;
                }

                if (!var28)
                {
                    var33 += 5.0F / 16.0F;
                }

                if (!var28)
                {
                    var19 += 1.25D / 64.0D;
                }

                if (!var29)
                {
                    var34 -= 5.0F / 16.0F;
                }

                if (!var29)
                {
                    var21 -= 1.25D / 64.0D;
                }
            }

            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var34, var17, var21);
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var33, var17, var19);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var33, var15, var19);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var34, var15, var21);
            var5.setColorOpaque_F(var8, var8, var8);
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var34, var17, var21 + 1.0D / 16.0D);
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var33, var17, var19 + 1.0D / 16.0D);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var33, var15, var19 + 1.0D / 16.0D);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var34, var15, var21 + 1.0D / 16.0D);
        }
        else if (var35 == 1)
        {
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var34, var17, var21);
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var33, var17, var19);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var33, var15, var19);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var34, var15, var21);
            var5.setColorOpaque_F(var8, var8, var8);
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var34, var17, var21 + 1.0D / 16.0D);
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var33, var17, var19 + 1.0D / 16.0D);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var33, var15, var19 + 1.0D / 16.0D);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var34, var15, var21 + 1.0D / 16.0D);
        }
        else if (var35 == 2)
        {
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var34, var17, var21);
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var33, var15, var21);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var33, var15, var19);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var34, var17, var19);
            var5.setColorOpaque_F(var8, var8, var8);
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var34, var17, var21 + 1.0D / 16.0D);
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var33, var15, var21 + 1.0D / 16.0D);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var33, var15, var19 + 1.0D / 16.0D);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var34, var17, var19 + 1.0D / 16.0D);
        }

        if (!_blockAccess.shouldSuffocate(var2, var3 + 1, var4))
        {
            var15 = (double)((var13 + 16) / 256.0F);
            var17 = (double)((var13 + 16 + 15.99F) / 256.0F);
            var19 = (double)(var14 / 256.0F);
            var21 = (double)((var14 + 15.99F) / 256.0F);
            if (_blockAccess.shouldSuffocate(var2 - 1, var3, var4) && _blockAccess.getBlockId(var2 - 1, var3 + 1, var4) == Block.RedstoneWire.id)
            {
                var5.setColorOpaque_F(var8 * var10, var8 * var11, var8 * var12);
                var5.addVertexWithUV((double)(var2 + (1 / 64f)), (double)(var3 + 1 + 7.0F / 320.0F), var4 + 1, var17, var19);
                var5.addVertexWithUV((double)(var2 + (1 / 64f)), var3 + 0, var4 + 1, var15, var19);
                var5.addVertexWithUV((double)(var2 + (1 / 64f)), var3 + 0, var4 + 0, var15, var21);
                var5.addVertexWithUV((double)(var2 + (1 / 64f)), (double)(var3 + 1 + 7.0F / 320.0F), var4 + 0, var17, var21);
                var5.setColorOpaque_F(var8, var8, var8);
                var5.addVertexWithUV((double)(var2 + (1 / 64f)), (double)(var3 + 1 + 7.0F / 320.0F), var4 + 1, var17, var19 + 1.0D / 16.0D);
                var5.addVertexWithUV((double)(var2 + (1 / 64f)), var3 + 0, var4 + 1, var15, var19 + 1.0D / 16.0D);
                var5.addVertexWithUV((double)(var2 + (1 / 64f)), var3 + 0, var4 + 0, var15, var21 + 1.0D / 16.0D);
                var5.addVertexWithUV((double)(var2 + (1 / 64f)), (double)(var3 + 1 + 7.0F / 320.0F), var4 + 0, var17, var21 + 1.0D / 16.0D);
            }

            if (_blockAccess.shouldSuffocate(var2 + 1, var3, var4) && _blockAccess.getBlockId(var2 + 1, var3 + 1, var4) == Block.RedstoneWire.id)
            {
                var5.setColorOpaque_F(var8 * var10, var8 * var11, var8 * var12);
                var5.addVertexWithUV((double)(var2 + 1 - (1 / 64f)), var3 + 0, var4 + 1, var15, var21);
                var5.addVertexWithUV((double)(var2 + 1 - (1 / 64f)), (double)(var3 + 1 + 7.0F / 320.0F), var4 + 1, var17, var21);
                var5.addVertexWithUV((double)(var2 + 1 - (1 / 64f)), (double)(var3 + 1 + 7.0F / 320.0F), var4 + 0, var17, var19);
                var5.addVertexWithUV((double)(var2 + 1 - (1 / 64f)), var3 + 0, var4 + 0, var15, var19);
                var5.setColorOpaque_F(var8, var8, var8);
                var5.addVertexWithUV((double)(var2 + 1 - (1 / 64f)), var3 + 0, var4 + 1, var15, var21 + 1.0D / 16.0D);
                var5.addVertexWithUV((double)(var2 + 1 - (1 / 64f)), (double)(var3 + 1 + 7.0F / 320.0F), var4 + 1, var17, var21 + 1.0D / 16.0D);
                var5.addVertexWithUV((double)(var2 + 1 - (1 / 64f)), (double)(var3 + 1 + 7.0F / 320.0F), var4 + 0, var17, var19 + 1.0D / 16.0D);
                var5.addVertexWithUV((double)(var2 + 1 - (1 / 64f)), var3 + 0, var4 + 0, var15, var19 + 1.0D / 16.0D);
            }

            if (_blockAccess.shouldSuffocate(var2, var3, var4 - 1) && _blockAccess.getBlockId(var2, var3 + 1, var4 - 1) == Block.RedstoneWire.id)
            {
                var5.setColorOpaque_F(var8 * var10, var8 * var11, var8 * var12);
                var5.addVertexWithUV(var2 + 1, var3 + 0, (double)(var4 + (1 / 64f)), var15, var21);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + 1 + 7.0F / 320.0F), (double)(var4 + (1 / 64f)), var17, var21);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 1 + 7.0F / 320.0F), (double)(var4 + (1 / 64f)), var17, var19);
                var5.addVertexWithUV(var2 + 0, var3 + 0, (double)(var4 + (1 / 64f)), var15, var19);
                var5.setColorOpaque_F(var8, var8, var8);
                var5.addVertexWithUV(var2 + 1, var3 + 0, (double)(var4 + (1 / 64f)), var15, var21 + 1.0D / 16.0D);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + 1 + 7.0F / 320.0F), (double)(var4 + (1 / 64f)), var17, var21 + 1.0D / 16.0D);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 1 + 7.0F / 320.0F), (double)(var4 + (1 / 64f)), var17, var19 + 1.0D / 16.0D);
                var5.addVertexWithUV(var2 + 0, var3 + 0, (double)(var4 + (1 / 64f)), var15, var19 + 1.0D / 16.0D);
            }

            if (_blockAccess.shouldSuffocate(var2, var3, var4 + 1) && _blockAccess.getBlockId(var2, var3 + 1, var4 + 1) == Block.RedstoneWire.id)
            {
                var5.setColorOpaque_F(var8 * var10, var8 * var11, var8 * var12);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + 1 + 7.0F / 320.0F), (double)(var4 + 1 - (1 / 64f)), var17, var19);
                var5.addVertexWithUV(var2 + 1, var3 + 0, (double)(var4 + 1 - (1 / 64f)), var15, var19);
                var5.addVertexWithUV(var2 + 0, var3 + 0, (double)(var4 + 1 - (1 / 64f)), var15, var21);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 1 + 7.0F / 320.0F), (double)(var4 + 1 - (1 / 64f)), var17, var21);
                var5.setColorOpaque_F(var8, var8, var8);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + 1 + 7.0F / 320.0F), (double)(var4 + 1 - (1 / 64f)), var17, var19 + 1.0D / 16.0D);
                var5.addVertexWithUV(var2 + 1, var3 + 0, (double)(var4 + 1 - (1 / 64f)), var15, var19 + 1.0D / 16.0D);
                var5.addVertexWithUV(var2 + 0, var3 + 0, (double)(var4 + 1 - (1 / 64f)), var15, var21 + 1.0D / 16.0D);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 1 + 7.0F / 320.0F), (double)(var4 + 1 - (1 / 64f)), var17, var21 + 1.0D / 16.0D);
            }
        }

        return true;
    }

    public bool renderBlockMinecartTrack(BlockRail var1, int var2, int var3, int var4)
    {
        Tessellator var5 = getTessellator();
        int var6 = _blockAccess.getBlockMeta(var2, var3, var4);
        int var7 = var1.getTexture(0, var6);
        if (_overrideBlockTexture >= 0)
        {
            var7 = _overrideBlockTexture;
        }

        if (var1.isAlwaysStraight())
        {
            var6 &= 7;
        }

        float var8 = var1.getLuminance(_blockAccess, var2, var3, var4);
        var5.setColorOpaque_F(var8, var8, var8);
        int var9 = (var7 & 15) << 4;
        int var10 = var7 & 240;
        double var11 = (double)(var9 / 256.0F);
        double var13 = (double)((var9 + 15.99F) / 256.0F);
        double var15 = (double)(var10 / 256.0F);
        double var17 = (double)((var10 + 15.99F) / 256.0F);
        float var19 = 1.0F / 16.0F;
        float var20 = var2 + 1;
        float var21 = var2 + 1;
        float var22 = var2 + 0;
        float var23 = var2 + 0;
        float var24 = var4 + 0;
        float var25 = var4 + 1;
        float var26 = var4 + 1;
        float var27 = var4 + 0;
        float var28 = var3 + var19;
        float var29 = var3 + var19;
        float var30 = var3 + var19;
        float var31 = var3 + var19;
        if (var6 != 1 && var6 != 2 && var6 != 3 && var6 != 7)
        {
            if (var6 == 8)
            {
                var21 = var2 + 0;
                var20 = var21;
                var23 = var2 + 1;
                var22 = var23;
                var27 = var4 + 1;
                var24 = var27;
                var26 = var4 + 0;
                var25 = var26;
            }
            else if (var6 == 9)
            {
                var23 = var2 + 0;
                var20 = var23;
                var22 = var2 + 1;
                var21 = var22;
                var25 = var4 + 0;
                var24 = var25;
                var27 = var4 + 1;
                var26 = var27;
            }
        }
        else
        {
            var23 = var2 + 1;
            var20 = var23;
            var22 = var2 + 0;
            var21 = var22;
            var25 = var4 + 1;
            var24 = var25;
            var27 = var4 + 0;
            var26 = var27;
        }

        if (var6 != 2 && var6 != 4)
        {
            if (var6 == 3 || var6 == 5)
            {
                ++var29;
                ++var30;
            }
        }
        else
        {
            ++var28;
            ++var31;
        }

        var5.addVertexWithUV((double)var20, (double)var28, (double)var24, var13, var15);
        var5.addVertexWithUV((double)var21, (double)var29, (double)var25, var13, var17);
        var5.addVertexWithUV((double)var22, (double)var30, (double)var26, var11, var17);
        var5.addVertexWithUV((double)var23, (double)var31, (double)var27, var11, var15);
        var5.addVertexWithUV((double)var23, (double)var31, (double)var27, var11, var15);
        var5.addVertexWithUV((double)var22, (double)var30, (double)var26, var11, var17);
        var5.addVertexWithUV((double)var21, (double)var29, (double)var25, var13, var17);
        var5.addVertexWithUV((double)var20, (double)var28, (double)var24, var13, var15);
        return true;
    }

    public bool renderBlockLadder(Block var1, int var2, int var3, int var4)
    {
        Tessellator var5 = getTessellator();
        int var6 = var1.getTexture(0);
        if (_overrideBlockTexture >= 0)
        {
            var6 = _overrideBlockTexture;
        }

        float var7 = var1.getLuminance(_blockAccess, var2, var3, var4);
        var5.setColorOpaque_F(var7, var7, var7);
        int var8 = (var6 & 15) << 4;
        int var9 = var6 & 240;
        double var10 = (double)(var8 / 256.0F);
        double var12 = (double)((var8 + 15.99F) / 256.0F);
        double var14 = (double)(var9 / 256.0F);
        double var16 = (double)((var9 + 15.99F) / 256.0F);
        int var18 = _blockAccess.getBlockMeta(var2, var3, var4);
        float var19 = 0.0F;
        float var20 = 0.05F;
        if (var18 == 5)
        {
            var5.addVertexWithUV((double)(var2 + var20), (double)(var3 + 1 + var19), (double)(var4 + 1 + var19), var10, var14);
            var5.addVertexWithUV((double)(var2 + var20), (double)(var3 + 0 - var19), (double)(var4 + 1 + var19), var10, var16);
            var5.addVertexWithUV((double)(var2 + var20), (double)(var3 + 0 - var19), (double)(var4 + 0 - var19), var12, var16);
            var5.addVertexWithUV((double)(var2 + var20), (double)(var3 + 1 + var19), (double)(var4 + 0 - var19), var12, var14);
        }

        if (var18 == 4)
        {
            var5.addVertexWithUV((double)(var2 + 1 - var20), (double)(var3 + 0 - var19), (double)(var4 + 1 + var19), var12, var16);
            var5.addVertexWithUV((double)(var2 + 1 - var20), (double)(var3 + 1 + var19), (double)(var4 + 1 + var19), var12, var14);
            var5.addVertexWithUV((double)(var2 + 1 - var20), (double)(var3 + 1 + var19), (double)(var4 + 0 - var19), var10, var14);
            var5.addVertexWithUV((double)(var2 + 1 - var20), (double)(var3 + 0 - var19), (double)(var4 + 0 - var19), var10, var16);
        }

        if (var18 == 3)
        {
            var5.addVertexWithUV((double)(var2 + 1 + var19), (double)(var3 + 0 - var19), (double)(var4 + var20), var12, var16);
            var5.addVertexWithUV((double)(var2 + 1 + var19), (double)(var3 + 1 + var19), (double)(var4 + var20), var12, var14);
            var5.addVertexWithUV((double)(var2 + 0 - var19), (double)(var3 + 1 + var19), (double)(var4 + var20), var10, var14);
            var5.addVertexWithUV((double)(var2 + 0 - var19), (double)(var3 + 0 - var19), (double)(var4 + var20), var10, var16);
        }

        if (var18 == 2)
        {
            var5.addVertexWithUV((double)(var2 + 1 + var19), (double)(var3 + 1 + var19), (double)(var4 + 1 - var20), var10, var14);
            var5.addVertexWithUV((double)(var2 + 1 + var19), (double)(var3 + 0 - var19), (double)(var4 + 1 - var20), var10, var16);
            var5.addVertexWithUV((double)(var2 + 0 - var19), (double)(var3 + 0 - var19), (double)(var4 + 1 - var20), var12, var16);
            var5.addVertexWithUV((double)(var2 + 0 - var19), (double)(var3 + 1 + var19), (double)(var4 + 1 - var20), var12, var14);
        }

        return true;
    }

    public bool renderBlockReed(Block var1, int var2, int var3, int var4)
    {
        Tessellator var5 = getTessellator();
        float var6 = var1.getLuminance(_blockAccess, var2, var3, var4);
        int var7 = var1.getColorMultiplier(_blockAccess, var2, var3, var4);
        float var8 = (var7 >> 16 & 255) / 255.0F;
        float var9 = (var7 >> 8 & 255) / 255.0F;
        float var10 = (var7 & 255) / 255.0F;

        var5.setColorOpaque_F(var6 * var8, var6 * var9, var6 * var10);
        double var19 = var2;
        double var20 = var3;
        double var15 = var4;
        if (var1 == Block.Grass)
        {
            long var17 = var2 * 3129871 ^ var4 * 116129781L ^ var3;
            var17 = var17 * var17 * 42317861L + var17 * 11L;
            var19 += ((double)((var17 >> 16 & 15L) / 15.0F) - 0.5D) * 0.5D;
            var20 += ((double)((var17 >> 20 & 15L) / 15.0F) - 1.0D) * 0.2D;
            var15 += ((double)((var17 >> 24 & 15L) / 15.0F) - 0.5D) * 0.5D;
        }

        renderCrossedSquares(var1, _blockAccess.getBlockMeta(var2, var3, var4), var19, var20, var15);
        return true;
    }

    public bool renderBlockCrops(Block var1, int var2, int var3, int var4)
    {
        Tessellator var5 = getTessellator();
        float var6 = var1.getLuminance(_blockAccess, var2, var3, var4);
        var5.setColorOpaque_F(var6, var6, var6);
        func_1245_b(var1, _blockAccess.getBlockMeta(var2, var3, var4), var2, (double)(var3 - 1.0F / 16.0F), var4);
        return true;
    }

    public void renderTorchAtAngle(Block var1, double var2, double var4, double var6, double var8, double var10)
    {
        Tessellator var12 = getTessellator();
        int var13 = var1.getTexture(0);
        if (_overrideBlockTexture >= 0)
        {
            var13 = _overrideBlockTexture;
        }

        int var14 = (var13 & 15) << 4;
        int var15 = var13 & 240;
        float var16 = var14 / 256.0F;
        float var17 = (var14 + 15.99F) / 256.0F;
        float var18 = var15 / 256.0F;
        float var19 = (var15 + 15.99F) / 256.0F;
        double var20 = (double)var16 + 1.75D / 64.0D;
        double var22 = (double)var18 + 6.0D / 256.0D;
        double var24 = (double)var16 + 9.0D / 256.0D;
        double var26 = (double)var18 + 1.0D / 32.0D;
        var2 += 0.5D;
        var6 += 0.5D;
        double var28 = var2 - 0.5D;
        double var30 = var2 + 0.5D;
        double var32 = var6 - 0.5D;
        double var34 = var6 + 0.5D;
        double var36 = 1.0D / 16.0D;
        double var38 = 0.625D;
        var12.addVertexWithUV(var2 + var8 * (1.0D - var38) - var36, var4 + var38, var6 + var10 * (1.0D - var38) - var36, var20, var22);
        var12.addVertexWithUV(var2 + var8 * (1.0D - var38) - var36, var4 + var38, var6 + var10 * (1.0D - var38) + var36, var20, var26);
        var12.addVertexWithUV(var2 + var8 * (1.0D - var38) + var36, var4 + var38, var6 + var10 * (1.0D - var38) + var36, var24, var26);
        var12.addVertexWithUV(var2 + var8 * (1.0D - var38) + var36, var4 + var38, var6 + var10 * (1.0D - var38) - var36, var24, var22);
        var12.addVertexWithUV(var2 - var36, var4 + 1.0D, var32, (double)var16, (double)var18);
        var12.addVertexWithUV(var2 - var36 + var8, var4 + 0.0D, var32 + var10, (double)var16, (double)var19);
        var12.addVertexWithUV(var2 - var36 + var8, var4 + 0.0D, var34 + var10, (double)var17, (double)var19);
        var12.addVertexWithUV(var2 - var36, var4 + 1.0D, var34, (double)var17, (double)var18);
        var12.addVertexWithUV(var2 + var36, var4 + 1.0D, var34, (double)var16, (double)var18);
        var12.addVertexWithUV(var2 + var8 + var36, var4 + 0.0D, var34 + var10, (double)var16, (double)var19);
        var12.addVertexWithUV(var2 + var8 + var36, var4 + 0.0D, var32 + var10, (double)var17, (double)var19);
        var12.addVertexWithUV(var2 + var36, var4 + 1.0D, var32, (double)var17, (double)var18);
        var12.addVertexWithUV(var28, var4 + 1.0D, var6 + var36, (double)var16, (double)var18);
        var12.addVertexWithUV(var28 + var8, var4 + 0.0D, var6 + var36 + var10, (double)var16, (double)var19);
        var12.addVertexWithUV(var30 + var8, var4 + 0.0D, var6 + var36 + var10, (double)var17, (double)var19);
        var12.addVertexWithUV(var30, var4 + 1.0D, var6 + var36, (double)var17, (double)var18);
        var12.addVertexWithUV(var30, var4 + 1.0D, var6 - var36, (double)var16, (double)var18);
        var12.addVertexWithUV(var30 + var8, var4 + 0.0D, var6 - var36 + var10, (double)var16, (double)var19);
        var12.addVertexWithUV(var28 + var8, var4 + 0.0D, var6 - var36 + var10, (double)var17, (double)var19);
        var12.addVertexWithUV(var28, var4 + 1.0D, var6 - var36, (double)var17, (double)var18);
    }

    public void renderCrossedSquares(Block var1, int var2, double var3, double var5, double var7)
    {
        Tessellator var9 = getTessellator();
        int var10 = var1.getTexture(0, var2);
        if (_overrideBlockTexture >= 0)
        {
            var10 = _overrideBlockTexture;
        }

        int var11 = (var10 & 15) << 4;
        int var12 = var10 & 240;
        double var13 = (double)(var11 / 256.0F);
        double var15 = (double)((var11 + 15.99F) / 256.0F);
        double var17 = (double)(var12 / 256.0F);
        double var19 = (double)((var12 + 15.99F) / 256.0F);
        double var21 = var3 + 0.5D - (double)0.45F;
        double var23 = var3 + 0.5D + (double)0.45F;
        double var25 = var7 + 0.5D - (double)0.45F;
        double var27 = var7 + 0.5D + (double)0.45F;
        var9.addVertexWithUV(var21, var5 + 1.0D, var25, var13, var17);
        var9.addVertexWithUV(var21, var5 + 0.0D, var25, var13, var19);
        var9.addVertexWithUV(var23, var5 + 0.0D, var27, var15, var19);
        var9.addVertexWithUV(var23, var5 + 1.0D, var27, var15, var17);
        var9.addVertexWithUV(var23, var5 + 1.0D, var27, var13, var17);
        var9.addVertexWithUV(var23, var5 + 0.0D, var27, var13, var19);
        var9.addVertexWithUV(var21, var5 + 0.0D, var25, var15, var19);
        var9.addVertexWithUV(var21, var5 + 1.0D, var25, var15, var17);
        var9.addVertexWithUV(var21, var5 + 1.0D, var27, var13, var17);
        var9.addVertexWithUV(var21, var5 + 0.0D, var27, var13, var19);
        var9.addVertexWithUV(var23, var5 + 0.0D, var25, var15, var19);
        var9.addVertexWithUV(var23, var5 + 1.0D, var25, var15, var17);
        var9.addVertexWithUV(var23, var5 + 1.0D, var25, var13, var17);
        var9.addVertexWithUV(var23, var5 + 0.0D, var25, var13, var19);
        var9.addVertexWithUV(var21, var5 + 0.0D, var27, var15, var19);
        var9.addVertexWithUV(var21, var5 + 1.0D, var27, var15, var17);
    }

    public void func_1245_b(Block var1, int var2, double var3, double var5, double var7)
    {
        Tessellator var9 = getTessellator();
        int var10 = var1.getTexture(0, var2);
        if (_overrideBlockTexture >= 0)
        {
            var10 = _overrideBlockTexture;
        }

        int var11 = (var10 & 15) << 4;
        int var12 = var10 & 240;
        double var13 = (double)(var11 / 256.0F);
        double var15 = (double)((var11 + 15.99F) / 256.0F);
        double var17 = (double)(var12 / 256.0F);
        double var19 = (double)((var12 + 15.99F) / 256.0F);
        double var21 = var3 + 0.5D - 0.25D;
        double var23 = var3 + 0.5D + 0.25D;
        double var25 = var7 + 0.5D - 0.5D;
        double var27 = var7 + 0.5D + 0.5D;
        var9.addVertexWithUV(var21, var5 + 1.0D, var25, var13, var17);
        var9.addVertexWithUV(var21, var5 + 0.0D, var25, var13, var19);
        var9.addVertexWithUV(var21, var5 + 0.0D, var27, var15, var19);
        var9.addVertexWithUV(var21, var5 + 1.0D, var27, var15, var17);
        var9.addVertexWithUV(var21, var5 + 1.0D, var27, var13, var17);
        var9.addVertexWithUV(var21, var5 + 0.0D, var27, var13, var19);
        var9.addVertexWithUV(var21, var5 + 0.0D, var25, var15, var19);
        var9.addVertexWithUV(var21, var5 + 1.0D, var25, var15, var17);
        var9.addVertexWithUV(var23, var5 + 1.0D, var27, var13, var17);
        var9.addVertexWithUV(var23, var5 + 0.0D, var27, var13, var19);
        var9.addVertexWithUV(var23, var5 + 0.0D, var25, var15, var19);
        var9.addVertexWithUV(var23, var5 + 1.0D, var25, var15, var17);
        var9.addVertexWithUV(var23, var5 + 1.0D, var25, var13, var17);
        var9.addVertexWithUV(var23, var5 + 0.0D, var25, var13, var19);
        var9.addVertexWithUV(var23, var5 + 0.0D, var27, var15, var19);
        var9.addVertexWithUV(var23, var5 + 1.0D, var27, var15, var17);
        var21 = var3 + 0.5D - 0.5D;
        var23 = var3 + 0.5D + 0.5D;
        var25 = var7 + 0.5D - 0.25D;
        var27 = var7 + 0.5D + 0.25D;
        var9.addVertexWithUV(var21, var5 + 1.0D, var25, var13, var17);
        var9.addVertexWithUV(var21, var5 + 0.0D, var25, var13, var19);
        var9.addVertexWithUV(var23, var5 + 0.0D, var25, var15, var19);
        var9.addVertexWithUV(var23, var5 + 1.0D, var25, var15, var17);
        var9.addVertexWithUV(var23, var5 + 1.0D, var25, var13, var17);
        var9.addVertexWithUV(var23, var5 + 0.0D, var25, var13, var19);
        var9.addVertexWithUV(var21, var5 + 0.0D, var25, var15, var19);
        var9.addVertexWithUV(var21, var5 + 1.0D, var25, var15, var17);
        var9.addVertexWithUV(var23, var5 + 1.0D, var27, var13, var17);
        var9.addVertexWithUV(var23, var5 + 0.0D, var27, var13, var19);
        var9.addVertexWithUV(var21, var5 + 0.0D, var27, var15, var19);
        var9.addVertexWithUV(var21, var5 + 1.0D, var27, var15, var17);
        var9.addVertexWithUV(var21, var5 + 1.0D, var27, var13, var17);
        var9.addVertexWithUV(var21, var5 + 0.0D, var27, var13, var19);
        var9.addVertexWithUV(var23, var5 + 0.0D, var27, var15, var19);
        var9.addVertexWithUV(var23, var5 + 1.0D, var27, var15, var17);
    }

    public bool renderBlockFluids(Block var1, int var2, int var3, int var4)
    {
        Tessellator var5 = getTessellator();
        Box blockBb = var1.BoundingBox;
        int var6 = var1.getColorMultiplier(_blockAccess, var2, var3, var4);
        float var7 = (var6 >> 16 & 255) / 255.0F;
        float var8 = (var6 >> 8 & 255) / 255.0F;
        float var9 = (var6 & 255) / 255.0F;
        bool var10 = var1.isSideVisible(_blockAccess, var2, var3 + 1, var4, 1);
        bool var11 = var1.isSideVisible(_blockAccess, var2, var3 - 1, var4, 0);
        bool[] var12 =
        [
            var1.isSideVisible(_blockAccess, var2, var3, var4 - 1, 2),
            var1.isSideVisible(_blockAccess, var2, var3, var4 + 1, 3),
            var1.isSideVisible(_blockAccess, var2 - 1, var3, var4, 4),
            var1.isSideVisible(_blockAccess, var2 + 1, var3, var4, 5)
        ];
        if (!var10 && !var11 && !var12[0] && !var12[1] && !var12[2] && !var12[3])
        {
            return false;
        }
        else
        {
            bool var13 = false;
            float var14 = 0.5F;
            float var15 = 1.0F;
            float var16 = 0.8F;
            float var17 = 0.6F;
            double var18 = 0.0D;
            double var20 = 1.0D;
            Material var22 = var1.material;
            int var23 = _blockAccess.getBlockMeta(var2, var3, var4);
            float var24 = func_1224_a(var2, var3, var4, var22);
            float var25 = func_1224_a(var2, var3, var4 + 1, var22);
            float var26 = func_1224_a(var2 + 1, var3, var4 + 1, var22);
            float var27 = func_1224_a(var2 + 1, var3, var4, var22);
            int var28;
            int var31;
            float var36;
            float var37;
            float var38;
            if (_renderAllFaces || var10)
            {
                var13 = true;
                var28 = var1.getTexture(1, var23);
                float var29 = (float)BlockFluid.getFlowingAngle(_blockAccess, var2, var3, var4, var22);
                if (var29 > -999.0F)
                {
                    var28 = var1.getTexture(2, var23);
                }

                int var30 = (var28 & 15) << 4;
                var31 = var28 & 240;
                double var32 = (var30 + 8.0D) / 256.0D;
                double var34 = (var31 + 8.0D) / 256.0D;
                if (var29 < -999.0F)
                {
                    var29 = 0.0F;
                }
                else
                {
                    var32 = (double)((var30 + 16) / 256.0F);
                    var34 = (double)((var31 + 16) / 256.0F);
                }

                var36 = MathHelper.Sin(var29) * 8.0F / 256.0F;
                var37 = MathHelper.Cos(var29) * 8.0F / 256.0F;
                var38 = var1.getLuminance(_blockAccess, var2, var3, var4);
                var5.setColorOpaque_F(var15 * var38 * var7, var15 * var38 * var8, var15 * var38 * var9);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + var24), var4 + 0, var32 - (double)var37 - (double)var36, var34 - (double)var37 + (double)var36);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + var25), var4 + 1, var32 - (double)var37 + (double)var36, var34 + (double)var37 + (double)var36);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + var26), var4 + 1, var32 + (double)var37 + (double)var36, var34 + (double)var37 - (double)var36);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + var27), var4 + 0, var32 + (double)var37 - (double)var36, var34 - (double)var37 - (double)var36);
            }

            if (_renderAllFaces || var11)
            {
                float var52 = var1.getLuminance(_blockAccess, var2, var3 - 1, var4);
                var5.setColorOpaque_F(var14 * var52, var14 * var52, var14 * var52);
                renderBottomFace(var1, var2, var3, var4, var1.getTexture(0));
                var13 = true;
            }

            for (var28 = 0; var28 < 4; ++var28)
            {
                int var53 = var2;
                var31 = var4;
                if (var28 == 0)
                {
                    var31 = var4 - 1;
                }

                if (var28 == 1)
                {
                    ++var31;
                }

                if (var28 == 2)
                {
                    var53 = var2 - 1;
                }

                if (var28 == 3)
                {
                    ++var53;
                }

                int var54 = var1.getTexture(var28 + 2, var23);
                int var33 = (var54 & 15) << 4;
                int var55 = var54 & 240;
                if (_renderAllFaces || var12[var28])
                {
                    float var35;
                    float var39;
                    float var40;
                    if (var28 == 0)
                    {
                        var35 = var24;
                        var36 = var27;
                        var37 = var2;
                        var39 = var2 + 1;
                        var38 = var4;
                        var40 = var4;
                    }
                    else if (var28 == 1)
                    {
                        var35 = var26;
                        var36 = var25;
                        var37 = var2 + 1;
                        var39 = var2;
                        var38 = var4 + 1;
                        var40 = var4 + 1;
                    }
                    else if (var28 == 2)
                    {
                        var35 = var25;
                        var36 = var24;
                        var37 = var2;
                        var39 = var2;
                        var38 = var4 + 1;
                        var40 = var4;
                    }
                    else
                    {
                        var35 = var27;
                        var36 = var26;
                        var37 = var2 + 1;
                        var39 = var2 + 1;
                        var38 = var4;
                        var40 = var4 + 1;
                    }

                    var13 = true;
                    double var41 = (double)((var33 + 0) / 256.0F);
                    double var43 = (var33 + 16 - 0.01D) / 256.0D;
                    double var45 = (double)((var55 + (1.0F - var35) * 16.0F) / 256.0F);
                    double var47 = (double)((var55 + (1.0F - var36) * 16.0F) / 256.0F);
                    double var49 = (var55 + 16 - 0.01D) / 256.0D;
                    float var51 = var1.getLuminance(_blockAccess, var53, var3, var31);
                    if (var28 < 2)
                    {
                        var51 *= var16;
                    }
                    else
                    {
                        var51 *= var17;
                    }

                    var5.setColorOpaque_F(var15 * var51 * var7, var15 * var51 * var8, var15 * var51 * var9);
                    var5.addVertexWithUV((double)var37, (double)(var3 + var35), (double)var38, var41, var45);
                    var5.addVertexWithUV((double)var39, (double)(var3 + var36), (double)var40, var43, var47);
                    var5.addVertexWithUV((double)var39, var3 + 0, (double)var40, var43, var49);
                    var5.addVertexWithUV((double)var37, var3 + 0, (double)var38, var41, var49);
                }
            }

            blockBb.MinY = var18;
            blockBb.MaxY = var20;
            return var13;
        }
    }

    private float func_1224_a(int var1, int var2, int var3, Material var4)
    {
        int var5 = 0;
        float var6 = 0.0F;

        for (int var7 = 0; var7 < 4; ++var7)
        {
            int var8 = var1 - (var7 & 1);
            int var10 = var3 - (var7 >> 1 & 1);
            if (_blockAccess.getMaterial(var8, var2 + 1, var10) == var4)
            {
                return 1.0F;
            }

            Material var11 = _blockAccess.getMaterial(var8, var2, var10);
            if (var11 != var4)
            {
                if (!var11.IsSolid)
                {
                    ++var6;
                    ++var5;
                }
            }
            else
            {
                int var12 = _blockAccess.getBlockMeta(var8, var2, var10);
                if (var12 >= 8 || var12 == 0)
                {
                    var6 += BlockFluid.getFluidHeightFromMeta(var12) * 10.0F;
                    var5 += 10;
                }

                var6 += BlockFluid.getFluidHeightFromMeta(var12);
                ++var5;
            }
        }

        return 1.0F - var6 / var5;
    }

    public void renderBlockFallingSand(Block var1, World var2, int var3, int var4, int var5)
    {
        float var6 = 0.5F;
        float var7 = 1.0F;
        float var8 = 0.8F;
        float var9 = 0.6F;
        Tessellator var10 = getTessellator();
        var10.startDrawingQuads();
        float var11 = var1.getLuminance(var2, var3, var4, var5);
        float var12 = var1.getLuminance(var2, var3, var4 - 1, var5);
        if (var12 < var11)
        {
            var12 = var11;
        }

        var10.setColorOpaque_F(var6 * var12, var6 * var12, var6 * var12);
        renderBottomFace(var1, -0.5D, -0.5D, -0.5D, var1.getTexture(0));
        var12 = var1.getLuminance(var2, var3, var4 + 1, var5);
        if (var12 < var11)
        {
            var12 = var11;
        }

        var10.setColorOpaque_F(var7 * var12, var7 * var12, var7 * var12);
        renderTopFace(var1, -0.5D, -0.5D, -0.5D, var1.getTexture(1));
        var12 = var1.getLuminance(var2, var3, var4, var5 - 1);
        if (var12 < var11)
        {
            var12 = var11;
        }

        var10.setColorOpaque_F(var8 * var12, var8 * var12, var8 * var12);
        renderEastFace(var1, -0.5D, -0.5D, -0.5D, var1.getTexture(2));
        var12 = var1.getLuminance(var2, var3, var4, var5 + 1);
        if (var12 < var11)
        {
            var12 = var11;
        }

        var10.setColorOpaque_F(var8 * var12, var8 * var12, var8 * var12);
        renderWestFace(var1, -0.5D, -0.5D, -0.5D, var1.getTexture(3));
        var12 = var1.getLuminance(var2, var3 - 1, var4, var5);
        if (var12 < var11)
        {
            var12 = var11;
        }

        var10.setColorOpaque_F(var9 * var12, var9 * var12, var9 * var12);
        renderNorthFace(var1, -0.5D, -0.5D, -0.5D, var1.getTexture(4));
        var12 = var1.getLuminance(var2, var3 + 1, var4, var5);
        if (var12 < var11)
        {
            var12 = var11;
        }

        var10.setColorOpaque_F(var9 * var12, var9 * var12, var9 * var12);
        renderSouthFace(var1, -0.5D, -0.5D, -0.5D, var1.getTexture(5));
        var10.draw();
    }

    public bool renderStandardBlock(Block var1, int var2, int var3, int var4)
    {
        int var5 = var1.getColorMultiplier(_blockAccess, var2, var3, var4);
        float var6 = (var5 >> 16 & 255) / 255.0F;
        float var7 = (var5 >> 8 & 255) / 255.0F;
        float var8 = (var5 & 255) / 255.0F;

        return Minecraft.isAmbientOcclusionEnabled() ? renderStandardBlockWithAmbientOcclusion(var1, var2, var3, var4, var6, var7, var8) : renderStandardBlockWithColorMultiplier(var1, var2, var3, var4, var6, var7, var8);
    }

    public bool renderStandardBlockWithAmbientOcclusion(Block var1, int var2, int var3, int var4, float var5, float var6, float var7)
    {
        _enableAO = true;
        bool var8 = false;
        float var9 = _lightValueOwn;
        float var10 = _lightValueOwn;
        float var11 = _lightValueOwn;
        float var12 = _lightValueOwn;
        bool var13 = true;
        bool var14 = true;
        bool var15 = true;
        bool var16 = true;
        bool var17 = true;
        bool var18 = true;
        _lightValueOwn = var1.getLuminance(_blockAccess, var2, var3, var4);
        _aoLightValueXNeg = var1.getLuminance(_blockAccess, var2 - 1, var3, var4);
        _aoLightValueYNeg = var1.getLuminance(_blockAccess, var2, var3 - 1, var4);
        _aoLightValueZNeg = var1.getLuminance(_blockAccess, var2, var3, var4 - 1);
        _aoLightValueXPos = var1.getLuminance(_blockAccess, var2 + 1, var3, var4);
        _aoLightValueYPos = var1.getLuminance(_blockAccess, var2, var3 + 1, var4);
        _aoLightValueZPos = var1.getLuminance(_blockAccess, var2, var3, var4 + 1);
        _aoBlockOpXPosYPos = Block.BlocksAllowVision[_blockAccess.getBlockId(var2 + 1, var3 + 1, var4)];
        _aoBlockOpYPosZNeg = Block.BlocksAllowVision[_blockAccess.getBlockId(var2 + 1, var3 - 1, var4)];
        _aoBlockOpXPosZNeg = Block.BlocksAllowVision[_blockAccess.getBlockId(var2 + 1, var3, var4 + 1)];
        _aoBlockOpXPosZPos = Block.BlocksAllowVision[_blockAccess.getBlockId(var2 + 1, var3, var4 - 1)];
        _aoBlockOpXNegYNeg = Block.BlocksAllowVision[_blockAccess.getBlockId(var2 - 1, var3 + 1, var4)];
        _aoBlockOpYNegZPos = Block.BlocksAllowVision[_blockAccess.getBlockId(var2 - 1, var3 - 1, var4)];
        _aoBlockOpXNegZNeg = Block.BlocksAllowVision[_blockAccess.getBlockId(var2 - 1, var3, var4 - 1)];
        _aoBlockOpXNegZPos = Block.BlocksAllowVision[_blockAccess.getBlockId(var2 - 1, var3, var4 + 1)];
        _aoBlockOpXPosYNeg = Block.BlocksAllowVision[_blockAccess.getBlockId(var2, var3 + 1, var4 + 1)];
        _aoBlockOpXNegYPos = Block.BlocksAllowVision[_blockAccess.getBlockId(var2, var3 + 1, var4 - 1)];
        _aoBlockOpYPosZPos = Block.BlocksAllowVision[_blockAccess.getBlockId(var2, var3 - 1, var4 + 1)];
        _aoBlockOpYNegZNeg = Block.BlocksAllowVision[_blockAccess.getBlockId(var2, var3 - 1, var4 - 1)];
        if (var1.textureId == 3)
        {
            var18 = false;
            var17 = var18;
            var16 = var18;
            var15 = var18;
            var13 = var18;
        }

        if (_overrideBlockTexture >= 0)
        {
            var18 = false;
            var17 = var18;
            var16 = var18;
            var15 = var18;
            var13 = var18;
        }

        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3 - 1, var4, 0))
        {
            if (_aoBlendMode <= 0)
            {
                var12 = _aoLightValueYNeg;
                var11 = var12;
                var10 = var12;
                var9 = var12;
            }
            else
            {
                --var3;
                _colorRedBottomLeft_V = var1.getLuminance(_blockAccess, var2 - 1, var3, var4);
                _colorRedTopRight_V = var1.getLuminance(_blockAccess, var2, var3, var4 - 1);
                _colorGreenTopLeft_V = var1.getLuminance(_blockAccess, var2, var3, var4 + 1);
                _colorGreenBottomRight_V = var1.getLuminance(_blockAccess, var2 + 1, var3, var4);
                if (!_aoBlockOpYNegZNeg && !_aoBlockOpYNegZPos)
                {
                    _colorRedTopLeft_V = _colorRedBottomLeft_V;
                }
                else
                {
                    _colorRedTopLeft_V = var1.getLuminance(_blockAccess, var2 - 1, var3, var4 - 1);
                }

                if (!_aoBlockOpYPosZPos && !_aoBlockOpYNegZPos)
                {
                    _colorRedBottomRight_V = _colorRedBottomLeft_V;
                }
                else
                {
                    _colorRedBottomRight_V = var1.getLuminance(_blockAccess, var2 - 1, var3, var4 + 1);
                }

                if (!_aoBlockOpYNegZNeg && !_aoBlockOpYPosZNeg)
                {
                    _colorGreenBottomLeft_V = _colorGreenBottomRight_V;
                }
                else
                {
                    _colorGreenBottomLeft_V = var1.getLuminance(_blockAccess, var2 + 1, var3, var4 - 1);
                }

                if (!_aoBlockOpYPosZPos && !_aoBlockOpYPosZNeg)
                {
                    _colorGreenTopRight_V = _colorGreenBottomRight_V;
                }
                else
                {
                    _colorGreenTopRight_V = var1.getLuminance(_blockAccess, var2 + 1, var3, var4 + 1);
                }

                ++var3;
                var9 = (_colorRedBottomRight_V + _colorRedBottomLeft_V + _colorGreenTopLeft_V + _aoLightValueYNeg) / 4.0F;
                var12 = (_colorGreenTopLeft_V + _aoLightValueYNeg + _colorGreenTopRight_V + _colorGreenBottomRight_V) / 4.0F;
                var11 = (_aoLightValueYNeg + _colorRedTopRight_V + _colorGreenBottomRight_V + _colorGreenBottomLeft_V) / 4.0F;
                var10 = (_colorRedBottomLeft_V + _colorRedTopLeft_V + _aoLightValueYNeg + _colorRedTopRight_V) / 4.0F;
            }

            _colorRedTopLeft = _colorRedBottomLeft = _colorRedBottomRight = _colorRedTopRight = (var13 ? var5 : 1.0F) * 0.5F;
            _colorGreenTopLeft = _colorGreenBottomLeft = _colorGreenBottomRight = _colorGreenTopRight = (var13 ? var6 : 1.0F) * 0.5F;
            _colorBlueTopLeft = _colorBlueBottomLeft = _colorBlueBottomRight = _colorBlueTopRight = (var13 ? var7 : 1.0F) * 0.5F;
            _colorRedTopLeft *= var9;
            _colorGreenTopLeft *= var9;
            _colorBlueTopLeft *= var9;
            _colorRedBottomLeft *= var10;
            _colorGreenBottomLeft *= var10;
            _colorBlueBottomLeft *= var10;
            _colorRedBottomRight *= var11;
            _colorGreenBottomRight *= var11;
            _colorBlueBottomRight *= var11;
            _colorRedTopRight *= var12;
            _colorGreenTopRight *= var12;
            _colorBlueTopRight *= var12;
            renderBottomFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, 0));
            var8 = true;
        }

        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3 + 1, var4, 1))
        {
            if (_aoBlendMode <= 0)
            {
                var12 = _aoLightValueYPos;
                var11 = var12;
                var10 = var12;
                var9 = var12;
            }
            else
            {
                ++var3;
                _colorBlueBottomLeft_V = var1.getLuminance(_blockAccess, var2 - 1, var3, var4);
                _aoLightValueScratch2 = var1.getLuminance(_blockAccess, var2 + 1, var3, var4);
                _colorBlueTopRight_V = var1.getLuminance(_blockAccess, var2, var3, var4 - 1);
                _aoLightValueScratch3 = var1.getLuminance(_blockAccess, var2, var3, var4 + 1);
                if (!_aoBlockOpXNegYPos && !_aoBlockOpXNegYNeg)
                {
                    _colorBlueTopLeft_V = _colorBlueBottomLeft_V;
                }
                else
                {
                    _colorBlueTopLeft_V = var1.getLuminance(_blockAccess, var2 - 1, var3, var4 - 1);
                }

                if (!_aoBlockOpXNegYPos && !_aoBlockOpXPosYPos)
                {
                    _aoLightValueScratch1 = _aoLightValueScratch2;
                }
                else
                {
                    _aoLightValueScratch1 = var1.getLuminance(_blockAccess, var2 + 1, var3, var4 - 1);
                }

                if (!_aoBlockOpXPosYNeg && !_aoBlockOpXNegYNeg)
                {
                    _colorBlueBottomRight_V = _colorBlueBottomLeft_V;
                }
                else
                {
                    _colorBlueBottomRight_V = var1.getLuminance(_blockAccess, var2 - 1, var3, var4 + 1);
                }

                if (!_aoBlockOpXPosYNeg && !_aoBlockOpXPosYPos)
                {
                    _aoLightValueScratch4 = _aoLightValueScratch2;
                }
                else
                {
                    _aoLightValueScratch4 = var1.getLuminance(_blockAccess, var2 + 1, var3, var4 + 1);
                }

                --var3;
                var12 = (_colorBlueBottomRight_V + _colorBlueBottomLeft_V + _aoLightValueScratch3 + _aoLightValueYPos) / 4.0F;
                var9 = (_aoLightValueScratch3 + _aoLightValueYPos + _aoLightValueScratch4 + _aoLightValueScratch2) / 4.0F;
                var10 = (_aoLightValueYPos + _colorBlueTopRight_V + _aoLightValueScratch2 + _aoLightValueScratch1) / 4.0F;
                var11 = (_colorBlueBottomLeft_V + _colorBlueTopLeft_V + _aoLightValueYPos + _colorBlueTopRight_V) / 4.0F;
            }

            _colorRedTopLeft = _colorRedBottomLeft = _colorRedBottomRight = _colorRedTopRight = var14 ? var5 : 1.0F;
            _colorGreenTopLeft = _colorGreenBottomLeft = _colorGreenBottomRight = _colorGreenTopRight = var14 ? var6 : 1.0F;
            _colorBlueTopLeft = _colorBlueBottomLeft = _colorBlueBottomRight = _colorBlueTopRight = var14 ? var7 : 1.0F;
            _colorRedTopLeft *= var9;
            _colorGreenTopLeft *= var9;
            _colorBlueTopLeft *= var9;
            _colorRedBottomLeft *= var10;
            _colorGreenBottomLeft *= var10;
            _colorBlueBottomLeft *= var10;
            _colorRedBottomRight *= var11;
            _colorGreenBottomRight *= var11;
            _colorBlueBottomRight *= var11;
            _colorRedTopRight *= var12;
            _colorGreenTopRight *= var12;
            _colorBlueTopRight *= var12;
            renderTopFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, 1));
            var8 = true;
        }

        int var19;
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3, var4 - 1, 2))
        {
            if (_aoBlendMode <= 0)
            {
                var12 = _aoLightValueZNeg;
                var11 = var12;
                var10 = var12;
                var9 = var12;
            }
            else
            {
                --var4;
                _aoLightValueScratch5 = var1.getLuminance(_blockAccess, var2 - 1, var3, var4);
                _colorRedTopRight_V = var1.getLuminance(_blockAccess, var2, var3 - 1, var4);
                _colorBlueTopRight_V = var1.getLuminance(_blockAccess, var2, var3 + 1, var4);
                _aoLightValueScratch6 = var1.getLuminance(_blockAccess, var2 + 1, var3, var4);
                if (!_aoBlockOpXNegZNeg && !_aoBlockOpYNegZNeg)
                {
                    _colorRedTopLeft_V = _aoLightValueScratch5;
                }
                else
                {
                    _colorRedTopLeft_V = var1.getLuminance(_blockAccess, var2 - 1, var3 - 1, var4);
                }

                if (!_aoBlockOpXNegZNeg && !_aoBlockOpXNegYPos)
                {
                    _colorBlueTopLeft_V = _aoLightValueScratch5;
                }
                else
                {
                    _colorBlueTopLeft_V = var1.getLuminance(_blockAccess, var2 - 1, var3 + 1, var4);
                }

                if (!_aoBlockOpXPosZPos && !_aoBlockOpYNegZNeg)
                {
                    _colorGreenBottomLeft_V = _aoLightValueScratch6;
                }
                else
                {
                    _colorGreenBottomLeft_V = var1.getLuminance(_blockAccess, var2 + 1, var3 - 1, var4);
                }

                if (!_aoBlockOpXPosZPos && !_aoBlockOpXNegYPos)
                {
                    _aoLightValueScratch1 = _aoLightValueScratch6;
                }
                else
                {
                    _aoLightValueScratch1 = var1.getLuminance(_blockAccess, var2 + 1, var3 + 1, var4);
                }

                ++var4;
                var9 = (_aoLightValueScratch5 + _colorBlueTopLeft_V + _aoLightValueZNeg + _colorBlueTopRight_V) / 4.0F;
                var10 = (_aoLightValueZNeg + _colorBlueTopRight_V + _aoLightValueScratch6 + _aoLightValueScratch1) / 4.0F;
                var11 = (_colorRedTopRight_V + _aoLightValueZNeg + _colorGreenBottomLeft_V + _aoLightValueScratch6) / 4.0F;
                var12 = (_colorRedTopLeft_V + _aoLightValueScratch5 + _colorRedTopRight_V + _aoLightValueZNeg) / 4.0F;
            }

            _colorRedTopLeft = _colorRedBottomLeft = _colorRedBottomRight = _colorRedTopRight = (var15 ? var5 : 1.0F) * 0.8F;
            _colorGreenTopLeft = _colorGreenBottomLeft = _colorGreenBottomRight = _colorGreenTopRight = (var15 ? var6 : 1.0F) * 0.8F;
            _colorBlueTopLeft = _colorBlueBottomLeft = _colorBlueBottomRight = _colorBlueTopRight = (var15 ? var7 : 1.0F) * 0.8F;
            _colorRedTopLeft *= var9;
            _colorGreenTopLeft *= var9;
            _colorBlueTopLeft *= var9;
            _colorRedBottomLeft *= var10;
            _colorGreenBottomLeft *= var10;
            _colorBlueBottomLeft *= var10;
            _colorRedBottomRight *= var11;
            _colorGreenBottomRight *= var11;
            _colorBlueBottomRight *= var11;
            _colorRedTopRight *= var12;
            _colorGreenTopRight *= var12;
            _colorBlueTopRight *= var12;
            var19 = var1.getTextureId(_blockAccess, var2, var3, var4, 2);
            renderEastFace(var1, var2, var3, var4, var19);
            if (s_fancyGrass && var19 == 3 && _overrideBlockTexture < 0)
            {
                _colorRedTopLeft *= var5;
                _colorRedBottomLeft *= var5;
                _colorRedBottomRight *= var5;
                _colorRedTopRight *= var5;
                _colorGreenTopLeft *= var6;
                _colorGreenBottomLeft *= var6;
                _colorGreenBottomRight *= var6;
                _colorGreenTopRight *= var6;
                _colorBlueTopLeft *= var7;
                _colorBlueBottomLeft *= var7;
                _colorBlueBottomRight *= var7;
                _colorBlueTopRight *= var7;
                renderEastFace(var1, var2, var3, var4, 38);
            }

            var8 = true;
        }

        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3, var4 + 1, 3))
        {
            if (_aoBlendMode <= 0)
            {
                var12 = _aoLightValueZPos;
                var11 = var12;
                var10 = var12;
                var9 = var12;
            }
            else
            {
                ++var4;
                _aoLightValueScratch7 = var1.getLuminance(_blockAccess, var2 - 1, var3, var4);
                _aoLightValueScratch8 = var1.getLuminance(_blockAccess, var2 + 1, var3, var4);
                _colorGreenTopLeft_V = var1.getLuminance(_blockAccess, var2, var3 - 1, var4);
                _aoLightValueScratch3 = var1.getLuminance(_blockAccess, var2, var3 + 1, var4);
                if (!_aoBlockOpXNegZPos && !_aoBlockOpYPosZPos)
                {
                    _colorRedBottomRight_V = _aoLightValueScratch7;
                }
                else
                {
                    _colorRedBottomRight_V = var1.getLuminance(_blockAccess, var2 - 1, var3 - 1, var4);
                }

                if (!_aoBlockOpXNegZPos && !_aoBlockOpXPosYNeg)
                {
                    _colorBlueBottomRight_V = _aoLightValueScratch7;
                }
                else
                {
                    _colorBlueBottomRight_V = var1.getLuminance(_blockAccess, var2 - 1, var3 + 1, var4);
                }

                if (!_aoBlockOpXPosZNeg && !_aoBlockOpYPosZPos)
                {
                    _colorGreenTopRight_V = _aoLightValueScratch8;
                }
                else
                {
                    _colorGreenTopRight_V = var1.getLuminance(_blockAccess, var2 + 1, var3 - 1, var4);
                }

                if (!_aoBlockOpXPosZNeg && !_aoBlockOpXPosYNeg)
                {
                    _aoLightValueScratch4 = _aoLightValueScratch8;
                }
                else
                {
                    _aoLightValueScratch4 = var1.getLuminance(_blockAccess, var2 + 1, var3 + 1, var4);
                }

                --var4;
                var9 = (_aoLightValueScratch7 + _colorBlueBottomRight_V + _aoLightValueZPos + _aoLightValueScratch3) / 4.0F;
                var12 = (_aoLightValueZPos + _aoLightValueScratch3 + _aoLightValueScratch8 + _aoLightValueScratch4) / 4.0F;
                var11 = (_colorGreenTopLeft_V + _aoLightValueZPos + _colorGreenTopRight_V + _aoLightValueScratch8) / 4.0F;
                var10 = (_colorRedBottomRight_V + _aoLightValueScratch7 + _colorGreenTopLeft_V + _aoLightValueZPos) / 4.0F;
            }

            _colorRedTopLeft = _colorRedBottomLeft = _colorRedBottomRight = _colorRedTopRight = (var16 ? var5 : 1.0F) * 0.8F;
            _colorGreenTopLeft = _colorGreenBottomLeft = _colorGreenBottomRight = _colorGreenTopRight = (var16 ? var6 : 1.0F) * 0.8F;
            _colorBlueTopLeft = _colorBlueBottomLeft = _colorBlueBottomRight = _colorBlueTopRight = (var16 ? var7 : 1.0F) * 0.8F;
            _colorRedTopLeft *= var9;
            _colorGreenTopLeft *= var9;
            _colorBlueTopLeft *= var9;
            _colorRedBottomLeft *= var10;
            _colorGreenBottomLeft *= var10;
            _colorBlueBottomLeft *= var10;
            _colorRedBottomRight *= var11;
            _colorGreenBottomRight *= var11;
            _colorBlueBottomRight *= var11;
            _colorRedTopRight *= var12;
            _colorGreenTopRight *= var12;
            _colorBlueTopRight *= var12;
            var19 = var1.getTextureId(_blockAccess, var2, var3, var4, 3);
            renderWestFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, 3));
            if (s_fancyGrass && var19 == 3 && _overrideBlockTexture < 0)
            {
                _colorRedTopLeft *= var5;
                _colorRedBottomLeft *= var5;
                _colorRedBottomRight *= var5;
                _colorRedTopRight *= var5;
                _colorGreenTopLeft *= var6;
                _colorGreenBottomLeft *= var6;
                _colorGreenBottomRight *= var6;
                _colorGreenTopRight *= var6;
                _colorBlueTopLeft *= var7;
                _colorBlueBottomLeft *= var7;
                _colorBlueBottomRight *= var7;
                _colorBlueTopRight *= var7;
                renderWestFace(var1, var2, var3, var4, 38);
            }

            var8 = true;
        }

        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2 - 1, var3, var4, 4))
        {
            if (_aoBlendMode <= 0)
            {
                var12 = _aoLightValueXNeg;
                var11 = var12;
                var10 = var12;
                var9 = var12;
            }
            else
            {
                --var2;
                _colorRedBottomLeft_V = var1.getLuminance(_blockAccess, var2, var3 - 1, var4);
                _aoLightValueScratch5 = var1.getLuminance(_blockAccess, var2, var3, var4 - 1);
                _aoLightValueScratch7 = var1.getLuminance(_blockAccess, var2, var3, var4 + 1);
                _colorBlueBottomLeft_V = var1.getLuminance(_blockAccess, var2, var3 + 1, var4);
                if (!_aoBlockOpXNegZNeg && !_aoBlockOpYNegZPos)
                {
                    _colorRedTopLeft_V = _aoLightValueScratch5;
                }
                else
                {
                    _colorRedTopLeft_V = var1.getLuminance(_blockAccess, var2, var3 - 1, var4 - 1);
                }

                if (!_aoBlockOpXNegZPos && !_aoBlockOpYNegZPos)
                {
                    _colorRedBottomRight_V = _aoLightValueScratch7;
                }
                else
                {
                    _colorRedBottomRight_V = var1.getLuminance(_blockAccess, var2, var3 - 1, var4 + 1);
                }

                if (!_aoBlockOpXNegZNeg && !_aoBlockOpXNegYNeg)
                {
                    _colorBlueTopLeft_V = _aoLightValueScratch5;
                }
                else
                {
                    _colorBlueTopLeft_V = var1.getLuminance(_blockAccess, var2, var3 + 1, var4 - 1);
                }

                if (!_aoBlockOpXNegZPos && !_aoBlockOpXNegYNeg)
                {
                    _colorBlueBottomRight_V = _aoLightValueScratch7;
                }
                else
                {
                    _colorBlueBottomRight_V = var1.getLuminance(_blockAccess, var2, var3 + 1, var4 + 1);
                }

                ++var2;
                var12 = (_colorRedBottomLeft_V + _colorRedBottomRight_V + _aoLightValueXNeg + _aoLightValueScratch7) / 4.0F;
                var9 = (_aoLightValueXNeg + _aoLightValueScratch7 + _colorBlueBottomLeft_V + _colorBlueBottomRight_V) / 4.0F;
                var10 = (_aoLightValueScratch5 + _aoLightValueXNeg + _colorBlueTopLeft_V + _colorBlueBottomLeft_V) / 4.0F;
                var11 = (_colorRedTopLeft_V + _colorRedBottomLeft_V + _aoLightValueScratch5 + _aoLightValueXNeg) / 4.0F;
            }

            _colorRedTopLeft = _colorRedBottomLeft = _colorRedBottomRight = _colorRedTopRight = (var17 ? var5 : 1.0F) * 0.6F;
            _colorGreenTopLeft = _colorGreenBottomLeft = _colorGreenBottomRight = _colorGreenTopRight = (var17 ? var6 : 1.0F) * 0.6F;
            _colorBlueTopLeft = _colorBlueBottomLeft = _colorBlueBottomRight = _colorBlueTopRight = (var17 ? var7 : 1.0F) * 0.6F;
            _colorRedTopLeft *= var9;
            _colorGreenTopLeft *= var9;
            _colorBlueTopLeft *= var9;
            _colorRedBottomLeft *= var10;
            _colorGreenBottomLeft *= var10;
            _colorBlueBottomLeft *= var10;
            _colorRedBottomRight *= var11;
            _colorGreenBottomRight *= var11;
            _colorBlueBottomRight *= var11;
            _colorRedTopRight *= var12;
            _colorGreenTopRight *= var12;
            _colorBlueTopRight *= var12;
            var19 = var1.getTextureId(_blockAccess, var2, var3, var4, 4);
            renderNorthFace(var1, var2, var3, var4, var19);
            if (s_fancyGrass && var19 == 3 && _overrideBlockTexture < 0)
            {
                _colorRedTopLeft *= var5;
                _colorRedBottomLeft *= var5;
                _colorRedBottomRight *= var5;
                _colorRedTopRight *= var5;
                _colorGreenTopLeft *= var6;
                _colorGreenBottomLeft *= var6;
                _colorGreenBottomRight *= var6;
                _colorGreenTopRight *= var6;
                _colorBlueTopLeft *= var7;
                _colorBlueBottomLeft *= var7;
                _colorBlueBottomRight *= var7;
                _colorBlueTopRight *= var7;
                renderNorthFace(var1, var2, var3, var4, 38);
            }

            var8 = true;
        }

        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2 + 1, var3, var4, 5))
        {
            if (_aoBlendMode <= 0)
            {
                var12 = _aoLightValueXPos;
                var11 = var12;
                var10 = var12;
                var9 = var12;
            }
            else
            {
                ++var2;
                _colorGreenBottomRight_V = var1.getLuminance(_blockAccess, var2, var3 - 1, var4);
                _aoLightValueScratch6 = var1.getLuminance(_blockAccess, var2, var3, var4 - 1);
                _aoLightValueScratch8 = var1.getLuminance(_blockAccess, var2, var3, var4 + 1);
                _aoLightValueScratch2 = var1.getLuminance(_blockAccess, var2, var3 + 1, var4);
                if (!_aoBlockOpYPosZNeg && !_aoBlockOpXPosZPos)
                {
                    _colorGreenBottomLeft_V = _aoLightValueScratch6;
                }
                else
                {
                    _colorGreenBottomLeft_V = var1.getLuminance(_blockAccess, var2, var3 - 1, var4 - 1);
                }

                if (!_aoBlockOpYPosZNeg && !_aoBlockOpXPosZNeg)
                {
                    _colorGreenTopRight_V = _aoLightValueScratch8;
                }
                else
                {
                    _colorGreenTopRight_V = var1.getLuminance(_blockAccess, var2, var3 - 1, var4 + 1);
                }

                if (!_aoBlockOpXPosYPos && !_aoBlockOpXPosZPos)
                {
                    _aoLightValueScratch1 = _aoLightValueScratch6;
                }
                else
                {
                    _aoLightValueScratch1 = var1.getLuminance(_blockAccess, var2, var3 + 1, var4 - 1);
                }

                if (!_aoBlockOpXPosYPos && !_aoBlockOpXPosZNeg)
                {
                    _aoLightValueScratch4 = _aoLightValueScratch8;
                }
                else
                {
                    _aoLightValueScratch4 = var1.getLuminance(_blockAccess, var2, var3 + 1, var4 + 1);
                }

                --var2;
                var9 = (_colorGreenBottomRight_V + _colorGreenTopRight_V + _aoLightValueXPos + _aoLightValueScratch8) / 4.0F;
                var12 = (_aoLightValueXPos + _aoLightValueScratch8 + _aoLightValueScratch2 + _aoLightValueScratch4) / 4.0F;
                var11 = (_aoLightValueScratch6 + _aoLightValueXPos + _aoLightValueScratch1 + _aoLightValueScratch2) / 4.0F;
                var10 = (_colorGreenBottomLeft_V + _colorGreenBottomRight_V + _aoLightValueScratch6 + _aoLightValueXPos) / 4.0F;
            }

            _colorRedTopLeft = _colorRedBottomLeft = _colorRedBottomRight = _colorRedTopRight = (var18 ? var5 : 1.0F) * 0.6F;
            _colorGreenTopLeft = _colorGreenBottomLeft = _colorGreenBottomRight = _colorGreenTopRight = (var18 ? var6 : 1.0F) * 0.6F;
            _colorBlueTopLeft = _colorBlueBottomLeft = _colorBlueBottomRight = _colorBlueTopRight = (var18 ? var7 : 1.0F) * 0.6F;
            _colorRedTopLeft *= var9;
            _colorGreenTopLeft *= var9;
            _colorBlueTopLeft *= var9;
            _colorRedBottomLeft *= var10;
            _colorGreenBottomLeft *= var10;
            _colorBlueBottomLeft *= var10;
            _colorRedBottomRight *= var11;
            _colorGreenBottomRight *= var11;
            _colorBlueBottomRight *= var11;
            _colorRedTopRight *= var12;
            _colorGreenTopRight *= var12;
            _colorBlueTopRight *= var12;
            var19 = var1.getTextureId(_blockAccess, var2, var3, var4, 5);
            renderSouthFace(var1, var2, var3, var4, var19);
            if (s_fancyGrass && var19 == 3 && _overrideBlockTexture < 0)
            {
                _colorRedTopLeft *= var5;
                _colorRedBottomLeft *= var5;
                _colorRedBottomRight *= var5;
                _colorRedTopRight *= var5;
                _colorGreenTopLeft *= var6;
                _colorGreenBottomLeft *= var6;
                _colorGreenBottomRight *= var6;
                _colorGreenTopRight *= var6;
                _colorBlueTopLeft *= var7;
                _colorBlueBottomLeft *= var7;
                _colorBlueBottomRight *= var7;
                _colorBlueTopRight *= var7;
                renderSouthFace(var1, var2, var3, var4, 38);
            }

            var8 = true;
        }

        _enableAO = false;
        return var8;
    }

    public bool renderStandardBlockWithColorMultiplier(Block var1, int var2, int var3, int var4, float var5, float var6, float var7)
    {
        _enableAO = false;
        Tessellator var8 = getTessellator();
        Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
        bool var9 = false;
        float var10 = 0.5F;
        float var11 = 1.0F;
        float var12 = 0.8F;
        float var13 = 0.6F;
        float var14 = var11 * var5;
        float var15 = var11 * var6;
        float var16 = var11 * var7;
        float var17 = var10;
        float var18 = var12;
        float var19 = var13;
        float var20 = var10;
        float var21 = var12;
        float var22 = var13;
        float var23 = var10;
        float var24 = var12;
        float var25 = var13;
        if (var1 != Block.GrassBlock)
        {
            var17 = var10 * var5;
            var18 = var12 * var5;
            var19 = var13 * var5;
            var20 = var10 * var6;
            var21 = var12 * var6;
            var22 = var13 * var6;
            var23 = var10 * var7;
            var24 = var12 * var7;
            var25 = var13 * var7;
        }

        float var26 = var1.getLuminance(_blockAccess, var2, var3, var4);
        float var27;
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3 - 1, var4, 0))
        {
            var27 = var1.getLuminance(_blockAccess, var2, var3 - 1, var4);
            var8.setColorOpaque_F(var17 * var27, var20 * var27, var23 * var27);
            renderBottomFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, 0));
            var9 = true;
        }

        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3 + 1, var4, 1))
        {
            var27 = var1.getLuminance(_blockAccess, var2, var3 + 1, var4);
            if (blockBB.MaxY != 1.0D && !var1.material.IsFluid)
            {
                var27 = var26;
            }

            var8.setColorOpaque_F(var14 * var27, var15 * var27, var16 * var27);
            renderTopFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, 1));
            var9 = true;
        }

        int var28;
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3, var4 - 1, 2))
        {
            var27 = var1.getLuminance(_blockAccess, var2, var3, var4 - 1);
            if (blockBB.MinZ > 0.0D)
            {
                var27 = var26;
            }

            var8.setColorOpaque_F(var18 * var27, var21 * var27, var24 * var27);
            var28 = var1.getTextureId(_blockAccess, var2, var3, var4, 2);
            renderEastFace(var1, var2, var3, var4, var28);
            if (s_fancyGrass && var28 == 3 && _overrideBlockTexture < 0)
            {
                var8.setColorOpaque_F(var18 * var27 * var5, var21 * var27 * var6, var24 * var27 * var7);
                renderEastFace(var1, var2, var3, var4, 38);
            }

            var9 = true;
        }

        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3, var4 + 1, 3))
        {
            var27 = var1.getLuminance(_blockAccess, var2, var3, var4 + 1);
            if (blockBB.MaxZ < 1.0D)
            {
                var27 = var26;
            }

            var8.setColorOpaque_F(var18 * var27, var21 * var27, var24 * var27);
            var28 = var1.getTextureId(_blockAccess, var2, var3, var4, 3);
            renderWestFace(var1, var2, var3, var4, var28);
            if (s_fancyGrass && var28 == 3 && _overrideBlockTexture < 0)
            {
                var8.setColorOpaque_F(var18 * var27 * var5, var21 * var27 * var6, var24 * var27 * var7);
                renderWestFace(var1, var2, var3, var4, 38);
            }

            var9 = true;
        }

        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2 - 1, var3, var4, 4))
        {
            var27 = var1.getLuminance(_blockAccess, var2 - 1, var3, var4);
            if (blockBB.MinX > 0.0D)
            {
                var27 = var26;
            }

            var8.setColorOpaque_F(var19 * var27, var22 * var27, var25 * var27);
            var28 = var1.getTextureId(_blockAccess, var2, var3, var4, 4);
            renderNorthFace(var1, var2, var3, var4, var28);
            if (s_fancyGrass && var28 == 3 && _overrideBlockTexture < 0)
            {
                var8.setColorOpaque_F(var19 * var27 * var5, var22 * var27 * var6, var25 * var27 * var7);
                renderNorthFace(var1, var2, var3, var4, 38);
            }

            var9 = true;
        }

        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2 + 1, var3, var4, 5))
        {
            var27 = var1.getLuminance(_blockAccess, var2 + 1, var3, var4);
            if (blockBB.MaxX < 1.0D)
            {
                var27 = var26;
            }

            var8.setColorOpaque_F(var19 * var27, var22 * var27, var25 * var27);
            var28 = var1.getTextureId(_blockAccess, var2, var3, var4, 5);
            renderSouthFace(var1, var2, var3, var4, var28);
            if (s_fancyGrass && var28 == 3 && _overrideBlockTexture < 0)
            {
                var8.setColorOpaque_F(var19 * var27 * var5, var22 * var27 * var6, var25 * var27 * var7);
                renderSouthFace(var1, var2, var3, var4, 38);
            }

            var9 = true;
        }

        return var9;
    }

    public bool renderBlockCactus(Block var1, int var2, int var3, int var4)
    {
        int var5 = var1.getColorMultiplier(_blockAccess, var2, var3, var4);
        float var6 = (var5 >> 16 & 255) / 255.0F;
        float var7 = (var5 >> 8 & 255) / 255.0F;
        float var8 = (var5 & 255) / 255.0F;

        return func_1230_b(var1, var2, var3, var4, var6, var7, var8);
    }

    public bool func_1230_b(Block var1, int var2, int var3, int var4, float var5, float var6, float var7)
    {
        Tessellator var8 = getTessellator();
        Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
        bool var9 = false;
        float var10 = 0.5F;
        float var11 = 1.0F;
        float var12 = 0.8F;
        float var13 = 0.6F;
        float var14 = var10 * var5;
        float var15 = var11 * var5;
        float var16 = var12 * var5;
        float var17 = var13 * var5;
        float var18 = var10 * var6;
        float var19 = var11 * var6;
        float var20 = var12 * var6;
        float var21 = var13 * var6;
        float var22 = var10 * var7;
        float var23 = var11 * var7;
        float var24 = var12 * var7;
        float var25 = var13 * var7;
        float var26 = 1.0F / 16.0F;
        float var27 = var1.getLuminance(_blockAccess, var2, var3, var4);
        float var28;
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3 - 1, var4, 0))
        {
            var28 = var1.getLuminance(_blockAccess, var2, var3 - 1, var4);
            var8.setColorOpaque_F(var14 * var28, var18 * var28, var22 * var28);
            renderBottomFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, 0));
            var9 = true;
        }

        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3 + 1, var4, 1))
        {
            var28 = var1.getLuminance(_blockAccess, var2, var3 + 1, var4);
            if (blockBB.MaxY != 1.0D && !var1.material.IsFluid)
            {
                var28 = var27;
            }

            var8.setColorOpaque_F(var15 * var28, var19 * var28, var23 * var28);
            renderTopFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, 1));
            var9 = true;
        }

        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3, var4 - 1, 2))
        {
            var28 = var1.getLuminance(_blockAccess, var2, var3, var4 - 1);
            if (blockBB.MinZ > 0.0D)
            {
                var28 = var27;
            }

            var8.setColorOpaque_F(var16 * var28, var20 * var28, var24 * var28);
            var8.setTranslationF(0.0F, 0.0F, var26);
            renderEastFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, 2));
            var8.setTranslationF(0.0F, 0.0F, -var26);
            var9 = true;
        }

        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3, var4 + 1, 3))
        {
            var28 = var1.getLuminance(_blockAccess, var2, var3, var4 + 1);
            if (blockBB.MaxZ < 1.0D)
            {
                var28 = var27;
            }

            var8.setColorOpaque_F(var16 * var28, var20 * var28, var24 * var28);
            var8.setTranslationF(0.0F, 0.0F, -var26);
            renderWestFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, 3));
            var8.setTranslationF(0.0F, 0.0F, var26);
            var9 = true;
        }

        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2 - 1, var3, var4, 4))
        {
            var28 = var1.getLuminance(_blockAccess, var2 - 1, var3, var4);
            if (blockBB.MinX > 0.0D)
            {
                var28 = var27;
            }

            var8.setColorOpaque_F(var17 * var28, var21 * var28, var25 * var28);
            var8.setTranslationF(var26, 0.0F, 0.0F);
            renderNorthFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, 4));
            var8.setTranslationF(-var26, 0.0F, 0.0F);
            var9 = true;
        }

        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2 + 1, var3, var4, 5))
        {
            var28 = var1.getLuminance(_blockAccess, var2 + 1, var3, var4);
            if (blockBB.MaxX < 1.0D)
            {
                var28 = var27;
            }

            var8.setColorOpaque_F(var17 * var28, var21 * var28, var25 * var28);
            var8.setTranslationF(-var26, 0.0F, 0.0F);
            renderSouthFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, 5));
            var8.setTranslationF(var26, 0.0F, 0.0F);
            var9 = true;
        }

        return var9;
    }

    public bool renderBlockFence(Block var1, int var2, int var3, int var4)
    {
        bool var5 = false;
        float var6 = 6.0F / 16.0F;
        float var7 = 10.0F / 16.0F;
        setOverrideBoundingBox(var6, 0.0F, var6, var7, 1.0F, var7);
        renderStandardBlock(var1, var2, var3, var4);
        var5 = true;
        bool var8 = false;
        bool var9 = false;
        if (_blockAccess.getBlockId(var2 - 1, var3, var4) == var1.id || _blockAccess.getBlockId(var2 + 1, var3, var4) == var1.id)
        {
            var8 = true;
        }

        if (_blockAccess.getBlockId(var2, var3, var4 - 1) == var1.id || _blockAccess.getBlockId(var2, var3, var4 + 1) == var1.id)
        {
            var9 = true;
        }

        bool var10 = _blockAccess.getBlockId(var2 - 1, var3, var4) == var1.id;
        bool var11 = _blockAccess.getBlockId(var2 + 1, var3, var4) == var1.id;
        bool var12 = _blockAccess.getBlockId(var2, var3, var4 - 1) == var1.id;
        bool var13 = _blockAccess.getBlockId(var2, var3, var4 + 1) == var1.id;
        if (!var8 && !var9)
        {
            var8 = true;
        }

        var6 = 7.0F / 16.0F;
        var7 = 9.0F / 16.0F;
        float var14 = 12.0F / 16.0F;
        float var15 = 15.0F / 16.0F;
        float var16 = var10 ? 0.0F : var6;
        float var17 = var11 ? 1.0F : var7;
        float var18 = var12 ? 0.0F : var6;
        float var19 = var13 ? 1.0F : var7;
        if (var8)
        {
            setOverrideBoundingBox(var16, var14, var6, var17, var15, var7);
            renderStandardBlock(var1, var2, var3, var4);
            var5 = true;
        }

        if (var9)
        {
            setOverrideBoundingBox(var6, var14, var18, var7, var15, var19);
            renderStandardBlock(var1, var2, var3, var4);
            var5 = true;
        }

        var14 = 6.0F / 16.0F;
        var15 = 9.0F / 16.0F;
        if (var8)
        {
            setOverrideBoundingBox(var16, var14, var6, var17, var15, var7);
            renderStandardBlock(var1, var2, var3, var4);
            var5 = true;
        }

        if (var9)
        {
            setOverrideBoundingBox(var6, var14, var18, var7, var15, var19);
            renderStandardBlock(var1, var2, var3, var4);
            var5 = true;
        }

        setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
        return var5;
    }

    public bool renderBlockStairs(Block var1, int var2, int var3, int var4)
    {
        bool var5 = false;
        int var6 = _blockAccess.getBlockMeta(var2, var3, var4);
        if (var6 == 0)
        {
            setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 0.5F, 0.5F, 1.0F);
            renderStandardBlock(var1, var2, var3, var4);
            setOverrideBoundingBox(0.5F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
            renderStandardBlock(var1, var2, var3, var4);
            var5 = true;
        }
        else if (var6 == 1)
        {
            setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 0.5F, 1.0F, 1.0F);
            renderStandardBlock(var1, var2, var3, var4);
            setOverrideBoundingBox(0.5F, 0.0F, 0.0F, 1.0F, 0.5F, 1.0F);
            renderStandardBlock(var1, var2, var3, var4);
            var5 = true;
        }
        else if (var6 == 2)
        {
            setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 0.5F, 0.5F);
            renderStandardBlock(var1, var2, var3, var4);
            setOverrideBoundingBox(0.0F, 0.0F, 0.5F, 1.0F, 1.0F, 1.0F);
            renderStandardBlock(var1, var2, var3, var4);
            var5 = true;
        }
        else if (var6 == 3)
        {
            setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.5F);
            renderStandardBlock(var1, var2, var3, var4);
            setOverrideBoundingBox(0.0F, 0.0F, 0.5F, 1.0F, 0.5F, 1.0F);
            renderStandardBlock(var1, var2, var3, var4);
            var5 = true;
        }

        setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
        return var5;
    }

    public bool renderBlockDoor(Block var1, int var2, int var3, int var4)
    {
        Tessellator var5 = getTessellator();
        BlockDoor var6 = (BlockDoor)var1;
        Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
        bool var7 = false;
        float var8 = 0.5F;
        float var9 = 1.0F;
        float var10 = 0.8F;
        float var11 = 0.6F;
        float var12 = var1.getLuminance(_blockAccess, var2, var3, var4);
        float var13 = var1.getLuminance(_blockAccess, var2, var3 - 1, var4);
        if (blockBB.MinY > 0.0D)
        {
            var13 = var12;
        }

        if (Block.BlocksLightLuminance[var1.id] > 0)
        {
            var13 = 1.0F;
        }

        var5.setColorOpaque_F(var8 * var13, var8 * var13, var8 * var13);
        renderBottomFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, 0));
        var7 = true;
        var13 = var1.getLuminance(_blockAccess, var2, var3 + 1, var4);
        if (blockBB.MaxY < 1.0D)
        {
            var13 = var12;
        }

        if (Block.BlocksLightLuminance[var1.id] > 0)
        {
            var13 = 1.0F;
        }

        var5.setColorOpaque_F(var9 * var13, var9 * var13, var9 * var13);
        renderTopFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, 1));
        var7 = true;
        var13 = var1.getLuminance(_blockAccess, var2, var3, var4 - 1);
        if (blockBB.MinZ > 0.0D)
        {
            var13 = var12;
        }

        if (Block.BlocksLightLuminance[var1.id] > 0)
        {
            var13 = 1.0F;
        }

        var5.setColorOpaque_F(var10 * var13, var10 * var13, var10 * var13);
        int var14 = var1.getTextureId(_blockAccess, var2, var3, var4, 2);
        if (var14 < 0)
        {
            _flipTexture = true;
            var14 = -var14;
        }

        renderEastFace(var1, var2, var3, var4, var14);
        var7 = true;
        _flipTexture = false;
        var13 = var1.getLuminance(_blockAccess, var2, var3, var4 + 1);
        if (blockBB.MaxZ < 1.0D)
        {
            var13 = var12;
        }

        if (Block.BlocksLightLuminance[var1.id] > 0)
        {
            var13 = 1.0F;
        }

        var5.setColorOpaque_F(var10 * var13, var10 * var13, var10 * var13);
        var14 = var1.getTextureId(_blockAccess, var2, var3, var4, 3);
        if (var14 < 0)
        {
            _flipTexture = true;
            var14 = -var14;
        }

        renderWestFace(var1, var2, var3, var4, var14);
        var7 = true;
        _flipTexture = false;
        var13 = var1.getLuminance(_blockAccess, var2 - 1, var3, var4);
        if (blockBB.MinX > 0.0D)
        {
            var13 = var12;
        }

        if (Block.BlocksLightLuminance[var1.id] > 0)
        {
            var13 = 1.0F;
        }

        var5.setColorOpaque_F(var11 * var13, var11 * var13, var11 * var13);
        var14 = var1.getTextureId(_blockAccess, var2, var3, var4, 4);
        if (var14 < 0)
        {
            _flipTexture = true;
            var14 = -var14;
        }

        renderNorthFace(var1, var2, var3, var4, var14);
        var7 = true;
        _flipTexture = false;
        var13 = var1.getLuminance(_blockAccess, var2 + 1, var3, var4);
        if (blockBB.MaxX < 1.0D)
        {
            var13 = var12;
        }

        if (Block.BlocksLightLuminance[var1.id] > 0)
        {
            var13 = 1.0F;
        }

        var5.setColorOpaque_F(var11 * var13, var11 * var13, var11 * var13);
        var14 = var1.getTextureId(_blockAccess, var2, var3, var4, 5);
        if (var14 < 0)
        {
            _flipTexture = true;
            var14 = -var14;
        }

        renderSouthFace(var1, var2, var3, var4, var14);
        var7 = true;
        _flipTexture = false;
        return var7;
    }

    public void renderBottomFace(Block var1, double var2, double var4, double var6, int var8)
    {
        Tessellator var9 = getTessellator();
        Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
        if (_overrideBlockTexture >= 0)
        {
            var8 = _overrideBlockTexture;
        }

        int var10 = (var8 & 15) << 4;
        int var11 = var8 & 240;
        double var12 = (var10 + blockBB.MinX * 16.0D) / 256.0D;
        double var14 = (var10 + blockBB.MaxX * 16.0D - 0.01D) / 256.0D;
        double var16 = (var11 + blockBB.MinZ * 16.0D) / 256.0D;
        double var18 = (var11 + blockBB.MaxZ * 16.0D - 0.01D) / 256.0D;
        if (blockBB.MinX < 0.0D || blockBB.MaxX > 1.0D)
        {
            var12 = (double)((var10 + 0.0F) / 256.0F);
            var14 = (double)((var10 + 15.99F) / 256.0F);
        }

        if (blockBB.MinZ < 0.0D || blockBB.MaxZ > 1.0D)
        {
            var16 = (double)((var11 + 0.0F) / 256.0F);
            var18 = (double)((var11 + 15.99F) / 256.0F);
        }

        double var20 = var14;
        double var22 = var12;
        double var24 = var16;
        double var26 = var18;
        if (_uvRotateBottom == 2)
        {
            var12 = (var10 + blockBB.MinZ * 16.0D) / 256.0D;
            var16 = (var11 + 16 - blockBB.MaxX * 16.0D) / 256.0D;
            var14 = (var10 + blockBB.MaxZ * 16.0D) / 256.0D;
            var18 = (var11 + 16 - blockBB.MinX * 16.0D) / 256.0D;
            var24 = var16;
            var26 = var18;
            var20 = var12;
            var22 = var14;
            var16 = var18;
            var18 = var24;
        }
        else if (_uvRotateBottom == 1)
        {
            var12 = (var10 + 16 - blockBB.MaxZ * 16.0D) / 256.0D;
            var16 = (var11 + blockBB.MinX * 16.0D) / 256.0D;
            var14 = (var10 + 16 - blockBB.MinZ * 16.0D) / 256.0D;
            var18 = (var11 + blockBB.MaxX * 16.0D) / 256.0D;
            var20 = var14;
            var22 = var12;
            var12 = var14;
            var14 = var22;
            var24 = var18;
            var26 = var16;
        }
        else if (_uvRotateBottom == 3)
        {
            var12 = (var10 + 16 - blockBB.MinX * 16.0D) / 256.0D;
            var14 = (var10 + 16 - blockBB.MaxX * 16.0D - 0.01D) / 256.0D;
            var16 = (var11 + 16 - blockBB.MinZ * 16.0D) / 256.0D;
            var18 = (var11 + 16 - blockBB.MaxZ * 16.0D - 0.01D) / 256.0D;
            var20 = var14;
            var22 = var12;
            var24 = var16;
            var26 = var18;
        }

        double var28 = var2 + blockBB.MinX;
        double var30 = var2 + blockBB.MaxX;
        double var32 = var4 + blockBB.MinY;
        double var34 = var6 + blockBB.MinZ;
        double var36 = var6 + blockBB.MaxZ;
        if (_enableAO)
        {
            var9.setColorOpaque_F(_colorRedTopLeft, _colorGreenTopLeft, _colorBlueTopLeft);
            var9.addVertexWithUV(var28, var32, var36, var22, var26);
            var9.setColorOpaque_F(_colorRedBottomLeft, _colorGreenBottomLeft, _colorBlueBottomLeft);
            var9.addVertexWithUV(var28, var32, var34, var12, var16);
            var9.setColorOpaque_F(_colorRedBottomRight, _colorGreenBottomRight, _colorBlueBottomRight);
            var9.addVertexWithUV(var30, var32, var34, var20, var24);
            var9.setColorOpaque_F(_colorRedTopRight, _colorGreenTopRight, _colorBlueTopRight);
            var9.addVertexWithUV(var30, var32, var36, var14, var18);
        }
        else
        {
            var9.addVertexWithUV(var28, var32, var36, var22, var26);
            var9.addVertexWithUV(var28, var32, var34, var12, var16);
            var9.addVertexWithUV(var30, var32, var34, var20, var24);
            var9.addVertexWithUV(var30, var32, var36, var14, var18);
        }

    }

    public void renderTopFace(Block var1, double var2, double var4, double var6, int var8)
    {
        Tessellator var9 = getTessellator();
        Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
        if (_overrideBlockTexture >= 0)
        {
            var8 = _overrideBlockTexture;
        }

        int var10 = (var8 & 15) << 4;
        int var11 = var8 & 240;
        double var12 = (var10 + blockBB.MinX * 16.0D) / 256.0D;
        double var14 = (var10 + blockBB.MaxX * 16.0D - 0.01D) / 256.0D;
        double var16 = (var11 + blockBB.MinZ * 16.0D) / 256.0D;
        double var18 = (var11 + blockBB.MaxZ * 16.0D - 0.01D) / 256.0D;
        if (blockBB.MinX < 0.0D || blockBB.MaxX > 1.0D)
        {
            var12 = (double)((var10 + 0.0F) / 256.0F);
            var14 = (double)((var10 + 15.99F) / 256.0F);
        }

        if (blockBB.MinZ < 0.0D || blockBB.MaxZ > 1.0D)
        {
            var16 = (double)((var11 + 0.0F) / 256.0F);
            var18 = (double)((var11 + 15.99F) / 256.0F);
        }

        double var20 = var14;
        double var22 = var12;
        double var24 = var16;
        double var26 = var18;
        if (_uvRotateTop == 1)
        {
            var12 = (var10 + blockBB.MinZ * 16.0D) / 256.0D;
            var16 = (var11 + 16 - blockBB.MaxX * 16.0D) / 256.0D;
            var14 = (var10 + blockBB.MaxZ * 16.0D) / 256.0D;
            var18 = (var11 + 16 - blockBB.MinX * 16.0D) / 256.0D;
            var24 = var16;
            var26 = var18;
            var20 = var12;
            var22 = var14;
            var16 = var18;
            var18 = var24;
        }
        else if (_uvRotateTop == 2)
        {
            var12 = (var10 + 16 - blockBB.MaxZ * 16.0D) / 256.0D;
            var16 = (var11 + blockBB.MinX * 16.0D) / 256.0D;
            var14 = (var10 + 16 - blockBB.MinZ * 16.0D) / 256.0D;
            var18 = (var11 + blockBB.MaxX * 16.0D) / 256.0D;
            var20 = var14;
            var22 = var12;
            var12 = var14;
            var14 = var22;
            var24 = var18;
            var26 = var16;
        }
        else if (_uvRotateTop == 3)
        {
            var12 = (var10 + 16 - blockBB.MinX * 16.0D) / 256.0D;
            var14 = (var10 + 16 - blockBB.MaxX * 16.0D - 0.01D) / 256.0D;
            var16 = (var11 + 16 - blockBB.MinZ * 16.0D) / 256.0D;
            var18 = (var11 + 16 - blockBB.MaxZ * 16.0D - 0.01D) / 256.0D;
            var20 = var14;
            var22 = var12;
            var24 = var16;
            var26 = var18;
        }

        double var28 = var2 + blockBB.MinX;
        double var30 = var2 + blockBB.MaxX;
        double var32 = var4 + blockBB.MaxY;
        double var34 = var6 + blockBB.MinZ;
        double var36 = var6 + blockBB.MaxZ;
        if (_enableAO)
        {
            var9.setColorOpaque_F(_colorRedTopLeft, _colorGreenTopLeft, _colorBlueTopLeft);
            var9.addVertexWithUV(var30, var32, var36, var14, var18);
            var9.setColorOpaque_F(_colorRedBottomLeft, _colorGreenBottomLeft, _colorBlueBottomLeft);
            var9.addVertexWithUV(var30, var32, var34, var20, var24);
            var9.setColorOpaque_F(_colorRedBottomRight, _colorGreenBottomRight, _colorBlueBottomRight);
            var9.addVertexWithUV(var28, var32, var34, var12, var16);
            var9.setColorOpaque_F(_colorRedTopRight, _colorGreenTopRight, _colorBlueTopRight);
            var9.addVertexWithUV(var28, var32, var36, var22, var26);
        }
        else
        {
            var9.addVertexWithUV(var30, var32, var36, var14, var18);
            var9.addVertexWithUV(var30, var32, var34, var20, var24);
            var9.addVertexWithUV(var28, var32, var34, var12, var16);
            var9.addVertexWithUV(var28, var32, var36, var22, var26);
        }

    }

    public void renderEastFace(Block var1, double var2, double var4, double var6, int var8)
    {
        Tessellator var9 = getTessellator();
        Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
        if (_overrideBlockTexture >= 0)
        {
            var8 = _overrideBlockTexture;
        }

        int var10 = (var8 & 15) << 4;
        int var11 = var8 & 240;
        double var12 = (var10 + blockBB.MinX * 16.0D) / 256.0D;
        double var14 = (var10 + blockBB.MaxX * 16.0D - 0.01D) / 256.0D;
        double var16 = (var11 + 16 - blockBB.MaxY * 16.0D) / 256.0D;
        double var18 = (var11 + 16 - blockBB.MinY * 16.0D - 0.01D) / 256.0D;
        double var20;
        if (_flipTexture)
        {
            var20 = var12;
            var12 = var14;
            var14 = var20;
        }

        if (blockBB.MinX < 0.0D || blockBB.MaxX > 1.0D)
        {
            var12 = (double)((var10 + 0.0F) / 256.0F);
            var14 = (double)((var10 + 15.99F) / 256.0F);
        }

        if (blockBB.MinY < 0.0D || blockBB.MaxY > 1.0D)
        {
            var16 = (double)((var11 + 0.0F) / 256.0F);
            var18 = (double)((var11 + 15.99F) / 256.0F);
        }

        var20 = var14;
        double var22 = var12;
        double var24 = var16;
        double var26 = var18;
        if (_uvRotateEast == 2)
        {
            var12 = (var10 + blockBB.MinY * 16.0D) / 256.0D;
            var16 = (var11 + 16 - blockBB.MinX * 16.0D) / 256.0D;
            var14 = (var10 + blockBB.MaxY * 16.0D) / 256.0D;
            var18 = (var11 + 16 - blockBB.MaxX * 16.0D) / 256.0D;
            var24 = var16;
            var26 = var18;
            var20 = var12;
            var22 = var14;
            var16 = var18;
            var18 = var24;
        }
        else if (_uvRotateEast == 1)
        {
            var12 = (var10 + 16 - blockBB.MaxY * 16.0D) / 256.0D;
            var16 = (var11 + blockBB.MaxX * 16.0D) / 256.0D;
            var14 = (var10 + 16 - blockBB.MinY * 16.0D) / 256.0D;
            var18 = (var11 + blockBB.MinX * 16.0D) / 256.0D;
            var20 = var14;
            var22 = var12;
            var12 = var14;
            var14 = var22;
            var24 = var18;
            var26 = var16;
        }
        else if (_uvRotateEast == 3)
        {
            var12 = (var10 + 16 - blockBB.MinX * 16.0D) / 256.0D;
            var14 = (var10 + 16 - blockBB.MaxX * 16.0D - 0.01D) / 256.0D;
            var16 = (var11 + blockBB.MaxY * 16.0D) / 256.0D;
            var18 = (var11 + blockBB.MinY * 16.0D - 0.01D) / 256.0D;
            var20 = var14;
            var22 = var12;
            var24 = var16;
            var26 = var18;
        }

        double var28 = var2 + blockBB.MinX;
        double var30 = var2 + blockBB.MaxX;
        double var32 = var4 + blockBB.MinY;
        double var34 = var4 + blockBB.MaxY;
        double var36 = var6 + blockBB.MinZ;
        if (_enableAO)
        {
            var9.setColorOpaque_F(_colorRedTopLeft, _colorGreenTopLeft, _colorBlueTopLeft);
            var9.addVertexWithUV(var28, var34, var36, var20, var24);
            var9.setColorOpaque_F(_colorRedBottomLeft, _colorGreenBottomLeft, _colorBlueBottomLeft);
            var9.addVertexWithUV(var30, var34, var36, var12, var16);
            var9.setColorOpaque_F(_colorRedBottomRight, _colorGreenBottomRight, _colorBlueBottomRight);
            var9.addVertexWithUV(var30, var32, var36, var22, var26);
            var9.setColorOpaque_F(_colorRedTopRight, _colorGreenTopRight, _colorBlueTopRight);
            var9.addVertexWithUV(var28, var32, var36, var14, var18);
        }
        else
        {
            var9.addVertexWithUV(var28, var34, var36, var20, var24);
            var9.addVertexWithUV(var30, var34, var36, var12, var16);
            var9.addVertexWithUV(var30, var32, var36, var22, var26);
            var9.addVertexWithUV(var28, var32, var36, var14, var18);
        }

    }

    public void renderWestFace(Block var1, double var2, double var4, double var6, int var8)
    {
        Tessellator var9 = getTessellator();
        Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
        if (_overrideBlockTexture >= 0)
        {
            var8 = _overrideBlockTexture;
        }

        int var10 = (var8 & 15) << 4;
        int var11 = var8 & 240;
        double var12 = (var10 + blockBB.MinX * 16.0D) / 256.0D;
        double var14 = (var10 + blockBB.MaxX * 16.0D - 0.01D) / 256.0D;
        double var16 = (var11 + 16 - blockBB.MaxY * 16.0D) / 256.0D;
        double var18 = (var11 + 16 - blockBB.MinY * 16.0D - 0.01D) / 256.0D;
        double var20;
        if (_flipTexture)
        {
            var20 = var12;
            var12 = var14;
            var14 = var20;
        }

        if (blockBB.MinX < 0.0D || blockBB.MaxX > 1.0D)
        {
            var12 = (double)((var10 + 0.0F) / 256.0F);
            var14 = (double)((var10 + 15.99F) / 256.0F);
        }

        if (blockBB.MinY < 0.0D || blockBB.MaxY > 1.0D)
        {
            var16 = (double)((var11 + 0.0F) / 256.0F);
            var18 = (double)((var11 + 15.99F) / 256.0F);
        }

        var20 = var14;
        double var22 = var12;
        double var24 = var16;
        double var26 = var18;
        if (_uvRotateWest == 1)
        {
            var12 = (var10 + blockBB.MinY * 16.0D) / 256.0D;
            var18 = (var11 + 16 - blockBB.MinX * 16.0D) / 256.0D;
            var14 = (var10 + blockBB.MaxY * 16.0D) / 256.0D;
            var16 = (var11 + 16 - blockBB.MaxX * 16.0D) / 256.0D;
            var24 = var16;
            var26 = var18;
            var20 = var12;
            var22 = var14;
            var16 = var18;
            var18 = var24;
        }
        else if (_uvRotateWest == 2)
        {
            var12 = (var10 + 16 - blockBB.MaxY * 16.0D) / 256.0D;
            var16 = (var11 + blockBB.MinX * 16.0D) / 256.0D;
            var14 = (var10 + 16 - blockBB.MinY * 16.0D) / 256.0D;
            var18 = (var11 + blockBB.MaxX * 16.0D) / 256.0D;
            var20 = var14;
            var22 = var12;
            var12 = var14;
            var14 = var22;
            var24 = var18;
            var26 = var16;
        }
        else if (_uvRotateWest == 3)
        {
            var12 = (var10 + 16 - blockBB.MinX * 16.0D) / 256.0D;
            var14 = (var10 + 16 - blockBB.MaxX * 16.0D - 0.01D) / 256.0D;
            var16 = (var11 + blockBB.MaxY * 16.0D) / 256.0D;
            var18 = (var11 + blockBB.MinY * 16.0D - 0.01D) / 256.0D;
            var20 = var14;
            var22 = var12;
            var24 = var16;
            var26 = var18;
        }

        double var28 = var2 + blockBB.MinX;
        double var30 = var2 + blockBB.MaxX;
        double var32 = var4 + blockBB.MinY;
        double var34 = var4 + blockBB.MaxY;
        double var36 = var6 + blockBB.MaxZ;
        if (_enableAO)
        {
            var9.setColorOpaque_F(_colorRedTopLeft, _colorGreenTopLeft, _colorBlueTopLeft);
            var9.addVertexWithUV(var28, var34, var36, var12, var16);
            var9.setColorOpaque_F(_colorRedBottomLeft, _colorGreenBottomLeft, _colorBlueBottomLeft);
            var9.addVertexWithUV(var28, var32, var36, var22, var26);
            var9.setColorOpaque_F(_colorRedBottomRight, _colorGreenBottomRight, _colorBlueBottomRight);
            var9.addVertexWithUV(var30, var32, var36, var14, var18);
            var9.setColorOpaque_F(_colorRedTopRight, _colorGreenTopRight, _colorBlueTopRight);
            var9.addVertexWithUV(var30, var34, var36, var20, var24);
        }
        else
        {
            var9.addVertexWithUV(var28, var34, var36, var12, var16);
            var9.addVertexWithUV(var28, var32, var36, var22, var26);
            var9.addVertexWithUV(var30, var32, var36, var14, var18);
            var9.addVertexWithUV(var30, var34, var36, var20, var24);
        }

    }

    public void renderNorthFace(Block var1, double var2, double var4, double var6, int var8)
    {
        Tessellator var9 = getTessellator();
        Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
        if (_overrideBlockTexture >= 0)
        {
            var8 = _overrideBlockTexture;
        }

        int var10 = (var8 & 15) << 4;
        int var11 = var8 & 240;
        double var12 = (var10 + blockBB.MinZ * 16.0D) / 256.0D;
        double var14 = (var10 + blockBB.MaxZ * 16.0D - 0.01D) / 256.0D;
        double var16 = (var11 + 16 - blockBB.MaxY * 16.0D) / 256.0D;
        double var18 = (var11 + 16 - blockBB.MinY * 16.0D - 0.01D) / 256.0D;
        double var20;
        if (_flipTexture)
        {
            var20 = var12;
            var12 = var14;
            var14 = var20;
        }

        if (blockBB.MinZ < 0.0D || blockBB.MaxZ > 1.0D)
        {
            var12 = (double)((var10 + 0.0F) / 256.0F);
            var14 = (double)((var10 + 15.99F) / 256.0F);
        }

        if (blockBB.MinY < 0.0D || blockBB.MaxY > 1.0D)
        {
            var16 = (double)((var11 + 0.0F) / 256.0F);
            var18 = (double)((var11 + 15.99F) / 256.0F);
        }

        var20 = var14;
        double var22 = var12;
        double var24 = var16;
        double var26 = var18;
        if (_uvRotateNorth == 1)
        {
            var12 = (var10 + blockBB.MinY * 16.0D) / 256.0D;
            var16 = (var11 + 16 - blockBB.MaxZ * 16.0D) / 256.0D;
            var14 = (var10 + blockBB.MaxY * 16.0D) / 256.0D;
            var18 = (var11 + 16 - blockBB.MinZ * 16.0D) / 256.0D;
            var24 = var16;
            var26 = var18;
            var20 = var12;
            var22 = var14;
            var16 = var18;
            var18 = var24;
        }
        else if (_uvRotateNorth == 2)
        {
            var12 = (var10 + 16 - blockBB.MaxY * 16.0D) / 256.0D;
            var16 = (var11 + blockBB.MinZ * 16.0D) / 256.0D;
            var14 = (var10 + 16 - blockBB.MinY * 16.0D) / 256.0D;
            var18 = (var11 + blockBB.MaxZ * 16.0D) / 256.0D;
            var20 = var14;
            var22 = var12;
            var12 = var14;
            var14 = var22;
            var24 = var18;
            var26 = var16;
        }
        else if (_uvRotateNorth == 3)
        {
            var12 = (var10 + 16 - blockBB.MinZ * 16.0D) / 256.0D;
            var14 = (var10 + 16 - blockBB.MaxZ * 16.0D - 0.01D) / 256.0D;
            var16 = (var11 + blockBB.MaxY * 16.0D) / 256.0D;
            var18 = (var11 + blockBB.MinY * 16.0D - 0.01D) / 256.0D;
            var20 = var14;
            var22 = var12;
            var24 = var16;
            var26 = var18;
        }

        double var28 = var2 + blockBB.MinX;
        double var30 = var4 + blockBB.MinY;
        double var32 = var4 + blockBB.MaxY;
        double var34 = var6 + blockBB.MinZ;
        double var36 = var6 + blockBB.MaxZ;
        if (_enableAO)
        {
            var9.setColorOpaque_F(_colorRedTopLeft, _colorGreenTopLeft, _colorBlueTopLeft);
            var9.addVertexWithUV(var28, var32, var36, var20, var24);
            var9.setColorOpaque_F(_colorRedBottomLeft, _colorGreenBottomLeft, _colorBlueBottomLeft);
            var9.addVertexWithUV(var28, var32, var34, var12, var16);
            var9.setColorOpaque_F(_colorRedBottomRight, _colorGreenBottomRight, _colorBlueBottomRight);
            var9.addVertexWithUV(var28, var30, var34, var22, var26);
            var9.setColorOpaque_F(_colorRedTopRight, _colorGreenTopRight, _colorBlueTopRight);
            var9.addVertexWithUV(var28, var30, var36, var14, var18);
        }
        else
        {
            var9.addVertexWithUV(var28, var32, var36, var20, var24);
            var9.addVertexWithUV(var28, var32, var34, var12, var16);
            var9.addVertexWithUV(var28, var30, var34, var22, var26);
            var9.addVertexWithUV(var28, var30, var36, var14, var18);
        }

    }

    public void renderSouthFace(Block var1, double var2, double var4, double var6, int var8)
    {
        Tessellator var9 = getTessellator();
        Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
        if (_overrideBlockTexture >= 0)
        {
            var8 = _overrideBlockTexture;
        }

        int var10 = (var8 & 15) << 4;
        int var11 = var8 & 240;
        double var12 = (var10 + blockBB.MinZ * 16.0D) / 256.0D;
        double var14 = (var10 + blockBB.MaxZ * 16.0D - 0.01D) / 256.0D;
        double var16 = (var11 + 16 - blockBB.MaxY * 16.0D) / 256.0D;
        double var18 = (var11 + 16 - blockBB.MinY * 16.0D - 0.01D) / 256.0D;
        double var20;
        if (_flipTexture)
        {
            var20 = var12;
            var12 = var14;
            var14 = var20;
        }

        if (blockBB.MinZ < 0.0D || blockBB.MaxZ > 1.0D)
        {
            var12 = (double)((var10 + 0.0F) / 256.0F);
            var14 = (double)((var10 + 15.99F) / 256.0F);
        }

        if (blockBB.MinY < 0.0D || blockBB.MaxY > 1.0D)
        {
            var16 = (double)((var11 + 0.0F) / 256.0F);
            var18 = (double)((var11 + 15.99F) / 256.0F);
        }

        var20 = var14;
        double var22 = var12;
        double var24 = var16;
        double var26 = var18;
        if (_uvRotateSouth == 2)
        {
            var12 = (var10 + blockBB.MinY * 16.0D) / 256.0D;
            var16 = (var11 + 16 - blockBB.MinZ * 16.0D) / 256.0D;
            var14 = (var10 + blockBB.MaxY * 16.0D) / 256.0D;
            var18 = (var11 + 16 - blockBB.MaxZ * 16.0D) / 256.0D;
            var24 = var16;
            var26 = var18;
            var20 = var12;
            var22 = var14;
            var16 = var18;
            var18 = var24;
        }
        else if (_uvRotateSouth == 1)
        {
            var12 = (var10 + 16 - blockBB.MaxY * 16.0D) / 256.0D;
            var16 = (var11 + blockBB.MaxZ * 16.0D) / 256.0D;
            var14 = (var10 + 16 - blockBB.MinY * 16.0D) / 256.0D;
            var18 = (var11 + blockBB.MinZ * 16.0D) / 256.0D;
            var20 = var14;
            var22 = var12;
            var12 = var14;
            var14 = var22;
            var24 = var18;
            var26 = var16;
        }
        else if (_uvRotateSouth == 3)
        {
            var12 = (var10 + 16 - blockBB.MinZ * 16.0D) / 256.0D;
            var14 = (var10 + 16 - blockBB.MaxZ * 16.0D - 0.01D) / 256.0D;
            var16 = (var11 + blockBB.MaxY * 16.0D) / 256.0D;
            var18 = (var11 + blockBB.MinY * 16.0D - 0.01D) / 256.0D;
            var20 = var14;
            var22 = var12;
            var24 = var16;
            var26 = var18;
        }

        double var28 = var2 + blockBB.MaxX;
        double var30 = var4 + blockBB.MinY;
        double var32 = var4 + blockBB.MaxY;
        double var34 = var6 + blockBB.MinZ;
        double var36 = var6 + blockBB.MaxZ;
        if (_enableAO)
        {
            var9.setColorOpaque_F(_colorRedTopLeft, _colorGreenTopLeft, _colorBlueTopLeft);
            var9.addVertexWithUV(var28, var30, var36, var22, var26);
            var9.setColorOpaque_F(_colorRedBottomLeft, _colorGreenBottomLeft, _colorBlueBottomLeft);
            var9.addVertexWithUV(var28, var30, var34, var14, var18);
            var9.setColorOpaque_F(_colorRedBottomRight, _colorGreenBottomRight, _colorBlueBottomRight);
            var9.addVertexWithUV(var28, var32, var34, var20, var24);
            var9.setColorOpaque_F(_colorRedTopRight, _colorGreenTopRight, _colorBlueTopRight);
            var9.addVertexWithUV(var28, var32, var36, var12, var16);
        }
        else
        {
            var9.addVertexWithUV(var28, var30, var36, var22, var26);
            var9.addVertexWithUV(var28, var30, var34, var14, var18);
            var9.addVertexWithUV(var28, var32, var34, var20, var24);
            var9.addVertexWithUV(var28, var32, var36, var12, var16);
        }

    }

    public void renderBlockOnInventory(Block var1, int var2, float var3)
    {
        Tessellator var4 = getTessellator();
        int var5;
        float var6;
        float var7;
        if (renderFromInside)
        {
            var5 = var1.getColor(var2);
            var6 = (var5 >> 16 & 255) / 255.0F;
            var7 = (var5 >> 8 & 255) / 255.0F;
            float var8 = (var5 & 255) / 255.0F;
            GLManager.GL.Color4(var6 * var3, var7 * var3, var8 * var3, 1.0F);
        }

        var5 = var1.getRenderType();
        if (var5 != 0 && var5 != 16)
        {
            if (var5 == 1)
            {
                var4.startDrawingQuads();
                var4.setNormal(0.0F, -1.0F, 0.0F);
                renderCrossedSquares(var1, var2, -0.5D, -0.5D, -0.5D);
                var4.draw();
            }
            else if (var5 == 13)
            {
                var1.setupRenderBoundingBox();
                GLManager.GL.Translate(-0.5F, -0.5F, -0.5F);
                var6 = 1.0F / 16.0F;
                var4.startDrawingQuads();
                var4.setNormal(0.0F, -1.0F, 0.0F);
                renderBottomFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(0));
                var4.draw();
                var4.startDrawingQuads();
                var4.setNormal(0.0F, 1.0F, 0.0F);
                renderTopFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(1));
                var4.draw();
                var4.startDrawingQuads();
                var4.setNormal(0.0F, 0.0F, -1.0F);
                var4.setTranslationF(0.0F, 0.0F, var6);
                renderEastFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(2));
                var4.setTranslationF(0.0F, 0.0F, -var6);
                var4.draw();
                var4.startDrawingQuads();
                var4.setNormal(0.0F, 0.0F, 1.0F);
                var4.setTranslationF(0.0F, 0.0F, -var6);
                renderWestFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(3));
                var4.setTranslationF(0.0F, 0.0F, var6);
                var4.draw();
                var4.startDrawingQuads();
                var4.setNormal(-1.0F, 0.0F, 0.0F);
                var4.setTranslationF(var6, 0.0F, 0.0F);
                renderNorthFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(4));
                var4.setTranslationF(-var6, 0.0F, 0.0F);
                var4.draw();
                var4.startDrawingQuads();
                var4.setNormal(1.0F, 0.0F, 0.0F);
                var4.setTranslationF(-var6, 0.0F, 0.0F);
                renderSouthFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(5));
                var4.setTranslationF(var6, 0.0F, 0.0F);
                var4.draw();
                GLManager.GL.Translate(0.5F, 0.5F, 0.5F);
            }
            else if (var5 == 6)
            {
                var4.startDrawingQuads();
                var4.setNormal(0.0F, -1.0F, 0.0F);
                func_1245_b(var1, var2, -0.5D, -0.5D, -0.5D);
                var4.draw();
            }
            else if (var5 == 2)
            {
                var4.startDrawingQuads();
                var4.setNormal(0.0F, -1.0F, 0.0F);
                renderTorchAtAngle(var1, -0.5D, -0.5D, -0.5D, 0.0D, 0.0D);
                var4.draw();
            }
            else
            {
                int var9;
                if (var5 == 10)
                {
                    for (var9 = 0; var9 < 2; ++var9)
                    {
                        if (var9 == 0)
                        {
                            var1.setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.5F);
                        }

                        if (var9 == 1)
                        {
                            var1.setBoundingBox(0.0F, 0.0F, 0.5F, 1.0F, 0.5F, 1.0F);
                        }

                        GLManager.GL.Translate(-0.5F, -0.5F, -0.5F);
                        var4.startDrawingQuads();
                        var4.setNormal(0.0F, -1.0F, 0.0F);
                        renderBottomFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(0));
                        var4.draw();
                        var4.startDrawingQuads();
                        var4.setNormal(0.0F, 1.0F, 0.0F);
                        renderTopFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(1));
                        var4.draw();
                        var4.startDrawingQuads();
                        var4.setNormal(0.0F, 0.0F, -1.0F);
                        renderEastFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(2));
                        var4.draw();
                        var4.startDrawingQuads();
                        var4.setNormal(0.0F, 0.0F, 1.0F);
                        renderWestFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(3));
                        var4.draw();
                        var4.startDrawingQuads();
                        var4.setNormal(-1.0F, 0.0F, 0.0F);
                        renderNorthFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(4));
                        var4.draw();
                        var4.startDrawingQuads();
                        var4.setNormal(1.0F, 0.0F, 0.0F);
                        renderSouthFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(5));
                        var4.draw();
                        GLManager.GL.Translate(0.5F, 0.5F, 0.5F);
                    }
                }
                else if (var5 == 11)
                {
                    for (var9 = 0; var9 < 4; ++var9)
                    {
                        var7 = 2.0F / 16.0F;
                        if (var9 == 0)
                        {
                            var1.setBoundingBox(0.5F - var7, 0.0F, 0.0F, 0.5F + var7, 1.0F, var7 * 2.0F);
                        }

                        if (var9 == 1)
                        {
                            var1.setBoundingBox(0.5F - var7, 0.0F, 1.0F - var7 * 2.0F, 0.5F + var7, 1.0F, 1.0F);
                        }

                        var7 = 1.0F / 16.0F;
                        if (var9 == 2)
                        {
                            var1.setBoundingBox(0.5F - var7, 1.0F - var7 * 3.0F, -var7 * 2.0F, 0.5F + var7, 1.0F - var7, 1.0F + var7 * 2.0F);
                        }

                        if (var9 == 3)
                        {
                            var1.setBoundingBox(0.5F - var7, 0.5F - var7 * 3.0F, -var7 * 2.0F, 0.5F + var7, 0.5F - var7, 1.0F + var7 * 2.0F);
                        }

                        GLManager.GL.Translate(-0.5F, -0.5F, -0.5F);
                        var4.startDrawingQuads();
                        var4.setNormal(0.0F, -1.0F, 0.0F);
                        renderBottomFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(0));
                        var4.draw();
                        var4.startDrawingQuads();
                        var4.setNormal(0.0F, 1.0F, 0.0F);
                        renderTopFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(1));
                        var4.draw();
                        var4.startDrawingQuads();
                        var4.setNormal(0.0F, 0.0F, -1.0F);
                        renderEastFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(2));
                        var4.draw();
                        var4.startDrawingQuads();
                        var4.setNormal(0.0F, 0.0F, 1.0F);
                        renderWestFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(3));
                        var4.draw();
                        var4.startDrawingQuads();
                        var4.setNormal(-1.0F, 0.0F, 0.0F);
                        renderNorthFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(4));
                        var4.draw();
                        var4.startDrawingQuads();
                        var4.setNormal(1.0F, 0.0F, 0.0F);
                        renderSouthFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(5));
                        var4.draw();
                        GLManager.GL.Translate(0.5F, 0.5F, 0.5F);
                    }

                    var1.setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                }
            }
        }
        else
        {
            if (var5 == 16)
            {
                var2 = 1;
            }

            var1.setupRenderBoundingBox();
            GLManager.GL.Translate(-0.5F, -0.5F, -0.5F);
            var4.startDrawingQuads();
            var4.setNormal(0.0F, -1.0F, 0.0F);
            renderBottomFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(0, var2));
            var4.draw();
            var4.startDrawingQuads();
            var4.setNormal(0.0F, 1.0F, 0.0F);
            renderTopFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(1, var2));
            var4.draw();
            var4.startDrawingQuads();
            var4.setNormal(0.0F, 0.0F, -1.0F);
            renderEastFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(2, var2));
            var4.draw();
            var4.startDrawingQuads();
            var4.setNormal(0.0F, 0.0F, 1.0F);
            renderWestFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(3, var2));
            var4.draw();
            var4.startDrawingQuads();
            var4.setNormal(-1.0F, 0.0F, 0.0F);
            renderNorthFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(4, var2));
            var4.draw();
            var4.startDrawingQuads();
            var4.setNormal(1.0F, 0.0F, 0.0F);
            renderSouthFace(var1, 0.0D, 0.0D, 0.0D, var1.getTexture(5, var2));
            var4.draw();
            GLManager.GL.Translate(0.5F, 0.5F, 0.5F);
        }

    }

    public static bool isSideLit(int var0)
    {
        return var0 == 0 ? true : var0 == 13 ? true : var0 == 10 ? true : var0 == 11 ? true : var0 == 16;
    }

    public static void rotateAroundX(ref Vector3D<double> vec, float var1)
    {
        float var2 = MathHelper.Cos(var1);
        float var3 = MathHelper.Sin(var1);
        double var4 = vec.X;
        double var6 = vec.Y * (double)var2 + vec.Z * (double)var3;
        double var8 = vec.Z * (double)var2 - vec.Y * (double)var3;
        vec.X = var4;
        vec.Y = var6;
        vec.Z = var8;
    }

    private static void rotateAroundY(ref Vector3D<double> vec, float var1)
    {
        float var2 = MathHelper.Cos(var1);
        float var3 = MathHelper.Sin(var1);
        double var4 = vec.X * (double)var2 + vec.Z * (double)var3;
        double var6 = vec.Y;
        double var8 = vec.Z * (double)var2 - vec.X * (double)var3;
        vec.X = var4;
        vec.Y = var6;
        vec.Z = var8;
    }
}
