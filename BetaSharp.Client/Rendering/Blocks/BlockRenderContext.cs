using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Blocks.Renderers;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;

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


    internal readonly void RenderBottomFace(Block block, in Vec3D pos, Tessellator tess, in FaceColors colors, int textureId)
    {
        Box blockBb = this.OverrideBounds ?? block.BoundingBox;
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

        if (this.UvRotateBottom == 2)
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
        else if (this.UvRotateBottom == 1)
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
        else if (this.UvRotateBottom == 3)
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

        if (this.EnableAo)
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

    internal readonly void RenderTopFace(Block block, in Vec3D pos, Tessellator tess, in FaceColors colors, int textureId)
    {
        Box blockBb = this.OverrideBounds ?? block.BoundingBox;
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

        if (this.UvRotateTop == 1)
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
        else if (this.UvRotateTop == 2)
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
        else if (this.UvRotateTop == 3)
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

        if (this.EnableAo)
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

    internal readonly void RenderEastFace(Block block, in Vec3D pos, Tessellator tess, in FaceColors colors, int textureId)
    {
        Box blockBb = this.OverrideBounds ?? block.BoundingBox;
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = (texU + blockBb.MinX * 16.0D) / 256.0D;
        double maxU = (texU + blockBb.MaxX * 16.0D - 0.01D) / 256.0D;
        double minV = (texV + 16 - blockBb.MaxY * 16.0D) / 256.0D;
        double maxV = (texV + 16 - blockBb.MinY * 16.0D - 0.01D) / 256.0D;

        if (this.FlipTexture)
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

        if (this.UvRotateEast == 2)
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
        else if (this.UvRotateEast == 1)
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
        else if (this.UvRotateEast == 3)
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

        if (this.EnableAo)
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

    internal readonly void RenderWestFace(Block block, in Vec3D pos, Tessellator tess, in FaceColors colors, int textureId)
    {
        Box blockBb = this.OverrideBounds ?? block.BoundingBox;
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = (texU + blockBb.MinX * 16.0D) / 256.0D;
        double maxU = (texU + blockBb.MaxX * 16.0D - 0.01D) / 256.0D;
        double minV = (texV + 16 - blockBb.MaxY * 16.0D) / 256.0D;
        double maxV = (texV + 16 - blockBb.MinY * 16.0D - 0.01D) / 256.0D;

        if (this.FlipTexture)
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

        if (this.UvRotateWest == 1)
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
        else if (this.UvRotateWest == 2)
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
        else if (this.UvRotateWest == 3)
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

        if (this.EnableAo)
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

    internal readonly void RenderNorthFace(Block block, in Vec3D pos, Tessellator tess, in FaceColors colors, int textureId)
    {
        Box blockBb = this.OverrideBounds ?? block.BoundingBox;
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = (texU + blockBb.MinZ * 16.0D) / 256.0D;
        double maxU = (texU + blockBb.MaxZ * 16.0D - 0.01D) / 256.0D;
        double minV = (texV + 16 - blockBb.MaxY * 16.0D) / 256.0D;
        double maxV = (texV + 16 - blockBb.MinY * 16.0D - 0.01D) / 256.0D;

        if (this.FlipTexture)
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

        if (this.UvRotateNorth == 1)
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
        else if (this.UvRotateNorth == 2)
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
        else if (this.UvRotateNorth == 3)
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

        if (this.EnableAo)
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

    internal readonly void RenderSouthFace(Block block, in Vec3D pos, Tessellator tess, in FaceColors colors, int textureId)
    {
        Box blockBb = this.OverrideBounds ?? block.BoundingBox;

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        double minU = (texU + blockBb.MinZ * 16.0D) / 256.0D;
        double maxU = (texU + blockBb.MaxZ * 16.0D - 0.01D) / 256.0D;
        double minV = (texV + 16 - blockBb.MaxY * 16.0D) / 256.0D;
        double maxV = (texV + 16 - blockBb.MinY * 16.0D - 0.01D) / 256.0D;

        if (this.FlipTexture)
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

        if (this.UvRotateSouth == 2)
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
        else if (this.UvRotateSouth == 1)
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
        else if (this.UvRotateSouth == 3)
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

        if (this.EnableAo)
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
}
