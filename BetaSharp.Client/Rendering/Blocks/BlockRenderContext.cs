using System.Runtime.CompilerServices;
using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks;

public ref struct BlockRenderContext
{
    public readonly IBlockAccess World;
    public readonly Tessellator Tess;

    public int OverrideTexture;
    public readonly bool RenderAllFaces;
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
        IBlockAccess world, Tessellator tess,
        int overrideTexture = -1, bool renderAllFaces = false,
        bool flipTexture = false, Box? bounds = null,
        int uvTop = 0, int uvBottom = 0,
        int uvNorth = 0, int uvSouth = 0,
        int uvEast = 0, int uvWest = 0,
        bool customFlag = false, bool enableAo = true,
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

        float bMinX = bb.MinX < 0.0D ? 0.0F : (float)(bb.MinX > 1.0F ? 1.0F : bb.MinX);
        float bMaxX = bb.MaxX < 0.0D ? 0.0F : (float)(bb.MaxX > 1.0F ? 1.0F : bb.MaxX);
        float bMinZ = bb.MinZ < 0.0D ? 0.0F : (float)(bb.MinZ > 1.0F ? 1.0F : bb.MinZ);
        float bMaxZ = bb.MaxZ < 0.0D ? 0.0F : (float)(bb.MaxZ > 1.0F ? 1.0F : bb.MaxZ);

        CalculateUv(bMinX, bMaxZ, UvRotateBottom, texU, texV, out float u0, out float v0);
        CalculateUv(bMinX, bMinZ, UvRotateBottom, texU, texV, out float u1, out float v1);
        CalculateUv(bMaxX, bMinZ, UvRotateBottom, texU, texV, out float u2, out float v2);
        CalculateUv(bMaxX, bMaxZ, UvRotateBottom, texU, texV, out float u3, out float v3);

        float minX = (float)(pos.x + bb.MinX);
        float maxX = (float)(pos.x + bb.MaxX);
        float minY = (float)(pos.y + bb.MinY);
        float minZ = (float)(pos.z + bb.MinZ);
        float maxZ = (float)(pos.z + bb.MaxZ);

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

        float bMinX = bb.MinX < 0.0D ? 0.0F : (float)(bb.MinX > 1.0F ? 1.0F : bb.MinX);
        float bMaxX = bb.MaxX < 0.0D ? 0.0F : (float)(bb.MaxX > 1.0F ? 1.0F : bb.MaxX);
        float bMinZ = bb.MinZ < 0.0D ? 0.0F : (float)(bb.MinZ > 1.0F ? 1.0F : bb.MinZ);
        float bMaxZ = bb.MaxZ < 0.0D ? 0.0F : (float)(bb.MaxZ > 1.0F ? 1.0F : bb.MaxZ);

        CalculateUv(bMaxX, bMaxZ, UvRotateTop, texU, texV, out float u0, out float v0);
        CalculateUv(bMaxX, bMinZ, UvRotateTop, texU, texV, out float u1, out float v1);
        CalculateUv(bMinX, bMinZ, UvRotateTop, texU, texV, out float u2, out float v2);
        CalculateUv(bMinX, bMaxZ, UvRotateTop, texU, texV, out float u3, out float v3);

        float minX = (float)(pos.x + bb.MinX);
        float maxX = (float)(pos.x + bb.MaxX);
        float maxY = (float)(pos.y + bb.MaxY);
        float minZ = (float)(pos.z + bb.MinZ);
        float maxZ = (float)(pos.z + bb.MaxZ);

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
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        CalculateUv((float)bb.MinZ, 1.0F - (float)bb.MaxY, UvRotateNorth, texU, texV, out float uTl, out float vTl);
        CalculateUv((float)bb.MinZ, 1.0F - (float)bb.MinY, UvRotateNorth, texU, texV, out float uBl, out float vBl);
        CalculateUv((float)bb.MaxZ, 1.0F - (float)bb.MinY, UvRotateNorth, texU, texV, out float uBr, out float vBr);
        CalculateUv((float)bb.MaxZ, 1.0F - (float)bb.MaxY, UvRotateNorth, texU, texV, out float uTr, out float vTr);

        float minX = (float)(pos.x + bb.MinX);
        float minY = (float)(pos.y + bb.MinY);
        float maxY = (float)(pos.y + bb.MaxY);
        float minZ = (float)(pos.z + bb.MinZ);
        float maxZ = (float)(pos.z + bb.MaxZ);

        if (EnableAo)
        {
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            Tess.addVertexWithUV(minX, maxY, minZ, uTl, vTl);

            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            Tess.addVertexWithUV(minX, minY, minZ, uBl, vBl);

            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            Tess.addVertexWithUV(minX, minY, maxZ, uBr, vBr);

            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            Tess.addVertexWithUV(minX, maxY, maxZ, uTr, vTr);
        }
        else
        {
            Tess.addVertexWithUV(minX, maxY, minZ, uTl, vTl);
            Tess.addVertexWithUV(minX, minY, minZ, uBl, vBl);
            Tess.addVertexWithUV(minX, minY, maxZ, uBr, vBr);
            Tess.addVertexWithUV(minX, maxY, maxZ, uTr, vTr);
        }
    }

    internal readonly void DrawSouthFace(Block block, in Vec3D pos, in FaceColors colors, int textureId)
    {
        Box bb = OverrideBounds ?? block.BoundingBox;
        float bMinY = bb.MinY < 0.0D ? 0.0F : (float)(bb.MinY > 1.0 ? 1.0 : bb.MinY);
        float bMaxY = bb.MaxY < 0.0D ? 0.0F : (float)(bb.MaxY > 1.0 ? 1.0 : bb.MaxY);
        float bMinZ = bb.MinZ < 0.0D ? 0.0F : (float)(bb.MinZ > 1.0 ? 1.0 : bb.MinZ);
        float bMaxZ = bb.MaxZ < 0.0D ? 0.0F : (float)(bb.MaxZ > 1.0 ? 1.0 : bb.MaxZ);

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        // X+ Face: Left = maxZ, Right = minZ
        CalculateUv(1.0F - bMaxZ, 1.0F - bMaxY, UvRotateSouth, texU, texV, out float uTl, out float vTl);
        CalculateUv(1.0F - bMaxZ, 1.0F - bMinY, UvRotateSouth, texU, texV, out float uBl, out float vBl);
        CalculateUv(1.0F - bMinZ, 1.0F - bMinY, UvRotateSouth, texU, texV, out float uBr, out float vBr);
        CalculateUv(1.0F - bMinZ, 1.0F - bMaxY, UvRotateSouth, texU, texV, out float uTr, out float vTr);

        float posX = (float)(pos.x + bb.MaxX);
        float minY = (float)(pos.y + bb.MinY);
        float maxY = (float)(pos.y + bb.MaxY);
        float minZ = (float)(pos.z + bb.MinZ);
        float maxZ = (float)(pos.z + bb.MaxZ);

        if (EnableAo)
        {
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            Tess.addVertexWithUV(posX, maxY, maxZ, uTl, vTl);
            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            Tess.addVertexWithUV(posX, minY, maxZ, uBl, vBl);
            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            Tess.addVertexWithUV(posX, minY, minZ, uBr, vBr);
            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            Tess.addVertexWithUV(posX, maxY, minZ, uTr, vTr);
        }
        else
        {
            Tess.addVertexWithUV(posX, maxY, maxZ, uTl, vTl);
            Tess.addVertexWithUV(posX, minY, maxZ, uBl, vBl);
            Tess.addVertexWithUV(posX, minY, minZ, uBr, vBr);
            Tess.addVertexWithUV(posX, maxY, minZ, uTr, vTr);
        }
    }

    internal readonly void DrawEastFace(Block block, in Vec3D pos, in FaceColors colors, int textureId)
    {
        Box bb = OverrideBounds ?? block.BoundingBox;
        float bMinY = bb.MinY < 0.0D ? 0.0F : (float)(bb.MinY > 1.0 ? 1.0 : bb.MinY);
        float bMaxY = bb.MaxY < 0.0D ? 0.0F : (float)(bb.MaxY > 1.0 ? 1.0 : bb.MaxY);
        float bMinX = bb.MinX < 0.0D ? 0.0F : (float)(bb.MinX > 1.0 ? 1.0 : bb.MinX);
        float bMaxX = bb.MaxX < 0.0D ? 0.0F : (float)(bb.MaxX > 1.0 ? 1.0 : bb.MaxX);

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        // Z- Face: Left = maxX, Right = minX
        CalculateUv(1.0F - bMaxX, 1.0F - bMaxY, UvRotateEast, texU, texV, out float uTl, out float vTl);
        CalculateUv(1.0F - bMaxX, 1.0F - bMinY, UvRotateEast, texU, texV, out float uBl, out float vBl);
        CalculateUv(1.0F - bMinX, 1.0F - bMinY, UvRotateEast, texU, texV, out float uBr, out float vBr);
        CalculateUv(1.0F - bMinX, 1.0F - bMaxY, UvRotateEast, texU, texV, out float uTr, out float vTr);

        float minX = (float)(pos.x + bb.MinX);
        float maxX = (float)(pos.x + bb.MaxX);
        float minY = (float)(pos.y + bb.MinY);
        float maxY = (float)(pos.y + bb.MaxY);
        float minZ = (float)(pos.z + bb.MinZ);

        if (EnableAo)
        {
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            Tess.addVertexWithUV(maxX, maxY, minZ, uTl, vTl);
            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            Tess.addVertexWithUV(maxX, minY, minZ, uBl, vBl);
            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            Tess.addVertexWithUV(minX, minY, minZ, uBr, vBr);
            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            Tess.addVertexWithUV(minX, maxY, minZ, uTr, vTr);
        }
        else
        {
            Tess.addVertexWithUV(maxX, maxY, minZ, uTl, vTl);
            Tess.addVertexWithUV(maxX, minY, minZ, uBl, vBl);
            Tess.addVertexWithUV(minX, minY, minZ, uBr, vBr);
            Tess.addVertexWithUV(minX, maxY, minZ, uTr, vTr);
        }
    }

    internal readonly void DrawWestFace(Block block, in Vec3D pos, in FaceColors colors, int textureId)
    {
        Box bb = OverrideBounds ?? block.BoundingBox;
        float bMinY = bb.MinY < 0.0D ? 0.0F : (float)(bb.MinY > 1.0 ? 1.0 : bb.MinY);
        float bMaxY = bb.MaxY < 0.0D ? 0.0F : (float)(bb.MaxY > 1.0 ? 1.0 : bb.MaxY);
        float bMinX = bb.MinX < 0.0D ? 0.0F : (float)(bb.MinX > 1.0 ? 1.0 : bb.MinX);
        float bMaxX = bb.MaxX < 0.0D ? 0.0F : (float)(bb.MaxX > 1.0 ? 1.0 : bb.MaxX);

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        // Z+ Face: Left = minX, Right = maxX
        CalculateUv(bMinX, 1.0F - bMaxY, UvRotateWest, texU, texV, out float uTl, out float vTl);
        CalculateUv(bMinX, 1.0F - bMinY, UvRotateWest, texU, texV, out float uBl, out float vBl);
        CalculateUv(bMaxX, 1.0F - bMinY, UvRotateWest, texU, texV, out float uBr, out float vBr);
        CalculateUv(bMaxX, 1.0F - bMaxY, UvRotateWest, texU, texV, out float uTr, out float vTr);

        float minX = (float)(pos.x + bb.MinX);
        float maxX = (float)(pos.x + bb.MaxX);
        float minY = (float)(pos.y + bb.MinY);
        float maxY = (float)(pos.y + bb.MaxY);
        float maxZ = (float)(pos.z + bb.MaxZ);

        if (EnableAo)
        {
            Tess.setColorOpaque_F(colors.RedTopLeft, colors.GreenTopLeft, colors.BlueTopLeft);
            Tess.addVertexWithUV(minX, maxY, maxZ, uTl, vTl);
            Tess.setColorOpaque_F(colors.RedBottomLeft, colors.GreenBottomLeft, colors.BlueBottomLeft);
            Tess.addVertexWithUV(minX, minY, maxZ, uBl, vBl);
            Tess.setColorOpaque_F(colors.RedBottomRight, colors.GreenBottomRight, colors.BlueBottomRight);
            Tess.addVertexWithUV(maxX, minY, maxZ, uBr, vBr);
            Tess.setColorOpaque_F(colors.RedTopRight, colors.GreenTopRight, colors.BlueTopRight);
            Tess.addVertexWithUV(maxX, maxY, maxZ, uTr, vTr);
        }
        else
        {
            Tess.addVertexWithUV(minX, maxY, maxZ, uTl, vTl);
            Tess.addVertexWithUV(minX, minY, maxZ, uBl, vBl);
            Tess.addVertexWithUV(maxX, minY, maxZ, uBr, vBr);
            Tess.addVertexWithUV(maxX, maxY, maxZ, uTr, vTr);
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
        if (RenderAllFaces || bounds.MinY > 0.0F || block.isSideVisible(World, pos.x, pos.y - 1, pos.z, 0))
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
                v0 = (sw + w + s + lYn) / 4.0F;
                v1 = (w + nw + lYn + n) / 4.0F;
                v2 = (lYn + n + e + ne) / 4.0F;
                v3 = (s + lYn + se + e) / 4.0F;
            }

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, r, g, b, 0.5F, tintBottom);
            int textureId = OverrideTexture >= 0 ? OverrideTexture : block.getTextureId(World, pos.x, pos.y, pos.z, 0);
            DrawBottomFace(block, new Vec3D(pos.x, pos.y, pos.z), colors, textureId);
            hasRendered = true;
        }

        // TOP FACE (Y + 1)
        if (RenderAllFaces || bounds.MaxY < 1.0F || block.isSideVisible(World, pos.x, pos.y + 1, pos.z, 1))
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
                v0 = (s + lYp + se + e) / 4.0F;
                v1 = (lYp + n + e + ne) / 4.0F;
                v2 = (w + nw + lYp + n) / 4.0F;
                v3 = (sw + w + s + lYp) / 4.0F;
            }

            var colors = FaceColors.AssignVertexColors(v0, v1, v2, v3, r, g, b, 1.0F, tintTop);
            int textureId = OverrideTexture >= 0 ? OverrideTexture : block.getTextureId(World, pos.x, pos.y, pos.z, 1);
            DrawTopFace(block, new Vec3D(pos.x, pos.y, pos.z), colors, textureId);
            hasRendered = true;
        }

        // EAST FACE (Z - 1)
        if (RenderAllFaces || bounds.MinZ > 0.0F || block.isSideVisible(World, pos.x, pos.y, pos.z - 1, 2))
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

            var colors = FaceColors.AssignVertexColors(v1, v2, v3, v0, r, g, b, 0.8F, tintEast);
            int textureId = OverrideTexture >= 0 ? OverrideTexture : block.getTextureId(World, pos.x, pos.y, pos.z, 2);
            DrawEastFace(block, new Vec3D(pos.x, pos.y, pos.z), colors, textureId);
            hasRendered = true;
        }

        // WEST FACE (Z + 1)
        if (RenderAllFaces || bounds.MaxZ < 1.0F || block.isSideVisible(World, pos.x, pos.y, pos.z + 1, 3))
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
            int textureId = OverrideTexture >= 0 ? OverrideTexture : block.getTextureId(World, pos.x, pos.y, pos.z, 3);
            DrawWestFace(block, new Vec3D(pos.x, pos.y, pos.z), colors, textureId);
            hasRendered = true;
        }

        // NORTH FACE (X - 1)
        if (RenderAllFaces || bounds.MinX > 0.0F || block.isSideVisible(World, pos.x - 1, pos.y, pos.z, 4))
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

            var colors = FaceColors.AssignVertexColors(v1, v2, v3, v0, r, g, b, 0.6F, tintNorth);
            int textureId = OverrideTexture >= 0 ? OverrideTexture : block.getTextureId(World, pos.x, pos.y, pos.z, 4);
            DrawNorthFace(block, new Vec3D(pos.x, pos.y, pos.z), colors, textureId);
            hasRendered = true;
        }

        // SOUTH FACE (X + 1)
        if (RenderAllFaces || bounds.MaxX < 1.0F || block.isSideVisible(World, pos.x + 1, pos.y, pos.z, 5))
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

            var colors = FaceColors.AssignVertexColors(v3, v0, v1, v2, r, g, b, 0.6F, tintSouth);
            int textureId = OverrideTexture >= 0 ? OverrideTexture : block.getTextureId(World, pos.x, pos.y, pos.z, 5);
            DrawSouthFace(block, new Vec3D(pos.x, pos.y, pos.z), colors, textureId);
            hasRendered = true;
        }

        return hasRendered;
    }

    internal readonly void DrawTorch(in Block block, in Vec3D pos, float tiltX, float tiltZ)
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
        float topMinU = minU + 7.0f / 256.0f;
        float topMinV = minV + 6.0f / 256.0f;
        float topMaxU = minU + 9.0f / 256.0f;
        float topMaxV = minV + 8.0f / 256.0f; // 1.0f / 32.0f = 8.0f / 256.0f

        // Shift origin to the center of the block for easier rotation/tilting math
        float centerX = (float)pos.x + 0.5f;
        float centerZ = (float)pos.z + 0.5f;

        float leftX = centerX - 0.5f;
        float rightX = centerX + 0.5f;
        float frontZ = centerZ - 0.5f;
        float backZ = centerZ + 0.5f;

        // Torch dimensions
        float radius = 1.0f / 16.0f; // 1 pixel thick from the center
        float height = 0.625f; // 10 pixels tall (10 / 16)

        // TOP FACE (The burning tip)
        float tipOffsetBase = 1.0f - height; // How far down from the top of the block space the tip sits
        float tipX = centerX + tiltX * tipOffsetBase;
        float tipZ = centerZ + tiltZ * tipOffsetBase;

        Tess.addVertexWithUV(tipX - radius, pos.y + height, tipZ - radius, topMinU, topMinV);
        Tess.addVertexWithUV(tipX - radius, pos.y + height, tipZ + radius, topMinU, topMaxV);
        Tess.addVertexWithUV(tipX + radius, pos.y + height, tipZ + radius, topMaxU, topMaxV);
        Tess.addVertexWithUV(tipX + radius, pos.y + height, tipZ - radius, topMaxU, topMinV);

        // SIDE FACES
        // The top vertices stay near the center, while the bottom vertices are shifted by tiltX and tiltZ

        // West Face
        Tess.addVertexWithUV(centerX - radius, pos.y + 1.0F, frontZ, minU, minV);
        Tess.addVertexWithUV(centerX - radius + tiltX, pos.y + 0.0F, frontZ + tiltZ, minU, maxV);
        Tess.addVertexWithUV(centerX - radius + tiltX, pos.y + 0.0F, backZ + tiltZ, maxU, maxV);
        Tess.addVertexWithUV(centerX - radius, pos.y + 1.0F, backZ, maxU, minV);

        // East Face
        Tess.addVertexWithUV(centerX + radius, pos.y + 1.0F, backZ, minU, minV);
        Tess.addVertexWithUV(centerX + radius + tiltX, pos.y + 0.0F, backZ + tiltZ, minU, maxV);
        Tess.addVertexWithUV(centerX + radius + tiltX, pos.y + 0.0F, frontZ + tiltZ, maxU, maxV);
        Tess.addVertexWithUV(centerX + radius, pos.y + 1.0F, frontZ, maxU, minV);

        // North Face
        Tess.addVertexWithUV(leftX, pos.y + 1.0F, centerZ + radius, minU, minV);
        Tess.addVertexWithUV(leftX + tiltX, pos.y + 0.0F, centerZ + radius + tiltZ, minU, maxV);
        Tess.addVertexWithUV(rightX + tiltX, pos.y + 0.0F, centerZ + radius + tiltZ, maxU, maxV);
        Tess.addVertexWithUV(rightX, pos.y + 1.0F, centerZ + radius, maxU, minV);

        // South Face
        Tess.addVertexWithUV(rightX, pos.y + 1.0F, centerZ - radius, minU, minV);
        Tess.addVertexWithUV(rightX + tiltX, pos.y + 0.0F, centerZ - radius + tiltZ, minU, maxV);
        Tess.addVertexWithUV(leftX + tiltX, pos.y + 0.0F, centerZ - radius + tiltZ, maxU, maxV);
        Tess.addVertexWithUV(leftX, pos.y + 1.0F, centerZ - radius, maxU, minV);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly void CalculateUv(float h, float v, int rotation, int texU, int texV, out float u, out float outV)
    {
        //  0 and no flip are the most common states
        if (rotation == 0 && !FlipTexture)
        {
            u = texU * 0.00390625f + h * 0.0625f;
            outV = texV * 0.00390625f + v * 0.0625f;
            return;
        }

        float fU, fV;

        // Stripped down switch (pure assignment, no inline math)
        switch (rotation)
        {
            case 1: fU = v; fV = 1.0f - h; break;
            case 2: fU = 1.0f - h; fV = 1.0f - v; break;
            case 3: fU = 1.0f - v; fV = h; break;
            case 4: fU = 1.0f - h; fV = v; break;
            case 5: fU = v; fV = h; break;
            case 6: fU = h; fV = 1.0f - v; break;
            case 7: fU = 1.0f - v; fV = 1.0f - h; break;
            default: fU = h; fV = v; break;
        }
        fU = FlipTexture ? 1.0f - fU : fU;

        u = texU * 0.00390625f + fU * 0.0625f;
        outV = texV * 0.00390625f + fV * 0.0625f;
    }
}
