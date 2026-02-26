using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class MinecartTrackRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess, in BlockRenderContext ctx)
    {
        // Cast the generic block to a BlockRail to access rail-specific methods
        BlockRail rail = (BlockRail)block;

        int metadata = world.getBlockMeta(pos.x, pos.y, pos.z);

        int textureId = rail.getTexture(0, metadata);
        if (ctx.OverrideTexture >= 0)
        {
            textureId = ctx.OverrideTexture;
        }

        // Powered/Detector rails use bit 3 for state, but the first 8 shapes are identical
        if (rail.isAlwaysStraight())
        {
            metadata &= 7;
        }

        float luminance = rail.getLuminance(world, pos.x, pos.y, pos.z);
        tess.setColorOpaque_F(luminance, luminance, luminance);

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = texU / 256.0D;
        double maxU = (texU + 15.99D) / 256.0D;
        double minV = texV / 256.0D;
        double maxV = (texV + 15.99D) / 256.0D;

        float verticalOffset = 1.0F / 16.0F; // 1 pixel above the ground

        // Default vertex positions (flat square)
        float x1 = pos.x + 1, x2 = pos.x + 1, x3 = pos.x + 0, x4 = pos.x + 0;
        float z1 = pos.z + 0, z2 = pos.z + 1, z3 = pos.z + 1, z4 = pos.z + 0;

        float h1 = pos.y + verticalOffset;
        float h2 = pos.y + verticalOffset;
        float h3 = pos.y + verticalOffset;
        float h4 = pos.y + verticalOffset;

        // Handle coordinate swapping for curves and orientation
        if (metadata != 1 && metadata != 2 && metadata != 3 && metadata != 7)
        {
            if (metadata == 8)
            {
                x2 = pos.x + 0;
                x1 = x2;
                x4 = pos.x + 1;
                x3 = x4;
                z4 = pos.z + 1;
                z1 = z4;
                z3 = pos.z + 0;
                z2 = z3;
            }
            else if (metadata == 9)
            {
                x4 = pos.x + 0;
                x1 = x4;
                x3 = pos.x + 1;
                x2 = x3;
                z2 = pos.z + 0;
                z1 = z2;
                z4 = pos.z + 1;
                z3 = z4;
            }
        }
        else
        {
            x4 = pos.x + 1;
            x1 = x4;
            x3 = pos.x + 0;
            x2 = x3;
            z2 = pos.z + 1;
            z1 = z2;
            z4 = pos.z + 0;
            z3 = z4;
        }

        // Handle Slopes (ascending heights)
        if (metadata != 2 && metadata != 4)
        {
            if (metadata == 3 || metadata == 5)
            {
                h2++;
                h3++; // Sloping up North/South
            }
        }
        else
        {
            h1++;
            h4++; // Sloping up West/East
        }

        // Render both sides of the quad so it's visible from below (for glass/transparent floors)
        tess.addVertexWithUV(x1, h1, z1, maxU, minV);
        tess.addVertexWithUV(x2, h2, z2, maxU, maxV);
        tess.addVertexWithUV(x3, h3, z3, minU, maxV);
        tess.addVertexWithUV(x4, h4, z4, minU, minV);

        tess.addVertexWithUV(x4, h4, z4, minU, minV);
        tess.addVertexWithUV(x3, h3, z3, minU, maxV);
        tess.addVertexWithUV(x2, h2, z2, maxU, maxV);
        tess.addVertexWithUV(x1, h1, z1, maxU, minV);

        return true;
    }
}
