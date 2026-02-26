using BetaSharp.Blocks;
using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class TorchRenderer : IBlockRenderer
{
    public bool Render(Block block, in BlockPos pos, in BlockRenderContext ctx)
    {
        int metadata = ctx.World.getBlockMeta(pos.x, pos.y, pos.z);

        float luminance = block.getLuminance(ctx.World, pos.x, pos.y, pos.z);
        if (Block.BlocksLightLuminance[block.id] > 0)
        {
            luminance = 1.0F;
        }

        ctx.Tess.setColorOpaque_F(luminance, luminance, luminance);

        double tiltAmount = 0.4F;
        double horizontalOffset = 0.5D - tiltAmount;
        double verticalOffset = 0.2F;

        if (metadata == 1) // Attached to West wall (pointing East)
        {
            ctx.DrawTorch(block, new Vec3D(pos.x - horizontalOffset, pos.y + verticalOffset, pos.z),
                -tiltAmount, 0.0D);
        }
        else if (metadata == 2) // Attached to East wall (pointing West)
        {
            ctx.DrawTorch(block, new Vec3D(pos.x + horizontalOffset, pos.y + verticalOffset, pos.z),
                tiltAmount, 0.0D);
        }
        else if (metadata == 3) // Attached to North wall (pointing South)
        {
            ctx.DrawTorch(block, new Vec3D(pos.x, pos.y + verticalOffset, pos.z - horizontalOffset), 0.0D,
                -tiltAmount);
        }
        else if (metadata == 4) // Attached to South wall (pointing North)
        {
            ctx.DrawTorch(block, new Vec3D(pos.x, pos.y + verticalOffset, pos.z + horizontalOffset), 0.0D,
                tiltAmount);
        }
        else // Standing on floor
        {
            ctx.DrawTorch(block, new Vec3D(pos.x, pos.y, pos.z), 0.0D, 0.0D);
        }

        return true;
    }
}
