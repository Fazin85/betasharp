using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public static class Helper
{
    internal static void RenderBottomFace(Block block, in Vec3D pos, Tessellator tess, in BlockRenderContext context, in FaceColors colors, int textureId, bool flipTexture)
    {
        Box blockBb = context.OverrideBounds ?? block.BoundingBox;
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

        if (context.UvRotateBottom == 2)
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
        else if (context.UvRotateBottom == 1)
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
        else if (context.UvRotateBottom == 3)
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

        if (context.EnableAo)
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

    internal static void RenderTopFace(Block block, in Vec3D pos, Tessellator tess, in BlockRenderContext context, in FaceColors colors, int textureId, bool flipTexture)
    {
        Box blockBb = context.OverrideBounds ?? block.BoundingBox;
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

        if (context.UvRotateTop == 1)
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
        else if (context.UvRotateTop == 2)
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
        else if (context.UvRotateTop == 3)
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

        if (context.EnableAo)
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

    internal static void RenderEastFace(Block block, in Vec3D pos, Tessellator tess, in BlockRenderContext context, in FaceColors colors, int textureId, bool flipTexture)
    {
        Box blockBb = context.OverrideBounds ?? block.BoundingBox;
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = (texU + blockBb.MinX * 16.0D) / 256.0D;
        double maxU = (texU + blockBb.MaxX * 16.0D - 0.01D) / 256.0D;
        double minV = (texV + 16 - blockBb.MaxY * 16.0D) / 256.0D;
        double maxV = (texV + 16 - blockBb.MinY * 16.0D - 0.01D) / 256.0D;

        if (flipTexture)
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

        if (context.UvRotateEast == 2)
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
        else if (context.UvRotateEast == 1)
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
        else if (context.UvRotateEast == 3)
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

        if (context.EnableAo)
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

    internal static void RenderWestFace(Block block, in Vec3D pos, Tessellator tess, in BlockRenderContext context, in FaceColors colors, int textureId, bool flipTexture)
    {
        Box blockBb = context.OverrideBounds ?? block.BoundingBox;
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = (texU + blockBb.MinX * 16.0D) / 256.0D;
        double maxU = (texU + blockBb.MaxX * 16.0D - 0.01D) / 256.0D;
        double minV = (texV + 16 - blockBb.MaxY * 16.0D) / 256.0D;
        double maxV = (texV + 16 - blockBb.MinY * 16.0D - 0.01D) / 256.0D;

        if (flipTexture)
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

        if (context.UvRotateWest == 1)
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
        else if (context.UvRotateWest == 2)
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
        else if (context.UvRotateWest == 3)
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

        if (context.EnableAo)
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

    internal static void RenderNorthFace(Block block, in Vec3D pos, Tessellator tess, in BlockRenderContext context, in FaceColors colors, int textureId, bool flipTexture)
    {

        Box blockBb = context.OverrideBounds ?? block.BoundingBox;
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

        if (context.UvRotateNorth == 1)
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
        else if (context.UvRotateNorth == 2)
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
        else if (context.UvRotateNorth == 3)
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

        if (context.EnableAo)
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

    internal static void RenderSouthFace(Block block, in Vec3D pos, Tessellator tess, in BlockRenderContext context, in FaceColors colors, int textureId, bool flipTexture)
    {
        Box blockBb = context.OverrideBounds ?? block.BoundingBox;

        int texU = (context.OverrideTexture & 15) << 4;
        int texV = context.OverrideTexture & 240;

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

        if (context.UvRotateSouth == 2)
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
        else if (context.UvRotateSouth == 1)
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
        else if (context.UvRotateSouth == 3)
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

        if (context.EnableAo)
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

    internal static void  RenderTorchAtAngle(in Block block, in Tessellator tess, in Vec3D pos, double tiltX, double tiltZ,
        in BlockRenderContext context)
    {
        int textureId = block.getTexture(0);
        if (context.OverrideTexture >= 0)
        {
            textureId = context.OverrideTexture;
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
