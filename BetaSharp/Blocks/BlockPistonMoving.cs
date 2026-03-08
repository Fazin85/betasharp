using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Blocks;

public class BlockPistonMoving : BlockWithEntity
{
    public BlockPistonMoving(int id) : base(id, Material.Piston) => setHardness(-1.0F);

    protected override BlockEntity getBlockEntity() => null;

    public override void onPlaced(World world, int x, int y, int z)
    {
    }

    public override void onBreak(World world, int x, int y, int z)
    {
        BlockEntity var5 = world.getBlockEntity(x, y, z);
        if (var5 != null && var5 is BlockEntityPiston)
        {
            ((BlockEntityPiston)var5).finish();
        }
        else
        {
            base.onBreak(world, x, y, z);
        }
    }

    public override bool canPlaceAt(CanPlaceAtCtx ctx) => false;

    public override BlockRendererType getRenderType() => BlockRendererType.Entity;

    public override bool isOpaque() => false;

    public override bool isFullCube() => false;

    public override bool onUse(OnUseEvt ctx)
    {
        if (!ctx.IsRemote && ctx.WorldRead.GetBlockEntity(ctx.X, ctx.Y, ctx.Z) == null)
        {
            ctx.WorldWrite.SetBlock(ctx.X, ctx.Y, ctx.Z, 0);
            return true;
        }

        return false;
    }

    public override int getDroppedItemId(int blockMeta) => 0;

    public override void dropStacks(OnDropEvt ctx)
    {
        if (!ctx.IsRemote)
        {
            BlockEntityPiston? piston = getPistonBlockEntity(ctx.WorldRead, ctx.X, ctx.Y, ctx.Z);
            if (piston != null)
            {
                // TODO: Implement this
                // Blocks[piston.getPushedBlockId()].dropStacks(ctx.WorldRead, ctx.X, ctx.Y, ctx.Z, piston.getPushedBlockData());
            }
        }
    }

    public override void neighborUpdate(OnTickEvt ctx)
    {
        if (!ctx.IsRemote && ctx.WorldRead.GetBlockEntity(ctx.X, ctx.Y, ctx.Z) == null)
        {
        }
    }

    public static BlockEntity createPistonBlockEntity(int blockId, int blockMeta, int facing, bool extending, bool source) => new BlockEntityPiston(blockId, blockMeta, facing, extending, source);

    public override Box? getCollisionShape(IBlockReader world, int x, int y, int z)
    {
        BlockEntityPiston var5 = getPistonBlockEntity(world, x, y, z);
        if (var5 == null)
        {
            return null;
        }

        float var6 = var5.getProgress(0.0F);
        if (var5.isExtending())
        {
            var6 = 1.0F - var6;
        }

        return getPushedBlockCollisionShape(world, x, y, z, var5.getPushedBlockId(), var6, var5.getFacing());
    }

    public override void updateBoundingBox(IBlockReader iBlockReader, int x, int y, int z)
    {
        BlockEntityPiston var5 = getPistonBlockEntity(iBlockReader, x, y, z);
        if (var5 != null)
        {
            Block var6 = Blocks[var5.getPushedBlockId()];
            if (var6 == null || var6 == this)
            {
                return;
            }

            var6.updateBoundingBox(iBlockReader, x, y, z);
            float var7 = var5.getProgress(0.0F);
            if (var5.isExtending())
            {
                var7 = 1.0F - var7;
            }

            int var8 = var5.getFacing();
            BoundingBox = BoundingBox.Offset(-(double)(PistonConstants.HEAD_OFFSET_X[var8] * var7), -(double)(PistonConstants.HEAD_OFFSET_Y[var8] * var7), -(double)(PistonConstants.HEAD_OFFSET_Z[var8] * var7));
        }
    }

    public Box? getPushedBlockCollisionShape(IBlockReader world, int x, int y, int z, int blockId, float sizeMultiplier, int facing)
    {
        if (blockId != 0 && blockId != id)
        {
            Box? shape = Blocks[blockId].getCollisionShape(world, x, y, z);
            if (shape == null)
            {
                return null;
            }

            Box res = shape.Value;
            res.MinX -= PistonConstants.HEAD_OFFSET_X[facing] * sizeMultiplier;
            res.MaxX -= PistonConstants.HEAD_OFFSET_X[facing] * sizeMultiplier;
            res.MinY -= PistonConstants.HEAD_OFFSET_Y[facing] * sizeMultiplier;
            res.MaxY -= PistonConstants.HEAD_OFFSET_Y[facing] * sizeMultiplier;
            res.MinZ -= PistonConstants.HEAD_OFFSET_Z[facing] * sizeMultiplier;
            res.MaxZ -= PistonConstants.HEAD_OFFSET_Z[facing] * sizeMultiplier;
            return res;
        }

        return null;
    }

    private BlockEntityPiston? getPistonBlockEntity(IBlockReader iBlockReader, int x, int y, int z)
    {
        BlockEntity? piston = iBlockReader.GetBlockEntity(x, y, z);
        return piston != null && piston is BlockEntityPiston ? (BlockEntityPiston)piston : null;
    }
}
