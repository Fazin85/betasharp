using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Blocks;

internal class BlockCactus : Block
{

    public BlockCactus(int id, int textureId) : base(id, textureId, Material.Cactus)
    {
        setTickRandomly(true);
    }

    public override void onTick(OnTickContext ctx)
    {
        if (ctx.WorldRead.IsAir(ctx.X, ctx.Y + 1, ctx.Z))
        {
            int heightBelow;
            for (heightBelow = 1; ctx.WorldRead.GetBlockId(ctx.X, ctx.Y - heightBelow, ctx.Z) == id; ++heightBelow)
            {
            }

            if (heightBelow < 3)
            {
                int growthStage = ctx.WorldRead.GetBlockMeta(ctx.X, ctx.Y, ctx.Z);
                if (growthStage == 15)
                {
                    ctx.WorldWrite.SetBlock(ctx.X, ctx.Y + 1, ctx.Z, id);
                    ctx.WorldWrite.SetBlockMeta(ctx.X, ctx.Y, ctx.Z, 0);
                }
                else
                {
                    ctx.WorldWrite.SetBlockMeta(ctx.X, ctx.Y, ctx.Z, growthStage + 1);
                }
            }
        }

    }

    public override Box? getCollisionShape(IBlockReader world, int x, int y, int z)
    {
        float edgeInset = 1.0F / 16.0F;
        return new Box((double)((float)x + edgeInset), (double)y, (double)((float)z + edgeInset), (double)((float)(x + 1) - edgeInset), (double)((float)(y + 1) - edgeInset), (double)((float)(z + 1) - edgeInset));
    }

    public override Box getBoundingBox(World world, int x, int y, int z)
    {
        float edgeInset = 1.0F / 16.0F;
        return new Box((double)((float)x + edgeInset), (double)y, (double)((float)z + edgeInset), (double)((float)(x + 1) - edgeInset), (double)(y + 1), (double)((float)(z + 1) - edgeInset));
    }

    public override int getTexture(int side)
    {
        return side == 1 ? textureId - 1 : (side == 0 ? textureId + 1 : textureId);
    }

    public override bool isFullCube()
    {
        return false;
    }

    public override bool isOpaque()
    {
        return false;
    }

    public override BlockRendererType getRenderType()
    {
        return BlockRendererType.Cactus;
    }

    public override bool canPlaceAt(OnPlacedContext ctx)
    {
        return !base.canPlaceAt(ctx) ? false : canGrow(ctx.WorldRead, ctx.X, ctx.Y, ctx.Z);
    }

    public override void neighborUpdate(OnTickContext ctx)
    {
        if (!canGrow(ctx))
        {
            dropStacks(ctx.WorldRead, ctx.X, ctx.Y, ctx.Z, ctx.WorldRead.GetBlockMeta(ctx.X, ctx.Y, ctx.Z));
            ctx.WorldWrite.SetBlock(ctx.X, ctx.Y, ctx.Z, 0);
        }

    }

    public override bool canGrow(OnTickContext ctx)
    {
        return canGrow(ctx.WorldRead, ctx.X, ctx.Y, ctx.Z);
    }

    private static bool canGrow(WorldBlockView world, int x, int y, int z)
    {
        if (world.GetMaterial(x - 1, y, z).IsSolid)
        {
            return false;
        }
        else if (world.GetMaterial(x + 1, y, z).IsSolid)
        {
            return false;
        }
        else if (world.GetMaterial(x, y, z - 1).IsSolid)
        {
            return false;
        }
        else if (world.GetMaterial(x, y, z + 1).IsSolid)
        {
            return false;
        }
        else
        {
            int blockBelowId = world.GetBlockId(x, y - 1, z);
            return blockBelowId == Cactus.id || blockBelowId == Sand.id;
        }
    }

    public override void onEntityCollision(World world, int x, int y, int z, Entity entity)
    {
        entity.damage(null, 1);
    }
}
