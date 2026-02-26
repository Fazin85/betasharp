using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class TorchRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess,
        in BlockRenderContext context)
    {
        int metadata = world.getBlockMeta(pos.x, pos.y, pos.z);

        float luminance = block.getLuminance(world, pos.x, pos.y, pos.z);
        if (Block.BlocksLightLuminance[block.id] > 0)
        {
            luminance = 1.0F;
        }

        tess.setColorOpaque_F(luminance, luminance, luminance);

        double tiltAmount = 0.4F;
        double horizontalOffset = 0.5D - tiltAmount;
        double verticalOffset = 0.2F;

        if (metadata == 1) // Attached to West wall (pointing East)
        {
            Helper.RenderTorchAtAngle(block, tess, new Vec3D(pos.x - horizontalOffset, pos.y + verticalOffset, pos.z),
                -tiltAmount, 0.0D,context);
        }
        else if (metadata == 2) // Attached to East wall (pointing West)
        {
            Helper.RenderTorchAtAngle(block, tess, new Vec3D(pos.x + horizontalOffset, pos.y + verticalOffset, pos.z),
                tiltAmount, 0.0D,context);
        }
        else if (metadata == 3) // Attached to North wall (pointing South)
        {
            Helper.RenderTorchAtAngle(block, tess, new Vec3D(pos.x, pos.y + verticalOffset, pos.z - horizontalOffset), 0.0D,
                -tiltAmount,context);
        }
        else if (metadata == 4) // Attached to South wall (pointing North)
        {
            Helper.RenderTorchAtAngle(block, tess, new Vec3D(pos.x, pos.y + verticalOffset, pos.z + horizontalOffset), 0.0D,
                tiltAmount,context);
        }
        else // Standing on floor
        {
            Helper.RenderTorchAtAngle(block, tess, new Vec3D(pos.x, pos.y, pos.z), 0.0D, 0.0D,context);
        }

        return true;
    }

    
}
