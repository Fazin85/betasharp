using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class RedstoneWireRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess,
        in BlockRenderContext context)
    {
        Tessellator tess = _tess;
        int powerLevel = _blockAccess.getBlockMeta(x, y, z);

        int textureId = block.getTexture(1, powerLevel);
        if (_overrideBlockTexture >= 0) textureId = _overrideBlockTexture;

        // --- 1. Calculate the Glow Color ---
        float luminance = block.getLuminance(_blockAccess, x, y, z);
        float powerPercent = powerLevel / 15.0F;

        // Red component increases with power
        float r = powerPercent * 0.6F + 0.4F;
        if (powerLevel == 0) r = 0.3F;

        // Green and Blue are much lower to keep it red, but they curve up slightly at high power
        float g = powerPercent * powerPercent * 0.7F - 0.5F;
        float b = powerPercent * powerPercent * 0.6F - 0.7F;
        if (g < 0.0F) g = 0.0F;
        if (b < 0.0F) b = 0.0F;

        tess.setColorOpaque_F(luminance * r, luminance * g, luminance * b);

        // --- 2. UV Mapping ---
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = texU / 256.0F;
        double maxU = (texU + 15.99F) / 256.0F;
        double minV = texV / 256.0F;
        double maxV = (texV + 15.99F) / 256.0F;

        // --- 3. Connection Logic ---
        // Checks neighbors on same level OR one level down (if the neighbor isn't solid)
        bool connectsWest = BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x - 1, y, z, 1) ||
                            (!_blockAccess.shouldSuffocate(x - 1, y, z) &&
                             BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x - 1, y - 1, z, -1));
        bool connectsEast = BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x + 1, y, z, 3) ||
                            (!_blockAccess.shouldSuffocate(x + 1, y, z) &&
                             BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x + 1, y - 1, z, -1));
        bool connectsNorth = BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x, y, z - 1, 2) ||
                             (!_blockAccess.shouldSuffocate(x, y, z - 1) &&
                              BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x, y - 1, z - 1, -1));
        bool connectsSouth = BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x, y, z + 1, 0) ||
                             (!_blockAccess.shouldSuffocate(x, y, z + 1) &&
                              BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x, y - 1, z + 1, -1));

        // Check for connections climbing UP a block
        if (!_blockAccess.shouldSuffocate(x, y + 1, z))
        {
            if (_blockAccess.shouldSuffocate(x - 1, y, z) &&
                BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x - 1, y + 1, z, -1)) connectsWest = true;
            if (_blockAccess.shouldSuffocate(x + 1, y, z) &&
                BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x + 1, y + 1, z, -1)) connectsEast = true;
            if (_blockAccess.shouldSuffocate(x, y, z - 1) &&
                BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x, y + 1, z - 1, -1)) connectsNorth = true;
            if (_blockAccess.shouldSuffocate(x, y, z + 1) &&
                BlockRedstoneWire.isPowerProviderOrWire(_blockAccess, x, y + 1, z + 1, -1)) connectsSouth = true;
        }

        // --- 4. Determine Shape (Straight vs Cross) ---
        float renderMinX = x, renderMaxX = x + 1;
        float renderMinZ = z, renderMaxZ = z + 1;
        int shapeType = 0; // 0 = Cross, 1 = East/West, 2 = North/South

        if ((connectsWest || connectsEast) && !connectsNorth && !connectsSouth) shapeType = 1;
        if ((connectsNorth || connectsSouth) && !connectsEast && !connectsWest) shapeType = 2;

        if (shapeType != 0) // Use the "Straight Line" texture variant
        {
            minU = (texU + 16) / 256.0F;
            maxU = (texU + 16 + 15.99F) / 256.0F;
        }

        // Shrink the footprint if no connection exists on a specific side
        if (shapeType == 0)
        {
            if (connectsWest || connectsEast || connectsNorth || connectsSouth)
            {
                if (!connectsWest)
                {
                    renderMinX += 0.3125F;
                    minU += 0.01953125D;
                }

                if (!connectsEast)
                {
                    renderMaxX -= 0.3125F;
                    maxU -= 0.01953125D;
                }

                if (!connectsNorth)
                {
                    renderMinZ += 0.3125F;
                    minV += 0.01953125D;
                }

                if (!connectsSouth)
                {
                    renderMaxZ -= 0.3125F;
                    maxV -= 0.01953125D;
                }
            }
        }

        // --- 5. Render Horizontal Ground Quad ---
        double groundY = y + 0.015625D; // 1/64 height offset to prevent Z-fighting

        // Render the colored redstone
        tess.addVertexWithUV(renderMaxX, groundY, renderMaxZ, maxU, maxV);
        tess.addVertexWithUV(renderMaxX, groundY, renderMinZ, maxU, minV);
        tess.addVertexWithUV(renderMinX, groundY, renderMinZ, minU, minV);
        tess.addVertexWithUV(renderMinX, groundY, renderMaxZ, minU, maxV);

        // Render the dark shroud (shadow) underneath
        tess.setColorOpaque_F(luminance, luminance, luminance);
        double shroudVOffset = 1.0D / 16.0D; // Texture atlas row for shadow
        tess.addVertexWithUV(renderMaxX, groundY, renderMaxZ, maxU, maxV + shroudVOffset);
        tess.addVertexWithUV(renderMaxX, groundY, renderMinZ, maxU, minV + shroudVOffset);
        tess.addVertexWithUV(renderMinX, groundY, renderMinZ, minU, minV + shroudVOffset);
        tess.addVertexWithUV(renderMinX, groundY, renderMaxZ, minU, maxV + shroudVOffset);

        // --- 6. Render Slopes (Rising up walls) ---
        if (!_blockAccess.shouldSuffocate(x, y + 1, z))
        {
            minU = (texU + 16) / 256.0F;
            maxU = (texU + 16 + 15.99F) / 256.0F;
            double slopeHeight = y + 1.021875D; // Slight offset above the block

            // West Slope
            if (_blockAccess.shouldSuffocate(x - 1, y, z) && _blockAccess.getBlockId(x - 1, y + 1, z) == block.id)
            {
                tess.setColorOpaque_F(luminance * r, luminance * g, luminance * b);
                tess.addVertexWithUV(x + 0.015625D, slopeHeight, z + 1, maxU, minV);
                tess.addVertexWithUV(x + 0.015625D, y, z + 1, minU, minV);
                tess.addVertexWithUV(x + 0.015625D, y, z + 0, minU, maxV);
                tess.addVertexWithUV(x + 0.015625D, slopeHeight, z + 0, maxU, maxV);

                tess.setColorOpaque_F(luminance, luminance, luminance);
                tess.addVertexWithUV(x + 0.015625D, slopeHeight, z + 1, maxU, minV + shroudVOffset);
                tess.addVertexWithUV(x + 0.015625D, y, z + 1, minU, minV + shroudVOffset);
                tess.addVertexWithUV(x + 0.015625D, y, z + 0, minU, maxV + shroudVOffset);
                tess.addVertexWithUV(x + 0.015625D, slopeHeight, z + 0, maxU, maxV + shroudVOffset);
            }

            // East Slope
            if (_blockAccess.shouldSuffocate(x + 1, y, z) && _blockAccess.getBlockId(x + 1, y + 1, z) == block.id)
            {
                tess.setColorOpaque_F(luminance * r, luminance * g, luminance * b);
                tess.addVertexWithUV(x + 1 - 0.015625D, y, z + 1, minU, maxV);
                tess.addVertexWithUV(x + 1 - 0.015625D, slopeHeight, z + 1, maxU, maxV);
                tess.addVertexWithUV(x + 1 - 0.015625D, slopeHeight, z + 0, maxU, minV);
                tess.addVertexWithUV(x + 1 - 0.015625D, y, z + 0, minU, minV);

                tess.setColorOpaque_F(luminance, luminance, luminance);
                tess.addVertexWithUV(x + 1 - 0.015625D, y, z + 1, minU, maxV + shroudVOffset);
                tess.addVertexWithUV(x + 1 - 0.015625D, slopeHeight, z + 1, maxU, maxV + shroudVOffset);
                tess.addVertexWithUV(x + 1 - 0.015625D, slopeHeight, z + 0, maxU, minV + shroudVOffset);
                tess.addVertexWithUV(x + 1 - 0.015625D, y, z + 0, minU, minV + shroudVOffset);
            }

            // North Slope
            if (_blockAccess.shouldSuffocate(x, y, z - 1) && _blockAccess.getBlockId(x, y + 1, z - 1) == block.id)
            {
                tess.setColorOpaque_F(luminance * r, luminance * g, luminance * b);
                tess.addVertexWithUV(x + 1, y, z + 0.015625D, minU, maxV);
                tess.addVertexWithUV(x + 1, slopeHeight, z + 0.015625D, maxU, maxV);
                tess.addVertexWithUV(x + 0, slopeHeight, z + 0.015625D, maxU, minV);
                tess.addVertexWithUV(x + 0, y, z + 0.015625D, minU, minV);

                tess.setColorOpaque_F(luminance, luminance, luminance);
                tess.addVertexWithUV(x + 1, y, z + 0.015625D, minU, maxV + shroudVOffset);
                tess.addVertexWithUV(x + 1, slopeHeight, z + 0.015625D, maxU, maxV + shroudVOffset);
                tess.addVertexWithUV(x + 0, slopeHeight, z + 0.015625D, maxU, minV + shroudVOffset);
                tess.addVertexWithUV(x + 0, y, z + 0.015625D, minU, minV + shroudVOffset);
            }

            // South Slope
            if (_blockAccess.shouldSuffocate(x, y, z + 1) && _blockAccess.getBlockId(x, y + 1, z + 1) == block.id)
            {
                tess.setColorOpaque_F(luminance * r, luminance * g, luminance * b);
                tess.addVertexWithUV(x + 1, slopeHeight, z + 1 - 0.015625D, maxU, minV);
                tess.addVertexWithUV(x + 1, y, z + 1 - 0.015625D, minU, minV);
                tess.addVertexWithUV(x + 0, y, z + 1 - 0.015625D, minU, maxV);
                tess.addVertexWithUV(x + 0, slopeHeight, z + 1 - 0.015625D, maxU, maxV);

                tess.setColorOpaque_F(luminance, luminance, luminance);
                tess.addVertexWithUV(x + 1, slopeHeight, z + 1 - 0.015625D, maxU, minV + shroudVOffset);
                tess.addVertexWithUV(x + 1, y, z + 1 - 0.015625D, minU, minV + shroudVOffset);
                tess.addVertexWithUV(x + 0, y, z + 1 - 0.015625D, minU, maxV + shroudVOffset);
                tess.addVertexWithUV(x + 0, slopeHeight, z + 1 - 0.015625D, maxU, maxV + shroudVOffset);
            }
        }

        return true;
    }
}
