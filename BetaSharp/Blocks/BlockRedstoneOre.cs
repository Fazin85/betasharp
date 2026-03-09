using BetaSharp.Blocks.Materials;
using BetaSharp.Items;
using BetaSharp.Worlds.Core;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Blocks;

internal class BlockRedstoneOre : Block
{
    private readonly bool lit;

    public BlockRedstoneOre(int id, int textureId, bool lit) : base(id, textureId, Material.Stone)
    {
        if (lit)
        {
            setTickRandomly(true);
        }

        this.lit = lit;
    }

    public override int getTickRate() => 30;

    public override void onBlockBreakStart(OnBlockBreakStartEvt evt)
    {
        light(evt.Level.BlockWriter, evt.Level.BlocksReader, evt.Level.Broadcaster, evt.X, evt.Y, evt.Z);
        base.onBlockBreakStart(evt);
    }

    public override void onSteppedOn(OnEntityStepEvt evt)
    {
        light(evt.Level.BlockWriter, evt.Level.BlocksReader, evt.Level.Broadcaster, evt.X, evt.Y, evt.Z);
        base.onSteppedOn(evt);
    }

    public override bool onUse(OnUseEvt evt)
    {
        light(evt.Level.BlockWriter, evt.Level.BlocksReader, evt.Level.Broadcaster, evt.X, evt.Y, evt.Z);
        return base.onUse(evt);
    }

    private void light(IBlockWrite worldWrite, IBlockReader worldRead, WorldEventBroadcaster broadcaster, int x, int y, int z)
    {
        spawnParticles(worldRead, broadcaster, x, y, z);
        if (worldRead.GetBlockId(x, y, z) == RedstoneOre.id)
        {
            worldWrite.SetBlock(x, y, z, LitRedstoneOre.id);
        }
    }

    public override void onTick(OnTickEvt evt)
    {
        if (id == LitRedstoneOre.id)
        {
            evt.Level.BlockWriter.SetBlock(evt.X, evt.Y, evt.Z, RedstoneOre.id);
        }
    }

    public override int getDroppedItemId(int blockMeta) => Item.Redstone.id;

    public override int getDroppedItemCount() => 4 + Random.Shared.Next(2);

    public override void randomDisplayTick(OnTickEvt ctx)
    {
        if (lit)
        {
            spawnParticles(ctx.Level.BlocksReader, ctx.Level.Broadcaster, ctx.X, ctx.Y, ctx.Z);
        }
    }

    private void spawnParticles(IBlockReader reader, WorldEventBroadcaster broadcaster, int x, int y, int z)
    {
        double faceOffset = 1.0D / 16.0D;
        for (int direction = 0; direction < 6; ++direction)
        {
            double particleX = x + Random.Shared.NextSingle();
            double particleY = y + Random.Shared.NextSingle();
            double particleZ = z + Random.Shared.NextSingle();
            if (direction == 0 && !reader.IsOpaque(x, y + 1, z))
            {
                particleY = y + 1 + faceOffset;
            }

            if (direction == 1 && !reader.IsOpaque(x, y - 1, z))
            {
                particleY = y + 0 - faceOffset;
            }

            if (direction == 2 && !reader.IsOpaque(x, y, z + 1))
            {
                particleZ = z + 1 + faceOffset;
            }

            if (direction == 3 && !reader.IsOpaque(x, y, z - 1))
            {
                particleZ = z + 0 - faceOffset;
            }

            if (direction == 4 && !reader.IsOpaque(x + 1, y, z))
            {
                particleX = x + 1 + faceOffset;
            }

            if (direction == 5 && !reader.IsOpaque(x - 1, y, z))
            {
                particleX = x + 0 - faceOffset;
            }

            if (particleX < x || particleX > x + 1 || particleY < 0.0D || particleY > y + 1 || particleZ < z || particleZ > z + 1)
            {
                broadcaster.AddParticle("reddust", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);
            }
        }
    }
}
