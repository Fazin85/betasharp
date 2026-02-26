using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class PistonExtensionRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess, in BlockRenderContext context)
    {
        int metadata = world.getBlockMeta(pos.x, pos.y, pos.z);
        int facing = BlockPistonExtension.getFacing(metadata);
        float luminance = block.getLuminance(world, pos.x, pos.y, pos.z);

        // Using CustomFlag to track if this is a ShortArm rendering phase
        bool isShortArm = context.CustomFlag;
        float armLength = isShortArm ? 1.0F : 0.5F;
        double texWidth = isShortArm ? 16.0D : 8.0D;

        int uvTop = 0, uvBottom = 0, uvNorth = 0, uvSouth = 0, uvEast = 0, uvWest = 0;
        Box? bounds = context.OverrideBounds ?? block.BoundingBox;

        // 1. Calculate the rotations and bounds for the "Head" of the piston
        switch (facing)
        {
            case 0: // Down
                uvTop = 3; uvBottom = 3; uvNorth = 3; uvSouth = 3; uvWest = 0; uvEast = 0;
                bounds = new Box(0.0F, 0.0F, 0.0F, 1.0F, 0.25F, 1.0F);
                break;
            case 1: // Up
                bounds = new Box(0.0F, 0.75F, 0.0F, 1.0F, 1.0F, 1.0F);
                break;
            case 2: // North
                uvTop = 0; uvBottom = 0; uvNorth = 2; uvSouth = 1; uvWest = 0; uvEast = 0;
                bounds = new Box(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.25F);
                break;
            case 3: // South
                uvTop = 3; uvBottom = 3; uvNorth = 1; uvSouth = 2; uvWest = 0; uvEast = 0;
                bounds = new Box(0.0F, 0.0F, 0.75F, 1.0F, 1.0F, 1.0F);
                break;
            case 4: // West
                uvTop = 2; uvBottom = 1; uvNorth = 0; uvSouth = 0; uvWest = 2; uvEast = 1;
                bounds = new Box(0.0F, 0.0F, 0.0F, 0.25F, 1.0F, 1.0F);
                break;
            case 5: // East
                uvTop = 1; uvBottom = 2; uvNorth = 0; uvSouth = 0; uvWest = 1; uvEast = 2;
                bounds = new Box(0.75F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                break;
        }

        var headCtx = context with
        {
            OverrideBounds = bounds,
            UvRotateTop = uvTop,
            UvRotateBottom = uvBottom,
            UvRotateNorth = uvNorth,
            UvRotateSouth = uvSouth,
            UvRotateEast = uvEast,
            UvRotateWest = uvWest
        };

        bool hasRendered = Helper.RenderStandardBlock(block, pos, world, tess, headCtx);

        // 2. Render the custom extension arm geometry
        double x = pos.x;
        double y = pos.y;
        double z = pos.z;

        switch (facing)
        {
            case 0:
                RenderPistonArmY(tess, context, x + 0.375, x + 0.625, y + 0.25, y + 0.25 + armLength, z + 0.625, z + 0.625, luminance * 0.8F, texWidth);
                RenderPistonArmY(tess, context, x + 0.625, x + 0.375, y + 0.25, y + 0.25 + armLength, z + 0.375, z + 0.375, luminance * 0.8F, texWidth);
                RenderPistonArmY(tess, context, x + 0.375, x + 0.375, y + 0.25, y + 0.25 + armLength, z + 0.375, z + 0.625, luminance * 0.6F, texWidth);
                RenderPistonArmY(tess, context, x + 0.625, x + 0.625, y + 0.25, y + 0.25 + armLength, z + 0.625, z + 0.375, luminance * 0.6F, texWidth);
                break;
            case 1:
                RenderPistonArmY(tess, context, x + 0.375, x + 0.625, y + 0.75 - armLength, y + 0.75, z + 0.625, z + 0.625, luminance * 0.8F, texWidth);
                RenderPistonArmY(tess, context, x + 0.625, x + 0.375, y + 0.75 - armLength, y + 0.75, z + 0.375, z + 0.375, luminance * 0.8F, texWidth);
                RenderPistonArmY(tess, context, x + 0.375, x + 0.375, y + 0.75 - armLength, y + 0.75, z + 0.375, z + 0.625, luminance * 0.6F, texWidth);
                RenderPistonArmY(tess, context, x + 0.625, x + 0.625, y + 0.75 - armLength, y + 0.75, z + 0.625, z + 0.375, luminance * 0.6F, texWidth);
                break;
            case 2:
                RenderPistonArmZ(tess, context, x + 0.375, x + 0.375, y + 0.625, y + 0.375, z + 0.25, z + 0.25 + armLength, luminance * 0.6F, texWidth);
                RenderPistonArmZ(tess, context, x + 0.625, x + 0.625, y + 0.375, y + 0.625, z + 0.25, z + 0.25 + armLength, luminance * 0.6F, texWidth);
                RenderPistonArmZ(tess, context, x + 0.375, x + 0.625, y + 0.375, y + 0.375, z + 0.25, z + 0.25 + armLength, luminance * 0.5F, texWidth);
                RenderPistonArmZ(tess, context, x + 0.625, x + 0.375, y + 0.625, y + 0.625, z + 0.25, z + 0.25 + armLength, luminance, texWidth);
                break;
            case 3:
                RenderPistonArmZ(tess, context, x + 0.375, x + 0.375, y + 0.625, y + 0.375, z + 0.75 - armLength, z + 0.75, luminance * 0.6F, texWidth);
                RenderPistonArmZ(tess, context, x + 0.625, x + 0.625, y + 0.375, y + 0.625, z + 0.75 - armLength, z + 0.75, luminance * 0.6F, texWidth);
                RenderPistonArmZ(tess, context, x + 0.375, x + 0.625, y + 0.375, y + 0.375, z + 0.75 - armLength, z + 0.75, luminance * 0.5F, texWidth);
                RenderPistonArmZ(tess, context, x + 0.625, x + 0.375, y + 0.625, y + 0.625, z + 0.75 - armLength, z + 0.75, luminance, texWidth);
                break;
            case 4:
                RenderPistonArmX(tess, context, x + 0.25, x + 0.25 + armLength, y + 0.375, y + 0.375, z + 0.625, z + 0.375, luminance * 0.5F, texWidth);
                RenderPistonArmX(tess, context, x + 0.25, x + 0.25 + armLength, y + 0.625, y + 0.625, z + 0.375, z + 0.625, luminance, texWidth);
                RenderPistonArmX(tess, context, x + 0.25, x + 0.25 + armLength, y + 0.375, y + 0.625, z + 0.375, z + 0.375, luminance * 0.6F, texWidth);
                RenderPistonArmX(tess, context, x + 0.25, x + 0.25 + armLength, y + 0.625, y + 0.375, z + 0.625, z + 0.625, luminance * 0.6F, texWidth);
                break;
            case 5:
                RenderPistonArmX(tess, context, x + 0.75 - armLength, x + 0.75, y + 0.375, y + 0.375, z + 0.625, z + 0.375, luminance * 0.5F, texWidth);
                RenderPistonArmX(tess, context, x + 0.75 - armLength, x + 0.75, y + 0.625, y + 0.625, z + 0.375, z + 0.625, luminance, texWidth);
                RenderPistonArmX(tess, context, x + 0.75 - armLength, x + 0.75, y + 0.375, y + 0.625, z + 0.375, z + 0.375, luminance * 0.6F, texWidth);
                RenderPistonArmX(tess, context, x + 0.75 - armLength, x + 0.75, y + 0.625, y + 0.375, z + 0.625, z + 0.625, luminance * 0.6F, texWidth);
                break;
        }

        return hasRendered;
    }

    private void RenderPistonArmY(Tessellator tess, in BlockRenderContext context, double x1, double x2, double y1, double y2, double z1, double z2, float luminance, double textureWidth)
    {
        int textureId = 108; // Base piston arm texture
        if (context.OverrideTexture >= 0) textureId = context.OverrideTexture;

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        double minU = texU / 256.0D;
        double minV = texV / 256.0D;
        double maxU = (texU + textureWidth - 0.01D) / 256.0D;
        double maxV = (texV + 4.0D - 0.01D) / 256.0D;

        tess.setColorOpaque_F(luminance, luminance, luminance);
        tess.addVertexWithUV(x1, y2, z1, maxU, minV);
        tess.addVertexWithUV(x1, y1, z1, minU, minV);
        tess.addVertexWithUV(x2, y1, z2, minU, maxV);
        tess.addVertexWithUV(x2, y2, z2, maxU, maxV);
    }

    private void RenderPistonArmZ(Tessellator tess, in BlockRenderContext context, double x1, double x2, double y1, double y2, double z1, double z2, float luminance, double textureWidth)
    {
        int textureId = 108;
        if (context.OverrideTexture >= 0) textureId = context.OverrideTexture;

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        double minU = texU / 256.0D;
        double minV = texV / 256.0D;
        double maxU = (texU + textureWidth - 0.01D) / 256.0D;
        double maxV = (texV + 4.0D - 0.01D) / 256.0D;

        tess.setColorOpaque_F(luminance, luminance, luminance);
        tess.addVertexWithUV(x1, y1, z2, maxU, minV);
        tess.addVertexWithUV(x1, y1, z1, minU, minV);
        tess.addVertexWithUV(x2, y2, z1, minU, maxV);
        tess.addVertexWithUV(x2, y2, z2, maxU, maxV);
    }

    private void RenderPistonArmX(Tessellator tess, in BlockRenderContext context, double x1, double x2, double y1, double y2, double z1, double z2, float luminance, double textureWidth)
    {
        int textureId = 108;
        if (context.OverrideTexture >= 0) textureId = context.OverrideTexture;

        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;

        double minU = texU / 256.0D;
        double minV = texV / 256.0D;
        double maxU = (texU + textureWidth - 0.01D) / 256.0D;
        double maxV = (texV + 4.0D - 0.01D) / 256.0D;

        tess.setColorOpaque_F(luminance, luminance, luminance);
        tess.addVertexWithUV(x2, y1, z1, maxU, minV);
        tess.addVertexWithUV(x1, y1, z1, minU, minV);
        tess.addVertexWithUV(x1, y2, z2, minU, maxV);
        tess.addVertexWithUV(x2, y2, z2, maxU, maxV);
    }
}
