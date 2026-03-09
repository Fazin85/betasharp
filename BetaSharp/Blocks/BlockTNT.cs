using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Items;

namespace BetaSharp.Blocks;

internal class BlockTNT : Block
{
    public BlockTNT(int id, int textureId) : base(id, textureId, Material.Tnt)
    {
    }

    public override int getTexture(int side) => side == 0 ? textureId + 2 : side == 1 ? textureId + 1 : textureId;

    public override void onPlaced(OnPlacedEvt evt)
    {
        base.onPlaced(evt);
        if (evt.Level.Redstone.IsPowered(evt.X, evt.Y, evt.Z))
        {
            onMetadataChange(new OnMetadataChangeEvt(evt.Level, evt.X, evt.Y, evt.Z, 1));
            evt.Level.BlockWriter.SetBlock(evt.X, evt.Y, evt.Z, 0);
        }
    }

    public override void neighborUpdate(OnTickEvt evt)
    {
        if (evt.BlockId > 0 && Blocks[evt.BlockId].canEmitRedstonePower() && evt.Level.Redstone.IsPowered(evt.X, evt.Y, evt.Z))
        {
            onMetadataChange(new OnMetadataChangeEvt(evt.Level, evt.X, evt.Y, evt.Z, 1));
            evt.Level.BlockWriter.SetBlock(evt.X, evt.Y, evt.Z, 0);
        }
    }

    public override int getDroppedItemCount() => 0;

    public override void onDestroyedByExplosion(OnDestroyedByExplosionEvt evt)
    {
        EntityTNTPrimed entityTNTPrimed = new(evt.Level, evt.X + 0.5F, evt.Y + 0.5F, evt.Z + 0.5F);
        entityTNTPrimed.fuse = evt.Level.random.NextInt(entityTNTPrimed.fuse / 4) + entityTNTPrimed.fuse / 8;
        evt.Level.Entities.SpawnEntity(entityTNTPrimed);
    }

    public override void onMetadataChange(OnMetadataChangeEvt evt)
    {
        if (!evt.Level.IsRemote)
        {
            if ((evt.Meta & 1) == 0)
            {
                dropStack(evt.Level, evt.X, evt.Y, evt.Z, new ItemStack(TNT.id, 1, 0));
            }
            else
            {
                EntityTNTPrimed entityTNTPrimed = new EntityTNTPrimed(evt.Level, evt.X + 0.5F, evt.Y + 0.5F, evt.Z + 0.5F);
                evt.Level.Entities.SpawnEntity(entityTNTPrimed);
                evt.Level.Broadcaster.PlaySoundAtPos(evt.X + 0.5F, evt.Y + 0.5F, evt.Z + 0.5F, "random.fuse", 1.0F, 1.0F);
            }
        }
    }

    public override void onBlockBreakStart(OnBlockBreakStartEvt ctx)
    {
        if (ctx.Player.getHand() != null && ctx.Player.getHand().itemId == Item.FlintAndSteel.id)
        {
            ctx.Level.BlockWriter.SetBlockMetaWithoutNotifyingNeighbors(ctx.X, ctx.Y, ctx.Z, 1);
        }

        base.onBlockBreakStart(ctx);
    }

    public override bool onUse(OnUseEvt ctx) => base.onUse(ctx);
}