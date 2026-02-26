using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class FireRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess, in BlockRenderContext ctx)
    {
        int textureId = block.getTexture(0);
        if (ctx.OverrideTexture >= 0) textureId = ctx.OverrideTexture;

        float luminance = block.getLuminance(world, pos.x, pos.y, pos.z);
        tess.setColorOpaque_F(luminance, luminance, luminance);

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = texU / 256.0F;
        double maxU = (texU + 15.99F) / 256.0F;
        double minV = texV / 256.0F;
        double maxV = (texV + 15.99F) / 256.0F;

        float fireHeight = 1.4F;

        // If not on a solid/flammable floor, render climbing flames on walls
        if (!world.shouldSuffocate(pos.x, pos.y - 1, pos.z) && !Block.Fire.isFlammable(world, pos.x, pos.y - 1, pos.z))
        {
            float sideInset = 0.2F;
            float yOffset = 1.0F / 16.0F;

            // Variation: Flip texture or use second fire frame based on position
            if ((pos.x + pos.y + pos.z & 1) == 1)
            {
                minV = (texV + 16) / 256.0F;
                maxV = (texV + 15.99F + 16.0F) / 256.0F;
            }

            if ((pos.x / 2 + pos.y / 2 + pos.z / 2 & 1) == 1)
            {
                (minU, maxU) = (maxU, minU);
            }

            // Climbing West Wall
            if (Block.Fire.isFlammable(world, pos.x - 1, pos.y, pos.z))
            {
                tess.addVertexWithUV(pos.x + sideInset, pos.y + fireHeight + yOffset, pos.z + 1, maxU, minV);
                tess.addVertexWithUV(pos.x, pos.y + yOffset, pos.z + 1, maxU, maxV);
                tess.addVertexWithUV(pos.x, pos.y + yOffset, pos.z, minU, maxV);
                tess.addVertexWithUV(pos.x + sideInset, pos.y + fireHeight + yOffset, pos.z, minU, minV);
                // Backface
                tess.addVertexWithUV(pos.x + sideInset, pos.y + fireHeight + yOffset, pos.z, minU, minV);
                tess.addVertexWithUV(pos.x, pos.y + yOffset, pos.z, minU, maxV);
                tess.addVertexWithUV(pos.x, pos.y + yOffset, pos.z + 1, maxU, maxV);
                tess.addVertexWithUV(pos.x + sideInset, pos.y + fireHeight + yOffset, pos.z + 1, maxU, minV);
            }

            // Climbing East Wall
            if (Block.Fire.isFlammable(world, pos.x + 1, pos.y, pos.z))
            {
                tess.addVertexWithUV(pos.x + 1 - sideInset, pos.y + fireHeight + yOffset, pos.z, minU, minV);
                tess.addVertexWithUV(pos.x + 1, pos.y + yOffset, pos.z, minU, maxV);
                tess.addVertexWithUV(pos.x + 1, pos.y + yOffset, pos.z + 1, maxU, maxV);
                tess.addVertexWithUV(pos.x + 1 - sideInset, pos.y + fireHeight + yOffset, pos.z + 1, maxU, minV);
                // Backface
                tess.addVertexWithUV(pos.x + 1 - sideInset, pos.y + fireHeight + yOffset, pos.z + 1, maxU, minV);
                tess.addVertexWithUV(pos.x + 1, pos.y + yOffset, pos.z + 1, maxU, maxV);
                tess.addVertexWithUV(pos.x + 1, pos.y + yOffset, pos.z, minU, maxV);
                tess.addVertexWithUV(pos.x + 1 - sideInset, pos.y + fireHeight + yOffset, pos.z, minU, minV);
            }

            // Climbing North Wall
            if (Block.Fire.isFlammable(world, pos.x, pos.y, pos.z - 1))
            {
                tess.addVertexWithUV(pos.x, pos.y + fireHeight + yOffset, pos.z + sideInset, maxU, minV);
                tess.addVertexWithUV(pos.x, pos.y + yOffset, pos.z, maxU, maxV);
                tess.addVertexWithUV(pos.x + 1, pos.y + yOffset, pos.z, minU, maxV);
                tess.addVertexWithUV(pos.x + 1, pos.y + fireHeight + yOffset, pos.z + sideInset, minU, minV);
                // Backface
                tess.addVertexWithUV(pos.x + 1, pos.y + fireHeight + yOffset, pos.z + sideInset, minU, minV);
                tess.addVertexWithUV(pos.x + 1, pos.y + yOffset, pos.z, minU, maxV);
                tess.addVertexWithUV(pos.x, pos.y + yOffset, pos.z, maxU, maxV);
                tess.addVertexWithUV(pos.x, pos.y + fireHeight + yOffset, pos.z + sideInset, maxU, minV);
            }

            // Climbing South Wall
            if (Block.Fire.isFlammable(world, pos.x, pos.y, pos.z + 1))
            {
                tess.addVertexWithUV(pos.x + 1, pos.y + fireHeight + yOffset, pos.z + 1 - sideInset, minU, minV);
                tess.addVertexWithUV(pos.x + 1, pos.y + yOffset, pos.z + 1, minU, maxV);
                tess.addVertexWithUV(pos.x, pos.y + yOffset, pos.z + 1, maxU, maxV);
                tess.addVertexWithUV(pos.x, pos.y + fireHeight + yOffset, pos.z + 1 - sideInset, maxU, minV);
                // Backface
                tess.addVertexWithUV(pos.x, pos.y + fireHeight + yOffset, pos.z + 1 - sideInset, maxU, minV);
                tess.addVertexWithUV(pos.x, pos.y + yOffset, pos.z + 1, maxU, maxV);
                tess.addVertexWithUV(pos.x + 1, pos.y + yOffset, pos.z + 1, minU, maxV);
                tess.addVertexWithUV(pos.x + 1, pos.y + fireHeight + yOffset, pos.z + 1 - sideInset, minU, minV);
            }

            // Climbing Ceilings
            if (Block.Fire.isFlammable(world, pos.x, pos.y + 1, pos.z))
            {
                double xMax = pos.x + 1, xMin = pos.x;
                double zMax = pos.z + 1, zMin = pos.z;

                minU = texU / 256.0F;
                maxU = (texU + 15.99F) / 256.0F;
                minV = texV / 256.0F;
                maxV = (texV + 15.99F) / 256.0F;

                int ceilY = pos.y + 1;
                float ceilOffset = -0.2F;

                if ((pos.x + ceilY + pos.z & 1) == 0)
                {
                    tess.addVertexWithUV(xMin, ceilY + ceilOffset, pos.z, maxU, minV);
                    tess.addVertexWithUV(xMax, ceilY, pos.z, maxU, maxV);
                    tess.addVertexWithUV(xMax, ceilY, pos.z + 1, minU, maxV);
                    tess.addVertexWithUV(xMin, ceilY + ceilOffset, pos.z + 1, minU, minV);

                    minV = (texV + 16) / 256.0F;
                    maxV = (texV + 15.99F + 16.0F) / 256.0F;

                    tess.addVertexWithUV(xMax, ceilY + ceilOffset, pos.z + 1, maxU, minV);
                    tess.addVertexWithUV(xMin, ceilY, pos.z + 1, maxU, maxV);
                    tess.addVertexWithUV(xMin, ceilY, pos.z, minU, maxV);
                    tess.addVertexWithUV(xMax, ceilY + ceilOffset, pos.z, minU, minV);
                }
                else
                {
                    tess.addVertexWithUV(pos.x, ceilY + ceilOffset, zMax, maxU, minV);
                    tess.addVertexWithUV(pos.x, ceilY, zMin, maxU, maxV);
                    tess.addVertexWithUV(pos.x + 1, ceilY, zMin, minU, maxV);
                    tess.addVertexWithUV(pos.x + 1, ceilY + ceilOffset, zMax, minU, minV);

                    minV = (texV + 16) / 256.0F;
                    maxV = (texV + 15.99F + 16.0F) / 256.0F;

                    tess.addVertexWithUV(pos.x + 1, ceilY + ceilOffset, zMin, maxU, minV);
                    tess.addVertexWithUV(pos.x + 1, ceilY, zMax, maxU, maxV);
                    tess.addVertexWithUV(pos.x, ceilY, zMax, minU, maxV);
                    tess.addVertexWithUV(pos.x, ceilY + ceilOffset, zMin, minU, minV);
                }
            }
        }
        else // Render central "X" flames for fire on solid floors
        {
            double insetSmall = 0.2D, insetLarge = 0.3D;
            double xC = pos.x + 0.5D, zC = pos.z + 0.5D;

            // First diagonal set
            tess.addVertexWithUV(xC - insetLarge, pos.y + fireHeight, pos.z + 1, maxU, minV);
            tess.addVertexWithUV(xC + insetSmall, pos.y, pos.z + 1, maxU, maxV);
            tess.addVertexWithUV(xC + insetSmall, pos.y, pos.z, minU, maxV);
            tess.addVertexWithUV(xC - insetLarge, pos.y + fireHeight, pos.z, minU, minV);

            tess.addVertexWithUV(xC + insetLarge, pos.y + fireHeight, pos.z, maxU, minV);
            tess.addVertexWithUV(xC - insetSmall, pos.y, pos.z, maxU, maxV);
            tess.addVertexWithUV(xC - insetSmall, pos.y, pos.z + 1, minU, maxV);
            tess.addVertexWithUV(xC + insetLarge, pos.y + fireHeight, pos.z + 1, minU, minV);

            // Switch texture frame
            minV = (texV + 16) / 256.0F;
            maxV = (texV + 15.99F + 16.0F) / 256.0F;

            // Second diagonal set (X-axis dominant)
            tess.addVertexWithUV(pos.x + 1, pos.y + fireHeight, zC + insetLarge, maxU, minV);
            tess.addVertexWithUV(pos.x + 1, pos.y, zC - insetSmall, maxU, maxV);
            tess.addVertexWithUV(pos.x, pos.y, zC - insetSmall, minU, maxV);
            tess.addVertexWithUV(pos.x, pos.y + fireHeight, zC + insetLarge, minU, minV);

            tess.addVertexWithUV(pos.x, pos.y + fireHeight, zC - insetLarge, maxU, minV);
            tess.addVertexWithUV(pos.x, pos.y, zC + insetSmall, maxU, maxV);
            tess.addVertexWithUV(pos.x + 1, pos.y, zC + insetSmall, minU, maxV);
            tess.addVertexWithUV(pos.x + 1, pos.y + fireHeight, zC - insetLarge, minU, minV);

            // Third set (outer crossing)
            double i4 = 0.4D, i5 = 0.5D;
            tess.addVertexWithUV(xC - i4, pos.y + fireHeight, pos.z, minU, minV);
            tess.addVertexWithUV(xC - i5, pos.y, pos.z, minU, maxV);
            tess.addVertexWithUV(xC - i5, pos.y, pos.z + 1, maxU, maxV);
            tess.addVertexWithUV(xC - i4, pos.y + fireHeight, pos.z + 1, maxU, minV);

            tess.addVertexWithUV(xC + i4, pos.y + fireHeight, pos.z + 1, minU, minV);
            tess.addVertexWithUV(xC + i5, pos.y, pos.z + 1, minU, maxV);
            tess.addVertexWithUV(xC + i5, pos.y, pos.z, maxU, maxV);
            tess.addVertexWithUV(xC + i4, pos.y + fireHeight, pos.z, maxU, minV);

            // Final set
            minV = texV / 256.0F;
            maxV = (texV + 15.99F) / 256.0F;
            tess.addVertexWithUV(pos.x, pos.y + fireHeight, zC + i4, minU, minV);
            tess.addVertexWithUV(pos.x, pos.y, zC + i5, minU, maxV);
            tess.addVertexWithUV(pos.x + 1, pos.y, zC + i5, maxU, maxV);
            tess.addVertexWithUV(pos.x + 1, pos.y + fireHeight, zC + i4, maxU, minV);

            tess.addVertexWithUV(pos.x + 1, pos.y + fireHeight, zC - i4, minU, minV);
            tess.addVertexWithUV(pos.x + 1, pos.y, zC - i5, minU, maxV);
            tess.addVertexWithUV(pos.x, pos.y, zC - i5, maxU, maxV);
            tess.addVertexWithUV(pos.x, pos.y + fireHeight, zC - i4, maxU, minV);
        }

        return true;
    }
}
