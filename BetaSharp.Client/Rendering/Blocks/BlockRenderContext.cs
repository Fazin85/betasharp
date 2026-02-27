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

    internal readonly void DrawEastFace(Block block, in Vec3D pos, in FaceColors colors, int textureId)
    {
        Box bb = OverrideBounds ?? block.BoundingBox;
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        double bMinX = bb.MinX < 0.0D ? 0.0D : (bb.MinX > 1.0D ? 1.0D : bb.MinX);
        double bMaxX = bb.MaxX < 0.0D ? 0.0D : (bb.MaxX > 1.0D ? 1.0D : bb.MaxX);
        double bMinY = bb.MinY < 0.0D ? 0.0D : (bb.MinY > 1.0D ? 1.0D : bb.MinY);
        double bMaxY = bb.MaxY < 0.0D ? 0.0D : (bb.MaxY > 1.0D ? 1.0D : bb.MaxY);

        double u0 = (texU + bMinX * 16.0D) / 256.0D;
        double u1 = (texU + bMaxX * 16.0D - 0.01D) / 256.0D;
        double v0 = (texV + 16.0D - bMaxY * 16.0D) / 256.0D;
        double v1 = (texV + 16.0D - bMinY * 16.0D - 0.01D) / 256.0D;

        if (FlipTexture) { (u0, u1) = (u1, u0); }

        double tlU = 0, tlV = 0, trU = 0, trV = 0, blU = 0, blV = 0, brU = 0, brV = 0;

        // 8 Absolute Rotation States
        switch (UvRotateEast)
        {
            case 0: tlU=u0; tlV=v0; trU=u1; trV=v0; blU=u0; blV=v1; brU=u1; brV=v1; break;
            case 1: tlU=u0; tlV=v1; trU=u0; trV=v0; blU=u1; blV=v1; brU=u1; brV=v0; break;
            case 2: tlU=u1; tlV=v1; trU=u0; trV=v1; blU=u1; blV=v0; brU=u0; brV=v0; break;
            case 3: tlU=u1; tlV=v0; trU=u1; trV=v1; blU=u0; blV=v0; brU=u0; brV=v1; break;
            case 4: tlU=u1; tlV=v0; trU=u0; trV=v0; blU=u1; blV=v1; brU=u0; brV=v1; break; // mirrored
            case 5: tlU=u1; tlV=v1; trU=u1; trV=v0; blU=u0; blV=v1; brU=u0; brV=v0; break; // mirrored
            case 6: tlU=u0; tlV=v1; trU=u1; trV=v1; blU=u0; blV=v0; brU=u1; brV=v0; break; // mirrored
            case 7: tlU=u0; tlV=v0; trU=u0; trV=v1; blU=u1; blV=v0; brU=u1; brV=v1; break; // mirrored
        }

        double minX = pos.x + bb.MinX; double maxX = pos.x + bb.MaxX;
        double minY = pos.y + bb.MinY; double maxY = pos.y + bb.MaxY;
        double minZ = pos.z + bb.MinZ;

        // Looking at Z-. Left side is maxX, Right side is minX.
        if (EnableAo)
        {
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            Tess.addVertexWithUV(minX, maxY, minZ, trU, trV); // Top Right

            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            Tess.addVertexWithUV(maxX, maxY, minZ, tlU, tlV); // Top Left

            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            Tess.addVertexWithUV(maxX, minY, minZ, blU, blV); // Bottom Left

            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            Tess.addVertexWithUV(minX, minY, minZ, brU, brV); // Bottom Right
        }
        else
        {
            Tess.addVertexWithUV(minX, maxY, minZ, trU, trV);
            Tess.addVertexWithUV(maxX, maxY, minZ, tlU, tlV);
            Tess.addVertexWithUV(maxX, minY, minZ, blU, blV);
            Tess.addVertexWithUV(minX, minY, minZ, brU, brV);
        }
    }

    internal readonly void DrawWestFace(Block block, in Vec3D pos, in FaceColors colors, int textureId)
    {
        Box bb = OverrideBounds ?? block.BoundingBox;
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        double bMinX = bb.MinX < 0.0D ? 0.0D : (bb.MinX > 1.0D ? 1.0D : bb.MinX);
        double bMaxX = bb.MaxX < 0.0D ? 0.0D : (bb.MaxX > 1.0D ? 1.0D : bb.MaxX);
        double bMinY = bb.MinY < 0.0D ? 0.0D : (bb.MinY > 1.0D ? 1.0D : bb.MinY);
        double bMaxY = bb.MaxY < 0.0D ? 0.0D : (bb.MaxY > 1.0D ? 1.0D : bb.MaxY);

        double u0 = (texU + bMinX * 16.0D) / 256.0D;
        double u1 = (texU + bMaxX * 16.0D - 0.01D) / 256.0D;
        double v0 = (texV + 16.0D - bMaxY * 16.0D) / 256.0D;
        double v1 = (texV + 16.0D - bMinY * 16.0D - 0.01D) / 256.0D;

        if (FlipTexture) { (u0, u1) = (u1, u0); }

        double tlU = 0, tlV = 0, trU = 0, trV = 0, blU = 0, blV = 0, brU = 0, brV = 0;

        // 8 Absolute Rotation States
        switch (UvRotateWest)
        {
            case 0: tlU=u0; tlV=v0; trU=u1; trV=v0; blU=u0; blV=v1; brU=u1; brV=v1; break;
            case 1: tlU=u0; tlV=v1; trU=u0; trV=v0; blU=u1; blV=v1; brU=u1; brV=v0; break;
            case 2: tlU=u1; tlV=v1; trU=u0; trV=v1; blU=u1; blV=v0; brU=u0; brV=v0; break;
            case 3: tlU=u1; tlV=v0; trU=u1; trV=v1; blU=u0; blV=v0; brU=u0; brV=v1; break;
            case 4: tlU=u1; tlV=v0; trU=u0; trV=v0; blU=u1; blV=v1; brU=u0; brV=v1; break; // mirrored
            case 5: tlU=u1; tlV=v1; trU=u1; trV=v0; blU=u0; blV=v1; brU=u0; brV=v0; break; // mirrored
            case 6: tlU=u0; tlV=v1; trU=u1; trV=v1; blU=u0; blV=v0; brU=u1; brV=v0; break; // mirrored
            case 7: tlU=u0; tlV=v0; trU=u0; trV=v1; blU=u1; blV=v0; brU=u1; brV=v1; break; // mirrored
        }

        double minX = pos.x + bb.MinX; double maxX = pos.x + bb.MaxX;
        double minY = pos.y + bb.MinY; double maxY = pos.y + bb.MaxY;
        double maxZ = pos.z + bb.MaxZ;

        // Looking at Z+. Left side is minX, Right side is maxX.
        if (EnableAo)
        {
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            Tess.addVertexWithUV(minX, maxY, maxZ, tlU, tlV); // Top Left

            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            Tess.addVertexWithUV(minX, minY, maxZ, blU, blV); // Bottom Left

            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            Tess.addVertexWithUV(maxX, minY, maxZ, brU, brV); // Bottom Right

            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            Tess.addVertexWithUV(maxX, maxY, maxZ, trU, trV); // Top Right
        }
        else
        {
            Tess.addVertexWithUV(minX, maxY, maxZ, tlU, tlV);
            Tess.addVertexWithUV(minX, minY, maxZ, blU, blV);
            Tess.addVertexWithUV(maxX, minY, maxZ, brU, brV);
            Tess.addVertexWithUV(maxX, maxY, maxZ, trU, trV);
        }
    }


