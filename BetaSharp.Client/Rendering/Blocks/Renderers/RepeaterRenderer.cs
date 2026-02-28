using BetaSharp.Blocks;
using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class RepeaterRenderer : IBlockRenderer
{
    public bool Draw(Block block, in BlockPos pos, ref BlockRenderContext ctx)
    {
        int metadata = ctx.World.getBlockMeta(pos.x, pos.y, pos.z);
        int direction = metadata & 3;
        int delay = (metadata & 12) >> 2;
        // 1. Base Rendering
        var slabCtx = ctx with { EnableAo = true, AoBlendMode = 0, UvRotateTop = direction % 4 };

        slabCtx.DrawBlock(block, pos);

        // 2. Prepare Torch Rendering
        float luminance = block.getLuminance(ctx.World, pos.x, pos.y, pos.z);
        if (Block.BlocksLightLuminance[block.id] > 0)
        {
            luminance = (luminance + 1.0F) * 0.5F;
        }

        ctx.Tess.setColorOpaque_F(luminance, luminance, luminance);

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
        slabCtx.DrawTorch(block, new Vec3D(pos.x + staticTorchX, pos.y + torchVerticalOffset, pos.z + staticTorchZ), 0.0f, 0.0f);
        slabCtx.DrawTorch(block, new Vec3D(pos.x + delayTorchX, pos.y + torchVerticalOffset, pos.z + delayTorchZ), 0.0f, 0.0f);
        return true;
    }
}
