using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class FluidsRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess,
        in BlockRenderContext context)
    {
        Tessellator tess = _tess;
        Box bounds = block.BoundingBox;

        // Base fluid color tint (e.g., biome water color)
        int colorMultiplier = block.getColorMultiplier(_blockAccess, x, y, z);
        float tintR = (colorMultiplier >> 16 & 255) / 255.0F;
        float tintG = (colorMultiplier >> 8 & 255) / 255.0F;
        float tintB = (colorMultiplier & 255) / 255.0F;

        // Determine which faces are actually visible to the player
        bool isTopVisible = block.isSideVisible(_blockAccess, x, y + 1, z, 1);
        bool isBottomVisible = block.isSideVisible(_blockAccess, x, y - 1, z, 0);
        bool[] sideVisible =
        [
            block.isSideVisible(_blockAccess, x, y, z - 1, 2), // North
            block.isSideVisible(_blockAccess, x, y, z + 1, 3), // South
            block.isSideVisible(_blockAccess, x - 1, y, z, 4), // West
            block.isSideVisible(_blockAccess, x + 1, y, z, 5) // East
        ];

        // Fast exit if completely surrounded
        if (!isTopVisible && !isBottomVisible && !sideVisible[0] && !sideVisible[1] && !sideVisible[2] &&
            !sideVisible[3])
        {
            return false;
        }

        bool hasRendered = false;

        // Directional shading
        float lightBottom = 0.5F;
        float lightTop = 1.0F;
        float lightZ = 0.8F; // North/South
        float lightX = 0.6F; // East/West

        Material material = block.material;
        int meta = _blockAccess.getBlockMeta(x, y, z);

        // Calculate the height of the fluid at each of the 4 corners of this block
        float heightNw = GetFluidVertexHeight(x, y, z, material);
        float heightSw = GetFluidVertexHeight(x, y, z + 1, material);
        float heightSe = GetFluidVertexHeight(x + 1, y, z + 1, material);
        float heightNe = GetFluidVertexHeight(x + 1, y, z, material);

        // TOP FACE (Flowing Surface)
        if (_renderAllFaces || isTopVisible)
        {
            hasRendered = true;
            int textureId = block.getTexture(1, meta);
            float flowAngle = (float)BlockFluid.getFlowingAngle(_blockAccess, x, y, z, material);

            // If flowing, switch to the flowing texture variant
            if (flowAngle > -999.0F)
            {
                textureId = block.getTexture(2, meta);
            }

            int texU = (textureId & 15) << 4;
            int texV = textureId & 240;
            double centerU = (texU + 8.0D) / 256.0D;
            double centerV = (texV + 8.0D) / 256.0D;

            // If completely still, use standard flat UVs
            if (flowAngle < -999.0F)
            {
                flowAngle = 0.0F;
            }
            else
            {
                // Shift UV center for flowing animation
                centerU = (texU + 16) / 256.0F;
                centerV = (texV + 16) / 256.0F;
            }

            // Calculate rotational offsets for the UVs to make the texture flow in the correct direction
            float sinAngle = MathHelper.Sin(flowAngle) * 8.0F / 256.0F;
            float cosAngle = MathHelper.Cos(flowAngle) * 8.0F / 256.0F;

            float luminance = block.getLuminance(_blockAccess, x, y, z);
            tess.setColorOpaque_F(lightTop * luminance * tintR, lightTop * luminance * tintG,
                lightTop * luminance * tintB);

            // Draw top face with dynamic heights and rotated UVs
            tess.addVertexWithUV(x + 0, y + heightNw, z + 0, centerU - cosAngle - sinAngle,
                centerV - cosAngle + sinAngle);
            tess.addVertexWithUV(x + 0, y + heightSw, z + 1, centerU - cosAngle + sinAngle,
                centerV + cosAngle + sinAngle);
            tess.addVertexWithUV(x + 1, y + heightSe, z + 1, centerU + cosAngle + sinAngle,
                centerV + cosAngle - sinAngle);
            tess.addVertexWithUV(x + 1, y + heightNe, z + 0, centerU + cosAngle - sinAngle,
                centerV - cosAngle - sinAngle);
        }

        // BOTTOM FACE
        if (_renderAllFaces || isBottomVisible)
        {
            float luminance = block.getLuminance(_blockAccess, x, y - 1, z);
            tess.setColorOpaque_F(lightBottom * luminance, lightBottom * luminance, lightBottom * luminance);
            Helper.RenderBottomFace(block, x, y, z, block.getTexture(0));
            hasRendered = true;
        }

        // SIDE FACES (North, South, West, East)
        for (int side = 0; side < 4; ++side)
        {
            int adjX = x;
            int adjZ = z;

            if (side == 0) adjZ = z - 1; // North
            if (side == 1) adjZ = z + 1; // South
            if (side == 2) adjX = x - 1; // West
            if (side == 3) adjX = x + 1; // East

            int textureId = block.getTexture(side + 2, meta);
            int texU = (textureId & 15) << 4;
            int texV = textureId & 240;

            if (_renderAllFaces || sideVisible[side])
            {
                float h1, h2; // Top corner heights for this face
                float x1, x2; // X coordinates
                float z1, z2; // Z coordinates

                if (side == 0) // North
                {
                    h1 = heightNw;
                    h2 = heightNe;
                    x1 = x;
                    x2 = x + 1;
                    z1 = z;
                    z2 = z;
                }
                else if (side == 1) // South
                {
                    h1 = heightSe;
                    h2 = heightSw;
                    x1 = x + 1;
                    x2 = x;
                    z1 = z + 1;
                    z2 = z + 1;
                }
                else if (side == 2) // West
                {
                    h1 = heightSw;
                    h2 = heightNw;
                    x1 = x;
                    x2 = x;
                    z1 = z + 1;
                    z2 = z;
                }
                else // East
                {
                    h1 = heightNe;
                    h2 = heightSe;
                    x1 = x + 1;
                    x2 = x + 1;
                    z1 = z;
                    z2 = z + 1;
                }

                hasRendered = true;

                // Crop the UVs vertically so the texture doesn't stretch on short flowing water blocks
                double minU = (texU + 0) / 256.0F;
                double maxU = (texU + 16 - 0.01D) / 256.0D;
                double minV1 = (texV + (1.0F - h1) * 16.0F) / 256.0F; // UV height match for corner 1
                double minV2 = (texV + (1.0F - h2) * 16.0F) / 256.0F; // UV height match for corner 2
                double maxV = (texV + 16 - 0.01D) / 256.0D;

                float luminance = block.getLuminance(_blockAccess, adjX, y, adjZ);
                float shadow = (side < 2) ? lightZ : lightX;
                luminance *= shadow;

                tess.setColorOpaque_F(lightTop * luminance * tintR, lightTop * luminance * tintG,
                    lightTop * luminance * tintB);

                // Draw the side face matching the sloped top corners
                tess.addVertexWithUV(x1, y + h1, z1, minU, minV1);
                tess.addVertexWithUV(x2, y + h2, z2, maxU, minV2);
                tess.addVertexWithUV(x2, y + 0, z2, maxU, maxV);
                tess.addVertexWithUV(x1, y + 0, z1, minU, maxV);
            }
        }

        // Reset bounding box state
        bounds.MinY = 0.0D;
        bounds.MaxY = 1.0D;
        return hasRendered;
    }

    private float GetFluidVertexHeight(int x, int y, int z, Material material)
    {
        int totalWeight = 0;
        float totalDepth = 0.0F;

        // Iterate through the 2x2 grid sharing this vertex: (x, z), (x-1, z), (x, z-1), (x-1, z-1)
        for (int i = 0; i < 4; ++i)
        {
            int checkX = x - (i & 1);
            int checkZ = z - (i >> 1 & 1);

            // If there is fluid directly above any of the 4 blocks, the corner must be completely full (height 1.0)
            if (_blockAccess.getMaterial(checkX, y + 1, checkZ) == material)
            {
                return 1.0F;
            }

            Material neighborMaterial = _blockAccess.getMaterial(checkX, y, checkZ);

            if (neighborMaterial != material)
            {
                // If the neighbor is air or a non-solid block, it contributes "full depth" (pulls the water level down to 0)
                if (!neighborMaterial.IsSolid)
                {
                    ++totalDepth;
                    ++totalWeight;
                }
            }
            else
            {
                int neighborMeta = _blockAccess.getBlockMeta(checkX, y, checkZ);
                float fluidDepth = BlockFluid.getFluidHeightFromMeta(neighborMeta);

                // Meta >= 8 (falling fluid) or Meta == 0 (source block)
                if (neighborMeta >= 8 || neighborMeta == 0)
                {
                    // Source blocks and falling columns get 10x the "weight" in the average,
                    // heavily anchoring the fluid corner to their height.
                    totalDepth += fluidDepth * 10.0F;
                    totalWeight += 10;
                }

                totalDepth += fluidDepth;
                ++totalWeight;
            }
        }

        // Depth is measured from the top down. Subtract from 1.0 to get height from bottom up.
        return 1.0F - totalDepth / totalWeight;
    }
}
