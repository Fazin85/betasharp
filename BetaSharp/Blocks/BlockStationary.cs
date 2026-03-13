using BetaSharp.Blocks.Materials;
using BetaSharp.Worlds.Core;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Blocks;

internal class BlockStationary : BlockFluid
{
    public BlockStationary(int id, Material material) : base(id, material)
    {
        setTickRandomly(false);
        if (material == Material.Lava)
        {
            setTickRandomly(true);
        }
    }

    public override void neighborUpdate(OnTickEvt evt)
    {
        base.neighborUpdate(evt);
        if (evt.Level.Reader.GetBlockId(evt.X, evt.Y, evt.Z) == id)
        {
            int meta = evt.Level.Reader.GetBlockMeta(evt.X, evt.Y, evt.Z);
            evt.Level.BlockWriter.SetBlockWithoutNotifyingNeighbors(evt.X, evt.Y, evt.Z, id - 1, meta, notifyBlockPlaced: false);
            evt.Level.TickScheduler.ScheduleBlockUpdate(evt.X, evt.Y, evt.Z, id - 1, getTickRate());
        }
    }

    private void convertToFlowing(OnTickEvt evt)
    {
        int meta = evt.Level.Reader.GetBlockMeta(evt.X, evt.Y, evt.Z);
        evt.Level.BlockWriter.SetBlockWithoutNotifyingNeighbors(evt.X, evt.Y, evt.Z, id - 1, meta, notifyBlockPlaced: false);
        evt.Level.Broadcaster.SetBlocksDirty(evt.X, evt.Y, evt.Z, evt.X, evt.Y, evt.Z);
        evt.Level.TickScheduler.ScheduleBlockUpdate(evt.X, evt.Y, evt.Z, id - 1, getTickRate());
    }

    public override void onTick(OnTickEvt evt)
    {
        int x = evt.X;
        int y = evt.Y;
        int z = evt.Z;
        if (evt.Level.Reader.GetBlockId(x, y, z) == id)
        {
            convertToFlowing(evt);
        }

        if (material == Material.Lava)
        {
            int attempts = evt.Level.random.NextInt(3);

            for (int attempt = 0; attempt < attempts; ++attempt)
            {
                x += evt.Level.random.NextInt(3) - 1;
                ++y;
                z += evt.Level.random.NextInt(3) - 1;
                int neighborBlockId = evt.Level.Reader.GetBlockId(x, y, z);
                if (neighborBlockId == 0)
                {
                    if (isFlammable(evt.Level.Reader, x - 1, y, z) || isFlammable(evt.Level.Reader, x + 1, y, z) || isFlammable(evt.Level.Reader, x, y, z - 1) ||
                        isFlammable(evt.Level.Reader, x, y, z + 1) || isFlammable(evt.Level.Reader, x, y - 1, z) || isFlammable(evt.Level.Reader, x, y + 1, z))
                    {
                        evt.Level.BlockWriter.SetBlock(x, y, z, Fire.id);
                        return;
                    }
                }
                else if (Blocks[neighborBlockId].material.BlocksMovement)
                {
                    return;
                }
            }
        }
    }

    private bool isFlammable(IBlockReader world, int x, int y, int z) => world.GetMaterial(x, y, z).IsBurnable;
}
