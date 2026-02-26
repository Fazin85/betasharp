using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Blocks.Renderers;

public class PistonBaseRenderer : IBlockRenderer
{
    public bool Render(IBlockAccess world, Block block, in BlockPos pos, Tessellator tess,
        in BlockRenderContext context)
    {
        int metadata = world.getBlockMeta(pos.x, pos.y, pos.z);

        // CustomFlag acts as our 'expanded' override from the BlockEntity animation
        bool isExpanded = context.CustomFlag || (metadata & 8) != 0;
        int facing = BlockPistonBase.getFacing(metadata);

        int uvTop = 0, uvBottom = 0, uvNorth = 0, uvSouth = 0, uvEast = 0, uvWest = 0;
        Box? bounds = context.OverrideBounds ?? block.BoundingBox;

        if (isExpanded)
        {
            // Shrink the base block's bounding box to make room for the extended arm
            switch (facing)
            {
                case 0: // Down
                    uvEast = 3;
                    uvWest = 3;
                    uvSouth = 3;
                    uvNorth = 3;
                    bounds = new Box(0.0F, 0.25F, 0.0F, 1.0F, 1.0F, 1.0F);
                    break;
                case 1: // Up
                    bounds = new Box(0.0F, 0.0F, 0.0F, 1.0F, 0.75F, 1.0F);
                    break;
                case 2: // North
                    uvSouth = 1;
                    uvNorth = 2;
                    bounds = new Box(0.0F, 0.0F, 0.25F, 1.0F, 1.0F, 1.0F);
                    break;
                case 3: // South
                    uvSouth = 2;
                    uvNorth = 1;
                    uvTop = 3;
                    uvBottom = 3;
                    bounds = new Box(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.75F);
                    break;
                case 4: // West
                    uvEast = 1;
                    uvWest = 2;
                    uvTop = 2;
                    uvBottom = 1;
                    bounds = new Box(0.25F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                    break;
                case 5: // East
                    uvEast = 2;
                    uvWest = 1;
                    uvTop = 1;
                    uvBottom = 2;
                    bounds = new Box(0.0F, 0.0F, 0.0F, 0.75F, 1.0F, 1.0F);
                    break;
            }
        }
        else
        {
            // Piston is retracted (full block), but we still apply UV rotation
            // so the wooden "face" points in the correct direction.
            switch (facing)
            {
                case 0: // Down
                    uvEast = 3;
                    uvWest = 3;
                    uvSouth = 3;
                    uvNorth = 3;
                    break;
                case 2: // North
                    uvSouth = 1;
                    uvNorth = 2;
                    break;
                case 3: // South
                    uvSouth = 2;
                    uvNorth = 1;
                    uvTop = 3;
                    uvBottom = 3;
                    break;
                case 4: // West
                    uvEast = 1;
                    uvWest = 2;
                    uvTop = 2;
                    uvBottom = 1;
                    break;
                case 5: // East
                    uvEast = 2;
                    uvWest = 1;
                    uvTop = 1;
                    uvBottom = 2;
                    break;
            }
        }

        // Clone the context, applying our specific rotations and bounds
        var baseCtx = context with
        {
            OverrideBounds = bounds,
            UvRotateTop = uvTop,
            UvRotateBottom = uvBottom,
            UvRotateNorth = uvNorth,
            UvRotateSouth = uvSouth,
            UvRotateEast = uvEast,
            UvRotateWest = uvWest
        };

        return baseCtx.RenderStandardBlock(block, pos, world, tess);
    }
}
