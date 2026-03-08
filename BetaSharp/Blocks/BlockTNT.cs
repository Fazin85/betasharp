using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Blocks;

internal class BlockTNT : Block
{
    public BlockTNT(int id, int textureId) : base(id, textureId, Material.Tnt)
    {
    }

    public override int getTexture(int side)
    {
        return side == 0 ? textureId + 2 : (side == 1 ? textureId + 1 : textureId);
    }

    public override void onPlaced(OnPlacedContext ctx)
    {
        base.onPlaced(ctx);
        if (ctx.Redstone.IsPowered(ctx.X, ctx.Y, ctx.Z))
        {
            onMetadataChange(ctx);
            ctx.WorldWrite.SetBlock(ctx.X, ctx.Y, ctx.Z, 0);
        }

    }

    public override void neighborUpdate(OnTickContext ctx)
    {
        if (ctx.BlockId > 0 && Block.Blocks[ctx.BlockId].canEmitRedstonePower() && ctx.Redstone.IsPowered(ctx.X, ctx.Y, ctx.Z))
        {
            onMetadataChange(ctx);
            ctx.WorldWrite.SetBlock(ctx.X, ctx.Y, ctx.Z, 0);
        }

    }

    public override int getDroppedItemCount(JavaRandom random)
    {
        return 0;
    }

    public override void onDestroyedByExplosion(World world, int x, int y, int z)
    {
        EntityTNTPrimed entityTNTPrimed = new EntityTNTPrimed(world, (double)((float)x + 0.5F), (double)((float)y + 0.5F), (double)((float)z + 0.5F));
        entityTNTPrimed.fuse = world.random.NextInt(entityTNTPrimed.fuse / 4) + entityTNTPrimed.fuse / 8;
        world.SpawnEntity(entityTNTPrimed);
    }

    public override void onMetadataChange(OnTickContext ctx)
    {
        if (!ctx.IsRemote)
        {
            if ((ctx.WorldRead.GetBlockMeta(ctx.X, ctx.Y, ctx.Z) & 1) == 0)
            {
                dropStack(ctx.WorldRead, ctx.X, ctx.Y, ctx.Z, new ItemStack(Block.TNT.id, 1, 0));
            }
            else
            {
                EntityTNTPrimed entityTNTPrimed = new EntityTNTPrimed(ctx.WorldRead, (double)((float)ctx.X + 0.5F), (double)((float)ctx.Y + 0.5F), (double)((float)ctx.Z + 0.5F));
                ctx.Entities.SpawnEntity(entityTNTPrimed);
                ctx.Broadcaster.PlaySoundAtPos(ctx.X + 0.5F, ctx.Y + 0.5F, ctx.Z + 0.5F, "random.fuse", 1.0F, 1.0F);
            }

        }
    }

    public override void onBlockBreakStart(World world, int x, int y, int z, EntityPlayer player)
    {
        if (player.getHand() != null && player.getHand().itemId == Item.FlintAndSteel.id)
        {
            world.SetBlockMetaWithoutNotifyingNeighbors(x, y, z, 1);
        }

        base.onBlockBreakStart(world, x, y, z, player);
    }

    public override bool onUse(World world, int x, int y, int z, EntityPlayer player)
    {
        return base.onUse(world, x, y, z, player);
    }
}
