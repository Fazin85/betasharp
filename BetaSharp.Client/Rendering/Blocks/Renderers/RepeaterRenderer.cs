using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class RepeaterRenderer:IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess, in BlockRenderContext context) {
        int metadata = world.getBlockMeta(pos.x, pos.y, pos.z);
        int direction = metadata & 3;
        int delay = (metadata & 12) >> 2;

        // Render the base slab
        RenderStandardBlock(block, pos.x, pos.y, pos.z);

        float luminance = block.getLuminance(world, pos.x, pos.y, pos.z);
        if (Block.BlocksLightLuminance[block.id] > 0)
        {
            luminance = (luminance + 1.0F) * 0.5F;
        }

        tess.setColorOpaque_F(luminance, luminance, luminance);

        double torchVerticalOffset = -0.1875D;
        double staticTorchX = 0.0D;
        double staticTorchZ = 0.0D;
        double delayTorchX = 0.0D;
        double delayTorchZ = 0.0D;

        // Calculate positions for the two torch pins based on direction and delay
        switch (direction)
        {
            case 0: // South
                delayTorchZ = -0.3125D;
                staticTorchZ = BlockRedstoneRepeater.RENDER_OFFSET[delay];
                break;
            case 1: // West
                delayTorchX = 0.3125D;
                staticTorchX = -BlockRedstoneRepeater.RENDER_OFFSET[delay];
                break;
            case 2: // North
                delayTorchZ = 0.3125D;
                staticTorchZ = -BlockRedstoneRepeater.RENDER_OFFSET[delay];
                break;
            case 3: // East
                delayTorchX = -0.3125D;
                staticTorchX = BlockRedstoneRepeater.RENDER_OFFSET[delay];
                break;
        }

        // Render the two torch pins on top of the slab
        Helper.RenderTorchAtAngle(block, tess, new Vec3D(pos.x + staticTorchX, pos.y + torchVerticalOffset, pos.z + staticTorchZ), 0.0D, 0.0D, context);
        Helper.RenderTorchAtAngle(block, tess, new Vec3D(pos.x + delayTorchX, pos.y + torchVerticalOffset, pos.z + delayTorchZ), 0.0D, 0.0D, context);

        // Render the top surface texture of the repeater slab
        int textureId = block.getTexture(1);
        int texU = (textureId & 15) << 4;
        int texV = textureId & 240;
        double minU = texU / 256.0F;
        double maxU = (texU + 15.99F) / 256.0F;
        double minV = texV / 256.0F;
        double maxV = (texV + 15.99F) / 256.0F;

        float surfaceHeight = pos.y + (2.0F / 16.0F);
        float x1 = pos.x + 1;
        float x2 = pos.x + 1;
        float x3 = pos.x + 0;
        float x4 = pos.x + 0;
        float z1 = pos.z + 0;
        float z2 = pos.z + 1;
        float z3 = pos.z + 1;
        float z4 = pos.z + 0;

        if (direction == 2) // North
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
        else if (direction == 3) // East
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
        else if (direction == 1) // West
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

        tess.addVertexWithUV(x4, surfaceHeight, z4, minU, minV);
        tess.addVertexWithUV(x3, surfaceHeight, z3, minU, maxV);
        tess.addVertexWithUV(x2, surfaceHeight, z2, maxU, maxV);
        tess.addVertexWithUV(x1, surfaceHeight, z1, maxU, minV);

        return true;
    }
}
