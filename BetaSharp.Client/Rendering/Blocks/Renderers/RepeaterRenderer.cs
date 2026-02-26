using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class RepeaterRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess,
        in BlockRenderContext ctx)
    {
        int metadata = world.getBlockMeta(pos.x, pos.y, pos.z);
        int direction = metadata & 3;
        int delay = (metadata & 12) >> 2;
        // 1. Base Rendering
        var slabCtx = ctx with { EnableAo = false, UvRotateTop = (direction + 1) % 4 };

        slabCtx.DrawBlock(block, pos, world, tess);

        // 2. Prepare Torch Rendering
        float luminance = block.getLuminance(world, pos.x, pos.y, pos.z);
        if (Block.BlocksLightLuminance[block.id] > 0)
        {
            luminance = (luminance + 1.0F) * 0.5F;
        }

        tess.setColorOpaque_F(luminance, luminance, luminance);

        // Torch pins are rendered slightly below the slab surface so they sit inside it
        double torchVerticalOffset = -0.1875D;
        double staticTorchX = 0.0D;
        double staticTorchZ = 0.0D;
        double delayTorchX = 0.0D;
        double delayTorchZ = 0.0D;

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

        // 3. Render the two torch pins
        slabCtx.DrawTorch(block, tess,
            new Vec3D(pos.x + staticTorchX, pos.y + torchVerticalOffset, pos.z + staticTorchZ), 0.0D, 0.0D);
        slabCtx.DrawTorch(block, tess,
            new Vec3D(pos.x + delayTorchX, pos.y + torchVerticalOffset, pos.z + delayTorchZ), 0.0D, 0.0D);
        return true;
    }
}
