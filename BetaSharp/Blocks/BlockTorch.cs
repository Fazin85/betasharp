using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Hit;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Blocks;

internal class BlockTorch : Block
{

    public BlockTorch(int id, int textureId) : base(id, textureId, Material.PistonBreakable)
    {
        setTickRandomly(true);
    }

    public override Box? getCollisionShape(IBlockReader world, int x, int y, int z)
    {
        return null;
    }

    public override bool isOpaque()
    {
        return false;
    }

    public override bool isFullCube()
    {
        return false;
    }

    public override BlockRendererType getRenderType()
    {
        return BlockRendererType.Torch;
    }

    private bool canPlaceOn(IBlockReader world, int x, int y, int z)
    {
        return world.ShouldSuffocate(x, y, z) || world.GetBlockId(x, y, z) == Fence.id;
    }

    public override bool canPlaceAt(OnPlacedContext ctx)
    {
        return ctx.WorldRead.ShouldSuffocate(ctx.X - 1, ctx.Y, ctx.Z) ? true : (ctx.WorldRead.ShouldSuffocate(ctx.X + 1, ctx.Y, ctx.Z) ? true : (ctx.WorldRead.ShouldSuffocate(ctx.X, ctx.Y, ctx.Z - 1) ? true : (ctx.WorldRead.ShouldSuffocate(ctx.X, ctx.Y, ctx.Z + 1) ? true : canPlaceOn(ctx.WorldRead, ctx.X, ctx.Y - 1, ctx.Z))));
    }

    public override void onPlaced(OnPlacedContext ctx)
    {
        int meta = ctx.WorldRead.GetBlockMeta(ctx.X, ctx.Y, ctx.Z);
        if (ctx.Direction == 1 && canPlaceOn(ctx.WorldRead, ctx.X, ctx.Y - 1, ctx.Z))
        {
            meta = 5;
        }

        if (ctx.Direction == 2 && ctx.WorldRead.ShouldSuffocate(ctx.X, ctx.Y, ctx.Z + 1))
        {
            meta = 4;
        }

        if (ctx.Direction == 3 && ctx.WorldRead.ShouldSuffocate(ctx.X, ctx.Y, ctx.Z - 1))
        {
            meta = 3;
        }

        if (ctx.Direction == 4 && ctx.WorldRead.ShouldSuffocate(ctx.X + 1, ctx.Y, ctx.Z))
        {
            meta = 2;
        }

        if (ctx.Direction == 5 && ctx.WorldRead.ShouldSuffocate(ctx.X - 1, ctx.Y, ctx.Z))
        {
            meta = 1;
        }

        ctx.WorldWrite.SetBlockMeta(ctx.X, ctx.Y, ctx.Z, meta);
    }

    public override void onTick(OnTickContext ctx)
    {
        base.onTick(ctx);
        if (ctx.WorldRead.GetBlockMeta(ctx.X, ctx.Y, ctx.Z) == 0)
        {
            onPlaced(ctx);
        }
    }

    public override void onPlaced(OnPlacedContext ctx)
    {
        if (ctx.WorldRead.ShouldSuffocate(ctx.X - 1, ctx.Y, ctx.Z))
        {
            ctx.WorldWrite.SetBlockMeta(ctx.X, ctx.Y, ctx.Z, 1);
        }
        else if (ctx.WorldRead.ShouldSuffocate(ctx.X + 1, ctx.Y, ctx.Z))
        {
            ctx.WorldWrite.SetBlockMeta(ctx.X, ctx.Y, ctx.Z, 2);
        }
        else if (ctx.WorldRead.ShouldSuffocate(ctx.X, ctx.Y, ctx.Z - 1))
        {
            ctx.WorldWrite.SetBlockMeta(ctx.X, ctx.Y, ctx.Z, 3);
        }
        else if (ctx.WorldRead.ShouldSuffocate(ctx.X, ctx.Y, ctx.Z + 1))
        {
            ctx.WorldWrite.SetBlockMeta(ctx.X, ctx.Y, ctx.Z, 4);
        }
        else if (canPlaceOn(ctx.WorldRead, ctx.X, ctx.Y - 1, ctx.Z))
        {
            ctx.WorldWrite.SetBlockMeta(ctx.X, ctx.Y, ctx.Z, 5);
        }

        breakIfCannotPlaceAt(ctx.WorldRead, ctx.X, ctx.Y, ctx.Z);
    }

