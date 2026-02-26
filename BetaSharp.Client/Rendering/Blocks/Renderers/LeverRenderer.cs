using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class LeverRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess, in BlockRenderContext context)
    {
        int metadata = world.getBlockMeta(pos.x, pos.y, pos.z);
        int orientation = metadata & 7;
        bool isActivated = (metadata & 8) > 0;

        float baseWidth = 0.25F;
        float baseThickness = 3.0F / 16.0F;
        float baseHeight = 3.0F / 16.0F;

        Box baseBox = new Box(0, 0, 0, 1, 1, 1); // Fallback box

        // --- 1. Calculate the Base Plate Bounding Box ---
        if (orientation == 5) // Floor (North/South)
        {
            baseBox = new Box(0.5F - baseHeight, 0.0F, 0.5F - baseWidth, 0.5F + baseHeight, baseThickness, 0.5F + baseWidth);
        }
        else if (orientation == 6) // Floor (East/West)
        {
            baseBox = new Box(0.5F - baseWidth, 0.0F, 0.5F - baseHeight, 0.5F + baseWidth, baseThickness, 0.5F + baseHeight);
        }
        else if (orientation == 4) // Wall South
        {
            baseBox = new Box(0.5F - baseHeight, 0.5F - baseWidth, 1.0F - baseThickness, 0.5F + baseHeight, 0.5F + baseWidth, 1.0F);
        }
        else if (orientation == 3) // Wall North
        {
            baseBox = new Box(0.5F - baseHeight, 0.5F - baseWidth, 0.0F, 0.5F + baseHeight, 0.5F + baseWidth, baseThickness);
        }
        else if (orientation == 2) // Wall East
        {
            baseBox = new Box(1.0F - baseThickness, 0.5F - baseWidth, 0.5F - baseHeight, 1.0F, 0.5F + baseWidth, 0.5F + baseHeight);
        }
        else if (orientation == 1) // Wall West
        {
            baseBox = new Box(0.0F, 0.5F - baseWidth, 0.5F - baseHeight, baseThickness, 0.5F + baseWidth, 0.5F + baseHeight);
        }

        // Levers use a cobblestone texture for the baseplate by default, unless overridden
        int baseTextureId = context.OverrideTexture >= 0 ? context.OverrideTexture : Block.Cobblestone.textureId;

        // Create a sub-context specifically for drawing the baseplate
        var baseCtx = new BlockRenderContext(
            overrideTexture: baseTextureId,
            renderAllFaces: context.RenderAllFaces,
            flipTexture: context.FlipTexture,
            bounds: baseBox,
            uvTop: context.UvRotateTop,
            uvBottom: context.UvRotateBottom,
            uvNorth: context.UvRotateNorth,
            uvSouth: context.UvRotateSouth,
            uvEast: context.UvRotateEast,
            uvWest: context.UvRotateWest,
            customFlag: context.CustomFlag
        );

        // Draw the base using the helper
        Helper.RenderStandardBlock(block, pos, world, tess, baseCtx);

        // --- 2. Calculate Handle Lighting & Texture ---
        float luminance = block.getLuminance(world, pos.x, pos.y, pos.z);
        if (Block.BlocksLightLuminance[block.id] > 0) luminance = 1.0F;
        tess.setColorOpaque_F(luminance, luminance, luminance);

        // Determine texture for the handle itself
        int handleTextureId = context.OverrideTexture >= 0 ? context.OverrideTexture : block.getTexture(0);

        int texU = (handleTextureId & 15) << 4;
        int texV = handleTextureId & 240;
        float minU = texU / 256.0F;
        float maxU = (texU + 15.99F) / 256.0F;
        float minV = texV / 256.0F;
        float maxV = (texV + 15.99F) / 256.0F;

        // --- 3. Handle Vertex Math ---
        Vec3D[] vertices = new Vec3D[8];
        float hRadius = 1.0F / 16.0F;
        float hLength = 10.0F / 16.0F;

        // Initial handle box (standing straight up)
        vertices[0] = new Vec3D(-hRadius, 0.0D, -hRadius);
        vertices[1] = new Vec3D(hRadius, 0.0D, -hRadius);
        vertices[2] = new Vec3D(hRadius, 0.0D, hRadius);
        vertices[3] = new Vec3D(-hRadius, 0.0D, hRadius);
        vertices[4] = new Vec3D(-hRadius, hLength, -hRadius);
        vertices[5] = new Vec3D(hRadius, hLength, -hRadius);
        vertices[6] = new Vec3D(hRadius, hLength, hRadius);
        vertices[7] = new Vec3D(-hRadius, hLength, hRadius);

        for (int i = 0; i < 8; ++i)
        {
            // Toggle angle based on state
            if (isActivated)
            {
                vertices[i].z -= 1.0D / 16.0D;
                Helper.RotateAroundX(ref vertices[i], (float)Math.PI * 2.0F / 9.0F);
            }
            else
            {
                vertices[i].z += 1.0D / 16.0D;
                Helper.RotateAroundX(ref vertices[i], -(float)Math.PI * 2.0F / 9.0F);
            }

            // Apply orientation rotations
            if (orientation == 6) Helper.RotateAroundY(ref vertices[i], (float)Math.PI * 0.5F);

            if (orientation < 5) // Wall mount requires extra rotation
            {
                vertices[i].y -= 0.375D;
                Helper.RotateAroundX(ref vertices[i], (float)Math.PI * 0.5F);

                if (orientation == 3) Helper.RotateAroundY(ref vertices[i], (float)Math.PI);
                if (orientation == 2) Helper.RotateAroundY(ref vertices[i], (float)Math.PI * 0.5F);
                if (orientation == 1) Helper.RotateAroundY(ref vertices[i], (float)Math.PI * -0.5F);

                vertices[i].x += pos.x + 0.5D; // Fixed .X to .x
                vertices[i].y += pos.y + 0.5D;
                vertices[i].z += pos.z + 0.5D;
            }
            else
            {
                vertices[i].x += pos.x + 0.5D; // Fixed .X to .x
                vertices[i].y += pos.y + 2.0F / 16.0F;
                vertices[i].z += pos.z + 0.5D;
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

            Vec3D v1 = default, v2 = default, v3 = default, v4 = default;

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

            tess.addVertexWithUV(v1.x, v1.y, v1.z, minU, maxV);
            tess.addVertexWithUV(v2.x, v2.y, v2.z, maxU, maxV);
            tess.addVertexWithUV(v3.x, v3.y, v3.z, maxU, minV);
            tess.addVertexWithUV(v4.x, v4.y, v4.z, minU, minV);
        }

        return true;
    }
}
