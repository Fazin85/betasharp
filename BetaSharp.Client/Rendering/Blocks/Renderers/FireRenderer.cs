using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class FireRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess,
        in BlockRenderContext context)
    {
        Tessellator tess = _tess;
        int textureId = block.getTexture(0);
        if (_overrideBlockTexture >= 0) textureId = _overrideBlockTexture;

        float luminance = block.getLuminance(_blockAccess, x, y, z);
        tess.setColorOpaque_F(luminance, luminance, luminance);

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = texU / 256.0F;
        double maxU = (texU + 15.99F) / 256.0F;
        double minV = texV / 256.0F;
        double maxV = (texV + 15.99F) / 256.0F;

        float fireHeight = 1.4F;

        // If not on a solid/flammable floor, render climbing flames on walls
        if (!_blockAccess.shouldSuffocate(x, y - 1, z) && !Block.Fire.isFlammable(_blockAccess, x, y - 1, z))
        {
            float sideInset = 0.2F;
            float yOffset = 1.0F / 16.0F;

            // Variation: Flip texture or use second fire frame based on position
            if ((x + y + z & 1) == 1)
            {
                minV = (texV + 16) / 256.0F;
                maxV = (texV + 15.99F + 16.0F) / 256.0F;
            }

            if ((x / 2 + y / 2 + z / 2 & 1) == 1)
            {
                (minU, maxU) = (maxU, minU);
            }

            // Climbing West Wall
            if (Block.Fire.isFlammable(_blockAccess, x - 1, y, z))
            {
                tess.addVertexWithUV(x + sideInset, y + fireHeight + yOffset, z + 1, maxU, minV);
                tess.addVertexWithUV(x, y + yOffset, z + 1, maxU, maxV);
                tess.addVertexWithUV(x, y + yOffset, z, minU, maxV);
                tess.addVertexWithUV(x + sideInset, y + fireHeight + yOffset, z, minU, minV);
                // Backface
                tess.addVertexWithUV(x + sideInset, y + fireHeight + yOffset, z, minU, minV);
                tess.addVertexWithUV(x, y + yOffset, z, minU, maxV);
                tess.addVertexWithUV(x, y + yOffset, z + 1, maxU, maxV);
                tess.addVertexWithUV(x + sideInset, y + fireHeight + yOffset, z + 1, maxU, minV);
            }

            // Climbing East Wall
            if (Block.Fire.isFlammable(_blockAccess, x + 1, y, z))
            {
                tess.addVertexWithUV(x + 1 - sideInset, y + fireHeight + yOffset, z, minU, minV);
                tess.addVertexWithUV(x + 1, y + yOffset, z, minU, maxV);
                tess.addVertexWithUV(x + 1, y + yOffset, z + 1, maxU, maxV);
                tess.addVertexWithUV(x + 1 - sideInset, y + fireHeight + yOffset, z + 1, maxU, minV);
                // Backface
                tess.addVertexWithUV(x + 1 - sideInset, y + fireHeight + yOffset, z + 1, maxU, minV);
                tess.addVertexWithUV(x + 1, y + yOffset, z + 1, maxU, maxV);
                tess.addVertexWithUV(x + 1, y + yOffset, z, minU, maxV);
                tess.addVertexWithUV(x + 1 - sideInset, y + fireHeight + yOffset, z, minU, minV);
            }

            // Climbing North Wall
            if (Block.Fire.isFlammable(_blockAccess, x, y, z - 1))
            {
                tess.addVertexWithUV(x, y + fireHeight + yOffset, z + sideInset, maxU, minV);
                tess.addVertexWithUV(x, y + yOffset, z, maxU, maxV);
                tess.addVertexWithUV(x + 1, y + yOffset, z, minU, maxV);
                tess.addVertexWithUV(x + 1, y + fireHeight + yOffset, z + sideInset, minU, minV);
                // Backface
                tess.addVertexWithUV(x + 1, y + fireHeight + yOffset, z + sideInset, minU, minV);
                tess.addVertexWithUV(x + 1, y + yOffset, z, minU, maxV);
                tess.addVertexWithUV(x, y + yOffset, z, maxU, maxV);
                tess.addVertexWithUV(x, y + fireHeight + yOffset, z + sideInset, maxU, minV);
            }

            // Climbing South Wall
            if (Block.Fire.isFlammable(_blockAccess, x, y, z + 1))
            {
                tess.addVertexWithUV(x + 1, y + fireHeight + yOffset, z + 1 - sideInset, minU, minV);
                tess.addVertexWithUV(x + 1, y + yOffset, z + 1, minU, maxV);
                tess.addVertexWithUV(x, y + yOffset, z + 1, maxU, maxV);
                tess.addVertexWithUV(x, y + fireHeight + yOffset, z + 1 - sideInset, maxU, minV);
                // Backface
                tess.addVertexWithUV(x, y + fireHeight + yOffset, z + 1 - sideInset, maxU, minV);
                tess.addVertexWithUV(x, y + yOffset, z + 1, maxU, maxV);
                tess.addVertexWithUV(x + 1, y + yOffset, z + 1, minU, maxV);
                tess.addVertexWithUV(x + 1, y + fireHeight + yOffset, z + 1 - sideInset, minU, minV);
            }

            // Climbing Ceilings
            if (Block.Fire.isFlammable(_blockAccess, x, y + 1, z))
            {
                double xMax = x + 1, xMin = x;
                double zMax = z + 1, zMin = z;

                minU = texU / 256.0F;
                maxU = (texU + 15.99F) / 256.0F;
                minV = texV / 256.0F;
                maxV = (texV + 15.99F) / 256.0F;

                int ceilY = y + 1;
                float ceilOffset = -0.2F;

                if ((x + ceilY + z & 1) == 0)
                {
                    tess.addVertexWithUV(xMin, ceilY + ceilOffset, z, maxU, minV);
                    tess.addVertexWithUV(xMax, ceilY, z, maxU, maxV);
                    tess.addVertexWithUV(xMax, ceilY, z + 1, minU, maxV);
                    tess.addVertexWithUV(xMin, ceilY + ceilOffset, z + 1, minU, minV);

                    minV = (texV + 16) / 256.0F;
                    maxV = (texV + 15.99F + 16.0F) / 256.0F;

                    tess.addVertexWithUV(xMax, ceilY + ceilOffset, z + 1, maxU, minV);
                    tess.addVertexWithUV(xMin, ceilY, z + 1, maxU, maxV);
                    tess.addVertexWithUV(xMin, ceilY, z, minU, maxV);
                    tess.addVertexWithUV(xMax, ceilY + ceilOffset, z, minU, minV);
                }
                else
                {
                    tess.addVertexWithUV(x, ceilY + ceilOffset, zMax, maxU, minV);
                    tess.addVertexWithUV(x, ceilY, zMin, maxU, maxV);
                    tess.addVertexWithUV(x + 1, ceilY, zMin, minU, maxV);
                    tess.addVertexWithUV(x + 1, ceilY + ceilOffset, zMax, minU, minV);

                    minV = (texV + 16) / 256.0F;
                    maxV = (texV + 15.99F + 16.0F) / 256.0F;

                    tess.addVertexWithUV(x + 1, ceilY + ceilOffset, zMin, maxU, minV);
                    tess.addVertexWithUV(x + 1, ceilY, zMax, maxU, maxV);
                    tess.addVertexWithUV(x, ceilY, zMax, minU, maxV);
                    tess.addVertexWithUV(x, ceilY + ceilOffset, zMin, minU, minV);
                }
            }
        }
        else // Render central "X" flames for fire on solid floors
        {
            double insetSmall = 0.2D, insetLarge = 0.3D;
            double xC = x + 0.5D, zC = z + 0.5D;

            // First diagonal set
            tess.addVertexWithUV(xC - insetLarge, y + fireHeight, z + 1, maxU, minV);
            tess.addVertexWithUV(xC + insetSmall, y, z + 1, maxU, maxV);
            tess.addVertexWithUV(xC + insetSmall, y, z, minU, maxV);
            tess.addVertexWithUV(xC - insetLarge, y + fireHeight, z, minU, minV);

            tess.addVertexWithUV(xC + insetLarge, y + fireHeight, z, maxU, minV);
            tess.addVertexWithUV(xC - insetSmall, y, z, maxU, maxV);
            tess.addVertexWithUV(xC - insetSmall, y, z + 1, minU, maxV);
            tess.addVertexWithUV(xC + insetLarge, y + fireHeight, z + 1, minU, minV);

            // Switch texture frame
            minV = (texV + 16) / 256.0F;
            maxV = (texV + 15.99F + 16.0F) / 256.0F;

            // Second diagonal set (X-axis dominant)
            tess.addVertexWithUV(x + 1, y + fireHeight, zC + insetLarge, maxU, minV);
            tess.addVertexWithUV(x + 1, y, zC - insetSmall, maxU, maxV);
            tess.addVertexWithUV(x, y, zC - insetSmall, minU, maxV);
            tess.addVertexWithUV(x, y + fireHeight, zC + insetLarge, minU, minV);

            tess.addVertexWithUV(x, y + fireHeight, zC - insetLarge, maxU, minV);
            tess.addVertexWithUV(x, y, zC + insetSmall, maxU, maxV);
            tess.addVertexWithUV(x + 1, y, zC + insetSmall, minU, maxV);
            tess.addVertexWithUV(x + 1, y + fireHeight, zC - insetLarge, minU, minV);

            // Third set (outer crossing)
            double i4 = 0.4D, i5 = 0.5D;
            tess.addVertexWithUV(xC - i4, y + fireHeight, z, minU, minV);
            tess.addVertexWithUV(xC - i5, y, z, minU, maxV);
            tess.addVertexWithUV(xC - i5, y, z + 1, maxU, maxV);
            tess.addVertexWithUV(xC - i4, y + fireHeight, z + 1, maxU, minV);

            tess.addVertexWithUV(xC + i4, y + fireHeight, z + 1, minU, minV);
            tess.addVertexWithUV(xC + i5, y, z + 1, minU, maxV);
            tess.addVertexWithUV(xC + i5, y, z, maxU, maxV);
            tess.addVertexWithUV(xC + i4, y + fireHeight, z, maxU, minV);

            // Final set
            minV = texV / 256.0F;
            maxV = (texV + 15.99F) / 256.0F;
            tess.addVertexWithUV(x, y + fireHeight, zC + i4, minU, minV);
            tess.addVertexWithUV(x, y, zC + i5, minU, maxV);
            tess.addVertexWithUV(x + 1, y, zC + i5, maxU, maxV);
            tess.addVertexWithUV(x + 1, y + fireHeight, zC + i4, maxU, minV);

            tess.addVertexWithUV(x + 1, y + fireHeight, zC - i4, minU, minV);
            tess.addVertexWithUV(x + 1, y, zC - i5, minU, maxV);
            tess.addVertexWithUV(x, y, zC - i5, maxU, maxV);
            tess.addVertexWithUV(x, y + fireHeight, zC - i4, maxU, minV);
        }

        return true;
    }
}