    public override void neighborUpdate(OnTickContext ctx)
    {
        if (breakIfCannotPlaceAt(ctx.WorldRead, ctx.X, ctx.Y, ctx.Z))
        {
            int meta = ctx.WorldRead.GetBlockMeta(ctx.X, ctx.Y, ctx.Z);
            bool canPlace = false;
            if (!ctx.WorldRead.ShouldSuffocate(ctx.X - 1, ctx.Y, ctx.Z) && meta == 1)
            {
                canPlace = true;
            }

            if (!ctx.WorldRead.ShouldSuffocate(ctx.X + 1, ctx.Y, ctx.Z) && meta == 2)
            {
                canPlace = true;
            }

            if (!ctx.WorldRead.ShouldSuffocate(ctx.X, ctx.Y, ctx.Z - 1) && meta == 3)
            {
                canPlace = true;
            }

            if (!ctx.WorldRead.ShouldSuffocate(ctx.X, ctx.Y, ctx.Z + 1) && meta == 4)
            {
                canPlace = true;
            }

            if (!canPlaceOn(ctx.WorldRead, ctx.X, ctx.Y - 1, ctx.Z) && meta == 5)
            {
                canPlace = true;
            }

            if (canPlace)
            {
                dropStacks(ctx.WorldRead, ctx.X, ctx.Y, ctx.Z, ctx.WorldRead.GetBlockMeta(ctx.X, ctx.Y, ctx.Z));
                ctx.WorldWrite.SetBlock(ctx.X, ctx.Y, ctx.Z, 0);
            }
        }

    }

    private bool breakIfCannotPlaceAt(IBlockReader world, int x, int y, int z)
    {
        if (!canPlaceAt(world, x, y, z))
        {
            dropStacks(ctx.WorldView, ctx.X, ctx.Y, ctx.Z, ctx.WorldView.GetBlockMeta(ctx.X, ctx.Y, ctx.Z));
            ctx.WorldWrite.SetBlock(ctx.X, ctx.Y, ctx.Z, 0);
            return false;
        }
        else
        {
            return true;
        }
    }

    public override HitResult raycast(IBlockReader world, int x, int y, int z, Vec3D startPos, Vec3D endPos)
    {
        int meta = world.GetBlockMeta(x, y, z) & 7;
        float torchWidth = 0.15F;
        if (meta == 1)
        {
            setBoundingBox(0.0F, 0.2F, 0.5F - torchWidth, torchWidth * 2.0F, 0.8F, 0.5F + torchWidth);
        }
        else if (meta == 2)
        {
            setBoundingBox(1.0F - torchWidth * 2.0F, 0.2F, 0.5F - torchWidth, 1.0F, 0.8F, 0.5F + torchWidth);
        }
        else if (meta == 3)
        {
            setBoundingBox(0.5F - torchWidth, 0.2F, 0.0F, 0.5F + torchWidth, 0.8F, torchWidth * 2.0F);
        }
        else if (meta == 4)
        {
            setBoundingBox(0.5F - torchWidth, 0.2F, 1.0F - torchWidth * 2.0F, 0.5F + torchWidth, 0.8F, 1.0F);
        }
        else
        {
            torchWidth = 0.1F;
            setBoundingBox(0.5F - torchWidth, 0.0F, 0.5F - torchWidth, 0.5F + torchWidth, 0.6F, 0.5F + torchWidth);
        }

        return base.raycast(world, x, y, z, startPos, endPos);
    }

    public override void randomDisplayTick(OnTickContext ctx)
    {
        int meta = ctx.WorldRead.GetBlockMeta(ctx.X, ctx.Y, ctx.Z);
        float flameX = ctx.X + 0.5F;
        float flameY = ctx.Y + 0.7F;
        float flameZ = ctx.Z + 0.5F;
        float yOffset = 0.22F;
        float xOffset = 0.27F;
        if (meta == 1)
        {
            ctx.Broadcaster.AddParticle("smoke", flameX - xOffset, flameY + yOffset, flameZ, 0.0D, 0.0D, 0.0D);
            ctx.Broadcaster.AddParticle("flame", flameX - xOffset, flameY + yOffset, flameZ, 0.0D, 0.0D, 0.0D);
        }
        else if (meta == 2)
        {
            ctx.Broadcaster.AddParticle("smoke", flameX + xOffset, flameY + yOffset, flameZ, 0.0D, 0.0D, 0.0D);
            ctx.Broadcaster.AddParticle("flame", flameX + xOffset, flameY + yOffset, flameZ, 0.0D, 0.0D, 0.0D);
        }
        else if (meta == 3)
        {
            ctx.Broadcaster.AddParticle("smoke", flameX, flameY + yOffset, flameZ - xOffset, 0.0D, 0.0D, 0.0D);
            ctx.Broadcaster.AddParticle("flame", flameX, flameY + yOffset, flameZ - xOffset, 0.0D, 0.0D, 0.0D);
        }
        else if (meta == 4)
        {
            ctx.Broadcaster.AddParticle("smoke", flameX, flameY + yOffset, flameZ + xOffset, 0.0D, 0.0D, 0.0D);
            ctx.Broadcaster.AddParticle("flame", flameX, flameY + yOffset, flameZ + xOffset, 0.0D, 0.0D, 0.0D);
        }
        else
        {
            ctx.Broadcaster.AddParticle("smoke", flameX, flameY, flameZ, 0.0D, 0.0D, 0.0D);
            ctx.Broadcaster.AddParticle("flame", flameX, flameY, flameZ, 0.0D, 0.0D, 0.0D);
        }

    }
}
