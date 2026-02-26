using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class BedRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess,
        in BlockRenderContext context)
    {
        Box bounds = context.OverrideBounds ?? block.BoundingBox;
        bool flipTexture;

        int metadata = world.getBlockMeta(pos.x, pos.y, pos.z);
        int direction = BlockBed.getDirection(metadata);
        bool isHead = BlockBed.isHeadOfBed(metadata);

        float lightBottom = 0.5F;
        float lightTop = 1.0F;
        float lightZ = 0.8F;
        float lightX = 0.6F;

        float centerLuminance = block.getLuminance(world, pos.x, pos.y, pos.z);

        // BOTTOM FACE
        tess.setColorOpaque_F(lightBottom * centerLuminance, lightBottom * centerLuminance,
            lightBottom * centerLuminance);

        int texBottom = block.getTextureId(world, pos.x, pos.y, pos.z, 0);
        int texU = (texBottom & 15) << 4;
        int texV = texBottom & 240;

        double minU = texU / 256.0F;
        double maxU = (texU + 15.99D) / 256.0D;
        double minV = texV / 256.0F;
        double maxV = (texV + 15.99D) / 256.0D;

        double minX = pos.x + bounds.MinX;
        double maxX = pos.x + bounds.MaxX;
        double bedBottomY = pos.y + bounds.MinY + 0.1875D; // Bed legs are 3 pixels tall (3/16 = 0.1875)
        double minZ = pos.z + bounds.MinZ;
        double maxZ = pos.z + bounds.MaxZ;

        tess.addVertexWithUV(minX, bedBottomY, maxZ, minU, maxV);
        tess.addVertexWithUV(minX, bedBottomY, minZ, minU, minV);
        tess.addVertexWithUV(maxX, bedBottomY, minZ, maxU, minV);
        tess.addVertexWithUV(maxX, bedBottomY, maxZ, maxU, maxV);

        // TOP FACE
        float topLuminance = block.getLuminance(world, pos.x, pos.y + 1, pos.z);
        tess.setColorOpaque_F(lightTop * topLuminance, lightTop * topLuminance, lightTop * topLuminance);

        int texTop = block.getTextureId(world, pos.x, pos.y, pos.z, 1);
        texU = (texTop & 15) << 4;
        texV = texTop & 240;

        minU = texU / 256.0F;
        maxU = (texU + 15.99D) / 256.0D;
        minV = texV / 256.0F;
        maxV = (texV + 15.99D) / 256.0D;

        double u1 = minU, u2 = maxU, u3 = minU, u4 = maxU;
        double v1 = minV, v2 = minV, v3 = maxV, v4 = maxV;

        // Rotate top texture based on bed orientation
        if (direction == 0) // South
        {
            u2 = minU;
            v2 = maxV;
            u3 = maxU;
            v3 = minV;
        }
        else if (direction == 2) // North
        {
            u1 = maxU;
            v1 = maxV;
            u4 = minU;
            v4 = minV;
        }
        else if (direction == 3) // East
        {
            u1 = maxU;
            v1 = maxV;
            u4 = minU;
            v4 = minV;
            u2 = minU;
            v2 = maxV;
            u3 = maxU;
            v3 = minV;
        }

        double bedTopY = pos.y + bounds.MaxY;

        tess.addVertexWithUV(maxX, bedTopY, maxZ, u3, v3);
        tess.addVertexWithUV(maxX, bedTopY, minZ, u1, v1);
        tess.addVertexWithUV(minX, bedTopY, minZ, u2, v2);
        tess.addVertexWithUV(minX, bedTopY, maxZ, u4, v4);

        // SIDE FACES
        int forwardDir = Facings.TO_DIR[direction];
        if (isHead)
        {
            forwardDir = Facings.TO_DIR[Facings.OPPOSITE[direction]];
        }

        byte textureFlipDir = 4;
        switch (direction)
        {
            case 0: textureFlipDir = 5; break;
            case 1:
                textureFlipDir = 3;
                goto case 2;
            case 2:
            default: break;
            case 3: textureFlipDir = 2; break;
        }

        float faceLuminance;

        // East Face (Z - 1)
        if (forwardDir != 2 && (context.RenderAllFaces || block.isSideVisible(world, pos.x, pos.y, pos.z - 1, 2)))
        {
            faceLuminance = bounds.MinZ > 0.0D ? centerLuminance : block.getLuminance(world, pos.x, pos.y, pos.z - 1);
            tess.setColorOpaque_F(lightZ * faceLuminance, lightZ * faceLuminance, lightZ * faceLuminance);

            flipTexture = textureFlipDir == 2;
            Helper.RenderEastFace(block, new Vec3D(pos.x, pos.y, pos.z), tess, context, new FaceColors(),
                block.getTextureId(world, pos.x, pos.y, pos.z, 2), flipTexture);
        }

        // West Face (Z + 1)
        if (forwardDir != 3 && (context.RenderAllFaces || block.isSideVisible(world, pos.x, pos.y, pos.z + 1, 3)))
        {
            faceLuminance = bounds.MaxZ < 1.0D ? centerLuminance : block.getLuminance(world, pos.x, pos.y, pos.z + 1);
            tess.setColorOpaque_F(lightZ * faceLuminance, lightZ * faceLuminance, lightZ * faceLuminance);

            flipTexture = textureFlipDir == 3;
            Helper.RenderWestFace(block, new Vec3D(pos.x, pos.y, pos.z), tess, context, new FaceColors(),
                block.getTextureId(world, pos.x, pos.y, pos.z, 3), flipTexture);
        }

        // North Face (X - 1)
        if (forwardDir != 4 && (context.RenderAllFaces || block.isSideVisible(world, pos.x - 1, pos.y, pos.z, 4)))
        {
            faceLuminance = bounds.MinX > 0.0D ? centerLuminance : block.getLuminance(world, pos.x - 1, pos.y, pos.z);
            tess.setColorOpaque_F(lightX * faceLuminance, lightX * faceLuminance, lightX * faceLuminance);

            flipTexture = textureFlipDir == 4;
            Helper.RenderNorthFace(block, new Vec3D(pos.x, pos.y, pos.z), tess, context, new FaceColors(),
                block.getTextureId(world, pos.x, pos.y, pos.z, 4), flipTexture);
        }

        // South Face (X + 1)
        if (forwardDir != 5 && (context.RenderAllFaces || block.isSideVisible(world, pos.x + 1, pos.y, pos.z, 5)))
        {
            faceLuminance = bounds.MaxX < 1.0D ? centerLuminance : block.getLuminance(world, pos.x + 1, pos.y, pos.z);
            tess.setColorOpaque_F(lightX * faceLuminance, lightX * faceLuminance, lightX * faceLuminance);

            flipTexture = textureFlipDir == 5;
            Helper.RenderSouthFace(block, new Vec3D(pos.x, pos.y, pos.z), tess, context, new FaceColors(),
                block.getTextureId(world, pos.x, pos.y, pos.z, 5), flipTexture);
        }

        return true;
    }
}
