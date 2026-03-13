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
        int x = evt.X;
        int y = evt.Y;
        int z = evt.Z;
        if (y > 0 && canFallThrough(new OnTickEvt(evt.Level, x, y - 1, z, 0, evt.BlockId)))
        {
            sbyte checkRadius = 32;
            if (!fallInstantly && evt.Level.BlockHost.IsRegionLoaded(x - checkRadius, y - checkRadius, z - checkRadius, x + checkRadius, y + checkRadius, z + checkRadius))
            {
                EntityFallingSand fallingSand = new(evt.Level, x + 0.5F, y + 0.5F, z + 0.5F, id);
                evt.Level.Entities.SpawnEntity(fallingSand);
            }
            else
            {
                evt.Level.BlockWriter.SetBlock(x, y, z, 0);

                while (canFallThrough(evt) && evt.Y > 0)
                {
                    --y;
                }

                if (y > 0)
                {
                    evt.Level.BlockWriter.SetBlock(x, y, z, id);
                }
            }
        }
    }

    public override int getTickRate() => 3;

    public static bool canFallThrough(OnTickEvt ctx)
    {
        int blockId = ctx.Level.Reader.GetBlockId(ctx.X, ctx.Y, ctx.Z);
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
