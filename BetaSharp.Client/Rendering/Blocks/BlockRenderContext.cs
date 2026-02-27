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


    internal readonly void DrawBottomFace(Block block, in Vec3D pos, in FaceColors colors, int textureId)
    {
        Box bb = OverrideBounds ?? block.BoundingBox;
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        double bMinX = bb.MinX < 0.0D ? 0.0D : (bb.MinX > 1.0D ? 1.0D : bb.MinX);
        double bMaxX = bb.MaxX < 0.0D ? 0.0D : (bb.MaxX > 1.0D ? 1.0D : bb.MaxX);
        double bMinZ = bb.MinZ < 0.0D ? 0.0D : (bb.MinZ > 1.0D ? 1.0D : bb.MinZ);
        double bMaxZ = bb.MaxZ < 0.0D ? 0.0D : (bb.MaxZ > 1.0D ? 1.0D : bb.MaxZ);

        void GetUV(double h, double v, out double u, out double outV, int uvRotate, bool flipTexture)
        {
            double fU = h, fV = v;
            switch (uvRotate)
            {
                case 1: fU = v; fV = 1.0 - h; break;
                case 2: fU = 1.0 - h; fV = 1.0 - v; break;
                case 3: fU = 1.0 - v; fV = h; break;
                case 4: fU = 1.0 - h; fV = v; break; // mirrored
                case 5: fU = v; fV = h; break;       // mirrored 90
                case 6: fU = h; fV = 1.0 - v; break; // mirrored 180
                case 7: fU = 1.0 - v; fV = 1.0 - h; break; // mirrored 270
            }
            if (flipTexture) fU = 1.0 - fU;
            u = (texU + fU * 16.0D) / 256.0D;
            outV = (texV + fV * 16.0D) / 256.0D;
        }

        GetUV(bMinX, bMaxZ, out double u0, out double v0, UvRotateBottom, FlipTexture);
        GetUV(bMinX, bMinZ, out double u1, out double v1, UvRotateBottom, FlipTexture);
        GetUV(bMaxX, bMinZ, out double u2, out double v2, UvRotateBottom, FlipTexture);
        GetUV(bMaxX, bMaxZ, out double u3, out double v3, UvRotateBottom, FlipTexture);

        double minX = pos.x + bb.MinX; double maxX = pos.x + bb.MaxX;
        double minY = pos.y + bb.MinY; 
        double minZ = pos.z + bb.MinZ; double maxZ = pos.z + bb.MaxZ;

        if (EnableAo)
        {
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            Tess.addVertexWithUV(minX, minY, maxZ, u0, v0);
            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            Tess.addVertexWithUV(minX, minY, minZ, u1, v1);
            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            Tess.addVertexWithUV(maxX, minY, minZ, u2, v2);
            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            Tess.addVertexWithUV(maxX, minY, maxZ, u3, v3);
        }
        else
        {
            Tess.addVertexWithUV(minX, minY, maxZ, u0, v0);
            Tess.addVertexWithUV(minX, minY, minZ, u1, v1);
            Tess.addVertexWithUV(maxX, minY, minZ, u2, v2);
            Tess.addVertexWithUV(maxX, minY, maxZ, u3, v3);
        }
    }

    internal readonly void DrawTopFace(Block block, in Vec3D pos, in FaceColors colors, int textureId)
    {
        Box bb = OverrideBounds ?? block.BoundingBox;
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        double bMinX = bb.MinX < 0.0D ? 0.0D : (bb.MinX > 1.0D ? 1.0D : bb.MinX);
        double bMaxX = bb.MaxX < 0.0D ? 0.0D : (bb.MaxX > 1.0D ? 1.0D : bb.MaxX);
        double bMinZ = bb.MinZ < 0.0D ? 0.0D : (bb.MinZ > 1.0D ? 1.0D : bb.MinZ);
        double bMaxZ = bb.MaxZ < 0.0D ? 0.0D : (bb.MaxZ > 1.0D ? 1.0D : bb.MaxZ);

        void GetUV(double h, double v, out double u, out double outV, int uvRotate, bool flipTexture)
        {
            double fU = h, fV = v;
            switch (uvRotate)
            {
                case 1: fU = v; fV = 1.0 - h; break;
                case 2: fU = 1.0 - h; fV = 1.0 - v; break;
                case 3: fU = 1.0 - v; fV = h; break;
                case 4: fU = 1.0 - h; fV = v; break; // mirrored
                case 5: fU = v; fV = h; break;       // mirrored 90
                case 6: fU = h; fV = 1.0 - v; break; // mirrored 180
                case 7: fU = 1.0 - v; fV = 1.0 - h; break; // mirrored 270
            }
            if (flipTexture) fU = 1.0 - fU;
            u = (texU + fU * 16.0D) / 256.0D;
            outV = (texV + fV * 16.0D) / 256.0D;
        }

        GetUV(bMaxX, bMaxZ, out double u0, out double v0, UvRotateTop, FlipTexture);
        GetUV(bMaxX, bMinZ, out double u1, out double v1, UvRotateTop, FlipTexture);
        GetUV(bMinX, bMinZ, out double u2, out double v2, UvRotateTop, FlipTexture);
        GetUV(bMinX, bMaxZ, out double u3, out double v3, UvRotateTop, FlipTexture);

        double minX = pos.x + bb.MinX; double maxX = pos.x + bb.MaxX;
        double maxY = pos.y + bb.MaxY; 
        double minZ = pos.z + bb.MinZ; double maxZ = pos.z + bb.MaxZ;

        if (EnableAo)
        {
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            Tess.addVertexWithUV(maxX, maxY, maxZ, u0, v0);
            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            Tess.addVertexWithUV(maxX, maxY, minZ, u1, v1);
            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            Tess.addVertexWithUV(minX, maxY, minZ, u2, v2);
            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            Tess.addVertexWithUV(minX, maxY, maxZ, u3, v3);
        }
        else
        {
            Tess.addVertexWithUV(maxX, maxY, maxZ, u0, v0);
            Tess.addVertexWithUV(maxX, maxY, minZ, u1, v1);
            Tess.addVertexWithUV(minX, maxY, minZ, u2, v2);
            Tess.addVertexWithUV(minX, maxY, maxZ, u3, v3);
        }
    }

    internal readonly void DrawNorthFace(Block block, in Vec3D pos, in FaceColors colors, int textureId)
    {
        Box bb = OverrideBounds ?? block.BoundingBox;
        double bMinY = bb.MinY < 0.0 ? 0.0 : (bb.MinY > 1.0 ? 1.0 : bb.MinY);
        double bMaxY = bb.MaxY < 0.0 ? 0.0 : (bb.MaxY > 1.0 ? 1.0 : bb.MaxY);
        double bMinZ = bb.MinZ < 0.0 ? 0.0 : (bb.MinZ > 1.0 ? 1.0 : bb.MinZ);
        double bMaxZ = bb.MaxZ < 0.0 ? 0.0 : (bb.MaxZ > 1.0 ? 1.0 : bb.MaxZ);

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        void GetUV(double h, double v, out double u, out double outV, int uvRotate, bool flipTexture)
        {
            double fU = h, fV = v;
            switch (uvRotate)
            {
                case 1: fU = v; fV = 1.0 - h; break;
                case 2: fU = 1.0 - h; fV = 1.0 - v; break;
                case 3: fU = 1.0 - v; fV = h; break;
                case 4: fU = 1.0 - h; fV = v; break;
                case 5: fU = v; fV = h; break;
                case 6: fU = h; fV = 1.0 - v; break;
                case 7: fU = 1.0 - v; fV = 1.0 - h; break;
            }
            if (flipTexture) fU = 1.0 - fU;
            u = (texU + fU * 16.0) / 256.0;
            outV = (texV + fV * 16.0) / 256.0;
        }

        // X- Face: Left = minZ, Right = maxZ
        GetUV(bMinZ, 1.0 - bMaxY, out double uTL, out double vTL, UvRotateNorth, FlipTexture);
        GetUV(bMinZ, 1.0 - bMinY, out double uBL, out double vBL, UvRotateNorth, FlipTexture);
        GetUV(bMaxZ, 1.0 - bMinY, out double uBR, out double vBR, UvRotateNorth, FlipTexture);
        GetUV(bMaxZ, 1.0 - bMaxY, out double uTR, out double vTR, UvRotateNorth, FlipTexture);

        double minX = pos.x + bb.MinX;
        double minY = pos.y + bb.MinY; double maxY = pos.y + bb.MaxY;
        double minZ = pos.z + bb.MinZ; double maxZ = pos.z + bb.MaxZ;

        if (EnableAo) {
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft); Tess.addVertexWithUV(minX, maxY, minZ, uTL, vTL);
            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft); Tess.addVertexWithUV(minX, minY, minZ, uBL, vBL);
            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight); Tess.addVertexWithUV(minX, minY, maxZ, uBR, vBR);
            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight); Tess.addVertexWithUV(minX, maxY, maxZ, uTR, vTR);
        } else {
            Tess.addVertexWithUV(minX, maxY, minZ, uTL, vTL);
            Tess.addVertexWithUV(minX, minY, minZ, uBL, vBL);
            Tess.addVertexWithUV(minX, minY, maxZ, uBR, vBR);
            Tess.addVertexWithUV(minX, maxY, maxZ, uTR, vTR);
        }
    }

    internal readonly void DrawSouthFace(Block block, in Vec3D pos, in FaceColors colors, int textureId)
    {
        Box bb = OverrideBounds ?? block.BoundingBox;
        double bMinY = bb.MinY < 0.0 ? 0.0 : (bb.MinY > 1.0 ? 1.0 : bb.MinY);
        double bMaxY = bb.MaxY < 0.0 ? 0.0 : (bb.MaxY > 1.0 ? 1.0 : bb.MaxY);
        double bMinZ = bb.MinZ < 0.0 ? 0.0 : (bb.MinZ > 1.0 ? 1.0 : bb.MinZ);
        double bMaxZ = bb.MaxZ < 0.0 ? 0.0 : (bb.MaxZ > 1.0 ? 1.0 : bb.MaxZ);

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        void GetUV(double h, double v, out double u, out double outV, int uvRotate, bool flipTexture)
        {
            double fU = h, fV = v;
            switch (uvRotate)
            {
                case 1: fU = v; fV = 1.0 - h; break;
                case 2: fU = 1.0 - h; fV = 1.0 - v; break;
                case 3: fU = 1.0 - v; fV = h; break;
                case 4: fU = 1.0 - h; fV = v; break;
                case 5: fU = v; fV = h; break;
                case 6: fU = h; fV = 1.0 - v; break;
                case 7: fU = 1.0 - v; fV = 1.0 - h; break;
            }
            if (flipTexture) fU = 1.0 - fU;
            u = (texU + fU * 16.0) / 256.0;
            outV = (texV + fV * 16.0) / 256.0;
        }

        // X+ Face: Left = maxZ, Right = minZ
        GetUV(1.0 - bMaxZ, 1.0 - bMaxY, out double uTL, out double vTL, UvRotateSouth, FlipTexture);
        GetUV(1.0 - bMaxZ, 1.0 - bMinY, out double uBL, out double vBL, UvRotateSouth, FlipTexture);
        GetUV(1.0 - bMinZ, 1.0 - bMinY, out double uBR, out double vBR, UvRotateSouth, FlipTexture);
        GetUV(1.0 - bMinZ, 1.0 - bMaxY, out double uTR, out double vTR, UvRotateSouth, FlipTexture);

        double posX = pos.x + bb.MaxX;
        double minY = pos.y + bb.MinY; double maxY = pos.y + bb.MaxY;
        double minZ = pos.z + bb.MinZ; double maxZ = pos.z + bb.MaxZ;

        if (EnableAo) {
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft); Tess.addVertexWithUV(posX, maxY, maxZ, uTL, vTL);
            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft); Tess.addVertexWithUV(posX, minY, maxZ, uBL, vBL);
            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight); Tess.addVertexWithUV(posX, minY, minZ, uBR, vBR);
            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight); Tess.addVertexWithUV(posX, maxY, minZ, uTR, vTR);
        } else {
            Tess.addVertexWithUV(posX, maxY, maxZ, uTL, vTL);
            Tess.addVertexWithUV(posX, minY, maxZ, uBL, vBL);
            Tess.addVertexWithUV(posX, minY, minZ, uBR, vBR);
            Tess.addVertexWithUV(posX, maxY, minZ, uTR, vTR);
        }
    }

    internal readonly void DrawEastFace(Block block, in Vec3D pos, in FaceColors colors, int textureId)
    {
        Box bb = OverrideBounds ?? block.BoundingBox;
        double bMinY = bb.MinY < 0.0 ? 0.0 : (bb.MinY > 1.0 ? 1.0 : bb.MinY);
        double bMaxY = bb.MaxY < 0.0 ? 0.0 : (bb.MaxY > 1.0 ? 1.0 : bb.MaxY);
        double bMinX = bb.MinX < 0.0 ? 0.0 : (bb.MinX > 1.0 ? 1.0 : bb.MinX);
        double bMaxX = bb.MaxX < 0.0 ? 0.0 : (bb.MaxX > 1.0 ? 1.0 : bb.MaxX);

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        void GetUV(double h, double v, out double u, out double outV, int uvRotate, bool flipTexture)
        {
            double fU = h, fV = v;
            switch (uvRotate)
            {
                case 1: fU = v; fV = 1.0 - h; break;
                case 2: fU = 1.0 - h; fV = 1.0 - v; break;
                case 3: fU = 1.0 - v; fV = h; break;
                case 4: fU = 1.0 - h; fV = v; break;
                case 5: fU = v; fV = h; break;
                case 6: fU = h; fV = 1.0 - v; break;
                case 7: fU = 1.0 - v; fV = 1.0 - h; break;
            }
            if (flipTexture) fU = 1.0 - fU;
            u = (texU + fU * 16.0) / 256.0;
            outV = (texV + fV * 16.0) / 256.0;
        }

        // Z- Face: Left = maxX, Right = minX
        GetUV(1.0 - bMaxX, 1.0 - bMaxY, out double uTL, out double vTL, UvRotateEast, FlipTexture);
        GetUV(1.0 - bMaxX, 1.0 - bMinY, out double uBL, out double vBL, UvRotateEast, FlipTexture);
        GetUV(1.0 - bMinX, 1.0 - bMinY, out double uBR, out double vBR, UvRotateEast, FlipTexture);
        GetUV(1.0 - bMinX, 1.0 - bMaxY, out double uTR, out double vTR, UvRotateEast, FlipTexture);

        double minX = pos.x + bb.MinX; double maxX = pos.x + bb.MaxX;
        double minY = pos.y + bb.MinY; double maxY = pos.y + bb.MaxY;
        double minZ = pos.z + bb.MinZ;

        if (EnableAo) {
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft); Tess.addVertexWithUV(maxX, maxY, minZ, uTL, vTL);
            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft); Tess.addVertexWithUV(maxX, minY, minZ, uBL, vBL);
            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight); Tess.addVertexWithUV(minX, minY, minZ, uBR, vBR);
            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight); Tess.addVertexWithUV(minX, maxY, minZ, uTR, vTR);
        } else {
            Tess.addVertexWithUV(maxX, maxY, minZ, uTL, vTL);
            Tess.addVertexWithUV(maxX, minY, minZ, uBL, vBL);
            Tess.addVertexWithUV(minX, minY, minZ, uBR, vBR);
            Tess.addVertexWithUV(minX, maxY, minZ, uTR, vTR);
        }
    }

    internal readonly void DrawWestFace(Block block, in Vec3D pos, in FaceColors colors, int textureId)
    {
        Box bb = OverrideBounds ?? block.BoundingBox;
        double bMinY = bb.MinY < 0.0 ? 0.0 : (bb.MinY > 1.0 ? 1.0 : bb.MinY);
        double bMaxY = bb.MaxY < 0.0 ? 0.0 : (bb.MaxY > 1.0 ? 1.0 : bb.MaxY);
        double bMinX = bb.MinX < 0.0 ? 0.0 : (bb.MinX > 1.0 ? 1.0 : bb.MinX);
        double bMaxX = bb.MaxX < 0.0 ? 0.0 : (bb.MaxX > 1.0 ? 1.0 : bb.MaxX);

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        void GetUV(double h, double v, out double u, out double outV, int uvRotate, bool flipTexture)
        {
            double fU = h, fV = v;
            switch (uvRotate)
            {
                case 1: fU = v; fV = 1.0 - h; break;
                case 2: fU = 1.0 - h; fV = 1.0 - v; break;
                case 3: fU = 1.0 - v; fV = h; break;
                case 4: fU = 1.0 - h; fV = v; break;
                case 5: fU = v; fV = h; break;
                case 6: fU = h; fV = 1.0 - v; break;
                case 7: fU = 1.0 - v; fV = 1.0 - h; break;
            }
            if (flipTexture) fU = 1.0 - fU;
            u = (texU + fU * 16.0) / 256.0;
            outV = (texV + fV * 16.0) / 256.0;
        }

        // Z+ Face: Left = minX, Right = maxX
        GetUV(bMinX, 1.0 - bMaxY, out double uTL, out double vTL, UvRotateWest, FlipTexture);
        GetUV(bMinX, 1.0 - bMinY, out double uBL, out double vBL, UvRotateWest, FlipTexture);
        GetUV(bMaxX, 1.0 - bMinY, out double uBR, out double vBR, UvRotateWest, FlipTexture);
        GetUV(bMaxX, 1.0 - bMaxY, out double uTR, out double vTR, UvRotateWest, FlipTexture);

        double minX = pos.x + bb.MinX; double maxX = pos.x + bb.MaxX;
        double minY = pos.y + bb.MinY; double maxY = pos.y + bb.MaxY;
        double maxZ = pos.z + bb.MaxZ;

        if (EnableAo) {
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft); Tess.addVertexWithUV(minX, maxY, maxZ, uTL, vTL);
            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft); Tess.addVertexWithUV(minX, minY, maxZ, uBL, vBL);
            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight); Tess.addVertexWithUV(maxX, minY, maxZ, uBR, vBR);
            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight); Tess.addVertexWithUV(maxX, maxY, maxZ, uTR, vTR);
        } else {
            Tess.addVertexWithUV(minX, maxY, maxZ, uTL, vTL);
            Tess.addVertexWithUV(minX, minY, maxZ, uBL, vBL);
            Tess.addVertexWithUV(maxX, minY, maxZ, uBR, vBR);
            Tess.addVertexWithUV(maxX, maxY, maxZ, uTR, vTR);
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

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, 1, 0, 0, 0.8F, tintWest); // Z+ (South)
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

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, 1, 1, 0, 0.6F, tintNorth); // X- (West)
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

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, 0, 1, 0, 0.6F, tintSouth); // X+ (East)
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
