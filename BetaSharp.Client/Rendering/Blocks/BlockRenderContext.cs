using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Blocks.Renderers;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks;

public ref struct BlockRenderContext
{
    public IBlockAccess World;
    public Tessellator Tess;

    public int OverrideTexture;
    public bool RenderAllFaces;
    public bool FlipTexture;
    public Box? OverrideBounds;
    public bool EnableAo = true;
    public int AoBlendMode = 0;

    // UV Rotations
    public int UvRotateTop;
    public int UvRotateBottom;
    public int UvRotateNorth;
    public int UvRotateSouth;
    public int UvRotateEast;
    public int UvRotateWest;

    // Custom flag for Pistons (Expanded/Short arm)
    public bool CustomFlag;

    public BlockRenderContext(
        IBlockAccess world,
        Tessellator tess,
        int overrideTexture = -1,
        bool renderAllFaces = false,
        bool flipTexture = false,
        Box? bounds = null,
        int uvTop = 0, int uvBottom = 0,
        int uvNorth = 0, int uvSouth = 0,
        int uvEast = 0, int uvWest = 0,
        bool customFlag = false,
        bool enableAo = true,
        int aoBlendMode = 0)
    {
        World = world;
        Tess = tess;

        OverrideTexture = overrideTexture;
        RenderAllFaces = renderAllFaces;
        FlipTexture = flipTexture;
        OverrideBounds = bounds;

        UvRotateTop = uvTop;
        UvRotateBottom = uvBottom;
        UvRotateNorth = uvNorth;
        UvRotateSouth = uvSouth;
        UvRotateEast = uvEast;
        UvRotateWest = uvWest;

        AoBlendMode = aoBlendMode;
        EnableAo = enableAo;

        CustomFlag = customFlag;
    }


    internal readonly void DrawBottomFace(Block block, in Vec3D pos, in FaceColors colors,
        int textureId)
    {
        Box blockBb = OverrideBounds ?? block.BoundingBox;
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = (texU + blockBb.MinX * 16.0D) / 256.0D;
        double maxU = (texU + blockBb.MaxX * 16.0D - 0.01D) / 256.0D;
        double minV = (texV + blockBb.MinZ * 16.0D) / 256.0D;
        double maxV = (texV + blockBb.MaxZ * 16.0D - 0.01D) / 256.0D;

        if (blockBb.MinX < 0.0D || blockBb.MaxX > 1.0D)
        {
            minU = texU / 256.0D;
            maxU = (texU + 15.99D) / 256.0D;
        }

        if (blockBb.MinZ < 0.0D || blockBb.MaxZ > 1.0D)
        {
            minV = texV / 256.0D;
            maxV = (texV + 15.99D) / 256.0D;
        }

        double u1 = maxU, u2 = minU, v1 = minV, v2 = maxV;

        if (UvRotateBottom == 2)
        {
            minU = (texU + blockBb.MinZ * 16.0D) / 256.0D;
            minV = (texV + 16 - blockBb.MaxX * 16.0D) / 256.0D;
            maxU = (texU + blockBb.MaxZ * 16.0D) / 256.0D;
            maxV = (texV + 16 - blockBb.MinX * 16.0D) / 256.0D;
            v1 = minV;
            v2 = maxV;
            u1 = minU;
            u2 = maxU;
            minV = maxV;
            maxV = v1;
        }
        else if (UvRotateBottom == 1)
        {
            minU = (texU + 16 - blockBb.MaxZ * 16.0D) / 256.0D;
            minV = (texV + blockBb.MinX * 16.0D) / 256.0D;
            maxU = (texU + 16 - blockBb.MinZ * 16.0D) / 256.0D;
            maxV = (texV + blockBb.MaxX * 16.0D) / 256.0D;
            u1 = maxU;
            u2 = minU;
            minU = maxU;
            maxU = u2;
            v1 = maxV;
            v2 = minV;
        }
        else if (UvRotateBottom == 3)
        {
            minU = (texU + 16 - blockBb.MinX * 16.0D) / 256.0D;
            maxU = (texU + 16 - blockBb.MaxX * 16.0D - 0.01D) / 256.0D;
            minV = (texV + 16 - blockBb.MinZ * 16.0D) / 256.0D;
            maxV = (texV + 16 - blockBb.MaxZ * 16.0D - 0.01D) / 256.0D;
            u1 = maxU;
            u2 = minU;
            v1 = minV;
            v2 = maxV;
        }

        double minX = pos.x + blockBb.MinX;
        double maxX = pos.x + blockBb.MaxX;
        double minY = pos.y + blockBb.MinY;
        double minZ = pos.z + blockBb.MinZ;
        double maxZ = pos.z + blockBb.MaxZ;

        if (EnableAo)
        {
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            Tess.addVertexWithUV(minX, minY, maxZ, u2, v2);
            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            Tess.addVertexWithUV(minX, minY, minZ, minU, minV);
            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            Tess.addVertexWithUV(maxX, minY, minZ, u1, v1);
            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            Tess.addVertexWithUV(maxX, minY, maxZ, maxU, maxV);
        }
        else
        {
            Tess.addVertexWithUV(minX, minY, maxZ, u2, v2);
            Tess.addVertexWithUV(minX, minY, minZ, minU, minV);
            Tess.addVertexWithUV(maxX, minY, minZ, u1, v1);
            Tess.addVertexWithUV(maxX, minY, maxZ, maxU, maxV);
        }
    }

    internal readonly void DrawTopFace(Block block, in Vec3D pos, in FaceColors colors,
        int textureId)
    {
        Box blockBb = OverrideBounds ?? block.BoundingBox;
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = (texU + blockBb.MinX * 16.0D) / 256.0D;
        double maxU = (texU + blockBb.MaxX * 16.0D - 0.01D) / 256.0D;
        double minV = (texV + blockBb.MinZ * 16.0D) / 256.0D;
        double maxV = (texV + blockBb.MaxZ * 16.0D - 0.01D) / 256.0D;

        if (blockBb.MinX < 0.0D || blockBb.MaxX > 1.0D)
        {
            minU = texU / 256.0D;
            maxU = (texU + 15.99D) / 256.0D;
        }

        if (blockBb.MinZ < 0.0D || blockBb.MaxZ > 1.0D)
        {
            minV = texV / 256.0D;
            maxV = (texV + 15.99D) / 256.0D;
        }

        double u1 = maxU, u2 = minU, v1 = minV, v2 = maxV;

        if (UvRotateTop == 1)
        {
            minU = (texU + blockBb.MinZ * 16.0D) / 256.0D;
            minV = (texV + 16 - blockBb.MaxX * 16.0D) / 256.0D;
            maxU = (texU + blockBb.MaxZ * 16.0D) / 256.0D;
            maxV = (texV + 16 - blockBb.MinX * 16.0D) / 256.0D;
            v1 = minV;
            v2 = maxV;
            u1 = minU;
            u2 = maxU;
            minV = maxV;
            maxV = v1;
        }
        else if (UvRotateTop == 2)
        {
            minU = (texU + 16 - blockBb.MaxZ * 16.0D) / 256.0D;
            minV = (texV + blockBb.MinX * 16.0D) / 256.0D;
            maxU = (texU + 16 - blockBb.MinZ * 16.0D) / 256.0D;
            maxV = (texV + blockBb.MaxX * 16.0D) / 256.0D;
            u1 = maxU;
            u2 = minU;
            minU = maxU;
            maxU = u2;
            v1 = maxV;
            v2 = minV;
        }
        else if (UvRotateTop == 3)
        {
            minU = (texU + 16 - blockBb.MinX * 16.0D) / 256.0D;
            maxU = (texU + 16 - blockBb.MaxX * 16.0D - 0.01D) / 256.0D;
            minV = (texV + 16 - blockBb.MinZ * 16.0D) / 256.0D;
            maxV = (texV + 16 - blockBb.MaxZ * 16.0D - 0.01D) / 256.0D;
            u1 = maxU;
            u2 = minU;
            v1 = minV;
            v2 = maxV;
        }

        double minX = pos.x + blockBb.MinX;
        double maxX = pos.x + blockBb.MaxX;
        double maxY = pos.y + blockBb.MaxY;
        double minZ = pos.z + blockBb.MinZ;
        double maxZ = pos.z + blockBb.MaxZ;

        if (EnableAo)
        {
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            Tess.addVertexWithUV(maxX, maxY, maxZ, maxU, maxV);
            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            Tess.addVertexWithUV(maxX, maxY, minZ, u1, v1);
            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            Tess.addVertexWithUV(minX, maxY, minZ, minU, minV);
            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            Tess.addVertexWithUV(minX, maxY, maxZ, u2, v2);
        }
        else
        {
            Tess.addVertexWithUV(maxX, maxY, maxZ, maxU, maxV);
            Tess.addVertexWithUV(maxX, maxY, minZ, u1, v1);
            Tess.addVertexWithUV(minX, maxY, minZ, minU, minV);
            Tess.addVertexWithUV(minX, maxY, maxZ, u2, v2);
        }
    }

    internal readonly void DrawEastFace(Block block, in Vec3D pos, in FaceColors colors,
        int textureId)
    {
        Box blockBb = OverrideBounds ?? block.BoundingBox;
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = (texU + blockBb.MinX * 16.0D) / 256.0D;
        double maxU = (texU + blockBb.MaxX * 16.0D - 0.01D) / 256.0D;
        double minV = (texV + 16 - blockBb.MaxY * 16.0D) / 256.0D;
        double maxV = (texV + 16 - blockBb.MinY * 16.0D - 0.01D) / 256.0D;

        if (FlipTexture)
        {
            (minU, maxU) = (maxU, minU);
        }

        if (blockBb.MinX < 0.0D || blockBb.MaxX > 1.0D)
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

        if (UvRotateEast == 2)
        {
            minU = (texU + blockBb.MinY * 16.0D) / 256.0D;
            minV = (texV + 16 - blockBb.MinX * 16.0D) / 256.0D;
            maxU = (texU + blockBb.MaxY * 16.0D) / 256.0D;
            maxV = (texV + 16 - blockBb.MaxX * 16.0D) / 256.0D;
            v1 = minV;
            v2 = maxV;
            u1 = minU;
            u2 = maxU;
            minV = maxV;
            maxV = v1;
        }
        else if (UvRotateEast == 1)
        {
            minU = (texU + 16 - blockBb.MaxY * 16.0D) / 256.0D;
            minV = (texV + blockBb.MaxX * 16.0D) / 256.0D;
            maxU = (texU + 16 - blockBb.MinY * 16.0D) / 256.0D;
            maxV = (texV + blockBb.MinX * 16.0D) / 256.0D;
            u1 = maxU;
            u2 = minU;
            minU = maxU;
            maxU = u2;
            v1 = maxV;
            v2 = minV;
        }
        else if (UvRotateEast == 3)
        {
            minU = (texU + 16 - blockBb.MinX * 16.0D) / 256.0D;
            maxU = (texU + 16 - blockBb.MaxX * 16.0D - 0.01D) / 256.0D;
            minV = (texV + blockBb.MaxY * 16.0D) / 256.0D;
            maxV = (texV + blockBb.MinY * 16.0D - 0.01D) / 256.0D;
            u1 = maxU;
            u2 = minU;
            v1 = minV;
            v2 = maxV;
        }

        double minX = pos.x + blockBb.MinX;
        double maxX = pos.x + blockBb.MaxX;
        double minY = pos.y + blockBb.MinY;
        double maxY = pos.y + blockBb.MaxY;
        double minZ = pos.z + blockBb.MinZ;

        if (EnableAo)
        {
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            Tess.addVertexWithUV(minX, maxY, minZ, u1, v1);
            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            Tess.addVertexWithUV(maxX, maxY, minZ, minU, minV);
            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            Tess.addVertexWithUV(maxX, minY, minZ, u2, v2);
            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            Tess.addVertexWithUV(minX, minY, minZ, maxU, maxV);
        }
        else
        {
            Tess.addVertexWithUV(minX, maxY, minZ, u1, v1);
            Tess.addVertexWithUV(maxX, maxY, minZ, minU, minV);
            Tess.addVertexWithUV(maxX, minY, minZ, u2, v2);
            Tess.addVertexWithUV(minX, minY, minZ, maxU, maxV);
        }
    }

    internal readonly void DrawWestFace(Block block, in Vec3D pos, in FaceColors colors,
        int textureId)
    {
        Box blockBb = OverrideBounds ?? block.BoundingBox;
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        double u12 = (texU + blockBb.MinX * 16.0D) / 256.0D;
        double u14 = (texU + blockBb.MaxX * 16.0D - 0.01D) / 256.0D;
        double v16 = (texV + 16 - blockBb.MaxY * 16.0D) / 256.0D;
        double v18 = (texV + 16 - blockBb.MinY * 16.0D - 0.01D) / 256.0D;

        if (FlipTexture)
        {
            (u12, u14) = (u14, u12);
        }

        if (blockBb.MinX < 0.0D || blockBb.MaxX > 1.0D)
        {
            u12 = texU / 256.0D;
            u14 = (texU + 15.99D) / 256.0D;
        }

        if (blockBb.MinY < 0.0D || blockBb.MaxY > 1.0D)
        {
            v16 = texV / 256.0D;
            v18 = (texV + 15.99D) / 256.0D;
        }

        double u20 = u14, u22 = u12, v24 = v16, v26 = v18;

        switch (UvRotateWest)
        {
            case 1:
                u12 = (texU + blockBb.MinY * 16.0D) / 256.0D;
                v18 = (texV + 16 - blockBb.MinX * 16.0D) / 256.0D;
                u14 = (texU + blockBb.MaxY * 16.0D) / 256.0D;
                v16 = (texV + 16 - blockBb.MaxX * 16.0D) / 256.0D;
                v24 = v16;
                v26 = v18;
                u20 = u12;
                u22 = u14;
                v16 = v18;
                v18 = v24;
                break;
            case 2:
                u12 = (texU + 16 - blockBb.MaxY * 16.0D) / 256.0D;
                v16 = (texV + blockBb.MinX * 16.0D) / 256.0D;
                u14 = (texU + 16 - blockBb.MinY * 16.0D) / 256.0D;
                v18 = (texV + blockBb.MaxX * 16.0D) / 256.0D;
                u20 = u14;
                u22 = u12;
                u12 = u14;
                u14 = u22;
                v24 = v18;
                v26 = v16;
                break;
            case 3:
                u12 = (texU + 16 - blockBb.MinX * 16.0D) / 256.0D;
                u14 = (texU + 16 - blockBb.MaxX * 16.0D - 0.01D) / 256.0D;
                v16 = (texV + blockBb.MaxY * 16.0D) / 256.0D;
                v18 = (texV + blockBb.MinY * 16.0D - 0.01D) / 256.0D;
                u20 = u14;
                u22 = u12;
                v24 = v16;
                v26 = v18;
                break;
        }

        double minX = pos.x + blockBb.MinX;
        double maxX = pos.x + blockBb.MaxX;
        double minY = pos.y + blockBb.MinY;
        double maxY = pos.y + blockBb.MaxY;
        double maxZ = pos.z + blockBb.MaxZ;

        if (EnableAo)
        {
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            Tess.addVertexWithUV(minX, maxY, maxZ, u12, v16);
            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            Tess.addVertexWithUV(minX, minY, maxZ, u22, v26);
            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            Tess.addVertexWithUV(maxX, minY, maxZ, u14, v18);
            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            Tess.addVertexWithUV(maxX, maxY, maxZ, u20, v24);
        }
        else
        {
            Tess.addVertexWithUV(minX, maxY, maxZ, u12, v16);
            Tess.addVertexWithUV(minX, minY, maxZ, u22, v26);
            Tess.addVertexWithUV(maxX, minY, maxZ, u14, v18);
            Tess.addVertexWithUV(maxX, maxY, maxZ, u20, v24);
        }
    }

    internal readonly void DrawNorthFace(Block block, in Vec3D pos, in FaceColors colors, int textureId)
    {
        Box blockBb = OverrideBounds ?? block.BoundingBox;
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        double u12 = (texU + blockBb.MinZ * 16.0D) / 256.0D;
        double u14 = (texU + blockBb.MaxZ * 16.0D - 0.01D) / 256.0D;
        double v16 = (texV + 16 - blockBb.MaxY * 16.0D) / 256.0D;
        double v18 = (texV + 16 - blockBb.MinY * 16.0D - 0.01D) / 256.0D;

        if (FlipTexture)
        {
            (u12, u14) = (u14, u12);
        }

        if (blockBb.MinZ < 0.0D || blockBb.MaxZ > 1.0D)
        {
            u12 = texU / 256.0D;
            u14 = (texU + 15.99D) / 256.0D;
        }

        if (blockBb.MinY < 0.0D || blockBb.MaxY > 1.0D)
        {
            v16 = texV / 256.0D;
            v18 = (texV + 15.99D) / 256.0D;
        }

        double u20 = u14, u22 = u12, v24 = v16, v26 = v18;

        switch (UvRotateNorth)
        {
            case 1:
                u12 = (texU + blockBb.MinY * 16.0D) / 256.0D;
                v16 = (texV + 16 - blockBb.MaxZ * 16.0D) / 256.0D;
                u14 = (texU + blockBb.MaxY * 16.0D) / 256.0D;
                v18 = (texV + 16 - blockBb.MinZ * 16.0D) / 256.0D;
                v24 = v16;
                v26 = v18;
                u20 = u12;
                u22 = u14;
                v16 = v18;
                v18 = v24;
                break;
            case 2:
                u12 = (texU + 16 - blockBb.MaxY * 16.0D) / 256.0D;
                v16 = (texV + blockBb.MinZ * 16.0D) / 256.0D;
                u14 = (texU + 16 - blockBb.MinY * 16.0D) / 256.0D;
                v18 = (texV + blockBb.MaxZ * 16.0D) / 256.0D;
                u20 = u14;
                u22 = u12;
                u12 = u14;
                u14 = u22;
                v24 = v18;
                v26 = v16;
                break;
            case 3:
                u12 = (texU + 16 - blockBb.MinZ * 16.0D) / 256.0D;
                u14 = (texU + 16 - blockBb.MaxZ * 16.0D - 0.01D) / 256.0D;
                v16 = (texV + blockBb.MaxY * 16.0D) / 256.0D;
                v18 = (texV + blockBb.MinY * 16.0D - 0.01D) / 256.0D;
                u20 = u14;
                u22 = u12;
                v24 = v16;
                v26 = v18;
                break;
        }

        double minX = pos.x + blockBb.MinX;
        double minY = pos.y + blockBb.MinY;
        double maxY = pos.y + blockBb.MaxY;
        double minZ = pos.z + blockBb.MinZ;
        double maxZ = pos.z + blockBb.MaxZ;

        if (EnableAo)
        {
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            Tess.addVertexWithUV(minX, maxY, maxZ, u20, v24);
            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            Tess.addVertexWithUV(minX, maxY, minZ, u12, v16);
            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            Tess.addVertexWithUV(minX, minY, minZ, u22, v26);
            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            Tess.addVertexWithUV(minX, minY, maxZ, u14, v18);
        }
        else
        {
            Tess.addVertexWithUV(minX, maxY, maxZ, u20, v24);
            Tess.addVertexWithUV(minX, maxY, minZ, u12, v16);
            Tess.addVertexWithUV(minX, minY, minZ, u22, v26);
            Tess.addVertexWithUV(minX, minY, maxZ, u14, v18);
        }
    }

    internal readonly void DrawSouthFace(Block block, in Vec3D pos, in FaceColors colors,
        int textureId)
    {
        Box blockBb = OverrideBounds ?? block.BoundingBox;

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        double u12 = (texU + blockBb.MinZ * 16.0D) / 256.0D;
        double u14 = (texU + blockBb.MaxZ * 16.0D - 0.01D) / 256.0D;
        double v16 = (texV + 16 - blockBb.MaxY * 16.0D) / 256.0D;
        double v18 = (texV + 16 - blockBb.MinY * 16.0D - 0.01D) / 256.0D;

        if (FlipTexture)
        {
            (u12, u14) = (u14, u12);
        }

        if (blockBb.MinZ < 0.0D || blockBb.MaxZ > 1.0D)
        {
            u12 = texU / 256.0D;
            u14 = (texU + 15.99D) / 256.0D;
        }

        if (blockBb.MinY < 0.0D || blockBb.MaxY > 1.0D)
        {
            v16 = texV / 256.0D;
            v18 = (texV + 15.99D) / 256.0D;
        }

        double u20 = u14, u22 = u12, v24 = v16, v26 = v18;

        switch (UvRotateSouth)
        {
            case 2:
                u12 = (texU + blockBb.MinY * 16.0D) / 256.0D;
                v16 = (texV + 16 - blockBb.MinZ * 16.0D) / 256.0D;
                u14 = (texU + blockBb.MaxY * 16.0D) / 256.0D;
                v18 = (texV + 16 - blockBb.MaxZ * 16.0D) / 256.0D;
                v24 = v16;
                v26 = v18;
                u20 = u12;
                u22 = u14;
                v16 = v18;
                break;
            case 1:
                u12 = (texU + 16 - blockBb.MaxY * 16.0D) / 256.0D;
                v16 = (texV + blockBb.MaxZ * 16.0D) / 256.0D;
                u14 = (texU + 16 - blockBb.MinY * 16.0D) / 256.0D;
                v18 = (texV + blockBb.MinZ * 16.0D) / 256.0D;
                u20 = u14;
                u22 = u12;
                u12 = u14;
                v24 = v18;
                v26 = v16;
                break;
            case 3:
                u12 = (texU + 16 - blockBb.MinZ * 16.0D) / 256.0D;
                u14 = (texU + 16 - blockBb.MaxZ * 16.0D - 0.01D) / 256.0D;
                v16 = (texV + blockBb.MaxY * 16.0D) / 256.0D;
                v18 = (texV + blockBb.MinY * 16.0D - 0.01D) / 256.0D;
                u20 = u14;
                u22 = u12;
                v24 = v16;
                v26 = v18;
                break;
        }

        double posX = pos.x + blockBb.MaxX;
        double minY = pos.y + blockBb.MinY;
        double maxY = pos.y + blockBb.MaxY;
        double minZ = pos.z + blockBb.MinZ;
        double maxZ = pos.z + blockBb.MaxZ;

        if (EnableAo)
        {
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            Tess.addVertexWithUV(posX, minY, maxZ, u22, v26);
            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            Tess.addVertexWithUV(posX, minY, minZ, u14, v18);
            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            Tess.addVertexWithUV(posX, maxY, minZ, u20, v24);
            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            Tess.addVertexWithUV(posX, maxY, maxZ, u12, v16);
        }
        else
        {
            Tess.addVertexWithUV(posX, minY, maxZ, u22, v26);
            Tess.addVertexWithUV(posX, minY, minZ, u14, v18);
            Tess.addVertexWithUV(posX, maxY, minZ, u20, v24);
            Tess.addVertexWithUV(posX, maxY, maxZ, u12, v16);
        }
    }

    internal readonly bool DrawBlock(in Block block, in BlockPos pos)
    {
        bool hasRendered = false;
        Box bounds = OverrideBounds ?? block.BoundingBox;

        // 1. Base Colors
        int colorMultiplier = block.getColorMultiplier(World, pos.x, pos.y, pos.z);
        float r = (colorMultiplier >> 16 & 255) / 255.0F;
        float g = (colorMultiplier >> 8 & 255) / 255.0F;
        float b = (colorMultiplier & 255) / 255.0F;

        bool tintBottom = true, tintTop = true, tintEast = true, tintWest = true, tintNorth = true, tintSouth = true;
        if (block.textureId == 3 || OverrideTexture >= 0)
        {
            tintBottom = tintEast = tintWest = tintNorth = tintSouth = false;
        }

        // Cache luminance for the 6 direct neighbors
        float lXn = block.getLuminance(World, pos.x - 1, pos.y, pos.z);
        float lXp = block.getLuminance(World, pos.x + 1, pos.y, pos.z);
        float lYn = block.getLuminance(World, pos.x, pos.y - 1, pos.z);
        float lYp = block.getLuminance(World, pos.x, pos.y + 1, pos.z);
        float lZn = block.getLuminance(World, pos.x, pos.y, pos.z - 1);
        float lZp = block.getLuminance(World, pos.x, pos.y, pos.z + 1);

        // Cache opacity for the 12 edges (Used for AO shadowing)
        // Format: isOpaque[Axis][Direction][Side]
        bool opXnYn = !Block.BlocksAllowVision[World.getBlockId(pos.x - 1, pos.y - 1, pos.z)];
        bool opXnYp = !Block.BlocksAllowVision[World.getBlockId(pos.x - 1, pos.y + 1, pos.z)];
        bool opXpYn = !Block.BlocksAllowVision[World.getBlockId(pos.x + 1, pos.y - 1, pos.z)];
        bool opXpYp = !Block.BlocksAllowVision[World.getBlockId(pos.x + 1, pos.y + 1, pos.z)];
        bool opXnZn = !Block.BlocksAllowVision[World.getBlockId(pos.x - 1, pos.y, pos.z - 1)];
        bool opXnZp = !Block.BlocksAllowVision[World.getBlockId(pos.x - 1, pos.y, pos.z + 1)];
        bool opXpZn = !Block.BlocksAllowVision[World.getBlockId(pos.x + 1, pos.y, pos.z - 1)];
        bool opXpZp = !Block.BlocksAllowVision[World.getBlockId(pos.x + 1, pos.y, pos.z + 1)];
        bool opYnZn = !Block.BlocksAllowVision[World.getBlockId(pos.x, pos.y - 1, pos.z - 1)];
        bool opYnZp = !Block.BlocksAllowVision[World.getBlockId(pos.x, pos.y - 1, pos.z + 1)];
        bool opYpZn = !Block.BlocksAllowVision[World.getBlockId(pos.x, pos.y + 1, pos.z - 1)];
        bool opYpZp = !Block.BlocksAllowVision[World.getBlockId(pos.x, pos.y + 1, pos.z + 1)];

        float v0, v1, v2, v3;

        // BOTTOM FACE (Y - 1)
        if (RenderAllFaces || bounds.MinY > 0.0D || block.isSideVisible(World, pos.x, pos.y - 1, pos.z, 0))
        {
            if (AoBlendMode <= 0) v0 = v1 = v2 = v3 = lYn;
            else
            {
                float n = block.getLuminance(World, pos.x, pos.y - 1, pos.z - 1);
                float s = block.getLuminance(World, pos.x, pos.y - 1, pos.z + 1);
                float w = block.getLuminance(World, pos.x - 1, pos.y - 1, pos.z);
                float e = block.getLuminance(World, pos.x + 1, pos.y - 1, pos.z);
                float nw = (opXnZn || opYnZn) ? w : block.getLuminance(World, pos.x - 1, pos.y - 1, pos.z - 1);
                float sw = (opXnZp || opYnZp) ? w : block.getLuminance(World, pos.x - 1, pos.y - 1, pos.z + 1);
                float ne = (opXpZn || opYnZn) ? e : block.getLuminance(World, pos.x + 1, pos.y - 1, pos.z - 1);
                float se = (opXpZp || opYnZp) ? e : block.getLuminance(World, pos.x + 1, pos.y - 1, pos.z + 1);
                v0 = (sw + w + s + lYn) / 4.0F; // minX, maxZ
                v1 = (w + nw + lYn + n) / 4.0F; // minX, minZ
                v2 = (lYn + n + e + ne) / 4.0F; // maxX, minZ
                v3 = (s + lYn + se + e) / 4.0F; // maxX, maxZ
            }

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, r, g, b, 0.5F, tintBottom);
            int textureId = OverrideTexture >= 0
                ? OverrideTexture
                : block.getTextureId(World, pos.x, pos.y, pos.z, 0);
            DrawBottomFace(block, new Vec3D(pos.x, pos.y, pos.z), colors, textureId);
            hasRendered = true;
        }

        // TOP FACE (Y + 1)
        if (RenderAllFaces || bounds.MaxY < 1.0D || block.isSideVisible(World, pos.x, pos.y + 1, pos.z, 1))
        {
            if (AoBlendMode <= 0) v0 = v1 = v2 = v3 = lYp;
            else
            {
                float n = block.getLuminance(World, pos.x, pos.y + 1, pos.z - 1);
                float s = block.getLuminance(World, pos.x, pos.y + 1, pos.z + 1);
                float w = block.getLuminance(World, pos.x - 1, pos.y + 1, pos.z);
                float e = block.getLuminance(World, pos.x + 1, pos.y + 1, pos.z);
                float nw = (opXnYp || opYpZn) ? w : block.getLuminance(World, pos.x - 1, pos.y + 1, pos.z - 1);
                float sw = (opXnYp || opYpZp) ? w : block.getLuminance(World, pos.x - 1, pos.y + 1, pos.z + 1);
                float ne = (opXpYp || opYpZn) ? e : block.getLuminance(World, pos.x + 1, pos.y + 1, pos.z - 1);
                float se = (opXpYp || opYpZp) ? e : block.getLuminance(World, pos.x + 1, pos.y + 1, pos.z + 1);
                v0 = (s + lYp + se + e) / 4.0F; // maxX, maxZ
                v1 = (lYp + n + e + ne) / 4.0F; // maxX, minZ
                v2 = (w + nw + lYp + n) / 4.0F; // minX, minZ
                v3 = (sw + w + s + lYp) / 4.0F; // minX, maxZ
            }

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, r, g, b, 1.0F, tintTop);
            int textureId = OverrideTexture >= 0
                ? OverrideTexture
                : block.getTextureId(World, pos.x, pos.y, pos.z, 1);
            DrawTopFace(block, new Vec3D(pos.x, pos.y, pos.z), colors, textureId);
            hasRendered = true;
        }

        // EAST FACE (Z - 1)
        if (RenderAllFaces || bounds.MinZ > 0.0D || block.isSideVisible(World, pos.x, pos.y, pos.z - 1, 2))
        {
            if (AoBlendMode <= 0) v0 = v1 = v2 = v3 = lZn;
            else
            {
                float u = block.getLuminance(World, pos.x, pos.y + 1, pos.z - 1);
                float d = block.getLuminance(World, pos.x, pos.y - 1, pos.z - 1);
                float w = block.getLuminance(World, pos.x - 1, pos.y, pos.z - 1);
                float e = block.getLuminance(World, pos.x + 1, pos.y, pos.z - 1);
                float uw = (opXnZn || opYpZn) ? w : block.getLuminance(World, pos.x - 1, pos.y + 1, pos.z - 1);
                float dw = (opXnZn || opYnZn) ? w : block.getLuminance(World, pos.x - 1, pos.y - 1, pos.z - 1);
                float ue = (opXpZn || opYpZn) ? e : block.getLuminance(World, pos.x + 1, pos.y + 1, pos.z - 1);
                float de = (opXpZn || opYnZn) ? e : block.getLuminance(World, pos.x + 1, pos.y - 1, pos.z - 1);
                v0 = (w + uw + lZn + u) / 4.0F;
                v1 = (lZn + u + e + ue) / 4.0F;
                v2 = (d + lZn + de + e) / 4.0F;
                v3 = (dw + w + d + lZn) / 4.0F;
            }

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, r, g, b, 0.8F, tintEast);
            int textureId = OverrideTexture >= 0
                ? OverrideTexture
                : block.getTextureId(World, pos.x, pos.y, pos.z, 2);
            DrawEastFace(block, new Vec3D(pos.x, pos.y, pos.z), colors, textureId);
            hasRendered = true;
        }

        // WEST FACE (Z + 1)
        if (RenderAllFaces || bounds.MaxZ < 1.0D || block.isSideVisible(World, pos.x, pos.y, pos.z + 1, 3))
        {
            if (AoBlendMode <= 0) v0 = v1 = v2 = v3 = lZp;
            else
            {
                float u = block.getLuminance(World, pos.x, pos.y + 1, pos.z + 1);
                float d = block.getLuminance(World, pos.x, pos.y - 1, pos.z + 1);
                float w = block.getLuminance(World, pos.x - 1, pos.y, pos.z + 1);
                float e = block.getLuminance(World, pos.x + 1, pos.y, pos.z + 1);
                float uw = (opXnZp || opYpZp) ? w : block.getLuminance(World, pos.x - 1, pos.y + 1, pos.z + 1);
                float dw = (opXnZp || opYnZp) ? w : block.getLuminance(World, pos.x - 1, pos.y - 1, pos.z + 1);
                float ue = (opXpZp || opYpZp) ? e : block.getLuminance(World, pos.x + 1, pos.y + 1, pos.z + 1);
                float de = (opXpZp || opYnZp) ? e : block.getLuminance(World, pos.x + 1, pos.y - 1, pos.z + 1);
                v0 = (w + uw + lZp + u) / 4.0F;
                v1 = (dw + w + d + lZp) / 4.0F;
                v2 = (d + lZp + de + e) / 4.0F;
                v3 = (lZp + u + e + ue) / 4.0F;
            }

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, r, g, b, 0.8F, tintWest);
            int textureId = OverrideTexture >= 0
                ? OverrideTexture
                : block.getTextureId(World, pos.x, pos.y, pos.z, 3);
            DrawWestFace(block, new Vec3D(pos.x, pos.y, pos.z), colors, textureId);
            hasRendered = true;
        }

        // NORTH FACE (X - 1)
        if (RenderAllFaces || bounds.MinX > 0.0D || block.isSideVisible(World, pos.x - 1, pos.y, pos.z, 4))
        {
            if (AoBlendMode <= 0) v0 = v1 = v2 = v3 = lXn;
            else
            {
                float u = block.getLuminance(World, pos.x - 1, pos.y + 1, pos.z);
                float d = block.getLuminance(World, pos.x - 1, pos.y - 1, pos.z);
                float n = block.getLuminance(World, pos.x - 1, pos.y, pos.z - 1);
                float s = block.getLuminance(World, pos.x - 1, pos.y, pos.z + 1);
                float un = (opXnZn || opXnYp) ? n : block.getLuminance(World, pos.x - 1, pos.y + 1, pos.z - 1);
                float dn = (opXnZn || opXnYn) ? n : block.getLuminance(World, pos.x - 1, pos.y - 1, pos.z - 1);
                float us = (opXnZp || opXnYp) ? s : block.getLuminance(World, pos.x - 1, pos.y + 1, pos.z + 1);
                float ds = (opXnZp || opXnYn) ? s : block.getLuminance(World, pos.x - 1, pos.y - 1, pos.z + 1);
                v0 = (u + us + lXn + s) / 4.0F;
                v1 = (u + un + n + lXn) / 4.0F;
                v2 = (n + lXn + dn + d) / 4.0F;
                v3 = (d + ds + lXn + s) / 4.0F;
            }

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, r, g, b, 0.6F, tintNorth);
            int textureId = OverrideTexture >= 0
                ? OverrideTexture
                : block.getTextureId(World, pos.x, pos.y, pos.z, 4);
            DrawNorthFace(block, new Vec3D(pos.x, pos.y, pos.z), colors, textureId);
            hasRendered = true;
        }

        // SOUTH FACE (X + 1)
        if (RenderAllFaces || bounds.MaxX < 1.0D || block.isSideVisible(World, pos.x + 1, pos.y, pos.z, 5))
        {
            if (AoBlendMode <= 0) v0 = v1 = v2 = v3 = lXp;
            else
            {
                float u = block.getLuminance(World, pos.x + 1, pos.y + 1, pos.z);
                float d = block.getLuminance(World, pos.x + 1, pos.y - 1, pos.z);
                float n = block.getLuminance(World, pos.x + 1, pos.y, pos.z - 1);
                float s = block.getLuminance(World, pos.x + 1, pos.y, pos.z + 1);
                float un = (opXpZn || opXpYp) ? n : block.getLuminance(World, pos.x + 1, pos.y + 1, pos.z - 1);
                float dn = (opXpZn || opXpYn) ? n : block.getLuminance(World, pos.x + 1, pos.y - 1, pos.z - 1);
                float us = (opXpZp || opXpYp) ? s : block.getLuminance(World, pos.x + 1, pos.y + 1, pos.z + 1);
                float ds = (opXpZp || opXpYn) ? s : block.getLuminance(World, pos.x + 1, pos.y - 1, pos.z + 1);
                v0 = (d + ds + lXp + s) / 4.0F;
                v1 = (n + lXp + dn + d) / 4.0F;
                v2 = (u + un + n + lXp) / 4.0F;
                v3 = (u + us + lXp + s) / 4.0F;
            }

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, r, g, b, 0.6F, tintSouth);
            int textureId = OverrideTexture >= 0 ? OverrideTexture : block.getTextureId(World, pos.x, pos.y, pos.z, 5);
            DrawSouthFace(block, new Vec3D(pos.x, pos.y, pos.z), colors, textureId);
            hasRendered = true;
        }

        return hasRendered;
    }

    internal readonly void DrawTorch(in Block block, in Vec3D pos, double tiltX, double tiltZ)
    {
        int textureId = block.getTexture(0);
        if (OverrideTexture >= 0)
        {
            textureId = OverrideTexture;
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
        double centerX = pos.x + 0.5D;
        double centerZ = pos.z + 0.5D;

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

        Tess.addVertexWithUV(tipX - radius, pos.y + height, tipZ - radius, topMinU, topMinV);
        Tess.addVertexWithUV(tipX - radius, pos.y + height, tipZ + radius, topMinU, topMaxV);
        Tess.addVertexWithUV(tipX + radius, pos.y + height, tipZ + radius, topMaxU, topMaxV);
        Tess.addVertexWithUV(tipX + radius, pos.y + height, tipZ - radius, topMaxU, topMinV);

        // SIDE FACES
        // The top vertices stay near the center, while the bottom vertices are shifted by tiltX and tiltZ

        // West Face
        Tess.addVertexWithUV(centerX - radius, pos.y + 1.0D, frontZ, minU, minV);
        Tess.addVertexWithUV(centerX - radius + tiltX, pos.y + 0.0D, frontZ + tiltZ, minU, maxV);
        Tess.addVertexWithUV(centerX - radius + tiltX, pos.y + 0.0D, backZ + tiltZ, maxU, maxV);
        Tess.addVertexWithUV(centerX - radius, pos.y + 1.0D, backZ, maxU, minV);

        // East Face
        Tess.addVertexWithUV(centerX + radius, pos.y + 1.0D, backZ, minU, minV);
        Tess.addVertexWithUV(centerX + radius + tiltX, pos.y + 0.0D, backZ + tiltZ, minU, maxV);
        Tess.addVertexWithUV(centerX + radius + tiltX, pos.y + 0.0D, frontZ + tiltZ, maxU, maxV);
        Tess.addVertexWithUV(centerX + radius, pos.y + 1.0D, frontZ, maxU, minV);

        // North Face
        Tess.addVertexWithUV(leftX, pos.y + 1.0D, centerZ + radius, minU, minV);
        Tess.addVertexWithUV(leftX + tiltX, pos.y + 0.0D, centerZ + radius + tiltZ, minU, maxV);
        Tess.addVertexWithUV(rightX + tiltX, pos.y + 0.0D, centerZ + radius + tiltZ, maxU, maxV);
        Tess.addVertexWithUV(rightX, pos.y + 1.0D, centerZ + radius, maxU, minV);

        // South Face
        Tess.addVertexWithUV(rightX, pos.y + 1.0D, centerZ - radius, minU, minV);
        Tess.addVertexWithUV(rightX + tiltX, pos.y + 0.0D, centerZ - radius + tiltZ, minU, maxV);
        Tess.addVertexWithUV(leftX + tiltX, pos.y + 0.0D, centerZ - radius + tiltZ, maxU, maxV);
        Tess.addVertexWithUV(leftX, pos.y + 1.0D, centerZ - radius, maxU, minV);
    }
}
