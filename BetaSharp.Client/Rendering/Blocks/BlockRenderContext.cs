using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Blocks.Renderers;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks;

public ref struct BlockRenderContext
{
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
    public readonly bool CustomFlag;

    public BlockRenderContext(
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


    internal readonly void RenderBottomFace(Block block, in Vec3D pos, Tessellator tess, in FaceColors colors,
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
            tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            tess.addVertexWithUV(minX, minY, maxZ, u2, v2);
            tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            tess.addVertexWithUV(minX, minY, minZ, minU, minV);
            tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            tess.addVertexWithUV(maxX, minY, minZ, u1, v1);
            tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            tess.addVertexWithUV(maxX, minY, maxZ, maxU, maxV);
        }
        else
        {
            tess.addVertexWithUV(minX, minY, maxZ, u2, v2);
            tess.addVertexWithUV(minX, minY, minZ, minU, minV);
            tess.addVertexWithUV(maxX, minY, minZ, u1, v1);
            tess.addVertexWithUV(maxX, minY, maxZ, maxU, maxV);
        }
    }

    internal readonly void RenderTopFace(Block block, in Vec3D pos, Tessellator tess, in FaceColors colors,
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
            tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            tess.addVertexWithUV(maxX, maxY, maxZ, maxU, maxV);
            tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            tess.addVertexWithUV(maxX, maxY, minZ, u1, v1);
            tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            tess.addVertexWithUV(minX, maxY, minZ, minU, minV);
            tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            tess.addVertexWithUV(minX, maxY, maxZ, u2, v2);
        }
        else
        {
            tess.addVertexWithUV(maxX, maxY, maxZ, maxU, maxV);
            tess.addVertexWithUV(maxX, maxY, minZ, u1, v1);
            tess.addVertexWithUV(minX, maxY, minZ, minU, minV);
            tess.addVertexWithUV(minX, maxY, maxZ, u2, v2);
        }
    }

    internal readonly void RenderEastFace(Block block, in Vec3D pos, Tessellator tess, in FaceColors colors,
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
            tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            tess.addVertexWithUV(minX, maxY, minZ, u1, v1);
            tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            tess.addVertexWithUV(maxX, maxY, minZ, minU, minV);
            tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            tess.addVertexWithUV(maxX, minY, minZ, u2, v2);
            tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            tess.addVertexWithUV(minX, minY, minZ, maxU, maxV);
        }
        else
        {
            tess.addVertexWithUV(minX, maxY, minZ, u1, v1);
            tess.addVertexWithUV(maxX, maxY, minZ, minU, minV);
            tess.addVertexWithUV(maxX, minY, minZ, u2, v2);
            tess.addVertexWithUV(minX, minY, minZ, maxU, maxV);
        }
    }

    internal readonly void RenderWestFace(Block block, in Vec3D pos, Tessellator tess, in FaceColors colors,
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

        if (UvRotateWest == 1)
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
        else if (UvRotateWest == 2)
        {
            minU = (texU + 16 - blockBb.MaxY * 16.0D) / 256.0D;
            minV = (texV + blockBb.MinX * 16.0D) / 256.0D;
            maxU = (texU + 16 - blockBb.MinY * 16.0D) / 256.0D;
            maxV = (texV + blockBb.MaxX * 16.0D) / 256.0D;
            u1 = maxU;
            u2 = minU;
            minU = maxU;
            maxU = u2;
            v1 = maxV;
            v2 = minV;
        }
        else if (UvRotateWest == 3)
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
        double maxZ = pos.z + blockBb.MaxZ;

        if (EnableAo)
        {
            tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            tess.addVertexWithUV(minX, maxY, maxZ, minU, minV);
            tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            tess.addVertexWithUV(minX, minY, maxZ, u2, v2);
            tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            tess.addVertexWithUV(maxX, minY, maxZ, maxU, maxV);
            tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            tess.addVertexWithUV(maxX, maxY, maxZ, u1, v1);
        }
        else
        {
            tess.addVertexWithUV(minX, maxY, maxZ, minU, minV);
            tess.addVertexWithUV(minX, minY, maxZ, u2, v2);
            tess.addVertexWithUV(maxX, minY, maxZ, maxU, maxV);
            tess.addVertexWithUV(maxX, maxY, maxZ, u1, v1);
        }
    }

    internal readonly void RenderNorthFace(Block block, in Vec3D pos, Tessellator tess, in FaceColors colors,
        int textureId)
    {
        Box blockBb = OverrideBounds ?? block.BoundingBox;
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = (texU + blockBb.MinZ * 16.0D) / 256.0D;
        double maxU = (texU + blockBb.MaxZ * 16.0D - 0.01D) / 256.0D;
        double minV = (texV + 16 - blockBb.MaxY * 16.0D) / 256.0D;
        double maxV = (texV + 16 - blockBb.MinY * 16.0D - 0.01D) / 256.0D;

        if (FlipTexture)
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

        if (UvRotateNorth == 1)
        {
            minU = (texU + blockBb.MinY * 16.0D) / 256.0D;
            minV = (texV + 16 - blockBb.MaxZ * 16.0D) / 256.0D;
            maxU = (texU + blockBb.MaxY * 16.0D) / 256.0D;
            maxV = (texV + 16 - blockBb.MinZ * 16.0D) / 256.0D;
            v1 = minV;
            v2 = maxV;
            u1 = minU;
            u2 = maxU;
            minV = maxV;
            maxV = v1;
        }
        else if (UvRotateNorth == 2)
        {
            minU = (texU + 16 - blockBb.MaxY * 16.0D) / 256.0D;
            minV = (texV + blockBb.MinZ * 16.0D) / 256.0D;
            maxU = (texU + 16 - blockBb.MinY * 16.0D) / 256.0D;
            maxV = (texV + blockBb.MaxZ * 16.0D) / 256.0D;
            u1 = maxU;
            u2 = minU;
            minU = maxU;
            maxU = u2;
            v1 = maxV;
            v2 = minV;
        }
        else if (UvRotateNorth == 3)
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

        double minX = pos.x + blockBb.MinX;
        double minY = pos.y + blockBb.MinY;
        double maxY = pos.y + blockBb.MaxY;
        double minZ = pos.z + blockBb.MinZ;
        double maxZ = pos.z + blockBb.MaxZ;

        if (EnableAo)
        {
            tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            tess.addVertexWithUV(minX, maxY, maxZ, u1, v1);
            tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            tess.addVertexWithUV(minX, maxY, minZ, minU, minV);
            tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            tess.addVertexWithUV(minX, minY, minZ, u2, v2);
            tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            tess.addVertexWithUV(minX, minY, maxZ, maxU, maxV);
        }
        else
        {
            tess.addVertexWithUV(minX, maxY, maxZ, u1, v1);
            tess.addVertexWithUV(minX, maxY, minZ, minU, minV);
            tess.addVertexWithUV(minX, minY, minZ, u2, v2);
            tess.addVertexWithUV(minX, minY, maxZ, maxU, maxV);
        }
    }

    internal readonly void RenderSouthFace(Block block, in Vec3D pos, Tessellator tess, in FaceColors colors,
        int textureId)
    {
        Box blockBb = OverrideBounds ?? block.BoundingBox;

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        double minU = (texU + blockBb.MinZ * 16.0D) / 256.0D;
        double maxU = (texU + blockBb.MaxZ * 16.0D - 0.01D) / 256.0D;
        double minV = (texV + 16 - blockBb.MaxY * 16.0D) / 256.0D;
        double maxV = (texV + 16 - blockBb.MinY * 16.0D - 0.01D) / 256.0D;

        if (FlipTexture)
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

        if (UvRotateSouth == 2)
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
        else if (UvRotateSouth == 1)
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
        else if (UvRotateSouth == 3)
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

        double posX = pos.x + blockBb.MaxX;
        double minY = pos.y + blockBb.MinY;
        double maxY = pos.y + blockBb.MaxY;
        double minZ = pos.z + blockBb.MinZ;
        double maxZ = pos.z + blockBb.MaxZ;

        if (EnableAo)
        {
            tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            tess.addVertexWithUV(posX, minY, maxZ, u2, v2);
            tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            tess.addVertexWithUV(posX, minY, minZ, u1, v2);
            tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            tess.addVertexWithUV(posX, maxY, minZ, u1, v1);
            tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
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

    internal readonly bool RenderStandardBlock(in Block block, in BlockPos pos, in IBlockAccess world, Tessellator tess)
    {
        bool hasRendered = false;
        Box bounds = OverrideBounds ?? block.BoundingBox;

        // 1. Base Colors
        int colorMultiplier = block.getColorMultiplier(world, pos.x, pos.y, pos.z);
        float r = (colorMultiplier >> 16 & 255) / 255.0F;
        float g = (colorMultiplier >> 8 & 255) / 255.0F;
        float b = (colorMultiplier & 255) / 255.0F;

        bool tintBottom = true, tintTop = true, tintEast = true, tintWest = true, tintNorth = true, tintSouth = true;
        if (block.textureId == 3 || OverrideTexture >= 0)
        {
            tintBottom = tintEast = tintWest = tintNorth = tintSouth = false;
        }

        // Cache luminance for the 6 direct neighbors
        float lXn = block.getLuminance(world, pos.x - 1, pos.y, pos.z);
        float lXp = block.getLuminance(world, pos.x + 1, pos.y, pos.z);
        float lYn = block.getLuminance(world, pos.x, pos.y - 1, pos.z);
        float lYp = block.getLuminance(world, pos.x, pos.y + 1, pos.z);
        float lZn = block.getLuminance(world, pos.x, pos.y, pos.z - 1);
        float lZp = block.getLuminance(world, pos.x, pos.y, pos.z + 1);

        // Cache opacity for the 12 edges (Used for AO shadowing)
        // Format: isOpaque[Axis][Direction][Side]
        bool opXnYn = !Block.BlocksAllowVision[world.getBlockId(pos.x - 1, pos.y - 1, pos.z)];
        bool opXnYp = !Block.BlocksAllowVision[world.getBlockId(pos.x - 1, pos.y + 1, pos.z)];
        bool opXpYn = !Block.BlocksAllowVision[world.getBlockId(pos.x + 1, pos.y - 1, pos.z)];
        bool opXpYp = !Block.BlocksAllowVision[world.getBlockId(pos.x + 1, pos.y + 1, pos.z)];
        bool opXnZn = !Block.BlocksAllowVision[world.getBlockId(pos.x - 1, pos.y, pos.z - 1)];
        bool opXnZp = !Block.BlocksAllowVision[world.getBlockId(pos.x - 1, pos.y, pos.z + 1)];
        bool opXpZn = !Block.BlocksAllowVision[world.getBlockId(pos.x + 1, pos.y, pos.z - 1)];
        bool opXpZp = !Block.BlocksAllowVision[world.getBlockId(pos.x + 1, pos.y, pos.z + 1)];
        bool opYnZn = !Block.BlocksAllowVision[world.getBlockId(pos.x, pos.y - 1, pos.z - 1)];
        bool opYnZp = !Block.BlocksAllowVision[world.getBlockId(pos.x, pos.y - 1, pos.z + 1)];
        bool opYpZn = !Block.BlocksAllowVision[world.getBlockId(pos.x, pos.y + 1, pos.z - 1)];
        bool opYpZp = !Block.BlocksAllowVision[world.getBlockId(pos.x, pos.y + 1, pos.z + 1)];

        float v0, v1, v2, v3;

        // BOTTOM FACE (Y - 1)
        if (RenderAllFaces || bounds.MinY > 0.0D || block.isSideVisible(world, pos.x, pos.y - 1, pos.z, 0))
        {
            if (AoBlendMode <= 0) v0 = v1 = v2 = v3 = lYn;
            else
            {
                float n = block.getLuminance(world, pos.x, pos.y - 1, pos.z - 1);
                float s = block.getLuminance(world, pos.x, pos.y - 1, pos.z + 1);
                float w = block.getLuminance(world, pos.x - 1, pos.y - 1, pos.z);
                float e = block.getLuminance(world, pos.x + 1, pos.y - 1, pos.z);
                float nw = (opXnZn || opYnZn) ? w : block.getLuminance(world, pos.x - 1, pos.y - 1, pos.z - 1);
                float sw = (opXnZp || opYnZp) ? w : block.getLuminance(world, pos.x - 1, pos.y - 1, pos.z + 1);
                float ne = (opXpZn || opYnZn) ? e : block.getLuminance(world, pos.x + 1, pos.y - 1, pos.z - 1);
                float se = (opXpZp || opYnZp) ? e : block.getLuminance(world, pos.x + 1, pos.y - 1, pos.z + 1);
                v0 = (sw + w + s + lYn) / 4.0F; // minX, maxZ
                v1 = (w + nw + lYn + n) / 4.0F; // minX, minZ
                v2 = (lYn + n + e + ne) / 4.0F; // maxX, minZ
                v3 = (s + lYn + se + e) / 4.0F; // maxX, maxZ
            }

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, r, g, b, 0.5F, tintBottom);
            int textureId = OverrideTexture >= 0
                ? OverrideTexture
                : block.getTextureId(world, pos.x, pos.y, pos.z, 0);
            RenderBottomFace(block, new Vec3D(pos.x, pos.y, pos.z), tess, colors, textureId);
            hasRendered = true;
        }

        // TOP FACE (Y + 1)
        if (RenderAllFaces || bounds.MaxY < 1.0D || block.isSideVisible(world, pos.x, pos.y + 1, pos.z, 1))
        {
            if (AoBlendMode <= 0) v0 = v1 = v2 = v3 = lYp;
            else
            {
                float n = block.getLuminance(world, pos.x, pos.y + 1, pos.z - 1);
                float s = block.getLuminance(world, pos.x, pos.y + 1, pos.z + 1);
                float w = block.getLuminance(world, pos.x - 1, pos.y + 1, pos.z);
                float e = block.getLuminance(world, pos.x + 1, pos.y + 1, pos.z);
                float nw = (opXnYp || opYpZn) ? w : block.getLuminance(world, pos.x - 1, pos.y + 1, pos.z - 1);
                float sw = (opXnYp || opYpZp) ? w : block.getLuminance(world, pos.x - 1, pos.y + 1, pos.z + 1);
                float ne = (opXpYp || opYpZn) ? e : block.getLuminance(world, pos.x + 1, pos.y + 1, pos.z - 1);
                float se = (opXpYp || opYpZp) ? e : block.getLuminance(world, pos.x + 1, pos.y + 1, pos.z + 1);
                v0 = (s + lYp + se + e) / 4.0F; // maxX, maxZ
                v1 = (lYp + n + e + ne) / 4.0F; // maxX, minZ
                v2 = (w + nw + lYp + n) / 4.0F; // minX, minZ
                v3 = (sw + w + s + lYp) / 4.0F; // minX, maxZ
            }

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, r, g, b, 1.0F, tintTop);
            int textureId = OverrideTexture >= 0
                ? OverrideTexture
                : block.getTextureId(world, pos.x, pos.y, pos.z, 1);
            RenderTopFace(block, new Vec3D(pos.x, pos.y, pos.z), tess, colors, textureId);
            hasRendered = true;
        }

        // EAST FACE (Z - 1)
        if (RenderAllFaces || bounds.MinZ > 0.0D || block.isSideVisible(world, pos.x, pos.y, pos.z - 1, 2))
        {
            if (AoBlendMode <= 0) v0 = v1 = v2 = v3 = lZn;
            else
            {
                float u = block.getLuminance(world, pos.x, pos.y + 1, pos.z - 1);
                float d = block.getLuminance(world, pos.x, pos.y - 1, pos.z - 1);
                float w = block.getLuminance(world, pos.x - 1, pos.y, pos.z - 1);
                float e = block.getLuminance(world, pos.x + 1, pos.y, pos.z - 1);
                float uw = (opXnZn || opYpZn) ? w : block.getLuminance(world, pos.x - 1, pos.y + 1, pos.z - 1);
                float dw = (opXnZn || opYnZn) ? w : block.getLuminance(world, pos.x - 1, pos.y - 1, pos.z - 1);
                float ue = (opXpZn || opYpZn) ? e : block.getLuminance(world, pos.x + 1, pos.y + 1, pos.z - 1);
                float de = (opXpZn || opYnZn) ? e : block.getLuminance(world, pos.x + 1, pos.y - 1, pos.z - 1);
                v0 = (w + uw + lZn + u) / 4.0F;
                v1 = (lZn + u + e + ue) / 4.0F;
                v2 = (d + lZn + de + e) / 4.0F;
                v3 = (dw + w + d + lZn) / 4.0F;
            }

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, r, g, b, 0.8F, tintEast);
            int textureId = OverrideTexture >= 0
                ? OverrideTexture
                : block.getTextureId(world, pos.x, pos.y, pos.z, 2);
            RenderEastFace(block, new Vec3D(pos.x, pos.y, pos.z), tess, colors, textureId);
            hasRendered = true;
        }

        // WEST FACE (Z + 1)
        if (RenderAllFaces || bounds.MaxZ < 1.0D || block.isSideVisible(world, pos.x, pos.y, pos.z + 1, 3))
        {
            if (AoBlendMode <= 0) v0 = v1 = v2 = v3 = lZp;
            else
            {
                float u = block.getLuminance(world, pos.x, pos.y + 1, pos.z + 1);
                float d = block.getLuminance(world, pos.x, pos.y - 1, pos.z + 1);
                float w = block.getLuminance(world, pos.x - 1, pos.y, pos.z + 1);
                float e = block.getLuminance(world, pos.x + 1, pos.y, pos.z + 1);
                float uw = (opXnZp || opYpZp) ? w : block.getLuminance(world, pos.x - 1, pos.y + 1, pos.z + 1);
                float dw = (opXnZp || opYnZp) ? w : block.getLuminance(world, pos.x - 1, pos.y - 1, pos.z + 1);
                float ue = (opXpZp || opYpZp) ? e : block.getLuminance(world, pos.x + 1, pos.y + 1, pos.z + 1);
                float de = (opXpZp || opYnZp) ? e : block.getLuminance(world, pos.x + 1, pos.y - 1, pos.z + 1);
                v0 = (w + uw + lZp + u) / 4.0F;
                v1 = (dw + w + d + lZp) / 4.0F;
                v2 = (d + lZp + de + e) / 4.0F;
                v3 = (lZp + u + e + ue) / 4.0F;
            }

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, r, g, b, 0.8F, tintWest);
            int textureId = OverrideTexture >= 0
                ? OverrideTexture
                : block.getTextureId(world, pos.x, pos.y, pos.z, 3);
            RenderWestFace(block, new Vec3D(pos.x, pos.y, pos.z), tess, colors, textureId);
            hasRendered = true;
        }

        // NORTH FACE (X - 1)
        if (RenderAllFaces || bounds.MinX > 0.0D || block.isSideVisible(world, pos.x - 1, pos.y, pos.z, 4))
        {
            if (AoBlendMode <= 0) v0 = v1 = v2 = v3 = lXn;
            else
            {
                float u = block.getLuminance(world, pos.x - 1, pos.y + 1, pos.z);
                float d = block.getLuminance(world, pos.x - 1, pos.y - 1, pos.z);
                float n = block.getLuminance(world, pos.x - 1, pos.y, pos.z - 1);
                float s = block.getLuminance(world, pos.x - 1, pos.y, pos.z + 1);
                float un = (opXnZn || opXnYp) ? n : block.getLuminance(world, pos.x - 1, pos.y + 1, pos.z - 1);
                float dn = (opXnZn || opXnYn) ? n : block.getLuminance(world, pos.x - 1, pos.y - 1, pos.z - 1);
                float us = (opXnZp || opXnYp) ? s : block.getLuminance(world, pos.x - 1, pos.y + 1, pos.z + 1);
                float ds = (opXnZp || opXnYn) ? s : block.getLuminance(world, pos.x - 1, pos.y - 1, pos.z + 1);
                v0 = (u + us + lXn + s) / 4.0F;
                v1 = (u + un + n + lXn) / 4.0F;
                v2 = (n + lXn + dn + d) / 4.0F;
                v3 = (d + ds + lXn + s) / 4.0F;
            }

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, r, g, b, 0.6F, tintNorth);
            int textureId = OverrideTexture >= 0
                ? OverrideTexture
                : block.getTextureId(world, pos.x, pos.y, pos.z, 4);
            RenderNorthFace(block, new Vec3D(pos.x, pos.y, pos.z), tess, colors, textureId);
            hasRendered = true;
        }

        // SOUTH FACE (X + 1)
        if (RenderAllFaces || bounds.MaxX < 1.0D || block.isSideVisible(world, pos.x + 1, pos.y, pos.z, 5))
        {
            if (AoBlendMode <= 0) v0 = v1 = v2 = v3 = lXp;
            else
            {
                float u = block.getLuminance(world, pos.x + 1, pos.y + 1, pos.z);
                float d = block.getLuminance(world, pos.x + 1, pos.y - 1, pos.z);
                float n = block.getLuminance(world, pos.x + 1, pos.y, pos.z - 1);
                float s = block.getLuminance(world, pos.x + 1, pos.y, pos.z + 1);
                float un = (opXpZn || opXpYp) ? n : block.getLuminance(world, pos.x + 1, pos.y + 1, pos.z - 1);
                float dn = (opXpZn || opXpYn) ? n : block.getLuminance(world, pos.x + 1, pos.y - 1, pos.z - 1);
                float us = (opXpZp || opXpYp) ? s : block.getLuminance(world, pos.x + 1, pos.y + 1, pos.z + 1);
                float ds = (opXpZp || opXpYn) ? s : block.getLuminance(world, pos.x + 1, pos.y - 1, pos.z + 1);
                v0 = (d + ds + lXp + s) / 4.0F;
                v1 = (n + lXp + dn + d) / 4.0F;
                v2 = (u + un + n + lXp) / 4.0F;
                v3 = (u + us + lXp + s) / 4.0F;
            }

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, r, g, b, 0.6F, tintSouth);
            int textureId = OverrideTexture >= 0 ? OverrideTexture : block.getTextureId(world, pos.x, pos.y, pos.z, 5);
            RenderSouthFace(block, new Vec3D(pos.x, pos.y, pos.z), tess, colors, textureId);
            hasRendered = true;
        }

        return hasRendered;
    }

    internal readonly void DrawTorch(in Block block, in Tessellator tess, in Vec3D pos, double tiltX, double tiltZ)
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

        tess.addVertexWithUV(tipX - radius, pos.y + height, tipZ - radius, topMinU, topMinV);
        tess.addVertexWithUV(tipX - radius, pos.y + height, tipZ + radius, topMinU, topMaxV);
        tess.addVertexWithUV(tipX + radius, pos.y + height, tipZ + radius, topMaxU, topMaxV);
        tess.addVertexWithUV(tipX + radius, pos.y + height, tipZ - radius, topMaxU, topMinV);

        // SIDE FACES
        // The top vertices stay near the center, while the bottom vertices are shifted by tiltX and tiltZ

        // West Face
        tess.addVertexWithUV(centerX - radius, pos.y + 1.0D, frontZ, minU, minV);
        tess.addVertexWithUV(centerX - radius + tiltX, pos.y + 0.0D, frontZ + tiltZ, minU, maxV);
        tess.addVertexWithUV(centerX - radius + tiltX, pos.y + 0.0D, backZ + tiltZ, maxU, maxV);
        tess.addVertexWithUV(centerX - radius, pos.y + 1.0D, backZ, maxU, minV);

        // East Face
        tess.addVertexWithUV(centerX + radius, pos.y + 1.0D, backZ, minU, minV);
        tess.addVertexWithUV(centerX + radius + tiltX, pos.y + 0.0D, backZ + tiltZ, minU, maxV);
        tess.addVertexWithUV(centerX + radius + tiltX, pos.y + 0.0D, frontZ + tiltZ, maxU, maxV);
        tess.addVertexWithUV(centerX + radius, pos.y + 1.0D, frontZ, maxU, minV);

        // North Face
        tess.addVertexWithUV(leftX, pos.y + 1.0D, centerZ + radius, minU, minV);
        tess.addVertexWithUV(leftX + tiltX, pos.y + 0.0D, centerZ + radius + tiltZ, minU, maxV);
        tess.addVertexWithUV(rightX + tiltX, pos.y + 0.0D, centerZ + radius + tiltZ, maxU, maxV);
        tess.addVertexWithUV(rightX, pos.y + 1.0D, centerZ + radius, maxU, minV);

        // South Face
        tess.addVertexWithUV(rightX, pos.y + 1.0D, centerZ - radius, minU, minV);
        tess.addVertexWithUV(rightX + tiltX, pos.y + 0.0D, centerZ - radius + tiltZ, minU, maxV);
        tess.addVertexWithUV(leftX + tiltX, pos.y + 0.0D, centerZ - radius + tiltZ, maxU, maxV);
        tess.addVertexWithUV(leftX, pos.y + 1.0D, centerZ - radius, maxU, minV);
    }
}
