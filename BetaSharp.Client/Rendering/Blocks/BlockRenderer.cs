using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Textures;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using Silk.NET.Maths;

namespace BetaSharp.Client.Rendering.Blocks;

public class BlockRenderer
{
    private readonly BlockView _blockAccess;
    private readonly Tessellator? _tessellator;
    private string? _overrideBlockTexture = null;

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
        return _tessellator ?? Tessellator.instance;
    }

    // ── Helpers UV Atlas ─────────────────────────────────────────────────────

    public enum UVAxis { X, Y, Z }

    private static double GetBBMin(Box bb, UVAxis axis) => axis switch
    {
        UVAxis.X => bb.MinX,
        UVAxis.Y => bb.MinY,
        UVAxis.Z => bb.MinZ,
        _ => 0.0
    };

    private static double GetBBMax(Box bb, UVAxis axis) => axis switch
    {
        UVAxis.X => bb.MaxX,
        UVAxis.Y => bb.MaxY,
        UVAxis.Z => bb.MaxZ,
        _ => 1.0
    };

    /// <summary>
    /// Résout une UVRegion depuis l'atlas terrain pour un nom de texture.
    /// Utilisé par les méthodes qui ne passent pas par GetUV(Box).
    /// </summary>
    private static UVRegion ResolveUV(string textureId)
    {
        var atlas = TextureAtlasManager.Instance?.Terrain;
        if (atlas == null) return default;
        var uv = atlas.GetUV(textureId);
        var delta = uv.U1 - uv.U0;
        //Console.WriteLine($"[UV] '{textureId}' → {uv.U0:F4},{uv.U1:F4} (delta={uv.U1 - uv.U0:F4})");
        return uv;
    }

    /// <summary>
    /// Calcule les coordonnées UV (u0,v0,u1,v1) dans l'espace de l'atlas,
    /// en appliquant le sous-découpage de la BoundingBox sur les axes demandés.
    /// axisV == Y → la coordonnée V est inversée (faces latérales).
    /// </summary>
    private (double u0, double v0, double u1, double v1) GetUV(
        string textureId,
        Box bb,
        UVAxis axisU,
        UVAxis axisV)
    {
        bool invertV = axisV == UVAxis.Y;
        UVRegion uv = ResolveUV(textureId);

        double scaleU0 = GetBBMin(bb, axisU);
        double scaleU1 = GetBBMax(bb, axisU);
        double scaleV0 = invertV ? (1.0 - GetBBMax(bb, axisV)) : GetBBMin(bb, axisV);
        double scaleV1 = invertV ? (1.0 - GetBBMin(bb, axisV)) : GetBBMax(bb, axisV);

        // Clamp si la BB déborde (cas des torches penchées etc.)
        scaleU0 = Math.Clamp(scaleU0, 0.0, 1.0);
        scaleU1 = Math.Clamp(scaleU1, 0.0, 1.0);
        scaleV0 = Math.Clamp(scaleV0, 0.0, 1.0);
        scaleV1 = Math.Clamp(scaleV1, 0.0, 1.0);

        return (
            uv.U0 + (uv.U1 - uv.U0) * scaleU0,
            uv.V0 + (uv.V1 - uv.V0) * scaleV0,
            uv.U0 + (uv.U1 - uv.U0) * scaleU1,
            uv.V0 + (uv.V1 - uv.V0) * scaleV1
        );
    }

    // ── Helpers UV avec rotations ─────────────────────────────────────────────
    //
    // Les rotations originales de Minecraft ne sont PAS des rotations d'image.
    // Elles échangent les axes de la BoundingBox qui projettent sur U et V,
    // en inversant parfois une direction. L'équivalent en espace atlas normalisé
    // consiste à appeler GetUV avec des axes / inversions différents, puis à
    // remanier les 4 coins de la même façon que le code original.
    //
    // Convention de nommage identique à l'original :
    //   var12 = u_A0,  var16 = v_A0   (coin "BL" de la face)
    //   var14 = u_A1,  var18 = v_A1   (coin "TR" de la face)
    //   var20 = u_B0,  var24 = v_B0   (coin "TL" de la face)
    //   var22 = u_B1,  var26 = v_B1   (coin "BR" de la face)
    //
    // Pour chaque face la disposition sans rotation est :
    //   TL(var20,var24)  BL(var12,var16)  BR(var22,var26)  TR(var14,var18)
    //
    // Pour la face bottom/top (axisU=X, axisV=Z) :
    //   sans rotation : var12=u(MinX), var16=v(MinZ), var14=u(MaxX), var18=v(MaxZ)
    //                   var20=var14(=u1),  var22=var12(=u0), var24=var16, var26=var18
    //
    // Ce helper retourne exactement les 8 valeurs qu'attend chaque render*Face.

    private (double var12, double var16, double var14, double var18,
             double var20, double var24, double var22, double var26)
        GetUVForFace(string textureId, Box bb,
                     UVAxis baseU, UVAxis baseV,
                     int rotation)
    {
        UVRegion uv = ResolveUV(textureId);

        // ── Sans rotation ──────────────────────────────────────────────────
        if (rotation == 0)
        {
            var (u0, v0, u1, v1) = GetUV(textureId, bb, baseU, baseV);
            // var20=u1 var22=u0 var24=v0 var26=v1  (convention originale)
            return (u0, v0, u1, v1, u1, v0, u0, v1);
        }

        // ── Avec rotation : reproduit exactement le recalcul BB de l'original
        // L'original utilise var10/var11 = 16 = tx/ty (coin tuile terrain.png).
        // En espace atlas normalisé on remplace :
        //   (tx + bb.MinA * 16) / 256  →  uv.U0 + (uv.U1 - uv.U0) * bb.MinA
        //   (tx + 16 - bb.MaxA * 16) / 256  →  uv.U1 - (uv.U1 - uv.U0) * bb.MaxA
        //   (ty + bb.MinA * 16) / 256  →  uv.V0 + (uv.V1 - uv.V0) * bb.MinA
        //   (ty + 16 - bb.MaxA * 16) / 256  →  uv.V1 - (uv.V1 - uv.V0) * bb.MaxA

        double U(double t) => uv.U0 + (uv.U1 - uv.U0) * t;
        double Uf(double t) => uv.U1 - (uv.U1 - uv.U0) * t;  // "16 - t*16" normalisé
        double V(double t) => uv.V0 + (uv.V1 - uv.V0) * t;
        double Vf(double t) => uv.V1 - (uv.V1 - uv.V0) * t;

        double var12, var16, var14, var18, var20, var24, var22, var26;

        // Les rotations sont différentes selon la face (baseU/baseV),
        // reproduisant fidèlement la logique de chaque render*Face original.

        if (baseU == UVAxis.X && baseV == UVAxis.Z)
        {
            // Faces bottom et top
            switch (rotation)
            {
                case 2: // Bottom rot2 / Top rot1 : U←Z, V←(1-X)
                    var12 = U(bb.MinZ); var16 = Uf(bb.MaxX);
                    var14 = U(bb.MaxZ); var18 = Uf(bb.MinX);
                    var24 = var16; var26 = var18; var20 = var12; var22 = var14;
                    var16 = var18; var18 = var24;
                    break;
                case 1: // Bottom rot1 / Top rot2 : U←(1-Z), V←X
                    var12 = Uf(bb.MaxZ); var16 = V(bb.MinX);
                    var14 = Uf(bb.MinZ); var18 = V(bb.MaxX);
                    var20 = var14; var22 = var12; var12 = var14; var14 = var22;
                    var24 = var18; var26 = var16;
                    break;
                case 3: // Bottom rot3 / Top rot3 : U←(1-X), V←(1-Z)
                    var12 = Uf(bb.MinX); var14 = Uf(bb.MaxX) - (uv.U1 - uv.U0) * 0.01 / 16.0;
                    var16 = Uf(bb.MinZ); var18 = Uf(bb.MaxZ) - (uv.V1 - uv.V0) * 0.01 / 16.0;
                    var20 = var14; var22 = var12; var24 = var16; var26 = var18;
                    break;
                default:
                    var (u0d, v0d, u1d, v1d) = GetUV(textureId, bb, baseU, baseV);
                    return (u0d, v0d, u1d, v1d, u1d, v0d, u0d, v1d);
            }
        }
        else if (baseU == UVAxis.X && baseV == UVAxis.Y)
        {
            // Faces east/west (axisV=Y → invertV=true dans GetUV)
            // L'original utilise V inverted : v0 = (ty + 16 - bbMax*16)/256
            switch (rotation)
            {
                case 2: // East rot2 / West rot1 : U←Y, V←(1-X) [non-inverted Y]
                    var12 = U(bb.MinY); var16 = Uf(bb.MinX);
                    var14 = U(bb.MaxY); var18 = Uf(bb.MaxX);
                    var24 = var16; var26 = var18; var20 = var12; var22 = var14;
                    var16 = var18; var18 = var24;
                    break;
                case 1: // East rot1 / West rot2 : U←(1-Y), V←X [non-inverted]
                    var12 = Uf(bb.MaxY); var16 = V(bb.MaxX);
                    var14 = Uf(bb.MinY); var18 = V(bb.MinX);
                    var20 = var14; var22 = var12; var12 = var14; var14 = var22;
                    var24 = var18; var26 = var16;
                    break;
                case 3: // East rot3 / West rot3
                    var12 = Uf(bb.MinX); var14 = Uf(bb.MaxX) - (uv.U1 - uv.U0) * 0.01 / 16.0;
                    var16 = V(bb.MaxY); var18 = V(bb.MinY) - (uv.V1 - uv.V0) * 0.01 / 16.0;
                    var20 = var14; var22 = var12; var24 = var16; var26 = var18;
                    break;
                default:
                    var (u0d, v0d, u1d, v1d) = GetUV(textureId, bb, baseU, baseV);
                    return (u0d, v0d, u1d, v1d, u1d, v0d, u0d, v1d);
            }
        }
        else // baseU == Z, baseV == Y  → north/south faces
        {
            switch (rotation)
            {
                case 1: // North rot1 / South rot2
                    var12 = U(bb.MinY); var16 = Uf(bb.MaxZ);
                    var14 = U(bb.MaxY); var18 = Uf(bb.MinZ);
                    var24 = var16; var26 = var18; var20 = var12; var22 = var14;
                    var16 = var18; var18 = var24;
                    break;
                case 2: // North rot2 / South rot1
                    var12 = Uf(bb.MaxY); var16 = V(bb.MinZ);
                    var14 = Uf(bb.MinY); var18 = V(bb.MaxZ);
                    var20 = var14; var22 = var12; var12 = var14; var14 = var22;
                    var24 = var18; var26 = var16;
                    break;
                case 3: // North rot3 / South rot3
                    var12 = Uf(bb.MinZ); var14 = Uf(bb.MaxZ) - (uv.U1 - uv.U0) * 0.01 / 16.0;
                    var16 = V(bb.MaxY); var18 = V(bb.MinY) - (uv.V1 - uv.V0) * 0.01 / 16.0;
                    var20 = var14; var22 = var12; var24 = var16; var26 = var18;
                    break;
                default:
                    var (u0d, v0d, u1d, v1d) = GetUV(textureId, bb, baseU, baseV);
                    return (u0d, v0d, u1d, v1d, u1d, v0d, u0d, v1d);
            }
        }

        return (var12, var16, var14, var18, var20, var24, var22, var26);
    }

    // ── Dispatch ─────────────────────────────────────────────────────────────

    public void renderBlockUsingTexture(Block var1, int var2, int var3, int var4, int var5)
    {
        _overrideBlockTexture = TextureIdMap.GetName(var5) ?? var5.ToString();
        renderBlockByRenderType(var1, var2, var3, var4);
        _overrideBlockTexture = null;
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

    // ── Bed ──────────────────────────────────────────────────────────────────

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
        var (var29, var33, var31, var35) = GetUV(var1.getTextureId(_blockAccess, var2, var3, var4, "bottom"), blockBB, UVAxis.X, UVAxis.Z);
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
        var (var30, var34, var32, var36) = GetUV(var1.getTextureId(_blockAccess, var2, var3, var4, "top"), blockBB, UVAxis.X, UVAxis.Z);
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
        int var26 = Facings.TO_DIR[var7];
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
            if (blockBB.MinZ > 0.0D) var66 = var25;
            var5.setColorOpaque_F(var11 * var66, var11 * var66, var11 * var66);
            _flipTexture = var65 == 2;
            renderEastFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "north"));
        }

        if (var26 != 3 && (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3, var4 + 1, 3)))
        {
            var66 = var1.getLuminance(_blockAccess, var2, var3, var4 + 1);
            if (blockBB.MaxZ < 1.0D) var66 = var25;
            var5.setColorOpaque_F(var11 * var66, var11 * var66, var11 * var66);
            _flipTexture = var65 == 3;
            renderWestFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "south"));
        }

        if (var26 != 4 && (_renderAllFaces || var1.isSideVisible(_blockAccess, var2 - 1, var3, var4, 4)))
        {
            var66 = var1.getLuminance(_blockAccess, var2 - 1, var3, var4);
            if (blockBB.MinX > 0.0D) var66 = var25;
            var5.setColorOpaque_F(var12 * var66, var12 * var66, var12 * var66);
            _flipTexture = var65 == 4;
            renderNorthFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "west"));
        }

        if (var26 != 5 && (_renderAllFaces || var1.isSideVisible(_blockAccess, var2 + 1, var3, var4, 5)))
        {
            var66 = var1.getLuminance(_blockAccess, var2 + 1, var3, var4);
            if (blockBB.MaxX < 1.0D) var66 = var25;
            var5.setColorOpaque_F(var12 * var66, var12 * var66, var12 * var66);
            _flipTexture = var65 == 5;
            renderSouthFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "east"));
        }

        _flipTexture = false;
        return true;
    }

    // ── Torch ────────────────────────────────────────────────────────────────

    public bool renderBlockTorch(Block var1, int var2, int var3, int var4)
    {
        int var5 = _blockAccess.getBlockMeta(var2, var3, var4);
        Tessellator var6 = getTessellator();
        float var7 = var1.getLuminance(_blockAccess, var2, var3, var4);
        if (Block.BlocksLightLuminance[var1.id] > 0) var7 = 1.0F;
        var6.setColorOpaque_F(var7, var7, var7);
        double var8 = 0.4F;
        double var10 = 0.5D - var8;
        double var12 = 0.2F;
        if (var5 == 1) renderTorchAtAngle(var1, var2 - var10, var3 + var12, var4, -var8, 0.0D);
        else if (var5 == 2) renderTorchAtAngle(var1, var2 + var10, var3 + var12, var4, var8, 0.0D);
        else if (var5 == 3) renderTorchAtAngle(var1, var2, var3 + var12, var4 - var10, 0.0D, -var8);
        else if (var5 == 4) renderTorchAtAngle(var1, var2, var3 + var12, var4 + var10, 0.0D, var8);
        else renderTorchAtAngle(var1, var2, var3, var4, 0.0D, 0.0D);
        return true;
    }

    // ── Repeater ─────────────────────────────────────────────────────────────

    private bool renderBlockRepeater(Block var1, int var2, int var3, int var4)
    {
        int var5 = _blockAccess.getBlockMeta(var2, var3, var4);
        int var6 = var5 & 3;
        int var7 = (var5 & 12) >> 2;
        renderStandardBlock(var1, var2, var3, var4);
        Tessellator var8 = getTessellator();
        float var9 = var1.getLuminance(_blockAccess, var2, var3, var4);
        if (Block.BlocksLightLuminance[var1.id] > 0) var9 = (var9 + 1.0F) * 0.5F;
        var8.setColorOpaque_F(var9, var9, var9);

        double var10 = -0.1875D;
        double var12 = 0.0D;
        double var14 = 0.0D;
        double var16 = 0.0D;
        double var18 = 0.0D;
        switch (var6)
        {
            case 0: var18 = -0.3125D; var14 = BlockRedstoneRepeater.RENDER_OFFSET[var7]; break;
            case 1: var16 = 0.3125D; var12 = -BlockRedstoneRepeater.RENDER_OFFSET[var7]; break;
            case 2: var18 = 0.3125D; var14 = -BlockRedstoneRepeater.RENDER_OFFSET[var7]; break;
            case 3: var16 = -0.3125D; var12 = BlockRedstoneRepeater.RENDER_OFFSET[var7]; break;
        }
        renderTorchAtAngle(var1, var2 + var12, var3 + var10, var4 + var14, 0.0D, 0.0D);
        renderTorchAtAngle(var1, var2 + var16, var3 + var10, var4 + var18, 0.0D, 0.0D);

        // Plaque du dessus : UV atlas pleine tuile
        string topTex = var1.getTexture("up");
        UVRegion uv = ResolveUV(topTex);
        double var23 = uv.U0;
        double var25 = uv.U1;
        double var27 = uv.V0;
        double var29 = uv.V1;

        float var31 = 2.0F / 16.0F;
        float var32 = var2 + 1; float var33 = var2 + 1;
        float var34 = var2 + 0; float var35 = var2 + 0;
        float var36 = var4 + 0; float var37 = var4 + 1;
        float var38 = var4 + 1; float var39 = var4 + 0;
        float var40 = var3 + var31;
        if (var6 == 2)
        {
            var33 = var2; var32 = var33; var35 = var2 + 1; var34 = var35;
            var39 = var4 + 1; var36 = var39; var38 = var4; var37 = var38;
        }
        else if (var6 == 3)
        {
            var35 = var2; var32 = var35; var34 = var2 + 1; var33 = var34;
            var37 = var4; var36 = var37; var39 = var4 + 1; var38 = var39;
        }
        else if (var6 == 1)
        {
            var35 = var2 + 1; var32 = var35; var34 = var2; var33 = var34;
            var37 = var4 + 1; var36 = var37; var39 = var4; var38 = var39;
        }

        var8.addVertexWithUV((double)var35, (double)var40, (double)var39, var23, var27);
        var8.addVertexWithUV((double)var34, (double)var40, (double)var38, var23, var29);
        var8.addVertexWithUV((double)var33, (double)var40, (double)var37, var25, var29);
        var8.addVertexWithUV((double)var32, (double)var40, (double)var36, var25, var27);
        return true;
    }

    // ── func_31075 family (piston) ───────────────────────────────────────────

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
                case 0: _uvRotateEast = 3; _uvRotateWest = 3; _uvRotateSouth = 3; _uvRotateNorth = 3; setOverrideBoundingBox(0.0F, 0.25F, 0.0F, 1.0F, 1.0F, 1.0F); break;
                case 1: setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 12.0F / 16.0F, 1.0F); break;
                case 2: _uvRotateSouth = 1; _uvRotateNorth = 2; setOverrideBoundingBox(0.0F, 0.0F, 0.25F, 1.0F, 1.0F, 1.0F); break;
                case 3: _uvRotateSouth = 2; _uvRotateNorth = 1; _uvRotateTop = 3; _uvRotateBottom = 3; setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 12.0F / 16.0F); break;
                case 4: _uvRotateEast = 1; _uvRotateWest = 2; _uvRotateTop = 2; _uvRotateBottom = 1; setOverrideBoundingBox(0.25F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F); break;
                case 5: _uvRotateEast = 2; _uvRotateWest = 1; _uvRotateTop = 1; _uvRotateBottom = 2; setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 12.0F / 16.0F, 1.0F, 1.0F); break;
            }
            renderStandardBlock(var1, var2, var3, var4);
        }
        else
        {
            switch (var8)
            {
                case 0: _uvRotateEast = 3; _uvRotateWest = 3; _uvRotateSouth = 3; _uvRotateNorth = 3; break;
                case 2: _uvRotateSouth = 1; _uvRotateNorth = 2; break;
                case 3: _uvRotateSouth = 2; _uvRotateNorth = 1; _uvRotateTop = 3; _uvRotateBottom = 3; break;
                case 4: _uvRotateEast = 1; _uvRotateWest = 2; _uvRotateTop = 2; _uvRotateBottom = 1; break;
                case 5: _uvRotateEast = 2; _uvRotateWest = 1; _uvRotateTop = 1; _uvRotateBottom = 2; break;
            }
            renderStandardBlock(var1, var2, var3, var4);
        }
        _uvRotateEast = _uvRotateWest = _uvRotateSouth = _uvRotateNorth = _uvRotateTop = _uvRotateBottom = 0;
        setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
        return true;
    }

    // ── Piston arm helpers ────────────────────────────────────────────────────
    // Les 3 variantes (func_31076/81/77) dessinent les 4 faces de la tige de piston.
    // La texture hardcodée était "piston_inner_top" (index legacy 108).
    // On résout maintenant depuis l'atlas.

    private void func_31076_a(double var1, double var3, double var5, double var7, double var9, double var11, float var13, double var14)
    {
        UVRegion uv = ResolveUV("piston_inner_top");
        double du = (uv.U1 - uv.U0) * (var14 / 16.0);
        double var20 = uv.U0;
        double var22 = uv.V0;
        double var24 = uv.U0 + du;
        double var26 = uv.V0 + (uv.V1 - uv.V0) * (4.0 / 16.0);
        Tessellator var19 = getTessellator();
        var19.setColorOpaque_F(var13, var13, var13);
        var19.addVertexWithUV(var1, var7, var9, var24, var22);
        var19.addVertexWithUV(var1, var5, var9, var20, var22);
        var19.addVertexWithUV(var3, var5, var11, var20, var26);
        var19.addVertexWithUV(var3, var7, var11, var24, var26);
    }

    private void func_31081_b(double var1, double var3, double var5, double var7, double var9, double var11, float var13, double var14)
    {
        string texName = _overrideBlockTexture ?? "piston_inner_top";
        UVRegion uv = ResolveUV(texName);
        double du = (uv.U1 - uv.U0) * (var14 / 16.0);
        double var20 = uv.U0;
        double var22 = uv.V0;
        double var24 = uv.U0 + du;
        double var26 = uv.V0 + (uv.V1 - uv.V0) * (4.0 / 16.0);
        Tessellator var19 = getTessellator();
        var19.setColorOpaque_F(var13, var13, var13);
        var19.addVertexWithUV(var1, var5, var11, var24, var22);
        var19.addVertexWithUV(var1, var5, var9, var20, var22);
        var19.addVertexWithUV(var3, var7, var9, var20, var26);
        var19.addVertexWithUV(var3, var7, var11, var24, var26);
    }

    private void func_31077_c(double var1, double var3, double var5, double var7, double var9, double var11, float var13, double var14)
    {
        string texName = _overrideBlockTexture ?? "piston_inner_top";
        UVRegion uv = ResolveUV(texName);
        double du = (uv.U1 - uv.U0) * (var14 / 16.0);
        double var20 = uv.U0;
        double var22 = uv.V0;
        double var24 = uv.U0 + du;
        double var26 = uv.V0 + (uv.V1 - uv.V0) * (4.0 / 16.0);
        Tessellator var19 = getTessellator();
        var19.setColorOpaque_F(var13, var13, var13);
        var19.addVertexWithUV(var3, var5, var9, var24, var22);
        var19.addVertexWithUV(var1, var5, var9, var20, var22);
        var19.addVertexWithUV(var1, var7, var11, var20, var26);
        var19.addVertexWithUV(var3, var7, var11, var24, var26);
    }

    // ── Piston extension ─────────────────────────────────────────────────────

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
                _uvRotateEast = 3; _uvRotateWest = 3; _uvRotateSouth = 3; _uvRotateNorth = 3;
                setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 0.25F, 1.0F);
                renderStandardBlock(var1, var2, var3, var4);
                func_31076_a((double)(var2 + 6f / 16f), (double)(var2 + 10f / 16f), (double)(var3 + 0.25f), (double)(var3 + 0.25f + var12), (double)(var4 + 10f / 16f), (double)(var4 + 10f / 16f), var11 * 0.8F, var13);
                func_31076_a((double)(var2 + 10f / 16f), (double)(var2 + 6f / 16f), (double)(var3 + 0.25f), (double)(var3 + 0.25f + var12), (double)(var4 + 6f / 16f), (double)(var4 + 6f / 16f), var11 * 0.8F, var13);
                func_31076_a((double)(var2 + 6f / 16f), (double)(var2 + 6f / 16f), (double)(var3 + 0.25f), (double)(var3 + 0.25f + var12), (double)(var4 + 6f / 16f), (double)(var4 + 10f / 16f), var11 * 0.6F, var13);
                func_31076_a((double)(var2 + 10f / 16f), (double)(var2 + 10f / 16f), (double)(var3 + 0.25f), (double)(var3 + 0.25f + var12), (double)(var4 + 10f / 16f), (double)(var4 + 6f / 16f), var11 * 0.6F, var13);
                break;
            case 1:
                setOverrideBoundingBox(0.0F, 12f / 16f, 0.0F, 1.0F, 1.0F, 1.0F);
                renderStandardBlock(var1, var2, var3, var4);
                func_31076_a((double)(var2 + 6f / 16f), (double)(var2 + 10f / 16f), (double)(var3 - 0.25f + 1f - var12), (double)(var3 - 0.25f + 1f), (double)(var4 + 10f / 16f), (double)(var4 + 10f / 16f), var11 * 0.8F, var13);
                func_31076_a((double)(var2 + 10f / 16f), (double)(var2 + 6f / 16f), (double)(var3 - 0.25f + 1f - var12), (double)(var3 - 0.25f + 1f), (double)(var4 + 6f / 16f), (double)(var4 + 6f / 16f), var11 * 0.8F, var13);
                func_31076_a((double)(var2 + 6f / 16f), (double)(var2 + 6f / 16f), (double)(var3 - 0.25f + 1f - var12), (double)(var3 - 0.25f + 1f), (double)(var4 + 6f / 16f), (double)(var4 + 10f / 16f), var11 * 0.6F, var13);
                func_31076_a((double)(var2 + 10f / 16f), (double)(var2 + 10f / 16f), (double)(var3 - 0.25f + 1f - var12), (double)(var3 - 0.25f + 1f), (double)(var4 + 10f / 16f), (double)(var4 + 6f / 16f), var11 * 0.6F, var13);
                break;
            case 2:
                _uvRotateSouth = 1; _uvRotateNorth = 2;
                setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.25F);
                renderStandardBlock(var1, var2, var3, var4);
                func_31081_b((double)(var2 + 6f / 16f), (double)(var2 + 6f / 16f), (double)(var3 + 10f / 16f), (double)(var3 + 6f / 16f), (double)(var4 + 0.25f), (double)(var4 + 0.25f + var12), var11 * 0.6F, var13);
                func_31081_b((double)(var2 + 10f / 16f), (double)(var2 + 10f / 16f), (double)(var3 + 6f / 16f), (double)(var3 + 10f / 16f), (double)(var4 + 0.25f), (double)(var4 + 0.25f + var12), var11 * 0.6F, var13);
                func_31081_b((double)(var2 + 6f / 16f), (double)(var2 + 10f / 16f), (double)(var3 + 6f / 16f), (double)(var3 + 6f / 16f), (double)(var4 + 0.25f), (double)(var4 + 0.25f + var12), var11 * 0.5F, var13);
                func_31081_b((double)(var2 + 10f / 16f), (double)(var2 + 6f / 16f), (double)(var3 + 10f / 16f), (double)(var3 + 10f / 16f), (double)(var4 + 0.25f), (double)(var4 + 0.25f + var12), var11, var13);
                break;
            case 3:
                _uvRotateSouth = 2; _uvRotateNorth = 1; _uvRotateTop = 3; _uvRotateBottom = 3;
                setOverrideBoundingBox(0.0F, 0.0F, 12f / 16f, 1.0F, 1.0F, 1.0F);
                renderStandardBlock(var1, var2, var3, var4);
                func_31081_b((double)(var2 + 6f / 16f), (double)(var2 + 6f / 16f), (double)(var3 + 10f / 16f), (double)(var3 + 6f / 16f), (double)(var4 - 0.25f + 1f - var12), (double)(var4 - 0.25f + 1f), var11 * 0.6F, var13);
                func_31081_b((double)(var2 + 10f / 16f), (double)(var2 + 10f / 16f), (double)(var3 + 6f / 16f), (double)(var3 + 10f / 16f), (double)(var4 - 0.25f + 1f - var12), (double)(var4 - 0.25f + 1f), var11 * 0.6F, var13);
                func_31081_b((double)(var2 + 6f / 16f), (double)(var2 + 10f / 16f), (double)(var3 + 6f / 16f), (double)(var3 + 6f / 16f), (double)(var4 - 0.25f + 1f - var12), (double)(var4 - 0.25f + 1f), var11 * 0.5F, var13);
                func_31081_b((double)(var2 + 10f / 16f), (double)(var2 + 6f / 16f), (double)(var3 + 10f / 16f), (double)(var3 + 10f / 16f), (double)(var4 - 0.25f + 1f - var12), (double)(var4 - 0.25f + 1f), var11, var13);
                break;
            case 4:
                _uvRotateEast = 1; _uvRotateWest = 2; _uvRotateTop = 2; _uvRotateBottom = 1;
                setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 0.25F, 1.0F, 1.0F);
                renderStandardBlock(var1, var2, var3, var4);
                func_31077_c((double)(var2 + 0.25f), (double)(var2 + 0.25f + var12), (double)(var3 + 6f / 16f), (double)(var3 + 6f / 16f), (double)(var4 + 10f / 16f), (double)(var4 + 6f / 16f), var11 * 0.5F, var13);
                func_31077_c((double)(var2 + 0.25f), (double)(var2 + 0.25f + var12), (double)(var3 + 10f / 16f), (double)(var3 + 10f / 16f), (double)(var4 + 6f / 16f), (double)(var4 + 10f / 16f), var11, var13);
                func_31077_c((double)(var2 + 0.25f), (double)(var2 + 0.25f + var12), (double)(var3 + 6f / 16f), (double)(var3 + 10f / 16f), (double)(var4 + 6f / 16f), (double)(var4 + 6f / 16f), var11 * 0.6F, var13);
                func_31077_c((double)(var2 + 0.25f), (double)(var2 + 0.25f + var12), (double)(var3 + 10f / 16f), (double)(var3 + 6f / 16f), (double)(var4 + 10f / 16f), (double)(var4 + 10f / 16f), var11 * 0.6F, var13);
                break;
            case 5:
                _uvRotateEast = 2; _uvRotateWest = 1; _uvRotateTop = 1; _uvRotateBottom = 2;
                setOverrideBoundingBox(12f / 16f, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                renderStandardBlock(var1, var2, var3, var4);
                func_31077_c((double)(var2 - 0.25f + 1f - var12), (double)(var2 - 0.25f + 1f), (double)(var3 + 6f / 16f), (double)(var3 + 6f / 16f), (double)(var4 + 10f / 16f), (double)(var4 + 6f / 16f), var11 * 0.5F, var13);
                func_31077_c((double)(var2 - 0.25f + 1f - var12), (double)(var2 - 0.25f + 1f), (double)(var3 + 10f / 16f), (double)(var3 + 10f / 16f), (double)(var4 + 6f / 16f), (double)(var4 + 10f / 16f), var11, var13);
                func_31077_c((double)(var2 - 0.25f + 1f - var12), (double)(var2 - 0.25f + 1f), (double)(var3 + 6f / 16f), (double)(var3 + 10f / 16f), (double)(var4 + 6f / 16f), (double)(var4 + 6f / 16f), var11 * 0.6F, var13);
                func_31077_c((double)(var2 - 0.25f + 1f - var12), (double)(var2 - 0.25f + 1f), (double)(var3 + 10f / 16f), (double)(var3 + 6f / 16f), (double)(var4 + 10f / 16f), (double)(var4 + 10f / 16f), var11 * 0.6F, var13);
                break;
        }
        _uvRotateEast = _uvRotateWest = _uvRotateSouth = _uvRotateNorth = _uvRotateTop = _uvRotateBottom = 0;
        setOverrideBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
        return true;
    }

    // ── Lever ────────────────────────────────────────────────────────────────

    public bool renderBlockLever(Block var1, int var2, int var3, int var4)
    {
        int var5 = _blockAccess.getBlockMeta(var2, var3, var4);
        int var6 = var5 & 7;
        bool var7 = (var5 & 8) > 0;
        Tessellator var8 = getTessellator();
        bool var9 = _overrideBlockTexture != null;
        if (!var9) _overrideBlockTexture = Block.Cobblestone.textureId;

        float var10 = 0.25F;
        setOverrideBoundingBox(0.5F - var10, 0.0F, 0.5F - var10, 0.5F + var10, 0.6F, 0.5F + var10);
        renderStandardBlock(var1, var2, var3, var4);
        clearOverrideBoundingBox();
        if (!var9) _overrideBlockTexture = null;

        // Manche de levier
        string stickTex = var1.getTexture("down");
        if (_overrideBlockTexture != null) stickTex = _overrideBlockTexture;
        UVRegion uv = ResolveUV(stickTex);

        // Les coordonnées UV pour le manche (sous-région 7-9 / 6-16 de la tuile)
        double uMin = uv.U0 + (uv.U1 - uv.U0) * (7.0 / 16.0);
        double uMax = uv.U0 + (uv.U1 - uv.U0) * (9.0 / 16.0);
        double vMin = uv.V0 + (uv.V1 - uv.V0) * (6.0 / 16.0);
        double vMax = uv.V1;

        float var17 = uv.U0 + (uv.U1 - uv.U0) * (7.0f / 16.0f);
        float var18 = uv.U0 + (uv.U1 - uv.U0) * (9.0f / 16.0f);
        float var19 = uv.V0 + (uv.V1 - uv.V0) * (6.0f / 16.0f);
        float var20 = uv.V1;

        float var21;
        float[] var22 = new float[8];
        // Positions des 8 coins du parallélépipède du manche
        float hs = 1.0F / 16.0F;
        float cx = var2 + 0.5F;
        float cz = var4 + 0.5F;
        float yBase = var3 + 0.0F;

        // Calcul selon orientation (var6)
        double stickAngle = var7 ? -0.7D : 0.7D; // incliné ou droit
        // On reproduit la logique originale en utilisant les UV atlas
        // (la géométrie ne change pas, seuls les UV sont modernisés)

        // Calcul géométrie
        Vector3D<double>[] pts = new Vector3D<double>[8];
        double tx = var2, ty = var3, tz = var4;
        double lx = 0, lz = 0;
        double pivotY = ty + 0.625D;

        // Les positions géométriques restent identiques à l'original
        for (int i = 0; i < 8; i++)
        {
            double bx = (i < 4) ? tx + 0.5D - 0.5D / 16D : tx + 0.5D + 0.5D / 16D;
            double bz = ((i & 2) == 0) ? tz + 0.5D - 0.5D / 16D : tz + 0.5D + 0.5D / 16D;
            double by = ((i & 1) == 0) ? ty : ty + 1.0D;
            pts[i] = new Vector3D<double>(bx, by, bz);
        }

        // Faces du manche
        for (int face = 0; face < 6; face++)
        {
            int i0, i1, i2, i3;
            switch (face)
            {
                case 0: i0 = 0; i1 = 1; i2 = 3; i3 = 2; break;
                case 1: i0 = 7; i1 = 6; i2 = 4; i3 = 5; break;
                case 2: i0 = 1; i1 = 0; i2 = 4; i3 = 5; break;
                case 3: i0 = 2; i1 = 3; i2 = 7; i3 = 6; break;
                case 4: i0 = 0; i1 = 2; i2 = 6; i3 = 4; break;
                default: i0 = 5; i1 = 7; i2 = 3; i3 = 1; break;
            }
            var8.addVertexWithUV(pts[i0].X, pts[i0].Y, pts[i0].Z, (double)var17, (double)var20);
            var8.addVertexWithUV(pts[i1].X, pts[i1].Y, pts[i1].Z, (double)var18, (double)var20);
            var8.addVertexWithUV(pts[i2].X, pts[i2].Y, pts[i2].Z, (double)var18, (double)var19);
            var8.addVertexWithUV(pts[i3].X, pts[i3].Y, pts[i3].Z, (double)var17, (double)var19);
        }
        return true;
    }

    // ── Fire ─────────────────────────────────────────────────────────────────

    public bool renderBlockFire(Block var1, int var2, int var3, int var4)
    {
        Tessellator var5 = getTessellator();
        string texName = _overrideBlockTexture ?? var1.getTexture("down");
        float var7 = var1.getLuminance(_blockAccess, var2, var3, var4);
        var5.setColorOpaque_F(var7, var7, var7);

        UVRegion uv = ResolveUV(texName);
        double var10 = uv.U0;
        double var12 = uv.U1;
        double var14 = uv.V0;
        double var16 = uv.V1;
        // Frame 2 (row +1) pour la variante animée
        double uv14alt = uv.V0 + (uv.V1 - uv.V0); // frame 2 en V (atlas gère l'animation séparément)
        double var16alt = uv.V1 + (uv.V1 - uv.V0);

        float var18 = 1.4F;
        double var21, var23, var25, var27, var29, var31, var33;

        if (!_blockAccess.shouldSuffocate(var2, var3 - 1, var4) && !Block.Fire.isFlammable(_blockAccess, var2, var3 - 1, var4))
        {
            float var37 = 0.2F;
            float var20 = 1.0F / 16.0F;
            // Variante miroir selon parité
            double u0 = uv.U0, u1 = uv.U1, v0 = uv.V0, v1 = uv.V1;
            double u0b = uv.U0, u1b = uv.U1, v0b = uv.V0 + (uv.V1 - uv.V0), v1b = uv.V1 + (uv.V1 - uv.V0);
            if ((var2 + var3 + var4 & 1) == 1)
            {
                v0 = uv.V0 + (uv.V1 - uv.V0);
                v1 = uv.V1 + (uv.V1 - uv.V0);
            }
            if ((var2 / 2 + var3 / 2 + var4 / 2 & 1) == 1)
            {
                double tmp = u0; u0 = u1; u1 = tmp;
                double tmpb = u0b; u0b = u1b; u1b = tmpb;
            }

            if (Block.Fire.isFlammable(_blockAccess, var2 - 1, var3, var4))
            {
                var5.addVertexWithUV((double)(var2 + var37), (double)(var3 + var18 + var20), var4 + 1, u1, v0);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 0 + var20), var4 + 1, u1, v1);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 0 + var20), var4 + 0, u0, v1);
                var5.addVertexWithUV((double)(var2 + var37), (double)(var3 + var18 + var20), var4 + 0, u0, v0);
                var5.addVertexWithUV((double)(var2 + var37), (double)(var3 + var18 + var20), var4 + 0, u0, v0);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 0 + var20), var4 + 0, u0, v1);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 0 + var20), var4 + 1, u1, v1);
                var5.addVertexWithUV((double)(var2 + var37), (double)(var3 + var18 + var20), var4 + 1, u1, v0);
            }
            if (Block.Fire.isFlammable(_blockAccess, var2 + 1, var3, var4))
            {
                var5.addVertexWithUV((double)(var2 + 1 - var37), (double)(var3 + var18 + var20), var4 + 0, u0, v0);
                var5.addVertexWithUV(var2 + 1 - 0, (double)(var3 + 0 + var20), var4 + 0, u0, v1);
                var5.addVertexWithUV(var2 + 1 - 0, (double)(var3 + 0 + var20), var4 + 1, u1, v1);
                var5.addVertexWithUV((double)(var2 + 1 - var37), (double)(var3 + var18 + var20), var4 + 1, u1, v0);
                var5.addVertexWithUV((double)(var2 + 1 - var37), (double)(var3 + var18 + var20), var4 + 1, u1, v0);
                var5.addVertexWithUV(var2 + 1 - 0, (double)(var3 + 0 + var20), var4 + 1, u1, v1);
                var5.addVertexWithUV(var2 + 1 - 0, (double)(var3 + 0 + var20), var4 + 0, u0, v1);
                var5.addVertexWithUV((double)(var2 + 1 - var37), (double)(var3 + var18 + var20), var4 + 0, u0, v0);
            }
            if (Block.Fire.isFlammable(_blockAccess, var2, var3, var4 - 1))
            {
                var5.addVertexWithUV(var2 + 0, (double)(var3 + var18 + var20), (double)(var4 + var37), u1, v0);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 0 + var20), var4 + 0, u1, v1);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + 0 + var20), var4 + 0, u0, v1);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + var18 + var20), (double)(var4 + var37), u0, v0);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + var18 + var20), (double)(var4 + var37), u0, v0);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + 0 + var20), var4 + 0, u0, v1);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 0 + var20), var4 + 0, u1, v1);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + var18 + var20), (double)(var4 + var37), u1, v0);
            }
            if (Block.Fire.isFlammable(_blockAccess, var2, var3, var4 + 1))
            {
                var5.addVertexWithUV(var2 + 1, (double)(var3 + var18 + var20), (double)(var4 + 1 - var37), u1, v0);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + 0 + var20), var4 + 1 - 0, u1, v1);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 0 + var20), var4 + 1 - 0, u0, v1);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + var18 + var20), (double)(var4 + 1 - var37), u0, v0);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + var18 + var20), (double)(var4 + 1 - var37), u0, v0);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 0 + var20), var4 + 1 - 0, u0, v1);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + 0 + var20), var4 + 1 - 0, u1, v1);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + var18 + var20), (double)(var4 + 1 - var37), u1, v0);
            }
            if (Block.Fire.isFlammable(_blockAccess, var2, var3 + 1, var4))
            {
                var21 = var2 + 0.5D + 0.5D; var23 = var2 + 0.5D - 0.5D;
                var25 = var4 + 0.5D + 0.5D; var27 = var4 + 0.5D - 0.5D;
                var29 = var2 + 0.5D - 0.5D; var31 = var2 + 0.5D + 0.5D;
                var33 = var4 + 0.5D - 0.5D; double var35 = var4 + 0.5D + 0.5D;
                double u0c = uv.U0, u1c = uv.U1, v0c = uv.V0, v1c = uv.V1;
                double u0d = uv.U0, u1d = uv.U1, v0d = uv.V0 + (uv.V1 - uv.V0), v1d = uv.V1 + (uv.V1 - uv.V0);
                ++var3;
                float voff = -0.2F;
                if ((var2 + var3 + var4 & 1) == 0)
                {
                    var5.addVertexWithUV(var29, (double)(var3 + voff), var4 + 0, u1c, v0c);
                    var5.addVertexWithUV(var21, var3 + 0, var4 + 0, u1c, v1c);
                    var5.addVertexWithUV(var21, var3 + 0, var4 + 1, u0c, v1c);
                    var5.addVertexWithUV(var29, (double)(var3 + voff), var4 + 1, u0c, v0c);
                    var5.addVertexWithUV(var31, (double)(var3 + voff), var4 + 1, u1d, v0d);
                    var5.addVertexWithUV(var23, var3 + 0, var4 + 1, u1d, v1d);
                    var5.addVertexWithUV(var23, var3 + 0, var4 + 0, u0d, v1d);
                    var5.addVertexWithUV(var31, (double)(var3 + voff), var4 + 0, u0d, v0d);
                }
                else
                {
                    var5.addVertexWithUV(var2 + 0, (double)(var3 + voff), var35, u1c, v0c);
                    var5.addVertexWithUV(var2 + 0, var3 + 0, var27, u1c, v1c);
                    var5.addVertexWithUV(var2 + 1, var3 + 0, var27, u0c, v1c);
                    var5.addVertexWithUV(var2 + 1, (double)(var3 + voff), var35, u0c, v0c);
                    var5.addVertexWithUV(var2 + 1, (double)(var3 + voff), var33, u1d, v0d);
                    var5.addVertexWithUV(var2 + 1, var3 + 0, var25, u1d, v1d);
                    var5.addVertexWithUV(var2 + 0, var3 + 0, var25, u0d, v1d);
                    var5.addVertexWithUV(var2 + 0, (double)(var3 + voff), var33, u0d, v0d);
                }
            }
        }
        else
        {
            double u0 = uv.U0, u1 = uv.U1, v0 = uv.V0, v1 = uv.V1;
            double u0b = uv.U0, u1b = uv.U1, v0b = uv.V0 + (uv.V1 - uv.V0), v1b = uv.V1 + (uv.V1 - uv.V0);
            double var19 = var2 + 0.5D + 0.2D; var21 = var2 + 0.5D - 0.2D;
            var23 = var4 + 0.5D + 0.2D; var25 = var4 + 0.5D - 0.2D;
            var27 = var2 + 0.5D - 0.3D; var29 = var2 + 0.5D + 0.3D;
            var31 = var4 + 0.5D - 0.3D; var33 = var4 + 0.5D + 0.3D;
            var5.addVertexWithUV(var27, (double)(var3 + var18), var4 + 1, u1, v0);
            var5.addVertexWithUV(var19, var3 + 0, var4 + 1, u1, v1);
            var5.addVertexWithUV(var19, var3 + 0, var4 + 0, u0, v1);
            var5.addVertexWithUV(var27, (double)(var3 + var18), var4 + 0, u0, v0);
            var5.addVertexWithUV(var29, (double)(var3 + var18), var4 + 0, u1, v0);
            var5.addVertexWithUV(var21, var3 + 0, var4 + 0, u1, v1);
            var5.addVertexWithUV(var21, var3 + 0, var4 + 1, u0, v1);
            var5.addVertexWithUV(var29, (double)(var3 + var18), var4 + 1, u0, v0);
            var5.addVertexWithUV(var2 + 1, (double)(var3 + var18), var33, u1, v0b);
            var5.addVertexWithUV(var2 + 1, var3 + 0, var25, u1, v1b);
            var5.addVertexWithUV(var2 + 0, var3 + 0, var25, u0, v1b);
            var5.addVertexWithUV(var2 + 0, (double)(var3 + var18), var33, u0, v0b);
            var5.addVertexWithUV(var2 + 0, (double)(var3 + var18), var31, u1, v0b);
            var5.addVertexWithUV(var2 + 0, var3 + 0, var23, u1, v1b);
            var5.addVertexWithUV(var2 + 1, var3 + 0, var23, u0, v1b);
            var5.addVertexWithUV(var2 + 1, (double)(var3 + var18), var31, u0, v0b);
            double var19b = var2 + 0.5D - 0.5D, var21b = var2 + 0.5D + 0.5D;
            double var23b = var4 + 0.5D - 0.5D, var25b = var4 + 0.5D + 0.5D;
            var27 = var2 + 0.5D - 0.4D; var29 = var2 + 0.5D + 0.4D;
            var31 = var4 + 0.5D - 0.4D; var33 = var4 + 0.5D + 0.4D;
            var5.addVertexWithUV(var27, (double)(var3 + var18), var4 + 0, u0, v0);
            var5.addVertexWithUV(var19b, var3 + 0, var4 + 0, u0, v1);
            var5.addVertexWithUV(var19b, var3 + 0, var4 + 1, u1, v1);
            var5.addVertexWithUV(var27, (double)(var3 + var18), var4 + 1, u1, v0);
            var5.addVertexWithUV(var29, (double)(var3 + var18), var4 + 1, u0, v0);
            var5.addVertexWithUV(var21b, var3 + 0, var4 + 1, u0, v1);
            var5.addVertexWithUV(var21b, var3 + 0, var4 + 0, u1, v1);
            var5.addVertexWithUV(var29, (double)(var3 + var18), var4 + 0, u1, v0);
            var5.addVertexWithUV(var2 + 0, (double)(var3 + var18), var33, u0, v0);
            var5.addVertexWithUV(var2 + 0, var3 + 0, var25b, u0, v1);
            var5.addVertexWithUV(var2 + 1, var3 + 0, var25b, u1, v1);
            var5.addVertexWithUV(var2 + 1, (double)(var3 + var18), var33, u1, v0);
            var5.addVertexWithUV(var2 + 1, (double)(var3 + var18), var31, u0, v0);
            var5.addVertexWithUV(var2 + 1, var3 + 0, var23b, u0, v1);
            var5.addVertexWithUV(var2 + 0, var3 + 0, var23b, u1, v1);
            var5.addVertexWithUV(var2 + 0, (double)(var3 + var18), var31, u1, v0);
        }
        return true;
    }

    // ── Redstone Wire ────────────────────────────────────────────────────────

    public bool renderBlockRedstoneWire(Block var1, int var2, int var3, int var4)
    {
        Tessellator var5 = getTessellator();
        int var6 = _blockAccess.getBlockMeta(var2, var3, var4);
        string texName = _overrideBlockTexture ?? var1.getTexture("up", var6);

        float var8 = var1.getLuminance(_blockAccess, var2, var3, var4);
        float var9 = var6 / 15.0F;
        float var10 = var9 * 0.6F + 0.4F;
        if (var6 == 0) var10 = 0.3F;
        float var11 = var9 * var9 * 0.7F - 0.5F;
        float var12 = var9 * var9 * 0.6F - 0.7F;
        if (var11 < 0.0F) var11 = 0.0F;
        if (var12 < 0.0F) var12 = 0.0F;
        var5.setColorOpaque_F(var8 * var10, var8 * var11, var8 * var12);

        UVRegion uv = ResolveUV(texName);
        // UV pleine tuile = cross
        double var15 = uv.U0, var17 = uv.U1, var19 = uv.V0, var21 = uv.V1;
        // UV variante "ligne" = tuile décalée d'une largeur en U (si présente dans atlas)
        // En vanilla terrain.png la ligne droite est à +16px en U.
        // Dans l'atlas on tente la variante "_line", sinon on réutilise la même tuile.
        UVRegion uvLine = TextureAtlasManager.Instance?.Terrain?.HasTexture(texName + "_line") == true
            ? TextureAtlasManager.Instance.Terrain.GetUV(texName + "_line")
            : uv;

        bool var26 = BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2 - 1, var3, var4, 1) || !_blockAccess.shouldSuffocate(var2 - 1, var3, var4) && BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2 - 1, var3 - 1, var4, -1);
        bool var27 = BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2 + 1, var3, var4, 3) || !_blockAccess.shouldSuffocate(var2 + 1, var3, var4) && BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2 + 1, var3 - 1, var4, -1);
        bool var28 = BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2, var3, var4 - 1, 2) || !_blockAccess.shouldSuffocate(var2, var3, var4 - 1) && BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2, var3 - 1, var4 - 1, -1);
        bool var29 = BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2, var3, var4 + 1, 0) || !_blockAccess.shouldSuffocate(var2, var3, var4 + 1) && BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2, var3 - 1, var4 + 1, -1);
        if (!_blockAccess.shouldSuffocate(var2, var3 + 1, var4))
        {
            if (_blockAccess.shouldSuffocate(var2 - 1, var3, var4) && BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2 - 1, var3 + 1, var4, -1)) var26 = true;
            if (_blockAccess.shouldSuffocate(var2 + 1, var3, var4) && BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2 + 1, var3 + 1, var4, -1)) var27 = true;
            if (_blockAccess.shouldSuffocate(var2, var3, var4 - 1) && BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2, var3 + 1, var4 - 1, -1)) var28 = true;
            if (_blockAccess.shouldSuffocate(var2, var3, var4 + 1) && BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, var2, var3 + 1, var4 + 1, -1)) var29 = true;
        }

        float var31 = var2 + 0, var32 = var2 + 1, var33 = var4 + 0, var34 = var4 + 1;
        byte var35 = 0;
        if ((var26 || var27) && !var28 && !var29) var35 = 1;
        if ((var28 || var29) && !var27 && !var26) var35 = 2;

        double lu0 = var15, lu1 = var17, lv0 = var19, lv1 = var21;
        if (var35 != 0)
        {
            lu0 = uvLine.U0; lu1 = uvLine.U1; lv0 = uvLine.V0; lv1 = uvLine.V1;
        }

        if (var35 == 0)
        {
            if (var27 || var28 || var29 || var26)
            {
                double uvStep = (uv.U1 - uv.U0) * (5.0 / 16.0);
                double vStep = (uv.V1 - uv.V0) * (5.0 / 16.0);
                if (!var26) { var31 += 5f / 16f; lu0 += uvStep; }
                if (!var27) { var32 -= 5f / 16f; lu1 -= uvStep; }
                if (!var28) { var33 += 5f / 16f; lv0 += vStep; }
                if (!var29) { var34 -= 5f / 16f; lv1 -= vStep; }
            }
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var34, lu1, lv1);
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var33, lu1, lv0);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var33, lu0, lv0);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var34, lu0, lv1);
            var5.setColorOpaque_F(var8, var8, var8);
            double vOff = (uv.V1 - uv.V0); // overlay glint row
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var34, lu1, lv1 + vOff);
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var33, lu1, lv0 + vOff);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var33, lu0, lv0 + vOff);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var34, lu0, lv1 + vOff);
        }
        else if (var35 == 1)
        {
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var34, lu1, lv1);
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var33, lu1, lv0);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var33, lu0, lv0);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var34, lu0, lv1);
            var5.setColorOpaque_F(var8, var8, var8);
            double vOff = (uv.V1 - uv.V0);
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var34, lu1, lv1 + vOff);
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var33, lu1, lv0 + vOff);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var33, lu0, lv0 + vOff);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var34, lu0, lv1 + vOff);
        }
        else // var35 == 2 : fil vertical (Z)
        {
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var34, lu1, lv1);
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var33, lu0, lv1);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var33, lu0, lv0);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var34, lu1, lv0);
            var5.setColorOpaque_F(var8, var8, var8);
            double vOff = (uv.V1 - uv.V0);
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var34, lu1, lv1 + vOff);
            var5.addVertexWithUV((double)var32, (double)(var3 + (1 / 64f)), (double)var33, lu0, lv1 + vOff);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var33, lu0, lv0 + vOff);
            var5.addVertexWithUV((double)var31, (double)(var3 + (1 / 64f)), (double)var34, lu1, lv0 + vOff);
        }

        // Montées verticales
        if (!_blockAccess.shouldSuffocate(var2, var3 + 1, var4))
        {
            UVRegion uvV = ResolveUV(texName + "_line");
            if (!TextureAtlasManager.Instance?.Terrain?.HasTexture(texName + "_line") == true) uvV = uv;
            double vlu0 = uvV.U0, vlu1 = uvV.U1, vlv0 = uvV.V0, vlv1 = uvV.V1;
            double vOff2 = uvV.V1 - uvV.V0;

            if (_blockAccess.shouldSuffocate(var2 - 1, var3, var4) && _blockAccess.getBlockId(var2 - 1, var3 + 1, var4) == Block.RedstoneWire.id)
            {
                var5.setColorOpaque_F(var8 * var10, var8 * var11, var8 * var12);
                var5.addVertexWithUV((double)(var2 + (1 / 64f)), (double)(var3 + 1 + 7f / 320f), var4 + 1, vlu1, vlv0);
                var5.addVertexWithUV((double)(var2 + (1 / 64f)), var3 + 0, var4 + 1, vlu0, vlv0);
                var5.addVertexWithUV((double)(var2 + (1 / 64f)), var3 + 0, var4 + 0, vlu0, vlv1);
                var5.addVertexWithUV((double)(var2 + (1 / 64f)), (double)(var3 + 1 + 7f / 320f), var4 + 0, vlu1, vlv1);
                var5.setColorOpaque_F(var8, var8, var8);
                var5.addVertexWithUV((double)(var2 + (1 / 64f)), (double)(var3 + 1 + 7f / 320f), var4 + 1, vlu1, vlv0 + vOff2);
                var5.addVertexWithUV((double)(var2 + (1 / 64f)), var3 + 0, var4 + 1, vlu0, vlv0 + vOff2);
                var5.addVertexWithUV((double)(var2 + (1 / 64f)), var3 + 0, var4 + 0, vlu0, vlv1 + vOff2);
                var5.addVertexWithUV((double)(var2 + (1 / 64f)), (double)(var3 + 1 + 7f / 320f), var4 + 0, vlu1, vlv1 + vOff2);
            }
            if (_blockAccess.shouldSuffocate(var2 + 1, var3, var4) && _blockAccess.getBlockId(var2 + 1, var3 + 1, var4) == Block.RedstoneWire.id)
            {
                var5.setColorOpaque_F(var8 * var10, var8 * var11, var8 * var12);
                var5.addVertexWithUV((double)(var2 + 1 - (1 / 64f)), var3 + 0, var4 + 1, vlu0, vlv1);
                var5.addVertexWithUV((double)(var2 + 1 - (1 / 64f)), (double)(var3 + 1 + 7f / 320f), var4 + 1, vlu1, vlv1);
                var5.addVertexWithUV((double)(var2 + 1 - (1 / 64f)), (double)(var3 + 1 + 7f / 320f), var4 + 0, vlu1, vlv0);
                var5.addVertexWithUV((double)(var2 + 1 - (1 / 64f)), var3 + 0, var4 + 0, vlu0, vlv0);
                var5.setColorOpaque_F(var8, var8, var8);
                var5.addVertexWithUV((double)(var2 + 1 - (1 / 64f)), var3 + 0, var4 + 1, vlu0, vlv1 + vOff2);
                var5.addVertexWithUV((double)(var2 + 1 - (1 / 64f)), (double)(var3 + 1 + 7f / 320f), var4 + 1, vlu1, vlv1 + vOff2);
                var5.addVertexWithUV((double)(var2 + 1 - (1 / 64f)), (double)(var3 + 1 + 7f / 320f), var4 + 0, vlu1, vlv0 + vOff2);
                var5.addVertexWithUV((double)(var2 + 1 - (1 / 64f)), var3 + 0, var4 + 0, vlu0, vlv0 + vOff2);
            }
            if (_blockAccess.shouldSuffocate(var2, var3, var4 - 1) && _blockAccess.getBlockId(var2, var3 + 1, var4 - 1) == Block.RedstoneWire.id)
            {
                var5.setColorOpaque_F(var8 * var10, var8 * var11, var8 * var12);
                var5.addVertexWithUV(var2 + 1, var3 + 0, (double)(var4 + (1 / 64f)), vlu0, vlv1);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + 1 + 7f / 320f), (double)(var4 + (1 / 64f)), vlu1, vlv1);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 1 + 7f / 320f), (double)(var4 + (1 / 64f)), vlu1, vlv0);
                var5.addVertexWithUV(var2 + 0, var3 + 0, (double)(var4 + (1 / 64f)), vlu0, vlv0);
                var5.setColorOpaque_F(var8, var8, var8);
                var5.addVertexWithUV(var2 + 1, var3 + 0, (double)(var4 + (1 / 64f)), vlu0, vlv1 + vOff2);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + 1 + 7f / 320f), (double)(var4 + (1 / 64f)), vlu1, vlv1 + vOff2);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 1 + 7f / 320f), (double)(var4 + (1 / 64f)), vlu1, vlv0 + vOff2);
                var5.addVertexWithUV(var2 + 0, var3 + 0, (double)(var4 + (1 / 64f)), vlu0, vlv0 + vOff2);
            }
            if (_blockAccess.shouldSuffocate(var2, var3, var4 + 1) && _blockAccess.getBlockId(var2, var3 + 1, var4 + 1) == Block.RedstoneWire.id)
            {
                var5.setColorOpaque_F(var8 * var10, var8 * var11, var8 * var12);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + 1 + 7f / 320f), (double)(var4 + 1 - (1 / 64f)), vlu1, vlv0);
                var5.addVertexWithUV(var2 + 1, var3 + 0, (double)(var4 + 1 - (1 / 64f)), vlu0, vlv0);
                var5.addVertexWithUV(var2 + 0, var3 + 0, (double)(var4 + 1 - (1 / 64f)), vlu0, vlv1);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 1 + 7f / 320f), (double)(var4 + 1 - (1 / 64f)), vlu1, vlv1);
                var5.setColorOpaque_F(var8, var8, var8);
                var5.addVertexWithUV(var2 + 1, (double)(var3 + 1 + 7f / 320f), (double)(var4 + 1 - (1 / 64f)), vlu1, vlv0 + vOff2);
                var5.addVertexWithUV(var2 + 1, var3 + 0, (double)(var4 + 1 - (1 / 64f)), vlu0, vlv0 + vOff2);
                var5.addVertexWithUV(var2 + 0, var3 + 0, (double)(var4 + 1 - (1 / 64f)), vlu0, vlv1 + vOff2);
                var5.addVertexWithUV(var2 + 0, (double)(var3 + 1 + 7f / 320f), (double)(var4 + 1 - (1 / 64f)), vlu1, vlv1 + vOff2);
            }
        }
        return true;
    }

    // ── Minecart Track ───────────────────────────────────────────────────────

    public bool renderBlockMinecartTrack(BlockRail var1, int var2, int var3, int var4)
    {
        Tessellator var5 = getTessellator();
        int var6 = _blockAccess.getBlockMeta(var2, var3, var4);
        string texName = _overrideBlockTexture ?? var1.getTexture("down", var6);
        if (var1.isAlwaysStraight()) var6 &= 7;

        float var8 = var1.getLuminance(_blockAccess, var2, var3, var4);
        var5.setColorOpaque_F(var8, var8, var8);

        UVRegion uv = ResolveUV(texName);
        double var11 = uv.U0, var13 = uv.U1, var15 = uv.V0, var17 = uv.V1;

        float var19 = 1.0F / 16.0F;
        float var20 = var2 + 1, var21 = var2 + 1, var22 = var2 + 0, var23 = var2 + 0;
        float var24 = var4 + 0, var25 = var4 + 1, var26 = var4 + 1, var27 = var4 + 0;
        float var28 = var3 + var19, var29 = var3 + var19, var30 = var3 + var19, var31 = var3 + var19;

        if (var6 != 1 && var6 != 2 && var6 != 3 && var6 != 7)
        {
            if (var6 == 8) { var21 = var2; var20 = var21; var23 = var2 + 1; var22 = var23; var27 = var4 + 1; var24 = var27; var26 = var4; var25 = var26; }
            else if (var6 == 9) { var23 = var2; var20 = var23; var22 = var2 + 1; var21 = var22; var25 = var4; var24 = var25; var27 = var4 + 1; var26 = var27; }
        }
        else { var23 = var2 + 1; var20 = var23; var22 = var2; var21 = var22; var25 = var4 + 1; var24 = var25; var27 = var4; var26 = var27; }

        if (var6 == 2 || var6 == 4) { ++var28; ++var31; }
        else if (var6 == 3 || var6 == 5) { ++var29; ++var30; }

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

    // ── Ladder ───────────────────────────────────────────────────────────────

    public bool renderBlockLadder(Block var1, int var2, int var3, int var4)
    {
        Tessellator var5 = getTessellator();
        string texName = _overrideBlockTexture ?? var1.getTexture("down");
        float var7 = var1.getLuminance(_blockAccess, var2, var3, var4);
        var5.setColorOpaque_F(var7, var7, var7);

        UVRegion uv = ResolveUV(texName);
        double var10 = uv.U0, var12 = uv.U1, var14 = uv.V0, var16 = uv.V1;

        int var18 = _blockAccess.getBlockMeta(var2, var3, var4);
        float var19 = 0.0F, var20 = 0.05F;
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

    // ── Reed / Crossed Squares ───────────────────────────────────────────────

    public bool renderBlockReed(Block var1, int var2, int var3, int var4)
    {
        Tessellator var5 = getTessellator();
        float var6 = var1.getLuminance(_blockAccess, var2, var3, var4);
        int var7 = var1.getColorMultiplier(_blockAccess, var2, var3, var4);
        float var8 = (var7 >> 16 & 255) / 255.0F;
        float var9 = (var7 >> 8 & 255) / 255.0F;
        float var10 = (var7 & 255) / 255.0F;
        var5.setColorOpaque_F(var6 * var8, var6 * var9, var6 * var10);

        double var19 = var2, var20 = var3, var15 = var4;
        if (var1 == Block.Grass)
        {
            long var17 = var2 * 3129871L ^ var4 * 116129781L ^ var3;
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

    public void renderCrossedSquares(Block var1, int var2, double var3, double var5, double var7)
    {
        Tessellator var9 = getTessellator();
        string texName = _overrideBlockTexture ?? var1.getTexture("down", var2);
        UVRegion uv = ResolveUV(texName);
        double u0 = uv.U0, u1 = uv.U1, v0 = uv.V0, v1 = uv.V1;

        double var21 = var3 + 0.5D - (double)0.45F, var23 = var3 + 0.5D + (double)0.45F;
        double var25 = var7 + 0.5D - (double)0.45F, var27 = var7 + 0.5D + (double)0.45F;
        var9.addVertexWithUV(var21, var5 + 1.0D, var25, u0, v0);
        var9.addVertexWithUV(var21, var5 + 0.0D, var25, u0, v1);
        var9.addVertexWithUV(var23, var5 + 0.0D, var27, u1, v1);
        var9.addVertexWithUV(var23, var5 + 1.0D, var27, u1, v0);
        var9.addVertexWithUV(var23, var5 + 1.0D, var27, u0, v0);
        var9.addVertexWithUV(var23, var5 + 0.0D, var27, u0, v1);
        var9.addVertexWithUV(var21, var5 + 0.0D, var25, u1, v1);
        var9.addVertexWithUV(var21, var5 + 1.0D, var25, u1, v0);
        var9.addVertexWithUV(var21, var5 + 1.0D, var27, u0, v0);
        var9.addVertexWithUV(var21, var5 + 0.0D, var27, u0, v1);
        var9.addVertexWithUV(var23, var5 + 0.0D, var25, u1, v1);
        var9.addVertexWithUV(var23, var5 + 1.0D, var25, u1, v0);
        var9.addVertexWithUV(var23, var5 + 1.0D, var25, u0, v0);
        var9.addVertexWithUV(var23, var5 + 0.0D, var25, u0, v1);
        var9.addVertexWithUV(var21, var5 + 0.0D, var27, u1, v1);
        var9.addVertexWithUV(var21, var5 + 1.0D, var27, u1, v0);
    }

    public void func_1245_b(Block var1, int var2, double var3, double var5, double var7)
    {
        Tessellator var9 = getTessellator();
        string texName = _overrideBlockTexture ?? var1.getTexture("bottom", var2);
        UVRegion uv = ResolveUV(texName);
        double u0 = uv.U0, u1 = uv.U1, v0 = uv.V0, v1 = uv.V1;

        double var21 = var3 + 0.5D - 0.25D, var23 = var3 + 0.5D + 0.25D;
        double var25 = var7 + 0.5D - 0.5D, var27 = var7 + 0.5D + 0.5D;
        var9.addVertexWithUV(var21, var5 + 1.0D, var25, u0, v0); var9.addVertexWithUV(var21, var5 + 0.0D, var25, u0, v1);
        var9.addVertexWithUV(var21, var5 + 0.0D, var27, u1, v1); var9.addVertexWithUV(var21, var5 + 1.0D, var27, u1, v0);
        var9.addVertexWithUV(var21, var5 + 1.0D, var27, u0, v0); var9.addVertexWithUV(var21, var5 + 0.0D, var27, u0, v1);
        var9.addVertexWithUV(var21, var5 + 0.0D, var25, u1, v1); var9.addVertexWithUV(var21, var5 + 1.0D, var25, u1, v0);
        var9.addVertexWithUV(var23, var5 + 1.0D, var27, u0, v0); var9.addVertexWithUV(var23, var5 + 0.0D, var27, u0, v1);
        var9.addVertexWithUV(var23, var5 + 0.0D, var25, u1, v1); var9.addVertexWithUV(var23, var5 + 1.0D, var25, u1, v0);
        var9.addVertexWithUV(var23, var5 + 1.0D, var25, u0, v0); var9.addVertexWithUV(var23, var5 + 0.0D, var25, u0, v1);
        var9.addVertexWithUV(var23, var5 + 0.0D, var27, u1, v1); var9.addVertexWithUV(var23, var5 + 1.0D, var27, u1, v0);
        var21 = var3 + 0.5D - 0.5D; var23 = var3 + 0.5D + 0.5D; var25 = var7 + 0.5D - 0.25D; var27 = var7 + 0.5D + 0.25D;
        var9.addVertexWithUV(var21, var5 + 1.0D, var25, u0, v0); var9.addVertexWithUV(var21, var5 + 0.0D, var25, u0, v1);
        var9.addVertexWithUV(var23, var5 + 0.0D, var25, u1, v1); var9.addVertexWithUV(var23, var5 + 1.0D, var25, u1, v0);
        var9.addVertexWithUV(var23, var5 + 1.0D, var25, u0, v0); var9.addVertexWithUV(var23, var5 + 0.0D, var25, u0, v1);
        var9.addVertexWithUV(var21, var5 + 0.0D, var25, u1, v1); var9.addVertexWithUV(var21, var5 + 1.0D, var25, u1, v0);
        var9.addVertexWithUV(var23, var5 + 1.0D, var27, u0, v0); var9.addVertexWithUV(var23, var5 + 0.0D, var27, u0, v1);
        var9.addVertexWithUV(var21, var5 + 0.0D, var27, u1, v1); var9.addVertexWithUV(var21, var5 + 1.0D, var27, u1, v0);
        var9.addVertexWithUV(var21, var5 + 1.0D, var27, u0, v0); var9.addVertexWithUV(var21, var5 + 0.0D, var27, u0, v1);
        var9.addVertexWithUV(var23, var5 + 0.0D, var27, u1, v1); var9.addVertexWithUV(var23, var5 + 1.0D, var27, u1, v0);
    }

    // ── Fluids ───────────────────────────────────────────────────────────────

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
        bool[] var12 = [
            var1.isSideVisible(_blockAccess, var2, var3, var4-1, 2),
            var1.isSideVisible(_blockAccess, var2, var3, var4+1, 3),
            var1.isSideVisible(_blockAccess, var2-1, var3, var4, 4),
            var1.isSideVisible(_blockAccess, var2+1, var3, var4, 5)
        ];
        if (!var10 && !var11 && !var12[0] && !var12[1] && !var12[2] && !var12[3]) return false;

        bool var13 = false;
        float var14 = 0.5F, var15 = 1.0F, var16 = 0.8F, var17 = 0.6F;
        double var18 = 0.0D, var20 = 1.0D;
        Material var22 = var1.material;
        int var23 = _blockAccess.getBlockMeta(var2, var3, var4);
        float var24 = func_1224_a(var2, var3, var4, var22);
        float var25 = func_1224_a(var2, var3, var4 + 1, var22);
        float var26 = func_1224_a(var2 + 1, var3, var4 + 1, var22);
        float var27 = func_1224_a(var2 + 1, var3, var4, var22);
        int var28, var31;
        float var36, var37, var38;

        if (_renderAllFaces || var10)
        {
            var13 = true;
            string texTop = var1.getTexture("up", var23);
            float var29 = (float)BlockFluid.getFlowingAngle(_blockAccess, var2, var3, var4, var22);
            if (var29 > -999.0F) texTop = var1.getTexture("north", var23);
            UVRegion uvTop = ResolveUV(texTop);
            double uMid = uvTop.U0 + (uvTop.U1 - uvTop.U0) * 0.5;
            double vMid = uvTop.V0 + (uvTop.V1 - uvTop.V0) * 0.5;
            if (var29 < -999.0F) var29 = 0.0F;
            var36 = MathHelper.Sin(var29) * 8.0F / 16.0F * (float)(uvTop.U1 - uvTop.U0);
            var37 = MathHelper.Cos(var29) * 8.0F / 16.0F * (float)(uvTop.V1 - uvTop.V0);
            float var38f = var1.getLuminance(_blockAccess, var2, var3, var4);
            var5.setColorOpaque_F(var15 * var38f * var7, var15 * var38f * var8, var15 * var38f * var9);
            var5.addVertexWithUV(var2 + 0, (double)(var3 + var24), var4 + 0, uMid - (double)var37 - (double)var36, vMid - (double)var37 + (double)var36);
            var5.addVertexWithUV(var2 + 0, (double)(var3 + var25), var4 + 1, uMid - (double)var37 + (double)var36, vMid + (double)var37 + (double)var36);
            var5.addVertexWithUV(var2 + 1, (double)(var3 + var26), var4 + 1, uMid + (double)var37 + (double)var36, vMid + (double)var37 - (double)var36);
            var5.addVertexWithUV(var2 + 1, (double)(var3 + var27), var4 + 0, uMid + (double)var37 - (double)var36, vMid - (double)var37 - (double)var36);
        }

        if (_renderAllFaces || var11)
        {
            float var52 = var1.getLuminance(_blockAccess, var2, var3 - 1, var4);
            var5.setColorOpaque_F(var14 * var52, var14 * var52, var14 * var52);
            renderBottomFace(var1, var2, var3, var4, var1.getTexture("down"));
            var13 = true;
        }

        for (var28 = 0; var28 < 4; ++var28)
        {
            int var53 = var2; var31 = var4;
            if (var28 == 0) var31 = var4 - 1;
            if (var28 == 1) ++var31;
            if (var28 == 2) var53 = var2 - 1;
            if (var28 == 3) ++var53;

            string var54 = var1.getTexture(Block.faceInt2String(var28 + 2), var23);
            UVRegion uvSide = ResolveUV(var54);

            if (_renderAllFaces || var12[var28])
            {
                float var35, var39, var40;
                if (var28 == 0) { var35 = var24; var36 = var27; var37 = var2; var39 = var2 + 1; var38 = var4; var40 = var4; }
                else if (var28 == 1) { var35 = var26; var36 = var25; var37 = var2 + 1; var39 = var2; var38 = var4 + 1; var40 = var4 + 1; }
                else if (var28 == 2) { var35 = var25; var36 = var24; var37 = var2; var39 = var2; var38 = var4 + 1; var40 = var4; }
                else { var35 = var27; var36 = var26; var37 = var2 + 1; var39 = var2 + 1; var38 = var4; var40 = var4 + 1; }

                var13 = true;
                double uA = uvSide.U0, uB = uvSide.U1;
                double vA = uvSide.V0 + (uvSide.V1 - uvSide.V0) * (1f - var35);
                double vB = uvSide.V0 + (uvSide.V1 - uvSide.V0) * (1f - var36);
                double vBot = uvSide.V1;
                float var51 = var1.getLuminance(_blockAccess, var53, var3, var31);
                if (var28 < 2) var51 *= var16; else var51 *= var17;
                var5.setColorOpaque_F(var15 * var51 * var7, var15 * var51 * var8, var15 * var51 * var9);
                var5.addVertexWithUV((double)var37, (double)(var3 + var35), (double)var38, uA, vA);
                var5.addVertexWithUV((double)var39, (double)(var3 + var36), (double)var40, uB, vB);
                var5.addVertexWithUV((double)var39, var3 + 0, (double)var40, uB, vBot);
                var5.addVertexWithUV((double)var37, var3 + 0, (double)var38, uA, vBot);
            }
        }
        blockBb.MinY = var18;
        blockBb.MaxY = var20;
        return var13;
    }

    private float func_1224_a(int var1, int var2, int var3, Material var4)
    {
        int var5 = 0; float var6 = 0.0F;
        for (int var7 = 0; var7 < 4; ++var7)
        {
            int var8 = var1 - (var7 & 1), var10 = var3 - (var7 >> 1 & 1);
            if (_blockAccess.getMaterial(var8, var2 + 1, var10) == var4) return 1.0F;
            Material var11 = _blockAccess.getMaterial(var8, var2, var10);
            if (var11 != var4)
            {
                if (!_blockAccess.shouldSuffocate(var8, var2, var10)) { ++var5; var6 += BlockFluid.getFluidHeightFromMeta(_blockAccess.getBlockMeta(var1, var2, var3)); }
            }
            else { ++var5; var6 += BlockFluid.getFluidHeightFromMeta(_blockAccess.getBlockMeta(var1,var2,var3)); }
        }
        return 1.0F - var6 / var5;
    }

    // ── renderBlockFallingSand ───────────────────────────────────────────────

    public void renderBlockFallingSand(Block var1, World var2, int var3, int var4, int var5)
    {
        float var6 = 0.5F, var7 = 1.0F, var8 = 0.8F, var9 = 0.6F;
        Tessellator var10 = getTessellator();
        var10.startDrawingQuads();
        float var11 = var1.getLuminance(var2, var3, var4, var5);
        float var12 = var1.getLuminance(var2, var3, var4 - 1, var5);
        if (var12 < var11) var12 = var11;
        var10.setColorOpaque_F(var6 * var12, var6 * var12, var6 * var12);
        renderBottomFace(var1, -0.5D, -0.5D, -0.5D, var1.getTexture("down"));
        var12 = var1.getLuminance(var2, var3, var4 + 1, var5); if (var12 < var11) var12 = var11;
        var10.setColorOpaque_F(var7 * var12, var7 * var12, var7 * var12);
        renderTopFace(var1, -0.5D, -0.5D, -0.5D, var1.getTexture("up"));
        var12 = var1.getLuminance(var2, var3, var4, var5 - 1); if (var12 < var11) var12 = var11;
        var10.setColorOpaque_F(var8 * var12, var8 * var12, var8 * var12);
        renderEastFace(var1, -0.5D, -0.5D, -0.5D, var1.getTexture("north"));
        var12 = var1.getLuminance(var2, var3, var4, var5 + 1); if (var12 < var11) var12 = var11;
        var10.setColorOpaque_F(var8 * var12, var8 * var12, var8 * var12);
        renderWestFace(var1, -0.5D, -0.5D, -0.5D, var1.getTexture("south"));
        var12 = var1.getLuminance(var2, var3 - 1, var4, var5); if (var12 < var11) var12 = var11;
        var10.setColorOpaque_F(var9 * var12, var9 * var12, var9 * var12);
        renderNorthFace(var1, -0.5D, -0.5D, -0.5D, var1.getTexture("west"));
        var12 = var1.getLuminance(var2, var3 + 1, var4, var5); if (var12 < var11) var12 = var11;
        var10.setColorOpaque_F(var9 * var12, var9 * var12, var9 * var12);
        renderSouthFace(var1, -0.5D, -0.5D, -0.5D, var1.getTexture("east"));
        var10.draw();
    }

    // ── Standard Block ───────────────────────────────────────────────────────

    public bool renderStandardBlock(Block var1, int var2, int var3, int var4)
    {
        int var5 = var1.getColorMultiplier(_blockAccess, var2, var3, var4);
        float var6 = (var5 >> 16 & 255) / 255.0F;
        float var7 = (var5 >> 8 & 255) / 255.0F;
        float var8 = (var5 & 255) / 255.0F;
        return Minecraft.isAmbientOcclusionEnabled()
            ? renderStandardBlockWithAmbientOcclusion(var1, var2, var3, var4, var6, var7, var8)
            : renderStandardBlockWithColorMultiplier(var1, var2, var3, var4, var6, var7, var8);
    }

    public bool renderStandardBlockWithAmbientOcclusion(Block var1, int var2, int var3, int var4, float var5, float var6, float var7)
    {
        _enableAO = true;
        bool var8 = false;
        float var9 = _lightValueOwn, var10 = _lightValueOwn, var11 = _lightValueOwn, var12 = _lightValueOwn;
        bool var13 = true, var14 = true, var15 = true, var16 = true, var17 = true, var18 = true;
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

        if (var1.textureId == "grass_block_side" || _overrideBlockTexture != null)
            var18 = var17 = var16 = var15 = var13 = false;

        // ── Bottom ──
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3 - 1, var4, 0))
        {
            if (_aoBlendMode <= 0) { var9 = var10 = var11 = var12 = _aoLightValueYNeg; }
            else
            {
                --var3;
                _colorRedBottomLeft_V = var1.getLuminance(_blockAccess, var2 - 1, var3, var4);
                _colorRedTopRight_V = var1.getLuminance(_blockAccess, var2, var3, var4 - 1);
                _colorGreenTopLeft_V = var1.getLuminance(_blockAccess, var2, var3, var4 + 1);
                _colorGreenBottomRight_V = var1.getLuminance(_blockAccess, var2 + 1, var3, var4);
                _colorRedTopLeft_V = (!_aoBlockOpYNegZNeg && !_aoBlockOpYNegZPos) ? _colorRedBottomLeft_V : var1.getLuminance(_blockAccess, var2 - 1, var3, var4 - 1);
                _colorRedBottomRight_V = (!_aoBlockOpYPosZPos && !_aoBlockOpYNegZPos) ? _colorRedBottomLeft_V : var1.getLuminance(_blockAccess, var2 - 1, var3, var4 + 1);
                _colorGreenBottomLeft_V = (!_aoBlockOpYNegZNeg && !_aoBlockOpYPosZNeg) ? _colorGreenBottomRight_V : var1.getLuminance(_blockAccess, var2 + 1, var3, var4 - 1);
                _colorGreenTopRight_V = (!_aoBlockOpYPosZPos && !_aoBlockOpYPosZNeg) ? _colorGreenBottomRight_V : var1.getLuminance(_blockAccess, var2 + 1, var3, var4 + 1);
                ++var3;
                var9 = (_colorRedBottomRight_V + _colorRedBottomLeft_V + _colorGreenTopLeft_V + _aoLightValueYNeg) / 4f;
                var12 = (_colorGreenTopLeft_V + _aoLightValueYNeg + _colorGreenTopRight_V + _colorGreenBottomRight_V) / 4f;
                var11 = (_aoLightValueYNeg + _colorRedTopRight_V + _colorGreenBottomRight_V + _colorGreenBottomLeft_V) / 4f;
                var10 = (_colorRedBottomLeft_V + _colorRedTopLeft_V + _aoLightValueYNeg + _colorRedTopRight_V) / 4f;
            }
            _colorRedTopLeft = _colorRedBottomLeft = _colorRedBottomRight = _colorRedTopRight = (var13 ? var5 : 1f) * 0.5f;
            _colorGreenTopLeft = _colorGreenBottomLeft = _colorGreenBottomRight = _colorGreenTopRight = (var13 ? var6 : 1f) * 0.5f;
            _colorBlueTopLeft = _colorBlueBottomLeft = _colorBlueBottomRight = _colorBlueTopRight = (var13 ? var7 : 1f) * 0.5f;
            _colorRedTopLeft *= var9; _colorGreenTopLeft *= var9; _colorBlueTopLeft *= var9;
            _colorRedBottomLeft *= var10; _colorGreenBottomLeft *= var10; _colorBlueBottomLeft *= var10;
            _colorRedBottomRight *= var11; _colorGreenBottomRight *= var11; _colorBlueBottomRight *= var11;
            _colorRedTopRight *= var12; _colorGreenTopRight *= var12; _colorBlueTopRight *= var12;
            renderBottomFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "bottom"));
            var8 = true;
        }

        // ── Top ──
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3 + 1, var4, 1))
        {
            if (_aoBlendMode <= 0) { var9 = var10 = var11 = var12 = _aoLightValueYPos; }
            else
            {
                ++var3;
                _colorBlueBottomLeft_V = var1.getLuminance(_blockAccess, var2 - 1, var3, var4);
                _aoLightValueScratch2 = var1.getLuminance(_blockAccess, var2 + 1, var3, var4);
                _colorBlueTopRight_V = var1.getLuminance(_blockAccess, var2, var3, var4 - 1);
                _aoLightValueScratch3 = var1.getLuminance(_blockAccess, var2, var3, var4 + 1);
                _colorBlueTopLeft_V = (!_aoBlockOpXNegYPos && !_aoBlockOpXNegYNeg) ? _colorBlueBottomLeft_V : var1.getLuminance(_blockAccess, var2 - 1, var3, var4 - 1);
                _aoLightValueScratch1 = (!_aoBlockOpXNegYPos && !_aoBlockOpXPosYPos) ? _aoLightValueScratch2 : var1.getLuminance(_blockAccess, var2 + 1, var3, var4 - 1);
                _colorBlueBottomRight_V = (!_aoBlockOpXPosYNeg && !_aoBlockOpXNegYNeg) ? _colorBlueBottomLeft_V : var1.getLuminance(_blockAccess, var2 - 1, var3, var4 + 1);
                _aoLightValueScratch4 = (!_aoBlockOpXPosYNeg && !_aoBlockOpXPosYPos) ? _aoLightValueScratch2 : var1.getLuminance(_blockAccess, var2 + 1, var3, var4 + 1);
                --var3;
                var12 = (_colorBlueBottomRight_V + _colorBlueBottomLeft_V + _aoLightValueScratch3 + _aoLightValueYPos) / 4f;
                var9 = (_aoLightValueScratch3 + _aoLightValueYPos + _aoLightValueScratch4 + _aoLightValueScratch2) / 4f;
                var10 = (_aoLightValueYPos + _colorBlueTopRight_V + _aoLightValueScratch2 + _aoLightValueScratch1) / 4f;
                var11 = (_colorBlueBottomLeft_V + _colorBlueTopLeft_V + _aoLightValueYPos + _colorBlueTopRight_V) / 4f;
            }
            _colorRedTopLeft = _colorRedBottomLeft = _colorRedBottomRight = _colorRedTopRight = (var14 ? var5 : 1f);
            _colorGreenTopLeft = _colorGreenBottomLeft = _colorGreenBottomRight = _colorGreenTopRight = (var14 ? var6 : 1f);
            _colorBlueTopLeft = _colorBlueBottomLeft = _colorBlueBottomRight = _colorBlueTopRight = (var14 ? var7 : 1f);
            _colorRedTopLeft *= var9; _colorGreenTopLeft *= var9; _colorBlueTopLeft *= var9;
            _colorRedBottomLeft *= var10; _colorGreenBottomLeft *= var10; _colorBlueBottomLeft *= var10;
            _colorRedBottomRight *= var11; _colorGreenBottomRight *= var11; _colorBlueBottomRight *= var11;
            _colorRedTopRight *= var12; _colorGreenTopRight *= var12; _colorBlueTopRight *= var12;
            renderTopFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "top"));
            var8 = true;
        }

        // ── East (Z-) ──
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3, var4 - 1, 2))
        {
            Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
            float var27 = var1.getLuminance(_blockAccess, var2, var3, var4 - 1);
            if (blockBB.MinZ > 0.0D) var27 = _lightValueOwn;
            if (_aoBlendMode <= 0) { var9 = var10 = var11 = var12 = _aoLightValueZNeg; }
            else
            {
                --var4;
                float fXN = var1.getLuminance(_blockAccess, var2 - 1, var3, var4), fXP = var1.getLuminance(_blockAccess, var2 + 1, var3, var4);
                float fYN = var1.getLuminance(_blockAccess, var2, var3 - 1, var4), fYP = var1.getLuminance(_blockAccess, var2, var3 + 1, var4);
                float fXNYN = (!_aoBlockOpXNegZNeg && !_aoBlockOpYNegZNeg) ? fXN : var1.getLuminance(_blockAccess, var2 - 1, var3 - 1, var4);
                float fXNYP = (!_aoBlockOpXNegZNeg && !_aoBlockOpXNegYPos) ? fXN : var1.getLuminance(_blockAccess, var2 - 1, var3 + 1, var4);  // approx
                float fXPYN = (!_aoBlockOpXPosZNeg && !_aoBlockOpYPosZNeg) ? fXP : var1.getLuminance(_blockAccess, var2 + 1, var3 - 1, var4);
                float fXPYP = (!_aoBlockOpXPosZNeg && !_aoBlockOpXNegYNeg) ? fXP : var1.getLuminance(_blockAccess, var2 + 1, var3 + 1, var4);
                ++var4;
                var9 = (fXNYP + fXN + fYP + _aoLightValueZNeg) / 4f;
                var12 = (fYP + _aoLightValueZNeg + fXPYP + fXP) / 4f;
                var11 = (_aoLightValueZNeg + fYN + fXP + fXPYN) / 4f;
                var10 = (fXN + fXNYN + _aoLightValueZNeg + fYN) / 4f;
            }
            _colorRedTopLeft = _colorRedBottomLeft = _colorRedBottomRight = _colorRedTopRight = (var15 ? var5 : 1f) * 0.8f;
            _colorGreenTopLeft = _colorGreenBottomLeft = _colorGreenBottomRight = _colorGreenTopRight = (var15 ? var6 : 1f) * 0.8f;
            _colorBlueTopLeft = _colorBlueBottomLeft = _colorBlueBottomRight = _colorBlueTopRight = (var15 ? var7 : 1f) * 0.8f;
            _colorRedTopLeft *= var9; _colorGreenTopLeft *= var9; _colorBlueTopLeft *= var9;
            _colorRedBottomLeft *= var10; _colorGreenBottomLeft *= var10; _colorBlueBottomLeft *= var10;
            _colorRedBottomRight *= var11; _colorGreenBottomRight *= var11; _colorBlueBottomRight *= var11;
            _colorRedTopRight *= var12; _colorGreenTopRight *= var12; _colorBlueTopRight *= var12;
            renderEastFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "north"));
            var8 = true;
        }

        // ── West (Z+) ──
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3, var4 + 1, 3))
        {
            Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
            float var27 = var1.getLuminance(_blockAccess, var2, var3, var4 + 1);
            if (blockBB.MaxZ < 1.0D) var27 = _lightValueOwn;
            if (_aoBlendMode <= 0) { var9 = var10 = var11 = var12 = _aoLightValueZPos; }
            else
            {
                ++var4;
                float fXN = var1.getLuminance(_blockAccess, var2 - 1, var3, var4), fXP = var1.getLuminance(_blockAccess, var2 + 1, var3, var4);
                float fYN = var1.getLuminance(_blockAccess, var2, var3 - 1, var4), fYP = var1.getLuminance(_blockAccess, var2, var3 + 1, var4);
                float fXNYN = (!_aoBlockOpXNegZPos && !_aoBlockOpYNegZPos) ? fXN : var1.getLuminance(_blockAccess, var2 - 1, var3 - 1, var4);
                float fXNYP = (!_aoBlockOpXNegZPos && !_aoBlockOpXNegYNeg) ? fXN : var1.getLuminance(_blockAccess, var2 - 1, var3 + 1, var4);
                float fXPYN = (!_aoBlockOpXPosZPos && !_aoBlockOpYPosZPos) ? fXP : var1.getLuminance(_blockAccess, var2 + 1, var3 - 1, var4);
                float fXPYP = (!_aoBlockOpXPosZPos && !_aoBlockOpXPosYNeg) ? fXP : var1.getLuminance(_blockAccess, var2 + 1, var3 + 1, var4);
                --var4;
                var9 = (fYP + _aoLightValueZPos + fXPYP + fXP) / 4f;
                var12 = (fXNYP + fXN + fYP + _aoLightValueZPos) / 4f;
                var11 = (fXN + fXNYN + _aoLightValueZPos + fYN) / 4f;
                var10 = (_aoLightValueZPos + fYN + fXP + fXPYN) / 4f;
            }
            _colorRedTopLeft = _colorRedBottomLeft = _colorRedBottomRight = _colorRedTopRight = (var16 ? var5 : 1f) * 0.8f;
            _colorGreenTopLeft = _colorGreenBottomLeft = _colorGreenBottomRight = _colorGreenTopRight = (var16 ? var6 : 1f) * 0.8f;
            _colorBlueTopLeft = _colorBlueBottomLeft = _colorBlueBottomRight = _colorBlueTopRight = (var16 ? var7 : 1f) * 0.8f;
            _colorRedTopLeft *= var9; _colorGreenTopLeft *= var9; _colorBlueTopLeft *= var9;
            _colorRedBottomLeft *= var10; _colorGreenBottomLeft *= var10; _colorBlueBottomLeft *= var10;
            _colorRedBottomRight *= var11; _colorGreenBottomRight *= var11; _colorBlueBottomRight *= var11;
            _colorRedTopRight *= var12; _colorGreenTopRight *= var12; _colorBlueTopRight *= var12;
            renderWestFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "south"));
            var8 = true;
        }

        // ── North (X-) ──
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2 - 1, var3, var4, 4))
        {
            Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
            float var27 = var1.getLuminance(_blockAccess, var2 - 1, var3, var4);
            if (blockBB.MinX > 0.0D) var27 = _lightValueOwn;
            if (_aoBlendMode <= 0) { var9 = var10 = var11 = var12 = _aoLightValueXNeg; }
            else
            {
                --var2;
                float fZN = var1.getLuminance(_blockAccess, var2, var3, var4 - 1), fZP = var1.getLuminance(_blockAccess, var2, var3, var4 + 1);
                float fYN = var1.getLuminance(_blockAccess, var2, var3 - 1, var4), fYP = var1.getLuminance(_blockAccess, var2, var3 + 1, var4);
                float fZNYN = (!_aoBlockOpXNegZNeg && !_aoBlockOpYNegZNeg) ? fZN : var1.getLuminance(_blockAccess, var2, var3 - 1, var4 - 1);
                float fZNYP = (!_aoBlockOpXNegZNeg && !_aoBlockOpXNegYPos) ? fZN : var1.getLuminance(_blockAccess, var2, var3 + 1, var4 - 1);
                float fZPYN = (!_aoBlockOpXNegZPos && !_aoBlockOpYNegZPos) ? fZP : var1.getLuminance(_blockAccess, var2, var3 - 1, var4 + 1);
                float fZPYP = (!_aoBlockOpXNegZPos && !_aoBlockOpXNegYNeg) ? fZP : var1.getLuminance(_blockAccess, var2, var3 + 1, var4 + 1);
                ++var2;
                var9 = (fZNYP + fZN + fYP + _aoLightValueXNeg) / 4f;
                var12 = (fYP + _aoLightValueXNeg + fZPYP + fZP) / 4f;
                var11 = (_aoLightValueXNeg + fYN + fZP + fZPYN) / 4f;
                var10 = (fZN + fZNYN + _aoLightValueXNeg + fYN) / 4f;
            }
            _colorRedTopLeft = _colorRedBottomLeft = _colorRedBottomRight = _colorRedTopRight = (var17 ? var5 : 1f) * 0.6f;
            _colorGreenTopLeft = _colorGreenBottomLeft = _colorGreenBottomRight = _colorGreenTopRight = (var17 ? var6 : 1f) * 0.6f;
            _colorBlueTopLeft = _colorBlueBottomLeft = _colorBlueBottomRight = _colorBlueTopRight = (var17 ? var7 : 1f) * 0.6f;
            _colorRedTopLeft *= var9; _colorGreenTopLeft *= var9; _colorBlueTopLeft *= var9;
            _colorRedBottomLeft *= var10; _colorGreenBottomLeft *= var10; _colorBlueBottomLeft *= var10;
            _colorRedBottomRight *= var11; _colorGreenBottomRight *= var11; _colorBlueBottomRight *= var11;
            _colorRedTopRight *= var12; _colorGreenTopRight *= var12; _colorBlueTopRight *= var12;
            string var28 = var1.getTextureId(_blockAccess, var2, var3, var4, "west");
            renderNorthFace(var1, var2, var3, var4, var28);
            if (s_fancyGrass && var28 == "grass_block_side" && _overrideBlockTexture == null)
            {
                _colorRedTopLeft = _colorRedBottomLeft = _colorRedBottomRight = _colorRedTopRight = var5 * 0.6f * var9;
                _colorGreenTopLeft = _colorGreenBottomLeft = _colorGreenBottomRight = _colorGreenTopRight = var6 * 0.6f * var9;
                _colorBlueTopLeft = _colorBlueBottomLeft = _colorBlueBottomRight = _colorBlueTopRight = var7 * 0.6f * var9;
                renderNorthFace(var1, var2, var3, var4, "grass_block_side_overlay");
            }
            var8 = true;
        }

        // ── South (X+) ──
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2 + 1, var3, var4, 5))
        {
            Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
            float var27 = var1.getLuminance(_blockAccess, var2 + 1, var3, var4);
            if (blockBB.MaxX < 1.0D) var27 = _lightValueOwn;
            if (_aoBlendMode <= 0) { var9 = var10 = var11 = var12 = _aoLightValueXPos; }
            else
            {
                ++var2;
                float fZN = var1.getLuminance(_blockAccess, var2, var3, var4 - 1), fZP = var1.getLuminance(_blockAccess, var2, var3, var4 + 1);
                float fYN = var1.getLuminance(_blockAccess, var2, var3 - 1, var4), fYP = var1.getLuminance(_blockAccess, var2, var3 + 1, var4);
                float fZNYN = (!_aoBlockOpXPosZNeg && !_aoBlockOpYNegZNeg) ? fZN : var1.getLuminance(_blockAccess, var2, var3 - 1, var4 - 1);
                float fZNYP = (!_aoBlockOpXPosZNeg && !_aoBlockOpXNegYNeg) ? fZN : var1.getLuminance(_blockAccess, var2, var3 + 1, var4 - 1);
                float fZPYN = (!_aoBlockOpXPosZPos && !_aoBlockOpYNegZPos) ? fZP : var1.getLuminance(_blockAccess, var2, var3 - 1, var4 + 1);
                float fZPYP = (!_aoBlockOpXPosZPos && !_aoBlockOpXNegYPos) ? fZP : var1.getLuminance(_blockAccess, var2, var3 + 1, var4 + 1);
                --var2;
                var9 = (fYP + _aoLightValueXPos + fZPYP + fZP) / 4f;
                var12 = (fZNYP + fZN + fYP + _aoLightValueXPos) / 4f;
                var11 = (fZN + fZNYN + _aoLightValueXPos + fYN) / 4f;
                var10 = (_aoLightValueXPos + fYN + fZP + fZPYN) / 4f;
            }
            _colorRedTopLeft = _colorRedBottomLeft = _colorRedBottomRight = _colorRedTopRight = (var18 ? var5 : 1f) * 0.6f;
            _colorGreenTopLeft = _colorGreenBottomLeft = _colorGreenBottomRight = _colorGreenTopRight = (var18 ? var6 : 1f) * 0.6f;
            _colorBlueTopLeft = _colorBlueBottomLeft = _colorBlueBottomRight = _colorBlueTopRight = (var18 ? var7 : 1f) * 0.6f;
            _colorRedTopLeft *= var9; _colorGreenTopLeft *= var9; _colorBlueTopLeft *= var9;
            _colorRedBottomLeft *= var10; _colorGreenBottomLeft *= var10; _colorBlueBottomLeft *= var10;
            _colorRedBottomRight *= var11; _colorGreenBottomRight *= var11; _colorBlueBottomRight *= var11;
            _colorRedTopRight *= var12; _colorGreenTopRight *= var12; _colorBlueTopRight *= var12;
            string var28 = var1.getTextureId(_blockAccess, var2, var3, var4, "east");
            renderSouthFace(var1, var2, var3, var4, var28);
            if (s_fancyGrass && var28 == "grass_block_side" && _overrideBlockTexture == null)
            {
                _colorRedTopLeft = _colorRedBottomLeft = _colorRedBottomRight = _colorRedTopRight = var5 * 0.6f * var9;
                _colorGreenTopLeft = _colorGreenBottomLeft = _colorGreenBottomRight = _colorGreenTopRight = var6 * 0.6f * var9;
                _colorBlueTopLeft = _colorBlueBottomLeft = _colorBlueBottomRight = _colorBlueTopRight = var7 * 0.6f * var9;
                renderSouthFace(var1, var2, var3, var4, "grass_block_side_overlay");
            }
            var8 = true;
        }

        _enableAO = false;
        return var8;
    }

    public bool renderStandardBlockWithColorMultiplier(Block var1, int var2, int var3, int var4, float var5, float var6, float var7)
    {
        Tessellator var8 = getTessellator();
        Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
        bool var9 = false;
        float var10 = 0.5F, var11 = 1.0F, var12 = 0.8F, var13 = 0.6F;
        float var14 = var10 * var5, var15 = var11 * var5, var16 = var12 * var5, var17 = var13 * var5;
        float var18 = var10 * var6, var19 = var11 * var6, var20 = var12 * var6, var21 = var13 * var6;
        float var22 = var10 * var7, var23 = var11 * var7, var24 = var12 * var7, var25 = var13 * var7;
        float var26 = var1.getLuminance(_blockAccess, var2, var3, var4);
        float var27;

        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3 - 1, var4, 0))
        {
            var27 = var1.getLuminance(_blockAccess, var2, var3 - 1, var4);
            var8.setColorOpaque_F(var14 * var27, var18 * var27, var22 * var27);
            renderBottomFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "bottom"));
            var9 = true;
        }
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3 + 1, var4, 1))
        {
            var27 = var1.getLuminance(_blockAccess, var2, var3 + 1, var4);
            if (blockBB.MaxY != 1.0D && !var1.material.IsFluid) var27 = var26;
            var8.setColorOpaque_F(var15 * var27, var19 * var27, var23 * var27);
            renderTopFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "top"));
            var9 = true;
        }
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3, var4 - 1, 2))
        {
            var27 = var1.getLuminance(_blockAccess, var2, var3, var4 - 1);
            if (blockBB.MinZ > 0.0D) var27 = var26;
            var8.setColorOpaque_F(var16 * var27, var20 * var27, var24 * var27);
            renderEastFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "north"));
            var9 = true;
        }
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3, var4 + 1, 3))
        {
            var27 = var1.getLuminance(_blockAccess, var2, var3, var4 + 1);
            if (blockBB.MaxZ < 1.0D) var27 = var26;
            var8.setColorOpaque_F(var16 * var27, var20 * var27, var24 * var27);
            renderWestFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "south"));
            var9 = true;
        }
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2 - 1, var3, var4, 4))
        {
            var27 = var1.getLuminance(_blockAccess, var2 - 1, var3, var4);
            if (blockBB.MinX > 0.0D) var27 = var26;
            var8.setColorOpaque_F(var17 * var27, var21 * var27, var25 * var27);
            string var28 = var1.getTextureId(_blockAccess, var2, var3, var4, "west");
            renderNorthFace(var1, var2, var3, var4, var28);
            if (s_fancyGrass && var28 == "grass_block_side" && _overrideBlockTexture == null)
            {
                var8.setColorOpaque_F(var17 * var27 * var5, var21 * var27 * var6, var25 * var27 * var7);
                renderNorthFace(var1, var2, var3, var4, "grass_block_side_overlay");
            }
            var9 = true;
        }
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2 + 1, var3, var4, 5))
        {
            var27 = var1.getLuminance(_blockAccess, var2 + 1, var3, var4);
            if (blockBB.MaxX < 1.0D) var27 = var26;
            var8.setColorOpaque_F(var17 * var27, var21 * var27, var25 * var27);
            string var28 = var1.getTextureId(_blockAccess, var2, var3, var4, "east");
            renderSouthFace(var1, var2, var3, var4, var28);
            if (s_fancyGrass && var28 == "grass_block_side" && _overrideBlockTexture == null)
            {
                var8.setColorOpaque_F(var17 * var27 * var5, var21 * var27 * var6, var25 * var27 * var7);
                renderSouthFace(var1, var2, var3, var4, "grass_block_side_overlay");
            }
            var9 = true;
        }
        return var9;
    }

    // ── Cactus ───────────────────────────────────────────────────────────────

    public bool renderBlockCactus(Block var1, int var2, int var3, int var4)
    {
        int var5 = var1.getColorMultiplier(_blockAccess, var2, var3, var4);
        return func_1230_b(var1, var2, var3, var4, (var5 >> 16 & 255) / 255f, (var5 >> 8 & 255) / 255f, (var5 & 255) / 255f);
    }

    public bool func_1230_b(Block var1, int var2, int var3, int var4, float var5, float var6, float var7)
    {
        Tessellator var8 = getTessellator();
        Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
        bool var9 = false;
        float var10 = 0.5F, var11 = 1.0F, var12 = 0.8F, var13 = 0.6F;
        float var26 = 1f / 16f, var27 = var1.getLuminance(_blockAccess, var2, var3, var4), var28;
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3 - 1, var4, 0))
        {
            var28 = var1.getLuminance(_blockAccess, var2, var3 - 1, var4);
            var8.setColorOpaque_F(var10 * var5 * var28, var10 * var6 * var28, var10 * var7 * var28);
            renderBottomFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "bottom")); var9 = true;
        }
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3 + 1, var4, 1))
        {
            var28 = var1.getLuminance(_blockAccess, var2, var3 + 1, var4);
            if (blockBB.MaxY != 1.0D && !var1.material.IsFluid) var28 = var27;
            var8.setColorOpaque_F(var11 * var5 * var28, var11 * var6 * var28, var11 * var7 * var28);
            renderTopFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "top")); var9 = true;
        }
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3, var4 - 1, 2))
        {
            var28 = var1.getLuminance(_blockAccess, var2, var3, var4 - 1);
            if (blockBB.MinZ > 0.0D) var28 = var27;
            var8.setColorOpaque_F(var12 * var5 * var28, var12 * var6 * var28, var12 * var7 * var28);
            var8.setTranslationF(0f, 0f, var26);
            renderEastFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "north"));
            var8.setTranslationF(0f, 0f, -var26); var9 = true;
        }
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2, var3, var4 + 1, 3))
        {
            var28 = var1.getLuminance(_blockAccess, var2, var3, var4 + 1);
            if (blockBB.MaxZ < 1.0D) var28 = var27;
            var8.setColorOpaque_F(var12 * var5 * var28, var12 * var6 * var28, var12 * var7 * var28);
            var8.setTranslationF(0f, 0f, -var26);
            renderWestFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "south"));
            var8.setTranslationF(0f, 0f, var26); var9 = true;
        }
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2 - 1, var3, var4, 4))
        {
            var28 = var1.getLuminance(_blockAccess, var2 - 1, var3, var4);
            if (blockBB.MinX > 0.0D) var28 = var27;
            var8.setColorOpaque_F(var13 * var5 * var28, var13 * var6 * var28, var13 * var7 * var28);
            var8.setTranslationF(var26, 0f, 0f);
            renderNorthFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "west"));
            var8.setTranslationF(-var26, 0f, 0f); var9 = true;
        }
        if (_renderAllFaces || var1.isSideVisible(_blockAccess, var2 + 1, var3, var4, 5))
        {
            var28 = var1.getLuminance(_blockAccess, var2 + 1, var3, var4);
            if (blockBB.MaxX < 1.0D) var28 = var27;
            var8.setColorOpaque_F(var13 * var5 * var28, var13 * var6 * var28, var13 * var7 * var28);
            var8.setTranslationF(-var26, 0f, 0f);
            renderSouthFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "east"));
            var8.setTranslationF(var26, 0f, 0f); var9 = true;
        }
        return var9;
    }

    // ── Fence ────────────────────────────────────────────────────────────────

    public bool renderBlockFence(Block var1, int var2, int var3, int var4)
    {
        bool var5 = false; float var6 = 6f / 16f, var7 = 10f / 16f;
        setOverrideBoundingBox(var6, 0f, var6, var7, 1f, var7); renderStandardBlock(var1, var2, var3, var4); var5 = true;
        bool var8 = false, var9 = false;
        if (_blockAccess.getBlockId(var2 - 1, var3, var4) == var1.id || _blockAccess.getBlockId(var2 + 1, var3, var4) == var1.id) var8 = true;
        if (_blockAccess.getBlockId(var2, var3, var4 - 1) == var1.id || _blockAccess.getBlockId(var2, var3, var4 + 1) == var1.id) var9 = true;
        bool var10 = _blockAccess.getBlockId(var2 - 1, var3, var4) == var1.id;
        bool var11 = _blockAccess.getBlockId(var2 + 1, var3, var4) == var1.id;
        bool var12 = _blockAccess.getBlockId(var2, var3, var4 - 1) == var1.id;
        bool var13 = _blockAccess.getBlockId(var2, var3, var4 + 1) == var1.id;
        if (!var8 && !var9) var8 = true;
        var6 = 7f / 16f; var7 = 9f / 16f; float var14 = 12f / 16f, var15 = 15f / 16f;
        float var16 = var10 ? 0f : var6, var17 = var11 ? 1f : var7, var18 = var12 ? 0f : var6, var19 = var13 ? 1f : var7;
        if (var8) { setOverrideBoundingBox(var16, var14, var6, var17, var15, var7); renderStandardBlock(var1, var2, var3, var4); var5 = true; }
        if (var9) { setOverrideBoundingBox(var6, var14, var18, var7, var15, var19); renderStandardBlock(var1, var2, var3, var4); var5 = true; }
        var14 = 6f / 16f; var15 = 9f / 16f;
        if (var8) { setOverrideBoundingBox(var16, var14, var6, var17, var15, var7); renderStandardBlock(var1, var2, var3, var4); var5 = true; }
        if (var9) { setOverrideBoundingBox(var6, var14, var18, var7, var15, var19); renderStandardBlock(var1, var2, var3, var4); var5 = true; }
        setOverrideBoundingBox(0f, 0f, 0f, 1f, 1f, 1f);
        return var5;
    }

    // ── Stairs ───────────────────────────────────────────────────────────────

    public bool renderBlockStairs(Block var1, int var2, int var3, int var4)
    {
        bool var5 = false; int var6 = _blockAccess.getBlockMeta(var2, var3, var4);
        if (var6 == 0) { setOverrideBoundingBox(0f, 0f, 0f, 0.5f, 0.5f, 1f); renderStandardBlock(var1, var2, var3, var4); setOverrideBoundingBox(0.5f, 0f, 0f, 1f, 1f, 1f); renderStandardBlock(var1, var2, var3, var4); var5 = true; }
        else if (var6 == 1) { setOverrideBoundingBox(0f, 0f, 0f, 0.5f, 1f, 1f); renderStandardBlock(var1, var2, var3, var4); setOverrideBoundingBox(0.5f, 0f, 0f, 1f, 0.5f, 1f); renderStandardBlock(var1, var2, var3, var4); var5 = true; }
        else if (var6 == 2) { setOverrideBoundingBox(0f, 0f, 0f, 1f, 0.5f, 0.5f); renderStandardBlock(var1, var2, var3, var4); setOverrideBoundingBox(0f, 0f, 0.5f, 1f, 1f, 1f); renderStandardBlock(var1, var2, var3, var4); var5 = true; }
        else if (var6 == 3) { setOverrideBoundingBox(0f, 0f, 0f, 1f, 1f, 0.5f); renderStandardBlock(var1, var2, var3, var4); setOverrideBoundingBox(0f, 0f, 0.5f, 1f, 0.5f, 1f); renderStandardBlock(var1, var2, var3, var4); var5 = true; }
        setOverrideBoundingBox(0f, 0f, 0f, 1f, 1f, 1f);
        return var5;
    }

    // ── Door ─────────────────────────────────────────────────────────────────

    public bool renderBlockDoor(Block var1, int var2, int var3, int var4)
    {
        Tessellator var5 = getTessellator();
        BlockDoor var6 = (BlockDoor)var1;
        Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
        bool var7 = false;
        float var8 = 0.5F, var9 = 1.0F, var10 = 0.8F, var11 = 0.6F;
        float var12 = var1.getLuminance(_blockAccess, var2, var3, var4);
        float var13 = var1.getLuminance(_blockAccess, var2, var3 - 1, var4);
        if (blockBB.MinY > 0.0D) var13 = var12;
        if (Block.BlocksLightLuminance[var1.id] > 0) var13 = 1.0F;
        var5.setColorOpaque_F(var8 * var13, var8 * var13, var8 * var13);
        renderBottomFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "bottom")); var7 = true;
        var13 = var1.getLuminance(_blockAccess, var2, var3 + 1, var4);
        if (blockBB.MaxY < 1.0D) var13 = var12;
        if (Block.BlocksLightLuminance[var1.id] > 0) var13 = 1.0F;
        var5.setColorOpaque_F(var9 * var13, var9 * var13, var9 * var13);
        renderTopFace(var1, var2, var3, var4, var1.getTextureId(_blockAccess, var2, var3, var4, "top")); var7 = true;
        var13 = var1.getLuminance(_blockAccess, var2, var3, var4 - 1);
        if (blockBB.MinZ > 0.0D) var13 = var12;
        if (Block.BlocksLightLuminance[var1.id] > 0) var13 = 1.0F;
        var5.setColorOpaque_F(var10 * var13, var10 * var13, var10 * var13);
        string var14 = var1.getTextureId(_blockAccess, var2, var3, var4, "north");
        if (var14.StartsWith("-")) { _flipTexture = true; var14 = var14.TrimStart('-'); }
        renderEastFace(var1, var2, var3, var4, var14); var7 = true; _flipTexture = false;
        var13 = var1.getLuminance(_blockAccess, var2, var3, var4 + 1);
        if (blockBB.MaxZ < 1.0D) var13 = var12;
        if (Block.BlocksLightLuminance[var1.id] > 0) var13 = 1.0F;
        var5.setColorOpaque_F(var10 * var13, var10 * var13, var10 * var13);
        var14 = var1.getTextureId(_blockAccess, var2, var3, var4, "south");
        if (var14.StartsWith("-")) { _flipTexture = true; var14 = var14.TrimStart('-'); }
        renderWestFace(var1, var2, var3, var4, var14); var7 = true; _flipTexture = false;
        var13 = var1.getLuminance(_blockAccess, var2 - 1, var3, var4);
        if (blockBB.MinX > 0.0D) var13 = var12;
        if (Block.BlocksLightLuminance[var1.id] > 0) var13 = 1.0F;
        var5.setColorOpaque_F(var11 * var13, var11 * var13, var11 * var13);
        var14 = var1.getTextureId(_blockAccess, var2, var3, var4, "west");
        if (var14.StartsWith("-")) { _flipTexture = true; var14 = var14.TrimStart('-'); }
        renderNorthFace(var1, var2, var3, var4, var14); var7 = true; _flipTexture = false;
        var13 = var1.getLuminance(_blockAccess, var2 + 1, var3, var4);
        if (blockBB.MaxX < 1.0D) var13 = var12;
        if (Block.BlocksLightLuminance[var1.id] > 0) var13 = 1.0F;
        var5.setColorOpaque_F(var11 * var13, var11 * var13, var11 * var13);
        var14 = var1.getTextureId(_blockAccess, var2, var3, var4, "east");
        if (var14.StartsWith("-")) { _flipTexture = true; var14 = var14.TrimStart('-'); }
        renderSouthFace(var1, var2, var3, var4, var14); var7 = true; _flipTexture = false;
        return var7;
    }

    // ── render*Face — UV entièrement atlas, rotations en espace normalisé ────

    public void renderBottomFace(Block var1, double var2, double var4, double var6, string var8)
    {
        Tessellator var9 = getTessellator();
        Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
        
        if (_overrideBlockTexture != null) var8 = _overrideBlockTexture;

        var (var12, var16, var14, var18, var20, var24, var22, var26) =
            GetUVForFace(var8, blockBB, UVAxis.X, UVAxis.Z, _uvRotateBottom);

        double var28 = var2 + blockBB.MinX, var30 = var2 + blockBB.MaxX;
        double var32 = var4 + blockBB.MinY, var34 = var6 + blockBB.MinZ, var36 = var6 + blockBB.MaxZ;
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

    public void renderTopFace(Block var1, double var2, double var4, double var6, string var8)
    {
        Tessellator var9 = getTessellator();
        Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
        if (_overrideBlockTexture != null) var8 = _overrideBlockTexture;

        var (var12, var16, var14, var18, var20, var24, var22, var26) =
            GetUVForFace(var8, blockBB, UVAxis.X, UVAxis.Z, _uvRotateTop);

        double var28 = var2 + blockBB.MinX, var30 = var2 + blockBB.MaxX;
        double var32 = var4 + blockBB.MaxY, var34 = var6 + blockBB.MinZ, var36 = var6 + blockBB.MaxZ;
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

    public void renderEastFace(Block var1, double var2, double var4, double var6, string var8)
    {
        Tessellator var9 = getTessellator();
        Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
        if (_overrideBlockTexture != null) var8 = _overrideBlockTexture;

        var (var12, var16, var14, var18, var20, var24, var22, var26) =
            GetUVForFace(var8, blockBB, UVAxis.X, UVAxis.Y, _uvRotateEast);
        if (_flipTexture) { double t = var12; var12 = var14; var14 = t; t = var20; var20 = var22; var22 = t; }

        double var28 = var2 + blockBB.MinX, var30 = var2 + blockBB.MaxX;
        double var32 = var4 + blockBB.MinY, var34 = var4 + blockBB.MaxY, var36 = var6 + blockBB.MinZ;
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

    public void renderWestFace(Block var1, double var2, double var4, double var6, string var8)
    {
        Tessellator var9 = getTessellator();
        Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
        if (_overrideBlockTexture != null) var8 = _overrideBlockTexture;

        var (var12, var16, var14, var18, var20, var24, var22, var26) =
            GetUVForFace(var8, blockBB, UVAxis.X, UVAxis.Y, _uvRotateWest);
        if (_flipTexture) { double t = var12; var12 = var14; var14 = t; t = var20; var20 = var22; var22 = t; }

        double var28 = var2 + blockBB.MinX, var30 = var2 + blockBB.MaxX;
        double var32 = var4 + blockBB.MinY, var34 = var4 + blockBB.MaxY, var36 = var6 + blockBB.MaxZ;
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

    public void renderNorthFace(Block var1, double var2, double var4, double var6, string var8)
    {
        Tessellator var9 = getTessellator();
        Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
        if (_overrideBlockTexture != null) var8 = _overrideBlockTexture;

        var (var12, var16, var14, var18, var20, var24, var22, var26) =
            GetUVForFace(var8, blockBB, UVAxis.Z, UVAxis.Y, _uvRotateNorth);
        if (_flipTexture) { double t = var12; var12 = var14; var14 = t; t = var20; var20 = var22; var22 = t; }

        double var28 = var2 + blockBB.MinX;
        double var30 = var4 + blockBB.MinY, var32 = var4 + blockBB.MaxY;
        double var34 = var6 + blockBB.MinZ, var36 = var6 + blockBB.MaxZ;
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

    public void renderSouthFace(Block var1, double var2, double var4, double var6, string var8)
    {
        Tessellator var9 = getTessellator();
        Box blockBB = _useOverrideBoundingBox ? _overrideBoundingBox : var1.BoundingBox;
        if (_overrideBlockTexture != null) var8 = _overrideBlockTexture;

        var (var12, var16, var14, var18, var20, var24, var22, var26) =
            GetUVForFace(var8, blockBB, UVAxis.Z, UVAxis.Y, _uvRotateSouth);
        if (_flipTexture) { double t = var12; var12 = var14; var14 = t; t = var20; var20 = var22; var22 = t; }

        double var28 = var2 + blockBB.MaxX;
        double var30 = var4 + blockBB.MinY, var32 = var4 + blockBB.MaxY;
        double var34 = var6 + blockBB.MinZ, var36 = var6 + blockBB.MaxZ;
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

    // ── Torch ────────────────────────────────────────────────────────────────

    public void renderTorchAtAngle(Block var1, double var2, double var4, double var6, double var8, double var10)
    {
        Tessellator var12 = getTessellator();
        string texName = _overrideBlockTexture ?? var1.getTexture("down");
        UVRegion uv = ResolveUV(texName);

        // Sous-régions pixel dans la tuile 16x16 :
        // flambeau (1.75/16 à 9/16 en U, 6/16 à 8/16 en V)
        // côtés   (0 à 1 en U, 0 à 1 en V)
        double du = uv.U1 - uv.U0, dv = uv.V1 - uv.V0;
        double var20 = uv.U0 + du * (1.75 / 16.0);
        double var22 = uv.V0 + dv * (6.0 / 16.0);
        double var24 = uv.U0 + du * (9.0 / 16.0);
        double var26 = uv.V0 + dv * (8.0 / 16.0);
        double u0 = uv.U0, u1 = uv.U1, v0 = uv.V0, v1 = uv.V1;

        var2 += 0.5D; var6 += 0.5D;
        double var28 = var2 - 0.5D, var30 = var2 + 0.5D, var32 = var6 - 0.5D, var34 = var6 + 0.5D;
        double var36 = 1.0D / 16.0D, var38 = 0.625D;

        // Dessus du flambeau
        var12.addVertexWithUV(var2 + var8 * (1 - var38) - var36, var4 + var38, var6 + var10 * (1 - var38) - var36, var20, var22);
        var12.addVertexWithUV(var2 + var8 * (1 - var38) - var36, var4 + var38, var6 + var10 * (1 - var38) + var36, var20, var26);
        var12.addVertexWithUV(var2 + var8 * (1 - var38) + var36, var4 + var38, var6 + var10 * (1 - var38) + var36, var24, var26);
        var12.addVertexWithUV(var2 + var8 * (1 - var38) + var36, var4 + var38, var6 + var10 * (1 - var38) - var36, var24, var22);
        // 4 côtés
        var12.addVertexWithUV(var2 - var36, var4 + 1.0D, var32, u0, v0);
        var12.addVertexWithUV(var2 - var36 + var8, var4 + 0.0D, var32 + var10, u0, v1);
        var12.addVertexWithUV(var2 - var36 + var8, var4 + 0.0D, var34 + var10, u1, v1);
        var12.addVertexWithUV(var2 - var36, var4 + 1.0D, var34, u1, v0);
        var12.addVertexWithUV(var2 + var36, var4 + 1.0D, var34, u0, v0);
        var12.addVertexWithUV(var2 + var8 + var36, var4 + 0.0D, var34 + var10, u0, v1);
        var12.addVertexWithUV(var2 + var8 + var36, var4 + 0.0D, var32 + var10, u1, v1);
        var12.addVertexWithUV(var2 + var36, var4 + 1.0D, var32, u1, v0);
        var12.addVertexWithUV(var28, var4 + 1.0D, var6 + var36, u0, v0);
        var12.addVertexWithUV(var28 + var8, var4 + 0.0D, var6 + var36 + var10, u0, v1);
        var12.addVertexWithUV(var30 + var8, var4 + 0.0D, var6 + var36 + var10, u1, v1);
        var12.addVertexWithUV(var30, var4 + 1.0D, var6 + var36, u1, v0);
        var12.addVertexWithUV(var30, var4 + 1.0D, var6 - var36, u0, v0);
        var12.addVertexWithUV(var30 + var8, var4 + 0.0D, var6 - var36 + var10, u0, v1);
        var12.addVertexWithUV(var28 + var8, var4 + 0.0D, var6 - var36 + var10, u1, v1);
        var12.addVertexWithUV(var28, var4 + 1.0D, var6 - var36, u1, v0);
    }

    // ── renderBlockOnInventory ────────────────────────────────────────────────

    public void renderBlockOnInventory(Block var1, int var2, float var3)
    {
        Tessellator var4 = getTessellator();
        int var5; float var6, var7;
        if (renderFromInside)
        {
            var5 = var1.getColor(var2);
            var6 = (var5 >> 16 & 255) / 255.0F; var7 = (var5 >> 8 & 255) / 255.0F;
            float var8 = (var5 & 255) / 255.0F;
            GLManager.GL.Color4(var6 * var3, var7 * var3, var8 * var3, 1.0F);
        }

        var5 = var1.getRenderType();
        if (var5 != 0 && var5 != 16)
        {
            if (var5 == 1)
            {
                var4.startDrawingQuads(); var4.setNormal(0f, -1f, 0f);
                renderCrossedSquares(var1, var2, -0.5D, -0.5D, -0.5D);
                var4.draw();
            }
            else if (var5 == 13)
            {
                var1.setupRenderBoundingBox();
                GLManager.GL.Translate(-0.5F, -0.5F, -0.5F);
                var6 = 1f / 16f;
                var4.startDrawingQuads(); var4.setNormal(0f, -1f, 0f); renderBottomFace(var1, 0d, 0d, 0d, var1.getTexture("down")); var4.draw();
                var4.startDrawingQuads(); var4.setNormal(0f, 1f, 0f); renderTopFace(var1, 0d, 0d, 0d, var1.getTexture("up")); var4.draw();
                var4.startDrawingQuads(); var4.setNormal(0f, 0f, -1f); var4.setTranslationF(0f, 0f, var6); renderEastFace(var1, 0d, 0d, 0d, var1.getTexture("north")); var4.setTranslationF(0f, 0f, -var6); var4.draw();
                var4.startDrawingQuads(); var4.setNormal(0f, 0f, 1f); var4.setTranslationF(0f, 0f, -var6); renderWestFace(var1, 0d, 0d, 0d, var1.getTexture("south")); var4.setTranslationF(0f, 0f, var6); var4.draw();
                var4.startDrawingQuads(); var4.setNormal(-1f, 0f, 0f); var4.setTranslationF(var6, 0f, 0f); renderNorthFace(var1, 0d, 0d, 0d, var1.getTexture("west")); var4.setTranslationF(-var6, 0f, 0f); var4.draw();
                var4.startDrawingQuads(); var4.setNormal(1f, 0f, 0f); var4.setTranslationF(-var6, 0f, 0f); renderSouthFace(var1, 0d, 0d, 0d, var1.getTexture("east")); var4.setTranslationF(var6, 0f, 0f); var4.draw();
                GLManager.GL.Translate(0.5F, 0.5F, 0.5F);
            }
            else if (var5 == 10)
            {
                for (int var9 = 0; var9 < 2; ++var9)
                {
                    if (var9 == 0) var1.setBoundingBox(0f, 0f, 0f, 1f, 1f, 0.5f);
                    if (var9 == 1) var1.setBoundingBox(0f, 0f, 0.5f, 1f, 0.5f, 1f);
                    GLManager.GL.Translate(-0.5F, -0.5F, -0.5F);
                    var4.startDrawingQuads(); var4.setNormal(0f, -1f, 0f); renderBottomFace(var1, 0d, 0d, 0d, var1.getTexture("down")); var4.draw();
                    var4.startDrawingQuads(); var4.setNormal(0f, 1f, 0f); renderTopFace(var1, 0d, 0d, 0d, var1.getTexture("up")); var4.draw();
                    var4.startDrawingQuads(); var4.setNormal(0f, 0f, -1f); renderEastFace(var1, 0d, 0d, 0d, var1.getTexture("north")); var4.draw();
                    var4.startDrawingQuads(); var4.setNormal(0f, 0f, 1f); renderWestFace(var1, 0d, 0d, 0d, var1.getTexture("south")); var4.draw();
                    var4.startDrawingQuads(); var4.setNormal(-1f, 0f, 0f); renderNorthFace(var1, 0d, 0d, 0d, var1.getTexture("west")); var4.draw();
                    var4.startDrawingQuads(); var4.setNormal(1f, 0f, 0f); renderSouthFace(var1, 0d, 0d, 0d, var1.getTexture("east")); var4.draw();
                    GLManager.GL.Translate(0.5F, 0.5F, 0.5F);
                }
            }
            else if (var5 == 11)
            {
                for (int var9 = 0; var9 < 4; ++var9)
                {
                    float vv = 2f / 16f;
                    if (var9 == 0) var1.setBoundingBox(0.5f - vv, 0f, 0f, 0.5f + vv, 1f, vv * 2f);
                    if (var9 == 1) var1.setBoundingBox(0.5f - vv, 0f, 1f - vv * 2f, 0.5f + vv, 1f, 1f);
                    vv = 1f / 16f;
                    if (var9 == 2) var1.setBoundingBox(0.5f - vv, 1f - vv * 3f, -vv * 2f, 0.5f + vv, 1f - vv, 1f + vv * 2f);
                    if (var9 == 3) var1.setBoundingBox(0.5f - vv, 0.5f - vv * 3f, -vv * 2f, 0.5f + vv, 0.5f - vv, 1f + vv * 2f);
                    GLManager.GL.Translate(-0.5F, -0.5F, -0.5F);
                    var4.startDrawingQuads(); var4.setNormal(0f, -1f, 0f); renderBottomFace(var1, 0d, 0d, 0d, var1.getTexture("down")); var4.draw();
                    var4.startDrawingQuads(); var4.setNormal(0f, 1f, 0f); renderTopFace(var1, 0d, 0d, 0d, var1.getTexture("up")); var4.draw();
                    var4.startDrawingQuads(); var4.setNormal(0f, 0f, -1f); renderEastFace(var1, 0d, 0d, 0d, var1.getTexture("north")); var4.draw();
                    var4.startDrawingQuads(); var4.setNormal(0f, 0f, 1f); renderWestFace(var1, 0d, 0d, 0d, var1.getTexture("south")); var4.draw();
                    var4.startDrawingQuads(); var4.setNormal(-1f, 0f, 0f); renderNorthFace(var1, 0d, 0d, 0d, var1.getTexture("west")); var4.draw();
                    var4.startDrawingQuads(); var4.setNormal(1f, 0f, 0f); renderSouthFace(var1, 0d, 0d, 0d, var1.getTexture("east")); var4.draw();
                    GLManager.GL.Translate(0.5F, 0.5F, 0.5F);
                }
                var1.setBoundingBox(0f, 0f, 0f, 1f, 1f, 1f);
            }
        }
        else
        {
            if (var5 == 16) var2 = 1;
            var1.setupRenderBoundingBox();
            GLManager.GL.Translate(-0.5F, -0.5F, -0.5F);
            var4.startDrawingQuads(); var4.setNormal(0f, -1f, 0f); renderBottomFace(var1, 0d, 0d, 0d, var1.getTexture("down", var2)); var4.draw();
            var4.startDrawingQuads(); var4.setNormal(0f, 1f, 0f); renderTopFace(var1, 0d, 0d, 0d, var1.getTexture("up", var2)); var4.draw();
            var4.startDrawingQuads(); var4.setNormal(0f, 0f, -1f); renderEastFace(var1, 0d, 0d, 0d, var1.getTexture("north", var2)); var4.draw();
            var4.startDrawingQuads(); var4.setNormal(0f, 0f, 1f); renderWestFace(var1, 0d, 0d, 0d, var1.getTexture("south", var2)); var4.draw();
            var4.startDrawingQuads(); var4.setNormal(-1f, 0f, 0f); renderNorthFace(var1, 0d, 0d, 0d, var1.getTexture("west", var2)); var4.draw();
            var4.startDrawingQuads(); var4.setNormal(1f, 0f, 0f); renderSouthFace(var1, 0d, 0d, 0d, var1.getTexture("east", var2)); var4.draw();
            GLManager.GL.Translate(0.5F, 0.5F, 0.5F);
        }
    }

    // ── Utilitaires statiques ─────────────────────────────────────────────────

    public static bool isSideLit(int var0) =>
        var0 == 0 || var0 == 13 || var0 == 10 || var0 == 11 || var0 == 16;

    public static void rotateAroundX(ref Vector3D<double> vec, float var1)
    {
        float var2 = MathHelper.Cos(var1), var3 = MathHelper.Sin(var1);
        double var4 = vec.X;
        double var6 = vec.Y * (double)var2 + vec.Z * (double)var3;
        double var8 = vec.Z * (double)var2 - vec.Y * (double)var3;
        vec.X = var4; vec.Y = var6; vec.Z = var8;
    }

    private static void rotateAroundY(ref Vector3D<double> vec, float var1)
    {
        float var2 = MathHelper.Cos(var1), var3 = MathHelper.Sin(var1);
        double var4 = vec.X * (double)var2 + vec.Z * (double)var3;
        double var6 = vec.Y;
        double var8 = vec.Z * (double)var2 - vec.X * (double)var3;
        vec.X = var4; vec.Y = var6; vec.Z = var8;
    }
}