internal readonly void DrawSouthFace(Block block, in Vec3D pos, in FaceColors colors, int textureId)
    {
        Box bb = OverrideBounds ?? block.BoundingBox;
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        double bMinZ = bb.MinZ < 0.0D ? 0.0D : (bb.MinZ > 1.0D ? 1.0D : bb.MinZ);
        double bMaxZ = bb.MaxZ < 0.0D ? 0.0D : (bb.MaxZ > 1.0D ? 1.0D : bb.MaxZ);
        double bMinY = bb.MinY < 0.0D ? 0.0D : (bb.MinY > 1.0D ? 1.0D : bb.MinY);
        double bMaxY = bb.MaxY < 0.0D ? 0.0D : (bb.MaxY > 1.0D ? 1.0D : bb.MaxY);

        double u0 = (texU + bMinZ * 16.0D) / 256.0D;
        double u1 = (texU + bMaxZ * 16.0D - 0.01D) / 256.0D;
        double v0 = (texV + 16.0D - bMaxY * 16.0D) / 256.0D;
        double v1 = (texV + 16.0D - bMinY * 16.0D - 0.01D) / 256.0D;

        if (FlipTexture) { (u0, u1) = (u1, u0); }

        double tlU = 0, tlV = 0, trU = 0, trV = 0, blU = 0, blV = 0, brU = 0, brV = 0;

        // 8 Absolute Rotation States
        switch (UvRotateSouth)
        {
            case 0: tlU=u0; tlV=v0; trU=u1; trV=v0; blU=u0; blV=v1; brU=u1; brV=v1; break;
            case 1: tlU=u0; tlV=v1; trU=u0; trV=v0; blU=u1; blV=v1; brU=u1; brV=v0; break;
            case 2: tlU=u1; tlV=v1; trU=u0; trV=v1; blU=u1; blV=v0; brU=u0; brV=v0; break;
            case 3: tlU=u1; tlV=v0; trU=u1; trV=v1; blU=u0; blV=v0; brU=u0; brV=v1; break;
            case 4: tlU=u1; tlV=v0; trU=u0; trV=v0; blU=u1; blV=v1; brU=u0; brV=v1; break; // mirrored
            case 5: tlU=u1; tlV=v1; trU=u1; trV=v0; blU=u0; blV=v1; brU=u0; brV=v0; break; // mirrored
            case 6: tlU=u0; tlV=v1; trU=u1; trV=v1; blU=u0; blV=v0; brU=u1; brV=v0; break; // mirrored
            case 7: tlU=u0; tlV=v0; trU=u0; trV=v1; blU=u1; blV=v0; brU=u1; brV=v1; break; // mirrored
        }

        double posX = pos.x + bb.MaxX; 
        double minY = pos.y + bb.MinY; double maxY = pos.y + bb.MaxY;
        double minZ = pos.z + bb.MinZ; double maxZ = pos.z + bb.MaxZ;

        // Looking at X+. Left side is maxZ, Right side is minZ.
        if (EnableAo)
        {
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            Tess.addVertexWithUV(posX, maxY, maxZ, tlU, tlV); // Top Left

            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            Tess.addVertexWithUV(posX, minY, maxZ, blU, blV); // Bottom Left

            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            Tess.addVertexWithUV(posX, minY, minZ, brU, brV); // Bottom Right

            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            Tess.addVertexWithUV(posX, maxY, minZ, trU, trV); // Top Right
        }
        else
        {
            Tess.addVertexWithUV(posX, maxY, maxZ, tlU, tlV);
            Tess.addVertexWithUV(posX, minY, maxZ, blU, blV);
            Tess.addVertexWithUV(posX, minY, minZ, brU, brV);
            Tess.addVertexWithUV(posX, maxY, minZ, trU, trV);
        }
    }

    internal readonly void DrawNorthFace(Block block, in Vec3D pos, in FaceColors colors, int textureId)
    {
        Box bb = OverrideBounds ?? block.BoundingBox;
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        double bMinZ = bb.MinZ < 0.0D ? 0.0D : (bb.MinZ > 1.0D ? 1.0D : bb.MinZ);
        double bMaxZ = bb.MaxZ < 0.0D ? 0.0D : (bb.MaxZ > 1.0D ? 1.0D : bb.MaxZ);
        double bMinY = bb.MinY < 0.0D ? 0.0D : (bb.MinY > 1.0D ? 1.0D : bb.MinY);
        double bMaxY = bb.MaxY < 0.0D ? 0.0D : (bb.MaxY > 1.0D ? 1.0D : bb.MaxY);

        double u0 = (texU + bMinZ * 16.0D) / 256.0D;
        double u1 = (texU + bMaxZ * 16.0D - 0.01D) / 256.0D;
        double v0 = (texV + 16.0D - bMaxY * 16.0D) / 256.0D;
        double v1 = (texV + 16.0D - bMinY * 16.0D - 0.01D) / 256.0D;

        if (FlipTexture) { (u0, u1) = (u1, u0); }

        double tlU = 0, tlV = 0, trU = 0, trV = 0, blU = 0, blV = 0, brU = 0, brV = 0;

        // 8 Absolute Rotation States
        switch (UvRotateNorth)
        {
            case 0: tlU=u0; tlV=v0; trU=u1; trV=v0; blU=u0; blV=v1; brU=u1; brV=v1; break; // 0
            case 1: tlU=u0; tlV=v1; trU=u0; trV=v0; blU=u1; blV=v1; brU=u1; brV=v0; break; // 90
            case 2: tlU=u1; tlV=v1; trU=u0; trV=v1; blU=u1; blV=v0; brU=u0; brV=v0; break; // 180
            case 3: tlU=u1; tlV=v0; trU=u1; trV=v1; blU=u0; blV=v0; brU=u0; brV=v1; break; // 270
            case 4: tlU=u1; tlV=v0; trU=u0; trV=v0; blU=u1; blV=v1; brU=u0; brV=v1; break; // mirrored
            case 5: tlU=u1; tlV=v1; trU=u1; trV=v0; blU=u0; blV=v1; brU=u0; brV=v0; break; // mirrored
            case 6: tlU=u0; tlV=v1; trU=u1; trV=v1; blU=u0; blV=v0; brU=u1; brV=v0; break; // mirrored
            case 7: tlU=u0; tlV=v0; trU=u0; trV=v1; blU=u1; blV=v0; brU=u1; brV=v1; break; // mirrored
        }

        double minX = pos.x + bb.MinX;
        double minY = pos.y + bb.MinY; double maxY = pos.y + bb.MaxY;
        double minZ = pos.z + bb.MinZ; double maxZ = pos.z + bb.MaxZ;

        if (EnableAo)
        {
            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            Tess.addVertexWithUV(minX, maxY, minZ, tlU, tlV); // Top Left
            
            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            Tess.addVertexWithUV(minX, minY, minZ, blU, blV); // Bottom Left
            
            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            Tess.addVertexWithUV(minX, minY, maxZ, brU, brV); // Bottom Right
            
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            Tess.addVertexWithUV(minX, maxY, maxZ, trU, trV); // Top Right
        }
        else
        {
            Tess.addVertexWithUV(minX, maxY, minZ, tlU, tlV);
            Tess.addVertexWithUV(minX, minY, minZ, blU, blV);
            Tess.addVertexWithUV(minX, minY, maxZ, brU, brV);
            Tess.addVertexWithUV(minX, maxY, maxZ, trU, trV);
        }
    }

    internal readonly bool DrawBlock(in Block block, in BlockPos pos)
    {
        bool hasRendered = false;
        Box bounds = OverrideBounds ?? block.BoundingBox;

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

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, 0, 0, 1, 0.8F, tintEast);
            int textureId = OverrideTexture >= 0 ? OverrideTexture : block.getTextureId(World, pos.x, pos.y, pos.z, 2);
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

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, 1, 0, 0, 0.8F, tintWest); // RED = Z+ (South)
            int textureId = OverrideTexture >= 0 ? OverrideTexture : block.getTextureId(World, pos.x, pos.y, pos.z, 3);
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

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, 1, 1, 0, 0.6F, tintNorth); // YELLOW = X- (West)
            int textureId = OverrideTexture >= 0 ? OverrideTexture : block.getTextureId(World, pos.x, pos.y, pos.z, 4);
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

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, 0, 1, 0, 0.6F, tintSouth); // GREEN = X+ (East)
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
