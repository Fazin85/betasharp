using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;

namespace BetaSharp.Blocks;

internal class BlockSand : Block
{
    private static readonly ThreadLocal<bool> s_fallInstantly = new(() => false);

    public BlockSand(int id, int textureId) : base(id, textureId, Material.Sand)
    {
    }

    public static bool fallInstantly
    {
        get => s_fallInstantly.Value;
        set => s_fallInstantly.Value = value;
    }

    public override void onPlaced(OnPlacedEvt ctx) => ctx.Level.TickScheduler.ScheduleBlockUpdate(ctx.X, ctx.Y, ctx.Z, id, getTickRate());

    public override void neighborUpdate(OnTickEvt ctx) => ctx.Level.TickScheduler.ScheduleBlockUpdate(ctx.X, ctx.Y, ctx.Z, id, getTickRate());

    public override void onTick(OnTickEvt evt) => processFall(evt);

    private void processFall(OnTickEvt evt)
    {
        // Check the block BELOW the sand (evt has sand position; canFallThrough checks ctx coords)
        if (evt.Y > 0 && canFallThrough(new OnTickEvt(evt.Level, evt.X, evt.Y - 1, evt.Z, 0, evt.BlockId)))
        {
            sbyte checkRadius = 32;
            if (!fallInstantly && evt.Level.BlockHost.IsRegionLoaded(evt.X - checkRadius, evt.Y - checkRadius, evt.Z - checkRadius, evt.X + checkRadius, evt.Y + checkRadius, evt.Z + checkRadius))
            {
                EntityFallingSand fallingSand = new(evt.Level, evt.X + 0.5F, evt.Y + 0.5F, evt.Z + 0.5F, id);
                evt.Level.Entities.SpawnEntity(fallingSand);
            }
            else
            {
                evt.Level.BlockWriter.SetBlock(evt.X, evt.Y, evt.Z, 0);

                while (canFallThrough(evt) && evt.Y > 0)
                {
                    --evt.Y;
                }

                if (evt.Y > 0)
                {
                    evt.Level.BlockWriter.SetBlock(evt.X, evt.Y, evt.Z, id);
                }
            }
        }
    }

    public override int getTickRate() => 3;

    public static bool canFallThrough(OnTickEvt ctx)
    {
        int blockId = ctx.Level.BlocksReader.GetBlockId(ctx.X, ctx.Y, ctx.Z);
        if (blockId == 0)
        {
            return true;
        }

        if (blockId == Fire.id)
        {
            return true;
        }

        Material material = Blocks[blockId].material;
        return material == Material.Water || material == Material.Lava;
    }
}
